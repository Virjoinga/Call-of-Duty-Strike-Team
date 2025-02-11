public class MedalManager : Stats<MedalStat>
{
	private MissionListings.eMissionID m_CurrentMissionId;

	private int m_CurrentMissionSection;

	private DifficultyMode m_MissionDifficultyMode;

	private SectionData m_SectionData;

	private bool m_MissionPassed;

	private float m_GameplayTimeInSeconds;

	protected override void SetEventListeners()
	{
		EventHub.Instance.OnStartMission += MissionStarted;
		EventHub.Instance.OnEndMission += MissionEnded;
		EventHub.Instance.OnKill += OnKill;
		EventHub.Instance.OnSpecOpsWaveComplete += OnWaveComplete;
		EventHub.Instance.OnHardCurrencyChanged += OnHardCurrencyChanged;
		EventHub.Instance.GameplayMinutePassed += OnGameplayMinutePassed;
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

	public override void SessionEnd()
	{
		SectionData sectionData = MissionListings.Instance.Mission(m_CurrentMissionId).Sections[m_CurrentMissionSection];
		if (sectionData.IsSpecOps)
		{
			CheckMedal(sectionData, MedalType.TargetXPReached, true);
		}
		else if (m_MissionPassed)
		{
			CheckMedal(sectionData, MedalType.MissionComplete, false);
			CheckMedal(sectionData, MedalType.TargetAccuracyReached, false);
			CheckMedal(sectionData, MedalType.TargetTimeReached, false);
			CheckMedal(sectionData, MedalType.TargetXPReached, false);
			CheckMedal(sectionData, MedalType.TargetHeadshotsReached, false);
			CheckMedal(sectionData, MedalType.TargetKillsReached, false);
			CheckMedal(sectionData, MedalType.TargetKillsUsingGrenadesOrClaymores, false);
			CheckMedal(sectionData, MedalType.TargetTokensEarned, false);
			CheckMedal(sectionData, MedalType.TargetWaveReached, false);
		}
		base.SessionEnd();
	}

	private void CheckMedal(SectionData sectionData, MedalType type, bool showPopUp)
	{
		string id = StatsManager.MissionStatId(m_CurrentMissionId, m_CurrentMissionSection);
		MedalStat currentMissionStat = GetCurrentMissionStat(id);
		MedalStat gameTotalStat = GetGameTotalStat(id);
		for (int i = 0; i < SectionData.numMedalSlots; i++)
		{
			if (sectionData.Medals != null && i < sectionData.Medals.Length && sectionData.Medals[i].MedalTargetType == type)
			{
				if (gameTotalStat.GetMedalStatus(i, m_MissionDifficultyMode) || currentMissionStat.GetMedalStatus(i, m_MissionDifficultyMode))
				{
					break;
				}
				int target = sectionData.Medals[i].Normal;
				if (m_MissionDifficultyMode == DifficultyMode.Veteran)
				{
					target = sectionData.Medals[i].Veteran;
				}
				switch (sectionData.Medals[i].MedalTargetType)
				{
				case MedalType.MissionComplete:
					CheckMedalMissionComplete(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TargetAccuracyReached:
					CheckMedalTargetAccuracyReached(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TargetHeadshotsReached:
					CheckMedalTargetHeadshotsReached(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TargetKillsReached:
					CheckMedalTargetKillsReached(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TargetTimeReached:
					CheckMedalTargetTimeReached(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TargetTokensEarned:
					CheckMedalTargetTokensEarned(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TargetKillsUsingGrenadesOrClaymores:
					CheckMedalTargetKillsUsingGrenadesOrClaymores(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TargetWaveReached:
					CheckMedalTargetWaveReached(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TargetXPReached:
					CheckMedalTargetXPReached(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.TimeTrialTargetTimeReached:
					CheckMedalTimeTrialTargetTimeReached(target, i, currentMissionStat, showPopUp);
					break;
				case MedalType.DominationControlAllBeacons:
					CheckDominationControlAllBeacons(target, i, currentMissionStat, showPopUp);
					break;
				}
			}
		}
	}

	private void CheckMedalMissionComplete(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		EarnMedal(index, currentMissionMedals, showPopUp);
	}

	private void CheckMedalTargetAccuracyReached(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		int num = (int)StatsManager.Instance.SquadStats().GetCurrentMission().AccuracyInFP;
		if (num >= target)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckMedalTargetHeadshotsReached(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		int numHeadShotsInFP = StatsManager.Instance.SquadStats().GetCurrentMission().NumHeadShotsInFP;
		if (numHeadShotsInFP >= target)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckMedalTargetKillsReached(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		int numKills = StatsManager.Instance.SquadStats().GetCurrentMission().NumKills;
		if (numKills >= target)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckMedalTimeTrialTargetTimeReached(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		if (m_GameplayTimeInSeconds >= (float)target)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckMedalTargetTimeReached(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		float timePlayed = StatsManager.Instance.MissionStats().GetCurrentMissionStat(m_CurrentMissionId, m_CurrentMissionSection).TimePlayed;
		if (timePlayed <= (float)target / 60f)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckMedalTargetTokensEarned(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		int hardCurrencyEarned = StatsManager.Instance.PlayerStats().GetCurrentMissionStat().HardCurrencyEarned;
		if (hardCurrencyEarned >= target)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckMedalTargetKillsUsingGrenadesOrClaymores(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		int numGrenadeAndClaymoreKills = StatsManager.Instance.MissionStats().GetCurrentMissionStat(m_CurrentMissionId, m_CurrentMissionSection).NumGrenadeAndClaymoreKills;
		if (numGrenadeAndClaymoreKills >= target)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckMedalTargetWaveReached(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		int specOpsWavesCompleted = StatsManager.Instance.MissionStats().GetCurrentMissionStat(m_CurrentMissionId, m_CurrentMissionSection).SpecOpsWavesCompleted;
		if (specOpsWavesCompleted >= target)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckMedalTargetXPReached(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		int xP = StatsManager.Instance.PlayerStats().GetCurrentMissionStat("Global").XP;
		if (xP >= target)
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void CheckDominationControlAllBeacons(int target, int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		if (DominationCapturePointManager.Instance != null && DominationCapturePointManager.Instance.AllPointsHeldByPlayer())
		{
			EarnMedal(index, currentMissionMedals, showPopUp);
		}
	}

	private void MissionStarted(object sender, Events.StartMission args)
	{
		m_CurrentMissionId = args.MissionId;
		m_CurrentMissionSection = args.Section;
		m_MissionDifficultyMode = args.MissionDifficulty;
		m_SectionData = MissionListings.Instance.Mission(m_CurrentMissionId).Sections[m_CurrentMissionSection];
		m_MissionPassed = false;
	}

	private void MissionEnded(object sender, Events.EndMission args)
	{
		if (args.Success)
		{
			m_MissionPassed = true;
		}
		if (m_SectionData != null && m_SectionData.IsSpecOps)
		{
			CheckMedal(m_SectionData, MedalType.MissionComplete, true);
			CheckMedal(m_SectionData, MedalType.TargetAccuracyReached, true);
			CheckMedal(m_SectionData, MedalType.TargetTimeReached, true);
			CheckMedal(m_SectionData, MedalType.TargetXPReached, true);
			CheckMedal(m_SectionData, MedalType.DominationControlAllBeacons, true);
		}
		m_SectionData = null;
	}

	private void OnKill(object sender, Events.Kill args)
	{
		if (m_SectionData != null && m_SectionData.IsSpecOps)
		{
			StatsManager.Instance.SquadStats().BuildFromCharacters();
			CheckMedal(m_SectionData, MedalType.TargetHeadshotsReached, true);
			CheckMedal(m_SectionData, MedalType.TargetKillsReached, true);
			CheckMedal(m_SectionData, MedalType.TargetKillsUsingGrenadesOrClaymores, true);
		}
	}

	private void OnHardCurrencyChanged(object sender, Events.HardCurrencyChanged args)
	{
		if (m_SectionData != null && m_SectionData.IsSpecOps)
		{
			CheckMedal(m_SectionData, MedalType.TargetTokensEarned, true);
		}
	}

	private void OnGameplayMinutePassed(object sender, Events.GameplayMinutePassed args)
	{
		if (m_SectionData != null && m_SectionData.IsSpecOps)
		{
			m_GameplayTimeInSeconds = args.SecondsPlayed;
			CheckMedal(m_SectionData, MedalType.TimeTrialTargetTimeReached, true);
		}
	}

	private void OnWaveComplete(object sender, Events.SpecOpsWaveComplete args)
	{
		if (m_SectionData != null && m_SectionData.IsSpecOps)
		{
			CheckMedal(m_SectionData, MedalType.TargetWaveReached, true);
		}
	}

	public bool HasMedalBeenEarnedGameTotal(MissionListings.eMissionID missionId, int section, int medal, DifficultyMode mode)
	{
		MedalStat gameTotalStat = GetGameTotalStat(StatsManager.MissionStatId(missionId, section));
		return HasMedalBeenEarned(gameTotalStat, medal, mode);
	}

	public bool HasMedalBeenEarnedCurrentMission(MissionListings.eMissionID missionId, int section, int medal, DifficultyMode mode)
	{
		MedalStat currentMissionStat = GetCurrentMissionStat(StatsManager.MissionStatId(missionId, section));
		return HasMedalBeenEarned(currentMissionStat, medal, mode);
	}

	public bool HasMedalBeenEarned(MedalStat stat, int medal, DifficultyMode mode)
	{
		if (mode == DifficultyMode.Veteran)
		{
			return stat.MedalStatusVeteran[medal];
		}
		return stat.MedalStatusNormal[medal];
	}

	public int TotalMedalsEarned()
	{
		MedalStat gameTotalCombinedStat = GetGameTotalCombinedStat();
		return gameTotalCombinedStat.MedalTotal;
	}

	private void EarnMedal(int index, MedalStat currentMissionMedals, bool showPopUp)
	{
		if (m_MissionDifficultyMode == DifficultyMode.Regular)
		{
			if (currentMissionMedals.MedalStatusNormal[index])
			{
				return;
			}
			currentMissionMedals.MedalStatusNormal[index] = true;
		}
		else
		{
			if (currentMissionMedals.MedalStatusVeteran[index])
			{
				return;
			}
			currentMissionMedals.MedalStatusVeteran[index] = true;
		}
		SectionData sectionData = MissionListings.Instance.Mission(m_CurrentMissionId).Sections[m_CurrentMissionSection];
		currentMissionMedals.MedalEarned(sectionData.Medals[index].MedalTargetType, index, m_MissionDifficultyMode);
		if (showPopUp)
		{
			NotificationPanel.Instance.Display(Language.GetFormatString("S_AWARDED_MEDAL_NEWLINE", GetMedalString(m_SectionData, index, m_MissionDifficultyMode)));
		}
	}

	public static string GetMedalString(SectionData data, int index, DifficultyMode mode)
	{
		int num = ((mode != 0) ? data.Medals[index].Veteran : data.Medals[index].Normal);
		MedalType medalTargetType = data.Medals[index].MedalTargetType;
		string text = string.Format("S_MISSION_MEDAL_{0:00}", (int)(medalTargetType + 1));
		if ((medalTargetType == MedalType.TargetKillsUsingGrenadesOrClaymores || medalTargetType == MedalType.TargetKillsReached || medalTargetType == MedalType.TargetTokensEarned || medalTargetType == MedalType.TargetHeadshotsReached) && num == 1)
		{
			text += "_SINGULAR";
		}
		string format = AutoLocalize.Get(text);
		switch (medalTargetType)
		{
		case MedalType.TargetTimeReached:
		{
			int num3 = num / 60;
			int num4 = num % 60;
			return string.Format(format, num3, num4);
		}
		case MedalType.TimeTrialTargetTimeReached:
		{
			int num2 = num / 60;
			return string.Format(format, num2);
		}
		default:
			return string.Format(format, num);
		case MedalType.MissionComplete:
			return string.Format(format);
		}
	}

	public static string GetMedalProgressString(MissionListings.eMissionID missionId, int section, SectionData data, int index, DifficultyMode mode)
	{
		int num = ((mode != 0) ? data.Medals[index].Veteran : data.Medals[index].Normal);
		MedalType medalTargetType = data.Medals[index].MedalTargetType;
		StatsManager.Instance.SquadStats().BuildFromCharacters();
		int num2 = 0;
		switch (medalTargetType)
		{
		case MedalType.TargetKillsUsingGrenadesOrClaymores:
			num2 = StatsManager.Instance.MissionStats().GetCurrentMissionStat(missionId, section).NumGrenadeAndClaymoreKills;
			break;
		case MedalType.TargetTokensEarned:
			num2 = StatsManager.Instance.PlayerStats().GetCurrentMissionStat().HardCurrencyEarned;
			break;
		case MedalType.TargetHeadshotsReached:
			num2 = StatsManager.Instance.SquadStats().GetCurrentMission().NumHeadShotsInFP;
			break;
		case MedalType.TargetKillsReached:
			num2 = StatsManager.Instance.SquadStats().GetCurrentMission().NumKills;
			break;
		case MedalType.TargetWaveReached:
			num2 = StatsManager.Instance.MissionStats().GetCurrentMissionStat(missionId, section).SpecOpsWavesCompleted;
			break;
		}
		if (StatsManager.Instance.MedalManager().HasMedalBeenEarnedCurrentMission(missionId, section, index, mode) || StatsManager.Instance.MedalManager().HasMedalBeenEarnedGameTotal(missionId, section, index, mode))
		{
			return string.Empty;
		}
		if (medalTargetType == MedalType.TimeTrialTargetTimeReached)
		{
			return string.Empty;
		}
		return string.Format("{0}/{1}", num2, num);
	}
}
