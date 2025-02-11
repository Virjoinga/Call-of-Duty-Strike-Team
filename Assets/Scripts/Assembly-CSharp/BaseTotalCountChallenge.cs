public abstract class BaseTotalCountChallenge : Challenge
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

	protected override void OnEnable()
	{
		base.OnEnable();
		EventHub.Instance.OnEndMission += HandleGameOver;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		EventHub.Instance.OnEndMission -= HandleGameOver;
	}

	private void HandleGameOver(object sender, Events.EndMission args)
	{
		ChallengeManager.Instance.ScoreSubmissionManager.SubmitCurrentScoreForChallenge(this);
	}
}
