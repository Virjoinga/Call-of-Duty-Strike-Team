using System;
using System.Collections.Generic;
using UnityEngine;

public static class OrdersHelper
{
	private class MultiManCandidate
	{
		public Actor ActorRef { get; private set; }

		public int SelectedActorsIndex { get; private set; }

		public bool IsInTheCaptureRange { get; private set; }

		public float SqrMagDistance { get; private set; }

		public float YDiff { get; private set; }

		public MultiManCandidate(Actor actor, int selectedActorsIndex, bool isInTheCaptureRange, float sqrMagDistance, float yDiff)
		{
			ActorRef = actor;
			SelectedActorsIndex = selectedActorsIndex;
			IsInTheCaptureRange = isInTheCaptureRange;
			SqrMagDistance = sqrMagDistance;
			YDiff = yDiff;
		}
	}

	public static void OrderUnitMove(GameplayController gameplayController, Actor selected, Vector3 destination, BaseCharacter.MovementStyle speed, CoverPointCore cp, Actor LookAtEnd)
	{
		if (selected.tasks.RunningTaskDeniesPlayerInput || selected.baseCharacter.IsInASetPiece)
		{
			return;
		}
		List<Actor> list = new List<Actor>();
		list.Add(selected);
		UpdatePlayerSquadTetherPoint(destination, list);
		TaskEnter taskEnter = selected.tasks.GetRunningTask(typeof(TaskEnter)) as TaskEnter;
		if (taskEnter != null && taskEnter.Container != null && taskEnter.HasFinishedEntering)
		{
			OrderExit(GameplayController.instance, taskEnter.Container);
			return;
		}
		selected.tasks.CancelTasks(typeof(TaskStealthKill));
		selected.tasks.CancelTasks(typeof(TaskRouteTo));
		selected.tasks.CancelTasks(typeof(TaskMoveTo));
		selected.tasks.CancelTasks(typeof(TaskShoot));
		selected.tasks.CancelTasks(typeof(TaskFollowMovingTarget));
		selected.tasks.CancelTasks(typeof(TaskMoveToCover));
		selected.tasks.CancelTasks(typeof(TaskHack));
		selected.tasks.CancelTasks(typeof(TaskCacheStanceState));
		selected.tasks.CancelTasks(typeof(TaskDropClaymore));
		selected.tasks.CancelTasks(typeof(TaskPlantC4));
		selected.tasks.CancelTasks(typeof(TaskPickUp));
		selected.behaviour.ClearAimedShotTarget();
		selected.behaviour.ClearSuppressionTarget();
		InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(speed, destination);
		WaypointMarker waypointMarker = WaypointMarkerManager.Instance.AddMarker(selected.gameObject, destination, WaypointMarker.Type.Undefined, WaypointMarker.State.Walk);
		if (cp != null && cp.Available(selected))
		{
			selected.awareness.BookCoverPoint(cp, 0f);
			waypointMarker.SetCoverPoint(cp);
			if (speed == BaseCharacter.MovementStyle.Run)
			{
				waypointMarker.SetState(WaypointMarker.State.Run);
			}
			inheritableMovementParams.holdCoverWhenBored = true;
			inheritableMovementParams.holdCoverWhenFlanked = true;
			RegisterTaskAsPlayerIssued(new TaskMoveToCover(selected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType | Task.Config.AbortIfDestinationBlocked, cp, inheritableMovementParams));
			return;
		}
		waypointMarker.SetCoverPoint(null);
		if (speed == BaseCharacter.MovementStyle.Run)
		{
			waypointMarker.SetState(WaypointMarker.State.Run);
		}
		else
		{
			inheritableMovementParams.stanceAtEnd = InheritableMovementParams.StanceOrder.CrouchFromStealth;
			waypointMarker.SetState(WaypointMarker.State.Walk);
		}
		if (LookAtEnd != null)
		{
			inheritableMovementParams.FinalLookAtObject = LookAtEnd.gameObject;
		}
		if (selected.realCharacter.Carried != null)
		{
			inheritableMovementParams.forceFaceForwards = true;
		}
		RegisterTaskAsPlayerIssued(new TaskRouteTo(selected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType | Task.Config.AbortIfDestinationBlocked, inheritableMovementParams));
	}

