public class KillMostEnemiesTimedChallenge : BaseTimedChallenge
{
	public const string ScriptName = "KillMostEnemiesTimedChallenge";

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
		if (args.Attacker.PlayerControlled)
		{
			base.LeaderboardValue++;
		}
	}
}
