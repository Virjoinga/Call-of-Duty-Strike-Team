using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskStealthKill : Task
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
		EngagingTarget = 2,
		EnagementComplete = 3,
		WaitForKill = 4
	}

	private const float kStealthKillDist = 1.5f;

	private const float kStealthKillDistSq = 2.25f;

	private const float kSuspiciouslyLongDistance = 1f;

	private Vector3 OriginalWeaponScale;

	private Actor mTarget;

	private Flags mFlags;

	private State mState;

	private bool mTargetCouldLookPreviously;

	private bool mTargetCouldHearPreviously;

	private bool mHasCachedTargetsAbilities;

	public TaskStealthKill(TaskManager owner, TaskManager.Priority priority, Actor target, Config flags)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mState = State.Start;
		mFlags = Flags.Default;
	}

	public override void Update()
	{
		if (mTarget == null || mTarget.realCharacter.IsDead() || mTarget.realCharacter.IsMortallyWounded() || TutorialToggles.IsRespotting)
		{
			mFlags |= Flags.TargetInvalidated;
			return;
		}
		switch (mState)
		{
		case State.Start:
			if (GameController.Instance.IsFirstPerson && mActor == GameController.Instance.mFirstPersonActor)
			{
				mState = State.EngagingTarget;
			}
			else
			{
				MoveToTarget();
			}
			break;
		case State.MovingToTarget:
			if (!mOwner.IsRunningTask(typeof(TaskFollowMovingTarget)))
			{
				float sqrMagnitude = (mTarget.GetPosition() - mActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude <= 2.25f && !mActor.awareness.Obstructed(mTarget))
				{
					mState = State.EngagingTarget;
					mActor.realCharacter.ImposeLookDirection(mTarget.GetPosition() - mActor.GetPosition());
				}
				else
				{
					mFlags |= Flags.TargetInvalidated;
					mActor.navAgent.ResetPath();
					mActor.navAgent.Stop();
				}
			}
			break;
		case State.EngagingTarget:
		{
			List<Actor> list = new List<Actor>();
			list.Add(mActor);
			list.Add(mTarget);
			mActor.realCharacter.ImposeLookDirection(mTarget.GetPosition() - mActor.GetPosition());
			if (mTarget.Pose.blend != 0f || mTarget.Pose.direction != 0)
			{
				mFlags |= Flags.TargetInvalidated;
				break;
			}
			SetPieceModule actionSetPieceModule = mActor.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kStealthKill);
			if (actionSetPieceModule != null)
			{
				Vector3 position = mTarget.transform.position;
				position.y = mActor.realCharacter.transform.position.y;
				SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
				setPieceLogic.SetModule(actionSetPieceModule);
				if (GameController.Instance.mFirstPersonActor == null || GameController.Instance.mFirstPersonActor != mActor)
				{
					Quaternion rot = Quaternion.Slerp(mTarget.model.transform.rotation, mActor.model.transform.rotation, 0.5f);
					setPieceLogic.PlaceSetPiece(mActor.GetPosition(), rot);
					Transform characterStartNode = setPieceLogic.GetCharacterStartNode(1);
					Vector3 vector = mTarget.GetPosition() - characterStartNode.position;
					setPieceLogic.PlaceSetPiece(mActor.GetPosition() + vector, rot);
					characterStartNode = setPieceLogic.GetCharacterStartNode(0);
					NavMeshPath path = new NavMeshPath();
					if (!NavMesh.CalculatePath(mActor.GetPosition(), characterStartNode.position, mActor.navAgent.walkableMask, path) || SuspiciouslyLong(path))
					{
						Vector3 forward = mTarget.GetPosition() - mActor.GetPosition();
						forward.y = 0f;
						rot = Quaternion.LookRotation(forward);
						setPieceLogic.PlaceSetPiece(mActor.GetPosition(), rot);
						characterStartNode = setPieceLogic.GetCharacterStartNode(0);
						vector = mActor.GetPosition() - characterStartNode.position;
						setPieceLogic.PlaceSetPiece(mActor.GetPosition() + vector, rot);
					}
					TaskMultiCharacterSetPiece taskMultiCharacterSetPiece = new TaskMultiCharacterSetPiece(mActor.tasks, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType, setPieceLogic, list);
					taskMultiCharacterSetPiece.ForceIntoPlace = true;
					taskMultiCharacterSetPiece.FixedActorOrder();
					taskMultiCharacterSetPiece.SetDestinationLookTowardsThreshold(0, 4f);
					taskMultiCharacterSetPiece.SetFinalLookAt(0, position);
				}
				else
				{
					Quaternion rot2 = Quaternion.Euler(new Vector3(0f, mActor.realCharacter.FirstPersonCamera.transform.rotation.eulerAngles.y, 0f));
					setPieceLogic.PlaceSetPiece(mActor.GetPosition(), rot2);
					TaskMultiCharacterSetPiece taskMultiCharacterSetPiece2 = new TaskMultiCharacterSetPiece(mActor.tasks, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType, setPieceLogic, list);
					taskMultiCharacterSetPiece2.FixedActorOrder();
					taskMultiCharacterSetPiece2.SetDestinationLookTowardsThreshold(0, 4f);
					taskMultiCharacterSetPiece2.SetFinalLookAt(0, position);
				}
				FirstPersonPenaliser.EventOccurred(FirstPersonPenaliser.EventEnum.ThirdPersonStealthKill);
			}
			WaitForSync(TaskSynchroniser.SyncState.Done);
			if (mTarget != null)
			{
				mTargetCouldLookPreviously = mTarget.awareness.CanLook;
				mTarget.awareness.CanLook = false;
				mTarget.awareness.FlushAllAwareness();
				mTargetCouldHearPreviously = mTarget.ears.CanHear;
				mTarget.ears.CanHear = false;
				mTarget.ears.ClearCanHear();
				mHasCachedTargetsAbilities = true;
			}
			mTarget.health.Kill((!(actionSetPieceModule.name == "SP_StealthKill_4")) ? "Silent" : "SilentNeckSnap", mActor.gameObject);
			mState = State.WaitForKill;
			break;
		}
		case State.WaitForKill:
			if (!mActor.tasks.IsRunningTask(typeof(TaskMultiCharacterSetPiece)))
			{
				mState = State.EnagementComplete;
				mActor.Command("Unstealth");
			}
			break;
		case State.EnagementComplete:
			break;
		}
	}

	private bool SuspiciouslyLong(NavMeshPath path)
	{
		if (path.status != 0)
		{
			return true;
		}
		float num = 0f;
		for (int i = 0; i < path.corners.Length - 1; i++)
		{
			num += (path.corners[i + 1] - path.corners[i]).sqrMagnitude;
			if (num > 1f)
			{
				return true;
			}
		}
		return false;
	}

	public override void Finish()
	{
		mActor.weapon.TakeOut();
		mActor.realCharacter.UseLaserSite(true);
		mActor.Command("Unstealth");
		if (mTarget != null && mHasCachedTargetsAbilities)
		{
			mTarget.awareness.CanLook = mTargetCouldLookPreviously;
			mTarget.awareness.FlushAllAwareness();
			mTarget.ears.CanHear = mTargetCouldHearPreviously;
			mTarget.ears.ClearCanHear();
		}
		if (mActor.behaviour.PlayerControlled)
		{
			List<Actor> list = new List<Actor>();
			list.Add(mActor);
			OrdersHelper.UpdatePlayerSquadTetherPoint(mActor.GetPosition(), list);
		}
		CommonHudController.Instance.StealthKillPressed = false;
		base.Finish();
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
		if ((base.ConfigFlags & Config.AbortIfSpotted) != 0)
		{
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(mActor.awareness.EnemiesWhoCanSeeMe());
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				if (a.behaviour.InActiveAlertState())
				{
					mActor.navAgent.ResetPath();
					mActor.navAgent.Stop();
					return true;
				}
			}
		}
		return mState == State.EnagementComplete;
	}

	private void MoveToTarget()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			mState = State.EngagingTarget;
			return;
		}
		mActor.weapon.PutAway();
		mActor.realCharacter.UseLaserSite(false);
		WaitForSync(TaskSynchroniser.SyncState.InPosition);
		if ((mTarget.GetPosition() - mActor.GetPosition()).sqrMagnitude > 0f)
		{
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Walk, mTarget.GetPosition());
			inheritableMovementParams.DestinationThreshold = 1.4f;
			inheritableMovementParams.LookTowardsDist = 3f;
			inheritableMovementParams.forceFaceForwards = true;
			inheritableMovementParams.forceCrouch = true;
			inheritableMovementParams.mGunAwayAtEnd = true;
			SyncChildTask(new TaskFollowMovingTarget(mOwner, TaskManager.Priority.IMMEDIATE, base.ConfigFlags | Config.AbortWhenInRangeOfTarget | Config.AbortIfSpotted, inheritableMovementParams, mTarget, true));
		}
		else
		{
			mFlags |= Flags.TargetInvalidated;
		}
		mState = State.MovingToTarget;
	}
}
