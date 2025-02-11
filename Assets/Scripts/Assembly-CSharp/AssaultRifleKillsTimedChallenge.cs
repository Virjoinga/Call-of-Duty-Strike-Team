public class AssaultRifleKillsTimedChallenge : BaseTimedChallenge
{
	public const string ScriptName = "AssaultRifleKillsTimedChallenge";

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
		if (args.Attacker.PlayerControlled && args.Attacker.WeaponClass == WeaponDescriptor.WeaponClass.AssaultRifle)
		{
			base.LeaderboardValue++;
		}
	}
}
