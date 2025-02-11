using UnityEngine;

public class MantleZone_3mOverride : ContainerOverride
{
	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		if (base.gameObject.name == "Mantle Zone 3m Up Only")
		{
			Object.DestroyImmediate(this);
		}
		NavigationZone navigationZone = cont.FindComponentOfType(typeof(NavigationZone)) as NavigationZone;
		if (navigationZone != null)
		{
			navigationZone.GenerateOffLinks();
		}
	}
}
