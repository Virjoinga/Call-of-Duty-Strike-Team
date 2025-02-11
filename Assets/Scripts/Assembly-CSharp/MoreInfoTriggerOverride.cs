using UnityEngine;

public class MoreInfoTriggerOverride : ContainerOverride
{
	public MoreInfoTriggerData m_OverrideData = new MoreInfoTriggerData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MoreInfoTrigger moreInfoTrigger = cont.FindComponentOfType(typeof(MoreInfoTrigger)) as MoreInfoTrigger;
		if (moreInfoTrigger != null)
		{
			moreInfoTrigger.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(moreInfoTrigger);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		MoreInfoTrigger componentInChildren = gameObj.GetComponentInChildren<MoreInfoTrigger>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
