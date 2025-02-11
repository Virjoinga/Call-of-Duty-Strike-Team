using UnityEngine;

public class SniperDescriptor : TaskDescriptor
{
	public Transform StartLookAt;

	public SniperOverrideData SniperOverrides;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskSniper>();
			return null;
		}
		if (StartLookAt != null)
		{
			return new TaskSniper(owner, TaskManager.Priority.LONG_TERM, Task.Config.Default, SniperOverrides, StartLookAt.position);
		}
		return new TaskSniper(owner, TaskManager.Priority.LONG_TERM, Task.Config.Default, SniperOverrides);
	}
}
