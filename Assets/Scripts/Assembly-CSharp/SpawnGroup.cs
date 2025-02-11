using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnGroup
{
	public int MaxSimultaneousEnemies = 5;

	public int RandomVariance;

	public int TotalEnemiesPerSpawner = 1;

	public int SpawnDelay = 1;

	public int SpawnDelayVarience;

	public bool UsePlayerRelativeSpawnRules;

	public GameObject SpawnerCoordinator;

	public List<GameObject> EnemySpawners;
}
