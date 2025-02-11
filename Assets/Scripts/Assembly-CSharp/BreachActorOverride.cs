using UnityEngine;

public class BreachActorOverride : ContainerOverride
{
	public BreachActorData m_OverrideData = new BreachActorData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		BreachActor breachActor = cont.FindComponentOfType(typeof(BreachActor)) as BreachActor;
		if (breachActor != null)
		{
			breachActor.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(breachActor);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		BreachActor componentInChildren = gameObj.GetComponentInChildren<BreachActor>();
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
		if (m_OverrideData.ObjectToCallOnDeath == null || m_OverrideData.ObjectToCallOnDeath.Count == 0)
		{
			return;
		}
		foreach (GameObject item2 in m_OverrideData.ObjectToCallOnDeath)
		{
			if (!(item2 == null))
			{
				LineFlag inFlag = LineFlag.Out;
				Transform inTrans = item2.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
	}
}
