public class DumbDescriptor : TaskDescriptor
{
	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskDumb>();
			return null;
		}
		return new TaskDumb(owner, priority, flags | ConfigFlags.GetAsFlags());
	}
}
