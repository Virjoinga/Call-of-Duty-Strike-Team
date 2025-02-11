using System.Collections.Generic;
using UnityEngine;

public static class SquadFormation
{
	public class FormationSlot
	{
		public Vector3 Position;

		public Vector3 Facing;

		public Actor Assigned;

		public float Cost;

		public FormationSlot(Vector3 position)
		{
			Position = position;
			Reset();
		}

		public FormationSlot(Vector3 position, Vector3 facing)
			: this(position)
		{
			Facing = facing;
		}

		public void Reset()
		{
			Assigned = null;
			Cost = float.MaxValue;
		}
	}

	public static void PreviewSquadMove(List<Actor> squad, Vector3 destination, BaseCharacter.MovementStyle speed, float formationSpacingRow, float formationSpacingColumn, float formationCoverSearchRadius)
	{
		Vector3 vector = destination - squad[0].GetPosition();
		vector.y = 0f;
		WaypointMarker.State state = WaypointMarker.State.Walk;
		if (speed == BaseCharacter.MovementStyle.Run)
		{
			state = WaypointMarker.State.Run;
		}
		if (vector.sqrMagnitude > 0f)
		{
			vector.Normalize();
			List<FormationSlot> list = new List<FormationSlot>();
			BuildLayout(list, destination, vector, squad, formationSpacingRow, formationSpacingColumn);
			List<Actor> list2 = new List<Actor>(squad);
			AssignSlotsByPenalisingFurthest(list2, list, list2.Count, list2.Count);
			for (int i = 0; i < list.Count; i++)
			{
				FormationSlot formationSlot = list[i];
				Actor assigned = formationSlot.Assigned;
				WaypointMarker waypointMarker;
				if (assigned != null && assigned.realCharacter.Carried == null && (i > 0 || list.Count == 1))
				{
					CoverPointCore coverNearestSpecifiedPosition = assigned.awareness.GetCoverNearestSpecifiedPosition(formationSlot.Position, formationCoverSearchRadius, 0f, false, 0f);
					if (coverNearestSpecifiedPosition != null)
					{
						coverNearestSpecifiedPosition.EarMark();
						waypointMarker = WaypointMarkerManager.Instance.AddMarker(assigned.gameObject, coverNearestSpecifiedPosition.gamePos, WaypointMarker.Type.Undefined, state);
						waypointMarker.transform.forward = coverNearestSpecifiedPosition.AnimationFacing;
						waypointMarker.SetCoverPoint(coverNearestSpecifiedPosition);
						waypointMarker.GetComponent<SelectableObject>().quickType = SelectableObject.QuickType.PreviewGhost;
						waypointMarker.PreviewPath();
						continue;
					}
				}
				waypointMarker = WaypointMarkerManager.Instance.AddMarker(assigned.gameObject, formationSlot.Position, WaypointMarker.Type.OpenGround, state);
				waypointMarker.transform.forward = vector;
				waypointMarker.SetCoverPoint(null);
				waypointMarker.GetComponent<SelectableObject>().quickType = SelectableObject.QuickType.PreviewGhost;
				waypointMarker.PreviewPath();
			}
		}
		CoverPointCore.ClearEarMarks();
	}

