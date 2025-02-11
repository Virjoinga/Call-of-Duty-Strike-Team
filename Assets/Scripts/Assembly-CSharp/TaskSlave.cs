public class TaskSlave : Task
{
	private bool ready = true;

	private bool cancelled;

	public bool Ready
	{
		get
		{
			return ready;
		}
	}

	public TaskSlave(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
	}

	public override void Update()
	{
	}

	public override bool HasFinished()
	{
		return cancelled;
	}

	public override void OnSleep()
	{
		ready = false;
	}

	public override void OnResume()
	{
		ready = true;
	}

	public void Cancel()
	{
		cancelled = true;
	}
}
