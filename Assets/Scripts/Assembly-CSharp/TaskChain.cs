public class TaskChain
{
	public Task ChainedTask;

	public Task Parent;

	public TaskChain(Task chainedTask, Task parentTask)
	{
		ChainedTask = chainedTask;
		Parent = parentTask;
	}
}
