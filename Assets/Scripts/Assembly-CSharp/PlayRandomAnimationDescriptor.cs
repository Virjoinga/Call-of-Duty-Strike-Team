using System.Collections.Generic;

public class PlayRandomAnimationDescriptor : TaskDescriptor
{
	public RoutineDescriptor ContainerOfAnimations;

	public List<RandomAnimationDescriptor> RandomAnimationList;

	public int Cycles = -1;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskPlayRandomAnimations>();
			return null;
		}
		return new TaskPlayRandomAnimations(owner, priority, flags | ConfigFlags.GetAsFlags(), RandomAnimationList, Cycles);
	}
}
