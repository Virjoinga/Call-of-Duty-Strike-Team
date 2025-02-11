using UnityEngine;

public class SendMessageToGroupOverride : ContainerOverride
{
	public GroupMessageData m_OverrideData = new GroupMessageData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SendMessageOnMessage sendMessageOnMessage = cont.FindComponentOfType(typeof(SendMessageOnMessage)) as SendMessageOnMessage;
		if (sendMessageOnMessage != null)
		{
			sendMessageOnMessage.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SendMessageOnMessage componentInChildren = gameObj.GetComponentInChildren<SendMessageOnMessage>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(1f, 0f, 0f, 1f);
		if (m_OverrideData.targetObjs == null || m_OverrideData.targetObjs.Count == 0)
		{
			return;
		}
		foreach (GameObject targetObj in m_OverrideData.targetObjs)
		{
			if (targetObj != null)
			{
				LineFlag inFlag = LineFlag.Out;
				Transform inTrans = targetObj.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
	}
}
