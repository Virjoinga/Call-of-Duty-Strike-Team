public class InteriorHandlerOverride : ContainerOverride
{
	public BuildingWithInteriorData m_OverrideData = new BuildingWithInteriorData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		BuildingWithInterior buildingWithInterior = cont.FindComponentOfType(typeof(BuildingWithInterior)) as BuildingWithInterior;
		if (buildingWithInterior != null)
		{
			buildingWithInterior.m_Interface = m_OverrideData;
		}
	}
}
