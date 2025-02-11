public class ShotgunKillsTimedChallenge : BaseTimedChallenge
{
	public const string ScriptName = "ShotgunKillsTimedChallenge";

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
		if (args.Attacker != null && args.Attacker.PlayerControlled && args.Attacker.WeaponClass == WeaponDescriptor.WeaponClass.Shotgun)
		{
			base.LeaderboardValue++;
		}
	}
}
