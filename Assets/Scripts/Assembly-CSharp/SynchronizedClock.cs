using System;
using System.Collections;
using UnityEngine;

public class SynchronizedClock : SingletonMonoBehaviour
{
	private const float WaitForSynchronizeTimeout = 10f;

	private static uint DebugAdjust = 0u;

	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(SynchronizedClock), LogLevel.Warning);

	private int? _clockDelta;

	private bool _isSynchronizing;

	public static SynchronizedClock Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<SynchronizedClock>();
		}
	}

	public uint? SynchronizedTime
	{
		get
		{
			if (!_clockDelta.HasValue)
			{
				_log.LogDebug("Someone asked for time, but it wasn't valid yet.");
				return null;
			}
			return Convert.ToUInt32(TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow) + _clockDelta.Value + DebugAdjust);
		}
	}

	public uint SynchronizedTimeOrBestGuess
	{
		get
		{
			if (_clockDelta.HasValue)
			{
				return SynchronizedTime.Value + DebugAdjust;
			}
			return TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow) + DebugAdjust;
		}
	}

	public bool IsSynchronized
	{
		get
		{
			return !_isSynchronizing && _clockDelta.HasValue;
		}
	}

	private ChallengeDataProvider DataProvider
	{
		get
		{
			return ChallengeManager.Instance.DataProvider;
		}
	}

	public static event EventHandler ClockDesynchronized;

	public static event EventHandler ClockSynchronized;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		ResynchronizeWithServer(true);
	}

	public void ResynchronizeWithServer(bool invalidateLocalTime)
	{
		_log.LogDebug("ResynchronizeWithServer({0})", invalidateLocalTime);
		if (invalidateLocalTime)
		{
			_clockDelta = null;
		}
		StartCoroutine(UpdateSynchronizedClockCoroutine());
	}

	public IEnumerator UpdateSynchronizedClockCoroutine()
	{
		if (_isSynchronizing)
		{
			yield break;
		}
		_isSynchronizing = true;
		int? newDelta = null;
		do
		{
			_log.LogDebug("Starting update of synchronized clock.");
			Bedrock.brUserConnectionStatus connectionStatus = ChallengeManager.ConnectionStatus;
			if (!connectionStatus.IsOnline())
			{
				_log.Log("Challenge status is {0}, not connected to internet. Waiting for clock sync.", connectionStatus);
			}
			else
			{
				using (BedrockTask task = DataProvider.BeginGetServerTime())
				{
					yield return StartCoroutine(task.WaitForTaskToCompleteOrTimeoutCoroutine());
					uint serverTime;
					if (DataProvider.EndGetServerTime(task, out serverTime))
					{
						_log.Log("Succeeded getting server time ({0})", serverTime);
						uint localUtcTime = TimeUtils.GetSecondsSinceUnixEpoch(DateTime.UtcNow);
						_log.LogDebug("Comparing server time {0} to current time {1}.", serverTime, localUtcTime);
						newDelta = Convert.ToInt32((long)serverTime - (long)localUtcTime);
						_log.LogDebug("New delta means that [local time] + {0} = [server time]", newDelta);
					}
					else
					{
						_log.LogError("Unable to get server time - " + task);
					}
				}
			}
			if (!newDelta.HasValue)
			{
				_log.LogDebug("Waiting to retry.");
				yield return StartCoroutine(CoroutineUtils.WaitForWallTime(1f));
			}
		}
		while (!newDelta.HasValue);
		_log.LogDebug("Succeeded getting new clock delta ({0}).", newDelta);
		_clockDelta = newDelta;
		OnClockSynchronized();
		_isSynchronizing = false;
	}

	public IEnumerator WaitForSynchronizeOrTimeoutCoroutine()
	{
		float timeoutTime = Time.realtimeSinceStartup + 10f;
		while (Time.realtimeSinceStartup < timeoutTime && !IsSynchronized)
		{
			yield return new WaitForEndOfFrame();
		}
	}

	private void OnClockDesynchronized()
	{
		_log.LogDebug("Raising desync event.");
		if (SynchronizedClock.ClockDesynchronized != null)
		{
			SynchronizedClock.ClockDesynchronized(this, new EventArgs());
		}
	}

	private void OnClockSynchronized()
	{
		_log.LogDebug("Raising sync event.");
		if (SynchronizedClock.ClockSynchronized != null)
		{
			SynchronizedClock.ClockSynchronized(this, new EventArgs());
		}
	}

	private void OnApplicationPause(bool paused)
	{
		_log.LogDebug("OnApplicationPause({0})", paused);
		_clockDelta = null;
		if (!paused)
		{
			_log.Log("Application is coming back from a pause. Invalidating synchronized time and requesting a new one.");
			_log.LogDebug("Bedrock Status: {0}", ChallengeManager.ConnectionStatus);
			ResynchronizeWithServer(true);
		}
	}
}
