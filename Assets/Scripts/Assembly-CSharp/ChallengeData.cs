using System;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeData : MonoBehaviour
{
	public class StatusChangedEventArgs : ChallengeEventArgs
	{
		public ChallengeStatus OldStatus { get; private set; }

		public StatusChangedEventArgs(ChallengeData challenge, ChallengeStatus oldStatus)
			: base(challenge)
		{
			OldStatus = oldStatus;
		}
	}

	public enum ScoreComparisonResult
	{
		FirstIsBetter = 0,
		SecondIsBetter = 1,
		Equal = 2
	}

	private const float PlaceholderBedrockWaitTime = 3f;

	private const string StartDateSwrveKey = "startdate";

	private const string ActiveDurationSwrveKey = "activeduration";

	private const string InactiveDurationSwrveKey = "inactiveduration";

	private const string InvalidDurationSwrveKey = "invalidduration";

	private const string NumResetsSwrveKey = "numresets";

	private const string LeaderboardIdSwrveKey = "leaderboardid";

	private const string ChallengeNameSwrveKey = "ChallengeName";

	private const string DescriptionSwrveKey = "ChallengeDescription";

	private const string ScriptTypeSwrveKey = "ScriptType";

	private const string ScriptParametersSwrveKey = "ScriptParameters";

	private const string LifetimeSwrveKey = "Lifetime";

	private const string BronzeGoalSwrveKey = "BronzeGoal";

	private const string SilverGoalSwrveKey = "SilverGoal";

	private const string GoldGoalSwrveKey = "GoldGoal";

	private const string LeaderboardTypeSwrveKey = "LeaderboardType";

	private const string FacebookUrlSwrveKey = "FacebookURL";

	private const string GoalNounSwrveKey = "GoalNoun";

	private const string IsTimeSwrveKey = "IsTime";

	private const string BronzeRewardSwrveKey = "BronzeReward";

	private const string SilverRewardSwrveKey = "SilverReward";

	private const string GoldRewardSwrveKey = "GoldReward";

	private const string FriendsBeatenRewardSwrveKey = "FriendsBeatenReward";

	private static readonly ILogger _classLogger = LogBuilder.Instance.GetLogger(typeof(ChallengeData), LogLevel.Warning);

	private ulong mFriendsBeaten;

	private ChallengeStatus _status;

	private bool _waitingForStatusUpdate;

	private ILogger _instanceLogger;

	private uint RewardToCollect;

	public uint Id { get; private set; }

	public uint StartDate { get; private set; }

	public uint ActiveDuration { get; private set; }

	public uint InactiveDuration { get; private set; }

	public uint InvalidDuration { get; private set; }

	public uint NumResets { get; private set; }

	public uint LeaderboardId { get; private set; }

	public ChallengeLeaderboardType LeaderboardType { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public string ScriptType { get; private set; }

	public string ScriptParameters { get; private set; }

	public uint Lifetime { get; set; }

	public long BronzeGoal { get; private set; }

	public long SilverGoal { get; private set; }

	public long GoldGoal { get; private set; }

	public string FacebookUrl { get; private set; }

	public string GoalNoun { get; private set; }

	public bool IsTime { get; private set; }

	public uint BronzeReward { get; private set; }

	public uint SilverReward { get; private set; }

	public uint GoldReward { get; private set; }

	public uint FriendsBeatenReward { get; private set; }

	public ChallengeStatus Status
	{
		get
		{
			return _status;
		}
		set
		{
			if (_status != value)
			{
				ChallengeStatus status = _status;
				_status = value;
				OnStatusChanged(status);
			}
		}
	}

	public string LocalizedName
	{
		get
		{
			return Language.Get(Name);
		}
	}

	public string LocalizedDescription
	{
		get
		{
			return Language.Get(Description);
		}
	}

	public string LocalizedGoalNoun
	{
		get
		{
			return Language.Get(GoalNoun);
		}
	}

	protected ILogger InstanceLog
	{
		get
		{
			if (_instanceLogger == null)
			{
				_instanceLogger = LogBuilder.Instance.GetCustomLogger("ChallengeData, " + Id, LogLevel.None);
			}
			return _instanceLogger;
		}
	}

	public Challenge ChallengeInstance
	{
		get
		{
			return ChallengeManager.Instance.ChallengesUnderway.GetChallengeInstance(Id);
		}
		set
		{
			if (value == null)
			{
				ChallengeManager.Instance.ChallengesUnderway.RemoveChallengeInstance(Id);
			}
			else
			{
				ChallengeManager.Instance.ChallengesUnderway.SetChallengeInstance(Id, value);
			}
		}
	}

	public bool IsJoined
	{
		get
		{
			return ChallengeInstance != null;
		}
	}

	public ChallengeLeaderboardWriteTypes WriteType
	{
		get
		{
			return (LeaderboardType != 0) ? ChallengeLeaderboardWriteTypes.UseMin : ChallengeLeaderboardWriteTypes.UseMax;
		}
	}

	public long? BestScoreSubmittedThisCycle
	{
		get
		{
			return SecureStorage.Instance.GetBestScoreSubmittedThisCycle(this);
		}
	}

	public uint? LastJoinTime
	{
		get
		{
			return SecureStorage.Instance.GetLastJoinTime(this);
		}
		private set
		{
			SecureStorage.Instance.SetLastJoinTime(this, value);
		}
	}

	public bool HighScoreShownSinceJoin
	{
		get
		{
			return SecureStorage.Instance.GetHighScoreShownSinceJoin(this);
		}
		set
		{
			SecureStorage.Instance.SetHighScoreShownSinceJoin(this, value);
		}
	}

	public bool HasPickedUpRewardSinceLastJoin
	{
		get
		{
			return SecureStorage.Instance.GetHasPickedUpRewardSinceLastJoin(this);
		}
		private set
		{
			SecureStorage.Instance.SetHasPickedUpRewardSinceLastJoin(this, value);
		}
	}

	public bool HasBeenStoppedSinceLastJoin
	{
		get
		{
			return SecureStorage.Instance.GetHasBeenStoppedSinceLastJoin(this);
		}
		set
		{
			SecureStorage.Instance.SetHasBeenStoppedSinceLastJoin(this, value);
		}
	}

	public static event EventHandler<StatusChangedEventArgs> StatusChanged;

	public static event EventHandler RewardCollected;

	public static ChallengeData BuildFromCsvLine(List<string> csvLine)
	{
		GameObject gameObject = new GameObject("Challenge");
		int num = 0;
		ChallengeData challengeData = gameObject.AddComponent<ChallengeData>();
		_classLogger.LogDebug("Loading challenge from: " + string.Join(", ", csvLine.ToArray()));
		num += 4;
		challengeData.Id = uint.Parse(csvLine[num++]);
		challengeData.LeaderboardId = uint.Parse(csvLine[num++]);
		challengeData.StartDate = uint.Parse(csvLine[num++]);
		challengeData.ActiveDuration = uint.Parse(csvLine[num++]);
		challengeData.InactiveDuration = uint.Parse(csvLine[num++]);
		challengeData.InvalidDuration = uint.Parse(csvLine[num++]);
		challengeData.NumResets = uint.Parse(csvLine[num++]);
		challengeData.Name = csvLine[num++];
		challengeData.Description = csvLine[num++];
		challengeData.ScriptType = csvLine[num++];
		challengeData.ScriptParameters = csvLine[num++];
		challengeData.Lifetime = uint.Parse(csvLine[num++]);
		challengeData.BronzeGoal = int.Parse(csvLine[num++]);
		challengeData.SilverGoal = int.Parse(csvLine[num++]);
		challengeData.GoldGoal = int.Parse(csvLine[num++]);
		challengeData.LeaderboardType = (ChallengeLeaderboardType)(int)Enum.Parse(typeof(ChallengeLeaderboardType), csvLine[num++]);
		challengeData.FacebookUrl = csvLine[num++];
		challengeData.GoalNoun = csvLine[num++];
		challengeData.IsTime = bool.Parse(csvLine[num++]);
		challengeData.BronzeReward = uint.Parse(csvLine[num++]);
		challengeData.SilverReward = uint.Parse(csvLine[num++]);
		challengeData.GoldReward = uint.Parse(csvLine[num++]);
		challengeData.FriendsBeatenReward = uint.Parse(csvLine[num++]);
		challengeData.Status = ChallengeStatus.Unknown;
		return challengeData;
	}

	public static ChallengeData BuildFromBedrockInfo(Bedrock.brChallengeInfo challengeInfo)
	{
		GameObject gameObject = new GameObject("Challenge");
		_classLogger.Log("Generating challenge Info from bedrock challenge data.");
		if (_classLogger.OutputLevel == LogLevel.Debug)
		{
			challengeInfo.DebugPrint();
		}
		ChallengeData challengeData = gameObject.AddComponent<ChallengeData>();
		challengeData.Id = challengeInfo._challengeId;
		challengeData.LeaderboardId = challengeInfo._leaderboardId;
		challengeData.StartDate = challengeInfo._startDate;
		challengeData.ActiveDuration = challengeInfo._activeDuration;
		challengeData.InactiveDuration = challengeInfo._inactiveDuration;
		challengeData.InvalidDuration = challengeInfo._invalidDuration;
		challengeData.NumResets = challengeInfo._numResets;
		string swrveDataKey = GetSwrveDataKey(challengeInfo._challengeId);
		Dictionary<string, string> resourceDictionary;
		if (!Bedrock.GetRemoteUserResources(swrveDataKey, out resourceDictionary))
		{
			_classLogger.LogError("Unable to get SWRVE resource '{0}'.", swrveDataKey);
			return null;
		}
		challengeData.Name = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ChallengeName", null);
		challengeData.Description = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ChallengeDescription", null);
		challengeData.ScriptType = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ScriptType", null);
		challengeData.ScriptParameters = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "ScriptParameters", null);
		challengeData.Lifetime = (uint)Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "Lifetime", 1);
		challengeData.BronzeGoal = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "BronzeGoal", 0);
		challengeData.SilverGoal = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "SilverGoal", 0);
		challengeData.GoldGoal = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "GoldGoal", 0);
		string fromResourceDictionaryAsString = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "LeaderboardType", null);
		challengeData.FacebookUrl = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "FacebookURL", null);
		challengeData.GoalNoun = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "GoalNoun", "things");
		challengeData.IsTime = Bedrock.GetFromResourceDictionaryAsBool(resourceDictionary, "IsTime", false);
		challengeData.BronzeReward = (uint)Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "BronzeReward", 0);
		challengeData.SilverReward = (uint)Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "SilverReward", 0);
		challengeData.GoldReward = (uint)Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "GoldReward", 0);
		challengeData.FriendsBeatenReward = (uint)Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FriendsBeatenReward", 0);
		if (string.IsNullOrEmpty(challengeData.Name) || string.IsNullOrEmpty(challengeData.Description) || string.IsNullOrEmpty(challengeData.ScriptType))
		{
			_classLogger.LogError("ChallengeInfo {0} was missing some required data. Could not construct valid ChallengeData.", challengeInfo._challengeId);
			return null;
		}
		ChallengeLeaderboardType value;
		if (!EnumUtils.TryParse<ChallengeLeaderboardType>(fromResourceDictionaryAsString, out value))
		{
			_classLogger.LogError("Unable to parse leaderboard type for challenge '{0}'.", challengeInfo._challengeId);
			return null;
		}
		challengeData.LeaderboardType = value;
		return challengeData;
	}

	private static string GetSwrveDataKey(uint challengeId)
	{
		return "challenge" + challengeId;
	}

	public void CollectReward(uint time, long score, ulong friendsBeaten)
	{
		ChallengeMedalType medalForScore = GetMedalForScore(score);
		uint rewardForMedal = GetRewardForMedal(medalForScore);
		uint rewardForFriendsBeaten = GetRewardForFriendsBeaten(friendsBeaten);
		mFriendsBeaten = friendsBeaten;
		CollectReward(time, rewardForMedal + rewardForFriendsBeaten);
	}

	public void CollectReward(uint time, uint reward)
	{
		if (HasPickedUpRewardSinceLastJoin)
		{
			InstanceLog.LogError("Trying to collect reward a second time in same cycle.");
			return;
		}
		RewardToCollect = reward;
		MessageBoxController.Instance.DoCollectChallengeRewardDialogue(RewardToCollect, Language.Get(Name), GetMedal(), this, "HaveCollectedReward");
	}

	private void HaveCollectedReward()
	{
		Debug.Log("Collecting challenge reward: " + RewardToCollect);
		SwrveEventsMetaGame.ChallengeClaimed(Id, mFriendsBeaten, GetMedal());
		GameSettings.Instance.PlayerCash().AwardHardCash((int)RewardToCollect, "ChallengeReward");
		HasPickedUpRewardSinceLastJoin = true;
		OnRewardCollected();
	}

	private ChallengeMedalType GetMedal()
	{
		ChallengeMedalType result = ChallengeMedalType.None;
		if (BestScoreSubmittedThisCycle.HasValue && BestScoreSubmittedThisCycle.HasValue)
		{
			result = GetMedalForScore(BestScoreSubmittedThisCycle.Value);
		}
		return result;
	}

	public void Join(uint joinTime)
	{
		SecureStorage.Instance.HasEverJoinedAChallenge = true;
		InstanceLog.Log("Joining challenge {0} at time {1}", Id, joinTime);
		if (ChallengeInstance != null)
		{
			InstanceLog.LogError("Joining challenge, but there is already a challenge instance associated with it... Skipping.");
			return;
		}
		Challenge challenge = CreateChallengeInstance(joinTime, false);
		if (challenge == null)
		{
			InstanceLog.LogError("Unable to join challenge, tried but failed.");
			return;
		}
		uint? bestScoreSubmittedThisCycleTime = SecureStorage.Instance.GetBestScoreSubmittedThisCycleTime(this);
		if (!bestScoreSubmittedThisCycleTime.HasValue || GetCycle(bestScoreSubmittedThisCycleTime.Value) != GetCycle(joinTime))
		{
			SecureStorage.Instance.ResetBestScoreSubmittedThisCycle(this);
		}
		LastJoinTime = joinTime;
		HighScoreShownSinceJoin = false;
		HasPickedUpRewardSinceLastJoin = false;
		HasBeenStoppedSinceLastJoin = false;
		if (!DidJoinInCurrentCycle(joinTime))
		{
		}
		ChallengeInstance = challenge;
		ScheduleDeviceNotificiations(joinTime);
		SecureStorage.Instance.PushToCloud();
	}

	private void ScheduleDeviceNotificiations(uint currentTime)
	{
	}

	private void PostJoinChallengeFacebookAction()
	{
		if (string.IsNullOrEmpty(FacebookUrl))
		{
			Debug.LogError("Unable to post to facebook because asset URL is empty.");
			return;
		}
		Bedrock.brFacebookActionParameters action = new Bedrock.brFacebookActionParameters("cloudpatrol:join", "challenge", FacebookUrl);
		Debug.Log(string.Format("Posting to facebook with '{0}', '{1}', '{2}'.", "cloudpatrol:join", "challenge", FacebookUrl));
		if (Bedrock.FacebookPostAction(action))
		{
			Debug.Log("Successfully posted join action to facebook.");
		}
		else
		{
			Debug.LogWarning("Failed to post join action to facebook.");
		}
	}

	private Challenge CreateChallengeInstance(uint joinTime, bool isBeingCreatedFromLoad)
	{
		Challenge challenge = Challenge.BuildChallengeInstance(this, joinTime, isBeingCreatedFromLoad);
		if (challenge != null)
		{
			challenge.gameObject.transform.parent = ChallengeManager.Instance.ChallengesUnderway.transform;
		}
		return challenge;
	}

	public void TryLoad(uint currentTime)
	{
		InstanceLog.LogDebug("Trying to load previous state for challenge.");
		uint cycle = GetCycle(currentTime);
		uint? bestScoreSubmittedThisCycleTime = SecureStorage.Instance.GetBestScoreSubmittedThisCycleTime(this);
		if (bestScoreSubmittedThisCycleTime.HasValue)
		{
			uint cycle2 = GetCycle(bestScoreSubmittedThisCycleTime.Value);
			if (cycle != cycle2)
			{
				InstanceLog.Log("Clearing BestScoreSubmittedThisCycle, our cycle ({0}) is not the submitted cycle (Cycle={1}, Time={2}).", cycle, cycle2, bestScoreSubmittedThisCycleTime);
				SecureStorage.Instance.ResetBestScoreSubmittedThisCycle(this);
			}
		}
		if (Status != ChallengeStatus.Open)
		{
			InstanceLog.LogDebug("Challenge is not open (it is {0}). Not instantiating.", Status);
			return;
		}
		uint? lastJoinTime = SecureStorage.Instance.GetLastJoinTime(this);
		if (!lastJoinTime.HasValue)
		{
			InstanceLog.LogDebug("No last join time found. Not instantiating.");
			return;
		}
		if (HasBeenStoppedSinceLastJoin)
		{
			InstanceLog.LogDebug("Challenge has valid join time, but it was stopped afterwards. Not instantiating.");
			return;
		}
		uint cycle3 = GetCycle(lastJoinTime.Value);
		if (cycle3 != cycle)
		{
			InstanceLog.LogDebug("Challenge was last joined in cycle {1}, but current cycle is {2}. Not instantiating.", Id, cycle3, cycle);
			ResetStoredChallengeState();
			return;
		}
		uint? currentLifeRemaining = SecureStorage.Instance.GetCurrentLifeRemaining(this);
		if (currentLifeRemaining.GetValueOrDefault() == 0 && currentLifeRemaining.HasValue)
		{
			InstanceLog.LogDebug("Challenge has valid join time, but has 0 life remaining. Not instantiating.");
			ResetStoredChallengeState();
			return;
		}
		InstanceLog.Log("Instantiating challenge from previously loaded data. LastJoinTime = {0}.", lastJoinTime);
		Challenge challengeInstance = CreateChallengeInstance(lastJoinTime.Value, true);
		ChallengeInstance = challengeInstance;
	}

	public void ResetStoredChallengeState()
	{
		InstanceLog.LogDebug("Resetting {0}'s stored challenge state...", Name);
		SecureStorage.Instance.ResetBestScoreSubmittedThisCycle(this);
		SecureStorage.Instance.SetHasBeenStoppedSinceLastJoin(this, false);
	}

	public uint GetRemainingTime(uint time)
	{
		if (time < StartDate)
		{
			return 0u;
		}
		if (time > StartDate)
		{
			uint num = ActiveDuration + InactiveDuration + InvalidDuration;
			uint num2 = time - StartDate;
			uint num3 = num2 / num;
			if (num3 > NumResets)
			{
				return 0u;
			}
			uint num4 = num2 % num;
			if (num4 > ActiveDuration)
			{
				return 0u;
			}
			return ActiveDuration - num4;
		}
		return 0u;
	}

	public ChallengeStatus GetStatusAtTime(uint time)
	{
		if (time < StartDate)
		{
			return ChallengeStatus.Invalid;
		}
		uint num = time - StartDate;
		uint num2 = ActiveDuration + InactiveDuration;
		uint num3 = num2 + InvalidDuration;
		uint num4 = num / num3;
		if (num4 > NumResets)
		{
			return ChallengeStatus.Invalid;
		}
		uint num5 = num % num3;
		if (num5 <= ActiveDuration)
		{
			return ChallengeStatus.Open;
		}
		if (num5 <= num2)
		{
			return ChallengeStatus.Finished;
		}
		return ChallengeStatus.Invalid;
	}

	public uint GetCycle(uint time)
	{
		if (time < StartDate)
		{
			InstanceLog.LogWarning("Trying to get cycle for time {0} which is before start time ({1}).", time, StartDate);
			return 0u;
		}
		uint num = time - StartDate;
		uint num2 = ActiveDuration + InactiveDuration + InvalidDuration;
		return num / num2;
	}

	public void UpdateStatusFromSynchronizedTime(uint time)
	{
		if (_waitingForStatusUpdate)
		{
			return;
		}
		_waitingForStatusUpdate = true;
		ChallengeStatus statusAtTime = GetStatusAtTime(time);
		if (statusAtTime != Status)
		{
			InstanceLog.LogDebug("Finished getting status. Bedrock said it was {0}..", statusAtTime);
			Status = statusAtTime;
			if (statusAtTime == ChallengeStatus.Invalid)
			{
				InstanceLog.Log("Clearing best score submitted this cycle, we are going to invalid status.");
				SecureStorage.Instance.ResetBestScoreSubmittedThisCycle(this);
			}
		}
		_waitingForStatusUpdate = false;
	}

	private void OnStatusChanged(ChallengeStatus oldStatus)
	{
		if (ChallengeData.StatusChanged != null)
		{
			ChallengeData.StatusChanged(this, new StatusChangedEventArgs(this, oldStatus));
		}
	}

	private bool GetIsRewardWaiting(uint currentTime)
	{
		if (GetStatusAtTime(currentTime) == ChallengeStatus.Finished && DidJoinInCurrentCycle(currentTime))
		{
			long? bestScoreSubmittedThisCycle = BestScoreSubmittedThisCycle;
			if (bestScoreSubmittedThisCycle.HasValue)
			{
				if (GetMedalForScore(bestScoreSubmittedThisCycle.Value) != 0)
				{
					return !HasPickedUpRewardSinceLastJoin;
				}
				ChallengeLeaderboardDataCache.CachedLeaderboardData data = ChallengeManager.Instance.LeaderboardDataCache.GetData(this);
				if (data != null && data.FriendData.FriendsRankedHigher != 0)
				{
					return !HasPickedUpRewardSinceLastJoin;
				}
			}
		}
		return false;
	}

	public bool ShouldBeVisibleInList(uint currentTime, uint? lastGameStartTime)
	{
		if (ActStructure.Instance.MissionInProgress)
		{
			if (Status == ChallengeStatus.Invalid || Status == ChallengeStatus.Unknown)
			{
				return false;
			}
			if (lastGameStartTime.HasValue && GetStatusAtTime(lastGameStartTime.Value) != ChallengeStatus.Open)
			{
				return false;
			}
			if (!DidJoinInCurrentCycle(currentTime))
			{
				return false;
			}
			return IsJoined;
		}
		if (Status == ChallengeStatus.Unknown || Status == ChallengeStatus.Invalid)
		{
			return false;
		}
		if (Status == ChallengeStatus.Finished && !DidJoinInCurrentCycle(currentTime))
		{
			return false;
		}
		return true;
	}

	public int CompareTo(ChallengeData other, uint currentTime)
	{
		if (other == this)
		{
			return 0;
		}
		if (other == null)
		{
			return -1;
		}
		if (Status != other.Status)
		{
			int num = ((int)Status).CompareTo((int)other.Status);
			if (num < 0)
			{
			}
			return num;
		}
		int num2 = GetRemainingTime(currentTime).CompareTo(other.GetRemainingTime(currentTime));
		switch (num2)
		{
		case 0:
			return Id.CompareTo(other.Id);
		default:
			return num2;
		}
	}

	private static ChallengeStatus Convert(Bedrock.brChallengeStatus status)
	{
		switch (status)
		{
		case Bedrock.brChallengeStatus.BR_CHALLENGE_ACTIVE:
			return ChallengeStatus.Open;
		case Bedrock.brChallengeStatus.BR_CHALLENGE_INACTIVE:
			return ChallengeStatus.Finished;
		case Bedrock.brChallengeStatus.BR_CHALLENGE_INVALID:
			return ChallengeStatus.Invalid;
		default:
			return ChallengeStatus.Unknown;
		}
	}

	public override string ToString()
	{
		return string.Format("[ChallengeData: Id={0}, StartDate={1}, ActiveDuration={2}, InactiveDuration={3}, InvalidDuration={4}, NumResets={5}, LeaderboardId={6}, Status={7}, Name={8}, Description={9}, ScriptType={10}, ScriptParameters={11}, Lifetime={12}, BronzeGoal={13}, SilverGoal={14}, GoldGoal={15}, GoalNoun={16}, IsTime={17}, IsJoined={18}]", Id, StartDate, ActiveDuration, InactiveDuration, InvalidDuration, NumResets, LeaderboardId, Status, Name, Description, ScriptType, ScriptParameters, Lifetime, BronzeGoal, SilverGoal, GoldGoal, GoalNoun, IsTime, IsJoined);
	}

	public string ToJsonString()
	{
		string swrveDataKey = GetSwrveDataKey(Id);
		string[] value = new string[19]
		{
			GetJsonString("item_class", "challenge"),
			GetJsonString("ChallengeName", Name),
			GetJsonString("ChallengeDescription", Description),
			GetJsonString("startdate", StartDate),
			GetJsonString("activeduration", ActiveDuration),
			GetJsonString("inactiveduration", InactiveDuration),
			GetJsonString("invalidduration", InvalidDuration),
			GetJsonString("numresets", NumResets),
			GetJsonString("leaderboardid", LeaderboardId),
			GetJsonString("ScriptType", ScriptType),
			GetJsonString("ScriptParameters", ScriptParameters),
			GetJsonString("Lifetime", Lifetime),
			GetJsonString("BronzeGoal", BronzeGoal),
			GetJsonString("SilverGoal", SilverGoal),
			GetJsonString("GoldGoal", GoldGoal),
			GetJsonString("LeaderboardType", LeaderboardType),
			GetJsonString("FacebookURL", FacebookUrl),
			GetJsonString("GoalNoun", GoalNoun),
			GetJsonString("IsTime", IsTime)
		};
		return "\"" + swrveDataKey + "\":{" + string.Join(",", value) + "}";
	}

	public static string GetJsonString(string key, object value)
	{
		return string.Format("\"{0}\":\"{1}\"", key, value.ToString());
	}

	public ChallengeMedalType GetMedalForScore(long score)
	{
		if (score == GoldGoal || IsBetterScore(score, GoldGoal))
		{
			return ChallengeMedalType.Gold;
		}
		if (score == SilverGoal || IsBetterScore(score, SilverGoal))
		{
			return ChallengeMedalType.Silver;
		}
		if (score == BronzeGoal || IsBetterScore(score, BronzeGoal))
		{
			return ChallengeMedalType.Bronze;
		}
		return ChallengeMedalType.None;
	}

	public uint GetRewardForMedal(ChallengeMedalType medal)
	{
		switch (medal)
		{
		case ChallengeMedalType.Bronze:
			return BronzeReward;
		case ChallengeMedalType.Silver:
			return SilverReward;
		case ChallengeMedalType.Gold:
			return GoldReward;
		default:
			return 0u;
		}
	}

	public string BuildGoalString(long numberOfItems)
	{
		if (IsTime)
		{
			return TimeUtils.GetLongTimeStringFromSeconds(numberOfItems);
		}
		return string.Format("{0} {1}", numberOfItems.ToString("n0"), LocalizedGoalNoun);
	}

	public string BuildRewardString(long numberOfItems)
	{
		if (IsTime)
		{
			return TimeUtils.GetShortTimeStringFromSeconds(numberOfItems);
		}
		return string.Format("{0} {1}", numberOfItems.ToString("n0"), LocalizedGoalNoun);
	}

	public uint GetRewardForFriendsBeaten(ulong friendsBeaten)
	{
		return FriendsBeatenReward * System.Convert.ToUInt32(friendsBeaten);
	}

	public bool DidJoinInCurrentCycle(uint currentTime)
	{
		uint? lastJoinTime = LastJoinTime;
		return lastJoinTime.HasValue && GetCycle(lastJoinTime.Value) == GetCycle(currentTime);
	}

	public bool IsBetterScore(long newScore, long referenceScore)
	{
		bool flag = CompareScores(newScore, referenceScore, LeaderboardType) == ScoreComparisonResult.FirstIsBetter;
		InstanceLog.LogDebug("Decided that '{0}' is {1}a better score than '{2}'", newScore, (!flag) ? "NOT " : string.Empty, referenceScore);
		return flag;
	}

	public static ScoreComparisonResult CompareScores(long firstScore, long secondScore, ChallengeLeaderboardType leaderboardType)
	{
		if (firstScore == secondScore)
		{
			return ScoreComparisonResult.Equal;
		}
		switch (leaderboardType)
		{
		case ChallengeLeaderboardType.MaxOnTop:
			return (firstScore <= secondScore) ? ScoreComparisonResult.SecondIsBetter : ScoreComparisonResult.FirstIsBetter;
		case ChallengeLeaderboardType.MinOnTop:
			return (firstScore > secondScore) ? ScoreComparisonResult.SecondIsBetter : ScoreComparisonResult.FirstIsBetter;
		default:
			Debug.LogError(string.Concat("Unable to compare ", firstScore, " and ", secondScore, " since '", leaderboardType, "' is not a recognized leaderboard type."));
			return ScoreComparisonResult.Equal;
		}
	}

	private void OnRewardCollected()
	{
		if (ChallengeData.RewardCollected != null)
		{
			ChallengeData.RewardCollected(this, new EventArgs());
		}
	}
}
