using System;
using UnityEngine;

public class TaskHeal : Task
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
		Healing = 1,
		HealingComplete = 2
	}

	private Flags mFlags;

	private State mState;

	private TaskSetPiece mSetPiece;

	public TaskHeal(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mState = State.Start;
		mFlags = Flags.Default;
		GameplayController.Instance().BroadcastEventHealed(mActor);
		mActor.baseCharacter.IsUncarriable = true;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			if (SectionTypeHelper.IsAGMG())
			{
				mActor.animDirector.EnableCategory(11, false, 0f);
				AnimatedScreenBackground.Instance.Deactivate();
				mActor.health.ModifyHealthOverTime(mActor.gameObject, mActor.health.HealthMax - mActor.health.Health, "Heal", mActor.transform.forward, false, 4f, true, 6f);
				if (CommonHudController.Instance != null)
				{
					CommonHudController.Instance.HUDInvulnerabilityEffect = true;
				}
				if (GameController.Instance.GMGReviveModeActive)
				{
					GameController.Instance.EndGMGReviveMode();
				}
				if (mActor.behaviour.PlayerControlled)
				{
					new TaskFirstPerson(mActor.tasks, TaskManager.Priority.IMMEDIATE, Config.Default);
				}
			}
			else
			{
				SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
				SetPieceModule actionSetPieceModule = mActor.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kHeal);
				setPieceLogic.SetModule(actionSetPieceModule);
				Vector3 eulerAngles = mActor.model.transform.rotation.eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				setPieceLogic.PlaceSetPiece(mActor.transform.position, Quaternion.Euler(eulerAngles));
				InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
				mSetPiece = new TaskSetPiece(mActor.tasks, base.Priority, Config.ClearAllCurrentType, setPieceLogic, false, false, false, moveParams, 0f);
				mActor.animDirector.EnableCategory(11, false, 0.25f);
			}
			mActor.speech.Healed();
			mState = State.Healing;
			break;
		case State.Healing:
			if (!mOwner.IsRunningTask(typeof(TaskSetPiece)))
			{
				mState = State.HealingComplete;
			}
			break;
		}
	}

	public override void Finish()
	{
		if (!(mActor != null))
		{
			return;
		}
		mActor.realCharacter.healBooked = false;
		mActor.baseCharacter.IsUncarriable = false;
		if (mState != State.HealingComplete)
		{
			if (mSetPiece != null && !mSetPiece.HasFinished())
			{
				mSetPiece.Skip();
			}
			mActor.health.QuickFinishHealOverTime();
			mActor.baseCharacter.EnableNavMesh(true);
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
		return mState == State.HealingComplete;
	}
}
