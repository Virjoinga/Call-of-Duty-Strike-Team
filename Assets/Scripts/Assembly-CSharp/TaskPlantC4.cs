public class TaskPlantC4 : Task
{
	private ExplodableObject mTarget;

	public TaskPlantC4(TaskManager owner, TaskManager.Priority priority, Config flags, ExplodableObject target)
		: base(owner, priority, flags)
	{
		mTarget = target;
		if (mTarget != null)
		{
			mTarget.ArmedBy = mActor;
		}
	}

	public override void Update()
	{
	}

	public override bool HasFinished()
	{
		return true;
	}

	public override void Finish()
	{
		base.Finish();
		if (mTarget != null && !mTarget.IsArmed)
		{
			mTarget.ArmedBy = null;
		}
		if (mActor.tasks.IsRunningTask<TaskSetPiece>())
		{
			mActor.tasks.CancelTasks<TaskSetPiece>();
		}
	}
}
