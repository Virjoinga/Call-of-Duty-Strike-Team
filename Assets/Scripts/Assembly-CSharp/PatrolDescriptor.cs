public class PatrolDescriptor : TaskDescriptor
{
	public PatrolRoute Route;

	public BaseCharacter.MovementStyle MovementStyle;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskPatrol>();
			return null;
		}
		return new TaskPatrol(owner, priority, flags | ConfigFlags.GetAsFlags(), Route, MovementStyle);
	}
}
