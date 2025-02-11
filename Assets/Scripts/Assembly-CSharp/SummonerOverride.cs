public class SummonerOverride : ContainerOverride
{
	public SummonerData m_OverrideData = new SummonerData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_OverrideData.ResolveGuidLinks();
		Summoner summoner = cont.FindComponentOfType(typeof(Summoner)) as Summoner;
		if (summoner != null)
		{
			summoner.mInterface = m_OverrideData;
		}
	}
}
