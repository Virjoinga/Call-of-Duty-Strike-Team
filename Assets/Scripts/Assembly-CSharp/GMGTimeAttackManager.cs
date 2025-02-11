using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMGTimeAttackManager : MonoBehaviour
{
	public List<GameObject> Coordinators = new List<GameObject>();

	private List<SpawnerCoordinator> mCoordinators = new List<SpawnerCoordinator>();

	private int mCurDifficultyLevel;

	private float mTimeSurvived;

	private float mStartingTime;

	private float mTimer;

	private string mNextReward = string.Empty;

	private string dial = string.Empty;

	private int NextReward = 1;

	private int NextRewardMultiplier = 2;

	private int RewardTime = 60;

	private int MaximumReward = 8;

	private bool mWarningDelay = true;

	private bool stopRewards;

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

	private void Update()
	{
		mTimer += Time.deltaTime;
		if (mTimer > (float)RewardTime)
		{
			StartCoroutine("RewardPlayer");
			mTimer = 0f;
		}
	}

	private IEnumerator RewardPlayer()
	{
		if (!stopRewards)
		{
			if (HUDMessenger.Instance != null)
			{
				mNextReward = string.Format(Language.Get("S_TT_REWARD"), CommonHelper.HardCurrencySymbol(), NextReward.ToString());
			}
			HUDMessenger.Instance.PushMessage(mNextReward, string.Empty, string.Empty, false);
			GameSettings.Instance.PlayerCash().AwardHardCash(NextReward, "SpecOpsWaveReward");
			GMGSFX.Instance.TokenAwarded.Play2D();
			if (NextReward * NextRewardMultiplier <= MaximumReward)
			{
				NextReward *= NextRewardMultiplier;
			}
			yield return new WaitForSeconds(1f);
			if (HUDMessenger.Instance != null)
			{
				mNextReward = string.Format(Language.Get("S_TT_NEXTREWARD"), CommonHelper.HardCurrencySymbol(), NextReward.ToString());
			}
			HUDMessenger.Instance.PushMessage(mNextReward, string.Empty, string.Empty, false);
		}
	}

	private void Awake()
	{
		OnDisable();
		WaveStats.Instance.Reset();
		foreach (GameObject coordinator in Coordinators)
		{
			mCoordinators.Add(coordinator.GetComponentInChildren<SpawnerCoordinator>());
		}
	}

	public void SetStartWave(string val)
	{
	}

	public void SetStartTime(string val)
	{
		TimeSurvived = Convert.ToInt32(val);
		CommonHudController.Instance.MissionTimer.Set(TimeSurvived, 0f);
		mStartingTime = TimeSurvived;
	}

	public void UpdateWave()
	{
	}

	private void UpdateScore(object sender, Events.XPEarned args)
	{
		EventHub.Instance.Report(new Events.GMGScoreAdded(args.XP));
	}

	private void BonusTimeReward(object sender, Events.Kill args)
	{
		if (args.Victim.Faction != 0)
		{
			int num = 0;
			string text = string.Empty;
			if (args.Knife)
			{
				num = GMGData.Instance.TTBonusKnifeKill;
				text = Language.Get("S_TIMEATTACK_EXTRA_MELEE");
			}
			else if (args.GrenadeKill || args.Explosion || args.ClaymoreKill)
			{
				num = GMGData.Instance.TTBonusExplosion;
				text = Language.Get("S_TIMEATTACK_EXTRA_EXPLOSIVE!");
			}
			else if (args.HeadShot && args.OneShotKill)
			{
				num = GMGData.Instance.TTBonusHeadshot;
				text = Language.Get("S_TIMEATTACK_EXTRA_HEADSHOT");
			}
			else
			{
				num = GMGData.Instance.TTBonusKill;
			}
			string msg = text + " +" + num + " " + Language.Get("S_TIME_SECONDS").ToUpper();
			CommonHudController.Instance.AddXpFeedback(0, msg, null);
			TimeSurvived += num;
			CommonHudController.Instance.MissionTimer.Add(num);
		}
	}

	private void UpdateEnemiesRemaining(GameObject sender)
	{
		SendMessageOnRecievedCount componentInChildren = sender.GetComponentInChildren<SendMessageOnRecievedCount>();
		CommonHudController.Instance.UpdateEnemiesRemaining(componentInChildren.Target - componentInChildren.Count);
	}

	protected void OnEnable()
	{
		NextReward = GMGData.Instance.TTNextReward;
		NextRewardMultiplier = GMGData.Instance.TTNextRewardMultiplier;
		RewardTime = GMGData.Instance.TTRewardInterval;
		MaximumReward = GMGData.Instance.TTMaximumReward;
		CommonHudController instance = CommonHudController.Instance;
		HUDMessenger.Instance.PushMessage(Language.Get("S_TT_QUICKTUTORIAL"), string.Empty, string.Empty, false);
		if (HUDMessenger.Instance != null)
		{
			mNextReward = string.Format(Language.Get("S_TT_NEXTREWARD"), CommonHelper.HardCurrencySymbol(), NextReward.ToString());
		}
		HUDMessenger.Instance.PushMessage(mNextReward, string.Empty, string.Empty, false);
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
				mObjective.m_Interface.ObjectiveLabel = "S_TT_OBJECTIVE";
				mObjective.Activate();
			}
		}
		EventHub.Instance.OnXPEarned += UpdateScore;
		EventHub.Instance.OnKill += BonusTimeReward;
		instance.UpdateTokens(GameSettings.Instance.PlayerCash().HardCash());
		SetStartTime("60");
		SetNextDifficultyLevel(0);
		instance.MissionTimer.StartTimer();
		instance.ShowGMGScore(true);
		StartCoroutine(TimerMonitor());
		GMGData.Instance.CurrentGameType = GMGData.GameType.TimeAttack;
	}

	protected void OnDisable()
	{
		EventHub.Instance.OnXPEarned -= UpdateScore;
		EventHub.Instance.OnKill -= BonusTimeReward;
		CommonHudController.Instance.MissionTimer.StopTimer();
	}

	private IEnumerator TimerMonitor()
	{
		float nextBleep = 5f;
		float curTime2 = 0f;
		int currentWarning = 1;
		while (CommonHudController.Instance.MissionTimer.CurrentTime() > 0f)
		{
			curTime2 = CommonHudController.Instance.MissionTimer.CurrentTime();
			CheckDifficultyFlow();
			if (curTime2 <= 7f && mWarningDelay)
			{
				StartCoroutine("WarningDelay");
				switch (currentWarning)
				{
				case 1:
					dial = "S_TT_DIALOGUE_TIMERUNNINGOUT_VAR1";
					CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
					PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
					currentWarning++;
					break;
				case 2:
					dial = "S_TT_DIALOGUE_TIMERUNNINGOUT_VAR2";
					CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
					PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
					currentWarning++;
					break;
				case 3:
					dial = "S_TT_DIALOGUE_TIMERUNNINGOUT_VAR1";
					CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
					PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
					currentWarning = 1;
					break;
				}
			}
			if (curTime2 <= nextBleep)
			{
				if (curTime2 <= 1f)
				{
					GMGSFX.Instance.NextWaveCountdownLast.Play2D();
				}
				else
				{
					GMGSFX.Instance.NextWaveCountdown.Play2D();
				}
				nextBleep -= 1f;
			}
			else if (curTime2 >= 5f && nextBleep < 5f)
			{
				nextBleep = 5f;
			}
			yield return null;
		}
		TutorialToggles.LockFPPMovement = true;
		CommonHudController.Instance.TriggerLocked = true;
		CharacterPropertyModifier.SetControl(GameController.Instance.mFirstPersonActor, false, base.gameObject, true);
		GameController.Instance.mFirstPersonActor.awareness.CanBeLookedAt = false;
		GameController.Instance.mFirstPersonActor.awareness.FlushAllAwareness();
		GameController.Instance.mFirstPersonActor.health.Invulnerable = true;
		TimeSpan t = TimeSpan.FromSeconds(TimeSurvived);
		string time = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
		HUDMessenger.Instance.PushMessage(Language.Get("S_TT_TIMEOUT"), Language.Get("S_TT_SURVIVE"), "[#FFFF00]" + time.ToString(), false);
		dial = "S_TT_DIALOGUE_TIMEOUT";
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
		stopRewards = true;
		float wait = 7f;
		while (wait > 0f)
		{
			wait -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		if (GlobalObjectiveManager.Instance == null)
		{
			CommonHudController.Instance.TriggerLocked = false;
			CinematicHelper.Begin(false, false, false);
			GameController.Instance.OverrideTimePlayed(TimeSurvived);
			GameController.Instance.OnMissionFailed(null);
		}
		else
		{
			GlobalObjectiveManager.Instance.SectionFailedGameOver(null);
		}
	}

	private IEnumerator WarningDelay()
	{
		mWarningDelay = false;
		yield return new WaitForSeconds(20f);
		mWarningDelay = true;
	}

	private void CheckDifficultyFlow()
	{
		if (TimeSurvived - mStartingTime >= (float)GMGData.Instance.TTDifficultyRampTimeGap)
		{
			mStartingTime = TimeSurvived;
			if (mCurDifficultyLevel < GMGData.Instance.TTMaxDifficultyLevel)
			{
				mCurDifficultyLevel++;
				SetNextDifficultyLevel(mCurDifficultyLevel);
			}
		}
	}

	private void SetNextDifficultyLevel(int level)
	{
		int num = 0;
		int num2 = 0;
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		list.Add(0);
		List<int> list2 = list;
		switch (level)
		{
		case 0:
			num2 = GMGData.Instance.TTMaxWaveGroupsActive[0];
			list2[0] = GMGData.Instance.TTMaxSimGroupEnemies[level, 0];
			break;
		case 1:
			num2 = GMGData.Instance.TTMaxWaveGroupsActive[1];
			list2[0] = GMGData.Instance.TTMaxSimGroupEnemies[level, 0];
			list2[1] = GMGData.Instance.TTMaxSimGroupEnemies[level, 1];
			break;
		case 2:
			num2 = GMGData.Instance.TTMaxWaveGroupsActive[2];
			list2[0] = GMGData.Instance.TTMaxSimGroupEnemies[level, 0];
			list2[1] = GMGData.Instance.TTMaxSimGroupEnemies[level, 1];
			list2[2] = GMGData.Instance.TTMaxSimGroupEnemies[level, 2];
			break;
		case 3:
			num2 = GMGData.Instance.TTMaxWaveGroupsActive[3];
			list2[0] = GMGData.Instance.TTMaxSimGroupEnemies[level, 0];
			list2[1] = GMGData.Instance.TTMaxSimGroupEnemies[level, 1];
			list2[2] = GMGData.Instance.TTMaxSimGroupEnemies[level, 2];
			list2[3] = GMGData.Instance.TTMaxSimGroupEnemies[level, 3];
			break;
		case 4:
			num2 = GMGData.Instance.TTMaxWaveGroupsActive[4];
			list2[0] = GMGData.Instance.TTMaxSimGroupEnemies[level, 0];
			list2[1] = GMGData.Instance.TTMaxSimGroupEnemies[level, 1];
			list2[2] = GMGData.Instance.TTMaxSimGroupEnemies[level, 2];
			list2[3] = GMGData.Instance.TTMaxSimGroupEnemies[level, 3];
			list2[4] = GMGData.Instance.TTMaxSimGroupEnemies[level, 4];
			break;
		default:
			num2 = Coordinators.Count;
			list2[0] = GMGData.Instance.TTMaxSimGroupEnemies[5, 0];
			list2[1] = GMGData.Instance.TTMaxSimGroupEnemies[5, 1];
			list2[2] = GMGData.Instance.TTMaxSimGroupEnemies[5, 2];
			list2[3] = GMGData.Instance.TTMaxSimGroupEnemies[5, 3];
			list2[4] = GMGData.Instance.TTMaxSimGroupEnemies[5, 4];
			list2[5] = GMGData.Instance.TTMaxSimGroupEnemies[5, 5];
			if (Coordinators.Count > 6)
			{
				list2[6] = GMGData.Instance.TTMaxSimGroupEnemies[5, 6];
			}
			break;
		}
		foreach (SpawnerCoordinator mCoordinator in mCoordinators)
		{
			if (num > num2)
			{
			}
			mCoordinator.m_Interface.MaxSimultaneousEnemies = list2[num];
			num++;
		}
	}
}
