public class BaseTimedChallenge : Challenge
{
	public override string LifeRemainingText
	{
		get
		{
			if (base.LifeRemaining == 0)
			{
				return null;
			}
			return TimeUtils.GetShortFuzzyTimeStringFromSeconds(base.LifeRemaining) + " " + Language.Get("S_GENERIC_LEFT");
		}
	}

	public override bool IsRetryable
	{
		get
		{
			return true;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		EventHub.Instance.OnEndMission += HandleGameOver;
		ChallengeManager.UpdateComplete += HandleChallengeManagerUpdateComplete;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		EventHub.Instance.OnEndMission -= HandleGameOver;
		ChallengeManager.UpdateComplete -= HandleChallengeManagerUpdateComplete;
	}

	private void HandleGameOver(object sender, Events.EndMission args)
	{
		ChallengeManager.Instance.ScoreSubmissionManager.SubmitCurrentScoreForChallenge(this);
	}

	private void HandleChallengeManagerUpdateComplete(object sender, ValueEventArgs<uint> e)
	{
		base.InstanceLog.LogDebug("HandleChallengeManagerUpdateComplete(?, {0})", e.Value);
		UpdateLifeRemainingBasedOnCurrentTime(e.Value);
	}

	private void UpdateLifeRemainingBasedOnCurrentTime(uint currentTime)
	{
		uint secondsSince = TimeUtils.GetSecondsSince(base.JoinTime, currentTime);
		uint lifetime = base.ChallengeData.Lifetime;
		if (secondsSince >= lifetime)
		{
			base.InstanceLog.LogWarning("Got update call after challenge would have expired. No time remaining. (currentTime={0},JoinTime={1},timeSinceJoin={2})", currentTime, base.JoinTime, secondsSince);
			base.LifeRemaining = 0u;
			return;
		}
		uint num = lifetime - secondsSince;
		uint remainingTime = base.ChallengeData.GetRemainingTime(currentTime);
		if (remainingTime < num)
		{
			num = remainingTime;
		}
		base.LifeRemaining = num;
	}
}
