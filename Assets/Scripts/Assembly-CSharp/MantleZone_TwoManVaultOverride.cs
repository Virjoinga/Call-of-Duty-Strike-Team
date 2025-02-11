using UnityEngine;

public class MantleZone_TwoManVaultOverride : ContainerOverride
{
	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		cont.transform.localScale = Vector3.one;
		NavigationZone navigationZone = cont.FindComponentOfType(typeof(NavigationZone)) as NavigationZone;
		if (navigationZone != null)
		{
			navigationZone.GenerateOffLinks();
		}
	}
}
