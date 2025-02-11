using UnityEngine;

public class ZipLineOverride : ContainerOverride
{
	public ZipLineData mOverrideData;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		ZipLine zipLine = cont.FindComponentOfType(typeof(ZipLine)) as ZipLine;
		if (zipLine != null && mOverrideData != null)
		{
			zipLine.Interface = mOverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		ZipLine componentInChildren = gameObj.GetComponentInChildren<ZipLine>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
