using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MockChallengeLeaderboardProvider : ChallengeLeaderboardProvider
{
	private const string CsvResourceName = "PlaceholderLeaderboards";

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(MockChallengeLeaderboardProvider), LogLevel.Debug);

	private Dictionary<uint, List<ChallengeLeaderboardRow>> _leaderboardData = new Dictionary<uint, List<ChallengeLeaderboardRow>>();

	private Dictionary<uint, Comparison<ChallengeLeaderboardRow>> _sortComparisons = new Dictionary<uint, Comparison<ChallengeLeaderboardRow>>();

	private void Awake()
	{
		uint[] array = new uint[6] { 3u, 4u, 6u, 9u, 10u, 19u };
		foreach (uint key in array)
		{
			_sortComparisons.Add(key, OrderByScoreAscending);
		}
		LoadDataFromCsv();
	}

	private void LoadDataFromCsv()
	{
		_leaderboardData.Clear();
		List<List<string>> source = CsvUtilities.LoadCsvDataFromResource("PlaceholderLeaderboards");
		foreach (List<string> item2 in source.Skip(1))
		{
			ChallengeLeaderboardRow item = new ChallengeLeaderboardRow(item2);
			uint key = uint.Parse(item2[0]);
			List<ChallengeLeaderboardRow> value;
			if (!_leaderboardData.TryGetValue(key, out value))
			{
				value = new List<ChallengeLeaderboardRow>();
				_leaderboardData.Add(key, value);
			}
			value.Add(item);
		}
		foreach (KeyValuePair<uint, List<ChallengeLeaderboardRow>> leaderboardDatum in _leaderboardData)
		{
			SortAndAssignRanks(leaderboardDatum.Key);
		}
		foreach (KeyValuePair<uint, List<ChallengeLeaderboardRow>> leaderboardDatum2 in _leaderboardData)
		{
			foreach (ChallengeLeaderboardRow item3 in leaderboardDatum2.Value)
			{
				_log.LogDebug("Mock Leaderboard Data : {0} -> {1}", leaderboardDatum2.Key, item3);
			}
		}
	}

	private bool TryGetLeaderboard(uint leaderboardId, out List<ChallengeLeaderboardRow> data)
	{
		bool flag = _leaderboardData.TryGetValue(leaderboardId, out data);
		if (!flag)
		{
			_log.LogWarning("Unable to get leaderboard '" + leaderboardId + "' from mock data. Is there an entry for it? Generating an empty one.");
			data = new List<ChallengeLeaderboardRow>();
			_leaderboardData.Add(leaderboardId, data);
		}
		return flag;
	}

	private void SortAndAssignRanks(uint leaderboardId)
	{
		List<ChallengeLeaderboardRow> data;
		if (TryGetLeaderboard(leaderboardId, out data))
		{
			Comparison<ChallengeLeaderboardRow> value;
			if (!_sortComparisons.TryGetValue(leaderboardId, out value))
			{
				value = OrderByScoreDescending;
			}
			SortAndAssignRanks(data, value);
		}
	}

	private void SortAndAssignRanks(List<ChallengeLeaderboardRow> rows, Comparison<ChallengeLeaderboardRow> sortComparison)
	{
		rows.Sort(sortComparison);
		for (int i = 0; i < rows.Count; i++)
		{
			ChallengeLeaderboardRow challengeLeaderboardRow = rows[i];
			challengeLeaderboardRow.Rank = (ulong)i;
		}
	}

	public int OrderByScoreAscending(ChallengeLeaderboardRow x, ChallengeLeaderboardRow y)
	{
		return x.Score.CompareTo(y.Score);
	}

	public int OrderByScoreDescending(ChallengeLeaderboardRow x, ChallengeLeaderboardRow y)
	{
		return y.Score.CompareTo(x.Score);
	}

	public override BedrockTask SubmitScore(uint leaderboardId, ChallengeLeaderboardWriteTypes writeType, long score)
	{
		ulong userId = ChallengeManager.UserId;
		string userName = ChallengeManager.UserName;
		_log.Log("Player submitted score {0} for leaderboard {1} with writeType {2}", score, leaderboardId, writeType);
		List<ChallengeLeaderboardRow> data;
		if (TryGetLeaderboard(leaderboardId, out data))
		{
			ChallengeLeaderboardRow challengeLeaderboardRow = null;
			foreach (ChallengeLeaderboardRow item2 in data)
			{
				if (item2.UserId == userId)
				{
					challengeLeaderboardRow = item2;
					break;
				}
			}
			if (challengeLeaderboardRow == null)
			{
				_log.LogDebug(string.Format("Inserting new score {0} in leaderboard {1} for user {2} ({3})", score, leaderboardId, userId, userName));
				int xPLevel = XPManager.Instance.GetXPLevel();
				ChallengeLeaderboardRow item = new ChallengeLeaderboardRow(userId, userName, score, xPLevel, SecureStorage.Instance.EliteAccountLinked);
				data.Add(item);
			}
			else
			{
				_log.LogDebug(string.Concat("Leaderboard ", leaderboardId, ": Attempting to replace ", challengeLeaderboardRow.Score, " with ", score, " (write type ", writeType, ")"));
				switch (writeType)
				{
				case ChallengeLeaderboardWriteTypes.UseMax:
					if (score > challengeLeaderboardRow.Score)
					{
						_log.LogDebug("Updated MAX score -> " + score);
						challengeLeaderboardRow.SetScore(score);
					}
					else
					{
						_log.LogDebug("Score is not > existing value. No update.");
					}
					break;
				case ChallengeLeaderboardWriteTypes.UseMin:
					if (score < challengeLeaderboardRow.Score)
					{
						_log.LogDebug("Updated MIN score -> " + score);
						challengeLeaderboardRow.SetScore(score);
					}
					else
					{
						_log.LogDebug("Score is not < existing score. No update.");
					}
					break;
				default:
					_log.LogError("Unsupported write-type: " + writeType);
					break;
				}
			}
			SortAndAssignRanks(leaderboardId);
		}
		return new MockBedrockTask();
	}

	public override BedrockTask StartGetFriendsAboveAndBelow(uint leaderboardId)
	{
		_log.LogDebug("StartGetFriendsAboveAndBelow ({0})", leaderboardId);
		MockBedrockTask mockBedrockTask = new MockBedrockTask();
		List<ChallengeLeaderboardRow> data;
		if (TryGetLeaderboard(leaderboardId, out data))
		{
			mockBedrockTask.Result = data.ToArray();
			_log.LogDebug("Generated Task: {0}", mockBedrockTask);
		}
		mockBedrockTask.Result = new ChallengeLeaderboardRow[0];
		return mockBedrockTask;
	}

	protected override ChallengeLeaderboardRow[] EndGetFriendsAboveAndBelow_Internal(BedrockTask task, ChallengeLeaderboardType leaderboardType, uint leaderboardId)
	{
		_log.LogDebug("EndGetFriendsAboveAndBelow: ", task);
		MockBedrockTask mockBedrockTask = (MockBedrockTask)task;
		if (task.Status == Bedrock.brTaskStatus.BR_TASK_SUCCESS)
		{
			return (ChallengeLeaderboardRow[])mockBedrockTask.Result;
		}
		return null;
	}

	public void ClearLeaderboard(uint leaderboardId)
	{
		List<ChallengeLeaderboardRow> data;
		if (TryGetLeaderboard(leaderboardId, out data))
		{
			data.Clear();
		}
		else
		{
			Debug.LogWarning("Trying to clear leaderboard (" + leaderboardId + ") that doesn't exist!");
		}
	}

	public override bool GetFriendsForCurrentPlayer(out FriendData[] friends)
	{
		friends = new FriendData[7]
		{
			new FriendData(0uL, "Abe"),
			new FriendData(10uL, "Karl"),
			new FriendData(11uL, "Lenny"),
			new FriendData(12uL, "Marry"),
			new FriendData(13uL, "Nancy"),
			new FriendData(14uL, "Olga"),
			new FriendData(15uL, "Peter")
		};
		return true;
	}
}
