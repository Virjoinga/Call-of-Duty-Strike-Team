using System.Collections.Generic;
using UnityEngine;

public abstract class Stats<T> : StatsManagerUpdatable where T : SingleStat<T>, new()
{
	protected const string currentMissionStatPrefix = "currentMission.";

	protected const string gameTotalStatPrefix = "gameTotal.";

	protected Dictionary<string, T> m_GameTotalStats;

	protected Dictionary<string, T> m_CurrentMissionStats;

	private int m_UpdateIndex;

	private T GetStat(Dictionary<string, T> dict, string id)
	{
		T value = (T)null;
		if (!dict.TryGetValue(id, out value))
		{
			value = new T
			{
				Id = id
			};
			dict.Add(id, value);
		}
		return value;
	}

	public abstract void Create();

	public override void Init()
	{
		m_GameTotalStats = new Dictionary<string, T>();
		m_CurrentMissionStats = new Dictionary<string, T>();
		base.Init();
	}

	public T GetGameTotalStat(string id)
	{
		return GetStat(m_GameTotalStats, id);
	}

	public void CreateStat(string id)
	{
		GetStat(m_CurrentMissionStats, id);
		GetStat(m_GameTotalStats, id);
	}

	public T GetCurrentMissionStat(string id)
	{
		return GetStat(m_CurrentMissionStats, id);
	}

	private T GetCombinedStat(Dictionary<string, T> dict)
	{
		T result = new T
		{
			Id = "CombinedStat"
		};
		foreach (KeyValuePair<string, T> item in dict)
		{
			result.CombineStat(item.Value);
		}
		return result;
	}

	public T GetGameTotalCombinedStat()
	{
		return GetCombinedStat(m_GameTotalStats);
	}

	public T GetCurrentMissionCombinedStat()
	{
		return GetCombinedStat(m_CurrentMissionStats);
	}

	public override void SessionEnd()
	{
		UpdateGameTotals();
	}

	protected void UpdateGameTotals()
	{
		foreach (KeyValuePair<string, T> currentMissionStat in m_CurrentMissionStats)
		{
			T gameTotalStat = GetGameTotalStat(currentMissionStat.Key);
			T value = currentMissionStat.Value;
			if (gameTotalStat != null)
			{
				gameTotalStat.CombineStat(value);
			}
		}
	}

	public override void ClearCurrentMission()
	{
		foreach (KeyValuePair<string, T> currentMissionStat in m_CurrentMissionStats)
		{
			T value = currentMissionStat.Value;
			value.Reset();
		}
	}

	public override void Reset()
	{
		m_GameTotalStats.Clear();
		m_CurrentMissionStats.Clear();
		Create();
	}

	public void ListAllEntries()
	{
		int num = 0;
		foreach (KeyValuePair<string, T> gameTotalStat in m_GameTotalStats)
		{
			Debug.Log("Entry " + num + ": " + gameTotalStat.Key);
			num++;
		}
	}

	public override void Load()
	{
		LoadGameTotalStats();
	}

	private void LoadCurrentMissionStats()
	{
		foreach (KeyValuePair<string, T> currentMissionStat in m_CurrentMissionStats)
		{
			T value = currentMissionStat.Value;
			value.Load("currentMission.");
		}
	}

	private void LoadGameTotalStats()
	{
		foreach (KeyValuePair<string, T> gameTotalStat in m_GameTotalStats)
		{
			T value = gameTotalStat.Value;
			value.Load("gameTotal.");
		}
	}

	public override void Save()
	{
		SaveGameTotalStats();
	}

	private void SaveCurrentMissionStats()
	{
		foreach (KeyValuePair<string, T> currentMissionStat in m_CurrentMissionStats)
		{
			T value = currentMissionStat.Value;
			value.Save("currentMission.");
		}
	}

	private void SaveGameTotalStats()
	{
		foreach (KeyValuePair<string, T> gameTotalStat in m_GameTotalStats)
		{
			T value = gameTotalStat.Value;
			value.Save("gameTotal.");
		}
	}
}
