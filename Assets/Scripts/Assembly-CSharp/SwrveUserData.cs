using System;
using System.Collections.Generic;
using UnityEngine;

public class SwrveUserData : MonoBehaviour, ISaveLoad
{
	private const string ChallengesJoinedKey = "userdata.challengesjoined";

	private const string ChallengesCompletedKey = "userdata.challengescompleted";

	private const string SpecOpsWavesStartedKey = "userdata.wavesstarted";

	private const string SpecOpsWave10CompletedKey = "userdata.wave10complete";

	private const string HighestSpecOpsWaveKey = "userdata.highestwave";

	private const string MaxConsecutiveDaysKey = "userdata.maxdays";

	private const string MissionsUnlockedKey = "userdata.missions";

	private const string CampaignMissionsStartedNormalKey = "userdata.campaignstartnorm";

	private const string CampaignMissionsStartedVeteranKey = "userdata.campaignstartvet";

	private const string CampaignMissionsCompletedNormalKey = "userdata.campaigncompletenorm";

	private const string CampaignMissionsCompletedVeteranKey = "userdata.campaigncompletevet";

	private const string PerkSlotsBoughtKey = "userdata.perkslots";

	private const string PerksBoughtKey = "userdata.perks";

	private const string PerkUpgradesBoughtKey = "userdata.perkupgrades";

	private const string AssaultOwnedKey = "userdata.assault";

	private const string StealthOwnedKey = "userdata.stealth";

	private const string HeavyOwnedKey = "userdata.heavy";

	private const string BalancedOwnedKey = "userdata.balanced";

	private const string CompleteOwnedKey = "userdata.complete";

	private const string ArmourUpgradesKey = "userdata.armourupgrades";

	private const string GrenadesBoughtKey = "userdata.grenades";

	private const string ClaymoresBoughtKey = "userdata.claymores";

	private const string MedPacksBoughtKey = "userdata.medpacks";

	private const string GrenadeCapacityBoughtKey = "userdata.grenadecapacity";

	private const string ClaymoreCapacityBoughtKey = "userdata.claymorecapacity";

	private const string MedPackCapacityBoughtKey = "userdata.medpackcapacity";

	private const string AmmoCachesUsedKey = "userdata.ammocachesused";

	private const string MysteryCachesUsedKey = "userdata.mysterycachesused";

	private const string TotalCurrencyPacksBoughtKey = "userdata.currencypacks";

	private static SwrveUserData mInstance;

	private int mChallengesJoined;

	private int mChallengesCompleted;

	private int mSpecOpsWavesStarted;

	private int mSpecOpsWave10Completed;

	private int mHighestSpecOpsWave;

	private int mMaxConsecutiveDays;

	private int mMissionsUnlocked;

	private int mCampaignMissionsStartedNormal;

	private int mCampaignMissionsStartedVeteran;

	private int mCampaignMissionsCompletedNormal;

	private int mCampaignMissionsCompletedVeteran;

	private int mPerkSlotsBought;

	private int mPerksBought;

	private int mPerkUpgradesBought;

	private int mAssaultOwned;

	private int mStealthOwned;

	private int mHeavyOwned;

	private int mBalancedOwned;

	private int mCompleteOwned;

	private int mArmourUpgrades;

	private int mGrenadesBought;

	private int mClaymoresBought;

	private int mMedPacksBought;

	private int mGrenadeCapacityBought;

	private int mClaymoreCapacityBought;

	private int mMedPackCapacityBought;

	private int mAmmoCachesUsed;

	private int mMysteryCachesUsed;

	private int mTotalCurrencyPacksBought;

	private float mRunningTime;

	public int ChallengesJoined
	{
		get
		{
			return mChallengesJoined;
		}
	}

	public static SwrveUserData Instance
	{
		get
		{
			return mInstance;
		}
	}

