using UnityEngine;

public abstract class BaseRaceChallenge : Challenge
{
	private uint? _lastValidTimeSpentInRace;

	public override bool IsRetryable
	{
		get
		{
			return true;
		}
	}

	public override string LifeRemainingText
	{
		get
		{
			return string.Format("{0} {1} {2}", base.LifeRemaining, Language.Get(base.ChallengeData.GoalNoun), Language.Get("S_GENERIC_LEFT"));
		}
	}

	public override string StatusPanelStatusText
	{
		get
		{
			uint? currentTimeSpentInRace = CurrentTimeSpentInRace;
			if (!currentTimeSpentInRace.HasValue)
			{
				return Language.GetFormatString("S_GMG_STATUS_REMAINING", base.LifeRemaining);
			}
			string text = "--";
			if (base.ChallengeData.BestScoreSubmittedThisCycle.HasValue)
			{
				text = TimeUtils.GetShortTimeStringFromSeconds(base.ChallengeData.BestScoreSubmittedThisCycle.Value);
			}
			string shortTimeStringFromSeconds = TimeUtils.GetShortTimeStringFromSeconds(currentTimeSpentInRace.Value);
			return string.Format("{0}\n{1}", Language.GetFormatString("S_GMG_STATUS_TIME_CURRENT", shortTimeStringFromSeconds), Language.GetFormatString("S_GMG_BEST_TIME", text));
		}
	}

	protected uint? CurrentTimeSpentInRace
	{
		get
		{
			uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
			if (!synchronizedTime.HasValue)
			{
				base.InstanceLog.LogError("Someone asked for the current time spent in race, but clock is not syncrhonized.");
				return null;
			}
			return GetCurrentTimeSpentInRace(synchronizedTime.Value);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		ChallengeManager.UpdateComplete += HandleChallengeManagerUpdateComplete;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		ChallengeManager.UpdateComplete -= HandleChallengeManagerUpdateComplete;
	}

	protected override void CompleteAttempt(bool timedOut)
	{
		if (!timedOut)
		{
			uint? currentTimeSpentInRace = CurrentTimeSpentInRace;
			if (currentTimeSpentInRace.HasValue)
			{
				base.LeaderboardValue = currentTimeSpentInRace.Value;
			}
			else
			{
				Debug.LogError("Could not set leaderboard score. CurrentTimeSpentInRace couldn't be calculated!");
			}
		}
		base.CompleteAttempt(timedOut);
	}

	private void HandleChallengeManagerUpdateComplete(object sender, ValueEventArgs<uint> e)
	{
		uint currentTimeSpentInRace = GetCurrentTimeSpentInRace(e.Value);
		base.InstanceLog.Log("Time spent in race updated: '{0}'", currentTimeSpentInRace);
		OnStatusTextChanged();
		long? bestScoreSubmittedThisCycle = base.ChallengeData.BestScoreSubmittedThisCycle;
		if (bestScoreSubmittedThisCycle.HasValue && bestScoreSubmittedThisCycle.HasValue && currentTimeSpentInRace > bestScoreSubmittedThisCycle.Value)
		{
			string shortTimeStringFromSeconds = TimeUtils.GetShortTimeStringFromSeconds(currentTimeSpentInRace);
			string shortTimeStringFromSeconds2 = TimeUtils.GetShortTimeStringFromSeconds(bestScoreSubmittedThisCycle.Value);
			string formatString = Language.GetFormatString("S_GMG_DIDNT_BEAT_OLD_RACE_TIME", shortTimeStringFromSeconds, shortTimeStringFromSeconds2);
			NotificationPanel.Instance.Display(formatString);
			base.InstanceLog.Log("Failed to beat your previous best score ({0})! Stopping Race Challenge: {1}", bestScoreSubmittedThisCycle, base.ChallengeData.Id);
			CompleteAttempt(true);
		}
	}

	private uint GetCurrentTimeSpentInRace(uint currentTime)
	{
		return TimeUtils.GetSecondsSince(base.JoinTime, currentTime);
	}
}
