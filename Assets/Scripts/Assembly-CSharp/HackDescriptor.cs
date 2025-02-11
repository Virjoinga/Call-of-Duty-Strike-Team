public class HackDescriptor : TaskDescriptor
{
	public HackableObject Obj;

	public GuidRef ObjRef;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (CancelTasksOfType)
		{
			owner.CancelTasks<TaskThrowGrenade>();
			return null;
		}
		return new TaskHack(owner, priority, flags | ConfigFlags.GetAsFlags(), Obj);
	}
}
