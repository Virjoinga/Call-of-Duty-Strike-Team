using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBase : MonoBehaviour
{
	public delegate void SpawnerBaseSpawnEventHandler(object spawner);

	public ActorWrapper PreferredTarget;

	public GameObject mCoordinator;

	[HideInInspector]
	public List<Actor> mMonitored;

	[HideInInspector]
	public GameObject EventsList;

	[HideInInspector]
	public bool mFinished;

	[HideInInspector]
	public int mSpawnCount;

	protected GameObject mSpawned;

	protected bool mSpawningInProgress;

	private float mTimeTillNextSpawn;

	public event SpawnerBaseSpawnEventHandler SpawnEvent;

	private void Start()
	{
	}

	public void AddEventHandler(SpawnerBaseSpawnEventHandler eventHandler)
	{
		this.SpawnEvent = (SpawnerBaseSpawnEventHandler)Delegate.Combine(this.SpawnEvent, eventHandler);
	}

	public void RemoveEventHandler(SpawnerBaseSpawnEventHandler eventHandler)
	{
		this.SpawnEvent = (SpawnerBaseSpawnEventHandler)Delegate.Remove(this.SpawnEvent, eventHandler);
	}

	public bool AllActorsDead()
	{
		if (mFinished && (mMonitored.Count == 0 || (mMonitored.Count == 1 && (mMonitored[0].realCharacter == null || (mMonitored[0].realCharacter != null && mMonitored[0].realCharacter.IsDead())))))
		{
			return true;
		}
		return false;
	}

	protected void Initialise(float pauseDuration)
	{
		mMonitored = new List<Actor>();
		mTimeTillNextSpawn = pauseDuration;
	}

	protected void RegisterSpawn(Actor actor, int globalLimit)
	{
		mMonitored.Add(actor);
		SendMessage("OnSpawned", mSpawned, SendMessageOptions.DontRequireReceiver);
		if (this.SpawnEvent != null)
		{
			this.SpawnEvent(mSpawned);
		}
		mSpawnCount++;
		if (globalLimit > 0 && mSpawnCount >= globalLimit)
		{
			mFinished = true;
		}
	}

	protected void Process(int count, float pauseDuration, ActorDescriptor spawn, AITetherPoint aiTetherPoint, string tag, BehaviourController.AlertState releaseAlerted, int globalLimit)
	{
		if (!GameController.Instance.GameplayStarted)
		{
			return;
		}
		if (!AllActorsDead())
		{
			for (int num = mMonitored.Count - 1; num >= 0; num--)
			{
				Actor actor = mMonitored[num];
				if (actor == null || actor.realCharacter.IsDead())
				{
					mMonitored.RemoveAt(num);
				}
			}
		}
		if (mFinished || mSpawningInProgress || mMonitored.Count >= count)
		{
			return;
		}
		mTimeTillNextSpawn -= Time.deltaTime;
		if (mTimeTillNextSpawn <= 0f)
		{
			if (GKM.AvailableSpawnSlots() >= 1)
			{
				mSpawningInProgress = true;
				StartCoroutine(ProcessSpawn(spawn, aiTetherPoint, tag, releaseAlerted, globalLimit));
			}
			mTimeTillNextSpawn = pauseDuration;
		}
	}

	protected void DebugDraw(ActorDescriptor spawn)
	{
		Vector3 vector = base.transform.position + new Vector3(0f, 0.5f, 0f);
		if (spawn != null)
		{
			if (spawn.PlayerControlled)
			{
				Gizmos.DrawIcon(vector, "PlayerSpawn");
			}
			else
			{
				Gizmos.DrawIcon(vector, "EnemySpawn");
			}
		}
		else
		{
			Gizmos.DrawIcon(vector, "PlayerSpawn");
		}
		Gizmos.color = Color.green;
		Vector3 vector2 = vector + base.transform.forward * 1f;
		Gizmos.DrawLine(vector, vector2);
		Quaternion quaternion = Quaternion.AngleAxis(45f, Vector3.up);
		Vector3 to = vector2 - quaternion * base.transform.forward * 0.3f;
		Gizmos.DrawLine(vector2, to);
		Quaternion quaternion2 = Quaternion.AngleAxis(-45f, Vector3.up);
		Vector3 to2 = vector2 - quaternion2 * base.transform.forward * 0.3f;
		Gizmos.DrawLine(vector2, to2);
	}

	protected virtual IEnumerator ProcessSpawn(ActorDescriptor spawn, AITetherPoint aiTetherPoint, string tag, BehaviourController.AlertState releaseAlerted, int globalLimit)
	{
		Transform spawnPoint = base.transform;
		mSpawned = SceneNanny.CreateActor(spawn, spawnPoint.position, spawnPoint.rotation);
		Actor actor = mSpawned.GetComponent<Actor>();
		actor.realCharacter.EnableNavMesh(true);
		SpawnerUtils.InitialiseSpawnedActor(actor, aiTetherPoint, releaseAlerted, PreferredTarget);
		RegisterSpawn(actor, globalLimit);
		mSpawned.AddComponent<Entity>().Type = tag;
		if (mCoordinator != null)
		{
			mCoordinator.SendMessage("EntitySpawned", mSpawned);
		}
		mSpawningInProgress = false;
		yield return null;
	}

	public void AddEventsList(Actor actor)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<Container>();
		EventsCreator eventsCreator = gameObject.AddComponent<EventsCreator>();
		if (EventsList != null)
		{
			EventsCreator component = EventsList.GetComponent<EventsCreator>();
			if (component != null)
			{
				eventsCreator.m_EventDescriptors = new List<EventDescriptor>();
				foreach (EventDescriptor eventDescriptor in component.m_EventDescriptors)
				{
					if (eventDescriptor != null)
					{
						eventsCreator.AddItemCopy(eventDescriptor);
					}
				}
			}
		}
		foreach (EventDescriptor @event in eventsCreator.GetEvents())
		{
			@event.Initialise(actor.gameObject);
			EventOnMove eventOnMove = @event as EventOnMove;
			if (eventOnMove != null)
			{
				actor.realCharacter.EventMoved = eventOnMove;
			}
		}
		gameObject.name = actor.name + " Events List";
		gameObject.transform.parent = actor.transform;
	}
}
