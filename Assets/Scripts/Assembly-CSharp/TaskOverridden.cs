public class TaskOverridden : Task
{
	public TaskOverridden(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
	}

	public override void Update()
	{
	}

	public override bool HasFinished()
	{
		return false;
	}
}
