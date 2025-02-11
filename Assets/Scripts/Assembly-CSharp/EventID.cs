public class EventID
{
	public class Mission
	{
		public const string Started = "game.missionstarted";

		public const string Ended = "game.missionend";

		public const string Unlocked = "game.missionunlocked";

		public const string Kill = "game.kill";

		public const string WeaponFired = "game.weaponfired";

		public const string GrenadeThrown = "game.grenadethrown";

		public const string ObjectiveCompleted = "game.objective";

		public const string IntelCollected = "game.intelcollected";

		public const string AmmoCacheUsed = "game.AmmoCacheUsed";

		public const string MysteryCacheUsed = "game.MysteryCacheUsed";

		public const string AmmoCollected = "game.AmmoCollected";

		public const string CharacterHealed = "game.CharacterHealed";

		public const string WaveComplete = "game.waveComplete";

		public const string WaveStarted = "game.waveStarted";

		public const string GMGScoreAdded = "game.gmgScoreAdded";

		public const string GameplayMinutePassed = "game.gameplayMinutePassed";

		public const string NewFlashpoint = "game.newflashpoint";
	}

	public class Stats
	{
		public const string MedalEarned = "stats.medalearned";

		public const string AchievementCompleted = "stats.achievement";

		public const string XPEarned = "stats.xpearned";

		public const string XPHeliDestroyed = "stats.xphelidestroyed";
	}

	public class Challenges
	{
		public const string ChallengeJoined = "gmg.challengejoined";

		public const string ChallengeCompleted = "gmg.challengecompleted";
	}

	public class UI
	{
		public const string OptionsPressed = "game.ui.optionspressed";
	}

	public class MTXSocial
	{
		public const string HardCurrencyChanged = "game.mtxsocial.hardcurrencychanged";

		public const string PurchaseArmour = "game.mtxsocial.purchasearmour";

		public const string PurchaseEquipment = "game.mtxsocial.purchaseequipment";

		public const string PurchasePerk = "game.mtxsocial.purchaseperk";

		public const string PerkUnlocked = "game.mtxsocial.perkunlocked";

		public const string Share = "game.mtxsocial.share";
	}
}
