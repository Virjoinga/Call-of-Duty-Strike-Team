using System;
using UnityEngine;

public class TaskClose : Task
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
		Closing = 2,
		ClosingComplete = 3
	}

	private BuildingDoor mTarget;

	private Flags mFlags;

	private State mState;

	public TaskClose(TaskManager owner, TaskManager.Priority priority, Config flags, BuildingDoor door)
		: base(owner, priority, flags)
	{
		mTarget = door;
		mState = State.Start;
		mFlags = Flags.Default;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			Vector3 position = mTarget.transform.position;
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible, position);
			new TaskRouteTo(mOwner, base.Priority, Config.ClearAllCurrentType, moveParams);
			mState = State.MovingToTarget;
			break;
		}
		case State.MovingToTarget:
			if (!mOwner.IsRunningTask(typeof(TaskRouteTo)))
			{
				float sqrMagnitude = (mTarget.transform.position - mActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude <= WorldHelper.CHARACTER_TOUCH_PROXIMITY_SQ)
				{
					mState = State.Closing;
				}
				else
				{
					mFlags |= Flags.TargetInvalidated;
				}
			}
			break;
		case State.Closing:
			Debug.LogWarning("AC: Closing of doors not currently supported!");
			mState = State.ClosingComplete;
			break;
		}
	}

	public override bool HasFinished()
	{
		if (mState == State.Start)
		{
			return false;
		}
		if ((mFlags & Flags.TargetInvalidated) != 0)
		{
			return true;
		}
		return mState == State.ClosingComplete;
	}
}
