using System;
using UnityEngine;

public class TaskRepair : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		TargetInvalidated = 1
	}

	private enum State
	{
		Start = 0,
		MovingToTarget = 1,
		Repairing = 2,
		RepairingComplete = 3
	}

	private Vector3 mEntryPoint;

	private Actor mTarget;

	private Flags mFlags;

	private State mState;

	private bool mWasSuccessful;

	private SetPieceLogic mSetPieceLogic;

	public TaskRepair(TaskManager owner, TaskManager.Priority priority, Config flags, Actor target)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mState = State.Start;
		mFlags = Flags.Default;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
			inheritableMovementParams.mDestination = mTarget.GetPosition();
			inheritableMovementParams.DestinationThreshold = 1f;
			new TaskRouteTo(mOwner, base.Priority, base.ConfigFlags | Config.ClearAllCurrentType, inheritableMovementParams);
			mState = State.MovingToTarget;
			break;
		}
		case State.MovingToTarget:
			if (!mOwner.IsRunningTask(typeof(TaskRouteTo)))
			{
				if ((mTarget.GetPosition() - mActor.GetPosition()).sqrMagnitude < 4f)
				{
					mState = State.Repairing;
				}
				else
				{
					mFlags |= Flags.TargetInvalidated;
				}
			}
			break;
		case State.Repairing:
			mTarget.health.ModifyHealth(mActor.gameObject, mTarget.health.HealthMax, "Repair", Vector3.zero, false);
			mState = State.RepairingComplete;
			break;
		}
	}

	public override bool HasFinished()
	{
		if (mState == State.RepairingComplete)
		{
			return true;
		}
		if ((mFlags & Flags.TargetInvalidated) != 0)
		{
			return true;
		}
		return false;
	}
}
