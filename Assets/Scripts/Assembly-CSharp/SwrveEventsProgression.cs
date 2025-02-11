using UnityEngine;

public class SwrveEventsProgression
{
	private static string OnBoardingStageKey = "OnBoardingStage";

	private static float startTime;

	public static void AchievementCompleted(string Identifier)
	{
		Bedrock.AnalyticsLogEvent("Progression.Achievements.AchievementAwarded", "Achievement", Identifier, false);
	}

	public static void PlayerLevelUp(int level)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", level.ToString(), "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel);
		Bedrock.AnalyticsLogEvent("Progression.Player.LevelUp", parameters, false);
	}

	public static void PlayerPrestiged(int prestigeLevel)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("CurrentHardCurrencyTotal", SwrvePayload.CurrentHardCurrencyTotal, "TimesPrestiged", prestigeLevel.ToString(), "MissionsStarted", SwrvePayload.MissionsStarted, "TotalMTX", SwrvePayload.TotalMTX);
		Bedrock.AnalyticsLogEvent("Progression.Player.Prestiged", parameters, false);
	}

	public static void LogosDisplayed()
	{
		int @int = SecureStorage.Instance.GetInt(OnBoardingStageKey);
		if (@int == 0)
		{
			@int++;
			SecureStorage.Instance.SetInt(OnBoardingStageKey, @int);
			Bedrock.AnalyticsLogEvent("Progression.Onboarding.LogosDisplayed");
		}
	}

	public static void TouchToStartDisplayed()
	{
		int @int = SecureStorage.Instance.GetInt(OnBoardingStageKey);
		if (@int == 1)
		{
			@int++;
			SecureStorage.Instance.SetInt(OnBoardingStageKey, @int);
			Bedrock.AnalyticsLogEvent("Progression.Onboarding.TouchToStartDisplayed");
		}
	}

	public static void TouchToStartTouched()
	{
		int @int = SecureStorage.Instance.GetInt(OnBoardingStageKey);
		if (@int == 2)
		{
			@int++;
			SecureStorage.Instance.SetInt(OnBoardingStageKey, @int);
			Bedrock.AnalyticsLogEvent("Progression.Onboarding.TouchToStartTouched");
		}
	}

	public static void FirstGlobeView()
	{
		int @int = SecureStorage.Instance.GetInt(OnBoardingStageKey);
		if (@int == 3)
		{
			@int++;
			SecureStorage.Instance.SetInt(OnBoardingStageKey, @int);
			Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel);
			Bedrock.AnalyticsLogEvent("Progression.Onboarding.FirstGlobeView", parameters, false);
		}
	}

	public static void FirstSectionSelect()
	{
		int @int = SecureStorage.Instance.GetInt(OnBoardingStageKey);
		if (@int == 4)
		{
			@int++;
			SecureStorage.Instance.SetInt(OnBoardingStageKey, @int);
			Bedrock.AnalyticsLogEvent("Progression.Onboarding.FirstSectionSelect");
		}
	}

	public static void StartTutorialSection(string section)
	{
		startTime = Time.realtimeSinceStartup;
		string name = "Progression.Tutorial.StartSection" + section;
		Bedrock.AnalyticsLogEvent(name);
	}

	public static void EndTutorialSection(string section, int timesFailed)
	{
		int num = (int)(Time.realtimeSinceStartup - startTime);
		string name = "Progression.Tutorial.CompletedSection" + section;
		Bedrock.AnalyticsLogEvent(name, "TimesFailed", timesFailed.ToString(), "TimeTaken", num.ToString(), false);
	}

	public static void StartedCampaignSection(MissionListings.eMissionID id, int section)
	{
		string name = "Progression.Campaign." + StatsManager.MissionSwrveId(id, section) + ".Start";
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Difficulty", SwrvePayload.Difficulty, "Perk1", SwrvePayload.PerkSlot(0), "Perk2", SwrvePayload.PerkSlot(1), "Perk3", SwrvePayload.PerkSlot(2), "Weapon1", SwrvePayload.WeaponforSoldier(0), "Weapon2", SwrvePayload.WeaponforSoldier(1), "Weapon3", SwrvePayload.WeaponforSoldier(2), "Weapon4", SwrvePayload.WeaponforSoldier(3));
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}

	public static void PassedCampaignSection(MissionListings.eMissionID id, int section)
	{
		string name = "Progression.Campaign." + StatsManager.MissionSwrveId(id, section) + ".Passed";
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Difficulty", SwrvePayload.Difficulty, "TimesFailed", SwrvePayload.CurrentMissionTimesFailed, "TimeInSection", SwrvePayload.TimeInSection);
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}

	public static void PerkUnlocked(PerkType perk, bool paid)
	{
		Perk perk2 = StatsManager.Instance.PerksList.Perks[(int)perk];
		string name = "Progression.Unlocks.Perk_" + perk2.Tier.ToString() + "_" + perk2.Index + "_" + perk2.Identifier;
		Bedrock.AnalyticsLogEvent(name, "Paid", paid.ToString(), "PlayerLevel", SwrvePayload.PlayerLevel, false);
	}

	public static void ProPerkUnlocked(PerkType perk, bool paid)
	{
		Perk perk2 = StatsManager.Instance.PerksList.Perks[(int)perk];
		string name = "Progression.Unlocks.ProPerk_" + perk2.Tier.ToString() + "_" + perk2.Index + "_" + perk2.Identifier;
		Bedrock.AnalyticsLogEvent(name, "Paid", paid.ToString(), "PlayerLevel", SwrvePayload.PlayerLevel, false);
	}

	public static void WeaponUnlocked(string name)
	{
		string name2 = "Progression.Unlocks.Weapon_" + name;
		Bedrock.AnalyticsLogEvent(name2);
	}
}