	public void Load()
	{
		mChallengesJoined = SecureStorage.Instance.GetInt("userdata.challengesjoined", mChallengesJoined);
		mChallengesCompleted = SecureStorage.Instance.GetInt("userdata.challengescompleted", mChallengesCompleted);
		mSpecOpsWavesStarted = SecureStorage.Instance.GetInt("userdata.wavesstarted", mSpecOpsWavesStarted);
		mSpecOpsWave10Completed = SecureStorage.Instance.GetInt("userdata.wave10complete", mSpecOpsWave10Completed);
		mHighestSpecOpsWave = SecureStorage.Instance.GetInt("userdata.highestwave", mHighestSpecOpsWave);
		mMaxConsecutiveDays = SecureStorage.Instance.GetInt("userdata.maxdays", mMaxConsecutiveDays);
		mMissionsUnlocked = SecureStorage.Instance.GetInt("userdata.missions", mMissionsUnlocked);
		mCampaignMissionsStartedNormal = SecureStorage.Instance.GetInt("userdata.campaignstartnorm", mCampaignMissionsStartedNormal);
		mCampaignMissionsStartedVeteran = SecureStorage.Instance.GetInt("userdata.campaignstartvet", mCampaignMissionsStartedVeteran);
		mCampaignMissionsCompletedNormal = SecureStorage.Instance.GetInt("userdata.campaigncompletenorm", mCampaignMissionsCompletedNormal);
		mCampaignMissionsCompletedVeteran = SecureStorage.Instance.GetInt("userdata.campaigncompletevet", mCampaignMissionsCompletedVeteran);
		mPerkSlotsBought = SecureStorage.Instance.GetInt("userdata.perkslots", mPerkSlotsBought);
		mPerksBought = SecureStorage.Instance.GetInt("userdata.perks", mPerksBought);
		mPerkUpgradesBought = SecureStorage.Instance.GetInt("userdata.perkupgrades", mPerkUpgradesBought);
		mAssaultOwned = SecureStorage.Instance.GetInt("userdata.assault", mAssaultOwned);
		mStealthOwned = SecureStorage.Instance.GetInt("userdata.stealth", mStealthOwned);
		mHeavyOwned = SecureStorage.Instance.GetInt("userdata.heavy", mHeavyOwned);
		mBalancedOwned = SecureStorage.Instance.GetInt("userdata.balanced", mBalancedOwned);
		mCompleteOwned = SecureStorage.Instance.GetInt("userdata.complete", mCompleteOwned);
		mArmourUpgrades = SecureStorage.Instance.GetInt("userdata.armourupgrades", mArmourUpgrades);
		mGrenadesBought = SecureStorage.Instance.GetInt("userdata.grenades", mGrenadesBought);
		mClaymoresBought = SecureStorage.Instance.GetInt("userdata.claymores", mClaymoresBought);
		mMedPacksBought = SecureStorage.Instance.GetInt("userdata.medpacks", mMedPacksBought);
		mGrenadeCapacityBought = SecureStorage.Instance.GetInt("userdata.grenadecapacity", mGrenadeCapacityBought);
		mClaymoreCapacityBought = SecureStorage.Instance.GetInt("userdata.claymorecapacity", mClaymoreCapacityBought);
		mMedPackCapacityBought = SecureStorage.Instance.GetInt("userdata.medpackcapacity", mMedPackCapacityBought);
		mAmmoCachesUsed = SecureStorage.Instance.GetInt("userdata.ammocachesused", mAmmoCachesUsed);
		mMysteryCachesUsed = SecureStorage.Instance.GetInt("userdata.mysterycachesused", mMysteryCachesUsed);
		mTotalCurrencyPacksBought = SecureStorage.Instance.GetInt("userdata.currencypacks", mTotalCurrencyPacksBought);
	}

