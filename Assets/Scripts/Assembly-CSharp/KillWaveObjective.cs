using System;
using System.Collections.Generic;
using UnityEngine;

public class KillWaveObjective : MissionObjective
{
	public KillWaveObjectiveData m_KillInterface;

	public List<SpawnerBase> Spawners;

	public List<SpawnerCoordinator> SpawnCoordinators;

	public int KillOverride;

	private int mNumKillsNeeded;

	public override void Start()
	{
		base.Start();
		foreach (KillObjectMonitor monitor in m_KillInterface.Monitors)
		{
			TBFAssert.DoAssert(monitor != null, "KillWaveObjective - Null hole in Monitors list");
			TBFAssert.DoAssert(monitor.Actor != null, "KillWaveObjective - Null ActorWrapper in Monitors list");
			monitor.Actor.TargetActorChanged += Setup;
			mNumKillsNeeded++;
		}
		foreach (SpawnerBase spawner in Spawners)
		{
			spawner.AddEventHandler(SpawnEvent);
			if (spawner as WaveSpawner != null)
			{
				mNumKillsNeeded += (spawner as WaveSpawner).GlobalLimit;
			}
			if (spawner as SpawnerDoor != null)
			{
				mNumKillsNeeded += (spawner as SpawnerDoor).m_Interface.TotalToSpawn;
			}
			if (spawner as SpawnerRoof != null)
			{
				mNumKillsNeeded += (spawner as SpawnerRoof).m_Interface.TotalToSpawn;
			}
			if (spawner as Spawner != null)
			{
				mNumKillsNeeded++;
			}
		}
		foreach (SpawnerCoordinator spawnCoordinator in SpawnCoordinators)
		{
			if (!(spawnCoordinator != null))
			{
				continue;
			}
			mNumKillsNeeded += spawnCoordinator.m_Interface.TotalEnemies;
			foreach (GameObject multiSpawner in spawnCoordinator.m_Interface.MultiSpawners)
			{
				SpawnerBase[] componentsInChildren = multiSpawner.GetComponentsInChildren<SpawnerBase>();
				foreach (SpawnerBase spawnerBase in componentsInChildren)
				{
					spawnerBase.AddEventHandler(SpawnEvent);
				}
			}
		}
		if (KillOverride > 0)
		{
			mNumKillsNeeded = KillOverride;
		}
		TBFAssert.DoAssert(mNumKillsNeeded != 0, "KillWaveObjective - number to kill is ZERO, either set the KillOverride to the num you want dead or set global count in the wave spawners");
	}

	private void Setup(object sender)
	{
		GameObject gameObject = sender as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		Actor component = gameObject.GetComponent<Actor>();
		TBFAssert.DoAssert(component != null, "KillWaveObjective - Not monitoring an actor");
		foreach (KillObjectMonitor monitor in m_KillInterface.Monitors)
		{
			if (component != monitor.Actor.GetActor() || !(monitor.Target == null))
			{
				continue;
			}
			monitor.Target = component.health;
			if (monitor.Target != null)
			{
				monitor.Target.OnHealthEmpty += OnHealthEmpty;
			}
			break;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (KillObjectMonitor monitor in m_KillInterface.Monitors)
		{
			if (monitor.Actor != null)
			{
				monitor.Actor.TargetActorChanged -= Setup;
			}
			if (monitor.Target != null)
			{
				monitor.Target.OnHealthEmpty -= OnHealthEmpty;
			}
		}
		foreach (SpawnerBase spawner in Spawners)
		{
			if (spawner != null)
			{
				spawner.RemoveEventHandler(SpawnEvent);
			}
		}
	}

	private void OnHealthEmpty(object sender, EventArgs args)
	{
		foreach (KillObjectMonitor monitor in m_KillInterface.Monitors)
		{
			if (monitor.Target != null)
			{
				mNumKillsNeeded--;
				break;
			}
		}
		if (mNumKillsNeeded == 0)
		{
			Pass();
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
			m_KillInterface.Monitors.Add(killObjectMonitor);
		}
	}
}
