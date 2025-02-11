using UnityEngine;

public class ExplodableObjectOverride : ContainerOverride
{
	public ExplodableObjectData m_OverrideData;

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		ExplodableObject explodableObject = cont.FindComponentOfType(typeof(ExplodableObject)) as ExplodableObject;
		if (explodableObject != null)
		{
			if (m_OverrideData.GlobalTriggerObject == null)
			{
				m_OverrideData.GlobalTriggerObject = explodableObject.gameObject;
			}
			explodableObject.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		ExplodableObject componentInChildren = gameObj.GetComponentInChildren<ExplodableObject>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
