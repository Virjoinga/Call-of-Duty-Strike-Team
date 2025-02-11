using UnityEngine;

public class SwitchEventsOverride : ContainerOverride
{
	public EventsSwitcherData m_OverrideData = new EventsSwitcherData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		EventsSwitcher eventsSwitcher = cont.FindComponentOfType(typeof(EventsSwitcher)) as EventsSwitcher;
		if (eventsSwitcher != null)
		{
			eventsSwitcher.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(eventsSwitcher);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		EventsSwitcher componentInChildren = gameObj.GetComponentInChildren<EventsSwitcher>();
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
		if (m_OverrideData.ActorsToSwitch != null && m_OverrideData.ActorsToSwitch.Count != 0)
		{
			foreach (GameObject item3 in m_OverrideData.ActorsToSwitch)
			{
				LineFlag inFlag = LineFlag.Out;
				Transform inTrans = item3.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
		color = new Color(0f, 1f, 0f, 1f);
		if (m_OverrideData.EventsToSwitch != null)
		{
			GameObject eventsToSwitch = m_OverrideData.EventsToSwitch;
			LineFlag inFlag2 = LineFlag.Out;
			Transform inTrans2 = eventsToSwitch.transform;
			Color inColor2 = color;
			LineDetail item2 = new LineDetail(inFlag2, inTrans2, inColor2);
			visibleConnections.lineProperties.Add(item2);
		}
	}
}
