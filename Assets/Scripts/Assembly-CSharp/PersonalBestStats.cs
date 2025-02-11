public class PersonalBestStats : Stats<PersonalBestStat>
{
	private MissionListings.eMissionID m_CurrentMissionId;

	private int m_CurrentMissionSection;

	protected override void SetEventListeners()
	{
		EventHub.Instance.OnStartMission += MissionStarted;
	}

	public override void Create()
	{
		MissionData[] missions = MissionListings.Instance.Missions;
		foreach (MissionData missionData in missions)
		{
			for (int j = 0; j < missionData.Sections.Count; j++)
			{
				CreateStat(StatsManager.MissionStatId(missionData.MissionId, j));
			}
		}
	}

	private void MissionStarted(object sender, Events.StartMission args)
	{
		m_CurrentMissionId = args.MissionId;
		m_CurrentMissionSection = args.Section;
	}

	public override void SessionEnd()
	{
	}

	public override void PostSessionEnd()
	{
		PersonalBestStat currentMissionStat = GetCurrentMissionStat(StatsManager.MissionStatId(m_CurrentMissionId, m_CurrentMissionSection));
		currentMissionStat.BestKDR = StatsManager.Instance.SquadStats().GetCurrentMission().KDR;
		currentMissionStat.MostHeadShots = StatsManager.Instance.SquadStats().GetCurrentMission().NumHeadShots;
		currentMissionStat.MostKills = StatsManager.Instance.SquadStats().GetCurrentMission().NumKills;
		currentMissionStat.HighestScore = StatsManager.Instance.PlayerStats().GetCurrentMissionStat().Score;
		currentMissionStat.HighestScoreAsVeteran = StatsManager.Instance.PlayerStats().GetCurrentMissionStat().ScoreAsVeteran;
		if (StatsManager.Instance.MissionStats().GetCurrentMissionStat(StatsManager.MissionStatId(m_CurrentMissionId, m_CurrentMissionSection)).NumTimesSucceeded > 0)
		{
			currentMissionStat.BestTimeToComplete = StatsManager.Instance.MissionStats().GetCurrentMissionStat(m_CurrentMissionId, m_CurrentMissionSection).TimePlayed;
		}
		base.SessionEnd();
	}
}
