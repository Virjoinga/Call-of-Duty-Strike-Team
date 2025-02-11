public class MostHeadshotsRaceChallenge : BaseRaceChallenge
{
	public const string ScriptName = "MostHeadshotsRaceChallenge";

	protected override void OnEnable()
	{
		EventHub.Instance.OnKill += Kill;
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		EventHub.Instance.OnKill -= Kill;
		base.OnDisable();
	}

	private void Kill(object sender, Events.Kill args)
	{
		if (args.HeadShot && args.Attacker != null && args.Attacker.PlayerControlled)
		{
			base.LifeRemaining--;
		}
	}
}
