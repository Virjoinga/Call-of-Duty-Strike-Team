public class TaskPatrol : Task
{
	private PatrolRoute mPatrolRoute;

	private BaseCharacter.MovementStyle mMovementStyle;

	private int mCurrentPatrolPointIndex;

	private TaskManager.Priority mPriority;

	public TaskPatrol(TaskManager owner, TaskManager.Priority priority, Config flags, PatrolRoute patrolRoute, BaseCharacter.MovementStyle movementStyle)
		: base(owner, priority, flags)
	{
		mPriority = priority;
		mPatrolRoute = patrolRoute;
		mMovementStyle = movementStyle;
		mCurrentPatrolPointIndex = -1;
	}

	public override void Update()
	{
		if (mPatrolRoute.PatrolPoints.Count != 0 && !mOwner.IsRunningTask(typeof(TaskRouteTo)))
		{
			if (mCurrentPatrolPointIndex != -1)
			{
				new TaskWait(mOwner, mPriority, Config.ClearAllCurrentType, 2f);
			}
			mCurrentPatrolPointIndex++;
			if (mCurrentPatrolPointIndex >= mPatrolRoute.PatrolPoints.Count)
			{
				mCurrentPatrolPointIndex = 0;
			}
			InheritableMovementParams moveParams = new InheritableMovementParams(mMovementStyle, mPatrolRoute.PatrolPoints[mCurrentPatrolPointIndex].position);
			new TaskRouteTo(mOwner, mPriority, Config.ClearAllCurrentType | Config.AbortOnAlert, moveParams);
		}
	}

	public override bool HasFinished()
	{
		if (mPatrolRoute.PatrolPoints.Count == 0)
		{
			return true;
		}
		return mActor.behaviour.InActiveAlertState();
	}
}
