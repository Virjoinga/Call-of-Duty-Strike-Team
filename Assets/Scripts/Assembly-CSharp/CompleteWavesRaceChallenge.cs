public class CompleteWavesRaceChallenge : BaseRaceChallenge
{
	public const string ScriptName = "CompleteWavesRaceChallenge";

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
		base.LifeRemaining--;
	}
}
