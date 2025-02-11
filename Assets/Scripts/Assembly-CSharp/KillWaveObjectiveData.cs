using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KillWaveObjectiveData
{
	public List<KillObjectMonitor> Monitors = new List<KillObjectMonitor>();

	public List<GameObject> Spawners = new List<GameObject>();

	public void CopyContainerData(KillWaveObjective kwo)
	{
		kwo.Spawners.Clear();
		kwo.SpawnCoordinators.Clear();
		foreach (GameObject spawner in Spawners)
		{
			if (spawner != null)
			{
				kwo.Spawners.AddRange(spawner.GetComponentsInChildren<SpawnerBase>());
				kwo.SpawnCoordinators.AddRange(spawner.GetComponentsInChildren<SpawnerCoordinator>());
			}
		}
	}
}
