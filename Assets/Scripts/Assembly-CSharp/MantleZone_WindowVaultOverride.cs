public class MantleZone_WindowVaultOverride : ContainerOverride
{
	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		NavigationZone navigationZone = cont.FindComponentOfType(typeof(NavigationZone)) as NavigationZone;
		if (navigationZone != null)
		{
			navigationZone.GenerateOffLinks();
		}
	}
}
