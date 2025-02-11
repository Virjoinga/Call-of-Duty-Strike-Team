using UnityEngine;

public class SendMessageDelayedOverride : ContainerOverride
{
	public DelayedMessageData m_OverrideData = new DelayedMessageData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		DelayedMessage delayedMessage = cont.FindComponentOfType(typeof(DelayedMessage)) as DelayedMessage;
		if (delayedMessage != null)
		{
			delayedMessage.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		DelayedMessage componentInChildren = gameObj.GetComponentInChildren<DelayedMessage>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
	}
}
