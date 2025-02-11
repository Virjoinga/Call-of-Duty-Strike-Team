using UnityEngine;

public class OverwatchOverride : ContainerOverride
{
	public OverwatchData m_OverrideData;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		OverwatchTrigger overwatchTrigger = cont.FindComponentOfType(typeof(OverwatchTrigger)) as OverwatchTrigger;
		if (overwatchTrigger != null)
		{
			overwatchTrigger.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		OverwatchTrigger componentInChildren = gameObj.GetComponentInChildren<OverwatchTrigger>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
