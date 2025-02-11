using System.Collections.Generic;
using UnityEngine;

public class SpawnerCoordinator : MonoBehaviour
{
	public SpawnerCoordinatorData m_Interface;

	public GameObject AvatarPrefab;

	private bool bActive;

	private float mTimeTillNextSpawn;

	private int iCurSimVarience;

	private int iActiveCount;

	private int iRequestedCount;

	private int iCurRandIdx;

	private int TotalEnemies;

	private List<GameObject> mMonitorList;

	private int[] iRandNumbers;

	public void Start()
	{
		bActive = false;
		Init();
	}

	private void Init()
	{
		iActiveCount = 0;
		iRequestedCount = 0;
		mTimeTillNextSpawn = 0f;
		iRandNumbers = new int[99];
		mMonitorList = new List<GameObject>();
		TotalEnemies = m_Interface.TotalEnemies;
		GenerateRandomNumberRange(m_Interface.MultiSpawners.Count);
		if (TotalEnemies == 0 && m_Interface.InfiniteSpawn)
		{
			TotalEnemies = 1;
		}
	}

	private void Update()
	{
		if (!bActive)
		{
			return;
		}
		foreach (GameObject mMonitor in mMonitorList)
		{
			RealCharacter realCharacter = null;
			bool flag = false;
			if (mMonitor == null)
			{
				flag = true;
			}
			else
			{
				realCharacter = mMonitor.GetComponent<RealCharacter>();
			}
			if (realCharacter == null || realCharacter.IsDead())
			{
				flag = true;
			}
			if (flag)
			{
				mMonitorList.Remove(mMonitor);
				iActiveCount--;
				break;
			}
		}
		if (iRequestedCount + iActiveCount < m_Interface.MaxSimultaneousEnemies + iCurSimVarience && TotalEnemies - iRequestedCount > 0)
		{
			mTimeTillNextSpawn -= Time.deltaTime;
			if (!(mTimeTillNextSpawn <= 0f))
			{
				return;
			}
			SpawnerDoor componentInChildren = m_Interface.MultiSpawners[iCurRandIdx].GetComponentInChildren<SpawnerDoor>();
			if (componentInChildren != null)
			{
				if (!componentInChildren.IsAvailableForUse(m_Interface.UsePlayerRelativeSpawnRules))
				{
					iCurRandIdx++;
					if (iCurRandIdx == m_Interface.MultiSpawners.Count)
					{
						GenerateRandomNumberRange(m_Interface.MultiSpawners.Count);
					}
				}
				else
				{
					GenerateEnemy();
				}
			}
			else
			{
				GenerateEnemy();
			}
		}
		else
		{
			if (TotalEnemies != 0 || iActiveCount != 0 || iRequestedCount != 0)
			{
				return;
			}
			bActive = false;
			base.enabled = false;
			if (m_Interface.NotifyOnAllDead != null)
			{
				Container.SendMessage(m_Interface.NotifyOnAllDead, m_Interface.NotifyFunction, base.gameObject);
			}
			if (m_Interface.GroupObjectToCall == null || m_Interface.GroupObjectToCall.Count <= 0)
			{
				return;
			}
			int num = 0;
			foreach (GameObject item in m_Interface.GroupObjectToCall)
			{
				string message = string.Empty;
				if (m_Interface.GroupFunctionToCall != null && num < m_Interface.GroupFunctionToCall.Count)
				{
					message = m_Interface.GroupFunctionToCall[num];
				}
				Container.SendMessage(item, message, base.gameObject);
				num++;
			}
		}
	}

