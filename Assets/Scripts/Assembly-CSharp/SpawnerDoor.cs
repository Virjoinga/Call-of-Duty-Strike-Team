using System.Collections;
using UnityEngine;

public class SpawnerDoor : SpawnerDoorBase
{
	public GameObject PhysicalDoor;

	[HideInInspector]
	public Transform DoorPivot;

	[HideInInspector]
	public AnimationClip DoorOpenAnimation;

	[HideInInspector]
	public AnimationClip DoorCloseAnimation;

	private Animation mDoorPlayer;

	[HideInInspector]
	public BuildingDoor.eDoorType DoorType;

	private bool doorClosed = true;

	public TutorialEnemyOverrideData m_TutorialEnemyOverrides;

	public bool IsTutorialEnemy;

	public override void Start()
	{
		mDoorPlayer = PhysicalDoor.GetComponentInChildren<Animation>();
		if (mDoorPlayer != null && DoorOpenAnimation != null)
		{
			mDoorPlayer.AddClip(DoorOpenAnimation, "OpenDoor");
			mDoorPlayer.AddClip(DoorCloseAnimation, "CloseDoor");
			mDoorPlayer.cullingType = AnimationCullingType.AlwaysAnimate;
			mDoorPlayer.enabled = false;
			mDoorPlayer.enabled = true;
		}
		BoxCollider componentInChildren = base.gameObject.GetComponentInChildren<BoxCollider>();
		if (componentInChildren != null)
		{
			Object.Destroy(componentInChildren);
		}
		base.Start();
	}

	public IEnumerator Rotate(float amountToRotate)
	{
		float rotationDirection = Mathf.Sign(amountToRotate);
		amountToRotate = Mathf.Abs(amountToRotate);
		float degreesPerSecond = 360f;
		while (amountToRotate > 0f)
		{
			float maximumRotationThisFrame = degreesPerSecond * Time.deltaTime;
			float actualRotationThisFrame = Mathf.Min(maximumRotationThisFrame, amountToRotate);
			amountToRotate -= actualRotationThisFrame;
			if (PhysicalDoor == null)
			{
				break;
			}
			PhysicalDoor.transform.RotateAround(DoorPivot.position, DoorPivot.up, rotationDirection * actualRotationThisFrame);
			yield return null;
		}
	}

