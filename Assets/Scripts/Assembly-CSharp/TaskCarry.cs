using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskCarry : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		CarryingInvalidated = 1
	}

	private enum State
	{
		Start = 0,
		MovingToTarget = 1,
		PickingUpTarget = 2,
		MovingWithTarget = 3,
		MovingToFloorDropOffPosition = 4,
		DroppingTargetToFloor = 5,
		DroppingTargetIntoContainer = 6,
		CarryingComplete = 7
	}

	private Flags mFlags;

	private State mState;

	private Actor mTarget;

	private Vector3 mDropOffPosition;

	private Quaternion mDropOffRotation;

	private HidingPlace mDropOffContainer;

	private TaskMultiCharacterSetPiece mySetPieceTask;

	private bool mTargetIsInContainer;

	private bool mCachedInvulnerableFlag;

	public HidingPlace DropOffContainer
	{
		set
		{
			mState = State.DroppingTargetIntoContainer;
			mDropOffContainer = value;
			if (mDropOffContainer != null)
			{
				GameController instance = GameController.Instance;
				if (instance != null && instance.IsFirstPerson && mActor == instance.mFirstPersonActor)
				{
					instance.ExitFirstPerson(true, true);
				}
				mDropOffContainer.SetOccupier(mTarget, true);
				DropIntoContainer(TaskManager.Priority.IMMEDIATE);
			}
		}
	}

	public TaskCarry(TaskManager owner, TaskManager.Priority priority, Config flags, Actor target)
		: base(owner, priority, flags)
	{
		TBFAssert.DoAssert(target != null, string.Format("Attempting to initialise TaskCarry for {0} with NULL target Character", owner.name));
		mTarget = target;
		mDropOffContainer = null;
		mFlags = Flags.Default;
		mState = State.Start;
		if (mActor != null)
		{
			mCachedInvulnerableFlag = mActor.health.Invulnerable;
			mActor.health.Invulnerable = true;
		}
	}

	public void PickupOtherBody(Actor target)
	{
		if (mActor.realCharacter.Carried != null)
		{
			DropToFloor(TaskManager.Priority.IMMEDIATE);
		}
		mTarget = target;
		mState = State.Start;
	}

	private void PutAwayWeapon()
	{
		mActor.weapon.PutAway();
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			if (mySetPieceTask == null || mySetPieceTask.HasFinished())
			{
				List<Actor> list = new List<Actor>();
				list.Add(mActor);
				list.Add(mTarget);
				mTarget.baseCharacter.carryBooked = true;
				SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
				SetPieceModule actionSetPieceModule = mActor.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kPickupBody);
				setPieceLogic.SetModule(actionSetPieceModule);
				setPieceLogic.PlaceSetPiece(mTarget.transform.position, mTarget.transform.rotation);
				mySetPieceTask = new TaskMultiCharacterSetPiece(mActor.tasks, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType, setPieceLogic, list);
				mySetPieceTask.FixedActorOrder();
				TaskMultiCharacterSetPiece taskMultiCharacterSetPiece = mySetPieceTask;
				taskMultiCharacterSetPiece.AboutToStartSetPieceCallback = (TaskMultiCharacterSetPiece.AboutToStartSetPiece)Delegate.Combine(taskMultiCharacterSetPiece.AboutToStartSetPieceCallback, new TaskMultiCharacterSetPiece.AboutToStartSetPiece(PutAwayWeapon));
				mState = State.PickingUpTarget;
			}
			break;
		case State.PickingUpTarget:
			mState = ((!(mActor.realCharacter.Carried != mTarget)) ? State.MovingWithTarget : State.CarryingComplete);
			if (mActor != null)
			{
				mActor.health.Invulnerable = mCachedInvulnerableFlag;
			}
			break;
		case State.MovingToFloorDropOffPosition:
			if ((mActor.GetPosition() - mDropOffPosition).sqrMagnitude <= 0.25f)
			{
				DropToFloor(TaskManager.Priority.IMMEDIATE);
				mState = State.DroppingTargetToFloor;
				GameplayController.Instance().HideGhostPreview(mActor);
				break;
			}
			mState = State.MovingWithTarget;
			if (mDropOffContainer != null)
			{
				if (mTargetIsInContainer)
				{
					mDropOffContainer.DeactivateBlip();
				}
				else
				{
					mDropOffContainer.ClearOccupier();
				}
			}
			break;
		case State.DroppingTargetToFloor:
			if (!mOwner.IsRunningTask<TaskMultiCharacterSetPiece>())
			{
				CleanupCarriedAfterDrop();
				mState = State.CarryingComplete;
			}
			break;
		case State.DroppingTargetIntoContainer:
			if (!mOwner.IsRunningTask<TaskMultiCharacterSetPiece>())
			{
				if (mTargetIsInContainer)
				{
					mState = State.CarryingComplete;
					DeactiveCarriedActor();
					SetTargetsSelectability(true);
					CleanupCarriedAfterDrop();
				}
				else
				{
					mState = State.MovingWithTarget;
				}
				if (mDropOffContainer != null)
				{
					mDropOffContainer.ClearOccupier();
				}
			}
			break;
		case State.MovingToTarget:
		case State.MovingWithTarget:
			break;
		}
	}

	private void CleanupCarriedAfterDrop()
	{
		if (mTarget != null)
		{
			mTarget.baseCharacter.carryBooked = false;
			mActor.realCharacter.Drop();
			GameplayController.Instance().CancelAnyPlacement();
		}
	}

	public void DropToFloor(TaskManager.Priority priority)
	{
		List<Actor> list = new List<Actor>();
		list.Add(mActor);
		list.Add(mTarget);
		SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
		SetPieceModule actionSetPieceModule = mActor.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kDropBody);
		setPieceLogic.SetModule(actionSetPieceModule);
		setPieceLogic.PlaceSetPiece(mDropOffPosition, mDropOffRotation);
		mySetPieceTask = new TaskMultiCharacterSetPiece(mActor.tasks, priority, Config.Default, setPieceLogic, list);
		mySetPieceTask.FixedActorOrder();
	}

	public void DropIntoContainer(TaskManager.Priority priority)
	{
		List<Actor> list = new List<Actor>();
		list.Add(mActor);
		list.Add(mTarget);
		Actor carried = mActor.realCharacter.Carried;
		SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
		if (carried != null)
		{
			carried.awareness.visible = false;
		}
		SetPieceModule hideBodySetPiece = mDropOffContainer.HideBodySetPiece;
		setPieceLogic.SetModule(hideBodySetPiece);
		setPieceLogic.PlaceSetPiece(mDropOffContainer.SetPieceLocation.transform);
		mySetPieceTask = new TaskMultiCharacterSetPiece(mActor.tasks, priority, Config.Default, setPieceLogic, list);
		mySetPieceTask.FixedActorOrder();
		setPieceLogic.SetObject(2, mDropOffContainer.Model);
	}

	public bool ForceDropDone(Task fromTask)
	{
		if (fromTask == mySetPieceTask)
		{
			return true;
		}
		switch (mState)
		{
		case State.Start:
		case State.MovingToTarget:
		case State.PickingUpTarget:
		case State.CarryingComplete:
			if (mySetPieceTask == null || mySetPieceTask.HasFinished())
			{
				mState = State.CarryingComplete;
			}
			return true;
		case State.DroppingTargetToFloor:
		case State.DroppingTargetIntoContainer:
			return false;
		case State.MovingWithTarget:
			mDropOffContainer = null;
			mState = State.CarryingComplete;
			mDropOffPosition = mActor.transform.position;
			mDropOffRotation = mActor.transform.rotation;
			DropToFloor(TaskManager.Priority.REACTIVE);
			return true;
		default:
			return false;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.CarryingInvalidated) != 0)
		{
			Debug.LogWarning("invalidating");
			return true;
		}
		if (HasTargetBecomeUncarriable())
		{
			return true;
		}
		return mState == State.CarryingComplete;
	}

	public override void Finish()
	{
		if (mActor != null)
		{
			mActor.weapon.TakeOut();
			mActor.health.Invulnerable = mCachedInvulnerableFlag;
		}
		if (mTarget != null)
		{
			mTarget.baseCharacter.carryBooked = false;
			if (mTarget.behaviour.SelectedMarkerObj != null)
			{
				mTarget.behaviour.SelectedMarkerObj.SwitchOn();
			}
		}
		if (mDropOffContainer != null)
		{
			DeactiveCarriedActor();
		}
		mActor.realCharacter.Drop();
		SetTargetsSelectability(true);
		CleanupCarriedAfterDrop();
		if (mDropOffContainer != null)
		{
			if (mTargetIsInContainer)
			{
				mDropOffContainer.DeactivateBlip();
			}
			else
			{
				mDropOffContainer.ClearOccupier();
			}
		}
		GameplayController.Instance().HideGhostPreview(mActor);
	}

	public override void OnResume()
	{
		if (mOwner.lastFeedback.mResult == TaskManager.TaskResult.Aborted)
		{
			mState = State.CarryingComplete;
			mFlags |= Flags.CarryingInvalidated;
		}
	}

	public void SetDropOffLocation(Vector3 pos, Vector3 rot)
	{
		mActor.tasks.CancelTasks<TaskMultiCharacterSetPiece>();
		if (mDropOffContainer != null)
		{
			mDropOffContainer.ClearOccupier();
			mDropOffContainer.ActivateBlip();
			mDropOffContainer = null;
		}
		mState = State.MovingToFloorDropOffPosition;
		UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
		if (!WorldHelper.CalculatePath_AvoidingMantlesWhenCarrying(mActor, pos, navMeshPath))
		{
			return;
		}
		Vector3 vector = mActor.GetPosition();
		for (int num = navMeshPath.corners.Length - 2; num >= 0; num--)
		{
			Vector3 vector2 = navMeshPath.corners[num + 1] - navMeshPath.corners[num];
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude >= 2.25f)
			{
				float num2 = Mathf.Sqrt(sqrMagnitude);
				vector = navMeshPath.corners[num] + vector2 * ((num2 - 1.5f) / num2);
				rot = vector2;
				break;
			}
		}
		mDropOffPosition = vector;
		mDropOffRotation = Quaternion.LookRotation(rot);
		InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(mDropOffPosition);
		inheritableMovementParams.DestinationThreshold = 0.2f;
		inheritableMovementParams.forceFaceForwards = true;
		new TaskRouteTo(mOwner, base.Priority, Config.ClearAllCurrentType, inheritableMovementParams);
	}

	public void DropImmediately()
	{
		if (mState < State.DroppingTargetToFloor)
		{
			DropToFloor(TaskManager.Priority.IMMEDIATE);
			mState = State.DroppingTargetToFloor;
			if (mTarget != null)
			{
				mTarget.realCharacter.carryBooked = false;
				SetTargetsSelectability(true);
			}
		}
	}

	public override void Command(string com)
	{
		switch (com)
		{
		case "BodyWasPickedUp":
			mActor.realCharacter.Carried = mTarget;
			SetTargetsSelectability(false);
			break;
		case "BodyWasDroppedOnFloor":
			mActor.realCharacter.Drop();
			SetTargetsSelectability(true);
			break;
		case "BodyWasHidden":
			mTargetIsInContainer = true;
			SetTargetsSelectability(false);
			break;
		}
	}

	private void SetTargetsSelectability(bool isSelectable)
	{
		if (mTarget == null)
		{
			return;
		}
		SelectableObject component = mTarget.GetComponent<SelectableObject>();
		if (component != null)
		{
			component.enabled = isSelectable;
		}
		if (mTarget.Picker != null)
		{
			CMPlayerSoldier component2 = mTarget.Picker.GetComponent<CMPlayerSoldier>();
			if (component2 != null)
			{
				component2.enabled = isSelectable;
			}
			CMEnemySoldier component3 = mTarget.Picker.GetComponent<CMEnemySoldier>();
			if (component3 != null)
			{
				component3.enabled = isSelectable;
			}
		}
	}

	private void DeactiveCarriedActor()
	{
		if (!(mTarget == null) && !(mTarget.model == null) && !(mDropOffContainer == null))
		{
			EventOnHidden componentInChildren = mTarget.GetComponentInChildren<EventOnHidden>();
			if (componentInChildren != null)
			{
				componentInChildren.OnHidden();
			}
			mTarget.gameObject.SetActive(false);
			mTarget.model.gameObject.SetActive(false);
		}
	}

	public bool IsDropping()
	{
		return mState == State.DroppingTargetToFloor || mState == State.DroppingTargetIntoContainer;
	}

	public bool HasTargetBecomeUncarriable()
	{
		return mTarget != null && mTarget.baseCharacter.IsUncarriable;
	}
}
