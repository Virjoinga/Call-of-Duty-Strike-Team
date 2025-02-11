using UnityEngine;

public class TriggerBoxOverride : ContainerOverride
{
	public TriggerVolumeData m_OverrideData = new TriggerVolumeData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		TriggerVolume triggerVolume = cont.FindComponentOfType(typeof(TriggerVolume)) as TriggerVolume;
		if (triggerVolume != null)
		{
			triggerVolume.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		TriggerVolume componentInChildren = gameObj.GetComponentInChildren<TriggerVolume>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(0f, 1f, 0f, 1f);
		if (m_OverrideData.NotifyOnEnter != null && m_OverrideData.NotifyOnEnter.Count != 0)
		{
			foreach (GameObject item3 in m_OverrideData.NotifyOnEnter)
			{
				if (item3 != null)
				{
					LineFlag inFlag = LineFlag.Out;
					Transform inTrans = item3.transform;
					Color inColor = color;
					LineDetail item = new LineDetail(inFlag, inTrans, inColor);
					visibleConnections.lineProperties.Add(item);
				}
			}
		}
		color = new Color(1f, 0f, 0f, 1f);
		if (m_OverrideData.NotifyOnLeave == null || m_OverrideData.NotifyOnLeave.Count == 0)
		{
			return;
		}
		foreach (GameObject item4 in m_OverrideData.NotifyOnLeave)
		{
			if (item4 != null)
			{
				LineFlag inFlag2 = LineFlag.In;
				Transform inTrans2 = item4.transform;
				Color inColor2 = color;
				LineDetail item2 = new LineDetail(inFlag2, inTrans2, inColor2);
				visibleConnections.lineProperties.Add(item2);
			}
		}
	}
}
