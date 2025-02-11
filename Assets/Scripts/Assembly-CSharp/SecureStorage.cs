using System;
using System.Collections.Generic;
using UnityEngine;

public class SecureStorage
{
	private const string LastPlayedDateTimeKey = "player.lastPlayedDateTime";

	private const string LastPlayedDateTimeSecondsKey = "player.lastPlayedDateTimeSeconds";

	private const string LastSaveDateTimeKey = "player.lastSaveDateTime";

	private const string FirstGameDateTimeKey = "player.firstPlayDateTime";

	private const string FirstGameDateTimeSecondsKey = "player.firstGameDateTimeSeconds";

	private const string GamesPlayedKey = "player.gamesPlayed";

	private const string GameBootsKey = "player.gameBoots";

	private const string EliteAccountLinkedKey = "player.eliteAccountLinked";

	private const string EliteAppRewardedKey = "player.eliteAppRewarded";

	private const string MigratedKey = "player.migrated";

	private const string IAPLastTransactionKey = "iap.lastTransaction";

	private const string FacebookAuthorizedKey = "player.facebooklinked";

	private const string LastActivateDialogKey = "activate.lastdialog";

	private const string MissionCompletedSinceActivateDialog = "activate.MissionCompletedSinceLastDialog";

	private const string ReceiptValidationFailedKey = "player.recieptvalidationfailed";

	private const string MetadataVersion = "0.1";

	private const string ChallengeScoreSubmissionQueueStateKey = "Challenges.ScoreSubmissionQueue";

	private static SecureStorage m_instance;

	public static string CloudMetaDataKey = "CloudMetadata";

	private List<ISaveLoad> m_SaveableItems = new List<ISaveLoad>();

	private Dictionary<string, bool> SanityChecks = new Dictionary<string, bool>();

	private bool DoSanityCheck;

