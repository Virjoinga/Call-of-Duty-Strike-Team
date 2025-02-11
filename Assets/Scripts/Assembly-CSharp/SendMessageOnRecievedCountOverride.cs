using UnityEngine;

public class SendMessageOnRecievedCountOverride : ContainerOverride
{
	public RecievedCountData m_OverrideData = new RecievedCountData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SendMessageOnRecievedCount sendMessageOnRecievedCount = cont.FindComponentOfType(typeof(SendMessageOnRecievedCount)) as SendMessageOnRecievedCount;
		if (sendMessageOnRecievedCount != null)
		{
			sendMessageOnRecievedCount.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SendMessageOnRecievedCount componentInChildren = gameObj.GetComponentInChildren<SendMessageOnRecievedCount>();
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
		if (m_OverrideData.ObjectToMessage != null)
		{
			GameObject objectToMessage = m_OverrideData.ObjectToMessage;
			LineFlag inFlag = LineFlag.Out;
			Transform inTrans = objectToMessage.transform;
			Color inColor = color;
			LineDetail item = new LineDetail(inFlag, inTrans, inColor);
			visibleConnections.lineProperties.Add(item);
		}
	}
}
