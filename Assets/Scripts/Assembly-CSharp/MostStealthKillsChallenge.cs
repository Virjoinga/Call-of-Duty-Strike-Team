public class MostStealthKillsChallenge : BaseTotalCountChallenge
{
	public const string ScriptName = "MostStealthKillsChallenge";

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
		if (args.Attacker != null && args.Attacker.PlayerControlled && args.SilentKill)
		{
			base.LeaderboardValue++;
		}
	}
}
