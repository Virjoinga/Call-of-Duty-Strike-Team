using System;
using UnityEngine;

public class TaskDefendFirstPerson : Task
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
		MoveIntoCover = 1,
		MovingIntoCover = 2,
		Covering = 3
	}

	private const float kInnerExclusionSearchRadius = 0.5f;

	private const float kPositionOffsetRange = 1f;

	private const float kFollowTouchDist = 1.5f;

	private const float kMaxTimeInView = 5f;

	private Flags mFlags;

	private State mState;

	private Actor mTarget;

	private Vector3 quadrantVariance;

	private bool mMoveIntoNewCoverOrdered;

	private CoverPointCore mNextCoverPoint;

	private Vector3 mTargetPosition;

	private float mInViewTimer;

	public Actor Target
	{
		get
		{
			return mTarget;
		}
	}

	public TaskDefendFirstPerson(TaskManager owner, TaskManager.Priority priority, Actor target, Config flags)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mFlags = Flags.Default;
		mState = State.Start;
		mMoveIntoNewCoverOrdered = false;
		mNextCoverPoint = null;
		mInViewTimer = 0f;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			mMoveIntoNewCoverOrdered = true;
			if (!ProcessMoveIntoPosition())
			{
				mState = State.MoveIntoCover;
			}
			mState = State.MoveIntoCover;
			break;
		case State.MoveIntoCover:
			MoveIntoCover();
			break;
		case State.MovingIntoCover:
			MovingIntoCover();
			break;
		case State.Covering:
			CoverTarget();
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.Invalidated) != 0)
		{
			return true;
		}
		if (mState == State.Start || mState == State.MoveIntoCover)
		{
			return false;
		}
		return false;
	}

	private bool ProcessMoveIntoPosition()
	{
		if (mActor == null || mTarget == null)
		{
			return false;
		}
		if (!mActor.awareness.CanSee(mTarget))
		{
			mState = State.MoveIntoCover;
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Run, mTarget.GetPosition());
			TaskRouteTo taskRouteTo = new TaskRouteTo(base.Owner, base.Priority, Config.ClearAllCurrentType | Config.AbortOnUnobstructedTarget, moveParams);
			taskRouteTo.AbortTarget = mTarget;
			return true;
		}
		return false;
	}

	private void MoveIntoCover()
	{
		if (!mMoveIntoNewCoverOrdered || !(mNextCoverPoint == null))
		{
			return;
		}
		mNextCoverPoint = mActor.awareness.GetDefensiveCoverAvoidingFirstPersonCam(mTarget.GetPosition(), 20f, 0.5f, true);
		if (mNextCoverPoint != null)
		{
			if (mNextCoverPoint.Available(mActor))
			{
				mActor.awareness.BookCoverPoint(mNextCoverPoint, 0f);
				InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Run);
				TaskMoveToCover taskMoveToCover = new TaskMoveToCover(mOwner, base.Priority, Config.ClearAllCurrentType | Config.AbortOnVisibleEnemy, mNextCoverPoint, moveParams);
				taskMoveToCover.PassiveTimeLimit = 3f;
				mState = State.MovingIntoCover;
			}
			else
			{
				mMoveIntoNewCoverOrdered = true;
				mNextCoverPoint = null;
				mMoveIntoNewCoverOrdered = true;
			}
		}
		else
		{
			Vector3 vector = -mTarget.realCharacter.FirstPersonCamera.transform.forward;
			vector *= 2f;
			Vector3 vector2 = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
			mTargetPosition = mTarget.realCharacter.TargetPosition + vector + vector2;
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(mTarget.realCharacter.MovementStyleActive, mTargetPosition);
			inheritableMovementParams.DestinationThreshold = 1.5f;
			new TaskRouteTo(base.Owner, base.Priority, Config.Default, inheritableMovementParams);
			mState = State.Covering;
		}
	}

	private void MovingIntoCover()
	{
		if (mActor.awareness.IsInCover())
		{
			mState = State.Covering;
		}
	}

	private void CoverTarget()
	{
		if (mInViewTimer > 5f)
		{
			mActor.realCharacter.ForceCrouch = false;
			mState = State.MoveIntoCover;
			mMoveIntoNewCoverOrdered = true;
			mNextCoverPoint = null;
			mInViewTimer = 0f;
		}
		Vector3 lhs = mActor.GetPosition() - mTarget.GetPosition();
		lhs.Normalize();
		float num = Vector3.Dot(lhs, mTarget.realCharacter.FirstPersonCamera.transform.forward);
		if (num > 0.75f && num < 1.25f)
		{
			mActor.realCharacter.ForceCrouch = true;
			mInViewTimer += Time.deltaTime;
		}
		else
		{
			mActor.realCharacter.ForceCrouch = false;
			mInViewTimer = 0f;
		}
	}

	private bool IsAutomatedBehaviourAllowed(Actor target)
	{
		if (mActor.realCharacter == null)
		{
			return false;
		}
		if (!mActor.behaviour.PlayerControlled)
		{
			return true;
		}
		if (target.behaviour.InActiveAlertState())
		{
			return true;
		}
		return false;
	}
}