	public void Activate()
	{
		bActive = true;
		base.enabled = true;
		Init();
		if (!m_Interface.ApplyQuickDestinations)
		{
			return;
		}
		foreach (GameObject multiSpawner in m_Interface.MultiSpawners)
		{
			EnemyFromDoorOverride componentInChildren = multiSpawner.GetComponentInChildren<EnemyFromDoorOverride>();
			if (componentInChildren != null)
			{
				componentInChildren.m_OverrideData.quickDestinations = m_Interface.QuickDestinations;
				Container component = multiSpawner.GetComponent<Container>();
				if (component != null)
				{
					componentInChildren.ApplyOverride(component);
				}
				continue;
			}
			EnemyFromRoofOverride componentInChildren2 = multiSpawner.GetComponentInChildren<EnemyFromRoofOverride>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.m_OverrideData.quickDestinations = m_Interface.QuickDestinations;
				Container component2 = multiSpawner.GetComponent<Container>();
				if (component2 != null)
				{
					componentInChildren2.ApplyOverride(component2);
				}
			}
		}
	}

	public void Deactivate()
	{
		bActive = false;
	}

	public void ToggleInfiniteSpawn()
	{
		m_Interface.InfiniteSpawn = !m_Interface.InfiniteSpawn;
		if (m_Interface.TotalEnemies == 0 && m_Interface.InfiniteSpawn)
		{
			m_Interface.TotalEnemies = 1;
		}
	}

	public void CleanupAssociatedActors()
	{
		foreach (GameObject mMonitor in mMonitorList)
		{
			Actor component = mMonitor.GetComponent<Actor>();
			TaskManager tasks = component.tasks;
			Object.Destroy(component.model.gameObject);
			if (component.realCharacter.Ragdoll != null)
			{
				Object.Destroy(component.realCharacter.Ragdoll.gameObject);
			}
			Object.Destroy(tasks.gameObject);
			mMonitorList.Remove(mMonitor);
		}
		iActiveCount = 0;
		iRequestedCount = 0;
	}

	public void EntitySpawned(GameObject spawn)
	{
		mMonitorList.Add(spawn);
	}

	private void GenerateEnemy()
	{
		iRequestedCount++;
		if (m_Interface.OverrideActors != null && m_Interface.OverrideActors.Count > 0)
		{
			m_Interface.MultiSpawners[iCurRandIdx].BroadcastMessage("OverrideNextEnemyType", m_Interface.OverrideActors[Random.Range(0, m_Interface.OverrideActors.Count)]);
		}
		if (m_Interface.EventsListOverride != null)
		{
			m_Interface.MultiSpawners[iCurRandIdx].BroadcastMessage("OverrideNextEnemyEvents", m_Interface.EventsListOverride);
		}
		m_Interface.MultiSpawners[iCurRandIdx].BroadcastMessage("GenerateEnemy", base.gameObject);
	}

	public void GenerateEnemySuccessfull()
	{
		iCurRandIdx++;
		if (iCurRandIdx == m_Interface.MultiSpawners.Count)
		{
			GenerateRandomNumberRange(m_Interface.MultiSpawners.Count);
		}
		if (!m_Interface.InfiniteSpawn)
		{
			TotalEnemies--;
		}
		iActiveCount++;
		iRequestedCount--;
		mTimeTillNextSpawn = m_Interface.SpawnDelay + Random.Range(0f - m_Interface.SpawnDelayVarience, m_Interface.SpawnDelayVarience);
	}

	public void GenerateEnemyFailed()
	{
		iRequestedCount--;
	}

	private void GenerateRandomNumberRange(int RangeSize)
	{
		int i;
		for (i = 0; i < RangeSize; i++)
		{
			iRandNumbers[i] = i;
		}
		i = RangeSize;
		while (i > 1)
		{
			int num = Random.Range(0, RangeSize);
			i--;
			int num2 = iRandNumbers[num];
			iRandNumbers[num] = iRandNumbers[i];
			iRandNumbers[i] = num2;
		}
		iCurRandIdx = 0;
		iCurSimVarience = Random.Range(0, m_Interface.RandomVarience);
	}

	public bool CanCloseDoor()
	{
		if (!m_Interface.LeaveDoorOpenBetweenSpawns)
		{
			return true;
		}
		if (iActiveCount >= m_Interface.MaxSimultaneousEnemies - m_Interface.MultiSpawners.Count || TotalEnemies <= m_Interface.MultiSpawners.Count)
		{
			return true;
		}
		return false;
	}
}
