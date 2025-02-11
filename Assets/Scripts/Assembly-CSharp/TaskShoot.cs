using System;
using UnityEngine;

public class TaskShoot : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		ContinueIfLosing = 1,
		Invalidated = 2,
		SuppressionFire = 4
	}

	private enum State
	{
		Start = 0,
		MoveToLOS = 1,
		Shooting = 2,
		Abort = 3
	}

	private Flags mFlags;

	private State mState;

	private Actor mTarget;

	public bool ContinueIfLosing
	{
		get
		{
			return (mFlags & Flags.ContinueIfLosing) != 0;
		}
		set
		{
			if (value)
			{
				mFlags |= Flags.ContinueIfLosing;
			}
			else
			{
				mFlags &= ~Flags.ContinueIfLosing;
			}
		}
	}

	public bool SuppressionFire
	{
		get
		{
			return (mFlags & Flags.SuppressionFire) != 0;
		}
		set
		{
			if (value)
			{
				mFlags |= Flags.SuppressionFire;
				ContinueIfLosing = true;
			}
			else
			{
				mFlags &= ~Flags.SuppressionFire;
			}
		}
	}

	public Actor Target
	{
		get
		{
			return mTarget;
		}
	}

	public TaskShoot(TaskManager owner, TaskManager.Priority priority, Actor target, Config flags)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mFlags = Flags.Default;
		mState = State.Start;
		if (mActor.realCharacter != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			gameplayController.BroadcastEventShootOrder(mActor);
		}
	}

	public override void Update()
	{
		if (!CanContinue())
		{
			return;
		}
		switch (mState)
		{
		case State.Start:
			if (WaitForSync(TaskSynchroniser.SyncState.WeaponsFree))
			{
				break;
			}
			mActor.Command("Shoot");
			if (mTarget != null)
			{
				if (ProcessMoveIntoPosition())
				{
					break;
				}
				mActor.weapon.SetTarget(mTarget);
			}
			mState = State.Shooting;
			break;
		case State.MoveToLOS:
			if (!base.Owner.IsRunningTask(typeof(TaskMoveTo)))
			{
				mState = State.Shooting;
			}
			break;
		case State.Shooting:
			if (mTarget != null)
			{
				mActor.weapon.SetTarget(mTarget);
				mActor.realCharacter.PickSomethingToAimAt(mTarget);
			}
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.Invalidated) != 0)
		{
			return true;
		}
		if (mState == State.Start || mState == State.MoveToLOS)
		{
			return false;
		}
		if (mState == State.Abort)
		{
			return true;
		}
		if (!IsTargetValid())
		{
			return true;
		}
		if (!ContinueIfLosing && !IsWinning())
		{
			return true;
		}
		if (mState == State.Shooting)
		{
			if (SuppressionFire && !mActor.realCharacter.CanBeSuppressed())
			{
				return true;
			}
			if (!SuppressionFire && (base.ConfigFlags & Config.IssuedByPlayerRequest) == 0 && (mActor.aiGunHandler.OnCooldown() || mActor.weapon.IsReloading()))
			{
				return true;
			}
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
		if (!mActor.aiGunHandler.IsValidTarget(mActor.weapon.ActiveWeapon, mTarget) || mActor.awareness.Obstructed(mTarget))
		{
			return false;
		}
		float magnitude = (mTarget.GetPosition() - mActor.GetPosition()).magnitude;
		if (mActor.realCharacter.FiringRange < magnitude)
		{
			float distanceSquared;
			Actor nearestVisibleEnemy = mActor.awareness.GetNearestVisibleEnemy(out distanceSquared);
			if (nearestVisibleEnemy != null && nearestVisibleEnemy != mTarget)
			{
				return false;
			}
		}
		return true;
	}

	private bool IsWinning()
	{
		if (mTarget == null)
		{
			return true;
		}
		if (mActor == null)
		{
			return true;
		}
		if (mTarget == null)
		{
			return true;
		}
		if ((base.ConfigFlags & Config.IssuedByPlayerRequest) != 0)
		{
			return true;
		}
		return mActor.behaviour.IsWinning(mTarget);
	}

	private bool ProcessMoveIntoPosition()
	{
		if (mActor == null || mTarget == null)
		{
			return false;
		}
		if (mActor.awareness.Obstructed(mTarget))
		{
			if (mActor.behaviour.PlayerControlled && (base.ConfigFlags & Config.IssuedByPlayerRequest) != 0)
			{
				mState = State.MoveToLOS;
				BaseCharacter.MovementStyle ms = BaseCharacter.MovementStyle.Run;
				if (mTarget.behaviour.alertState <= BehaviourController.AlertState.Focused)
				{
					ms = BaseCharacter.MovementStyle.Walk;
				}
				InheritableMovementParams moveParams = new InheritableMovementParams(ms, mTarget.GetPosition());
				TaskRouteTo taskRouteTo = new TaskRouteTo(base.Owner, base.Priority, Config.ClearAllCurrentType | Config.AbortOnUnobstructedTarget, moveParams);
				taskRouteTo.AbortTarget = mTarget;
				return true;
			}
			mState = State.Abort;
		}
		return false;
	}

	public GameObject ManageLookAt()
	{
		if (IsTargetValid())
		{
			if (mTarget != null)
			{
				mActor.realCharacter.TurnToFacePosition(mTarget.GetPosition());
				return mTarget.gameObject;
			}
			mActor.realCharacter.TurnToFaceDirection(mActor.realCharacter.transform.forward);
		}
		return null;
	}
}
