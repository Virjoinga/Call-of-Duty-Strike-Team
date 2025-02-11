using UnityEngine;

public class GenericTriggerOverride : ContainerOverride
{
	public GenericTriggerData m_OverrideData = new GenericTriggerData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		GenericTrigger genericTrigger = cont.FindComponentOfType(typeof(GenericTrigger)) as GenericTrigger;
		if (genericTrigger != null)
		{
			genericTrigger.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(genericTrigger);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		GenericTrigger componentInChildren = gameObj.GetComponentInChildren<GenericTrigger>();
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
		if (m_OverrideData.Actors != null && m_OverrideData.Actors.Count != 0)
		{
			foreach (GameObject actor in m_OverrideData.Actors)
			{
				if (!(actor == null))
				{
					LineFlag inFlag = LineFlag.In;
					Transform inTrans = actor.transform;
					Color inColor = color;
					LineDetail item = new LineDetail(inFlag, inTrans, inColor);
					visibleConnections.lineProperties.Add(item);
				}
			}
		}
		color = new Color(1f, 0f, 0f, 1f);
		if (m_OverrideData.ObjectToCall != null)
		{
			GameObject objectToCall = m_OverrideData.ObjectToCall;
			if (objectToCall != null)
			{
				LineFlag inFlag2 = LineFlag.Out;
				Transform inTrans2 = objectToCall.transform;
				Color inColor2 = color;
				LineDetail item2 = new LineDetail(inFlag2, inTrans2, inColor2);
				visibleConnections.lineProperties.Add(item2);
			}
		}
	}
}
