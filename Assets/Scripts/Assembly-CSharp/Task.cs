using System;
using System.Collections.Generic;

[Serializable]
public abstract class Task
{
	[Flags]
	public enum Config
	{
		Default = 0,
		ClearAllCurrentType = 1,
		Parallel = 2,
		Chain = 4,
		AbortOnAlert = 8,
		AbortOnVisibleEnemy = 0x10,
		AbortOnUnobstructedEnemy = 0x20,
		AbortOnUnobstructedTarget = 0x40,
		IssuedByPlayerRequest = 0x80,
		DenyPlayerInput = 0x100,
		AbortOnAllEnemiesDead = 0x200,
		AbortWhenInRangeOfTarget = 0x400,
		AbortIfSpotted = 0x800,
		ClearCurrentTypeEqualOrHigher = 0x1000,
		ConsultParent = 0x2000,
		AbortIfDestinationBlocked = 0x4000
	}

	protected TaskManager mOwner;

	private GameObjectBroadcaster mBroadcastOnCompletion;

	private TaskManager.Priority mPriority;

	protected Config mConfigFlags;

	protected Actor mActor;

	protected Actor mAbortTarget;

	public TaskSynchroniser mTaskSync;

	public Task mConsultant;

	public ActionComponent.ActionType TaskActionType = ActionComponent.ActionType.Ignore;

	public TaskManager Owner
	{
		get
		{
			return mOwner;
		}
	}

	public TaskManager.Priority Priority
	{
		get
		{
			return mPriority;
		}
	}

	public Config ConfigFlags
	{
		get
		{
			return mConfigFlags;
		}
		set
		{
			mConfigFlags |= value;
		}
	}

	public GameObjectBroadcaster BroadcastOnCompletion
	{
		get
		{
			return mBroadcastOnCompletion;
		}
		set
		{
			mBroadcastOnCompletion = value;
		}
	}

	public Actor AbortTarget
	{
		get
		{
			return mAbortTarget;
		}
		set
		{
			mAbortTarget = value;
		}
	}

	public Task(TaskManager owner, TaskManager.Priority priority, Config flags)
	{
		if ((flags & Config.ClearAllCurrentType) != 0)
		{
			RemoveTasksOfType(owner.Reactive.Stack);
			RemoveTasksOfType(owner.Immediate.Stack);
			RemoveTasksOfType(owner.LongTerm.Stack);
		}
		else if ((flags & Config.ClearCurrentTypeEqualOrHigher) != 0)
		{
			if (priority >= TaskManager.Priority.LONG_TERM)
			{
				RemoveTasksOfType(owner.LongTerm.Stack);
			}
			if (priority >= TaskManager.Priority.IMMEDIATE)
			{
				RemoveTasksOfType(owner.Immediate.Stack);
			}
			if (priority >= TaskManager.Priority.REACTIVE)
			{
				RemoveTasksOfType(owner.Reactive.Stack);
			}
		}
		mOwner = owner;
		mActor = mOwner.GetComponent<Actor>();
		mPriority = priority;
		mConfigFlags = flags & ~Config.ConsultParent;
		if (mOwner.UpdatingTask != null)
		{
			if ((flags & Config.ConsultParent) != 0)
			{
				mConsultant = mOwner.UpdatingTask;
			}
			else
			{
				mConsultant = mOwner.UpdatingTask.mConsultant;
			}
		}
		owner.StackMe(this);
	}

	public abstract void Update();

	public abstract bool HasFinished();

	public virtual void Finish()
	{
	}

	public virtual void Destroy()
	{
	}

	public virtual void OnSleep()
	{
	}

	public virtual void OnResume()
	{
	}

	private void RemoveTasksOfType(List<Task> taskStack)
	{
		int num = 0;
		while (RemoveTaskOfType(taskStack) && num < 10)
		{
			num++;
		}
		TBFAssert.DoAssert(num < 10, "Task System is state-thrashing!");
	}

	private bool RemoveTaskOfType(List<Task> taskStack)
	{
		for (int num = taskStack.Count - 1; num >= 0; num--)
		{
			if (taskStack[num].GetType() == GetType())
			{
				Task task = taskStack[num];
				taskStack.RemoveAt(num);
				task.Finish();
				return true;
			}
		}
		return false;
	}

	public bool CheckConfigFlagsFinished()
	{
		if (mConsultant != null && mConsultant.Consult(this))
		{
			return true;
		}
		if ((ConfigFlags & Config.AbortOnAlert) != 0 && mActor.behaviour.InActiveAlertState())
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "Alert");
			return true;
		}
		if ((ConfigFlags & Config.AbortOnVisibleEnemy) != 0 && mActor.awareness.closestVisibleEnemy != null)
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "Enemy");
			return true;
		}
		if ((ConfigFlags & Config.AbortOnUnobstructedEnemy) != 0 && (GKM.InCrowdOf(mActor).obstructed & GKM.EnemiesMask(mActor)) != 0)
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "UnobEnemy");
			return true;
		}
		if (mAbortTarget != null && (ConfigFlags & Config.AbortOnUnobstructedTarget) != 0 && (mActor.awareness.ObstructedMask() & mAbortTarget.ident) == 0)
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "UnobTarget");
			return true;
		}
		if ((ConfigFlags & Config.AbortIfSpotted) != 0)
		{
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(mActor.awareness.EnemiesWhoCanSeeMe());
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				if (a.behaviour.InActiveAlertState())
				{
					return true;
				}
			}
		}
		if ((ConfigFlags & Config.AbortOnAllEnemiesDead) != 0 && (GKM.EnemiesMask(mActor) & GKM.AliveMask) == 0)
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "AbortEnemiesDead");
			return true;
		}
		return false;
	}

	public virtual void Command(string com)
	{
	}

	protected bool CanContinue()
	{
		return mTaskSync.CanContinue(this);
	}

	protected bool WaitForSync(TaskSynchroniser.SyncState s)
	{
		return mTaskSync.WaitForSync(this, s);
	}

	protected void SyncDone()
	{
		mTaskSync.WaitForSync(this, TaskSynchroniser.SyncState.Done);
	}

	protected void SyncChildTask(Task t)
	{
		t.mTaskSync.TaskFinished(t);
		mTaskSync.AddTask(t);
		t.mTaskSync = mTaskSync;
	}

	public virtual bool Consult(Task child)
	{
		return false;
	}
}
