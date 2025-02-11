public class RPGDescriptor : TaskDescriptor
{
	public RPGOverrideData m_Interface;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		Task.Config config = flags | ConfigFlags.GetAsFlags();
		config |= Task.Config.ClearAllCurrentType;
		return new TaskRPG(owner, priority, config, m_Interface);
	}
}
