using System;
using UnityEngine;

public class TaskFollow : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		Invalidated = 1
	}

	private enum State
	{
		Start = 0,
		MoveToLOS = 1,
		Following = 2
	}

	private const float kFollowTouchDist = 2f;

	private const float kAdditionalFollowTouchDist = 0.25f;

	private const float kMaxMovement = 5f;

	private Flags mFlags;

	private State mState;

	private Actor mTarget;

	private Vector3 mPreviousTargetPosition;

	private int mFollowId;

	private float mFollowTouchDist;

	public Actor Target
	{
		get
		{
			return mTarget;
		}
	}

	public TaskFollow(TaskManager owner, TaskManager.Priority priority, Actor target, Config flags, float distanceFromTargetToStop)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mFlags = Flags.Default;
		mState = State.Start;
		mPreviousTargetPosition = mActor.GetPosition();
		mFollowId = ++mTarget.realCharacter.NumberOfFollowers;
		mFollowTouchDist = distanceFromTargetToStop;
	}

	public TaskFollow(TaskManager owner, TaskManager.Priority priority, Actor target, Config flags)
		: this(owner, priority, target, flags, 2f)
	{
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			if (!ProcessMoveIntoPosition())
			{
				mState = State.Following;
			}
			break;
		case State.MoveToLOS:
			if (!base.Owner.IsRunningTask(typeof(TaskMoveTo)))
			{
				mState = State.Following;
			}
			break;
		case State.Following:
			FollowTarget();
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.Invalidated) != 0)
		{
			mTarget.realCharacter.NumberOfFollowers--;
			return true;
		}
		if ((base.ConfigFlags & Config.AbortWhenInRangeOfTarget) != 0 && (mTarget.GetPosition() - mActor.GetPosition()).magnitude <= mFollowTouchDist)
		{
			mTarget.realCharacter.NumberOfFollowers--;
			return true;
		}
		if ((base.ConfigFlags & Config.AbortIfSpotted) != 0 && mActor.awareness.CanAnyEnemiesSeeMe())
		{
			mTarget.realCharacter.NumberOfFollowers--;
			return true;
		}
		if (mTarget != null && mTarget.realCharacter.IsDead() && mActor.awareness.CanSee(mTarget))
		{
			mTarget.realCharacter.NumberOfFollowers--;
			return true;
		}
		if (mState == State.Start || mState == State.MoveToLOS)
		{
			return false;
		}
		return false;
	}

	private bool IsTargetValid()
	{
		if (mActor.realCharacter == null)
		{
			return true;
		}
		if (mTarget == null)
		{
			return false;
		}
		if (mTarget.realCharacter.IsDead())
		{
			return false;
		}
		return true;
	}

	private bool ProcessMoveIntoPosition()
	{
		if (mActor == null || mTarget == null)
		{
			return false;
		}
		if (!mActor.awareness.CanSee(mTarget))
		{
			mState = State.MoveToLOS;
			BaseCharacter.MovementStyle ms = BaseCharacter.MovementStyle.Walk;
			if (mTarget.behaviour.InActiveAlertState())
			{
				ms = BaseCharacter.MovementStyle.Run;
			}
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(ms, mTarget.GetPosition());
			float num = mFollowTouchDist;
			if ((base.ConfigFlags & Config.AbortIfSpotted) != 0)
			{
				num *= 0.5f;
			}
			inheritableMovementParams.DestinationThreshold = num;
			new TaskRouteTo(base.Owner, base.Priority, Config.ClearAllCurrentType | Config.AbortOnUnobstructedTarget, inheritableMovementParams);
			return true;
		}
		return false;
	}

	private void FollowTarget()
	{
		if (mPreviousTargetPosition != mTarget.GetPosition())
		{
			Vector3 vector = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
			Vector3 vector2 = mTarget.GetPosition();
			float magnitude = (mActor.GetPosition() - mTarget.GetPosition()).magnitude;
			Vector3 vector3 = (mTarget.GetPosition() - mActor.GetPosition()) / magnitude * 5f;
			float magnitude2 = vector3.magnitude;
			if (magnitude > magnitude2)
			{
				vector2 = mActor.GetPosition() + vector3;
			}
			Vector3 destination = vector2 + vector;
			float destinationThreshold = mFollowTouchDist + 0.25f * (float)mFollowId;
			if ((base.ConfigFlags & Config.AbortIfSpotted) != 0)
			{
				destination = vector2;
				destinationThreshold = mFollowTouchDist * 0.5f;
			}
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(mTarget.realCharacter.MovementStyleActive, destination);
			inheritableMovementParams.DestinationThreshold = destinationThreshold;
			new TaskRouteTo(base.Owner, base.Priority, Config.Default, inheritableMovementParams);
			mPreviousTargetPosition = mTarget.GetPosition();
		}
	}
}
