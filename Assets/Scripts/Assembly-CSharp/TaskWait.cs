using UnityEngine;

public class TaskWait : Task
{
	private float mSeconds;

	public TaskWait(TaskManager owner, TaskManager.Priority priority, Config flags, float seconds)
		: base(owner, priority, flags)
	{
		mSeconds = seconds;
	}

	public override void Update()
	{
		mSeconds -= Time.deltaTime;
	}

	public override bool HasFinished()
	{
		if (mActor != null && (base.ConfigFlags & Config.AbortOnAlert) != 0 && mActor.behaviour.InActiveAlertState())
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "Alert");
			return true;
		}
		return mSeconds <= 0f;
	}
}
