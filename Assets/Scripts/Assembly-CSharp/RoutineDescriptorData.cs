using System;
using UnityEngine;

[Serializable]
public class RoutineDescriptorData
{
	public GameObject TaskListObject;

	public GameObject AlertedRoutineObject;

	public bool NoneCombatAI;

	public bool OneShotRoutineTasks;

	public bool PingPongRoutineOrdering;

	public GuidRef Magnet = new GuidRef();

	public void CopyContainerData(RoutineDescriptor gt)
	{
		gt.Tasks.Clear();
		if (TaskListObject != null)
		{
			RoutineDescriptor componentInChildren = TaskListObject.GetComponentInChildren<RoutineDescriptor>();
			if (componentInChildren != null)
			{
				gt.Tasks.AddRange(componentInChildren.Tasks);
			}
			else
			{
				gt.Tasks.AddRange(TaskListObject.GetComponentsInChildren<TaskDescriptor>());
			}
		}
		foreach (TaskDescriptor task in gt.Tasks)
		{
			task.ResolveGuidLinks();
		}
	}
}
