using System.Collections.Generic;

public class TaskOverrideRoutine : Task
{
	private bool oneShotCompleted;

	private TaskDescriptor[] mTasks;

	private int mCurrentTaskIndex;

	public TaskOverrideRoutine(TaskManager owner, TaskManager.Priority priority, Config flags, ICollection<TaskDescriptor> tasks, bool noneCombatAI)
		: base(owner, priority, flags)
	{
		mTasks = new TaskDescriptor[tasks.Count];
		tasks.CopyTo(mTasks, 0);
		mCurrentTaskIndex = -1;
		if (!(mActor.fireAtWill == null))
		{
			if (noneCombatAI)
			{
				mActor.fireAtWill.Enabled = false;
			}
			else
			{
				mActor.fireAtWill.Enabled = true;
			}
		}
	}

	private void LogRoutineDebug(Task t)
	{
	}

	public override void Update()
	{
		if (mTasks.Length == 0)
		{
			oneShotCompleted = true;
			return;
		}
		mCurrentTaskIndex++;
		if (mCurrentTaskIndex >= mTasks.Length)
		{
			mCurrentTaskIndex = 0;
			mTasks = new TaskDescriptor[0];
			oneShotCompleted = true;
			return;
		}
		Task task = mTasks[mCurrentTaskIndex].CreateTask(base.Owner, base.Priority, Config.ClearAllCurrentType);
		LogRoutineDebug(task);
		if (task != null && task is TaskPlayAnimation)
		{
			int num = mCurrentTaskIndex;
			num++;
			if (num < mTasks.Length && mTasks[num] is PlayAnimationDescriptor)
			{
				TaskPlayAnimation taskPlayAnimation = task as TaskPlayAnimation;
				taskPlayAnimation.PartOfSequence = true;
			}
		}
	}

	public override bool HasFinished()
	{
		return oneShotCompleted;
	}
}
