using System;
using UnityEngine;

public class TaskHack : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		TargetInvalidated = 1
	}

	private Flags mFlags;

	private HackableObject mTarget;

	private bool mHasStartedEnterSetPiece;

	private bool mHasStartedExitSetPiece;

	private bool mHasStarted;

	private bool mHasEnded;

	private Vector3 mInitialSetPieceRotation;

	public bool HasStarted
	{
		get
		{
			return mHasStarted;
		}
	}

	public bool FinishQuick { private get; set; }

	public TaskHack(TaskManager owner, TaskManager.Priority priority, Config flags, HackableObject target)
		: base(owner, priority, flags)
	{
		mTarget = target;
		if (mTarget != null)
		{
			mTarget.Init(mActor);
			mTarget.HackingActor = mActor;
			mTarget.HasReset = false;
			mTarget.IsInUse = true;
		}
	}

	public override void Update()
	{
		if (mTarget == null)
		{
			mFlags |= Flags.TargetInvalidated;
			return;
		}
		HackableObject.State hackState = mTarget.HackState;
		if (hackState == HackableObject.State.Clean || hackState == HackableObject.State.Dirty)
		{
			if (!mHasStartedEnterSetPiece)
			{
				mHasStartedEnterSetPiece = true;
				StartEnterSetPiece();
			}
			else if (!mOwner.IsRunningTask<TaskSetPiece>() && !mHasEnded)
			{
				mFlags |= Flags.TargetInvalidated;
				mActor.tasks.CancelTasks<TaskSetPiece>();
				mActor.tasks.CancelTasks<TaskCacheStanceState>();
				mTarget.IsInUse = false;
			}
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.TargetInvalidated) != 0)
		{
			return true;
		}
		if (mTarget != null)
		{
			HackableObject.State hackState = mTarget.HackState;
			if (hackState == HackableObject.State.TimeSwitchActive || hackState == HackableObject.State.HackFinished || mTarget.HasReset)
			{
				return true;
			}
		}
		return false;
	}

	public override void Finish()
	{
		mHasEnded = false;
		if (mTarget != null)
		{
			mTarget.CleanUp(mActor);
			mTarget.IsInUse = false;
		}
		if ((mFlags & Flags.TargetInvalidated) == 0 && (!FinishQuick || !mHasStartedEnterSetPiece || mHasStartedExitSetPiece) && mTarget != null)
		{
			if (mActor.baseCharacter.IsMortallyWounded() || mActor.baseCharacter.IsDead())
			{
				mTarget.FailHackAttempt(false, true);
			}
			else if (!mHasStartedExitSetPiece)
			{
				mHasStartedExitSetPiece = true;
				StartExitSetPiece();
				mTarget.StopHacking();
			}
		}
	}

	public override bool Consult(Task child)
	{
		if (mTarget.IsInUse && mTarget.HackingActor != mActor)
		{
			mActor.tasks.CancelTasks<TaskHack>();
		}
		return false;
	}

	private void StartEnterSetPiece()
	{
		if (mTarget != null && mTarget.SetPieceEnter != null)
		{
			GameController instance = GameController.Instance;
			if (instance != null && instance.IsFirstPerson && mActor == instance.mFirstPersonActor)
			{
				instance.ExitFirstPerson(true, true);
			}
			SetPieceLogic setPieceLogic = mActor.baseCharacter.CreateSetPieceLogic();
			setPieceLogic.SetModule(mTarget.SetPieceEnter);
			mInitialSetPieceRotation = mTarget.transform.eulerAngles;
			setPieceLogic.PlaceSetPiece(mTarget.transform);
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
			new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType | Config.ConsultParent, setPieceLogic, false, true, true, moveParams, 0f);
			mConsultant = this;
		}
		else
		{
			mFlags |= Flags.TargetInvalidated;
		}
	}

	private void StartExitSetPiece()
	{
		if (mTarget != null && mTarget.SetPieceExit != null)
		{
			SetPieceLogic setPieceLogic = mActor.baseCharacter.CreateSetPieceLogic();
			setPieceLogic.SetModule(mTarget.SetPieceExit);
			setPieceLogic.PlaceSetPiece(mTarget.transform.position, Quaternion.Euler(mInitialSetPieceRotation));
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
			TaskSetPiece taskSetPiece = new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType, setPieceLogic, false, true, false, moveParams, 0f);
			taskSetPiece.DoNotInterrupt = true;
		}
		else
		{
			mFlags |= Flags.TargetInvalidated;
		}
	}

	public override void Command(string com)
	{
		if (com == "HackStart")
		{
			mHasStarted = true;
			if (mTarget != null)
			{
				mTarget.ShowBlip();
				mTarget.StartHacking();
			}
		}
		else if (com == "HackEnd")
		{
			mHasEnded = true;
			if (mTarget != null)
			{
				mTarget.HideBlip();
			}
		}
	}
}
