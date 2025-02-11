using UnityEngine;

public class GateSlideOverride : ContainerOverride
{
	public GateSlideData m_override = new GateSlideData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		GateSlide gateSlide = cont.FindComponentOfType(typeof(GateSlide)) as GateSlide;
		if (gateSlide != null)
		{
			gateSlide.m_interface = m_override;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		GateSlide componentInChildren = gameObj.GetComponentInChildren<GateSlide>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
