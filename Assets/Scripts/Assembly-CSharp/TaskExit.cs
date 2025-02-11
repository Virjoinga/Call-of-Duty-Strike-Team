using System;

public class TaskExit : Task
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
		Exiting = 1,
		RemoveOccupier = 2,
		ExitingComplete = 3
	}

	private HidingPlace mTarget;

	private Flags mFlags;

	private State mState;

	private bool mWasSuccessful;

	public TaskExit(TaskManager owner, TaskManager.Priority priority, Config flags, HidingPlace hidingPlace)
		: base(owner, priority, flags)
	{
		mTarget = hidingPlace;
		mState = State.Start;
		mFlags = Flags.Default;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			if (mTarget == null)
			{
				mState = State.ExitingComplete;
				mFlags |= Flags.TargetInvalidated;
				break;
			}
			if (mTarget != null && mActor != null && mTarget.Occupier != mActor)
			{
				mState = State.ExitingComplete;
				mFlags |= Flags.TargetInvalidated;
				break;
			}
			if (HasWarped())
			{
				mState = State.ExitingComplete;
				mFlags |= Flags.TargetInvalidated;
				break;
			}
			SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
			setPieceLogic.SetModule(mTarget.ExitSetPiece);
			setPieceLogic.PlaceSetPiece(mTarget.SetPieceLocation);
			setPieceLogic.SetObject(1, mTarget.Model);
			InheritableMovementParams moveParams = new InheritableMovementParams();
			new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType, setPieceLogic, false, false, false, moveParams, 0f);
			mState = State.Exiting;
			break;
		}
		case State.Exiting:
			if (!mOwner.IsRunningTask(typeof(TaskSetPiece)))
			{
				if (mWasSuccessful)
				{
					mState = State.RemoveOccupier;
				}
				else
				{
					mFlags |= Flags.TargetInvalidated;
				}
			}
			break;
		case State.RemoveOccupier:
			mActor.realCharacter.EnableNavMesh(true);
			mState = State.ExitingComplete;
			break;
		}
	}

	private bool HasWarped()
	{
		return mActor.tasks.IsRunningTask<TaskUseFixedGun>();
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
		return mState == State.ExitingComplete;
	}

	public override void Finish()
	{
		mActor.tasks.CancelTasks(typeof(TaskEnter));
	}

	public override void Command(string com)
	{
		if (com == "ExitOk")
		{
			mWasSuccessful = true;
		}
		else if (com == "Show")
		{
			mActor.ShowHide(true);
		}
	}
}
