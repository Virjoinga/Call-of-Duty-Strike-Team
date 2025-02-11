using System.Collections.Generic;

public class TaskSequence : Task
{
	private TaskDescriptor[] mTasks;

	private int mCurrentTaskIndex;

	public TaskSequence(TaskManager owner, TaskManager.Priority priority, Config flags, ICollection<TaskDescriptor> tasks)
		: base(owner, priority, flags)
	{
		mTasks = new TaskDescriptor[tasks.Count];
		tasks.CopyTo(mTasks, 0);
		mCurrentTaskIndex = -1;
	}

	public override void Update()
	{
		if (mTasks.Length != 0)
		{
			mCurrentTaskIndex++;
			if (mCurrentTaskIndex < mTasks.Length)
			{
				mTasks[mCurrentTaskIndex].CreateTask(base.Owner, base.Priority, Config.ClearAllCurrentType);
			}
		}
	}

	public override bool HasFinished()
	{
		return mCurrentTaskIndex >= mTasks.Length;
	}
}
