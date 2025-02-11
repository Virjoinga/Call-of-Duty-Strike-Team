public class SwrveEventsGameplay
{
	public static void FirstRun()
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, "OSVersion", SwrvePayload.OSVersion, "Language", SwrvePayload.CurrentLanguage);
		Bedrock.AnalyticsLogEvent("General.FirstRun", parameters, false);
	}

	public static void MissionStarted(MissionListings.eMissionID id, int section)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Mission", StatsManager.MissionSwrveId(id, section), "Perk1", SwrvePayload.PerkSlot(0), "Perk2", SwrvePayload.PerkSlot(1), "Perk3", SwrvePayload.PerkSlot(2), "LoadoutSet", SwrvePayload.AutoLoadout);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.MissionStarted", parameters, false);
	}

	public static void MissionComplete(MissionListings.eMissionID id, int section)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Mission", StatsManager.MissionSwrveId(id, section), "GyroscopeOnOff", SwrvePayload.GyroscopeOnOff, "GesturesOnOff", SwrvePayload.GesturesOnOff, "Audio", SwrvePayload.Audio, "RegularMissionsCompleted", SwrvePayload.RegularMissionsCompleted, "VeteranMissionsCompleted", SwrvePayload.VeteranMissionsCompleted, "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, "ControllerType", SwrvePayload.ControllerType);
		Bedrock.AnalyticsLogEvent("Gameplay.Game.MissionComplete", parameters, false);
	}

	public static void MissionPass(MissionListings.eMissionID id, int section)
	{
		Bedrock.AnalyticsLogEvent("Gameplay.Game.MissionPass", "Mission", StatsManager.MissionSwrveId(id, section), false);
	}

	public static void MissionFail(MissionListings.eMissionID id, int section)
	{
		Bedrock.AnalyticsLogEvent("Gameplay.Game.MissionFail", "Mission", StatsManager.MissionSwrveId(id, section), false);
	}

	public static void GrenadeUsed()
	{
		SecureStorage.Instance.UsedGrenadeSinceLastPurchase = true;
		Bedrock.AnalyticsLogEvent("Gameplay.Consumable.GrenadeUsed", "Mission", StatsManager.MissionSwrveId(ActStructure.Instance.CurrentMissionID, ActStructure.Instance.CurrentMissionSection), false);
	}

	public static void ClaymoreUsed()
	{
		SecureStorage.Instance.UsedClaymoreSinceLastPurchase = true;
		Bedrock.AnalyticsLogEvent("Gameplay.Consumable.ClaymoreUsed", "Mission", StatsManager.MissionSwrveId(ActStructure.Instance.CurrentMissionID, ActStructure.Instance.CurrentMissionSection), false);
	}

	public static void HealthKitUsed()
	{
		SecureStorage.Instance.UsedHealthKitSinceLastPurchase = true;
		Bedrock.AnalyticsLogEvent("Gameplay.Consumable.HealthKitUsed", "Mission", StatsManager.MissionSwrveId(ActStructure.Instance.CurrentMissionID, ActStructure.Instance.CurrentMissionSection), false);
	}

	public static void WaveComplete(MissionListings.eMissionID id, int section, GMGData.GameType type, int wave)
	{
		string name = "Gameplay." + type.ToString() + "." + StatsManager.MissionSwrveId(id, section) + ".Wave_" + wave;
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Revives", SwrvePayload.WaveTimesHealed, "AmmoBought_Shotgun", SwrvePayload.WaveAmmoCacheTimesUsed(WeaponDescriptor.WeaponClass.Shotgun), "AmmoBought_AssaultRifle", SwrvePayload.WaveAmmoCacheTimesUsed(WeaponDescriptor.WeaponClass.AssaultRifle), "AmmoBought_LightMachineGun", SwrvePayload.WaveAmmoCacheTimesUsed(WeaponDescriptor.WeaponClass.LightMachineGun), "AmmoBought_SniperRifle", SwrvePayload.WaveAmmoCacheTimesUsed(WeaponDescriptor.WeaponClass.SniperRifle));
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}

	public static void SpecOpsMissionStarted(MissionListings.eMissionID id, int section, GMGData.GameType type)
	{
		string name = "Gameplay." + type.ToString() + "." + StatsManager.MissionSwrveId(id, section) + ".Started";
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Perk1", SwrvePayload.PerkSlot(0), "Perk2", SwrvePayload.PerkSlot(1), "Perk3", SwrvePayload.PerkSlot(2));
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}

	public static void SpecOpsMissionComplete(MissionListings.eMissionID id, int section, GMGData.GameType type)
	{
		string name = "Gameplay." + type.ToString() + "." + StatsManager.MissionSwrveId(id, section) + ".Finished";
		int num = StatsHelper.CurrentMissionHardCurrencyEarned();
		int num2 = StatsHelper.CurrentMissionHardCurrencySpent();
		int num3 = num - num2;
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("TokensEarned", num.ToString(), "TokensSpent", num2.ToString(), "TokensSplit", num3.ToString(), "Weapon", SwrvePayload.CurrentWeapon, "MinutesInSection", SwrvePayload.MinutesInSection);
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}
}
