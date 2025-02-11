using System.Collections.Generic;
using UnityEngine;

public class EnemyFromDoorOverride : ContainerOverride
{
	public SpawnerDoorData m_OverrideData = new SpawnerDoorData();

	public bool TutorialEnemy;

	private GameObject FindObjectAt(Vector3 pos)
	{
		float num = float.MaxValue;
		GameObject result = null;
		GameObject[] array = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float sqrMagnitude = (gameObject.transform.position - pos).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				result = gameObject;
				num = sqrMagnitude;
			}
		}
		return result;
	}

	public void RescueQuickDestinationsFromOblivion()
	{
		List<QuickDestination> list = new List<QuickDestination>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (!(child.name == "Automated"))
			{
				continue;
			}
			MoveToDescriptor component = child.GetComponent<MoveToDescriptor>();
			if (component != null)
			{
				QuickDestination quickDestination = new QuickDestination();
				quickDestination.goToOnSpawn.theObject = FindObjectAt(component.Parameters.mDestination);
				quickDestination.takeCover = false;
				quickDestination.holdCover = false;
				quickDestination.gait = component.Parameters.mMovementStyle;
				list.Add(quickDestination);
				if (component.Parameters.coverCluster != null)
				{
					quickDestination.coverCluster.theObject = component.Parameters.coverCluster.gameObject;
				}
				continue;
			}
			MoveToCoverDescriptor component2 = child.GetComponent<MoveToCoverDescriptor>();
			if (component2 != null)
			{
				component2.ResolveGuidLinks();
				QuickDestination quickDestination = new QuickDestination();
				quickDestination.goToOnSpawn.theObject = FindObjectAt(component2.Parameters.mDestination);
				quickDestination.takeCover = true;
				quickDestination.holdCover = component2.Parameters.holdCoverWhenBored;
				quickDestination.gait = component2.Parameters.mMovementStyle;
				if (component2.Parameters.coverCluster != null)
				{
					quickDestination.coverCluster.theObject = component2.Parameters.coverCluster.gameObject;
				}
				list.Add(quickDestination);
			}
		}
		m_OverrideData.quickDestinations = list.ToArray();
	}

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_OverrideData.ResolveGuidLinks();
		SpawnerDoor spawnerDoor = cont.FindComponentOfType(typeof(SpawnerDoor)) as SpawnerDoor;
		WorldHelper.KillNamedChildren(base.gameObject, "Automated");
		if (spawnerDoor != null && m_OverrideData.quickDestinations != null && m_OverrideData.quickDestinations.GetLength(0) > 0)
		{
			m_OverrideData.Routines = new List<RoutineDescriptor>();
			for (int i = 0; i < m_OverrideData.quickDestinations.GetLength(0); i++)
			{
				RoutineDescriptor routineDescriptor = new GameObject("Automated").AddComponent<RoutineDescriptor>();
				TaskDescriptor taskDescriptor = m_OverrideData.quickDestinations[i].GenerateRoutine(ref routineDescriptor.m_Interface, routineDescriptor.gameObject, m_OverrideData.AssaultParameters);
				if (taskDescriptor != null)
				{
					routineDescriptor.m_Interface.TaskListObject = routineDescriptor.gameObject;
					routineDescriptor.Tasks = new List<TaskDescriptor>();
					routineDescriptor.Tasks.Add(taskDescriptor);
					m_OverrideData.Routines.Add(routineDescriptor);
					routineDescriptor.transform.parent = base.transform;
					routineDescriptor.transform.localPosition = Vector3.zero;
				}
				else if (Application.isPlaying)
				{
					Object.Destroy(routineDescriptor.gameObject);
				}
				else
				{
					Object.DestroyImmediate(routineDescriptor.gameObject);
				}
			}
		}
		if (spawnerDoor != null)
		{
			spawnerDoor.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(spawnerDoor);
		}
		MeshSwitchOverride meshSwitchOverride = cont.FindComponentOfType(typeof(MeshSwitchOverride)) as MeshSwitchOverride;
		if (meshSwitchOverride != null)
		{
			meshSwitchOverride.m_Interface.DoorType = m_OverrideData.DoorType;
			Container container = meshSwitchOverride.gameObject.GetComponent(typeof(Container)) as Container;
			container.ApplyOverride();
			spawnerDoor.PhysicalDoor = container.GetInnerObject();
		}
		if (TutorialEnemy && GetComponent<TutorialEnemy>() == null)
		{
			base.gameObject.AddComponent<TutorialEnemy>();
		}
		else if (!TutorialEnemy && GetComponent<TutorialEnemy>() != null)
		{
			Object.DestroyImmediate(GetComponent<TutorialEnemy>());
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SpawnerDoor componentInChildren = gameObj.GetComponentInChildren<SpawnerDoor>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void SendOverrideMessageWithParam(GameObject gameObj, string methodName, GameObject obj)
	{
		base.SendOverrideMessageWithParam(gameObj, methodName, obj);
		SpawnerDoor componentInChildren = gameObj.GetComponentInChildren<SpawnerDoor>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName, obj);
		}
	}

	public override void CreateAssociatedDefaultObjects(Container cont)
	{
		ContainerOverride containerOverride = cont.gameObject.GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		m_OverrideData.EventsList = containerOverride.CreateNewBagObject("Script/Character/Behaviours/Character Event Listeners", "Event Listeners");
	}

	public override void HookUpVisibleConnections(Container cont)
	{
		VisibleConnections visibleConnections = cont.GetComponent<VisibleConnections>() ?? cont.gameObject.AddComponent<VisibleConnections>();
		visibleConnections.lineProperties.Clear();
		Color color = new Color(1f, 0f, 0f, 1f);
		if (m_OverrideData.NotifyOnAllDead != null)
		{
			GameObject notifyOnAllDead = m_OverrideData.NotifyOnAllDead;
			LineFlag inFlag = LineFlag.Out;
			Transform inTrans = notifyOnAllDead.transform;
			Color inColor = color;
			LineDetail item = new LineDetail(inFlag, inTrans, inColor);
			visibleConnections.lineProperties.Add(item);
		}
	}
}
