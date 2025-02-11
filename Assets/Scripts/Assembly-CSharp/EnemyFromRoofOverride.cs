using System.Collections.Generic;
using UnityEngine;

public class EnemyFromRoofOverride : ContainerOverride
{
	public SpawnerRoofData m_OverrideData = new SpawnerRoofData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_OverrideData.ResolveGuidLinks();
		WorldHelper.KillNamedChildren(base.gameObject, "Automated");
		SpawnerRoof spawnerRoof = cont.FindComponentOfType(typeof(SpawnerRoof)) as SpawnerRoof;
		if (spawnerRoof != null && m_OverrideData.quickDestinations != null && m_OverrideData.quickDestinations.GetLength(0) > 0)
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
		if (spawnerRoof != null)
		{
			spawnerRoof.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(spawnerRoof);
		}
		MeshSwitchOverride meshSwitchOverride = cont.FindComponentOfType(typeof(MeshSwitchOverride)) as MeshSwitchOverride;
		if (meshSwitchOverride != null)
		{
			Container container = meshSwitchOverride.gameObject.GetComponent(typeof(Container)) as Container;
			container.ApplyOverride();
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		SpawnerRoof componentInChildren = gameObj.GetComponentInChildren<SpawnerRoof>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
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
