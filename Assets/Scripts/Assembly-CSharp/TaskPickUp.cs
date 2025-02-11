using System;
using UnityEngine;

public class TaskPickUp : Task
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
		PickingUp = 1,
		PickUpComplete = 2
	}

	private CMPickUp mTarget;

	private State mState;

	private Flags mFlags;

	private bool mWasSuccessful;

	private TaskSetPiece mSetPieceTask;

	public TaskPickUp(TaskManager owner, TaskManager.Priority priority, Config flags, CMPickUp target)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mState = State.Start;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			if (mTarget != null)
			{
				PickUpObject componentInChildren = mTarget.GetComponentInChildren<PickUpObject>();
				SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
				setPieceLogic.SetModule(mTarget.GetSetPiece(componentInChildren, mActor));
				setPieceLogic.PlaceSetPiece(mTarget.IntelToPickUp.transform);
				setPieceLogic.SetObject(1, mTarget.IntelToPickUp);
				InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
				mSetPieceTask = new TaskSetPiece(mOwner, base.Priority, base.ConfigFlags, setPieceLogic, false, true, true, moveParams, 0f);
				mState = State.PickingUp;
			}
			break;
		case State.PickingUp:
			if (!mOwner.IsRunningTask<TaskSetPiece>())
			{
				if (mWasSuccessful)
				{
					mState = State.PickUpComplete;
				}
				else
				{
					mFlags |= Flags.TargetInvalidated;
				}
			}
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.TargetInvalidated) != 0)
		{
			return true;
		}
		return mState == State.PickUpComplete;
	}

	public override void Finish()
	{
		if (mActor != null)
		{
			if (mSetPieceTask != null)
			{
				if (mSetPieceTask.IsPlayingSetPiece() && !mSetPieceTask.HasFinished())
				{
					mSetPieceTask.Skip();
					mActor.baseCharacter.EnableNavMesh(true);
					mActor.tether.TetherToSelf();
					mActor.Command("ResetStand");
					CleanUpIntel();
				}
				mSetPieceTask.Cancel();
			}
			mActor.tasks.CancelTasks<TaskSetPiece>();
		}
		if (mTarget != null)
		{
			if ((mFlags & Flags.TargetInvalidated) != 0)
			{
				mTarget.Activate();
			}
			else if (mWasSuccessful)
			{
				CleanUpIntel();
			}
		}
	}

	private void CleanUpIntel()
	{
		if (mTarget.IntelToPickUp != null)
		{
			UnityEngine.Object.Destroy(mTarget.IntelToPickUp);
			mTarget.IntelToPickUp = null;
		}
		mTarget.Deactivate();
	}

	public override void Command(string com)
	{
		switch (com)
		{
		case "PickedUpIntel":
		{
			PickUpObject componentInChildren = mTarget.GetComponentInChildren<PickUpObject>();
			componentInChildren.Collected();
			if (mActor != null && mActor.realCharacter != null)
			{
				if (mActor.realCharacter.IsFirstPerson)
				{
					InterfaceSFX.Instance.PickupIntel.Play(mActor.gameObject);
				}
				mActor.speech.IntelCollected();
			}
			mWasSuccessful = true;
			break;
		}
		case "DoScreenStaticEffect":
			break;
		case "KillSetPieceIfFpp":
			if (mActor != null && GameController.Instance != null && mActor == GameController.Instance.mFirstPersonActor)
			{
				mActor.tasks.CancelTasks<TaskSetPiece>();
				mActor.tasks.CancelTasks<TaskPickUp>();
			}
			break;
		case "RemoveBlip":
			if (mTarget != null)
			{
				mTarget.TurnOff();
			}
			break;
		}
	}
}
