using UnityEngine;

public class MetalGateSwingOverride : ContainerOverride
{
	public MetalGateSwingData m_override = new MetalGateSwingData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MetalGateSwing metalGateSwing = cont.FindComponentOfType(typeof(MetalGateSwing)) as MetalGateSwing;
		if (metalGateSwing != null)
		{
			metalGateSwing.m_interface = m_override;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		MetalGateSwing componentInChildren = gameObj.GetComponentInChildren<MetalGateSwing>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
