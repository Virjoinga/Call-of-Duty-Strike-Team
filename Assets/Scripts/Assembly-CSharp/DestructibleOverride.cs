using UnityEngine;

public class DestructibleOverride : ContainerOverride
{
	public DestructibleData m_OverrideData = new DestructibleData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		Destructible destructible = cont.FindComponentOfType(typeof(Destructible)) as Destructible;
		if (destructible != null)
		{
			destructible.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		Destructible componentInChildren = gameObj.GetComponentInChildren<Destructible>();
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
		if (m_OverrideData.ObjectToCallOnDestruction != null)
		{
			GameObject gameObject = m_OverrideData.ObjectToCallOnDestruction.gameObject;
			if (gameObject != null)
			{
				LineFlag inFlag = LineFlag.Out;
				Transform inTrans = gameObject.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
	}
}
