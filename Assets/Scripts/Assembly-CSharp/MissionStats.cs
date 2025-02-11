public class MissionStats : Stats<MissionStat>
{
	private MissionListings.eMissionID m_CurrentMissionId;

	private int m_CurrentMissionSection;

	private bool m_MissionInProgress;

	private int m_SessionId;

	public MissionListings.eMissionID CurrentMissionId()
	{
		return m_CurrentMissionId;
	}

	public int CurrentMissionSection()
	{
		return m_CurrentMissionSection;
	}

	public bool IsMissionInProgress(out MissionListings.eMissionID id)
	{
		id = m_CurrentMissionId;
		return m_MissionInProgress;
	}

	public override void Reset()
	{
		m_MissionInProgress = false;
		base.Reset();
	}

	public void Resume(MissionListings.eMissionID id, int section)
	{
		m_MissionInProgress = true;
		m_CurrentMissionId = id;
		m_CurrentMissionSection = section;
	}

	public override void Create()
	{
		MissionData[] missions = MissionListings.Instance.Missions;
		foreach (MissionData missionData in missions)
		{
			if (!missionData.IncludeInStats)
			{
				continue;
			}
			for (int j = 0; j < missionData.Sections.Count; j++)
			{
				if (missionData.Sections[j].IsValidInCurrentBuild)
				{
					CreateStat(StatsManager.MissionStatId(missionData.MissionId, j));
				}
			}
		}
	}

	protected override void SetEventListeners()
	{
		EventHub.Instance.OnEndMission += MissionEnded;
		EventHub.Instance.OnStartMission += MissionStarted;
		EventHub.Instance.OnSpecOpsWaveComplete += WaveComplete;
		EventHub.Instance.OnKill += OnKill;
	}

	public MissionStat GetCurrentMissionStat(MissionListings.eMissionID id, int section)
	{
		return GetCurrentMissionStat(StatsManager.MissionStatId(id, section));
	}

	public MissionStat GetGameTotalStat(MissionListings.eMissionID id, int section)
	{
		return GetGameTotalStat(StatsManager.MissionStatId(id, section));
	}

	private void MissionStarted(object sender, Events.StartMission args)
	{
		TBFAssert.DoAssert(!m_MissionInProgress, "MissionStarted event received when mission is already in progress: " + m_CurrentMissionId);
		m_CurrentMissionId = args.MissionId;
		m_CurrentMissionSection = args.Section;
		m_MissionInProgress = true;
	}

	private void MissionEnded(object sender, Events.EndMission args)
	{
		TBFAssert.DoAssert(m_MissionInProgress, "MissionEnded event recieved when no mission has started");
		TBFAssert.DoAssert(args.MissionId == m_CurrentMissionId && args.Section == m_CurrentMissionSection, string.Concat("Ending mission ", args.MissionId, ",", args.Section, " Does not match start mission ", m_CurrentMissionId, ",", m_CurrentMissionSection));
		m_MissionInProgress = false;
		MissionStat currentMissionStat = GetCurrentMissionStat(m_CurrentMissionId, m_CurrentMissionSection);
		currentMissionStat.NumTimesPlayed++;
		if (args.Success)
		{
			currentMissionStat.NumTimesSucceeded++;
		}
		currentMissionStat.TimePlayed = args.TimePlayed;
	}

	public int TotalMissionsPlayed()
	{
		return GetGameTotalCombinedStat().NumTimesPlayed;
	}

	public int TotalMissionsSucceeded()
	{
		return GetGameTotalCombinedStat().NumTimesSucceeded;
	}

	public float TotalTimePlayed()
	{
		return GetGameTotalCombinedStat().TimePlayed;
	}

	private void WaveComplete(object sender, Events.SpecOpsWaveComplete args)
	{
		MissionStat currentMissionStat = GetCurrentMissionStat(m_CurrentMissionId, m_CurrentMissionSection);
		currentMissionStat.SpecOpsWavesCompleted++;
		currentMissionStat.HighestSpecOpsWaveCompleted++;
	}

	private void OnKill(object sender, Events.Kill args)
	{
		MissionStat currentMissionStat = GetCurrentMissionStat(m_CurrentMissionId, m_CurrentMissionSection);
		if (args.Attacker != null && args.Attacker.PlayerControlled && (args.ClaymoreKill || args.GrenadeKill))
		{
			currentMissionStat.NumGrenadeAndClaymoreKills++;
		}
	}
}
