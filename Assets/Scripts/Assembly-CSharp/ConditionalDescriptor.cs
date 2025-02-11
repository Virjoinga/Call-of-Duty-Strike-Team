using System.Collections.Generic;
using UnityEngine;

public class ConditionalDescriptor : TaskDescriptor
{
	public List<Condition> Conditions = new List<Condition>();

	public GameObject Task;

	public GameObject ElseTask;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (Task != null)
		{
			TaskDescriptor componentInChildren = Task.GetComponentInChildren<TaskDescriptor>();
			if (Conditions.Count > 0)
			{
				foreach (Condition condition in Conditions)
				{
					if (!condition.Value())
					{
						if (!(ElseTask != null))
						{
							return null;
						}
						componentInChildren = ElseTask.GetComponentInChildren<TaskDescriptor>();
					}
				}
				return componentInChildren.CreateTask(owner, priority, flags | ConfigFlags.GetAsFlags());
			}
		}
		else
		{
			Debug.LogWarning("ConditionalDescriptor has null Task");
		}
		return null;
	}
}
