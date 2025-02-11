using UnityEngine;

public class DumpsterOverride : ContainerOverride
{
	public DumpsterData mOverrideData;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		Dumpster dumpster = cont.FindComponentOfType(typeof(Dumpster)) as Dumpster;
		if (dumpster != null && mOverrideData != null && !Application.isPlaying)
		{
			dumpster.Interface = mOverrideData;
			dumpster.SwitchModelToMatchType();
		}
		base.ApplyOverride(cont);
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		Dumpster componentInChildren = gameObj.GetComponentInChildren<Dumpster>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
