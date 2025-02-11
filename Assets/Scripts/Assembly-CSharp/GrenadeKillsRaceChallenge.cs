public class GrenadeKillsRaceChallenge : BaseRaceChallenge
{
	public const string ScriptName = "GrenadeKillsRaceChallenge";

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
		if (args.Attacker != null && args.Attacker.PlayerControlled && args.GrenadeKill)
		{
			base.LifeRemaining--;
		}
	}
}
