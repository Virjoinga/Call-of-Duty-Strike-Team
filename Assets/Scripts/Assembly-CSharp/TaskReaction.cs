using System;
using UnityEngine;

public class TaskReaction : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		SpottedEnemy = 1
	}

	private enum State
	{
		Start = 0,
		MovingToTarget = 1,
		Loiter = 2,
		SearchArea = 3,
		Searching = 4,
		Complete = 5
	}

	private const float kMAX_LOITER_TIME = 5f;

	private const float kMIN_LOITER_TIME = 2f;

	private Flags mFlags;

	private State mState;

	private Actor mTarget;

	private Vector3 mStartSearchPosition;

	private bool mInteruptSearch;

	private float mCurrentLoiterTime;

	private float mMaxLoiterTime = 5f;

	public bool Interrupt
	{
		get
		{
			return mInteruptSearch;
		}
	}

	public TaskReaction(TaskManager owner, Actor target, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mState = State.Start;
		mFlags = Flags.Default;
		mTarget = target;
		mInteruptSearch = false;
	}

	public void OverrideSearch(Actor newTarget)
	{
		mTarget = newTarget;
		if (mState == State.Searching)
		{
			mActor.tasks.CancelTasks(typeof(TaskMoveTo));
			mActor.tasks.CancelTasks(typeof(TaskRouteTo));
			mState = State.Start;
		}
		mInteruptSearch = true;
	}

	public override void Update()
	{
		float distanceSquared;
		Actor nearestVisibleEnemy = mActor.awareness.GetNearestVisibleEnemy(out distanceSquared);
		if (nearestVisibleEnemy != null && !nearestVisibleEnemy.realCharacter.IsDead())
		{
			mFlags |= Flags.SpottedEnemy;
			return;
		}
		if (mInteruptSearch && mState == State.SearchArea)
		{
			mState = State.Start;
		}
		switch (mState)
		{
		case State.Start:
			base.Owner.CancelTasks(typeof(TaskRouteTo), base.Priority);
			base.Owner.CancelTasks(typeof(TaskMoveTo), base.Priority);
			mInteruptSearch = false;
			GetValidStartPosition();
			mState = State.MovingToTarget;
			break;
		case State.MovingToTarget:
			if (!base.Owner.IsRunningTask(typeof(TaskRouteTo)))
			{
				MoveToTarget();
				mCurrentLoiterTime = 0f;
				mMaxLoiterTime = UnityEngine.Random.Range(2f, 5f);
				mState = State.Loiter;
			}
			break;
		case State.Loiter:
			if (LoiterTimeExpired())
			{
				mState = State.SearchArea;
			}
			break;
		case State.SearchArea:
			StartSearch();
			mState = State.Searching;
			break;
		case State.Searching:
			if (!base.Owner.IsRunningTask(typeof(TaskRouteTo)))
			{
				mState = State.Complete;
			}
			break;
		case State.Complete:
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.SpottedEnemy) != 0)
		{
			return true;
		}
		State state = mState;
		if (state == State.Complete)
		{
			return true;
		}
		return false;
	}

	public override void Finish()
	{
	}

	private void GetValidStartPosition()
	{
		if (mTarget != null)
		{
			mStartSearchPosition = mTarget.GetPosition();
			if (mActor.tether.IsDestinationWithinTether(mStartSearchPosition))
			{
				return;
			}
		}
		mStartSearchPosition = mActor.GetPosition();
	}

	private void MoveToTarget()
	{
		BaseCharacter.MovementStyle ms = BaseCharacter.MovementStyle.Run;
		InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(ms, mStartSearchPosition);
		inheritableMovementParams.FinalLookAt = mStartSearchPosition;
		inheritableMovementParams.DestinationThreshold = 3f;
		new TaskRouteTo(mOwner, TaskManager.Priority.LONG_TERM, Config.ClearAllCurrentType | Config.AbortOnVisibleEnemy, inheritableMovementParams);
	}

	private bool LoiterTimeExpired()
	{
		mCurrentLoiterTime += Time.deltaTime;
		if (mCurrentLoiterTime > mMaxLoiterTime)
		{
			return true;
		}
		return false;
	}

	private void StartSearch()
	{
		new TaskFlushOutEnemies(mActor.tasks, TaskManager.Priority.LONG_TERM, Config.Default, this);
	}
}
