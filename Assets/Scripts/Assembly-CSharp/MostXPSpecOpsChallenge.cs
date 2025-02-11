public class MostXPSpecOpsChallenge : BaseTotalCountChallenge
{
	public const string ScriptName = "MostXPSpecOpsChallenge";

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
		if (ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			base.LeaderboardValue += args.XP;
		}
	}
}
