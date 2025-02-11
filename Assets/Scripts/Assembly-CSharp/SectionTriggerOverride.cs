using UnityEngine;

public class SectionTriggerOverride : ContainerOverride
{
	public SectionTriggerData m_OverrideData = new SectionTriggerData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SectionTrigger sectionTrigger = cont.FindComponentOfType(typeof(SectionTrigger)) as SectionTrigger;
		if (sectionTrigger != null)
		{
			sectionTrigger.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SectionTrigger componentInChildren = gameObj.GetComponentInChildren<SectionTrigger>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
