using UnityEngine;

public class HostageSequenceOverride : ContainerOverride
{
	public HostageSequenceData m_OverrideData = new HostageSequenceData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		HostageSequence hostageSequence = cont.FindComponentOfType(typeof(HostageSequence)) as HostageSequence;
		if (hostageSequence != null)
		{
			hostageSequence.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(hostageSequence);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		HostageSequence componentInChildren = gameObj.GetComponentInChildren<HostageSequence>();
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
		if (m_OverrideData.ObjectToCallOnHostageDeath != null && m_OverrideData.ObjectToCallOnHostageDeath.Count != 0)
		{
			foreach (GameObject item3 in m_OverrideData.ObjectToCallOnHostageDeath)
			{
				if (!(item3 == null))
				{
					LineFlag inFlag = LineFlag.Out;
					Transform inTrans = item3.transform;
					Color inColor = color;
					LineDetail item = new LineDetail(inFlag, inTrans, inColor);
					visibleConnections.lineProperties.Add(item);
				}
			}
		}
		if (m_OverrideData.ObjectToCallOnTakerDeath == null || m_OverrideData.ObjectToCallOnTakerDeath.Count == 0)
		{
			return;
		}
		foreach (GameObject item4 in m_OverrideData.ObjectToCallOnTakerDeath)
		{
			if (!(item4 == null))
			{
				LineFlag inFlag2 = LineFlag.Out;
				Transform inTrans2 = item4.transform;
				Color inColor2 = color;
				LineDetail item2 = new LineDetail(inFlag2, inTrans2, inColor2);
				visibleConnections.lineProperties.Add(item2);
			}
		}
	}
}
