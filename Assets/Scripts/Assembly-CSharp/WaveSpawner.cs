using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : SpawnerBase
{
	public ActorDescriptor Spawn;

	public string EntityTypeTag;

	public int Count;

	public int GlobalLimit;

	public List<Transform> RandomTargets;

	public float PauseDuration = 1f;

	public BehaviourController.AlertState SpawnedAlertState = BehaviourController.AlertState.Focused;

	public AITetherPoint StaticTether;

	private void Start()
	{
		Initialise(PauseDuration);
	}

	private void Update()
	{
		Process(Count, PauseDuration, Spawn, StaticTether, base.tag, SpawnedAlertState, GlobalLimit);
	}

	public void GenerateEnemy(GameObject coordinator)
	{
		mCoordinator = coordinator;
		Process(Count, PauseDuration, Spawn, StaticTether, base.tag, SpawnedAlertState, GlobalLimit);
	}

	private void OnDrawGizmos()
	{
		DebugDraw(Spawn);
	}
}
