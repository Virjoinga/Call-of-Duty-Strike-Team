using System;
using UnityEngine;

public static class StatsHelper
{
	public static int NumKillsBySquad()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumKills;
	}

	public static int NumDeathsForSquad()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumTimesKilled;
	}

	public static float KDRForSquad()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().KDR;
	}

	public static string SecondsToString(float seconds)
	{
		string empty = string.Empty;
		int num = Mathf.FloorToInt(seconds) / 60;
		int num2 = (int)(seconds - (float)(60 * num));
		if (num > 60)
		{
			int num3 = Mathf.FloorToInt(num) / 60;
			return string.Format("{0}:{1}:{2}", num3, (num - 60 * num3).ToString("00"), num2.ToString("00"));
		}
		return string.Format("{0}:{1}", num, num2.ToString("00"));
	}

	public static string MinutesToString(float timeInMinutes)
	{
		return SecondsToString(timeInMinutes * 60f);
	}

	public static string KDRToString(float kdr)
	{
		if (kdr < 0f)
		{
			return "--";
		}
		if (kdr == 0f || kdr >= 1f)
		{
			return kdr.ToString("N");
		}
		return string.Format("{0:.00}", kdr);
	}

	public static string AccuracyToString(float acc)
	{
		if (acc < 0f)
		{
			return "--";
		}
		return string.Format("{0:.00}%", acc);
	}

	public static int NumKillsByPlayerFP()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumKillsInFP;
	}

	public static int NumDeathsForPlayerFP()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumTimesKilledInFP;
	}

	public static int NumKillsByPlayer()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumKills;
	}

	public static int NumDeathsForPlayer()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumTimesKilled;
	}

	public static float KDRForPlayer()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().KDRinFP;
	}

	public static int ShotsFiredByPlayer()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumShotsFiredInFP;
	}

	public static int ShotsHitByPlayer()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumShotsHitInFP;
	}

	public static float AccuracyForPlayer()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().AccuracyInFP;
	}

	public static int HeadShotsForPlayer()
	{
		return StatsManager.Instance.SquadStats().GetGameTotal().NumHeadShotsInFP;
	}

	public static string MostFiredWeaponByPlayer()
	{
		return StatsManager.Instance.WeaponStats().MostFiredWeapon();
	}

	public static int PlayerXP()
	{
		PlayerStat gameTotalStat = StatsManager.Instance.PlayerStats().GetGameTotalStat("Global");
		return gameTotalStat.XP;
	}

	public static int TotalScore()
	{
		PlayerStat gameTotalStat = StatsManager.Instance.PlayerStats().GetGameTotalStat("Global");
		return gameTotalStat.Score + gameTotalStat.ScoreAsVeteran;
	}

	public static int CurrentMissionScore()
	{
		PlayerStat currentMissionStat = StatsManager.Instance.PlayerStats().GetCurrentMissionStat("Global");
		if (currentMissionStat.ScoreAsVeteran > 0)
		{
			return currentMissionStat.ScoreAsVeteran;
		}
		return currentMissionStat.Score;
	}

	public static int CurrentMissionXP()
	{
		PlayerStat currentMissionStat = StatsManager.Instance.PlayerStats().GetCurrentMissionStat("Global");
		return currentMissionStat.XP;
	}

	public static int CurrentMissionHardCurrencyEarned()
	{
		PlayerStat currentMissionStat = StatsManager.Instance.PlayerStats().GetCurrentMissionStat("Global");
		return currentMissionStat.HardCurrencyEarned;
	}

	public static int CurrentMissionHardCurrencySpent()
	{
		PlayerStat currentMissionStat = StatsManager.Instance.PlayerStats().GetCurrentMissionStat("Global");
		return currentMissionStat.HardCurrencySpent;
	}

	public static int CurrentMissionHardCurrencyPurchased()
	{
		PlayerStat currentMissionStat = StatsManager.Instance.PlayerStats().GetCurrentMissionStat("Global");
		return currentMissionStat.HardCurrencyPurchased;
	}

	public static int TotalHardCurrencyEarned()
	{
		PlayerStat gameTotalStat = StatsManager.Instance.PlayerStats().GetGameTotalStat("Global");
		return gameTotalStat.HardCurrencyEarned;
	}

	public static int TotalHardCurrencySpent()
	{
		PlayerStat gameTotalStat = StatsManager.Instance.PlayerStats().GetGameTotalStat("Global");
		return gameTotalStat.HardCurrencySpent;
	}

	public static int TotalHardCurrencyPurchased()
	{
		PlayerStat gameTotalStat = StatsManager.Instance.PlayerStats().GetGameTotalStat("Global");
		return gameTotalStat.HardCurrencyPurchased;
	}

	public static float TotalTimePlayed()
	{
		return StatsManager.Instance.MissionStats().TotalTimePlayed();
	}

	public static int MissionsPlayed()
	{
		return StatsManager.Instance.MissionStats().TotalMissionsPlayed();
	}

	public static bool AllCampaignMissionsComplete()
	{
		MissionData[] missions = MissionListings.Instance.Missions;
		foreach (MissionData missionData in missions)
		{
			if (missionData == null || !missionData.IncludeInStats)
			{
				continue;
			}
			for (int j = 0; j < missionData.Sections.Count; j++)
			{
				if (missionData.Sections[j].IsValidInCurrentBuild && !ActStructure.Instance.MissionIsSpecOps(missionData.MissionId, j))
				{
					MissionStat gameTotalStat = StatsManager.Instance.MissionStats().GetGameTotalStat(StatsManager.MissionStatId(missionData.MissionId, j));
					if (gameTotalStat.NumTimesSucceeded == 0)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public static void NumMissionsComplete(out int regular, out int veteran)
	{
		regular = 0;
		veteran = 0;
		MissionData[] missions = MissionListings.Instance.Missions;
		foreach (MissionData missionData in missions)
		{
			if (missionData == null || !missionData.IncludeInStats)
			{
				continue;
			}
			for (int j = 0; j < missionData.Sections.Count; j++)
			{
				SectionData sectionData = missionData.Sections[j];
				if (!sectionData.IsValidInCurrentBuild)
				{
					continue;
				}
				for (int k = 0; k < sectionData.Medals.Length; k++)
				{
					if (sectionData.Medals[k].MedalTargetType == MedalType.MissionComplete)
					{
						if (StatsManager.Instance.MedalManager().HasMedalBeenEarnedCurrentMission(missionData.MissionId, j, k, DifficultyMode.Regular) || StatsManager.Instance.MedalManager().HasMedalBeenEarnedGameTotal(missionData.MissionId, j, k, DifficultyMode.Regular))
						{
							regular++;
						}
						if (StatsManager.Instance.MedalManager().HasMedalBeenEarnedCurrentMission(missionData.MissionId, j, k, DifficultyMode.Veteran) || StatsManager.Instance.MedalManager().HasMedalBeenEarnedGameTotal(missionData.MissionId, j, k, DifficultyMode.Veteran))
						{
							veteran++;
						}
					}
				}
			}
		}
	}

	public static int ConsecutiveDaysPlayed()
	{
		uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
		DateTime dateTime = DateTime.Now;
		if (synchronizedTime.HasValue)
		{
			dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(synchronizedTime.Value);
			int num = ((!DateTime.IsLeapYear(dateTime.Year - 1)) ? 365 : 366);
			if (dateTime.DayOfYear == SecureStorage.Instance.LastPlayed + 1 || (SecureStorage.Instance.LastPlayed == num && dateTime.DayOfYear == 1))
			{
				SecureStorage.Instance.ConsecutiveDays++;
			}
			else if (dateTime.DayOfYear != SecureStorage.Instance.LastPlayed)
			{
				SecureStorage.Instance.ConsecutiveDays = 1;
				SecureStorage.Instance.NeedsDailyReward = false;
			}
		}
		if (Bedrock.getUserConnectionStatus() != 0)
		{
			if (SecureStorage.Instance.LastPlayed != dateTime.DayOfYear && SecureStorage.Instance.ConsecutiveDays > 1)
			{
				SecureStorage.Instance.NeedsDailyReward = true;
			}
			SecureStorage.Instance.LastPlayed = dateTime.DayOfYear;
		}
		return SecureStorage.Instance.ConsecutiveDays;
	}

	public static bool PlayedToday()
	{
		uint? synchronizedTime = SynchronizedClock.Instance.SynchronizedTime;
		DateTime now = DateTime.Now;
		if (synchronizedTime.HasValue)
		{
			now = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(synchronizedTime.Value);
			return SecureStorage.Instance.LastPlayed == now.DayOfYear;
		}
		return true;
	}

	public static int MissionsSuccessful()
	{
		return StatsManager.Instance.MissionStats().TotalMissionsSucceeded();
	}

	public static bool CurrentMissionSuccessful()
	{
		MissionListings.eMissionID id = StatsManager.Instance.MissionStats().CurrentMissionId();
		int section = StatsManager.Instance.MissionStats().CurrentMissionSection();
		return StatsManager.Instance.MissionStats().GetCurrentMissionStat(id, section).NumTimesSucceeded > 0;
	}

	private static MissionListings.eMissionID CurrentMissionId()
	{
		return StatsManager.Instance.MissionStats().CurrentMissionId();
	}

	public static void NumMissions(out int StoryMissions, out int SpecOpsOrTutorialMissions)
	{
		MissionListings.Instance.GetNumberOfMissionsForStats(out StoryMissions, out SpecOpsOrTutorialMissions);
	}

	public static float BestKDR(MissionListings.eMissionID missionId, int section)
	{
		PersonalBestStat gameTotalStat = StatsManager.Instance.PersonalBestStats().GetGameTotalStat(StatsManager.MissionStatId(missionId, section));
		return gameTotalStat.BestKDR;
	}

	public static float MostHeadShots(MissionListings.eMissionID missionId, int section)
	{
		PersonalBestStat gameTotalStat = StatsManager.Instance.PersonalBestStats().GetGameTotalStat(StatsManager.MissionStatId(missionId, section));
		return gameTotalStat.MostHeadShots;
	}

	public static float MostKills(MissionListings.eMissionID missionId, int section)
	{
		PersonalBestStat gameTotalStat = StatsManager.Instance.PersonalBestStats().GetGameTotalStat(StatsManager.MissionStatId(missionId, section));
		return gameTotalStat.MostKills;
	}

	public static int HighestScoreOverall(MissionListings.eMissionID missionId, int section)
	{
		int num = HighestScore(missionId, section);
		int num2 = HighestScoreAsVeteran(missionId, section);
		if (num > num2)
		{
			return num;
		}
		return num2;
	}

	public static int HighestScore(MissionListings.eMissionID missionId, int section)
	{
		PersonalBestStat gameTotalStat = StatsManager.Instance.PersonalBestStats().GetGameTotalStat(StatsManager.MissionStatId(missionId, section));
		return gameTotalStat.HighestScore;
	}

	public static int HighestScoreAsVeteran(MissionListings.eMissionID missionId, int section)
	{
		PersonalBestStat gameTotalStat = StatsManager.Instance.PersonalBestStats().GetGameTotalStat(StatsManager.MissionStatId(missionId, section));
		return gameTotalStat.HighestScoreAsVeteran;
	}

	public static float BestTimeToComplete(MissionListings.eMissionID missionId, int section)
	{
		PersonalBestStat gameTotalStat = StatsManager.Instance.PersonalBestStats().GetGameTotalStat(StatsManager.MissionStatId(missionId, section));
		return gameTotalStat.BestTimeToComplete;
	}

	public static int TotalMedalsEarned()
	{
		return StatsManager.Instance.MedalManager().TotalMedalsEarned();
	}

	public static int NumberOfMedalsAvailable()
	{
		int StoryMissions = 0;
		int SpecOpsOrTutorialMissions = 0;
		NumMissions(out StoryMissions, out SpecOpsOrTutorialMissions);
		return StoryMissions * 2 * SectionData.numMedalSlots + SpecOpsOrTutorialMissions * SectionData.numMedalSlots;
	}

	public static bool HasMedalBeenEarnedGameTotal(MissionListings.eMissionID missionId, int section, int medal, DifficultyMode mode)
	{
		return StatsManager.Instance.MedalManager().HasMedalBeenEarnedGameTotal(missionId, section, medal, mode);
	}

	public static bool HasMedalBeenEarnedCurrentMission(int medal, DifficultyMode mode)
	{
		MissionListings.eMissionID missionId = StatsManager.Instance.MissionStats().CurrentMissionId();
		int section = StatsManager.Instance.MissionStats().CurrentMissionSection();
		return StatsManager.Instance.MedalManager().HasMedalBeenEarnedCurrentMission(missionId, section, medal, mode);
	}

	public static int IntelCollectedForCurrentMission()
	{
		MissionListings.eMissionID missionId = StatsManager.Instance.MissionStats().CurrentMissionId();
		int section = StatsManager.Instance.MissionStats().CurrentMissionSection();
		return IntelCollectedForMission(missionId, section);
	}

	public static int IntelCollectedForMission(MissionListings.eMissionID missionId, int section)
	{
		int result = 0;
		MissionData missionData = MissionListings.Instance.Mission(missionId);
		if (missionData != null && section >= 0 && section < missionData.Sections.Count)
		{
			SectionData sectionData = missionData.Sections[section];
			result = PickupManager.Instance.NumBeenPickedUp(missionId, section, sectionData.IntelToCollect);
		}
		return result;
	}

	public static int GetXPFromKillsForCharacter(string id)
	{
		return StatsManager.Instance.CharacterXPStats().GetCurrentMissionStat(id).XPFromKills;
	}

	public static int GetXPFromBonusesForCharacter(string id)
	{
		return StatsManager.Instance.CharacterXPStats().GetCurrentMissionStat(id).XPFromBonuses;
	}

	public static int GetNumKillsForCharacter(string id)
	{
		return StatsManager.Instance.CharacterStats().GetCurrentMissionStat(id).NumKills;
	}

	public static int GetTotalKillsForCharacter(string id)
	{
		return StatsManager.Instance.CharacterStats().GetGameTotalStat(id).NumKills;
	}

	public static int GetTotalDeathsForCharacter(string id)
	{
		return StatsManager.Instance.CharacterStats().GetGameTotalStat(id).NumTimesKilled;
	}

	public static float GetTotalKDRForCharacter(string id)
	{
		return StatsManager.Instance.CharacterStats().GetGameTotalStat(id).KDR;
	}

	public static int GetNumBonusesForCharacter(string id)
	{
		return StatsManager.Instance.CharacterXPStats().GetCurrentMissionStat(id).NumBonuses;
	}

	public static bool WasCharacterKIA(string id)
	{
		return StatsManager.Instance.CharacterStats().GetCurrentMissionStat(id).NumTimesKilled > StatsManager.Instance.CharacterStats().GetCurrentMissionStat(id).NumTimesHealed;
	}

	public static void SubtractXPForCharacter(string id, int amt)
	{
		StatsManager.Instance.CharacterXPStats().GetCurrentMissionStat(id).XPFromKills -= amt;
	}

	public static bool WasAchievementEarned(string id)
	{
		return StatsManager.Instance.AchievementManager().GetCurrentMissionStat(id).Step > 0 && StatsManager.Instance.AchievementManager().GetGameTotalStat(id).PercentComplete() == 100;
	}

	public static bool DidPerkReachPro(PerkType id)
	{
		return StatsManager.Instance.PerksManager().ReachedProThisMission(id);
	}

	public static float CurrentMissionTotalTime()
	{
		MissionListings.eMissionID missionId = StatsManager.Instance.MissionStats().CurrentMissionId();
		int section = StatsManager.Instance.MissionStats().CurrentMissionSection();
		return StatsManager.Instance.MissionStats().GetCurrentMissionStat(StatsManager.MissionStatId(missionId, section)).TimePlayed;
	}

	public static int CurrentMissionTimesPlayed()
	{
		MissionListings.eMissionID missionId = StatsManager.Instance.MissionStats().CurrentMissionId();
		int section = StatsManager.Instance.MissionStats().CurrentMissionSection();
		return StatsManager.Instance.MissionStats().GetCurrentMissionStat(StatsManager.MissionStatId(missionId, section)).NumTimesPlayed;
	}

	public static int CurrentMissionTimesSucceeded()
	{
		MissionListings.eMissionID missionId = StatsManager.Instance.MissionStats().CurrentMissionId();
		int section = StatsManager.Instance.MissionStats().CurrentMissionSection();
		return StatsManager.Instance.MissionStats().GetCurrentMissionStat(StatsManager.MissionStatId(missionId, section)).NumTimesSucceeded;
	}

	public static int HighestSpecOpsWaveCompletedGameTotal(MissionListings.eMissionID id, int s)
	{
		MissionStat gameTotalStat = StatsManager.Instance.MissionStats().GetGameTotalStat(StatsManager.MissionStatId(id, s));
		return gameTotalStat.HighestSpecOpsWaveCompleted;
	}

	public static int HighestSpecOpsWaveCompletedThisMission(MissionListings.eMissionID id, int s)
	{
		MissionStat currentMissionStat = StatsManager.Instance.MissionStats().GetCurrentMissionStat(StatsManager.MissionStatId(id, s));
		return currentMissionStat.HighestSpecOpsWaveCompleted;
	}
}
