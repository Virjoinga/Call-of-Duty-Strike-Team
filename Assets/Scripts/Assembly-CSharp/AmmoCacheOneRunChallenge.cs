public class AmmoCacheOneRunChallenge : BaseOneRunChallenge
{
	public const string ScriptName = "AmmoCacheOneRunChallenge";

	protected override void OnEnable()
	{
		EventHub.Instance.OnAmmoCacheUsed += AmmoCacheUsed;
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		EventHub.Instance.OnAmmoCacheUsed -= AmmoCacheUsed;
		base.OnDisable();
	}

	private void AmmoCacheUsed(object sender, Events.AmmoCacheUsed args)
	{
		base.LeaderboardValue++;
	}
}
