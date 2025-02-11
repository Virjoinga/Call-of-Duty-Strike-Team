public class CloseDoorDescriptor : TaskDescriptor
{
	public BuildingDoor Door;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskClose>();
			return null;
		}
		return new TaskClose(owner, priority, flags | ConfigFlags.GetAsFlags(), Door);
	}
}
