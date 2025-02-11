using System;
using UnityEngine;

public class TaskDefend : Task
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
		Defending = 2
	}

	private const float kFollowTouchDist = 1.5f;

	private const float kAdditionalFollowTouchDist = 0.5f;

	private const float kMaxMovement = 5f;

	private const float kSearchRadius = 10f;

	private const float kPositionOffsetRange = 1f;

	private Flags mFlags;

	private State mState;

	private Actor mTarget;

	private Vector3 mPreviousTargetPosition;

	private int mDefendId;

	private Vector3 quadrantVariance;

	public Actor Target
	{
		get
		{
			return mTarget;
		}
	}

	public TaskDefend(TaskManager owner, TaskManager.Priority priority, Actor target, Config flags)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mFlags = Flags.Default;
		mState = State.Start;
		mPreviousTargetPosition = mActor.GetPosition();
		mDefendId = ++target.realCharacter.NumberOfDefenders;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			if (!ProcessMoveIntoPosition())
			{
				mState = State.Defending;
			}
			break;
		case State.MoveToLOS:
			if (!base.Owner.IsRunningTask(typeof(TaskMoveTo)))
			{
				mState = State.Defending;
			}
			break;
		case State.Defending:
			DefendTarget();
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.Invalidated) != 0)
		{
			return true;
		}
		if (mTarget != null && mTarget.realCharacter.IsDead() && mActor.awareness.CanSee(mTarget))
		{
			mTarget.realCharacter.NumberOfDefenders--;
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
		if (mActor == null)
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
		if (mTarget == null)
		{
			return false;
		}
		if (!mActor.awareness.CanSee(Target))
		{
			mState = State.MoveToLOS;
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Run, mTarget.GetPosition());
			TaskRouteTo taskRouteTo = new TaskRouteTo(base.Owner, base.Priority, Config.ClearAllCurrentType | Config.AbortOnUnobstructedTarget, moveParams);
			taskRouteTo.AbortTarget = mTarget;
			return true;
		}
		return false;
	}

	private void DefendTarget()
	{
		bool flag = (mPreviousTargetPosition - mTarget.GetPosition()).magnitude > 1.5f;
		if (mActor.behaviour.alertState >= BehaviourController.AlertState.Reacting)
		{
			float distanceSquared;
			Actor nearestVisibleEnemy = mActor.awareness.GetNearestVisibleEnemy(out distanceSquared);
			if (!mActor.awareness.CanSee(mTarget))
			{
				InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(mTarget.GetPosition());
				CoverPointCore validDefensiveCoverNearestSpecifiedPosition = mActor.awareness.GetValidDefensiveCoverNearestSpecifiedPosition(mTarget.GetPosition(), 10f, 0f, false);
				if (validDefensiveCoverNearestSpecifiedPosition != null)
				{
					inheritableMovementParams.mMovementStyle = BaseCharacter.MovementStyle.Run;
					new TaskMoveToCover(mOwner, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType, validDefensiveCoverNearestSpecifiedPosition, inheritableMovementParams);
				}
				else
				{
					new TaskRouteTo(mOwner, TaskManager.Priority.LONG_TERM, Config.ClearAllCurrentType | Config.AbortOnAlert, inheritableMovementParams);
				}
			}
			else
			{
				if (!(nearestVisibleEnemy != null))
				{
					return;
				}
				bool canMoveIntoCover;
				if (ShouldMoveIntoCover(nearestVisibleEnemy, distanceSquared, out canMoveIntoCover))
				{
					OrderMoveToCover(nearestVisibleEnemy);
				}
				else if (IsAutomatedBehaviourAllowed(nearestVisibleEnemy))
				{
					TaskShoot taskShoot = new TaskShoot(mOwner, TaskManager.Priority.IMMEDIATE, nearestVisibleEnemy, Config.Default);
					if (!canMoveIntoCover)
					{
						taskShoot.ContinueIfLosing = true;
					}
				}
			}
		}
		else if (flag)
		{
			switch (mDefendId % 4)
			{
			case 0:
				quadrantVariance = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, 1f);
				break;
			case 1:
				quadrantVariance = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, -1f);
				break;
			case 2:
				quadrantVariance = new Vector3(1f, 0f, UnityEngine.Random.Range(-1f, 1f));
				break;
			default:
				quadrantVariance = new Vector3(-1f, 0f, UnityEngine.Random.Range(-1f, 1f));
				break;
			}
			Vector3 targetPosition = mTarget.realCharacter.TargetPosition;
			InheritableMovementParams inheritableMovementParams2 = new InheritableMovementParams(mTarget.realCharacter.MovementStyleActive, targetPosition + quadrantVariance);
			inheritableMovementParams2.DestinationThreshold = 1.5f;
			new TaskRouteTo(base.Owner, base.Priority, Config.Default, inheritableMovementParams2);
			mPreviousTargetPosition = mTarget.GetPosition();
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

	private bool ShouldMoveIntoCover(Actor nearestEnemyInView, float nearestEnemyInViewDistSq, out bool canMoveIntoCover)
	{
		bool isInCover = mActor.awareness.isInCover;
		bool flag = mActor.awareness.IsCoverAvailable();
		canMoveIntoCover = !isInCover && flag;
		if (flag && nearestEnemyInViewDistSq < CoverPoint.COVER_TOO_CLOSE_TO_ENEMY_DIST_SQ)
		{
			return true;
		}
		if ((!mActor.behaviour.IsWinning(nearestEnemyInView) || mActor.aiGunHandler.OnCooldown() || mActor.weapon.IsReloading()) && canMoveIntoCover)
		{
			return true;
		}
		return false;
	}

	private void OrderMoveToCover(Actor nearestEnemyInView)
	{
		InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Run);
		inheritableMovementParams.mDestination = mActor.GetPosition();
		new TaskMoveToCover(mOwner, TaskManager.Priority.LONG_TERM, Config.ClearAllCurrentType, inheritableMovementParams);
		mActor.behaviour.ResetEngagementAssessor(nearestEnemyInView);
	}
}
