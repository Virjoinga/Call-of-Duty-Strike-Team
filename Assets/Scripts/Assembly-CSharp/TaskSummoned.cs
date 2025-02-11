public class TaskSummoned : Task
{
	private Summoner mySummoner;

	public TaskSummoned(TaskManager owner, TaskManager.Priority priority, Config flags, Summoner summoner)
		: base(owner, priority, flags)
	{
		mySummoner = summoner;
	}

	public override void Update()
	{
	}

	public override void Finish()
	{
		if (mySummoner != null)
		{
			mySummoner.SummonTaskFinished(this);
		}
	}

	public override bool HasFinished()
	{
		return true;
	}
}
