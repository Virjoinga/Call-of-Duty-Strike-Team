using UnityEngine;

public class DestroyObjectOverride : ContainerOverride
{
	public MissionObjectiveData m_OverrideData = new MissionObjectiveData();

	public DestroyObjectData m_DestroyOverrideData = new DestroyObjectData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		DestroyObjectObjective destroyObjectObjective = cont.FindComponentOfType(typeof(DestroyObjectObjective)) as DestroyObjectObjective;
		if (destroyObjectObjective != null)
		{
			destroyObjectObjective.m_Interface = m_OverrideData;
			destroyObjectObjective.m_destroyInterface = m_DestroyOverrideData;
			m_OverrideData.CopyContainerObjectives(destroyObjectObjective);
			m_DestroyOverrideData.CopyContainerData(destroyObjectObjective);
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
		if (m_DestroyOverrideData.ObjectsToDestroy == null || m_DestroyOverrideData.ObjectsToDestroy.Count == 0)
		{
			return;
		}
		foreach (GameObject item7 in m_DestroyOverrideData.ObjectsToDestroy)
		{
			LineFlag inFlag5 = LineFlag.In;
			Transform inTrans5 = item7.transform;
			Color inColor5 = color;
			LineDetail item5 = new LineDetail(inFlag5, inTrans5, inColor5);
			visibleConnections.lineProperties.Add(item5);
		}
	}
}
