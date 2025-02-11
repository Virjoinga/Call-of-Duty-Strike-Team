using System.Collections.Generic;
using UnityEngine;

public class KillOverride : ContainerOverride
{
	public MissionObjectiveData m_OverrideData = new MissionObjectiveData();

	public List<KillObjectMonitor> m_OverrideMonitors = new List<KillObjectMonitor>();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		KillObjectObjective killObjectObjective = cont.FindComponentOfType(typeof(KillObjectObjective)) as KillObjectObjective;
		if (!(killObjectObjective != null))
		{
			return;
		}
		killObjectObjective.m_Interface = m_OverrideData;
		killObjectObjective.Monitors = m_OverrideMonitors;
		m_OverrideData.CopyContainerObjectives(killObjectObjective);
		foreach (KillObjectMonitor overrideMonitor in m_OverrideMonitors)
		{
			overrideMonitor.CopyContainerData(killObjectObjective);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		MissionObjective componentInChildren = gameObj.GetComponentInChildren<MissionObjective>();
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
		if (m_OverrideData.ObjectivesToEnableOnPass != null && m_OverrideData.ObjectivesToEnableOnPass.Length != 0)
		{
			Container[] objectivesToEnableOnPass = m_OverrideData.ObjectivesToEnableOnPass;
			foreach (Container container in objectivesToEnableOnPass)
			{
				GameObject gameObject = container.gameObject;
				LineFlag inFlag = LineFlag.Out;
				Transform inTrans = gameObject.transform;
				Color inColor = color;
				LineDetail item = new LineDetail(inFlag, inTrans, inColor);
				visibleConnections.lineProperties.Add(item);
			}
		}
		if (m_OverrideData.ObjectivesToDisableOnPass != null && m_OverrideData.ObjectivesToDisableOnPass.Length != 0)
		{
			Container[] objectivesToDisableOnPass = m_OverrideData.ObjectivesToDisableOnPass;
			foreach (Container container2 in objectivesToDisableOnPass)
			{
				GameObject gameObject2 = container2.gameObject;
				LineFlag inFlag2 = LineFlag.Out;
				Transform inTrans2 = gameObject2.transform;
				Color inColor2 = color;
				LineDetail item2 = new LineDetail(inFlag2, inTrans2, inColor2);
				visibleConnections.lineProperties.Add(item2);
			}
		}
		if (m_OverrideData.GameObjectsToActivate != null && m_OverrideData.GameObjectsToActivate.Length != 0)
		{
			GameObject[] gameObjectsToActivate = m_OverrideData.GameObjectsToActivate;
			foreach (GameObject gameObject3 in gameObjectsToActivate)
			{
				LineFlag inFlag3 = LineFlag.Out;
				Transform inTrans3 = gameObject3.transform;
				Color inColor3 = color;
				LineDetail item3 = new LineDetail(inFlag3, inTrans3, inColor3);
				visibleConnections.lineProperties.Add(item3);
			}
		}
		if (m_OverrideData.ObjectToCallOnComplete != null && m_OverrideData.ObjectToCallOnComplete.Count != 0)
		{
			foreach (GameObject item6 in m_OverrideData.ObjectToCallOnComplete)
			{
				LineFlag inFlag4 = LineFlag.Out;
				Transform inTrans4 = item6.transform;
				Color inColor4 = color;
				LineDetail item4 = new LineDetail(inFlag4, inTrans4, inColor4);
				visibleConnections.lineProperties.Add(item4);
			}
		}
		color = new Color(0f, 1f, 0f, 1f);
		if (m_OverrideMonitors == null || m_OverrideMonitors.Count == 0)
		{
			return;
		}
		foreach (KillObjectMonitor overrideMonitor in m_OverrideMonitors)
		{
			GameObject actorToKill = overrideMonitor.ActorToKill;
			LineFlag inFlag5 = LineFlag.In;
			Transform inTrans5 = actorToKill.transform;
			Color inColor5 = color;
			LineDetail item5 = new LineDetail(inFlag5, inTrans5, inColor5);
			visibleConnections.lineProperties.Add(item5);
		}
	}
}
