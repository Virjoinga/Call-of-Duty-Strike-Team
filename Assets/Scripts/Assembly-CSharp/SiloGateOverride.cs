using UnityEngine;

public class SiloGateOverride : ContainerOverride
{
	public GateGrillData m_override = new GateGrillData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SiloGrillGate siloGrillGate = cont.FindComponentOfType(typeof(SiloGrillGate)) as SiloGrillGate;
		if (siloGrillGate != null)
		{
			siloGrillGate.m_Interface = m_override;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SiloGrillGate componentInChildren = gameObj.GetComponentInChildren<SiloGrillGate>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
