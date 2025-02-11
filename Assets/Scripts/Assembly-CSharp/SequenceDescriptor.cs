using System.Collections.Generic;

public class SequenceDescriptor : TaskDescriptor
{
	public List<TaskDescriptor> Tasks;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		return new TaskSequence(owner, priority, flags | ConfigFlags.GetAsFlags(), Tasks);
	}
}
