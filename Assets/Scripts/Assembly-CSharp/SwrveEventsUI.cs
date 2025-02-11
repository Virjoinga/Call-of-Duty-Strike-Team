public class SwrveEventsUI
{
	public static void UsedPreset()
	{
		Bedrock.AnalyticsLogEvent("UI.Loadout.UsedPreset");
	}

	public static void StoreButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.General.StoreButtonTouched");
	}

	public static void LeaderboardsButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.General.LeaderboardsButtonTouched");
	}

	public static void AchievementsButtonTouched()
	{
		Bedrock.AnalyticsLogEvent("UI.General.AchievementsButtonTouched");
	}

	public static void MusicTurnedOn()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.MusicTurnedOn");
	}

	public static void MusicTurnedOff()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.MusicTurnedOff");
	}

	public static void SFXTurnedOn()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.SFXTurnedOn");
	}

	public static void SFXTurnedOff()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.SFXTurnedOff");
	}

	public static void ViewedOptions()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedOptions");
	}

	public static void ViewedCredits()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedCredits");
	}

	public static void ViewedTBFFacebook()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedTBFFacebookLink");
	}

	public static void ViewedTBFTwitter()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedTBFTwitterLink");
	}

	public static void ViewedCODFacebook()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedCODFacebookLink");
	}

	public static void ViewedCODTwitter()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedCODTwitterLink");
	}

	public static void ViewedMoreGames()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedMoreGamesLink");
	}

	public static void CancelledTBFInfoMessage()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.CancelledTBFInfoMessage");
	}

	public static void ViewedChallenges()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedChallenges");
	}

	public static void ViewedStats()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedStats");
	}

	public static void ViewedFAQ()
	{
		Bedrock.AnalyticsLogEvent("UI.Globe.ViewedFAQ");
	}

	public static void ViewedCareer()
	{
		Bedrock.AnalyticsLogEvent("UI.Stats.ViewedCareer");
	}

	public static void ViewedCombat()
	{
		Bedrock.AnalyticsLogEvent("UI.Stats.ViewedCombat");
	}

	public static void ViewedSquad()
	{
		Bedrock.AnalyticsLogEvent("UI.Stats.ViewedSquad");
	}

	public static void ViewedMissions()
	{
		Bedrock.AnalyticsLogEvent("UI.Stats.ViewedMissions");
	}

	public static void ViewedLeaderboards()
	{
		Bedrock.AnalyticsLogEvent("UI.Stats.ViewedLeaderboards");
	}

	public static void ViewedAchievements()
	{
		Bedrock.AnalyticsLogEvent("UI.Stats.ViewedAchievements");
	}

	public static void ViewedWeapons()
	{
		Bedrock.AnalyticsLogEvent("UI.Loadout.ViewedWeapons");
	}

	public static void ViewedBundles()
	{
		Bedrock.AnalyticsLogEvent("UI.Loadout.ViewedBundles");
	}

	public static void ViewedArmour()
	{
		Bedrock.AnalyticsLogEvent("UI.Loadout.ViewedArmour");
	}

	public static void ViewedPerks()
	{
		Bedrock.AnalyticsLogEvent("UI.Loadout.ViewedPerks");
	}

	public static void ViewedEquipment()
	{
		Bedrock.AnalyticsLogEvent("UI.Loadout.ViewedEquipment");
	}

	public static void ViewedLockedSlot()
	{
		Bedrock.AnalyticsLogEvent("UI.Perks.ViewedLockedSlot");
	}

	public static void GyroscopeTurnedOn()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.GyroscopeTurnedOn");
	}

	public static void GyroscopeTurnedOff()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.GyroscopeTurnedOff");
	}

	public static void GesturesTurnedOn()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.GesturesTurnedOn");
	}

	public static void GesturesTurnedOff()
	{
		Bedrock.AnalyticsLogEvent("UI.Options.GesturesTurnedOff");
	}

	public static void SeenPromo()
	{
		Bedrock.AnalyticsLogEvent("UI.Activate.SeenPromo", "PlayerLevel", SwrvePayload.PlayerLevel, false);
	}

	public static void OpenedDialog()
	{
		Bedrock.AnalyticsLogEvent("UI.Activate.OpenedDialog");
	}

	public static void ActivateFirstSignIn()
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed);
		Bedrock.AnalyticsLogEvent("UI.Activate.FirstSignIn", parameters, false);
	}

	public static void RatedApp()
	{
		Bedrock.AnalyticsLogEvent("UI.RatedApp");
	}

	public static void SeenRateUs()
	{
		Bedrock.AnalyticsLogEvent("UI.SeenRateUs");
	}

	public static void ViewedTokenPurchase()
	{
		Bedrock.AnalyticsLogEvent("UI.ViewedTokenPurchase");
	}

	public static void ViewedLoadOut()
	{
		Bedrock.AnalyticsLogEvent("UI.Loadout.ViewedLoadOut");
	}

	public static void LinkedAccountPressed()
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed);
		Bedrock.AnalyticsLogEvent("UI.Elite.LinkedAccountPressed", parameters, false);
	}

	public static void LinkedAccountDone()
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed);
		Bedrock.AnalyticsLogEvent("UI.Elite.LinkedAccountDone", parameters, false);
	}

	public static void EliteAppInstalled()
	{
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash("PlayerLevel", SwrvePayload.PlayerLevel, "MissionsStarted", SwrvePayload.MissionsStarted, "MissionsPassed", SwrvePayload.MissionsPassed);
		Bedrock.AnalyticsLogEvent("UI.Elite.EliteAppInstalled", parameters, false);
	}

	public static void Viewed(MissionListings.eMissionID id)
	{
		string name = "UI.Location." + id.ToString() + ".Viewed";
		Bedrock.AnalyticsLogEvent(name, false);
	}

	public static void ViewedAll(MissionListings.eMissionID id)
	{
		string name = "UI.Location." + id.ToString() + ".ViewedAll";
		Bedrock.AnalyticsLogEvent(name, false);
	}

	public static void MoreInfoViewed(string id)
	{
		Bedrock.AnalyticsLogEvent("UI.MoreInfoViewed." + id, "Mission", SwrvePayload.CurrentMission, false);
	}

	public static void SwrveTalkTrigger_MainUI()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.mainui");
	}

	public static void SwrveTalkTrigger_Store()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.store");
	}

	public static void SwrveTalkTrigger_Globe()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.globe");
	}

	public static void SwrveTalkTrigger_LoadOut()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.loadout");
	}

	public static void SwrveTalkTrigger_MissionSelect()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.missionselect");
	}

	public static void SwrveTalkTrigger_Results()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.results");
	}

	public static void SwrveTalkTrigger_LevelUp()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.levelup");
	}

	public static void SwrveTalkTrigger_PurchaseItem()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.purchaseitem");
	}

	public static void SwrveTalkTrigger_PurchaseTokens()
	{
		Bedrock.AnalyticsLogEvent("swrvetalk.trigger.purchasetokens");
	}

	public static void ControllerFirstConnected()
	{
		Bedrock.AnalyticsLogEvent("Controller.FirstConnected", "PlayerLevel", SwrvePayload.PlayerLevel, "ControllerType", SwrvePayload.ControllerType, false);
	}
}