	protected override IEnumerator ProcessSpawn(ActorDescriptor spawn, AITetherPoint aiTetherPoint, string tag, BehaviourController.AlertState releaseAlerted, int globalLimit)
	{
		if (mCoordinator != null && (SpawnerCoordinator == null || SpawnerCoordinator.gameObject != mCoordinator))
		{
			SpawnerCoordinator = mCoordinator.GetComponent<SpawnerCoordinator>();
			attachedToCoordinator = SpawnerCoordinator != null;
		}
		bool canCloseDoor = false;
		if (attachedToCoordinator && SpawnerCoordinator != null && SpawnerCoordinator.CanCloseDoor())
		{
			canCloseDoor = true;
		}
		Vector3 pos = SpawnFromPoint.position;
		Quaternion rot = SpawnFromPoint.rotation;
		LogSpawnDebug("About to Spawn from ", null, base.transform.parent.transform.parent.gameObject);
		float spawnTimeGap2 = Time.time - SceneNanny.TimeOfLastSpawn;
		if ((double)spawnTimeGap2 < 0.1)
		{
			SceneNanny.TimeOfLastSpawn += 0.2f;
			spawnTimeGap2 = SceneNanny.TimeOfLastSpawn - Time.time;
			LogSpawnDebug("Delayed Spawn for " + spawnTimeGap2 + " from ", null, base.transform.parent.transform.parent.gameObject);
			yield return new WaitForSeconds(spawnTimeGap2);
		}
		if (GKM.AvailableSpawnSlots() < 1)
		{
			SpawnFailed(canCloseDoor);
			yield break;
		}
		mSpawned = SceneNanny.CreateActor(spawn, pos, rot);
		Actor actor = mSpawned.GetComponent<Actor>();
		LogSpawnDebug(" Spawned from ", actor, base.transform.parent.transform.parent.gameObject);
		actor.awareness.forcedClosestCover = closestCoverPoint;
		actor.health.OnlyDamagedByPlayer = m_Interface.OnlyDamagedByPlayer;
		mSpawned.AddComponent<Entity>().Type = tag;
		actor.realCharacter.StartMovingManually();
		actor.realCharacter.mPersistentAssaultParams.CopyFrom(m_Interface.AssaultParameters);
		actor.realCharacter.DontDropAmmo = m_Interface.DontDropAmmo;
		SpawnerUtils.InitialiseSpawnedActor(actor, StaticTether, m_Interface.SpawnedAlertState, PreferredTarget);
		if (m_TutorialEnemyOverrides != null && IsTutorialEnemy)
		{
			actor.awareness.FoV = m_TutorialEnemyOverrides.LineOfSightFOV;
			actor.awareness.PeripheralFoV = m_TutorialEnemyOverrides.LineOfSightPeripheralFOV;
			actor.awareness.VisionRange = m_TutorialEnemyOverrides.LineOfSightDistance;
			if ((bool)actor.ears)
			{
				actor.ears.Range = m_TutorialEnemyOverrides.AuditoryAwarenessRange;
				actor.ears.RangeSqr = actor.ears.Range * actor.ears.Range;
			}
			actor.realCharacter.DamageModifier = m_TutorialEnemyOverrides.DamageMultiplier;
			float health = actor.health.Health * m_TutorialEnemyOverrides.HealthModifier;
			actor.health.Initialise(0f, health, health);
		}
		if (actor != null)
		{
			AddEventsList(actor);
		}
		RegisterSpawn(actor, globalLimit);
		TaskDescriptor dummyTask = null;
		if (DoorType != 0 && doorClosed)
		{
			dummyTask = new GameObject("SpawnerDoorDummyTask").AddComponent<OverriddenDescriptor>();
			dummyTask.CreateTask(actor.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.DenyPlayerInput);
			if (mDoorPlayer != null && DoorOpenAnimation != null)
			{
				mDoorPlayer.animation["OpenDoor"].speed = 4f;
				mDoorPlayer.Play("OpenDoor");
				yield return new WaitForSeconds(DoorOpenAnimation.length);
			}
			else
			{
				yield return StartCoroutine(Rotate(-90f));
			}
			doorClosed = false;
		}
		else
		{
			yield return null;
		}
		float timer = m_Interface.TimeToWalkOut;
		while (timer > 0f)
		{
			if (mSpawned == null)
			{
				mSpawningInProgress = false;
				EmergencyDoorClose(canCloseDoor);
				yield break;
			}
			mSpawned.transform.position += Time.deltaTime * m_Interface.Speed * SpawnFromPoint.forward;
			timer -= Time.deltaTime;
			yield return null;
		}
		if (mSpawned == null)
		{
			SpawnFailed(canCloseDoor);
			yield break;
		}
		actor.realCharacter.StopMovingManually();
		actor.awareness.forcedClosestCover = null;
		actor.tasks.CancelTasks(typeof(TaskOverridden));
		if (dummyTask != null)
		{
			Object.Destroy(dummyTask.gameObject);
		}
		bool usingSpecialistRoutine = false;
		if ((actor.awareness.ChDefCharacterType == CharacterType.RPG || spawn.name == "Enemy_Sniper") && m_Interface.SpecialistRoutines != null && m_Interface.SpecialistRoutines.Count > 0)
		{
			foreach (RoutineDescriptor rd2 in m_Interface.SpecialistRoutines)
			{
				foreach (TaskDescriptor td in rd2.Tasks)
				{
					if ((td as RPGDescriptor != null && actor.awareness.ChDefCharacterType == CharacterType.RPG) || (td as SniperDescriptor != null && spawn.name == "Enemy_Sniper"))
					{
						Task.Config flags2 = Task.Config.Default;
						actor.tasks.CancelTasks<TaskRoutine>();
						rd2.CreateTask(actor.tasks, TaskManager.Priority.LONG_TERM, flags2);
						usingSpecialistRoutine = true;
					}
				}
			}
		}
		if (!usingSpecialistRoutine && m_Interface.Routines != null && m_Interface.Routines.Count > 0)
		{
			int routIndex = mRandNumbers[RoutineIdx];
			if (routIndex < m_Interface.Routines.Count && m_Interface.Routines[routIndex] != null)
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
		}
		yield return new WaitForSeconds(0.8f);
		if (mSpawned == null)
		{
			SpawnFailed(canCloseDoor);
			yield break;
		}
		actor.health.Invulnerable = false;
		if (DoorType != 0 && (mSpawnCount >= m_Interface.SpawnCount || canCloseDoor || mDeactivated))
		{
			if (mDoorPlayer != null && DoorCloseAnimation != null)
			{
				mDoorPlayer.Play("CloseDoor");
				yield return new WaitForSeconds(DoorCloseAnimation.length);
			}
			else
			{
				yield return StartCoroutine(Rotate(90f));
			}
			doorClosed = true;
		}
		if (mSpawned == null)
		{
			SpawnFailed(canCloseDoor);
			yield break;
		}
		if (mCoordinator != null)
		{
			mCoordinator.SendMessage("EntitySpawned", mSpawned);
			mCoordinator.SendMessage("GenerateEnemySuccessfull");
		}
		if (m_Interface.SpawnOnlyOnMessage)
		{
			mActivated = false;
		}
		mSpawningInProgress = false;
		yield return null;
	}

	private void SpawnFailed(bool canCloseDoor)
	{
		if (mCoordinator != null)
		{
			mCoordinator.SendMessage("GenerateEnemyFailed");
		}
		mSpawningInProgress = false;
		EmergencyDoorClose(canCloseDoor);
	}

	private void LogSpawnDebug(string message, Actor a, GameObject spawner)
	{
	}

	private void EmergencyDoorClose(bool canCloseDoor)
	{
		if (DoorType != 0 && (mSpawnCount >= m_Interface.SpawnCount || canCloseDoor || mDeactivated) && mDoorPlayer != null && DoorCloseAnimation != null)
		{
			mDoorPlayer.Play("CloseDoor");
		}
	}

	public override IEnumerator CloseDoor()
	{
		if (mDoorPlayer != null && DoorCloseAnimation != null && DoorType != 0)
		{
			if (!doorClosed)
			{
				mDoorPlayer.Play("CloseDoor");
				yield return new WaitForSeconds(DoorCloseAnimation.length);
			}
		}
		else if (!doorClosed)
		{
			yield return StartCoroutine(Rotate(90f));
		}
		doorClosed = true;
		yield return null;
	}
}