	public void Save()
	{
		SecureStorage.Instance.SetInt("userdata.challengesjoined", mChallengesJoined);
		SecureStorage.Instance.SetInt("userdata.challengescompleted", mChallengesCompleted);
		SecureStorage.Instance.SetInt("userdata.wavesstarted", mSpecOpsWavesStarted);
		SecureStorage.Instance.SetInt("userdata.wave10complete", mSpecOpsWave10Completed);
		SecureStorage.Instance.SetInt("userdata.highestwave", mHighestSpecOpsWave);
		SecureStorage.Instance.SetInt("userdata.maxdays", mMaxConsecutiveDays);
		SecureStorage.Instance.SetInt("userdata.missions", mMissionsUnlocked);
		SecureStorage.Instance.SetInt("userdata.campaignstartnorm", mCampaignMissionsStartedNormal);
		SecureStorage.Instance.SetInt("userdata.campaignstartvet", mCampaignMissionsStartedVeteran);
		SecureStorage.Instance.SetInt("userdata.campaigncompletenorm", mCampaignMissionsCompletedNormal);
		SecureStorage.Instance.SetInt("userdata.campaigncompletevet", mCampaignMissionsCompletedVeteran);
		SecureStorage.Instance.SetInt("userdata.perkslots", mPerkSlotsBought);
		SecureStorage.Instance.SetInt("userdata.perks", mPerksBought);
		SecureStorage.Instance.SetInt("userdata.perkupgrades", mPerkUpgradesBought);
		SecureStorage.Instance.SetInt("userdata.assault", mAssaultOwned);
		SecureStorage.Instance.SetInt("userdata.stealth", mStealthOwned);
		SecureStorage.Instance.SetInt("userdata.heavy", mHeavyOwned);
		SecureStorage.Instance.SetInt("userdata.balanced", mBalancedOwned);
		SecureStorage.Instance.SetInt("userdata.complete", mCompleteOwned);
		SecureStorage.Instance.SetInt("userdata.armourupgrades", mArmourUpgrades);
		SecureStorage.Instance.SetInt("userdata.grenades", mGrenadesBought);
		SecureStorage.Instance.SetInt("userdata.claymores", mClaymoresBought);
		SecureStorage.Instance.SetInt("userdata.medpacks", mMedPacksBought);
		SecureStorage.Instance.SetInt("userdata.grenadecapacity", mGrenadeCapacityBought);
		SecureStorage.Instance.SetInt("userdata.claymorecapacity", mClaymoreCapacityBought);
		SecureStorage.Instance.SetInt("userdata.medpackcapacity", mMedPackCapacityBought);
		SecureStorage.Instance.SetInt("userdata.ammocachesused", mAmmoCachesUsed);
		SecureStorage.Instance.SetInt("userdata.mysterycachesused", mMysteryCachesUsed);
		SecureStorage.Instance.SetInt("userdata.currencypacks", mTotalCurrencyPacksBought);
	}

	public void Reset()
	{
		mChallengesJoined = 0;
		mChallengesCompleted = 0;
		mSpecOpsWavesStarted = 0;
		mSpecOpsWave10Completed = 0;
		mHighestSpecOpsWave = 0;
		mMaxConsecutiveDays = 0;
		mMissionsUnlocked = 0;
		mCampaignMissionsStartedNormal = 0;
		mCampaignMissionsStartedVeteran = 0;
		mCampaignMissionsCompletedNormal = 0;
		mCampaignMissionsCompletedVeteran = 0;
		mPerkSlotsBought = 0;
		mPerksBought = 0;
		mPerkUpgradesBought = 0;
		mAssaultOwned = 0;
		mStealthOwned = 0;
		mHeavyOwned = 0;
		mBalancedOwned = 0;
		mCompleteOwned = 0;
		mArmourUpgrades = 0;
		mGrenadesBought = 0;
		mClaymoresBought = 0;
		mMedPacksBought = 0;
		mGrenadeCapacityBought = 0;
		mClaymoreCapacityBought = 0;
		mMedPackCapacityBought = 0;
		mAmmoCachesUsed = 0;
		mMysteryCachesUsed = 0;
		mTotalCurrencyPacksBought = 0;
	}

	private void Awake()
	{
		mInstance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		UpdateTimeStats();
	}

