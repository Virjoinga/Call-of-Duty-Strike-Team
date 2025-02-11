using System.Collections.Generic;
using UnityEngine;

public class CharacterXP : SingleStat<CharacterXP>
{
	private class KillData
	{
		public float TimeStamp;
	}

	private List<KillData> m_RecentEnemyKills = new List<KillData>();

	private List<KillData> m_RecentEnemyStealthKills = new List<KillData>();

	public int XPFromKills;

	public int XPFromBonuses;

	public int NumBonuses;

	public bool HasBoughtBack;

	public int TotalXP
	{
		get
		{
			return XPFromKills + XPFromBonuses;
		}
	}

	public override void Reset()
	{
		XPFromKills = 0;
		XPFromBonuses = 0;
		NumBonuses = 0;
	}

	private bool ShouldAddXPForThisCharacter(string id)
	{
		if (ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			return true;
		}
		CharacterStat currentMissionStat = StatsManager.Instance.CharacterStats().GetCurrentMissionStat(id);
		return currentMissionStat.NumTimesKilled == currentMissionStat.NumTimesHealed;
	}

	public override void CombineStat(CharacterXP source)
	{
		if (ShouldAddXPForThisCharacter(source.Id) || source.HasBoughtBack)
		{
			XPFromKills += source.XPFromKills;
			XPFromBonuses += source.XPFromBonuses;
			NumBonuses += source.NumBonuses;
		}
	}

	public override void Save(string prefix)
	{
		Save(prefix, ref XPFromKills, "XPFromKills");
		Save(prefix, ref XPFromBonuses, "XPFromBonuses");
		Save(prefix, ref NumBonuses, "NumBonuses");
	}

	public override void Load(string prefix)
	{
		Load(prefix, ref XPFromKills, "XPFromKills");
		Load(prefix, ref XPFromBonuses, "XPFromBonuses");
		Load(prefix, ref NumBonuses, "NumBonuses");
	}

	public void ClearKillData()
	{
		m_RecentEnemyStealthKills.Clear();
		m_RecentEnemyKills.Clear();
	}

	public void StoreKill(bool thirdPerson)
	{
		KillData killData = new KillData();
		killData.TimeStamp = Time.time;
		if (thirdPerson)
		{
			m_RecentEnemyStealthKills.Add(killData);
		}
		else
		{
			m_RecentEnemyKills.Add(killData);
		}
	}

	public int CheckForMultiKills(bool thirdPerson)
	{
		if (thirdPerson)
		{
			return CheckForMultiKills(m_RecentEnemyStealthKills);
		}
		return CheckForMultiKills(m_RecentEnemyKills);
	}

	private int CheckForMultiKills(List<KillData> kills)
	{
		if (Time.timeScale == 0f)
		{
			return 0;
		}
		float num = 2f / Time.timeScale;
		int num2 = 0;
		if (kills.Count > 0)
		{
			int num3 = kills.Count - 1;
			if (Time.time - kills[num3].TimeStamp > num)
			{
				num2 = 1;
				while (num3 > 0 && !(kills[num3].TimeStamp - kills[num3 - 1].TimeStamp > num))
				{
					num2++;
					num3--;
				}
				kills.Clear();
			}
		}
		return num2;
	}
}
