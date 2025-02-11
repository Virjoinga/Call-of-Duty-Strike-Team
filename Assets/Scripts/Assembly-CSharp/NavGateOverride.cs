using UnityEngine;

public class NavGateOverride : ContainerOverride
{
	public NavGateData m_OverrideData = new NavGateData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		NavGate navGate = cont.FindComponentOfType(typeof(NavGate)) as NavGate;
		if (navGate != null)
		{
			navGate.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		NavGate componentInChildren = gameObj.GetComponentInChildren<NavGate>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
