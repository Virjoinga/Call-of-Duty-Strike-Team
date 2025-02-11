using System;
using System.Collections.Generic;
using UnityEngine;

public class Condition_TargetsDead : Condition
{
	private List<KillObjectMonitor> monitors = new List<KillObjectMonitor>();

	public List<GameObject> spawnerObjects = new List<GameObject>();

	public bool killAllTargets;

	public int targetAmountToKill;

	private List<SpawnerBase> spawners = new List<SpawnerBase>();

	private List<SpawnerCoordinator> spawnCoordinators = new List<SpawnerCoordinator>();

	private int totalTargets;

	private int numberKilled;

	private void Start()
	{
		foreach (GameObject spawnerObject in spawnerObjects)
		{
			if (spawnerObject != null)
			{
				spawners.AddRange(spawnerObject.GetComponentsInChildren<SpawnerBase>());
				spawnCoordinators.AddRange(spawnerObject.GetComponentsInChildren<SpawnerCoordinator>());
			}
		}
		foreach (SpawnerBase spawner in spawners)
		{
			spawner.AddEventHandler(SpawnEvent);
			if (spawner as WaveSpawner != null)
			{
				totalTargets += (spawner as WaveSpawner).GlobalLimit;
			}
			if (spawner as SpawnerDoor != null)
			{
				totalTargets += (spawner as SpawnerDoor).m_Interface.TotalToSpawn;
			}
			if (spawner as SpawnerRoof != null)
			{
				totalTargets += (spawner as SpawnerRoof).m_Interface.TotalToSpawn;
			}
			if (spawner as Spawner != null)
			{
				totalTargets++;
			}
		}
		foreach (SpawnerCoordinator spawnCoordinator in spawnCoordinators)
		{
			if (!(spawnCoordinator != null))
			{
				continue;
			}
			totalTargets += spawnCoordinator.m_Interface.TotalEnemies;
			foreach (GameObject multiSpawner in spawnCoordinator.m_Interface.MultiSpawners)
			{
				SpawnerBase[] componentsInChildren = multiSpawner.GetComponentsInChildren<SpawnerBase>();
				foreach (SpawnerBase spawnerBase in componentsInChildren)
				{
					spawnerBase.AddEventHandler(SpawnEvent);
				}
			}
		}
	}

	public override bool Value()
	{
		if (killAllTargets)
		{
			return numberKilled >= totalTargets;
		}
		return numberKilled >= targetAmountToKill;
	}

	private void OnDestroy()
	{
		foreach (KillObjectMonitor monitor in monitors)
		{
			if (monitor.Target != null)
			{
				monitor.Target.OnHealthEmpty -= OnHealthEmpty;
			}
		}
		foreach (SpawnerBase spawner in spawners)
		{
			if (spawner != null)
			{
				spawner.RemoveEventHandler(SpawnEvent);
			}
		}
	}

	private void OnHealthEmpty(object sender, EventArgs args)
	{
		foreach (KillObjectMonitor monitor in monitors)
		{
			if (monitor.Target != null)
			{
				numberKilled++;
				break;
			}
		}
	}

	private void SpawnEvent(object spawned)
	{
		KillObjectMonitor killObjectMonitor = new KillObjectMonitor();
		killObjectMonitor.Actor = null;
		GameObject gameObject = spawned as GameObject;
		if (!(gameObject == null))
		{
			Actor component = gameObject.GetComponent<Actor>();
			if (component != null)
			{
				killObjectMonitor.Target = component.health;
				killObjectMonitor.Target.OnHealthEmpty += OnHealthEmpty;
			}
			monitors.Add(killObjectMonitor);
		}
	}
}
