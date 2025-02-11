using UnityEngine;

public class HackableOverride : ContainerOverride
{
	public HackableObjectData mOverrideData = new HackableObjectData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		HackableObject hackableObject = cont.FindComponentOfType(typeof(HackableObject)) as HackableObject;
		if (hackableObject != null)
		{
			hackableObject.m_Interface = mOverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		HackableObject componentInChildren = gameObj.GetComponentInChildren<HackableObject>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