	public static void OrderSquadMove(List<Actor> squad, Vector3 destination, BaseCharacter.MovementStyle speed, float formationSpacingRow, float formationSpacingColumn, float formationCoverSearchRadius, float unitCoverSearchRadius, bool addWaypointMarker)
	{
		Vector3 vector = destination - squad[0].GetPosition();
		vector.y = 0f;
		WaypointMarker.State state = WaypointMarker.State.Walk;
		if (speed == BaseCharacter.MovementStyle.Run)
		{
			state = WaypointMarker.State.Run;
		}
		if (!(vector.sqrMagnitude > 0f))
		{
			return;
		}
		CoverPointCore coverPointCore = null;
		vector.Normalize();
		destination = ValidateDestinationAgainstNavMesh(squad[0], destination);
		if (squad[0].realCharacter != null && squad[0].realCharacter.Carried == null)
		{
			coverPointCore = squad[0].awareness.GetCoverNearestSpecifiedPosition(destination, unitCoverSearchRadius, 0f, false, 0f);
		}
		List<FormationSlot> list = new List<FormationSlot>();
		BuildLayout(list, destination, vector, squad, formationSpacingRow, formationSpacingColumn, coverPointCore);
		List<Actor> list2 = new List<Actor>(squad);
		AssignSlotsByPenalisingFurthest(list2, list, list2.Count, list2.Count);
		for (int i = 0; i < list.Count; i++)
		{
			FormationSlot formationSlot = list[i];
			Actor assigned = formationSlot.Assigned;
			if (assigned.baseCharacter.IsPerformingUninterruptableSetPiece)
			{
				continue;
			}
			TaskEnter taskEnter = assigned.tasks.GetRunningTask(typeof(TaskEnter)) as TaskEnter;
			if (taskEnter != null && taskEnter.Container != null && taskEnter.HasFinishedEntering)
			{
				OrdersHelper.OrderExit(GameplayController.instance, taskEnter.Container);
				break;
			}
			TaskHack taskHack = assigned.tasks.GetRunningTask(typeof(TaskHack)) as TaskHack;
			if (taskHack != null)
			{
				if (taskHack.HasStarted)
				{
					assigned.tasks.CancelTasks<TaskHack>();
					break;
				}
				taskHack.FinishQuick = true;
				assigned.tasks.CancelTasks<TaskCacheStanceState>();
				assigned.tasks.CancelTasks<TaskSetPiece>();
				assigned.tasks.CancelTasks<TaskHack>();
			}
			assigned.tasks.CancelTasks(typeof(TaskStealthKill));
			assigned.tasks.CancelTasks(typeof(TaskRouteTo));
			assigned.tasks.CancelTasks(typeof(TaskMoveTo));
			assigned.tasks.CancelTasks(typeof(TaskMoveToCover));
			assigned.tasks.CancelTasks(typeof(TaskFollowMovingTarget));
			assigned.tasks.CancelTasks(typeof(TaskShoot));
			assigned.tasks.CancelTasks(typeof(TaskCacheStanceState));
			assigned.tasks.CancelTasks(typeof(TaskDropClaymore));
			assigned.tasks.CancelTasks(typeof(TaskPlantC4));
			assigned.tasks.CancelTasks(typeof(TaskPickUp));
			assigned.behaviour.ClearAimedShotTarget();
			assigned.behaviour.ClearSuppressionTarget();
			if (assigned.realCharacter != null)
			{
				if (assigned.realCharacter.Carried == null && i > 0)
				{
					coverPointCore = null;
					if (assigned.awareness.CanAnyEnemiesSeeMe())
					{
						coverPointCore = assigned.awareness.GetValidDefensiveCoverNearestSpecifiedPosition(formationSlot.Position, formationCoverSearchRadius, 0f, false);
					}
					if (coverPointCore == null)
					{
						coverPointCore = assigned.awareness.GetCoverNearestSpecifiedPosition(formationSlot.Position, formationCoverSearchRadius, 0f, false, 0f);
					}
				}
				if (coverPointCore != null)
				{
					InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(speed);
					inheritableMovementParams.holdCoverWhenBored = true;
					inheritableMovementParams.holdCoverWhenFlanked = true;
					OrdersHelper.RegisterTaskAsPlayerIssued(new TaskMoveToCover(assigned.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, coverPointCore, inheritableMovementParams));
					WaypointMarker waypointMarker = WaypointMarkerManager.Instance.AddMarker(assigned.gameObject, coverPointCore.gamePos, WaypointMarker.Type.Undefined, state);
					waypointMarker.SetCoverPoint(coverPointCore);
					waypointMarker.GetComponent<SelectableObject>().quickType = SelectableObject.QuickType.WayPointMarker;
					continue;
				}
			}
			InheritableMovementParams inheritableMovementParams2 = new InheritableMovementParams(speed, formationSlot.Position);
			if (speed == BaseCharacter.MovementStyle.Walk)
			{
				inheritableMovementParams2.stanceAtEnd = InheritableMovementParams.StanceOrder.CrouchFromStealth;
			}
			if (assigned.realCharacter.Carried != null)
			{
				inheritableMovementParams2.forceFaceForwards = true;
			}
			TaskRouteTo task = new TaskRouteTo(assigned.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, inheritableMovementParams2);
			OrdersHelper.RegisterTaskAsPlayerIssued(task);
			if (addWaypointMarker)
			{
				WaypointMarker waypointMarker = WaypointMarkerManager.Instance.AddMarker(assigned.gameObject, formationSlot.Position, WaypointMarker.Type.OpenGround, state);
				waypointMarker.transform.forward = vector;
				waypointMarker.SetCoverPoint(null);
				waypointMarker.GetComponent<SelectableObject>().quickType = SelectableObject.QuickType.WayPointMarker;
			}
		}
	}

