public class InteriorCrateOverride : ContainerOverride
{
	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		Dumpster dumpster = cont.FindComponentOfType(typeof(Dumpster)) as Dumpster;
		if (!(dumpster != null))
		{
		}
	}
}
