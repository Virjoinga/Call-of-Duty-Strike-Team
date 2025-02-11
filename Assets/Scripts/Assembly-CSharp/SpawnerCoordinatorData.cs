using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnerCoordinatorData
{
	public List<GameObject> MultiSpawners;

	public GameObject EventsListOverride;

	public int MaxSimultaneousEnemies;

	public int RandomVarience;

	public int TotalEnemies;

	public bool InfiniteSpawn;

	public float SpawnDelay = 2f;

	public float SpawnDelayVarience;

	public GameObject NotifyOnAllDead;

	public string NotifyFunction;

	public List<GameObject> GroupObjectToCall;

	public List<string> GroupFunctionToCall;

	public QuickDestination[] QuickDestinations;

	public bool ApplyQuickDestinations;

	public bool LeaveDoorOpenBetweenSpawns;

	public bool UsePlayerRelativeSpawnRules;

	public List<ActorDescriptor> OverrideActors;

	public void ResolveGuidLinks()
	{
		if (QuickDestinations != null)
		{
			QuickDestination[] quickDestinations = QuickDestinations;
			foreach (QuickDestination quickDestination in quickDestinations)
			{
				quickDestination.ResolveGuidLinks();
			}
		}
	}
}
