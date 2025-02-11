using System;
using System.Collections;
using UnityEngine;

public class GMGDominationManager : MonoBehaviour
{
	public DominationCapturePointManager DCPM;

	private int targetScore = 5000;

	private int tokenReward = 2;

	private int tokenRewardMultiplier = 2;

	private int scoreInterval = 5000;

	private int scoreIntervalMultiplier = 1;

	private string dial = string.Empty;

	private float mTimeSurvived;

	private int mScore;

	private bool mOverTimeScoreEarned = true;

	private string mJustEarnedString = string.Empty;

	private string mNextGoalString = string.Empty;

	private float scoreTimer;

	private ScriptedObjective mObjective;

	public GameObject mObjective_Object;

	public float TimeSurvived
	{
		get
		{
			return mTimeSurvived;
		}
		set
		{
			mTimeSurvived = value;
		}
	}

	private void Start()
	{
	}

	private void Awake()
	{
		OnDisable();
		WaveStats.Instance.Reset();
	}

	public void SetStartWave(string val)
	{
		if ((bool)GMGData.Instance)
		{
			GMGData.Instance.SetStartWave(Convert.ToInt32(val));
		}
	}

	public void UpdateWave()
	{
	}

	public void SetStartTime(string val)
	{
		TimeSurvived = Convert.ToInt32(val);
		CommonHudController.Instance.MissionTimer.Set(TimeSurvived, 0f);
	}

	private void UpdateEnemiesRemaining()
	{
	}

	private void UpdateEnemiesRemaining(GameObject sender)
	{
	}

	private void Update()
	{
		if ((bool)CommonHudController.Instance)
		{
			CommonHudController.Instance.UpdateMultipiler(DCPM.TotalUnderPlayerControl);
		}
		if (mScore >= targetScore)
		{
			StartCoroutine("RewardPlayer");
		}
		scoreTimer += Time.deltaTime;
		if (scoreTimer >= 1f && mOverTimeScoreEarned)
		{
			int num = 10 * DCPM.TotalUnderPlayerControl;
			EventHub.Instance.Report(new Events.GMGScoreAdded(num));
			mScore += num;
			scoreTimer = 0f;
		}
	}

	private IEnumerator RewardPlayer()
	{
		mJustEarnedString = string.Format(Language.Get("S_DOMINATION_TOKENREWARD"), "[#5CC6CC]", "[#FFFFFF]", CommonHelper.HardCurrencySymbol(), tokenReward.ToString());
		GameSettings.Instance.PlayerCash().AwardHardCash(tokenReward, "SpecOpsWaveReward");
		GMGSFX.Instance.TokenAwarded.Play2D();
		if (scoreIntervalMultiplier > 1)
		{
			scoreInterval *= scoreIntervalMultiplier;
		}
		else
		{
			targetScore += scoreInterval;
		}
		if (tokenReward * tokenRewardMultiplier <= 16)
		{
			tokenReward *= tokenRewardMultiplier;
		}
		mNextGoalString = string.Format(Language.Get("S_DOMINATION_NEXTTOKENREWARD"), "[#5CC6CC]", "[#FFFFFF]", targetScore.ToString(), "[#5CC6CC]", "[#FFFFFF]", CommonHelper.HardCurrencySymbol(), tokenReward.ToString());
		HUDMessenger.Instance.PushMessage(mJustEarnedString, string.Empty, mNextGoalString, false);
		yield break;
	}

	private void UpdateScore(object sender, Events.XPEarned args)
	{
		if (mOverTimeScoreEarned)
		{
			int num = args.XP * DCPM.TotalUnderPlayerControl;
			EventHub.Instance.Report(new Events.GMGScoreAdded(num));
			mScore += num;
		}
	}

