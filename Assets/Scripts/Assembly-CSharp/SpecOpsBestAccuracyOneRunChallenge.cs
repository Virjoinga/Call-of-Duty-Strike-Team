public class SpecOpsBestAccuracyOneRunChallenge : BaseOneRunChallenge
{
	public const string ScriptName = "SpecOpsBestAccuracyOneRunChallenge";

	private const int WaveToReach = 5;

	public override string StatusPanelStatusText
	{
		get
		{
			if (GMGData.Instance.CurrentWave() <= 5)
			{
				return base.StatusPanelStatusText;
			}
			return Language.GetFormatString("S_GMG_BEST_SCORE", base.BestScoreInCycle);
		}
	}

	protected override void OnEnable()
	{
		EventHub.Instance.OnSpecOpsWaveComplete += WaveComplete;
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		EventHub.Instance.OnSpecOpsWaveComplete -= WaveComplete;
		base.OnDisable();
	}

	private void WaveComplete(object sender, Events.SpecOpsWaveComplete args)
	{
		if (args.WaveNum == 5)
		{
			base.LeaderboardValue = (int)StatsManager.Instance.CharacterStats().GetCurrentMissionCombinedStat().AccuracyInFP;
			SubmitScoreAfterRunEnd();
			CompleteAttemptFromGameplayAction();
		}
	}
}