	public static Vector3 ValidateDestinationAgainstNavMesh(Actor soldier, Vector3 desiredDestination)
	{
		Vector3 result = desiredDestination;
		if (soldier.navAgent.enabled)
		{
			NavMeshPath navMeshPath = new NavMeshPath();
			if (WorldHelper.CalculatePath_AvoidingMantlesWhenCarrying(soldier, desiredDestination, navMeshPath))
			{
				if (navMeshPath.status == NavMeshPathStatus.PathInvalid)
				{
					result = soldier.GetPosition();
				}
				result = navMeshPath.corners[navMeshPath.corners.Length - 1];
			}
			else
			{
				int navMeshLayerFromName = NavMesh.GetNavMeshLayerFromName("Default");
				NavMeshHit navMeshHit = NavMeshUtils.SampleNavMesh(desiredDestination, 1 << navMeshLayerFromName);
				result = ((!navMeshHit.hit) ? soldier.GetPosition() : navMeshHit.position);
			}
		}
		return result;
	}

	public static float GetPathTravelCost(Actor soldier, Vector3 desiredDestination)
	{
		float num = float.MaxValue;
		if (soldier.navAgent.enabled)
		{
			NavMeshPath navMeshPath = new NavMeshPath();
			if (soldier.navAgent.CalculatePath(desiredDestination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
			{
				num = 0f;
				for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
				{
					num += (navMeshPath.corners[i + 1] - navMeshPath.corners[i]).magnitude;
				}
			}
		}
		return num;
	}

	public static void AssignSlotsByPenalisingFurthest(List<Actor> actors, List<FormationSlot> layout, int charCount, int count)
	{
		float[] array = new float[count];
		int[] array2 = new int[count];
		for (int i = 0; i < count; i++)
		{
			float num = -1f;
			int num2 = -1;
			for (int j = 0; j < count; j++)
			{
				array[j] = float.MaxValue;
			}
			for (int k = 0; k < count; k++)
			{
				if (!(layout[k].Assigned == null))
				{
					continue;
				}
				for (int l = 0; l < charCount; l++)
				{
					if (actors[l] != null)
					{
						float num3 = Vector3.SqrMagnitude(actors[l].transform.position - layout[k].Position);
						if (num3 > num)
						{
							num = num3;
							num2 = k;
						}
						if (num3 < array[k])
						{
							array2[k] = l;
							array[k] = num3;
						}
					}
				}
			}
			layout[num2].Assigned = actors[array2[num2]];
			actors[array2[num2]] = null;
		}
	}

	private static void AssignSlotsByNearest(List<Actor> squad, List<FormationSlot> layout)
	{
		List<Actor> list = new List<Actor>(squad);
		while (list.Count > 0)
		{
			foreach (FormationSlot item in layout)
			{
				if (item.Assigned != null)
				{
					continue;
				}
				float num = float.MaxValue;
				foreach (Actor item2 in list)
				{
					float sqrMagnitude = (item.Position - item2.GetPosition()).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						item.Assigned = item2;
						item.Cost = num;
					}
				}
			}
			foreach (FormationSlot item3 in layout)
			{
				foreach (FormationSlot item4 in layout)
				{
					if (item3 != item4 && item3.Assigned == item4.Assigned)
					{
						if (item3.Cost > item4.Cost)
						{
							item3.Reset();
						}
						else
						{
							item4.Reset();
						}
					}
				}
			}
			foreach (FormationSlot item5 in layout)
			{
				if (item5.Assigned != null)
				{
					list.Remove(item5.Assigned);
				}
			}
		}
	}

	private static void BuildLayout(List<FormationSlot> layout, Vector3 target, Vector3 facing, List<Actor> squad, float spacingRow, float spacingColumn)
	{
		target = ValidateDestinationAgainstNavMesh(squad[0], target);
		FormationSlot item = new FormationSlot(target);
		layout.Add(item);
		for (int i = 1; i < squad.Count; i++)
		{
			int num = (i + 1) / 2;
			bool flag = (i + 1) % 2 == 0;
			Vector3 desiredDestination = target;
			desiredDestination -= facing * num * spacingColumn;
			if (num < 2)
			{
				Quaternion quaternion = Quaternion.AngleAxis((!flag) ? (-90f) : 90f, Vector3.up);
				desiredDestination += quaternion * facing * num * spacingRow;
			}
			desiredDestination = ValidateDestinationAgainstNavMesh(squad[i], desiredDestination);
			item = new FormationSlot(desiredDestination);
			layout.Add(item);
		}
	}

	private static void TryAdjacentCover(List<Actor> squad, ref int squindex, List<FormationSlot> layout, int adjacent)
	{
		if (adjacent == -1 || squindex >= squad.Count)
		{
			return;
		}
		CoverPointCore coverPointCore = NewCoverPointManager.Instance().coverPoints[adjacent];
		if (squad[squindex].awareness.CanAnyEnemiesSeeMe())
		{
			if (!coverPointCore.AvailableAndDesirable(squad[squindex]))
			{
				return;
			}
		}
		else if (!coverPointCore.Available(squad[squindex]))
		{
			return;
		}
		FormationSlot item = new FormationSlot(coverPointCore.gamePos);
		layout.Add(item);
		squindex++;
	}

	private static void BuildLayout(List<FormationSlot> layout, Vector3 target, Vector3 facing, List<Actor> squad, float spacingRow, float spacingColumn, CoverPointCore snappedCover)
	{
		if (snappedCover == null)
		{
			BuildLayout(layout, target, facing, squad, spacingRow, spacingColumn);
			return;
		}
		FormationSlot item = new FormationSlot(target);
		layout.Add(item);
		int squindex = 1;
		TryAdjacentCover(squad, ref squindex, layout, snappedCover.adjacentLeft);
		TryAdjacentCover(squad, ref squindex, layout, snappedCover.adjacentRight);
		for (int i = squindex; i < squad.Count; i++)
		{
			int num = (i + 1) / 2;
			bool flag = (i + 1) % 2 == 0;
			Vector3 desiredDestination = target;
			desiredDestination -= facing * num * spacingColumn;
			if (num < 2)
			{
				Quaternion quaternion = Quaternion.AngleAxis((!flag) ? (-90f) : 90f, Vector3.up);
				desiredDestination += quaternion * facing * num * spacingRow;
			}
			desiredDestination = ValidateDestinationAgainstNavMesh(squad[i], desiredDestination);
			item = new FormationSlot(desiredDestination);
			layout.Add(item);
		}
	}
}