	protected void OnEnable()
	{
		targetScore = GMGData.Instance.DominationTargetScore;
		tokenReward = GMGData.Instance.DominationTokenReward;
		tokenRewardMultiplier = GMGData.Instance.DominationTokenRewardMultiplier;
		scoreInterval = GMGData.Instance.DominationScoreInterval;
		scoreIntervalMultiplier = GMGData.Instance.DominationScoreIntervalMultiplier;
		string strTitleMessage = string.Format(Language.Get("S_DOMINATION_QUICKTUTORIAL1"), "[#BDB030]", "[#FFFFFF]", "[#BDB030]", "[#FFFFFF]");
		string strSubMessage = string.Format(Language.Get("S_DOMINATION_QUICKTUTORIAL2"), "[#BDB030]", "[#FFFFFF]", "[#BDB030]", "[#FFFFFF]", "[#BDB030]", "[#FFFFFF]");
		HUDMessenger.Instance.PushMessage(strTitleMessage, string.Empty, strSubMessage, false);
		mNextGoalString = string.Format(Language.Get("S_DOMINATION_NEXTTOKENREWARD"), "[#5CC6CC]", "[#FFFFFF]", targetScore.ToString(), "[#5CC6CC]", "[#FFFFFF]", CommonHelper.HardCurrencySymbol(), tokenReward.ToString());
		HUDMessenger.Instance.PushMessage(mNextGoalString, string.Empty, string.Empty, false);
		if (mObjective_Object == null)
		{
			mObjective_Object = GameObject.Find("Scripted");
		}
		if (mObjective_Object != null)
		{
			mObjective = mObjective_Object.GetComponentInChildren<ScriptedObjective>();
			if (mObjective != null)
			{
				mObjective.SetDormantState();
				mObjective.m_Interface.ObjectiveLabel = "S_DOM_OBJECTIVE";
				mObjective.Activate();
			}
		}
		scoreInterval = targetScore;
		SetStartTime("60");
		CommonHudController instance = CommonHudController.Instance;
		GMGData.Instance.CurrentGameType = GMGData.GameType.Domination;
		instance.ShowWave(false);
		instance.ShowGMGScore(true);
		instance.UpdateTokens(GameSettings.Instance.PlayerCash().HardCash());
		instance.MissionTimer.StartTimer();
		EventHub.Instance.OnXPEarned += UpdateScore;
		StartCoroutine(TimerMonitor());
	}

	protected void OnDisable()
	{
		if ((bool)EventHub.Instance)
		{
			EventHub.Instance.OnXPEarned -= UpdateScore;
		}
		if ((bool)CommonHudController.Instance)
		{
			CommonHudController.Instance.MissionTimer.StopTimer();
		}
	}

	private IEnumerator TimerMonitor()
	{
		float nextBleep = 5f;
		bool hasPlayed = false;
		while (CommonHudController.Instance.MissionTimer.CurrentTime() > 0f)
		{
			float curTime = CommonHudController.Instance.MissionTimer.CurrentTime();
			if (curTime <= 10f && !hasPlayed)
			{
				hasPlayed = true;
				dial = "S_DOMINATION_DIALOGUE_TIMERUNNINGOUT";
				CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
				PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
			}
			if (curTime <= nextBleep)
			{
				if (curTime <= 1f)
				{
					GMGSFX.Instance.NextWaveCountdownLast.Play2D();
				}
				else
				{
					GMGSFX.Instance.NextWaveCountdown.Play2D();
				}
				nextBleep -= 1f;
			}
			else if (curTime >= 5f && nextBleep < 5f)
			{
				nextBleep = 5f;
			}
			yield return null;
		}
		TutorialToggles.LockFPPMovement = true;
		CommonHudController.Instance.TriggerLocked = true;
		mOverTimeScoreEarned = false;
		CharacterPropertyModifier.SetControl(GameController.Instance.mFirstPersonActor, false, base.gameObject, true);
		GameController.Instance.mFirstPersonActor.awareness.CanBeLookedAt = false;
		GameController.Instance.mFirstPersonActor.awareness.FlushAllAwareness();
		GameController.Instance.mFirstPersonActor.health.Invulnerable = true;
		string finalScore = string.Format(Language.Get("S_DOMINATION_END2"), "[#BDB030]", "[#FFFFFF]", mScore.ToString(), "[#BDB030]");
		string missionComplete = string.Format(Language.Get("S_DOMINATION_END1"), "[#D91515]");
		dial = "S_TT_DIALOGUE_TIMEOUT";
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
		float wait1 = 1f;
		while (wait1 > 0f)
		{
			wait1 -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		HUDMessenger.Instance.PushMessage(missionComplete, string.Empty, finalScore, true);
		float wait2 = 6f;
		while (wait2 > 0f)
		{
			wait2 -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		if (GlobalObjectiveManager.Instance == null)
		{
			GameController.Instance.OnMissionPassed(this, 1f);
			GameController.Instance.OverrideTimePlayed(TimeSurvived);
		}
		else
		{
			GlobalObjectiveManager.Instance.SectionPassed();
		}
	}
}
