public class SniperKillsRaceChallenge : BaseRaceChallenge
{
	public const string ScriptName = "SniperKillsRaceChallenge";

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
		if (args.Attacker != null && args.Attacker.PlayerControlled && args.Attacker.WeaponClass == WeaponDescriptor.WeaponClass.SniperRifle)
		{
			base.LifeRemaining--;
		}
	}
}
