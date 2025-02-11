public class OpenDoorDescriptor : TaskDescriptor
{
	public BuildingDoor Door;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskOpen>();
			return null;
		}
		return new TaskOpen(owner, priority, flags | ConfigFlags.GetAsFlags(), Door);
	}
}
