using UnityEngine;

public class SuggestedHUDModeOverride : ContainerOverride
{
	public SuggestedStateData m_OverrideData = new SuggestedStateData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SuggestedModeTrigger suggestedModeTrigger = cont.FindComponentOfType(typeof(SuggestedModeTrigger)) as SuggestedModeTrigger;
		if (suggestedModeTrigger != null)
		{
			suggestedModeTrigger.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SuggestedModeTrigger componentInChildren = gameObj.GetComponentInChildren<SuggestedModeTrigger>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
