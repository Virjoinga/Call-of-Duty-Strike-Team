public class MostMultiKillChallenge : BaseTotalCountChallenge
{
	public const string ScriptName = "MostMultiKillChallenge";

	protected override void OnEnable()
	{
		EventHub.Instance.OnXPEarned += XPEarned;
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		EventHub.Instance.OnXPEarned -= XPEarned;
		base.OnDisable();
	}

	private void XPEarned(object sender, Events.XPEarned args)
	{
		if (ActStructure.Instance.CurrentMissionIsSpecOps() && (args.Type == "Bonus_Double Kill" || args.Type == "Bonus_Triple Kill" || args.Type == "Bonus_Multi Kill"))
		{
			base.LeaderboardValue++;
		}
	}
}
