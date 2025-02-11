using UnityEngine;

public class StatsVisualiserMission : StatsVisualiserBase
{
	private int m_CurrentMission;

	private int m_CurrentSection;

	public override void Start()
	{
		xPos = 740;
		yPos = 20;
		height = 200;
		m_CurrentMission = 0;
		m_CurrentSection = -1;
		FindNextValidMission();
		base.Start();
	}

	private void OnGUI()
	{
		if (!(StatsVisualiser.Instance() != null) || !StatsVisualiser.Instance().Hidden())
		{
			if (GUI.Button(prevRect, "<"))
			{
				FindPreviousValidMission();
			}
			if (GUI.Button(nextRect, ">"))
			{
				FindNextValidMission();
			}
			string empty = string.Empty;
			MissionListings.eMissionID currentMission = (MissionListings.eMissionID)m_CurrentMission;
			MissionStat gameTotalStat = StatsManager.Instance.MissionStats().GetGameTotalStat(currentMission, m_CurrentSection);
			empty = empty + "Times played: " + gameTotalStat.NumTimesPlayed;
			empty = empty + "\nTimes successful: " + gameTotalStat.NumTimesSucceeded;
			int num = Mathf.FloorToInt(gameTotalStat.TimePlayed);
			int num2 = (int)(60f * (gameTotalStat.TimePlayed - (float)num));
			empty += string.Format("\nPlaying time: {0}:{1}", num, num2);
			empty = empty + "\nAccumulated score: " + gameTotalStat.ScoreAwarded;
			empty = empty + "\nBest FP KDR: " + StatsHelper.BestKDR(currentMission, m_CurrentSection);
			empty = empty + "\nMost FP head shots: " + StatsHelper.MostHeadShots(currentMission, m_CurrentSection);
			empty = empty + "\nMost FP kills: " + StatsHelper.MostKills(currentMission, m_CurrentSection);
			empty = empty + "\nHighest score: " + StatsHelper.HighestScore(currentMission, m_CurrentSection);
			empty = empty + "\nHighest score Vet: " + StatsHelper.HighestScoreAsVeteran(currentMission, m_CurrentSection);
			string text = empty;
			empty = text + "\nHighest wave: " + StatsHelper.HighestSpecOpsWaveCompletedGameTotal(currentMission, m_CurrentSection) + " (" + StatsHelper.HighestSpecOpsWaveCompletedThisMission(currentMission, m_CurrentSection) + ")";
			float num3 = StatsHelper.BestTimeToComplete(currentMission, m_CurrentSection);
			int num4 = Mathf.FloorToInt(num3);
			int num5 = (int)(60f * (num3 - (float)num4));
			empty += string.Format("\nBest time: {0}:{1}", num4, num5);
			empty += "\n";
			for (int i = 0; i < SectionData.numMedalSlots; i++)
			{
				empty = ((!StatsHelper.HasMedalBeenEarnedGameTotal(currentMission, m_CurrentSection, i, DifficultyMode.Regular)) ? (empty + "-") : (empty + "R"));
				empty = ((!StatsHelper.HasMedalBeenEarnedGameTotal(currentMission, m_CurrentSection, i, DifficultyMode.Veteran)) ? (empty + "- ") : (empty + "V "));
			}
			empty += "\nIntel ";
			for (int j = 0; j < 3; j++)
			{
				empty = ((!PickupManager.Instance.BeenPickedUp(currentMission, m_CurrentSection, j)) ? (empty + "-") : (empty + "Y"));
			}
			GUI.TextField(statsRect, empty);
			GUI.TextField(titleRect, StatsManager.MissionStatId(currentMission, m_CurrentSection));
		}
	}

	private void FindNextValidMission()
	{
		while (true)
		{
			MissionData missionData = MissionListings.Instance.Mission((MissionListings.eMissionID)m_CurrentMission);
			if (missionData != null && missionData.IncludeInStats)
			{
				m_CurrentSection++;
				if (m_CurrentSection < missionData.Sections.Count && missionData.Sections[m_CurrentSection].IsValidInCurrentBuild)
				{
					break;
				}
			}
			if (++m_CurrentMission >= (int)MissionListings.MI_MISSION_MAX)
			{
				m_CurrentMission = 0;
			}
			m_CurrentSection = -1;
		}
	}

	private void FindPreviousValidMission()
	{
		while (true)
		{
			MissionData missionData = MissionListings.Instance.Mission((MissionListings.eMissionID)m_CurrentMission);
			if (missionData != null && missionData.IncludeInStats)
			{
				m_CurrentSection--;
				if (m_CurrentSection >= 0 && missionData.Sections[m_CurrentSection].IsValidInCurrentBuild)
				{
					break;
				}
			}
			if (--m_CurrentMission < 0)
			{
				m_CurrentMission = (int)(MissionListings.MI_MISSION_MAX - 1);
			}
			missionData = MissionListings.Instance.Mission((MissionListings.eMissionID)m_CurrentMission);
			if (missionData != null)
			{
				m_CurrentSection = missionData.Sections.Count;
			}
		}
	}
}
