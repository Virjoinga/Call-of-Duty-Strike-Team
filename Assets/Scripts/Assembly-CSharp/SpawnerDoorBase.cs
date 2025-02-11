using System.Collections;
using UnityEngine;

public class SpawnerDoorBase : SpawnerBase
{
	public SpawnerDoorDataBase m_Interface;

	public Transform SpawnFromPoint;

	[HideInInspector]
	public AITetherPoint StaticTether;

	[HideInInspector]
	public AnimationClip ClimbAnimation;

	protected int[] mRandNumbers = new int[99];

	protected int RoutineIdx;

	protected bool mActivated;

	protected bool mDeactivated;

	private ActorDescriptor OverrideActor;

	protected bool attachedToCoordinator;

	protected SpawnerCoordinator SpawnerCoordinator;

	private bool sentCompletionMesages;

	protected CoverPointCore closestCoverPoint;

	public virtual void Start()
	{
		mFinished = false;
		mSpawningInProgress = false;
		mSpawnCount = 0;
		if (mMonitored != null)
		{
			mMonitored.Clear();
		}
		if (m_Interface.TotalToSpawn == 0)
		{
			m_Interface.TotalToSpawn = m_Interface.SpawnCount;
		}
		if (m_Interface.InfiniteSpawn)
		{
			m_Interface.TotalToSpawn = 0;
		}
		Initialise(m_Interface.Delay + Random.Range(0f - m_Interface.DelayVarience, m_Interface.DelayVarience));
		OverrideActor = null;
		if (m_Interface.EventsList != null)
		{
			EventsList = m_Interface.EventsList;
		}
		if (!m_Interface.StartOnMessage && !m_Interface.SpawnOnlyOnMessage)
		{
			Activate();
		}
		closestCoverPoint = NewCoverPointManager.Instance().FindClosestCoverPoint_Fast(base.transform.position);
	}

	private void Update()
	{
		if ((!(SectionManager.GetSectionManager() != null) || SectionManager.GetSectionManager().SectionActivated) && GameController.Instance.GameplayStarted && (mActivated || !sentCompletionMesages))
		{
			if (AllActorsDead())
			{
				SendCompletionMessages();
				Deactivate();
			}
			else if (mActivated)
			{
				Process(m_Interface.SpawnCount, m_Interface.Delay + Random.Range(0f - m_Interface.DelayVarience, m_Interface.DelayVarience), m_Interface.Actors[Random.Range(0, m_Interface.Actors.Count)], StaticTether, m_Interface.EntityTypeTag, m_Interface.SpawnedAlertState, m_Interface.TotalToSpawn);
			}
		}
	}

	public void SendCompletionMessages()
	{
		if ((bool)m_Interface.NotifyOnAllDead)
		{
			Container.SendMessage(m_Interface.NotifyOnAllDead, m_Interface.NotifyFunction, base.gameObject);
		}
		if (m_Interface.NotifyGroupOnAllDead != null && m_Interface.NotifyGroupOnAllDead.Count > 0)
		{
			int num = 0;
			foreach (GameObject item in m_Interface.NotifyGroupOnAllDead)
			{
				string message = string.Empty;
				if (m_Interface.NotifyGroupFunction != null && num < m_Interface.NotifyGroupFunction.Count)
				{
					message = m_Interface.NotifyGroupFunction[num];
				}
				Container.SendMessage(item, message, base.gameObject);
				num++;
			}
		}
		sentCompletionMesages = true;
	}

	public void Activate()
	{
		mDeactivated = false;
		if (!attachedToCoordinator)
		{
			Init();
			mActivated = true;
			Process(m_Interface.SpawnCount, m_Interface.Delay + Random.Range(0f - m_Interface.DelayVarience, m_Interface.DelayVarience), m_Interface.Actors[Random.Range(0, m_Interface.Actors.Count)], StaticTether, m_Interface.EntityTypeTag, m_Interface.SpawnedAlertState, m_Interface.TotalToSpawn);
		}
		else
		{
			GenerateRandomNumberRange(m_Interface.Routines.Count);
			RoutineIdx = 0;
			mActivated = true;
		}
	}

	public void ToggleInfiniteSpawn()
	{
		m_Interface.InfiniteSpawn = !m_Interface.InfiniteSpawn;
		if (m_Interface.TotalToSpawn == 0 && m_Interface.InfiniteSpawn)
		{
			m_Interface.TotalToSpawn = 1;
		}
	}

	public void SwitchRoutine(GameObject routines)
	{
		m_Interface.Routines.Clear();
		m_Interface.Routines.Add(routines.GetComponent<RoutineDescriptor>());
	}

	public void OverrideNextEnemyType(ActorDescriptor ad)
	{
		OverrideActor = ad;
	}

	public void OverrideNextEnemyEvents(GameObject go)
	{
		EventsList = go;
	}

	public bool GenerateEnemy(GameObject coordinator)
	{
		if (GKM.AvailableSpawnSlots() < 1)
		{
			return false;
		}
		Init();
		mCoordinator = coordinator;
		mSpawningInProgress = true;
		if ((bool)OverrideActor)
		{
			StartCoroutine(ProcessSpawn(OverrideActor, StaticTether, m_Interface.EntityTypeTag, m_Interface.SpawnedAlertState, m_Interface.TotalToSpawn));
			OverrideActor = null;
		}
		else
		{
			StartCoroutine(ProcessSpawn(m_Interface.Actors[Random.Range(0, m_Interface.Actors.Count)], StaticTether, m_Interface.EntityTypeTag, m_Interface.SpawnedAlertState, m_Interface.TotalToSpawn));
		}
		return true;
	}

	public void Deactivate()
	{
		mActivated = false;
		mDeactivated = true;
		if (!mSpawningInProgress)
		{
			StartCoroutine(CloseDoor());
		}
		if (!m_Interface.clearEventsOnDeactivate)
		{
			return;
		}
		foreach (Actor item in mMonitored)
		{
			if (!(item != null))
			{
				continue;
			}
			EventsCreator componentInChildren = item.GetComponentInChildren<EventsCreator>();
			if (!(componentInChildren != null))
			{
				continue;
			}
			foreach (EventDescriptor @event in componentInChildren.GetEvents())
			{
				if (@event != null)
				{
					Object.Destroy(@event);
				}
			}
		}
	}

	protected void GenerateRandomNumberRange(int RangeSize)
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

	private void Init()
	{
		mCoordinator = null;
		GenerateRandomNumberRange(m_Interface.Routines.Count);
		RoutineIdx = 0;
	}

	public bool IsInUse()
	{
		return mSpawningInProgress;
	}

	public bool IsAvailableForUse(bool useRules)
	{
		if (mSpawningInProgress || mDeactivated)
		{
			return false;
		}
		if (useRules)
		{
			Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
			if ((bool)mFirstPersonActor)
			{
				CoverPointCore coverPointCore = NewCoverPointManager.Instance().FindClosestCoverPoint_Fast(mFirstPersonActor.transform.position);
				if (closestCoverPoint == null)
				{
					return false;
				}
				if (coverPointCore == null)
				{
					return true;
				}
				bool visibilityUncertain;
				bool coverUncertain;
				CoverTable.CoverProvided coverProvided = NewCoverPointManager.Instance().baked.coverTable.GetCoverProvided(closestCoverPoint.index, coverPointCore.index, out visibilityUncertain, out coverUncertain);
				if (!visibilityUncertain && (coverProvided == CoverTable.CoverProvided.Stupid || coverProvided == CoverTable.CoverProvided.Full))
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public virtual IEnumerator CloseDoor()
	{
		yield return 0;
	}
}
