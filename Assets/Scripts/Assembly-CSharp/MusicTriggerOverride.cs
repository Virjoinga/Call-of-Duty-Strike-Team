using UnityEngine;

public class MusicTriggerOverride : ContainerOverride
{
	public MusicTriggerData m_OverrideData = new MusicTriggerData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MusicTrigger musicTrigger = cont.FindComponentOfType(typeof(MusicTrigger)) as MusicTrigger;
		if (musicTrigger != null)
		{
			musicTrigger.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		MusicTrigger componentInChildren = gameObj.GetComponentInChildren<MusicTrigger>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
