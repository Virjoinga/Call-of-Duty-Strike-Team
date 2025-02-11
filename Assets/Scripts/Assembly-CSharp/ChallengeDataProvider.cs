using System.Collections.Generic;
using UnityEngine;

public abstract class ChallengeDataProvider : MonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(ChallengeDataProvider), LogLevel.None);

	private List<ChallengeData> _cache;

	protected abstract uint NumberOfChallenges_Internal { get; }

	public uint NumberOfChallenges
	{
		get
		{
			_log.LogDebug("Getting NumberOfChallenges");
			if (_cache == null)
			{
				_log.LogDebug("No challenges cached. Getting total count from Implementation.");
				return NumberOfChallenges_Internal;
			}
			return (uint)_cache.Count;
		}
	}

	public IEnumerable<ChallengeData> AllChallenges
	{
		get
		{
			EnsureCacheIsValid();
			return _cache.ToArray();
		}
	}

	protected abstract ChallengeData GetChallengeData_Internal(uint challengeId);

	public abstract BedrockTask BeginUpdateAllChallenges();

	public abstract bool EndUpdateAllChallenges(BedrockTask task);

	public abstract BedrockTask BeginGetServerTime();

	public abstract bool EndGetServerTime(BedrockTask task, out uint currentTime);

	public abstract BedrockTask BeginGetChallengeStatus(uint challengeId);

	public abstract bool EndGetChallengeStatus(BedrockTask task, out ChallengeStatus status);

	public void InvalidateCache()
	{
		_log.LogDebug("Invalidating Cache");
		if (_cache != null)
		{
			foreach (ChallengeData item in _cache)
			{
				Object.Destroy(item);
			}
			_cache.Clear();
			_cache = null;
		}
		else
		{
			_log.LogDebug("Cache already invalid");
		}
	}

	private void EnsureCacheIsValid()
	{
		if (_cache == null || _cache.Count == 0)
		{
			_log.LogDebug("Cached data does not appear to be valid. Refreshing cache.");
			RegenerateCache();
		}
	}

	private void RegenerateCache()
	{
		_log.LogDebug("RegenerateCacheData");
		InvalidateCache();
		uint numberOfChallenges = NumberOfChallenges;
		_cache = new List<ChallengeData>();
		_log.LogDebug("No challenges cached. Refilling cache from implemenatation.");
		for (uint num = 0u; num < numberOfChallenges; num++)
		{
			ChallengeData challengeData_Internal = GetChallengeData_Internal(num);
			challengeData_Internal.transform.parent = base.transform;
			challengeData_Internal.name = challengeData_Internal.Name;
			_cache.Add(challengeData_Internal);
		}
		_log.LogDebug("Finished getting data for {0} challenges from implementation.", numberOfChallenges);
	}

	public ChallengeData GetChallengeData(uint challengeId)
	{
		_log.LogDebug("Getting challenge data for {0}", challengeId);
		EnsureCacheIsValid();
		if (_cache == null)
		{
			_log.LogError("No challenge data cached yet! Returning null.");
			return null;
		}
		if (challengeId >= _cache.Count)
		{
			_log.LogError("Asking for challenge info {0} that does not exist!", challengeId);
			return null;
		}
		ChallengeData challengeData = _cache[(int)challengeId];
		if (challengeData == null)
		{
			_log.LogError("Cached data for challenge {0} appears to be null.", challengeId);
		}
		return challengeData;
	}
}
