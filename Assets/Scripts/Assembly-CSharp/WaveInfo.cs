using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveInfo
{
	public enum Criteria
	{
		Always = 0,
		Kills = 1,
		Delay = 2,
		Infinite = 3
	}

	public Criteria TriggerOn = Criteria.Kills;

	public Criteria StopOn = Criteria.Kills;

	public int NumberOfKillsPerWave = 10;

	public int DelayedStartTime;

	public int DelayedEndTime;

	public int NextWaveTrigger = -2;

	public bool BonusWave;

	public int WaitBetweenWavesInSeconds;

	public List<SpawnGroup> SpawnGroups = new List<SpawnGroup>();

	public List<GameObject> BossSpawners;

	public List<GameObject> CustomObjects;

	public GameObject EventListener;
}