	private void OnEnable()
	{
		EventHub.Instance.OnStartMission += MissionStart;
		EventHub.Instance.OnEndMission += MissionEnd;
		EventHub.Instance.ChallengeJoined += ChallengeJoined;
		EventHub.Instance.OnSpecOpsWaveComplete += SpecOpsWaveComplete;
		EventHub.Instance.ChallengeCompleted += ChallengeAttemptCompleted;
		EventHub.Instance.OnAmmoCacheUsed += AmmoCacheUsed;
		EventHub.Instance.OnMissionUnlocked += MissionUnlocked;
		EventHub.Instance.OnMysteryCacheUsed += MysteryCacheUsed;
	}

	private void OnDisable()
	{
		EventHub.Instance.OnStartMission -= MissionStart;
		EventHub.Instance.OnEndMission -= MissionEnd;
		EventHub.Instance.ChallengeJoined -= ChallengeJoined;
		EventHub.Instance.OnSpecOpsWaveComplete -= SpecOpsWaveComplete;
		EventHub.Instance.ChallengeCompleted -= ChallengeAttemptCompleted;
		EventHub.Instance.OnAmmoCacheUsed -= AmmoCacheUsed;
		EventHub.Instance.OnMissionUnlocked -= MissionUnlocked;
		EventHub.Instance.OnMysteryCacheUsed -= MysteryCacheUsed;
	}

	public void MissionUnlocked(object sender, Events.MissionUnlocked args)
	{
		mMissionsUnlocked++;
	}

	public void AmmoCacheUsed(object sender, Events.AmmoCacheUsed args)
	{
		mAmmoCachesUsed++;
	}

	public void MysteryCacheUsed(object sender, Events.MysteryCacheUsed args)
	{
		mMysteryCachesUsed++;
	}

	public void MissionStart(object sender, Events.StartMission args)
	{
		if (ActStructure.Instance.MissionIsSpecOps(args.MissionId, args.Section))
		{
			mSpecOpsWavesStarted++;
		}
		else if (args.MissionDifficulty == DifficultyMode.Veteran)
		{
			mCampaignMissionsStartedVeteran++;
		}
		else
		{
			mCampaignMissionsStartedNormal++;
		}
	}

	public void MissionEnd(object sender, Events.EndMission args)
	{
		if (!ActStructure.Instance.MissionIsSpecOps(args.MissionId, args.Section) && args.Success)
		{
			if (args.Mode == DifficultyMode.Veteran)
			{
				mCampaignMissionsCompletedVeteran++;
			}
			else
			{
				mCampaignMissionsCompletedNormal++;
			}
		}
	}

	public void SpecOpsWaveComplete(object sender, Events.SpecOpsWaveComplete args)
	{
		if (args.WaveNum == 10)
		{
			mSpecOpsWave10Completed++;
		}
		if (args.WaveNum > mHighestSpecOpsWave)
		{
			mHighestSpecOpsWave = args.WaveNum;
		}
	}

	public void ChallengeJoined(object sender, Events.ChallengeJoined args)
	{
		mChallengesJoined++;
		Save();
	}

	public void ChallengeAttemptCompleted(object sender, Events.ChallengeCompleted args)
	{
		mChallengesCompleted++;
		Save();
	}

	private int ConsecutiveDaysMax()
	{
		mMaxConsecutiveDays = Mathf.Max(mMaxConsecutiveDays, SecureStorage.Instance.ConsecutiveDays);
		return mMaxConsecutiveDays;
	}

