using UnityEngine;

public class OverrideLogicOverride : ContainerOverride
{
	public TaskDesriptOverrideHubData m_OverrideData = new TaskDesriptOverrideHubData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		TaskDescriptorOverrideHub taskDescriptorOverrideHub = cont.FindComponentOfType(typeof(TaskDescriptorOverrideHub)) as TaskDescriptorOverrideHub;
		if (!(taskDescriptorOverrideHub != null))
		{
			return;
		}
		taskDescriptorOverrideHub.m_Interface = m_OverrideData;
		taskDescriptorOverrideHub.EntitiesToOverride.Clear();
		if (m_OverrideData.Actors == null)
		{
			return;
		}
		foreach (ActorOverride actor2 in m_OverrideData.Actors)
		{
			if (actor2 == null)
			{
				continue;
			}
			GameObject actor = actor2.Actor;
			if (actor == null && actor2.contGUID != string.Empty)
			{
				Container containerFromGuid = Container.GetContainerFromGuid(actor2.contGUID);
				if (containerFromGuid != null)
				{
					actor = containerFromGuid.gameObject;
					actor2.Actor = containerFromGuid.gameObject;
				}
			}
			if (actor != null)
			{
				ActorWrapper[] componentsInChildren = actor.GetComponentsInChildren<ActorWrapper>();
				foreach (ActorWrapper item in componentsInChildren)
				{
					taskDescriptorOverrideHub.EntitiesToOverride.Add(item);
				}
				Container component = actor.GetComponent<Container>();
				if (component != null)
				{
					actor2.contGUID = component.m_Guid;
				}
			}
		}
		m_OverrideData.ResolveGuidLinks();
		m_OverrideData.CopyContainerData(taskDescriptorOverrideHub);
		taskDescriptorOverrideHub.TasksToAssign.Clear();
		foreach (GameObject item3 in m_OverrideData.TasksToAssign)
		{
			RoutineDescriptor[] componentsInChildren2 = item3.GetComponentsInChildren<RoutineDescriptor>();
			foreach (RoutineDescriptor item2 in componentsInChildren2)
			{
				taskDescriptorOverrideHub.TasksToAssign.Add(item2);
			}
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		TaskDescriptorOverrideHub componentInChildren = gameObj.GetComponentInChildren<TaskDescriptorOverrideHub>();
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
		if (m_OverrideData.Actors == null || m_OverrideData.Actors.Count == 0)
		{
			return;
		}
		foreach (ActorOverride actor2 in m_OverrideData.Actors)
		{
			if (actor2 != null)
			{
				GameObject actor = actor2.Actor;
				if (actor != null)
				{
					LineFlag inFlag = LineFlag.Out;
					Transform inTrans = actor.transform;
					Color inColor = color;
					LineDetail item = new LineDetail(inFlag, inTrans, inColor);
					visibleConnections.lineProperties.Add(item);
				}
			}
		}
	}
}
