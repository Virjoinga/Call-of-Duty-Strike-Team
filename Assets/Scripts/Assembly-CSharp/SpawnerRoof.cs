using System.Collections;
using UnityEngine;

public class SpawnerRoof : SpawnerDoorBase
{
	protected override IEnumerator ProcessSpawn(ActorDescriptor spawn, AITetherPoint aiTetherPoint, string tag, BehaviourController.AlertState releaseAlerted, int globalLimit)
	{
		mSpawned = SceneNanny.CreateActor(spawn, SpawnFromPoint.position, SpawnFromPoint.rotation);
		yield return null;
		Actor actor = mSpawned.GetComponent<Actor>();
		mSpawned.AddComponent<Entity>().Type = tag;
		SpawnerUtils.InitialiseSpawnedActor(actor, StaticTether, m_Interface.SpawnedAlertState, PreferredTarget);
		actor.realCharacter.StartMovingManually();
		actor.health.Invulnerable = true;
		float timer = m_Interface.TimeToWalkOut;
		while (timer > 0f)
		{
			mSpawned.transform.position += Time.deltaTime * m_Interface.Speed * SpawnFromPoint.up;
			timer -= Time.deltaTime;
			yield return null;
		}
		actor.realCharacter.StopMovingManually();
		if (m_Interface.Routines != null && m_Interface.Routines.Count > 0 && m_Interface.Routines[mRandNumbers[RoutineIdx]] != null)
		{
			Task.Config flags = Task.Config.Default;
			actor.tasks.CancelTasks<TaskRoutine>();
			if (m_Interface.AlertedRoutines != null && m_Interface.Routines.Count == m_Interface.AlertedRoutines.Count)
			{
				RoutineDescriptor rd = m_Interface.AlertedRoutines[mRandNumbers[RoutineIdx]].gameObject.GetComponent<RoutineDescriptor>();
				rd.CreateTask(actor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default);
				flags |= Task.Config.AbortOnAlert;
			}
			m_Interface.Routines[mRandNumbers[RoutineIdx]].CreateTask(actor.tasks, TaskManager.Priority.LONG_TERM, flags, false);
			RoutineIdx++;
			if (RoutineIdx > m_Interface.Routines.Count)
			{
				RoutineIdx = 0;
			}
		}
		RegisterSpawn(actor, globalLimit);
		yield return new WaitForSeconds(0.8f);
		Actor spawnedActor = mSpawned.GetComponent<Actor>();
		if (spawnedActor != null)
		{
			AddEventsList(spawnedActor);
		}
		actor.health.Invulnerable = false;
		if (mCoordinator != null)
		{
			mCoordinator.SendMessage("EntitySpawned", mSpawned);
		}
		if (m_Interface.SpawnOnlyOnMessage)
		{
			mActivated = false;
		}
		mSpawningInProgress = false;
		yield return null;
	}
}
