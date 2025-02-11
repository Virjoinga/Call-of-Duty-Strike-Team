using UnityEngine;

public class AlarmPanelScenarioOverride : ContainerOverride
{
	public AlarmPanelData m_OverrideData = new AlarmPanelData();

	public HackableObjectData mHackOverrideData = new HackableObjectData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		AlarmPanel alarmPanel = cont.FindComponentOfType(typeof(AlarmPanel)) as AlarmPanel;
		if (alarmPanel != null)
		{
			alarmPanel.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(alarmPanel);
		}
		HackableObject hackableObject = cont.FindComponentOfType(typeof(HackableObject)) as HackableObject;
		if (hackableObject != null)
		{
			hackableObject.m_Interface = mHackOverrideData;
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
