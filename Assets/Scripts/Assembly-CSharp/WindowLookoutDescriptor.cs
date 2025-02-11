public class WindowLookoutDescriptor : TaskDescriptor
{
	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskSentryGun>();
			return null;
		}
		priority = TaskManager.Priority.LONG_TERM;
		return new TaskWindowLookout(owner, priority, Task.Config.Default);
	}
}
