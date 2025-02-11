using UnityEngine;

public class WaitDescriptor : TaskDescriptor
{
	public float Seconds;

	public Transform StareAt;

	public bool ForceFacing = true;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (StareAt != null)
		{
			if (CancelTasksOfType)
			{
				owner.CancelTasks<TaskStopAndStare>();
				return null;
			}
			return new TaskStopAndStare(owner, priority, flags | ConfigFlags.GetAsFlags(), Seconds, StareAt, ForceFacing);
		}
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskWait>();
			return null;
		}
		return new TaskWait(owner, priority, flags | ConfigFlags.GetAsFlags(), Seconds);
	}
}
