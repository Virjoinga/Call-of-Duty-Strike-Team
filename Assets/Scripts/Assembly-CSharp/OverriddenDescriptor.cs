public class OverriddenDescriptor : TaskDescriptor
{
	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskOverridden>();
			return null;
		}
		return new TaskOverridden(owner, priority, flags | ConfigFlags.GetAsFlags());
	}
}