	public static void OrderUnitMoveToCover(GameplayController gameplayController, Actor selected, Vector3 destination, BaseCharacter.MovementStyle speed, float unitCoverSearchRadius, bool addWaypointMarker)
	{
		if (!selected.tasks.RunningTaskDeniesPlayerInput && !selected.baseCharacter.IsInASetPiece)
		{
			List<Actor> list = new List<Actor>();
			list.Add(selected);
			UpdatePlayerSquadTetherPoint(destination, list);
			SquadFormation.OrderSquadMove(list, destination, speed, gameplayController.FormationSpacingRow, gameplayController.FormationSpacingColumn, gameplayController.FormationCoverSearchRadius, unitCoverSearchRadius, addWaypointMarker);
		}
	}

	public static void OrderSquadMove(GameplayController gameplayController, Vector3 destination, BaseCharacter.MovementStyle speed, float coverSearchRadius, float unitCoverSearchRadius)
	{
		List<Actor> list = new List<Actor>();
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput && !item.baseCharacter.IsInASetPiece)
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			UpdatePlayerSquadTetherPoint(destination, list);
			SquadFormation.OrderSquadMove(list, destination, speed, gameplayController.FormationSpacingRow, gameplayController.FormationSpacingColumn, coverSearchRadius, unitCoverSearchRadius, true);
		}
	}

	public static void OrderHoldPosition(GameplayController gameplayController)
	{
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput)
			{
				item.realCharacter.HoldPosition();
				List<Actor> list = new List<Actor>();
				list.Add(item);
				UpdatePlayerSquadTetherPoint(item.GetPosition(), list);
			}
		}
	}

	public static void OrderCrouch(GameplayController gameplayController)
	{
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput)
			{
				item.realCharacter.Crouch();
			}
		}
	}

	public static void OrderStand(GameplayController gameplayController)
	{
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput)
			{
				item.realCharacter.Stand();
			}
		}
	}

	public static void OrderEngagement(GameplayController gameplayController, GameObject go)
	{
		Actor component = go.GetComponent<Actor>();
		if (component == null)
		{
			return;
		}
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput)
			{
				item.behaviour.EngageTarget(component);
			}
		}
	}

	public static void OrderShootAtTarget(GameplayController gameplayController, Actor target, bool isSuppressionFire)
	{
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput)
			{
				ExecuteShootAtTarget(item, target, isSuppressionFire, false);
			}
		}
	}

	public static void ExecuteShootAtTarget(Actor selected, Actor target, bool isSuppressionFire, bool sync)
	{
		if (isSuppressionFire)
		{
			selected.behaviour.Suppress(target, sync);
		}
		else
		{
			selected.behaviour.AimedShot(target, sync);
		}
	}

	public static void OrderPrimeGrenade(GameplayController gameplayController, Vector3 initialTargetPosition)
	{
		TBFAssert.DoAssert(!GameController.Instance.GrenadeThrowingModeActive, "OrderPrimeGrenade being called when already in GameController mode");
		Actor nearestSelectedForGrenadeOrClaymore = GetNearestSelectedForGrenadeOrClaymore(gameplayController, initialTargetPosition);
		if (!(nearestSelectedForGrenadeOrClaymore == null) && nearestSelectedForGrenadeOrClaymore.grenadeThrower.Prime(initialTargetPosition) && !GameController.Instance.StartGrenadeThrowingMode())
		{
			nearestSelectedForGrenadeOrClaymore.grenadeThrower.Cancel();
		}
	}

	public static void OrderDropClaymore(GameplayController gameplayController, Vector3 dropPosition, Vector3 facingDirection)
	{
		TBFAssert.DoAssert(!GameController.Instance.ClaymoreDroppingModeActive, "OrderDropClaymore being called when already in GameController mode");
		Actor nearestSelectedForGrenadeOrClaymore = GetNearestSelectedForGrenadeOrClaymore(gameplayController, dropPosition);
		if (!(nearestSelectedForGrenadeOrClaymore == null))
		{
			WaypointMarkerManager.Instance.RemoveMarker(nearestSelectedForGrenadeOrClaymore.gameObject);
			if (!nearestSelectedForGrenadeOrClaymore.baseCharacter.IsInASetPiece && !nearestSelectedForGrenadeOrClaymore.tasks.IsRunningTask<TaskDropClaymore>())
			{
				new TaskCacheStanceState(nearestSelectedForGrenadeOrClaymore.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelectedForGrenadeOrClaymore);
				RegisterTaskAsPlayerIssued(new TaskDropClaymore(nearestSelectedForGrenadeOrClaymore.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, dropPosition, facingDirection));
			}
		}
	}

	public static void OrderMeleeAttack(GameplayController gameplayController, Actor target)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, target.GetPosition(), typeof(TaskStealthKill));
		if (!(nearestSelected == null))
		{
			WaypointMarkerManager.Instance.RemoveMarker(nearestSelected.gameObject);
			ExecuteMeleeAttack(nearestSelected, target);
		}
	}

	public static void ExecuteMeleeAttack(Actor selected, Actor target)
	{
		if (!(selected == null) && !(target == null) && !selected.tasks.IsRunningTask<TaskStealthKill>())
		{
			new TaskCacheStanceState(selected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, selected);
			RegisterTaskAsPlayerIssued(new TaskStealthKill(selected.tasks, TaskManager.Priority.IMMEDIATE, target, Task.Config.ClearAllCurrentType | Task.Config.AbortIfSpotted));
		}
	}

	public static void OrderPeekThroughWindow(GameplayController gameplayController, BuildingWindow window)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, window.transform.position);
		if (!(nearestSelected == null))
		{
			new TaskCacheStanceState(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelected);
			RegisterTaskAsPlayerIssued(new TaskPeekaboo(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, window, Task.Config.ClearAllCurrentType));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(window.transform.position, list);
		}
	}

	public static void OrderOpen(GameplayController gameplayController, BuildingDoor door)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, door.transform.position);
		if (!(nearestSelected == null))
		{
			RegisterTaskAsPlayerIssued(new TaskOpen(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, door));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(door.transform.position, list);
		}
	}

	public static void OrderClose(GameplayController gameplayController, BuildingDoor door)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, door.transform.position);
		if (!(nearestSelected == null))
		{
			RegisterTaskAsPlayerIssued(new TaskClose(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, door));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(door.transform.position, list);
		}
	}

	public static void OrderBreachByExplosive(GameplayController gameplayController, BuildingDoor door)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, door.transform.position);
		if (!(nearestSelected == null))
		{
			BreachSequence breachSequence = door.BreachSequence;
			RegisterTaskAsPlayerIssued(new TaskBreach(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, breachSequence));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(door.transform.position, list);
		}
	}

	public static void OrderPlantC4(Transform trans)
	{
		Actor nearestSelected = GetNearestSelected(GameplayController.instance, trans.position, typeof(TaskSetPiece));
		if (!(nearestSelected == null))
		{
			SetPieceLogic setPieceLogic = nearestSelected.realCharacter.CreateSetPieceLogic();
			SetPieceModule actionSetPieceModule = nearestSelected.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kPlantC4);
			setPieceLogic.SetModule(actionSetPieceModule);
			setPieceLogic.PlaceSetPiece(trans);
			new TaskPlantC4(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, null);
			OrderSetPiece(GameplayController.instance, setPieceLogic, null, typeof(TaskSetPiece));
		}
	}

	public static void OrderHack(GameplayController gameplayController, HackableObject target)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, target.transform.position, typeof(TaskHack));
		if (!(nearestSelected == null) && !nearestSelected.baseCharacter.IsInASetPiece && !nearestSelected.tasks.IsRunningTask<TaskHack>())
		{
			new TaskCacheStanceState(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelected);
			RegisterTaskAsPlayerIssued(new TaskHack(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, target));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(target.transform.position, list);
		}
	}

	public static void OrderRepair(GameplayController gameplayController, Actor robot)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, robot.GetPosition());
		if (!(nearestSelected == null))
		{
			RegisterTaskAsPlayerIssued(new TaskRepair(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, robot));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(robot.GetPosition(), list);
		}
	}

	public static void OrderCarry(GameplayController gameplayController, Actor target)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, target.GetPosition());
		if (!(nearestSelected == null) && !nearestSelected.baseCharacter.IsInASetPiece && (!(target.realCharacter != null) || !target.realCharacter.IsBeingCarried))
		{
			TBFAssert.DoAssert(target != null, "OrderCarry being called with NULL target character");
			new TaskCacheStanceState(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelected);
			TaskCarry runningTask = nearestSelected.tasks.GetRunningTask<TaskCarry>();
			if (runningTask != null)
			{
				runningTask.PickupOtherBody(target);
			}
			else
			{
				RegisterTaskAsPlayerIssued(new TaskCarry(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, target));
			}
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(target.GetPosition(), list);
		}
	}

	public static void OrderHeal(GameplayController gameplayController, Actor target)
	{
		if (!(target == null) && (!(target.realCharacter != null) || target.realCharacter.IsMortallyWounded()))
		{
			RegisterTaskAsPlayerIssued(new TaskHeal(target.tasks, TaskManager.Priority.REACTIVE, Task.Config.ClearAllCurrentType));
			List<Actor> list = new List<Actor>();
			list.Add(target);
			UpdatePlayerSquadTetherPoint(target.GetPosition(), list);
		}
	}

	public static void OrderDrop(GameplayController gameplayController, Vector3 target)
	{
		Actor[] array = gameplayController.Selected.ToArray();
		Actor[] array2 = array;
		foreach (Actor actor in array2)
		{
			if (!actor.tasks.RunningTaskDeniesPlayerInput)
			{
				TaskCarry taskCarry = actor.tasks.GetRunningTask(typeof(TaskCarry)) as TaskCarry;
				if (taskCarry != null)
				{
					taskCarry.SetDropOffLocation(target, target - actor.baseCharacter.transform.position);
					List<Actor> list = new List<Actor>();
					list.Add(actor);
					GameplayController.Instance().HideGhostPreview(actor);
					UpdatePlayerSquadTetherPoint(target, list);
				}
			}
		}
	}

	public static void OrderDropImmediately(GameplayController gameplayController)
	{
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput)
			{
				TaskCarry taskCarry = item.tasks.GetRunningTask(typeof(TaskCarry)) as TaskCarry;
				if (taskCarry != null)
				{
					taskCarry.DropImmediately();
					List<Actor> list = new List<Actor>();
					list.Add(item);
					UpdatePlayerSquadTetherPoint(item.transform.position, list);
				}
			}
		}
	}

	public static void OrderFollow(GameplayController gameplayController, Actor target)
	{
		bool flag = false;
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput)
			{
				RegisterTaskAsPlayerIssued(new TaskFollow(item.tasks, TaskManager.Priority.IMMEDIATE, target, Task.Config.ClearAllCurrentType));
				flag = true;
			}
		}
		if (flag)
		{
			UpdatePlayerSquadTetherPoint(target.GetPosition(), gameplayController.Selected);
		}
	}

	public static void OrderDefend(GameplayController gameplayController, Actor target)
	{
		bool flag = false;
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput)
			{
				RegisterTaskAsPlayerIssued(new TaskDefend(item.tasks, TaskManager.Priority.IMMEDIATE, target, Task.Config.ClearAllCurrentType));
				flag = true;
			}
		}
		if (flag)
		{
			UpdatePlayerSquadTetherPoint(target.GetPosition(), gameplayController.Selected);
		}
	}

	public static void OrderEnter(GameplayController gameplayController, HidingPlace target)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, target.transform.position);
		if (!(nearestSelected == null) && !nearestSelected.baseCharacter.IsInASetPiece)
		{
			RegisterTaskAsPlayerIssued(new TaskEnter(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.AbortIfSpotted, target));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(target.transform.position, list);
		}
	}

	public static void OrderExit(GameplayController gameplayController, HidingPlace target)
	{
		if (target.IsOccupied)
		{
			Actor occupier = target.Occupier;
			if (!(occupier == null) && !occupier.tasks.RunningTaskDeniesPlayerInput)
			{
				RegisterTaskAsPlayerIssued(new TaskExit(occupier.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, target));
			}
		}
	}

	public static void OrderHideBody(GameplayController gameplayController, HidingPlace target)
	{
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput && !item.baseCharacter.IsInASetPiece)
			{
				TaskCarry taskCarry = item.tasks.GetRunningTask(typeof(TaskCarry)) as TaskCarry;
				if (taskCarry != null)
				{
					taskCarry.DropOffContainer = target;
					List<Actor> list = new List<Actor>();
					list.Add(item);
					GameplayController.Instance().HideGhostPreview(item);
					UpdatePlayerSquadTetherPoint(target.transform.position, list);
					break;
				}
			}
		}
	}

	public static void OrderUseZipLine(GameplayController gameplayController, ZipLine target)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, target.transform.position);
		if (!(nearestSelected == null))
		{
			new TaskCacheStanceState(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelected);
			if (target != null && target.Interface.ForceFirstPerson && !GameController.Instance.IsFirstPerson)
			{
				GameController.Instance.SwitchToFirstPerson(nearestSelected, true);
			}
			RegisterTaskAsPlayerIssued(new TaskUseZipLine(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, target));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(target.transform.position, list);
		}
	}

	public static void OrderUseFixedGun(GameplayController gameplayController, FixedGun target)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, target.transform.position);
		if (!(nearestSelected == null))
		{
			new TaskCacheStanceState(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelected);
			RegisterTaskAsPlayerIssued(new TaskUseFixedGun(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType | Task.Config.AbortIfSpotted, target, false, false, false, false));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(target.transform.position, list);
		}
	}

	public static void OrderArmExplosives(GameplayController gameplayController, SetPieceLogic target, CMSetPiece contextMenu, ExplodableObject explodableObject)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, target.transform.position, typeof(TaskSetPiece));
		if (!(nearestSelected == null))
		{
			new TaskPlantC4(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, explodableObject);
			OrderSetPiece(gameplayController, target, contextMenu, typeof(TaskSetPiece));
		}
	}

	public static void OrderSetPiece(GameplayController gameplayController, SetPieceLogic target)
	{
		OrderSetPiece(gameplayController, target, null, null);
	}

	public static void OrderSetPiece(GameplayController gameplayController, SetPieceLogic target, CMSetPiece contextMenu)
	{
		OrderSetPiece(gameplayController, target, contextMenu, null);
	}

	public static void OrderSetPiece(GameplayController gameplayController, SetPieceLogic target, CMSetPiece contextMenu, Type excludeIfHasTask)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, target.transform.position, excludeIfHasTask);
		if (!(nearestSelected == null))
		{
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
			new TaskCacheStanceState(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelected);
			RegisterTaskAsPlayerIssued(new TaskSetPiece(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, target, false, true, true, moveParams, 0f, contextMenu));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(target.transform.position, list);
		}
	}

	public static void OrderMultiCharacterSetPiece(GameplayController gameplayController, SetPieceLogic target, Vector3 position, CMSetPiece contextMenu)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, position);
		if (nearestSelected == null)
		{
			return;
		}
		int numActorsRequired = target.GetNumActorsRequired();
		if (!GameController.Instance.IsFirstPerson && gameplayController.Selected.Count < numActorsRequired)
		{
			PlayNotEnoughUnitsVO();
			Debug.LogWarning(string.Format("Can't start the multiman setpiece ({0}) as you didn't have enough actors selected", target.name));
			return;
		}
		List<MultiManCandidate> list = new List<MultiManCandidate>();
		List<Actor> list2 = null;
		if (GameController.Instance.IsFirstPerson)
		{
			list2 = gameplayController.ActorsOtherThanFPP;
			list2.Insert(0, GameController.Instance.mFirstPersonActor);
		}
		else
		{
			list2 = gameplayController.Selected;
		}
		if (list2 == null)
		{
			return;
		}
		for (int i = 0; i < list2.Count; i++)
		{
			Actor actor = list2[i];
			if (actor == null || actor.baseCharacter.IsInASetPiece)
			{
				continue;
			}
			float sqrMagnitude = (actor.GetPosition().xz() - position.xz()).sqrMagnitude;
			bool isInTheCaptureRange = false;
			if (contextMenu.MultiManCoverCluster != null)
			{
				if (actor.awareness.closestCoverPoint != null)
				{
					isInTheCaptureRange = contextMenu.MultiManCoverCluster.Includes(actor.awareness.closestCoverPoint);
				}
			}
			else
			{
				isInTheCaptureRange = contextMenu.IsInTPPCaptureRange(actor.GetPosition() - position, sqrMagnitude);
			}
			list.Add(new MultiManCandidate(actor, i, isInTheCaptureRange, sqrMagnitude, Mathf.Abs(actor.GetPosition().y - position.y)));
		}
		if (GameController.Instance.IsFirstPerson)
		{
			int num = 0;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].IsInTheCaptureRange)
				{
					num++;
				}
			}
			if (num < numActorsRequired)
			{
				PlayNotEnoughUnitsVO();
				Debug.LogWarning(string.Format("Failed to find enough near units for the multiman set piece {0}", target.name));
				return;
			}
		}
		else if (list.Count <= 1 || !list.Exists((MultiManCandidate c) => c.IsInTheCaptureRange))
		{
			Debug.LogWarning(string.Format("Failed to find any near units for the multiman set piece {0}", target.name));
			return;
		}
		list.Sort((MultiManCandidate a, MultiManCandidate b) => a.SqrMagDistance.CompareTo(b.SqrMagDistance));
		list.Sort((MultiManCandidate a, MultiManCandidate b) => a.YDiff.CompareTo(2.5f));
		for (int k = 0; k < list.Count; k++)
		{
			if (gameplayController.IsSelectedLeader(list[k].ActorRef) && (list2.Count <= 2 || !(list[k].YDiff >= 2f)))
			{
				MultiManCandidate item = list[k];
				list.RemoveAt(k);
				list.Insert(0, item);
				break;
			}
		}
		List<Actor> list3 = new List<Actor>(numActorsRequired);
		for (int l = 0; l < numActorsRequired; l++)
		{
			list3.Add(list[l].ActorRef);
			WaypointMarkerManager.Instance.RemoveMarker(list[l].ActorRef.gameObject);
		}
		TaskMultiCharacterSetPiece taskMultiCharacterSetPiece = new TaskMultiCharacterSetPiece(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, target, list3, contextMenu);
		RegisterTaskAsPlayerIssued(taskMultiCharacterSetPiece);
		taskMultiCharacterSetPiece.FixedActorOrder();
		UpdatePlayerSquadTetherPoint(position, list3);
	}

	public static void PlayNotEnoughUnitsVO()
	{
		if (!AlreadyPlayingVaultVO())
		{
			int num = UnityEngine.Random.Range(1, 7);
			if (num == 2)
			{
				num += UnityEngine.Random.Range(1, 4);
			}
			string imp = "S_PICKUP_TWOMAN_PROMPT_0" + num;
			DialogueManager dialogueManager = UnityEngine.Object.FindObjectOfType(typeof(DialogueManager)) as DialogueManager;
			if (dialogueManager != null)
			{
				dialogueManager.PlayDialogue(imp);
			}
		}
	}

	public static bool AlreadyPlayingVaultVO()
	{
		for (int i = 1; i < 8; i++)
		{
			string text = "S_PICKUP_TWOMAN_PROMPT_0" + i;
			if (CommonHudController.Instance.MissionDialogueQueue.AlreadyInQueue(text))
			{
				return true;
			}
		}
		return false;
	}

	public static void OrderPickUp(GameplayController gameplayController, CMPickUp cm)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, cm.IntelToPickUp.transform.position);
		if (!(nearestSelected == null) && !nearestSelected.baseCharacter.IsInASetPiece && !nearestSelected.tasks.IsRunningTask<TaskPickUp>())
		{
			new TaskCacheStanceState(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelected);
			RegisterTaskAsPlayerIssued(new TaskPickUp(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, cm));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			GameplayController.Instance().HideGhostPreview(nearestSelected);
			UpdatePlayerSquadTetherPoint(cm.IntelToPickUp.transform.position, list);
		}
	}

	public static void OrderAmmoCache(GameplayController gameplayController, CMAmmoCache cm)
	{
		if (!(cm == null))
		{
			Actor nearestSelected = GetNearestSelected(gameplayController, cm.transform.position);
			if (!(nearestSelected == null))
			{
				RegisterTaskAsPlayerIssued(new TaskUseAmmoCache(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, cm));
				List<Actor> list = new List<Actor>();
				list.Add(nearestSelected);
				GameplayController.Instance().HideGhostPreview(nearestSelected);
				UpdatePlayerSquadTetherPoint(cm.transform.position, list);
			}
		}
	}

	public static void OrderMysteryCache(GameplayController gameplayController, CMMysteryCache cm)
	{
		if (!(cm == null))
		{
			Actor nearestSelected = GetNearestSelected(gameplayController, cm.transform.position);
			if (!(nearestSelected == null))
			{
				RegisterTaskAsPlayerIssued(new TaskUseMysteryCache(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, cm));
				List<Actor> list = new List<Actor>();
				list.Add(nearestSelected);
				GameplayController.Instance().HideGhostPreview(nearestSelected);
				UpdatePlayerSquadTetherPoint(cm.transform.position, list);
			}
		}
	}

	public static void RegisterTaskAsPlayerIssued(Task task)
	{
		task.ConfigFlags = Task.Config.IssuedByPlayerRequest;
	}

	public static void UpdatePlayerUnitTetherPoint(Vector3 position, Actor actor)
	{
		List<Actor> list = new List<Actor>();
		list.Add(actor);
		UpdatePlayerSquadTetherPoint(position, list);
	}

	public static void UpdatePlayerSquadTetherPoint(Vector3 position, List<Actor> newTetherMembers)
	{
		TetheringManager tetheringManager = TetheringManager.Instance();
		tetheringManager.RefreshPlayerSquadTethers(position, newTetherMembers);
	}

	public static Actor GetNearestSelected(GameplayController gameplayController, Vector3 target)
	{
		return GetNearestSelected(gameplayController, target, typeof(DefinitelyNotATask));
	}

	public static Actor GetNearestSelected(GameplayController gameplayController, Vector3 target, Type excludeIfHasTask)
	{
		if (GameController.Instance.mFirstPersonActor != null)
		{
			return GameController.Instance.mFirstPersonActor;
		}
		Actor result = null;
		float num = float.MaxValue;
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput && !item.tasks.IsRunningTask<TaskCarry>() && (excludeIfHasTask == null || !item.tasks.IsRunningTask(excludeIfHasTask)))
			{
				float sqrMagnitude = (target - item.GetPosition()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = item;
				}
			}
		}
		return result;
	}

	private static Actor GetNearestSelectedForGrenadeOrClaymore(GameplayController gameplayController, Vector3 target)
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.tasks.IsRunningTask<TaskBreach>() || a.tasks.IsRunningTask<TaskPeekaboo>())
			{
				return null;
			}
		}
		if (GameController.Instance.mFirstPersonActor != null)
		{
			return GameController.Instance.mFirstPersonActor;
		}
		Actor result = null;
		float num = float.MaxValue;
		foreach (Actor item in gameplayController.Selected)
		{
			if (!item.tasks.RunningTaskDeniesPlayerInput && !item.baseCharacter.Carried && !item.tasks.IsRunningTask<TaskCarry>() && !item.tasks.IsRunningTask<TaskHack>())
			{
				float sqrMagnitude = (target - item.GetPosition()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = item;
				}
			}
		}
		return result;
	}

	public static bool IsAnySelectedPlayerSquadMemberInInterior(GameplayController gameplayController, BuildingWithInterior interior)
	{
		foreach (Actor item in gameplayController.Selected)
		{
			if (item.realCharacter.Location == interior && interior != null)
			{
				return true;
			}
		}
		return false;
	}

	public static void OrderUseAlarmPanel(GameplayController gameplayController, AlarmPanel alarmPanel)
	{
		Actor nearestSelected = GetNearestSelected(gameplayController, alarmPanel.transform.position);
		if (!(nearestSelected == null))
		{
			new TaskCacheStanceState(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.Default, nearestSelected);
			RegisterTaskAsPlayerIssued(new TaskUseAlarmPanel(nearestSelected.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, alarmPanel));
			List<Actor> list = new List<Actor>();
			list.Add(nearestSelected);
			UpdatePlayerSquadTetherPoint(alarmPanel.transform.position, list);
		}
	}
}
