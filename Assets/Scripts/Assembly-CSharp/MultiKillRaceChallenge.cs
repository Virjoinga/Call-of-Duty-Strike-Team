public class MultiKillRaceChallenge : BaseRaceChallenge
{
	public const string ScriptName = "MultiKillRaceChallenge";

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
		if (args.Type == "Bonus_Double Kill" || args.Type == "Bonus_Tripple Kill" || args.Type == "Bonus_Multi Kill")
		{
			base.LifeRemaining--;
		}
	}
}
