public abstract class BaseOneRunChallenge : Challenge
{
	public override bool IsRetryable
	{
		get
		{
			return false;
		}
	}

	public override string LifeRemainingText
	{
		get
		{
			return null;
		}
	}

	public override string StatusPanelStatusText
	{
		get
		{
			if (ActStructure.Instance.MissionInProgress)
			{
				return base.StatusPanelStatusText;
			}
			return Language.GetFormatString("S_GMG_BEST_SCORE", base.BestScoreInCycle);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		EventHub.Instance.OnStartMission += HandleGameManagerMissionStarted;
		EventHub.Instance.OnEndMission += HandleGameManagerMissionOver;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		EventHub.Instance.OnStartMission -= HandleGameManagerMissionStarted;
		EventHub.Instance.OnEndMission -= HandleGameManagerMissionOver;
	}

	private void HandleGameManagerMissionStarted(object sender, Events.StartMission args)
	{
		Reset();
	}

	protected virtual void HandleGameManagerMissionOver(object sender, Events.EndMission args)
	{
		SubmitScoreAfterRunEnd();
	}

	protected virtual void SubmitScoreAfterRunEnd()
	{
		ChallengeManager.Instance.ScoreSubmissionManager.SubmitCurrentScoreForChallenge(this);
	}
}
