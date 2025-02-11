using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChallengeLeaderboardDataCache : MonoBehaviour
{
	public class UpdatedEventArgs : ChallengeEventArgs
	{
		public CachedLeaderboardData CacheData { get; private set; }

		public UpdatedEventArgs(ChallengeData challengeData, CachedLeaderboardData cachedData)
			: base(challengeData)
		{
			CacheData = cachedData;
		}
	}

	public class CachedLeaderboardData
	{
		public ChallengeFriendData FriendData { get; private set; }

		public uint LastSyncTime { get; private set; }

		public CachedLeaderboardData(uint lastSyncTime, ChallengeFriendData friendData)
		{
			LastSyncTime = lastSyncTime;
			FriendData = friendData;
		}

		public override string ToString()
		{
			return string.Format("[CachedLeaderboardData: FriendData={0}, LastSyncTime={1}]", FriendData, LastSyncTime);
		}
	}

	private class NetworkTaskRetryCounter
	{
		public int Attempts { get; set; }
	}

	private const int MaxRetries = 10;

	private const uint SecondsBeforeCacheIsInvalid = 300u;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ChallengeLeaderboardDataCache), LogLevel.Warning);

	private Dictionary<uint, NetworkTaskRetryCounter> _pendingQueryIds = new Dictionary<uint, NetworkTaskRetryCounter>();

	private Dictionary<uint, CachedLeaderboardData> _cache = new Dictionary<uint, CachedLeaderboardData>();

	private ChallengeLeaderboardProvider LeaderboardProvider
	{
		get
		{
			return ChallengeManager.Instance.LeaderboardProvider;
		}
	}

	public static event EventHandler<ChallengeEventArgs> Invalidated;

	public static event EventHandler<UpdatedEventArgs> Updated;

	private void Start()
	{
		StartCoroutine(TimerCoroutine());
	}

	public void UpdateCache(ChallengeData data, bool invalidateOldData)
	{
		_log.LogDebug("TryUpdateCachedData " + data.Id);
		if (invalidateOldData)
		{
			_pendingQueryIds.Remove(data.Id);
			_cache.Remove(data.Id);
			OnInvalidated(data);
		}
		else
		{
			uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
			if (synchronizedTime.HasValue)
			{
				CachedLeaderboardData data2 = GetData(data);
				if (data2 != null)
				{
					uint num = synchronizedTime.Value - data2.LastSyncTime;
					if (num < 300)
					{
						_log.LogDebug("Cache is only {0} seconds old. Good enough.", num);
						return;
					}
				}
			}
		}
		if (!_pendingQueryIds.ContainsKey(data.Id))
		{
			_log.LogDebug("Enqueued request for challenge leaderboard data.");
			_pendingQueryIds.Add(data.Id, new NetworkTaskRetryCounter());
		}
		else
		{
			_log.Log("Already pending update for challenge {0}. Resetting attempt count.", data.Id);
			_pendingQueryIds[data.Id].Attempts = 0;
		}
	}

	private IEnumerator TimerCoroutine()
	{
		while (true)
		{
			Bedrock.brUserConnectionStatus connectionStatus = ChallengeManager.ConnectionStatus;
			if (!connectionStatus.IsOnline())
			{
				_log.LogDebug("Skipping update - Connection status is {0}, not online enough for us.", connectionStatus);
			}
			else if (_pendingQueryIds.Any())
			{
				_log.LogDebug("There are {0} pending queries.", _pendingQueryIds.Count);
				uint? currentTime = SynchronizedClock.Instance.SynchronizedTime;
				if (currentTime.HasValue)
				{
					_log.LogDebug("Starting update of pending query status updates.");
					uint[] challengeIds = _pendingQueryIds.Keys.ToArray();
					ChallengeData[] challengeDatas = new ChallengeData[challengeIds.Length];
					ChallengeDataProvider dataProvider = ChallengeManager.Instance.DataProvider;
					for (int i = 0; i < challengeIds.Length; i++)
					{
						challengeDatas[i] = dataProvider.GetChallengeData(challengeIds[i]);
					}
					BedrockTask[] friendTasks = new BedrockTask[challengeIds.Length];
					for (int j = 0; j < challengeIds.Length; j++)
					{
						ChallengeData challengeData = challengeDatas[j];
						_cache.Remove(challengeData.Id);
						OnInvalidated(challengeData);
						_log.LogDebug("Starting query for challenge {0}", challengeData.Id);
						friendTasks[j] = LeaderboardProvider.StartGetFriendsAboveAndBelow(challengeData.LeaderboardId);
					}
					_log.LogDebug("Waiting for task to complete...");
					yield return StartCoroutine(BedrockTask.WaitForAllTasksToCompleteOrTimeoutCoroutine(friendTasks));
					_log.LogDebug("... Done waiting. Processing {0} task results.", challengeIds.Length);
					for (int k = 0; k < challengeIds.Length; k++)
					{
						ChallengeData challengeData2 = challengeDatas[k];
						BedrockTask friendTask = friendTasks[k];
						CachedLeaderboardData cacheData = null;
						ChallengeFriendData friendData;
						bool success = LeaderboardProvider.EndGetFriendsAboveAndBelow(friendTask, out friendData, challengeData2.LeaderboardType, challengeData2.LeaderboardId);
						if (friendTask != null)
						{
							friendTask.Dispose();
						}
						if (success)
						{
							_log.LogDebug("Got result for challenge {0}: {1}", challengeData2.Id, friendData);
							cacheData = new CachedLeaderboardData(currentTime.Value, friendData);
							_cache[challengeData2.Id] = cacheData;
							_pendingQueryIds.Remove(challengeData2.Id);
							_log.LogDebug("Raising updated event for challenge {0}", challengeData2.Id);
							OnUpdated(challengeData2, cacheData);
							continue;
						}
						NetworkTaskRetryCounter retryCounter = _pendingQueryIds[challengeData2.Id];
						_log.LogError("Failed to get friend data for challenge '{0}' (Attempt {1}, BedrockTask: {2}).", challengeData2.Name, retryCounter.Attempts, friendTask);
						retryCounter.Attempts++;
						if (retryCounter.Attempts >= 10)
						{
							_log.LogError("Failed to get friend data after {0} attemps. Aborting update.", 10);
							_pendingQueryIds.Remove(challengeData2.Id);
						}
					}
					_log.LogDebug("Finished queries.");
				}
				else
				{
					_log.LogWarning("Unable to get synchronized time. Not doing any leaderboard queries.");
				}
			}
			yield return StartCoroutine(CoroutineUtils.WaitForWallTime(1f));
		}
	}

	public CachedLeaderboardData GetData(ChallengeData challengeData)
	{
		CachedLeaderboardData value;
		if (_cache.TryGetValue(challengeData.Id, out value))
		{
			return value;
		}
		return null;
	}

	public bool IsQueryPending(ChallengeData data)
	{
		return _pendingQueryIds.ContainsKey(data.Id);
	}

	private void OnInvalidated(ChallengeData challengeData)
	{
		if (ChallengeLeaderboardDataCache.Invalidated != null)
		{
			ChallengeLeaderboardDataCache.Invalidated(this, new ChallengeEventArgs(challengeData));
		}
	}

	private void OnUpdated(ChallengeData challengeData, CachedLeaderboardData cacheData)
	{
		if (ChallengeLeaderboardDataCache.Updated != null)
		{
			Debug.Log("ONUPDATED: Challenge leaderboard Id " + challengeData.LeaderboardId + " cached " + cacheData.FriendData.LeaderboardID);
			ChallengeLeaderboardDataCache.Updated(this, new UpdatedEventArgs(challengeData, cacheData));
		}
	}
}
