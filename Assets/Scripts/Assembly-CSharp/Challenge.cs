using System;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
	public class AttemptCompletingEventArgs : EventArgs
	{
		public bool TimedOut { get; private set; }

		public AttemptCompletingEventArgs(bool timedOut)
		{
			TimedOut = timedOut;
		}
	}

	public class BestScoreAchievedEventArgs : EventArgs
	{
		public long Score { get; private set; }

		public long? PreviousBest { get; private set; }

		public BestScoreAchievedEventArgs(long score, long? previousBest)
		{
			Score = score;
			PreviousBest = previousBest;
		}
	}

	protected const string UndefinedTimeRemainingString = null;

	private static readonly ILogger _classLogger = LogBuilder.Instance.GetLogger(typeof(Challenge), LogLevel.Warning);

	private uint _lifeRemaining;

	private long _leaderboardValue;

	private ILogger _instanceLogger;

	private ChallengeData _challengeData;

	public uint ChallengeId { get; private set; }

	public bool MadeProgress { get; private set; }

	protected ILogger InstanceLog
	{
		get
		{
			if (_instanceLogger == null)
			{
				_instanceLogger = LogBuilder.Instance.GetCustomLogger("Challenge, " + ChallengeId, LogLevel.Warning);
			}
			return _instanceLogger;
		}
	}

	public bool BeatBestScoreSubmittedSinceJoin
	{
		get
		{
			if (!MadeProgress)
			{
				return false;
			}
			long? bestScoreSubmittedThisCycle = ChallengeData.BestScoreSubmittedThisCycle;
			if (!bestScoreSubmittedThisCycle.HasValue)
			{
				return true;
			}
			return ChallengeData.IsBetterScore(LeaderboardValue, bestScoreSubmittedThisCycle.Value);
		}
	}

	public long BestScoreInCycle
	{
		get
		{
			long? bestScoreSubmittedThisCycle = ChallengeData.BestScoreSubmittedThisCycle;
			long leaderboardValue = LeaderboardValue;
			if (!bestScoreSubmittedThisCycle.HasValue || ChallengeData.IsBetterScore(leaderboardValue, bestScoreSubmittedThisCycle.Value))
			{
				return leaderboardValue;
			}
			return bestScoreSubmittedThisCycle.Value;
		}
	}

	public virtual bool IsRetryable
	{
		get
		{
			return false;
		}
	}

	protected virtual long DefaultLeaderboardScore
	{
		get
		{
			return 0L;
		}
	}

	public uint JoinTime { get; private set; }

	public uint LifeRemaining
	{
		get
		{
			return _lifeRemaining;
		}
		protected set
		{
			InstanceLog.Log("Life changed from " + _lifeRemaining + " to " + value);
			_lifeRemaining = value;
			OnLifeChanged();
			if (_lifeRemaining == 0)
			{
				InstanceLog.Log("No life remaining. Finishing challenge");
				CompleteAttemptFromGameplayAction();
			}
		}
	}

	public abstract string LifeRemainingText { get; }

	public virtual string StatusPanelStatusText
	{
		get
		{
			if (!ChallengeData.BestScoreSubmittedThisCycle.HasValue)
			{
				return Language.GetFormatString("S_GMG_CURRENT_SCORE", LeaderboardValue);
			}
			return string.Format("{0}\n{1}", Language.GetFormatString("S_GMG_CURRENT_SCORE", LeaderboardValue), Language.GetFormatString("S_GMG_BEST_SCORE", BestScoreInCycle));
		}
	}

	public long LeaderboardValue
	{
		get
		{
			return _leaderboardValue;
		}
		protected set
		{
			InstanceLog.LogDebug("Updating leaderboard value to {0}", value);
			SetLeaderboardValueInternal(value, true);
		}
	}

	public ChallengeData ChallengeData
	{
		get
		{
			return ChallengeManager.Instance.DataProvider.GetChallengeData(ChallengeId);
		}
	}

	public static event EventHandler LifeChanged;

	public static event EventHandler LeaderboardValueChanged;

	public static event EventHandler<ValueEventArgs<ChallengeMedalType>> MedalEarned;

	public static event EventHandler<AttemptCompletingEventArgs> AttemptCompleting;

	public static event EventHandler StatusTextChanged;

	public static event EventHandler<BestScoreAchievedEventArgs> HighScoreAchieved;

	protected virtual void OnEnable()
	{
		ChallengeData.StatusChanged += HandleChallengeDataStatusChanged;
		ChallengeManager.ChallengesInvalidated += HandleChallengeManagerChallengesInvalidated;
	}

	protected virtual void OnDisable()
	{
		ChallengeData.StatusChanged -= HandleChallengeDataStatusChanged;
		ChallengeManager.ChallengesInvalidated -= HandleChallengeManagerChallengesInvalidated;
	}

	private void HandleChallengeManagerChallengesInvalidated(object sender, EventArgs e)
	{
		InstanceLog.Log("User changing. Committing suicide.");
		ChallengeManager.Instance.ChallengesUnderway.RemoveChallengeInstance(ChallengeId);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void HandleChallengeDataStatusChanged(object sender, ChallengeEventArgs e)
	{
		ChallengeData challenge = e.Challenge;
		if (challenge.Id == ChallengeId && challenge.Status == ChallengeStatus.Finished)
		{
			InstanceLog.Log("Finishing due to Status change.");
			CompleteAttempt(true);
		}
	}

	public void AbortAttempt()
	{
		SwrveEventsMetaGame.ChallengeStopped(ChallengeId);
		StopInternal();
	}

	protected void CompleteAttemptFromGameplayAction()
	{
		CompleteAttempt(false);
	}

	protected virtual void CompleteAttempt(bool timedOut)
	{
		OnAttemptCompleting(timedOut);
		StopInternal();
		long leaderboardValue = LeaderboardValue;
		ChallengeMedalType medalForScore = ChallengeData.GetMedalForScore(leaderboardValue);
		if (medalForScore != 0)
		{
			OnMedalEarned(medalForScore);
		}
		ulong friendsPlaying = 0uL;
		ulong friendsBeaten = 0uL;
		GetFriendsData(out friendsPlaying, out friendsBeaten);
		Debug.Log(string.Concat("SWRVE Callenge ", ChallengeId, " finished: medal ", medalForScore, " playing ", friendsPlaying, " beaten ", friendsBeaten));
		SwrveEventsMetaGame.ChallengeFinished(ChallengeId, timedOut, medalForScore, friendsPlaying, friendsBeaten);
	}

	public void GetFriendsData(out ulong friendsPlaying, out ulong friendsBeaten)
	{
		friendsPlaying = 0uL;
		friendsBeaten = 0uL;
		ChallengeLeaderboardDataCache.CachedLeaderboardData data = ChallengeManager.Instance.LeaderboardDataCache.GetData(ChallengeData);
		if (data != null && data.FriendData != null)
		{
			friendsBeaten = data.FriendData.FriendsRankedLower;
			friendsPlaying = data.FriendData.FriendsRankedLower + data.FriendData.FriendsRankedHigher;
		}
	}

	private void StopInternal()
	{
		InstanceLog.Log("Stopping challenge");
		ChallengeData.ChallengeInstance = null;
		ChallengeData.HasBeenStoppedSinceLastJoin = true;
		UnityEngine.Object.Destroy(base.gameObject);
		SecureStorage.Instance.PushToCloud();
	}

	protected virtual void Initialize(ChallengeData challengeData, uint joinTime, bool isBeingInitializedFromLoad)
	{
		ChallengeId = challengeData.Id;
		InstanceLog.LogDebug("Initialize({0}, {1}, {2})", challengeData.Id, joinTime, isBeingInitializedFromLoad);
		JoinTime = joinTime;
		if (isBeingInitializedFromLoad)
		{
			long? currentScore = SecureStorage.Instance.GetCurrentScore(challengeData);
			if (currentScore.HasValue)
			{
				InstanceLog.LogDebug("Restoring score ({0}) from save data.", currentScore.Value);
				SetLeaderboardValueInternal(currentScore.Value, false);
			}
			else
			{
				InstanceLog.LogDebug("No save for current score found. Reverting to default.");
				SetLeaderboardValueInternal(DefaultLeaderboardScore, false);
			}
			uint? currentLifeRemaining = SecureStorage.Instance.GetCurrentLifeRemaining(challengeData);
			if (currentLifeRemaining.HasValue)
			{
				InstanceLog.LogDebug("Restoring LifeRemaining ({0}) from save data.", currentLifeRemaining.Value);
				LifeRemaining = currentLifeRemaining.Value;
			}
			else
			{
				InstanceLog.LogDebug("No save for life remaining found. Reverting to default.");
				LifeRemaining = ChallengeData.Lifetime;
			}
		}
		else
		{
			Reset();
			SecureStorage.Instance.SetCurrentLifeRemaining(challengeData, LifeRemaining);
			SecureStorage.Instance.SetCurrentScore(challengeData, LeaderboardValue);
			SwrveEventsMetaGame.ChallengeJoined(ChallengeId);
		}
	}

	protected void Reset()
	{
		SetLeaderboardValueInternal(DefaultLeaderboardScore, false);
		LifeRemaining = ChallengeData.Lifetime;
	}

	private void SetLeaderboardValueInternal(long value, bool checkForMedalChange)
	{
		InstanceLog.LogDebug("SetLeaderboardValueInternal({0}, {1})", value, checkForMedalChange);
		if (_leaderboardValue != value)
		{
			InstanceLog.LogDebug(string.Concat("Challenge LeaderboardValueChanged ", _leaderboardValue, " -> ", value, " (", this, ")"));
			MadeProgress = true;
			_leaderboardValue = value;
			InstanceLog.LogDebug("Checking for best leaderboard value because of change.");
			TryUpdateBestLeaderboardValues();
			SecureStorage.Instance.SetCurrentScore(ChallengeData, LeaderboardValue);
			OnLeaderboardValueChanged();
		}
	}

	private void TryUpdateBestLeaderboardValues()
	{
		if (MadeProgress)
		{
			long? bestScoreSubmittedThisCycle = ChallengeData.BestScoreSubmittedThisCycle;
			if (!bestScoreSubmittedThisCycle.HasValue || ChallengeData.IsBetterScore(LeaderboardValue, bestScoreSubmittedThisCycle.Value))
			{
				OnHighScoreAchieved(LeaderboardValue, bestScoreSubmittedThisCycle);
			}
		}
	}

	private void OnLifeChanged()
	{
		SecureStorage.Instance.SetCurrentLifeRemaining(ChallengeData, LifeRemaining);
		if (Challenge.LifeChanged != null)
		{
			Challenge.LifeChanged(this, new EventArgs());
		}
	}

	private void OnLeaderboardValueChanged()
	{
		if (Challenge.LeaderboardValueChanged != null)
		{
			Challenge.LeaderboardValueChanged(this, new EventArgs());
		}
	}

	protected void OnMedalEarned(ChallengeMedalType medalType)
	{
		if (Challenge.MedalEarned != null)
		{
			Challenge.MedalEarned(this, new ValueEventArgs<ChallengeMedalType>(medalType));
		}
	}

	private void OnAttemptCompleting(bool timedOut)
	{
		InstanceLog.LogDebug("Attempt Complete - Timed Out = {0}.", timedOut);
		TryUpdateBestLeaderboardValues();
		if (Challenge.AttemptCompleting != null)
		{
			Challenge.AttemptCompleting(this, new AttemptCompletingEventArgs(timedOut));
		}
	}

	public override string ToString()
	{
		return ChallengeData.LocalizedName;
	}

	protected void OnStatusTextChanged()
	{
		if (Challenge.StatusTextChanged != null)
		{
			Challenge.StatusTextChanged(this, new EventArgs());
		}
	}

	protected void OnHighScoreAchieved(long score, long? previousBest)
	{
		if (Challenge.HighScoreAchieved != null)
		{
			Challenge.HighScoreAchieved(this, new BestScoreAchievedEventArgs(score, previousBest));
		}
	}

	public static Challenge BuildChallengeInstance(ChallengeData data, uint startTime, bool isBeingBuiltFromLoad)
	{
		GameObject gameObject = new GameObject(data.Name);
		Challenge challenge = null;
		switch (data.ScriptType)
		{
		case "MostXPSpecOpsChallenge":
			challenge = gameObject.AddComponent<MostXPSpecOpsChallenge>();
			break;
		case "MostHeadshotsTimedChallenge":
			challenge = gameObject.AddComponent<MostHeadshotsTimedChallenge>();
			break;
		case "SpecOpsKillsRaceChallenge":
			challenge = gameObject.AddComponent<SpecOpsKillsRaceChallenge>();
			break;
		case "MostGrenadeKillsOneRunChallenge":
			challenge = gameObject.AddComponent<MostGrenadeKillsOneRunChallenge>();
			break;
		case "KillEnemiesWithClaymoreTimedChallenge":
			challenge = gameObject.AddComponent<KillEnemiesWithClaymoreTimedChallenge>();
			break;
		case "CompleteWavesRaceChallenge":
			challenge = gameObject.AddComponent<CompleteWavesRaceChallenge>();
			break;
		case "ShotgunKillsTimedChallenge":
			challenge = gameObject.AddComponent<ShotgunKillsTimedChallenge>();
			break;
		case "MostClaymoreKillsChallenge":
			challenge = gameObject.AddComponent<MostClaymoreKillsChallenge>();
			break;
		case "MostStealthKillsChallenge":
			challenge = gameObject.AddComponent<MostStealthKillsChallenge>();
			break;
		case "LMGKillsSpecOpsRaceChallenge":
			challenge = gameObject.AddComponent<LMGKillsSpecOpsRaceChallenge>();
			break;
		case "MostSniperKillsTimedChallenge":
			challenge = gameObject.AddComponent<MostSniperKillsTimedChallenge>();
			break;
		case "MostMultiKillChallenge":
			challenge = gameObject.AddComponent<MostMultiKillChallenge>();
			break;
		case "MostFragKillsTimedChallenge":
			challenge = gameObject.AddComponent<MostFragKillsTimedChallenge>();
			break;
		case "MostHeadshotsRaceChallenge":
			challenge = gameObject.AddComponent<MostHeadshotsRaceChallenge>();
			break;
		case "KillMostEnemiesTimedChallenge":
			challenge = gameObject.AddComponent<KillMostEnemiesTimedChallenge>();
			break;
		case "EarnXPRaceChallenge":
			challenge = gameObject.AddComponent<EarnXPRaceChallenge>();
			break;
		case "AssaultRifleKillsTimedChallenge":
			challenge = gameObject.AddComponent<AssaultRifleKillsTimedChallenge>();
			break;
		case "SpecOpsBestAccuracyOneRunChallenge":
			challenge = gameObject.AddComponent<SpecOpsBestAccuracyOneRunChallenge>();
			break;
		case "ClaymoreKillsRaceChallenge":
			challenge = gameObject.AddComponent<ClaymoreKillsRaceChallenge>();
			break;
		case "SniperKillsRaceChallenge":
			challenge = gameObject.AddComponent<SniperKillsRaceChallenge>();
			break;
		case "LMGKillsTimedChallenge":
			challenge = gameObject.AddComponent<LMGKillsTimedChallenge>();
			break;
		case "MultiKillRaceChallenge":
			challenge = gameObject.AddComponent<MultiKillRaceChallenge>();
			break;
		case "GrenadeKillsRaceChallenge":
			challenge = gameObject.AddComponent<GrenadeKillsRaceChallenge>();
			break;
		case "AmmoCacheOneRunChallenge":
			challenge = gameObject.AddComponent<AmmoCacheOneRunChallenge>();
			break;
		default:
			_classLogger.LogError("Unable to build challenge. Unknown script type '" + data.ScriptType + "'.");
			UnityEngine.Object.Destroy(gameObject);
			return null;
		}
		challenge.Initialize(data, startTime, isBeingBuiltFromLoad);
		return challenge;
	}
}
