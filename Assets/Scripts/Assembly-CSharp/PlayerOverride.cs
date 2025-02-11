using UnityEngine;

public class PlayerOverride : ContainerOverride
{
	public RoutineDescriptorData m_RoutineOverrideData = new RoutineDescriptorData();

	public SpawnerData m_SpawnerOverrideData = new SpawnerData();

	public bool DontSelectOnSpawn;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_SpawnerOverrideData.ResolveGuidLinks();
		WorldHelper.KillNamedChildren(base.gameObject, "Automated");
		if (m_SpawnerOverrideData.quickDestination != null)
		{
			GameObject gameObject = new GameObject("Automated");
			TaskDescriptor taskDescriptor = m_SpawnerOverrideData.quickDestination.GenerateRoutine(ref m_RoutineOverrideData, gameObject, m_SpawnerOverrideData.AssaultParameters);
			if (taskDescriptor != null)
			{
				m_RoutineOverrideData.TaskListObject = gameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
			}
			else if (Application.isPlaying)
			{
				gameObject.transform.parent = null;
				Object.Destroy(gameObject.gameObject);
			}
			else
			{
				Object.DestroyImmediate(gameObject.gameObject);
			}
		}
		RoutineDescriptor routineDescriptor = cont.FindComponentOfType(typeof(RoutineDescriptor)) as RoutineDescriptor;
		if (routineDescriptor != null)
		{
			routineDescriptor.m_Interface = m_RoutineOverrideData;
			m_RoutineOverrideData.CopyContainerData(routineDescriptor);
		}
		Spawner spawner = cont.FindComponentOfType(typeof(Spawner)) as Spawner;
		if (spawner != null)
		{
			m_SpawnerOverrideData.EntityType = ((m_SpawnerOverrideData.EntityType != null && m_SpawnerOverrideData.EntityType.Length != 0) ? m_SpawnerOverrideData.EntityType : spawner.m_Interface.EntityType);
			spawner.m_Interface = m_SpawnerOverrideData;
			m_SpawnerOverrideData.CopyContainerData(spawner);
			spawner.DontSelectOnSpawn = DontSelectOnSpawn;
		}
	}

	public override void CreateAssociatedDefaultObjects(Container cont)
	{
		ContainerOverride containerOverride = cont.gameObject.GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		m_SpawnerOverrideData.EventsList = containerOverride.CreateNewBagObject("Script/Character/Behaviours/Character Event Listeners", "Event Listeners");
		EventsCreator component = m_SpawnerOverrideData.EventsList.GetComponent<EventsCreator>();
		if (component == null)
		{
			component = m_SpawnerOverrideData.EventsList.AddComponent<EventsCreator>();
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		Spawner componentInChildren = gameObj.GetComponentInChildren<Spawner>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void SendOverrideMessageWithParam(GameObject gameObj, string methodName, GameObject param)
	{
		base.SendOverrideMessageWithParam(gameObj, methodName, param);
		Spawner componentInChildren = gameObj.GetComponentInChildren<Spawner>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName, param);
		}
	}

	public override void SendOverrideMessageWithParam(GameObject gameObj, string methodName, string param)
	{
		base.SendOverrideMessageWithParam(gameObj, methodName, param);
		Spawner componentInChildren = gameObj.GetComponentInChildren<Spawner>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName, param);
		}
	}
}
