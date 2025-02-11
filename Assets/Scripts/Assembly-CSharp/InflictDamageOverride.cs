using UnityEngine;

public class InflictDamageOverride : ContainerOverride
{
	public InflictDamageData m_OverrideData = new InflictDamageData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		InflictDamage inflictDamage = cont.FindComponentOfType(typeof(InflictDamage)) as InflictDamage;
		if (inflictDamage != null)
		{
			inflictDamage.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(inflictDamage);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		InflictDamage componentInChildren = gameObj.GetComponentInChildren<InflictDamage>();
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
		if (m_OverrideData.ActorsToDamage == null || m_OverrideData.ActorsToDamage.Count == 0)
		{
			return;
		}
		foreach (GameObject item2 in m_OverrideData.ActorsToDamage)
		{
			LineFlag inFlag = LineFlag.Out;
			Transform inTrans = item2.transform;
			Color inColor = color;
			LineDetail item = new LineDetail(inFlag, inTrans, inColor);
			visibleConnections.lineProperties.Add(item);
		}
	}
}