	public void UploadAllAttributes()
	{
		Debug.Log("Uploading User Attributes");
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["AppVersion"] = SwrvePayload.Version;
		dictionary["OsVersion"] = SwrvePayload.OSVersion;
		dictionary["Device"] = SwrvePayload.Device;
		dictionary["Platform"] = SwrvePayload.Platform;
		dictionary["IsCheater"] = SwrvePayload.IsCheater;
		dictionary["ConsecutiveDaysMax"] = ConsecutiveDaysMax().ToString();
		dictionary["ConsecutiveDaysCurrent"] = StatsHelper.ConsecutiveDaysPlayed().ToString();
		dictionary["FirstPlayTimeStampFormatted"] = SecureStorage.Instance.FirstPlayedDateTime;
		dictionary["FirstPlayTimeStampSeconds"] = SecureStorage.Instance.FirstGameDateTimeSeconds.ToString();
		dictionary["LastPlayedTimeStampFormatted"] = SecureStorage.Instance.LastPlayedDateTime;
		dictionary["LastPlayedTimeStampSeconds"] = SecureStorage.Instance.LastPlayedDateTimeSeconds.ToString();
		dictionary["TotalSessions"] = SecureStorage.Instance.NumGameSessions.ToString();
		dictionary["AverageSessionsPerDay"] = string.Format("{0:0.00}", AverageSessionsPerDay());
		dictionary["AverageMinutesPerDay"] = string.Format("{0:0.00}", AverageMinutesPerDay());
		dictionary["TotalDaysPlayed"] = string.Format("0:0.00", TotalDaysPlayed());
		dictionary["AverageMinutesPerDayOnDaysPlayed"] = string.Format("{0:0.00}", AverageMinutesPerDayOnDaysPlayed());
		dictionary["TotalMinutesPlayed"] = SecureStorage.Instance.TotalPlayingTime.ToString();
		dictionary["AverageSessionsPerWeek"] = string.Format("{0:0.00}", AverageSessionsPerWeek());
		dictionary["AverageDaysPlayedPerWeek"] = string.Format("{0:0.00}", AverageDaysPlayedPerWeek());
		dictionary["MissionsStarted"] = SwrvePayload.MissionsStarted;
		dictionary["CampaignMissionsStarted"] = (mCampaignMissionsStartedNormal + mCampaignMissionsStartedVeteran).ToString();
		dictionary["CampaignMissionsStartedNormal"] = mCampaignMissionsStartedNormal.ToString();
		dictionary["CampaignMissionsStartedVeteran"] = mCampaignMissionsStartedVeteran.ToString();
		dictionary["CampaignMissionsCompleted"] = (mCampaignMissionsCompletedNormal + mCampaignMissionsCompletedVeteran).ToString();
		dictionary["CampaignMissionsCompletedNormal"] = mCampaignMissionsCompletedNormal.ToString();
		dictionary["CampaignMissionsCompletedVeteran"] = mCampaignMissionsCompletedVeteran.ToString();
		dictionary["SpecOpsMissionsStarted"] = mSpecOpsWavesStarted.ToString();
		dictionary["SpecOpsMissionsCompletedWave10"] = mSpecOpsWave10Completed.ToString();
		dictionary["HighestSpecOpsWave"] = mHighestSpecOpsWave.ToString();
		dictionary["MissionsUnlocked"] = mMissionsUnlocked.ToString();
		dictionary["PlayerLevel"] = SwrvePayload.PlayerLevel;
		dictionary["PlayerXP"] = SwrvePayload.PlayerXP;
		dictionary["ChallengesJoined"] = ChallengesJoined.ToString();
		dictionary["ChallengesWon"] = mChallengesCompleted.ToString();
		dictionary["LifeTimeCurrency"] = SwrvePayload.TotalMTX + SwrvePayload.TotalCurrencyAwarded;
		dictionary["TotalCurrencySpent"] = SwrvePayload.TotalCurrencySpent;
		dictionary["TotalCurrencyBought"] = SwrvePayload.TotalMTX;
		dictionary["TotalCurrency"] = SwrvePayload.CurrentHardCurrencyTotal;
		dictionary["TotalCurrencyPacksBought"] = mTotalCurrencyPacksBought.ToString();
		dictionary["OwnsBundleStealth"] = ((mStealthOwned != 1) ? "False" : "True");
		dictionary["OwnsBundleAssault"] = ((mAssaultOwned != 1) ? "False" : "True");
		dictionary["OwnsBundleHeavy"] = ((mHeavyOwned != 1) ? "False" : "True");
		dictionary["OwnsBundleBalanced"] = ((mBalancedOwned != 1) ? "False" : "True");
		dictionary["OwnsBundleComplete"] = ((mCompleteOwned != 1) ? "False" : "True");
		dictionary["ArmourBought"] = mArmourUpgrades.ToString();
		dictionary["PerkSlotsBought"] = mPerkSlotsBought.ToString();
		dictionary["PerksBought"] = mPerksBought.ToString();
		dictionary["ProPerksBought"] = mPerkUpgradesBought.ToString();
		dictionary["GrenadeCapacityBought"] = mGrenadeCapacityBought.ToString();
		dictionary["ClaymoreCapacityBought"] = mClaymoreCapacityBought.ToString();
		dictionary["HealthKitCapacityBought"] = mMedPackCapacityBought.ToString();
		dictionary["GrenadesBought"] = mGrenadesBought.ToString();
		dictionary["ClaymoresBought"] = mClaymoresBought.ToString();
		dictionary["HealthKitsBought"] = mMedPacksBought.ToString();
		dictionary["AmmoCachesUsed"] = mAmmoCachesUsed.ToString();
		dictionary["MysteryCachesUsed"] = mMysteryCachesUsed.ToString();
		dictionary["ActivateLoggedIn"] = SwrvePayload.ActivateLoggedIn;
		dictionary["ActivateFriends"] = SwrvePayload.ActivateFriends;
		dictionary["EliteConnected"] = SwrvePayload.EliteConnected;
		dictionary["GyroscopeControlsOn"] = SwrvePayload.GyroscopeOnOff;
		dictionary["GestureControlsOn"] = SwrvePayload.GesturesOnOff;
		dictionary["AcceptedPushNotifications"] = SwrvePayload.AcceptedPushNotifications;
		dictionary["TotalPacksPurchased"] = SwrvePayload.TotalPreviousPurchases;
		for (int i = 0; i < PurchaseHandler.Instance.NumProducts(); i++)
		{
			dictionary["Pack" + i + "Purchased"] = SwrvePayload.PreviousPurchases(PurchaseHandler.Instance.ProductId(i));
		}
		dictionary["LastPackPurchased"] = SwrvePayload.LastPackPurchased;
		dictionary["LastPackPurchasedDateLocal"] = SwrvePayload.LastPackPurchasedDateLocal;
		dictionary["LastPackPurchasedDate"] = SwrvePayload.LastPackPurchasedDate;
		dictionary["ControllerType"] = SwrvePayload.ControllerType;
		Bedrock.brKeyValueArray parameters = BedrockUtils.Hash(dictionary);
		Bedrock.AnalyticsSetCustomUserInformation(parameters);
		Debug.Log("Uploading User Attributes DONE");
	}

