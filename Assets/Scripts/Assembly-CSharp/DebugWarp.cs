using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugWarp : MonoBehaviour
{
	public enum WarpRequirements
	{
		AllActorsRequiredInTrigger = 0,
		OnlyWarpFirstToTrigger = 1,
		WarpSpecificNumber = 2,
		AllActorsNotInDirectPlayerControl = 3
	}

	public DebugWarpData m_Interface;

	private bool triggered;

	public List<ActorWrapper> Actors;

	public List<Actor> ActorsToWarp;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (IsUnitOfInterest(other.gameObject))
		{
			ActorsToWarp.Add(other.gameObject.GetComponentInChildren<Actor>());
			switch (m_Interface.WarpRule)
			{
			case WarpRequirements.AllActorsRequiredInTrigger:
				if (ActorsToWarp.Count == Actors.Count)
				{
					DoWarp();
				}
				break;
			case WarpRequirements.WarpSpecificNumber:
				if (ActorsToWarp.Count == m_Interface.RequiredWarpNumber)
				{
					DoWarp();
				}
				break;
			case WarpRequirements.OnlyWarpFirstToTrigger:
			case WarpRequirements.AllActorsNotInDirectPlayerControl:
				DoWarp();
				break;
			}
		}
		if (triggered)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public void OnTriggerLeave(Collider other)
	{
		if (IsUnitOfInterest(other.gameObject))
		{
			ActorsToWarp.Remove(other.gameObject.GetComponentInChildren<Actor>());
		}
	}

	public void Activate()
	{
		switch (m_Interface.WarpRule)
		{
		case WarpRequirements.AllActorsRequiredInTrigger:
		case WarpRequirements.WarpSpecificNumber:
		{
			for (int i = 0; i < Actors.Count; i++)
			{
				ActorsToWarp.Add(Actors[i].GetActor().gameObject.GetComponentInChildren<Actor>());
			}
			break;
		}
		case WarpRequirements.OnlyWarpFirstToTrigger:
			ActorsToWarp.Add(Actors[0].GetActor().gameObject.GetComponentInChildren<Actor>());
			break;
		case WarpRequirements.AllActorsNotInDirectPlayerControl:
		{
			ActorsToWarp = ActorIdentIterator.AsList(GKM.PlayerControlledMask);
			Actor item = null;
			foreach (Actor item2 in ActorsToWarp)
			{
				if (GameplayController.Instance().IsSelectedLeader(item2))
				{
					item = item2;
				}
			}
			ActorsToWarp.Remove(item);
			break;
		}
		}
		DoWarp();
	}

	private void DoWarp()
	{
		if (m_Interface.WarpPosition != null)
		{
			m_Interface.WarpPositions.Insert(0, m_Interface.WarpPosition);
		}
		int num = 0;
		Vector3 pos = Vector3.zero;
		foreach (Actor item in ActorsToWarp)
		{
			if (num < m_Interface.WarpPositions.Count)
			{
				pos = m_Interface.WarpPositions[num].transform.position;
			}
			WarpActor(item, pos);
			num++;
		}
		triggered = true;
	}

	public static void WarpActor(Actor a, Vector3 pos)
	{
		WarpActor(a, pos, a.gameObject.transform.eulerAngles);
	}

	public static void WarpActor(Actor a, Vector3 pos, Vector3 rot)
	{
		if (a != null && a.realCharacter != null && !(a.health.Health <= 0f) && a.tasks.GetRunningTask<TaskMortallyWounded>() == null)
		{
			if (a.IsHidden)
			{
				a.ShowHide(true);
			}
			PoseModuleSharedData poseModuleSharedData = a.poseModuleSharedData;
			poseModuleSharedData.blend = 1f;
			poseModuleSharedData.onAxisTrans.position = pos;
			poseModuleSharedData.onAxisTrans.rotation = Quaternion.Euler(rot);
			poseModuleSharedData.idealOnAxisPos = poseModuleSharedData.onAxisTrans.position;
			poseModuleSharedData.idealOnAxisRot = poseModuleSharedData.onAxisTrans.rotation;
			poseModuleSharedData.BlendOntoAxis(0f, AnimDirector.BlendEasing.Linear, AnimDirector.BlendEasing.Linear);
			Type[] taskTypes = new Type[2]
			{
				typeof(TaskRoutine),
				typeof(TaskFirstPerson)
			};
			a.tasks.CancelTasksExcluding(taskTypes);
			WaypointMarkerManager.Instance.RemoveMarker(a.gameObject);
			a.awareness.dominantSound = DominantSoundType.Silence;
			a.awareness.currentNoiseRadius = 0f;
			if (a.navAgent.enabled)
			{
				a.navAgent.ResetPath();
			}
			a.navAgent.enabled = false;
			int navMeshLayerFromName = UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Default");
			UnityEngine.AI.NavMeshHit navMeshHit = NavMeshUtils.SampleNavMesh(pos, 1 << navMeshLayerFromName);
			if (navMeshHit.hit)
			{
				a.SetPosition(navMeshHit.position);
			}
			else
			{
				a.SetPosition(pos);
				Debug.LogError("Navmesh sampling fail");
			}
			a.realCharacter.transform.eulerAngles = rot;
			if (a.realCharacter.FirstPersonCamera != null)
			{
				a.realCharacter.FirstPersonCamera.Angles = rot;
			}
			List<Actor> list = new List<Actor>();
			list.Add(a);
			OrdersHelper.UpdatePlayerSquadTetherPoint(a.realCharacter.transform.position, list);
			a.navAgent.enabled = true;
		}
	}

	private bool IsUnitOfInterest(GameObject unitToCheck)
	{
		foreach (ActorWrapper actor in Actors)
		{
			if (actor != null && actor.GetGameObject() == unitToCheck)
			{
				return true;
			}
		}
		return false;
	}
}
