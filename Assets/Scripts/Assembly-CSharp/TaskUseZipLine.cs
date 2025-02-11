using System;

public class TaskUseZipLine : Task
{
	private enum State
	{
		Starting = 0,
		Animating = 1,
		Finished = 2
	}

	[Flags]
	private enum Flags
	{
		Default = 0,
		TargetInvalidated = 1
	}

	private Flags mFlags;

	private ZipLine mTarget;

	private State mState;

	public TaskUseZipLine(TaskManager owner, TaskManager.Priority priority, Config flags, ZipLine target)
		: base(owner, priority, flags)
	{
		mTarget = target;
	}

	public override void Update()
	{
		if (mTarget == null)
		{
			mFlags |= Flags.TargetInvalidated;
			return;
		}
		switch (mState)
		{
		case State.Starting:
			StartSetPiece();
			mState = State.Animating;
			break;
		case State.Animating:
			mState = State.Finished;
			break;
		}
	}

	private void StartSetPiece()
	{
		if (mActor != null && mTarget != null)
		{
			SetPieceLogic setPieceLogic = mActor.baseCharacter.CreateSetPieceLogic();
			setPieceLogic.SetModule(mTarget.SPModule);
			setPieceLogic.PlaceSetPiece(mTarget.transform);
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
			new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType, setPieceLogic, false, true, false, moveParams, 0f);
		}
		else
		{
			mFlags |= Flags.TargetInvalidated;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.TargetInvalidated) != 0)
		{
			return true;
		}
		return mState == State.Finished;
	}

	public override void Finish()
	{
		if (mTarget != null && mTarget.Interface.ForceFirstPerson && mActor != null)
		{
			mActor.tasks.CancelTasks<TaskFirstPerson>();
		}
	}

	public override void Command(string com)
	{
		if (com == "NowOnZipLine" && mTarget != null)
		{
			mTarget.OnStart();
		}
	}
}