	public void LogMTXPurchase()
	{
		mTotalCurrencyPacksBought++;
	}

	public void LogPurchase(PurchaseFlowHelper.PurchaseData data)
	{
		switch (data.Type)
		{
		case PurchaseFlowHelper.PurchaseData.PurchaseType.Perk:
			mPerksBought++;
			break;
		case PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment:
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.Grenade)
			{
				mGrenadesBought += data.NumItems;
			}
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.Claymore)
			{
				mClaymoresBought += data.NumItems;
			}
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.MediPack)
			{
				mMedPacksBought += data.NumItems;
			}
			break;
		case PurchaseFlowHelper.PurchaseData.PurchaseType.EquipmentSlot:
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.Grenade)
			{
				mGrenadeCapacityBought++;
			}
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.Claymore)
			{
				mClaymoreCapacityBought++;
			}
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.MediPack)
			{
				mMedPackCapacityBought++;
			}
			break;
		case PurchaseFlowHelper.PurchaseData.PurchaseType.PerkSlot:
			mPerkSlotsBought++;
			break;
		case PurchaseFlowHelper.PurchaseData.PurchaseType.ArmourUpgrade:
			mArmourUpgrades++;
			break;
		case PurchaseFlowHelper.PurchaseData.PurchaseType.PerkUpgrade:
			mPerkUpgradesBought++;
			break;
		case PurchaseFlowHelper.PurchaseData.PurchaseType.Bundles:
		{
			for (int i = 0; i < data.Bundles.Length; i++)
			{
				if (data.Bundles[i].ToString() == "AssaultBundle")
				{
					mAssaultOwned = 1;
				}
				else if (data.Bundles[i].ToString() == "StealthBundle")
				{
					mStealthOwned = 1;
				}
				else if (data.Bundles[i].ToString() == "HeavyBundle")
				{
					mHeavyOwned = 1;
				}
				else if (data.Bundles[i].ToString() == "BalancedBundle")
				{
					mBalancedOwned = 1;
				}
			}
			if (data.Bundles.Length == 4)
			{
				mCompleteOwned = 1;
			}
			break;
		}
		}
	}

	private void Update()
	{
		mRunningTime += Time.deltaTime;
		if (mRunningTime > 60f)
		{
			SecureStorage.Instance.TotalTimeToday += mRunningTime / 60f;
			mRunningTime = 0f;
		}
	}

	private void OnApplicationPause(bool isPaused)
	{
		if (!isPaused)
		{
			UpdateTimeStats();
		}
	}

	private void UpdateTimeStats()
	{
		SecureStorage.Instance.NumGameSessions++;
		if (SecureStorage.Instance.NumDaysPlayed == 0 || DateTime.Now.DayOfYear != SecureStorage.Instance.DayOfYear)
		{
			SecureStorage.Instance.TotalPlayingTime += SecureStorage.Instance.TotalTimeToday;
			SecureStorage.Instance.NumDaysPlayed++;
			SecureStorage.Instance.TotalTimeToday = 0f;
			SecureStorage.Instance.DayOfYear = DateTime.Now.DayOfYear;
		}
	}

	private void ShowTimeStats()
	{
	}

	private float AverageSessionsPerDay()
	{
		return (float)SecureStorage.Instance.NumGameSessions / (float)SecureStorage.Instance.NumDaysPlayed;
	}

	private float AverageMinutesPerDay()
	{
		return (SecureStorage.Instance.TotalPlayingTime + SecureStorage.Instance.TotalTimeToday) / (float)DaysSinceFirstPlay();
	}

	private float TotalDaysPlayed()
	{
		float num = 1440f;
		return SecureStorage.Instance.TotalPlayingTime / num;
	}

	private float AverageMinutesPerDayOnDaysPlayed()
	{
		return (SecureStorage.Instance.TotalPlayingTime + SecureStorage.Instance.TotalTimeToday) / (float)SecureStorage.Instance.NumDaysPlayed;
	}

	private int WeeksSinceFirstPlay()
	{
		int num = (int)TimeUtils.GetSecondsSinceUnixEpoch() - SecureStorage.Instance.FirstGameDateTimeSeconds;
		int num2 = 604800;
		int num3 = num / num2;
		if (num3 <= 0)
		{
			num3 = 1;
		}
		Debug.Log("Weeks since first play " + num3);
		return num3;
	}

	private int DaysSinceFirstPlay()
	{
		int num = (int)TimeUtils.GetSecondsSinceUnixEpoch() - SecureStorage.Instance.FirstGameDateTimeSeconds;
		int num2 = 86400;
		int num3 = num / num2;
		if (num3 <= 0)
		{
			num3 = 1;
		}
		return num3;
	}

	private float AverageSessionsPerWeek()
	{
		return (float)SecureStorage.Instance.NumGameSessions / (float)WeeksSinceFirstPlay();
	}

	private float AverageDaysPlayedPerWeek()
	{
		return (float)SecureStorage.Instance.NumDaysPlayed / (float)WeeksSinceFirstPlay();
	}
}
