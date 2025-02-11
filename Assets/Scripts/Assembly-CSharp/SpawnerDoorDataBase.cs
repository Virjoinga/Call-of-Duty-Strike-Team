using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnerDoorDataBase
{
	public List<ActorDescriptor> Actors;

	public List<RoutineDescriptor> Routines;

	public List<RoutineDescriptor> AlertedRoutines;

	public List<RoutineDescriptor> SpecialistRoutines;

	public string EntityTypeTag = "Spawned";

	public int SpawnCount;

	public int TotalToSpawn;

	public bool InfiniteSpawn;

	public bool SpawnOnlyOnMessage;

	public float Delay = 2f;

	public float DelayVarience;

	public float Speed = 2f;

	public float TimeToWalkOut = 1.8f;

	public BehaviourController.AlertState SpawnedAlertState = BehaviourController.AlertState.Focused;

	public GameObject StaticTether;

	public bool StartOnMessage;

	public GameObject NotifyOnAllDead;

	public string NotifyFunction;

	public List<GameObject> NotifyGroupOnAllDead;

	public List<string> NotifyGroupFunction;

	public GameObject EventsList;

	public GameObject PreferredTarget;

	public bool clearEventsOnDeactivate;

	public bool OnlyDamagedByPlayer;

	public AssaultParams AssaultParameters = new AssaultParams();

	public bool DontDropAmmo;

	public QuickDestination[] quickDestinations;

	public void ResolveGuidLinks()
	{
		if (quickDestinations != null)
		{
			QuickDestination[] array = quickDestinations;
			foreach (QuickDestination quickDestination in array)
			{
				quickDestination.ResolveGuidLinks();
			}
		}
	}
}
