using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerMulti : MonoBehaviour
{
	public List<ActorDescriptor> ActorDescriptors;

	public List<Transform> SpawnPoints;

	public int MaxSimActors = 1;

	public int MaxTotalSpawn = -1;

	public float RandomSpawnTime = 1f;

	public string EntityType;

	public bool AttackOnSpawn = true;

	public bool StartOnTrigger = true;

	public bool SpawnInvincible;

	public AITetherPoint StaticTether;

	private bool mRunning;

	private int mSpawnedAlive;

	private int mSpawnCount;

	private int[] mRandNumbers = new int[99];

	private void Start()
	{
		mRunning = !StartOnTrigger;
		GenerateRandomNumberRange(SpawnPoints.Count);
		mSpawnCount = 0;
		if (!StartOnTrigger)
		{
			StartCoroutine(MonitorEnemies());
		}
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		foreach (Transform spawnPoint in SpawnPoints)
		{
			Vector3 vector = spawnPoint.transform.position + new Vector3(0f, 0.5f, 0f);
			Gizmos.color = Color.white;
			Vector3 vector2 = vector + spawnPoint.transform.forward * 2f;
			Gizmos.DrawLine(vector, vector2);
			Quaternion quaternion = Quaternion.AngleAxis(45f, Vector3.up);
			Vector3 to = vector2 - quaternion * spawnPoint.transform.forward * 0.3f;
			Gizmos.DrawLine(vector2, to);
			Quaternion quaternion2 = Quaternion.AngleAxis(-45f, Vector3.up);
			Vector3 to2 = vector2 - quaternion2 * spawnPoint.transform.forward * 0.3f;
			Gizmos.DrawLine(vector2, to2);
		}
	}

	private void OnEnter()
	{
		mRunning = true;
		StartCoroutine(MonitorEnemies());
	}

	private void OnExit()
	{
		mRunning = false;
	}

	private IEnumerator MonitorEnemies()
	{
		while ((MaxTotalSpawn > 0 || MaxTotalSpawn == -1) && mRunning)
		{
			if (mSpawnedAlive < MaxSimActors)
			{
				if (MaxTotalSpawn != -1)
				{
					MaxTotalSpawn--;
				}
				yield return new WaitForSeconds(Random.Range(1f, RandomSpawnTime));
				Spawn();
			}
			yield return null;
		}
	}

	private void Spawn()
	{
		ActorDescriptor descriptor = ActorDescriptors[Random.Range(0, ActorDescriptors.Count)];
		Transform transform = SpawnPoints[mRandNumbers[mSpawnCount]];
		GameObject gameObject = SceneNanny.CreateActor(descriptor, transform.position, transform.rotation);
		gameObject.AddComponent<Entity>().Type = EntityType;
		Actor component = gameObject.GetComponent<Actor>();
		if (component != null)
		{
			component.realCharacter.EnableNavMesh(true);
			component.health.OnHealthEmpty += delegate
			{
				mSpawnedAlive--;
			};
			component.health.Invulnerable = SpawnInvincible;
			if (StaticTether != null && !component.behaviour.PlayerControlled)
			{
				component.tether.TetherToAITetherPoint(StaticTether);
			}
			if (AttackOnSpawn)
			{
				RouteToNearestPlayer(component);
			}
		}
		mSpawnedAlive++;
		mSpawnCount++;
		if (mSpawnCount == SpawnPoints.Count)
		{
			GenerateRandomNumberRange(SpawnPoints.Count);
			mSpawnCount = 0;
		}
	}

	private void RouteToNearestPlayer(Actor actor)
	{
		float num = float.MaxValue;
		GameObject gameObject = null;
		TaskManager tasks = actor.tasks;
		GameplayController gameplayController = GameplayController.Instance();
		Actor[] array = gameplayController.Selected.ToArray();
		Actor[] array2 = array;
		foreach (Actor actor2 in array2)
		{
			GameObject gameObject2 = actor2.gameObject;
			if (!actor2.realCharacter.IsDead() && WorldHelper.IsPlayerControlledActor(actor2))
			{
				float sqrMagnitude = (gameObject2.transform.position - actor.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					gameObject = gameObject2;
				}
			}
		}
		if (gameObject != null)
		{
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Run, gameObject.transform.position);
			new TaskRouteTo(tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType | Task.Config.AbortOnAlert | Task.Config.AbortOnVisibleEnemy, moveParams);
		}
	}

	private void GenerateRandomNumberRange(int RangeSize)
	{
		int i;
		for (i = 0; i < RangeSize; i++)
		{
			mRandNumbers[i] = i;
		}
		i = RangeSize;
		while (i > 1)
		{
			int num = Random.Range(0, RangeSize);
			i--;
			int num2 = mRandNumbers[num];
			mRandNumbers[num] = mRandNumbers[i];
			mRandNumbers[i] = num2;
		}
	}
}
