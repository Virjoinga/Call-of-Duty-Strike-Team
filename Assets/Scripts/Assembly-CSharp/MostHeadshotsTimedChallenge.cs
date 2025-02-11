public class MostHeadshotsTimedChallenge : BaseTimedChallenge
{
	public const string ScriptName = "MostHeadshotsTimedChallenge";

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
		if (args.Attacker != null && args.Attacker.PlayerControlled && args.HeadShot)
		{
			base.LeaderboardValue++;
		}
	}
}
