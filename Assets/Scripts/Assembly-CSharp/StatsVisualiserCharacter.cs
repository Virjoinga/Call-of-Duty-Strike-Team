using UnityEngine;

public class StatsVisualiserCharacter : StatsVisualiserBase
{
	private int m_CurrentCharacter;

	public override void Start()
	{
		xPos = 740;
		yPos = 280;
		height = 180;
		m_CurrentCharacter = 0;
		base.Start();
	}

	private void OnGUI()
	{
		int num = 5;
		if (!StatsVisualiser.Instance().Hidden())
		{
			if (GUI.Button(prevRect, "<") && --m_CurrentCharacter < 0)
			{
				m_CurrentCharacter = num - 1;
			}
			if (GUI.Button(nextRect, ">") && ++m_CurrentCharacter >= num)
			{
				m_CurrentCharacter = 0;
			}
			string text = StatsManager.ConvertSoldierIndexToId(m_CurrentCharacter);
			CharacterStat gameTotalStat = StatsManager.Instance.CharacterStats().GetGameTotalStat(text);
			CharacterXP gameTotalStat2 = StatsManager.Instance.CharacterXPStats().GetGameTotalStat(text);
			CharacterStat currentMissionStat = StatsManager.Instance.CharacterStats().GetCurrentMissionStat(text);
			CharacterXP currentMissionStat2 = StatsManager.Instance.CharacterXPStats().GetCurrentMissionStat(text);
			string empty = string.Empty;
			empty = empty + "Deaths: " + string.Format("{0} ({1})", gameTotalStat.NumTimesKilled, currentMissionStat.NumTimesKilled);
			empty = empty + " Healed: " + string.Format("{0} ({1})", gameTotalStat.NumTimesHealed, currentMissionStat.NumTimesHealed);
			empty = empty + "\nKills: " + string.Format("{0} ({1})", gameTotalStat.NumKills, currentMissionStat.NumKills);
			empty = empty + "\nBonuses: " + string.Format("{0} ({1})", gameTotalStat2.NumBonuses, currentMissionStat2.NumBonuses);
			empty = empty + "\nXP From Kills: " + string.Format("{0} ({1})", gameTotalStat2.XPFromKills, currentMissionStat2.XPFromKills);
			empty = empty + "\nXP From Bonuses: " + string.Format("{0} ({1})", gameTotalStat2.XPFromBonuses, currentMissionStat2.XPFromBonuses);
			empty = empty + "\nFP Kills: " + string.Format("{0} ({1})", gameTotalStat.NumKillsInFP, currentMissionStat.NumKillsInFP);
			empty = empty + "\nFP Deaths: " + string.Format("{0} ({1})", gameTotalStat.NumTimesKilledInFP, currentMissionStat.NumTimesKilledInFP);
			empty = empty + "\nFP KDR: " + string.Format("{0} ({1})", gameTotalStat.KDRinFP, currentMissionStat.KDRinFP);
			empty = empty + "\nFP Shots fired: " + string.Format("{0} ({1})", gameTotalStat.NumShotsFiredInFP, currentMissionStat.NumShotsFiredInFP);
			empty = empty + "\nFP Shots hit: " + string.Format("{0} ({1})", gameTotalStat.NumShotsHitInFP, currentMissionStat.NumShotsHitInFP);
			empty = empty + "\nFP Accuracy: " + string.Format("{0}% ({1}%)", gameTotalStat.AccuracyInFP, currentMissionStat.AccuracyInFP);
			empty = empty + "\nFP Head Shots: " + string.Format("{0} ({1})", gameTotalStat.NumHeadShotsInFP, currentMissionStat.NumHeadShotsInFP);
			GUI.TextField(titleRect, "Character: " + text);
			GUI.TextField(statsRect, empty);
		}
	}
}
