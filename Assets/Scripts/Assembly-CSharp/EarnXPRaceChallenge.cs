public class EarnXPRaceChallenge : BaseRaceChallenge
{
	public const string ScriptName = "EarnXPRaceChallenge";

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
		uint num = (uint)args.XP;
		if (num > base.LifeRemaining)
		{
			num = base.LifeRemaining;
		}
		base.LifeRemaining -= num;
	}
}
