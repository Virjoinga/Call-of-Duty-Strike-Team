using UnityEngine;

public class SwitchRoutineOverride : ContainerOverride
{
	public SwitchRoutineData m_OverrideData = new SwitchRoutineData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SwitchRoutine switchRoutine = cont.FindComponentOfType(typeof(SwitchRoutine)) as SwitchRoutine;
		if (switchRoutine != null)
		{
			switchRoutine.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(switchRoutine);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SwitchRoutine componentInChildren = gameObj.GetComponentInChildren<SwitchRoutine>();
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
		if (m_OverrideData.Actors != null && m_OverrideData.Actors.Count != 0)
		{
			foreach (GameObject actor in m_OverrideData.Actors)
			{
				LineFlag inFlag = LineFlag.Out;
				if (actor != null)
				{
					Transform inTrans = actor.transform;
					Color inColor = color;
					LineDetail item = new LineDetail(inFlag, inTrans, inColor);
					visibleConnections.lineProperties.Add(item);
				}
			}
		}
		if (m_OverrideData.TriggerActors == null || m_OverrideData.TriggerActors.Count == 0)
		{
			return;
		}
		foreach (GameObject triggerActor in m_OverrideData.TriggerActors)
		{
			LineFlag inFlag2 = LineFlag.Out;
			if (triggerActor != null)
			{
				Transform inTrans2 = triggerActor.transform;
				Color inColor2 = color;
				LineDetail item2 = new LineDetail(inFlag2, inTrans2, inColor2);
				visibleConnections.lineProperties.Add(item2);
			}
		}
	}
}
