using System;
using UnityEngine;

public class TaskFlushOutEnemies : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		SpottedEnemy = 1
	}

	private enum State
	{
		Start = 0,
		MovingToInitialTarget = 1,
		SearchArea = 2,
		SearchComplete = 3
	}

	private ActorMask awarenessOfEnemiesAtStart = new ActorMask(0u, "Flush Enemies");

	private Flags mFlags;

	private State mState;

	private Vector3 mCurrentSearchPosition;

	private TargetWrapper mTargetWrapper;

	private TaskReaction mReactionTaskReference;

	public TaskFlushOutEnemies(TaskManager owner, TaskManager.Priority priority, Config flags, TaskReaction reactionTask)
		: this(owner, priority, flags)
	{
		mReactionTaskReference = reactionTask;
	}

	public TaskFlushOutEnemies(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		owner.CancelTasks(typeof(TaskRouteTo));
		owner.CancelTasks(typeof(TaskMoveTo));
		mState = State.Start;
		mFlags = Flags.Default;
		mTargetWrapper = null;
		if (mActor.tether.Active)
		{
			mActor.realCharacter.SetSearchArea(mActor.tether.Position, mActor.tether.TetherLimitSq, false);
		}
		else
		{
			mActor.realCharacter.SetSearchArea(false);
		}
		if (mActor.realCharacter.mSearchArea != null)
		{
			mActor.realCharacter.mSearchArea.RefreshUnreservedTargets();
		}
		awarenessOfEnemiesAtStart.Set(mActor.awareness.EnemiesIKnowAbout);
	}

	public static bool GetValidTrackingPosition(Actor actor, out Vector3 trackingTarget)
	{
		Vector3 lastKnownPosition;
		Actor nearestKnownEnemy = actor.awareness.GetNearestKnownEnemy(out lastKnownPosition, false);
		if (nearestKnownEnemy != null)
		{
			trackingTarget = lastKnownPosition;
			return true;
		}
		trackingTarget = actor.GetPosition();
		return false;
	}

	public override void Update()
	{
		float distanceSquared;
		Actor nearestVisibleEnemy = mActor.awareness.GetNearestVisibleEnemy(out distanceSquared);
		if (nearestVisibleEnemy != null && !AllEnemyUnitsDead())
		{
			mFlags |= Flags.SpottedEnemy;
			return;
		}
		if (mReactionTaskReference != null && mReactionTaskReference.Interrupt)
		{
			mState = State.SearchComplete;
		}
		switch (mState)
		{
		case State.Start:
		{
			Vector3 trackingTarget;
			if (GetValidTrackingPosition(mActor, out trackingTarget))
			{
				mCurrentSearchPosition = trackingTarget;
				if (mActor.tether.Active)
				{
					mActor.realCharacter.SetSearchArea(mActor.tether.Position, mActor.tether.TetherLimitSq, true);
				}
				else
				{
					mActor.realCharacter.SetSearchArea(true);
				}
			}
			else
			{
				mCurrentSearchPosition = GetTargetWrapperPositionToSearch();
			}
			if (mCurrentSearchPosition != Vector3.zero)
			{
				mState = State.MovingToInitialTarget;
			}
			else
			{
				mState = State.SearchComplete;
			}
			break;
		}
		case State.MovingToInitialTarget:
			if (!base.Owner.IsRunningTask(typeof(TaskRouteTo)))
			{
				MoveToNextTargetWrapper();
				mState = State.SearchArea;
			}
			break;
		case State.SearchArea:
			if (!base.Owner.IsRunningTask(typeof(TaskMoveTo)))
			{
				mCurrentSearchPosition = GetTargetWrapperPositionToSearch();
				if (!MoveToNextTargetWrapper())
				{
					mState = State.SearchComplete;
				}
			}
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.SpottedEnemy) != 0)
		{
			float distanceSquared;
			Actor nearestVisibleEnemy = mActor.awareness.GetNearestVisibleEnemy(out distanceSquared);
			if (nearestVisibleEnemy != null)
			{
				AutoClearNearbyVisibleTargetWrappers(nearestVisibleEnemy.GetPosition());
			}
			return true;
		}
		switch (mState)
		{
		case State.SearchArea:
			if ((mActor.awareness.EnemiesIKnowAboutRecent & ~(uint)awarenessOfEnemiesAtStart) != 0)
			{
				return true;
			}
			break;
		case State.SearchComplete:
			return true;
		}
		return false;
	}

	public override void Finish()
	{
		awarenessOfEnemiesAtStart.Release();
		if (mActor.realCharacter.IsDead())
		{
			AutoClearNearbyVisibleTargetWrappers();
		}
	}

	private bool MoveToNextTargetWrapper()
	{
		if (mCurrentSearchPosition != Vector3.zero)
		{
			BaseCharacter.MovementStyle ms = BaseCharacter.MovementStyle.Walk;
			if (mState == State.MovingToInitialTarget)
			{
				ms = BaseCharacter.MovementStyle.Run;
			}
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(ms, mCurrentSearchPosition);
			inheritableMovementParams.DestinationThreshold = 0.5f;
			inheritableMovementParams.FinalLookAt = mCurrentSearchPosition;
			if (mState == State.MovingToInitialTarget)
			{
				inheritableMovementParams.abortOnExitTether = true;
			}
			new TaskRouteTo(mOwner, TaskManager.Priority.LONG_TERM, Config.ClearAllCurrentType | Config.AbortOnVisibleEnemy, inheritableMovementParams, mTargetWrapper);
			return true;
		}
		return false;
	}

	private Vector3 GetTargetWrapperPositionToSearch(Vector3 positionToSeachAround)
	{
		if (mActor.behaviour.PlayerControlled)
		{
			return Vector3.zero;
		}
		if (mActor.realCharacter.mSearchArea == null)
		{
			return Vector3.zero;
		}
		AutoSearchNearbyVisibleTargetWrappers();
		TargetWrapper targetWrapper = null;
		if (mActor.realCharacter.mSearchArea != null)
		{
			targetWrapper = mActor.realCharacter.mSearchArea.GetNearestUnsearchedTargetCheap(positionToSeachAround);
		}
		if (targetWrapper != null)
		{
			targetWrapper.Reserve();
			mTargetWrapper = targetWrapper;
			return targetWrapper.GetPosition();
		}
		if (!mActor.realCharacter.mSearchArea.DynamicSearchArea)
		{
			mActor.realCharacter.mSearchArea.RefreshAllTargets();
			targetWrapper = mActor.realCharacter.mSearchArea.GetNearestUnsearchedTargetCheap(positionToSeachAround);
		}
		else
		{
			if (mActor.tether.Active)
			{
				mActor.realCharacter.SetSearchArea(mActor.tether.Position, mActor.tether.TetherLimitSq, true);
			}
			else
			{
				mActor.realCharacter.SetSearchArea(true);
			}
			targetWrapper = mActor.realCharacter.mSearchArea.GetNearestUnsearchedTargetCheap(positionToSeachAround);
		}
		if (targetWrapper != null)
		{
			targetWrapper.Search();
			return targetWrapper.GetPosition();
		}
		return Vector3.zero;
	}

	private Vector3 GetTargetWrapperPositionToSearch()
	{
		return GetTargetWrapperPositionToSearch(mActor.GetPosition());
	}

	private void AutoSearchNearbyVisibleTargetWrappers()
	{
	}

	private void AutoClearNearbyVisibleTargetWrappers()
	{
		AutoClearNearbyVisibleTargetWrappers(mActor.GetPosition());
	}

	private void AutoClearNearbyVisibleTargetWrappers(Vector3 position)
	{
		if (mTargetWrapper != null)
		{
			mTargetWrapper.ClearSearch();
		}
		TargetWrapperManager.Instance().AutoClearNearbyVisibleTargetWrappers(position);
	}

	private bool AllEnemyUnitsDead()
	{
		return (GKM.EnemiesMask(mActor) & GKM.AliveMask) == 0;
	}
}
