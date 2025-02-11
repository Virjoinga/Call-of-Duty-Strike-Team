using System;

public class SwrveEventsPurchase
{
	public static string PaymentProvider = "Other";

	public static void HardCurrencyPack(string Identifier, int index, ulong Cost, ulong VirtualCurrencyAmount)
	{
		int value = SecureStorage.Instance.GetInt("totalPreviousPurchases") + 1;
		SecureStorage.Instance.SetInt("totalPreviousPurchases", value);
		string key = SwrveNames.General.PreviousPurchases(Identifier);
		int value2 = SecureStorage.Instance.GetInt(key) + 1;
		SecureStorage.Instance.SetInt(key, value2);
		SecureStorage.Instance.SetString("LastPackPurchased", Identifier);
		DateTime now = DateTime.Now;
		DateTime utcNow = DateTime.UtcNow;
		string text = now.ToString("dd-MM-yyyy");
		string text2 = now.ToString("HH:mm:ss");
		SecureStorage.Instance.SetString("LastPackPurchasedDate", text + " " + text2);
		string text3 = utcNow.ToString("dd-MM-yyyy");
		string text4 = utcNow.ToString("HH:mm:ss");
		SecureStorage.Instance.SetString("LastPackPurchasedDateLocal", text3 + " " + text4);
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("CurrentHardCurrencyTotal", SwrvePayload.CurrentHardCurrencyTotal, "PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed, "Identifier", Identifier, "PrevPurchases", value2.ToString(), "TotalPrevPurchases", value.ToString());
		Bedrock.AnalyticsLogEvent("Purchases.Currency.HardCurrencyPack", parameters, false);
		Bedrock.AnalyticsLogEvent("Purchases.Currency.HardCurrencyPack" + index, parameters, false);
		SwrveEventsUI.SwrveTalkTrigger_PurchaseTokens();
	}

	public static void HardCurrencyPackFailed(string Identifier)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("CurrentHardCurrencyTotal", SwrvePayload.CurrentHardCurrencyTotal, "PlayerLevel", SwrvePayload.PlayerLevel, "Identifier", Identifier);
		Bedrock.AnalyticsLogEvent("Purchases.Currency.HardCurrencyPackFailed", parameters, false);
	}

	public static void HardCurrencyPackCancelled(string Identifier)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("CurrentHardCurrencyTotal", SwrvePayload.CurrentHardCurrencyTotal, "PlayerLevel", SwrvePayload.PlayerLevel, "Identifier", Identifier);
		Bedrock.AnalyticsLogEvent("Purchases.Currency.HardCurrencyPackCancelled", parameters, false);
	}

	public static void ProductListRecieved(int count)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Count", count.ToString(), "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel);
		Bedrock.AnalyticsLogEvent("Purchases.Currency.ProductListReceived", parameters, false);
	}

	public static void HardCurrencyPurchase(ulong cost, ulong num, string itemName)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("Cost", cost.ToString(), "Device", SwrvePayload.Device, "DeviceManufacturer", SwrvePayload.DeviceManufacturer, "DeviceModel", SwrvePayload.DeviceModel, "PurchaseItem", itemName);
		Bedrock.AnalyticsLogEvent("Purchases.Currency.HardCurrencyPurchase", parameters, false);
		Bedrock.brKeyValueArray parameters2 = BedrockUtils.Hash("CurrentHardCurrencyTotal", SwrvePayload.CurrentHardCurrencyTotal);
		ulong itemCost = cost / num;
		Bedrock.AnalyticsLogVirtualPurchase(itemName, itemCost, num, "Tokens", parameters2);
		SwrveEventsUI.SwrveTalkTrigger_PurchaseItem();
	}

	public static void HardCurrencyAwarded(ulong amount, string itemName)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("AwardItem", itemName);
		Bedrock.AnalyticsLogVirtualCurrencyAwarded(amount, "Tokens", parameters);
	}

	public static void PerkSlotPurchase()
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed, "Mission", SwrvePayload.CurrentMission);
		Bedrock.AnalyticsLogEvent("Purchases.Enhancements.PerkSlotPurchase", parameters, false);
	}

	public static void BundlePurchase(string name)
	{
		int value = SecureStorage.Instance.GetInt("bundlesPurchased") + 1;
		SecureStorage.Instance.SetInt("bundlesPurchased", value);
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed, "BundlesPurchased", value.ToString(), "Mission", SwrvePayload.CurrentMission);
		Bedrock.AnalyticsLogEvent("Purchases.Bundles.BundlePurchase_" + name, parameters, false);
		Bedrock.AnalyticsLogEvent("Purchases.Bundles.BundlePurchase_any", parameters, false);
	}

	public static void HealthKitPurchase(int numItems)
	{
		if (SecureStorage.Instance.UsedHealthKitSinceLastPurchase)
		{
			RepeatPurchase("HealthKits");
			SecureStorage.Instance.UsedHealthKitSinceLastPurchase = false;
		}
		Bedrock.AnalyticsLogEvent("Purchases.Equipment.HealthKit", "Amount", numItems.ToString(), "Location", SwrvePayload.Location, "Mission", SwrvePayload.CurrentMission, false);
	}

	public static void AmmoPackPurchase(int cost)
	{
		Bedrock.AnalyticsLogEvent("Purchases.Equipment.Ammo", "Mission", SwrvePayload.CurrentMission, "Cost", cost.ToString(), false);
	}

	public static void GrenadePurchase(int numItems)
	{
		if (SecureStorage.Instance.UsedGrenadeSinceLastPurchase)
		{
			RepeatPurchase("Grenades");
			SecureStorage.Instance.UsedGrenadeSinceLastPurchase = false;
		}
		Bedrock.AnalyticsLogEvent("Purchases.Equipment.GrenadePurchase", "Amount", numItems.ToString(), "Location", SwrvePayload.Location, "Mission", SwrvePayload.CurrentMission, false);
	}

	public static void ClaymorePurchase(int numItems)
	{
		if (SecureStorage.Instance.UsedClaymoreSinceLastPurchase)
		{
			RepeatPurchase("Claymores");
			SecureStorage.Instance.UsedClaymoreSinceLastPurchase = false;
		}
		Bedrock.AnalyticsLogEvent("Purchases.Equipment.Claymore", "Amount", numItems.ToString(), "Location", SwrvePayload.Location, "Mission", SwrvePayload.CurrentMission, false);
	}

	public static void RepeatPurchase(string itemName)
	{
		Bedrock.AnalyticsLogEvent("Purchases.Equipment.RepeatPurchase", "PurchaseItem", itemName, false);
	}

	public static void ArmourIncrease(int level)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed, "Mission", SwrvePayload.CurrentMission);
		Bedrock.AnalyticsLogEvent("Purchases.Enhancements.Armour_" + level, parameters, false);
	}

	public static void SlotIncrease(EquipmentIconController.EquipmentType type, int increaseLevel)
	{
		string name = "Purchases.Enhancements." + type.ToString() + "Increase_" + increaseLevel;
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed, "Mission", SwrvePayload.CurrentMission);
		Bedrock.AnalyticsLogEvent(name, parameters, false);
	}

	public static void XPRecover(int xp)
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "Mission", SwrvePayload.CurrentMission, "XPAmount", xp.ToString());
		Bedrock.AnalyticsLogEvent("Purchases.Other.XPRecover", parameters, false);
	}
}
