using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementData
{
	public Achievement[] Achievements;

	private Dictionary<string, Achievement> m_LookUp;

	public void Awake()
	{
		if (m_LookUp == null)
		{
			GenerateLookUpTable();
		}
	}

	private void GenerateLookUpTable()
	{
		m_LookUp = new Dictionary<string, Achievement>();
		Achievement[] achievements = Achievements;
		foreach (Achievement achievement in achievements)
		{
			m_LookUp.Add(achievement.Identifier, achievement);
		}
	}

	public Achievement GetAchievement(string id)
	{
		if (m_LookUp == null)
		{
			GenerateLookUpTable();
		}
		Achievement value = null;
		if (!m_LookUp.TryGetValue(id, out value))
		{
			Debug.LogError("Unknown achievement " + id);
		}
		return value;
	}
}