	public static SecureStorage Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = new SecureStorage();
			}
			return m_instance;
		}
	}

	public bool CloudPushingDisabled { get; set; }

	public string FirstPlayedDateTime
	{
		get
		{
			return GetString("player.firstPlayDateTime", string.Empty);
		}
		set
		{
			SetString("player.firstPlayDateTime", value);
		}
	}

	public string LastPlayedDateTime
	{
		get
		{
			return GetString("player.lastPlayedDateTime", string.Empty);
		}
		set
		{
			SetString("player.lastPlayedDateTime", value);
		}
	}

	public int LastPlayedDateTimeSeconds
	{
		get
		{
			return GetInt("player.lastPlayedDateTimeSeconds");
		}
		set
		{
			SetInt("player.lastPlayedDateTimeSeconds", value);
		}
	}

	public string LastSaveDateTime
	{
		get
		{
			return GetString("player.lastSaveDateTime", "Unknown");
		}
		set
		{
			SetString("player.lastSaveDateTime", value);
		}
	}

	public int FirstGameDateTimeSeconds
	{
		get
		{
			return GetInt("player.firstGameDateTimeSeconds");
		}
	}

	public int NumberOfGameBoots
	{
		get
		{
			return GetInt("player.gameBoots");
		}
	}

	public DateTime LastActivateDialogShownTime
	{
		get
		{
			long? nullableLong = GetNullableLong("activate.lastdialog");
			if (nullableLong.HasValue)
			{
				return new DateTime(nullableLong.Value);
			}
			return DateTime.Now;
		}
	}

	public int MissionsCompletedSinceLastActivateDialog
	{
		get
		{
			return GetInt("activate.MissionCompletedSinceLastDialog", 1);
		}
	}

	public bool EliteAccountLinked
	{
		get
		{
			return GetBool("player.eliteAccountLinked");
		}
		set
		{
			SetBool("player.eliteAccountLinked", value);
		}
	}

	public bool EliteAppRewarded
	{
		get
		{
			return GetBool("player.eliteAppRewarded");
		}
		set
		{
			SetBool("player.eliteAppRewarded", value);
		}
	}

	public bool HasMigrated
	{
		get
		{
			return GetBool("player.migrated");
		}
		set
		{
			SetBool("player.migrated", value);
		}
	}

	public bool ReceiptValidationFailed
	{
		get
		{
			return GetBool("player.recieptvalidationfailed");
		}
		set
		{
			SetBool("player.recieptvalidationfailed", value);
		}
	}

	public bool FacebookAuthorized
	{
		get
		{
			return GetBool("player.facebooklinked");
		}
		set
		{
			SetBool("player.facebooklinked", value);
		}
	}

	public bool HasEverJoinedAChallenge
	{
		get
		{
			return GetBool("HasEverJoinedAChallenge");
		}
		set
		{
			SetBool("HasEverJoinedAChallenge", value);
		}
	}

	public string ChallengeScoreSubmissionQueueState
	{
		get
		{
			return GetString("Challenges.ScoreSubmissionQueue");
		}
		set
		{
			SetString("Challenges.ScoreSubmissionQueue", value);
		}
	}

	public bool HasViewedReviveTutorial
	{
		get
		{
			return GetBool("HasViewedReviveTutorial");
		}
		set
		{
			SetBool("HasViewedReviveTutorial", value);
		}
	}

	public bool UsedClaymoreSinceLastPurchase
	{
		get
		{
			return GetBool("ClaymoreSinceLastPurchase");
		}
		set
		{
			SetBool("ClaymoreSinceLastPurchase", value);
		}
	}

	public bool UsedGrenadeSinceLastPurchase
	{
		get
		{
			return GetBool("GrenadeSinceLastPurchase");
		}
		set
		{
			SetBool("GrenadeSinceLastPurchase", value);
		}
	}

	public bool UsedHealthKitSinceLastPurchase
	{
		get
		{
			return GetBool("HealthKitSinceLastPurchase");
		}
		set
		{
			SetBool("HealthKitSinceLastPurchase", value);
		}
	}

	public float SoundFXVolume
	{
		get
		{
			float val = 1f;
			GetFloat("SoundFXVolume", ref val);
			return val;
		}
		set
		{
			SetFloat("SoundFXVolume", value);
		}
	}

	private string TimesInGlobeScreenKey
	{
		get
		{
			return "TimesInGlobeScreen_" + TBFUtils.BundleVersion;
		}
	}

	public int TimesInGlobeScreen
	{
		get
		{
			return GetInt(TimesInGlobeScreenKey);
		}
		set
		{
			SetInt(TimesInGlobeScreenKey, value);
		}
	}

	public float MusicVolume
	{
		get
		{
			float val = 1f;
			GetFloat("MusicVolume", ref val);
			return val;
		}
		set
		{
			SetFloat("MusicVolume", value);
		}
	}

	public bool HasSeenGrenadeTutorial
	{
		get
		{
			return GetBool("HasSeenGrenadeTutorial");
		}
		set
		{
			SetBool("HasSeenGrenadeTutorial", value);
		}
	}

	public bool HasSeenClaymoreTutorial
	{
		get
		{
			return GetBool("HasSeenClaymoreTutorial");
		}
		set
		{
			SetBool("HasSeenClaymoreTutorial", value);
		}
	}

	public bool HasSeenFlashpointTutorial
	{
		get
		{
			return GetBool("HasSeenFlashpointTutorial");
		}
		set
		{
			SetBool("HasSeenFlashpointTutorial", value);
		}
	}

	public bool HasSeenFlashpointLeaguesTutorial
	{
		get
		{
			return GetBool("HasSeenFlashpointLeaguesTutorial");
		}
		set
		{
			SetBool("HasSeenFlashpointLeaguesTutorial", value);
		}
	}

	public bool HasSeenFlashpointTrackerTutorial
	{
		get
		{
			return GetBool("HasSeenFlashpointTrackerTutorial");
		}
		set
		{
			SetBool("HasSeenFlashpointTrackerTutorial", value);
		}
	}

	public bool HasSeenDropBodyTutorial
	{
		get
		{
			return GetBool("HasSeenDropBodyTutorial");
		}
		set
		{
			SetBool("HasSeenDropBodyTutorial", value);
		}
	}

	public int NumGameSessions
	{
		get
		{
			return GetInt("NumGameSessions");
		}
		set
		{
			SetInt("NumGameSessions", value);
		}
	}

	public float TotalTimeToday
	{
		get
		{
			return GetFloat("TotalTimeToday");
		}
		set
		{
			SetFloat("TotalTimeToday", value);
		}
	}

	public int DayOfYear
	{
		get
		{
			return GetInt("DayOfYear");
		}
		set
		{
			SetInt("DayOfYear", value);
		}
	}

	public int NumDaysPlayed
	{
		get
		{
			return GetInt("NumDaysPlayed");
		}
		set
		{
			SetInt("NumDaysPlayed", value);
		}
	}

	public float TotalPlayingTime
	{
		get
		{
			return GetFloat("TotalPlayingTime");
		}
		set
		{
			SetFloat("TotalPlayingTime", value);
		}
	}

	public int LastPlayed
	{
		get
		{
			return GetInt("LastPlayed");
		}
		set
		{
			SetInt("LastPlayed", value);
		}
	}

	public int ConsecutiveDays
	{
		get
		{
			return GetInt("ConsecutiveDays");
		}
		set
		{
			SetInt("ConsecutiveDays", value);
		}
	}

	public bool NeedsDailyReward
	{
		get
		{
			return GetBool("NeedsDailyReward");
		}
		set
		{
			SetBool("NeedsDailyReward", value);
		}
	}

	public bool ControllerHasBeenConnected
	{
		get
		{
			return GetBool("ControllerHasBeenConnected");
		}
		set
		{
			SetBool("ControllerHasBeenConnected", value);
		}
	}

	public static event EventHandler<EventArgs> OnLoad;

	private SecureStorage()
	{
	}

	public bool HasImportantSaveData()
	{
		if (StatsHelper.PlayerXP() > 0)
		{
			return true;
		}
		if (GameSettings.Instance.PlayerCash().TotalHardCashEverPurchased() > 0 || GameSettings.Instance.PlayerCash().HardCash() > 0)
		{
			return true;
		}
		return false;
	}

	public void RegisterSaveableItem(ISaveLoad item)
	{
		m_SaveableItems.Add(item);
	}

	public void ResetAllData()
	{
		foreach (ISaveLoad saveableItem in m_SaveableItems)
		{
			saveableItem.Reset();
		}
	}

	public void SaveGlobalUnrest()
	{
		GlobalUnrestController.Instance.Save();
		Bedrock.Flush();
		SaveCurrentDateAndTime();
		PushToCloud();
	}

	public void SaveGameSettings()
	{
		GameSettings.Instance.Save();
		Bedrock.Flush();
		SaveCurrentDateAndTime();
		PushToCloud();
	}

	public void SaveActStructure()
	{
		ActStructure.Instance.Save();
		Bedrock.Flush();
		SaveCurrentDateAndTime();
		PushToCloud();
	}

	public void SaveAchievements()
	{
		StatsManager.Instance.AchievementManager().Save();
		Bedrock.Flush();
		SaveCurrentDateAndTime();
		PushToCloud();
	}

	public void SavePlayerStats()
	{
		StatsManager.Instance.PlayerStats().Save();
		Bedrock.Flush();
		SaveCurrentDateAndTime();
		PushToCloud();
	}

	public void SaveAllData()
	{
		foreach (ISaveLoad saveableItem in m_SaveableItems)
		{
			saveableItem.Save();
		}
		Bedrock.Flush();
		SaveCurrentDateAndTime();
		PushToCloud();
	}

	private void SaveCurrentDateAndTime()
	{
		LastSaveDateTime = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();
	}

	public void LoadAllData()
	{
		foreach (ISaveLoad saveableItem in m_SaveableItems)
		{
			saveableItem.Load();
		}
		StatsManager.Instance.SyncLeaderboards();
		if (SecureStorage.OnLoad != null)
		{
			SecureStorage.OnLoad(this, new EventArgs());
		}
	}

	public void SetSanityCheck(bool on)
	{
		DoSanityCheck = on;
		if (on)
		{
			SanityChecks.Clear();
		}
	}

	private void SanityCheck(string Key)
	{
		if (DoSanityCheck && SanityChecks.ContainsKey(Key))
		{
			Debug.LogError("Duplicate key found: " + Key);
		}
		SanityChecks[Key] = true;
	}

	public void SetInt(string Key, int Value)
	{
		SanityCheck(Key);
		Bedrock.SetUserVariableAsInt(Key, Value);
	}

	public int GetInt(string Key)
	{
		return Bedrock.GetUserVariableAsInt(Key, 0);
	}

	public int GetInt(string Key, int defaultValue)
	{
		return Bedrock.GetUserVariableAsInt(Key, defaultValue);
	}

	public void SetBool(string Key, bool Value)
	{
		SanityCheck(Key);
		Bedrock.SetUserVariableAsBool(Key, Value);
	}

	public bool GetBool(string Key)
	{
		return Bedrock.GetUserVariableAsBool(Key, false);
	}

	public void GetBool(string Key, ref bool val)
	{
		if (Bedrock.UserVariableExists(Key))
		{
			val = Bedrock.GetUserVariableAsBool(Key, val);
		}
	}

	public void SetFloat(string Key, float Value)
	{
		SanityCheck(Key);
		Bedrock.SetUserVariableAsFloat(Key, Value);
	}

	public float GetFloat(string Key)
	{
		return Bedrock.GetUserVariableAsFloat(Key, 0f);
	}

	public void GetFloat(string Key, ref float val)
	{
		if (Bedrock.UserVariableExists(Key))
		{
			val = Bedrock.GetUserVariableAsFloat(Key, val);
		}
	}

	public void SetString(string Key, string Value)
	{
		SanityCheck(Key);
		Bedrock.SetUserVariableAsString(Key, Value);
	}

	public string GetString(string Key)
	{
		return Bedrock.GetUserVariableAsString(Key, string.Empty);
	}

	public string GetString(string Key, string defaultValue)
	{
		return Bedrock.GetUserVariableAsString(Key, defaultValue);
	}

	public void SetArrayOfStrings(string Key, string[] Value)
	{
		for (int i = 0; i < Value.Length; i++)
		{
			Bedrock.SetUserVariableAsString(Key + i, Value[i]);
		}
	}

	public void GetArrayOfStrings(string Key, ref string[] Value)
	{
		for (int i = 0; i < Value.Length; i++)
		{
			Value[i] = Bedrock.GetUserVariableAsString(Key + i, string.Empty);
		}
	}

	public bool HasKey(string Key)
	{
		return Bedrock.UserVariableExists(Key);
	}

	private uint? GetNullableUint(string key)
	{
		string @string = GetString(key);
		uint result;
		if (!uint.TryParse(@string, out result))
		{
			return null;
		}
		return result;
	}

	private void SetNullableUint(string key, uint? value)
	{
		string value2 = ((!value.HasValue) ? null : value.Value.ToString());
		SetString(key, value2);
	}

	private long? GetNullableLong(string key)
	{
		string @string = GetString(key);
		long result;
		if (!long.TryParse(@string, out result))
		{
			return null;
		}
		return result;
	}

	private void SetNullableLong(string key, long? value)
	{
		string value2 = ((!value.HasValue) ? null : value.Value.ToString());
		SetString(key, value2);
	}

	public bool LogFirstGameDateTime()
	{
		bool result = false;
		if (FirstPlayedDateTime == null || FirstPlayedDateTime == string.Empty)
		{
			FirstPlayedDateTime = DateTime.UtcNow.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();
			SetInt("player.firstGameDateTimeSeconds", (int)TimeUtils.GetSecondsSinceUnixEpoch());
			result = true;
		}
		return result;
	}

	public void LogLastPlayedDateTime()
	{
		string lastPlayedDateTime = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString();
		LastPlayedDateTime = lastPlayedDateTime;
		LastPlayedDateTimeSeconds = (int)TimeUtils.GetSecondsSinceUnixEpoch();
	}

	public void LogGameBoot()
	{
		SetInt("player.gameBoots", NumberOfGameBoots + 1);
	}

	public void LogActivateDialogShown()
	{
		SetNullableLong("activate.lastdialog", DateTime.Now.Ticks);
	}

	public void LogMissionCompletedSinceActivateDialog()
	{
		SetInt("activate.MissionCompletedSinceLastDialog", MissionsCompletedSinceLastActivateDialog + 1);
	}

	public void ResetActivateDialogMissionCounter()
	{
		SetInt("activate.MissionCompletedSinceLastDialog", 0);
	}

	public void PushToCloud()
	{
		if (!CloudPushingDisabled)
		{
			if (Application.platform != RuntimePlatform.WindowsEditor)
			{
				SetString(CloudMetaDataKey, EncodeMetadata());
				if (!Bedrock.getUserConnectionStatus().IsOnline() || !Bedrock.ResolveUserCacheVariablesWithCloud(true))
				{
					ActivateWatcher.Instance.QueueCloudSave();
					Debug.Log("!! Could not push to cloud !!");
				}
				else
				{
					Debug.Log("!! Push to cloud returned success !!");
					ActivateWatcher.Instance.CancelQueuedCloudSaveOnSuccess();
				}
			}
		}
		else if (CloudPushingDisabled)
		{
			SetString(CloudMetaDataKey, EncodeMetadata());
		}
	}

	private string EncodeMetadata()
	{
		SecureStorageMetadata metadata = default(SecureStorageMetadata);
		bool flag = DecodeMetadata(GetString(CloudMetaDataKey), ref metadata);
		string text = UnityEngine.Random.Range(int.MinValue, int.MaxValue).ToString();
		if (flag && Bedrock.getUserConnectionStatus() == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE)
		{
			text = metadata.Tag;
		}
		string text2 = string.Format("{0},{1},{2},{3},{4}", "0.1", text, StatsHelper.PlayerXP(), GameSettings.Instance.PlayerCash().HardCash(), LastSaveDateTime);
		Debug.Log("EncodedMetadata: " + text2);
		return text2;
	}

	public bool DecodeMetadata(string dataToDecode, ref SecureStorageMetadata metadata)
	{
		Debug.Log("DATA TO DECODE: '" + dataToDecode + "'");
		if (string.IsNullOrEmpty(dataToDecode))
		{
			return false;
		}
		string[] array = dataToDecode.Split(',');
		uint num = 0u;
		string[] array2 = array;
		foreach (string text in array2)
		{
			switch (num)
			{
			case 0u:
				metadata.Version = text;
				break;
			case 1u:
				metadata.Tag = text;
				break;
			case 2u:
				metadata.TotalXP = int.Parse(text);
				break;
			case 3u:
				metadata.HardCurrencyPurchased = int.Parse(text);
				break;
			case 4u:
				metadata.LastPlayedDateTime = text;
				break;
			}
			num++;
		}
		if (metadata.Version == "0.1")
		{
			return true;
		}
		return false;
	}

	public void SetLastIAPTransaction(string name)
	{
		SetString("iap.lastTransaction", name);
	}

	public string GetLastIAPTransaction()
	{
		return GetString("iap.lastTransaction");
	}

	private string BuildChallengeKey(ChallengeData challenge, string identifier)
	{
		return string.Format("Challenge.{0}.{1}", challenge.Id, identifier);
	}

	public uint? GetLastJoinTime(ChallengeData challenge)
	{
		return GetNullableUint(BuildChallengeKey(challenge, "JoinTime"));
	}

	public void SetLastJoinTime(ChallengeData challenge, uint? utcTime)
	{
		SetNullableUint(BuildChallengeKey(challenge, "JoinTime"), utcTime);
	}

	public uint? GetBestScoreSubmittedThisCycleTime(ChallengeData challenge)
	{
		return GetNullableUint(BuildChallengeKey(challenge, "BestScoreSubmittedThisCycleTime"));
	}

	public void SetBestScoreSubmittedThisCycleTime(ChallengeData challenge, uint? value)
	{
		SetNullableUint(BuildChallengeKey(challenge, "BestScoreSubmittedThisCycleTime"), value);
	}

	public void ResetBestScoreSubmittedThisCycle(ChallengeData challengeData)
	{
		SetBestScoreSubmittedThisCycle(challengeData, null);
		SetBestScoreSubmittedThisCycleTime(challengeData, null);
	}

	public long? GetBestScoreSubmittedThisCycle(ChallengeData challenge)
	{
		return GetNullableLong(BuildChallengeKey(challenge, "BestScoreSubmittedThisCycle"));
	}

	public void SetBestScoreSubmittedThisCycle(ChallengeData challenge, long? score)
	{
		SetNullableLong(BuildChallengeKey(challenge, "BestScoreSubmittedThisCycle"), score);
	}

	public long? GetCurrentScore(ChallengeData challenge)
	{
		return GetNullableLong(BuildChallengeKey(challenge, "CurrentScore"));
	}

	public void SetCurrentScore(ChallengeData challenge, long? score)
	{
		SetNullableLong(BuildChallengeKey(challenge, "CurrentScore"), score);
	}

	public uint? GetCurrentLifeRemaining(ChallengeData challengeData)
	{
		return GetNullableUint(BuildChallengeKey(challengeData, "CurrentLifeRemaining"));
	}

	public void SetCurrentLifeRemaining(ChallengeData challengeData, uint? lifeRemaining)
	{
		SetNullableUint(BuildChallengeKey(challengeData, "CurrentLifeRemaining"), lifeRemaining);
	}

	public bool GetHasPickedUpRewardSinceLastJoin(ChallengeData challengeData)
	{
		return GetBool(BuildChallengeKey(challengeData, "HasPickedUpRewardSinceLastJoin"));
	}

	public void SetHasPickedUpRewardSinceLastJoin(ChallengeData challengeData, bool value)
	{
		SetBool(BuildChallengeKey(challengeData, "HasPickedUpRewardSinceLastJoin"), value);
	}

	public bool GetHasBeenStoppedSinceLastJoin(ChallengeData challengeData)
	{
		return GetBool(BuildChallengeKey(challengeData, "HasBeenStoppedSinceLastJoin"));
	}

	public void SetHasBeenStoppedSinceLastJoin(ChallengeData challengeData, bool value)
	{
		SetBool(BuildChallengeKey(challengeData, "HasBeenStoppedSinceLastJoin"), value);
	}

	public uint? GetLastChallengeNotificationTime(ChallengeData challengeData)
	{
		return GetNullableUint(BuildChallengeKey(challengeData, "LastNotificationScheduleTime"));
	}

	public void SetLastChallengeNotificationTime(ChallengeData challengeData, uint? notificationTime)
	{
		SetNullableUint(BuildChallengeKey(challengeData, "LastNotificationScheduleTime"), notificationTime);
	}

	public bool GetHighScoreShownSinceJoin(ChallengeData challengeData)
	{
		return GetBool(BuildChallengeKey(challengeData, "HighScoreShownSinceJoin"));
	}

	public void SetHighScoreShownSinceJoin(ChallengeData challengeData, bool value)
	{
		SetBool(BuildChallengeKey(challengeData, "HighScoreShownSinceJoin"), value);
	}
}
