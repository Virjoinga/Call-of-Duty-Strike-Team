public class MostGrenadeKillsOneRunChallenge : BaseOneRunChallenge
{
	public const string ScriptName = "MostGrenadeKillsOneRunChallenge";

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
		if (args.Attacker != null && args.Attacker.PlayerControlled && args.GrenadeKill && ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			base.LeaderboardValue++;
		}
	}
}
