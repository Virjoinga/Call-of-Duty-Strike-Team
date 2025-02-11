public class RiotShieldDescriptor : TaskDescriptor
{
	public RiotShieldDescriptorConfig SpawnConfig;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		Task.Config config = flags | ConfigFlags.GetAsFlags();
		config |= Task.Config.ClearAllCurrentType;
		return new TaskRiotShield(owner, priority, config, SpawnConfig);
	}
}
