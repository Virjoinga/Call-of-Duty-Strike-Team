using System.Collections.Generic;
using UnityEngine;

public class MissionCompleteMenuNavigator : FrontEndScreen
{
	private enum Stage
	{
		NotStarted = 0,
		OverviewPanelAppears = 1,
		OverviewAndObjectives = 2,
		SoldierPanelsAppear = 3,
		KiaMiaStatusAppears = 4,
		ButtonsAppear = 5,
		FinalStage = 6
	}

	private enum Messaging
	{
		None = 0,
		LevelUp = 1,
		LevelUpDone = 2,
		RateTheApp = 3,
		KInvite = 4,
		KInviteDecline = 5,
		AllDone = 6
	}

	private const float DEFAULT_TIME_IN_STAGE = 0.15f;

	private const int NUM_TIP_MESSAGES = 10;

	public MissionCompleteSoldierPanelsSeqencer SoldierPanels;

	public OverviewAndObjectivesController OverviewPanel;

	public MenuScreenBlade Buttons;

	public MenuScreenBlade Tips;

	public MenuScreenBlade Leaderboard;

	public SpriteText ScreenSubTitle;

	public SpriteText SpecOpsTip;

	public GameObject AdditionalBladesRoot;

	public UIButton ContinueButton;

	public UIButton QuitReplayButton;

	public UIButton ShareButton;

	public float[] StageTimings;

	private MenuScreenBlade[] mAdditionalBlades;

	private bool[] mPurchasedSoldier;

	private PurchaseFlowHelper.PurchaseData mPurchaseData;

	private MissionData mMissionData;

	private MissionListings.eMissionID mMissionID;

	private Stage mCurrentStage;

	private Stage mNextStage;

	private Messaging mMessage;

	private float mTimeInThisStage;

	private int mSectionIndex;

	private int mPlayerLevelLastMessaged;

	private int mPlayerPrestigeLevelLastMessaged;

	private bool mSkipping;

	private bool mIsSpecOps;

	private bool mIsFlashpoint;

	public MiniLeaderboard FriendsLeaderboard;

	private SocialBroadcastDialogHelper m_DialogHelper;

	private SocialBroadcastDialogHelper m_LevelUpDialogHelper;

	protected override void Awake()
	{
		ID = ScreenID.MissionComplete;
		base.Awake();
		if (AdditionalBladesRoot != null)
		{
			mAdditionalBlades = AdditionalBladesRoot.GetComponentsInChildren<MenuScreenBlade>();
		}
		mCurrentStage = Stage.NotStarted;
		mNextStage = Stage.NotStarted;
		mTimeInThisStage = 0f;
		mSkipping = false;
		mIsSpecOps = ActStructure.Instance.CurrentMissionIsSpecOps();
		mIsFlashpoint = ActStructure.Instance.CurrentMissionType() == GMGData.GameType.Flashpoint;
		int xp = StatsHelper.PlayerXP();
		XPManager instance = XPManager.Instance;
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		instance.ConvertXPToLevel(xp, out level, out prestigeLevel, out xpToNextLevel, out percent);
		mPlayerLevelLastMessaged = level;
		mPlayerPrestigeLevelLastMessaged = prestigeLevel;
		if (m_DialogHelper == null)
		{
			m_DialogHelper = base.gameObject.AddComponent<SocialBroadcastDialogHelper>();
		}
		if (m_LevelUpDialogHelper == null)
		{
			m_LevelUpDialogHelper = base.gameObject.AddComponent<SocialBroadcastDialogHelper>();
		}
		mPurchaseData = new PurchaseFlowHelper.PurchaseData();
		mPurchaseData.ScriptToCallWithResult = this;
		mPurchaseData.MethodToCallWithResult = "ConfirmSoldierXpPurchase";
		mPurchaseData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.KiaSoldier;
		mPurchaseData.ConfirmPurchase = true;
		m_DialogHelper.AllowedToPost = true;
		m_LevelUpDialogHelper.AllowedToPost = true;
		if (ShareButton != null && !OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.GByteDevice))
		{
			ShareButton.gameObject.SetActive(false);
		}
		GameObject gameObject = GameObject.Find("InformationButton");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}

	private bool ResultsForFailedMission()
	{
		string id = StatsManager.MissionStatId(mMissionID, mSectionIndex);
		Debug.Log("====> ID " + mMissionID);
		Debug.Log("====> Succeeded " + StatsManager.Instance.MissionStats().GetCurrentMissionStat(id).NumTimesSucceeded);
		return StatsManager.Instance.MissionStats().GetCurrentMissionStat(id).NumTimesSucceeded == 0;
	}

	private bool ResultsForFinishedMission()
	{
		int count = mMissionData.Sections.Count;
		if (mSectionIndex == count - 1)
		{
			return true;
		}
		if (mMissionData.Sections[mSectionIndex + 1].IsSpecOps)
		{
			return true;
		}
		return false;
	}

	protected override void Start()
	{
		base.Start();
	}

	private void OnEnable()
	{
		EnableAllAdditionalBlades(true);
	}

	private void OnDisable()
	{
		EnableAllAdditionalBlades(false);
	}

	protected override void Update()
	{
		base.Update();
		if (mCurrentStage != mNextStage && !mSkipping)
		{
			mTimeInThisStage += TimeManager.DeltaTime;
			float num = 0.15f;
			if ((int)mCurrentStage < StageTimings.Length)
			{
				num = StageTimings[(int)mCurrentStage];
			}
			if (mTimeInThisStage >= num)
			{
				NextStage();
			}
		}
		if (mCurrentStage == Stage.FinalStage)
		{
			UpdateMessaging();
		}
	}

	public void UpdateMessaging()
	{
		MessageBoxController instance = MessageBoxController.Instance;
		if (!(instance != null) || instance.IsAnyMessageActive)
		{
			return;
		}
		if (mMessage == Messaging.None || mMessage == Messaging.LevelUpDone)
		{
			int xp = StatsHelper.PlayerXP();
			XPManager instance2 = XPManager.Instance;
			int level = 0;
			int prestigeLevel = 0;
			float percent = 0f;
			int xpToNextLevel = 0;
			instance2.ConvertXPToLevel(xp, out level, out prestigeLevel, out xpToNextLevel, out percent);
			int maxLevel = instance2.m_XPLevels.Count - 1;
			if (mPlayerLevelLastMessaged != level || mPlayerPrestigeLevelLastMessaged != prestigeLevel)
			{
				if (!GameSettings.Instance.LevelUpTutorialSeen)
				{
					instance.DoLevelUpTutorialDialogue();
					GameSettings.Instance.LevelUpTutorialSeen = true;
					return;
				}
				if (mPlayerPrestigeLevelLastMessaged != prestigeLevel)
				{
					m_LevelUpDialogHelper.AllowedToPost = true;
					instance.DoPrestigeDialogue(this, "MessageBoxResultDismissLevelUp", "MessageBoxResultShareLevelUp", mPlayerLevelLastMessaged, level, prestigeLevel, maxLevel);
					ReportSwrveLevels(mPlayerLevelLastMessaged, level);
					mPlayerLevelLastMessaged = level;
					mPlayerPrestigeLevelLastMessaged = prestigeLevel;
					SwrveEventsProgression.PlayerPrestiged(mPlayerPrestigeLevelLastMessaged);
				}
				else
				{
					m_LevelUpDialogHelper.AllowedToPost = true;
					instance.DoLevelUpDialogue(this, "MessageBoxResultDismissLevelUp", "MessageBoxResultShareLevelUp", mPlayerLevelLastMessaged, level, mPlayerPrestigeLevelLastMessaged, maxLevel);
					ReportSwrveLevels(mPlayerLevelLastMessaged, level);
					mPlayerLevelLastMessaged = level;
				}
				mMessage = Messaging.LevelUp;
				return;
			}
			StatsManager instance3 = StatsManager.Instance;
			if (instance3 != null)
			{
				List<Perk> list = new List<Perk>();
				Perk[] perks = instance3.PerksList.Perks;
				if (perks != null)
				{
					for (int i = 0; i < perks.Length; i++)
					{
						if (StatsHelper.DidPerkReachPro(perks[i].Identifier))
						{
							list.Add(perks[i]);
						}
					}
				}
				if (list.Count > 0)
				{
					instance.DoProPerkDialogue(this, "MessageBoxResultShareProPerkUpgrade", list.ToArray());
				}
			}
			mMessage = Messaging.RateTheApp;
		}
		else if (mMessage == Messaging.RateTheApp)
		{
			int num = StatsHelper.MissionsSuccessful();
			int num2 = 0;
			int num3 = 0;
			if (SwrveServerVariables.Instance != null)
			{
				num2 = SwrveServerVariables.Instance.RateAppFirstTrigger;
				num3 = SwrveServerVariables.Instance.RateAppTriggerInterval;
			}
			if (num3 == 0)
			{
				num3 = 1;
			}
			if (instance != null && !GeneralSettings.Instance.HasRatedApp && (num + num2) % num3 == 0 && StatsHelper.CurrentMissionSuccessful() && !TBFUtils.IsUnsupportedDevice() && !OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.DisableRateTheApp))
			{
				instance.DoRateAppDialogue();
			}
			mMessage = Messaging.KInvite;
		}
		else if (mMessage == Messaging.KInvite)
		{
			int num4 = StatsHelper.MissionsSuccessful();
			int num5 = 0;
			int num6 = 0;
			bool flag = false;
			if (SwrveServerVariables.Instance != null)
			{
				num5 = SwrveServerVariables.Instance.KInviteFirstTrigger;
				num6 = SwrveServerVariables.Instance.KInviteTriggerInterval;
				flag = SwrveServerVariables.Instance.KInviteEnabled;
			}
			if (num6 == 0)
			{
				num6 = 1;
			}
			if (instance != null && !GeneralSettings.Instance.HasKInvited && (num4 + num5) % num6 == 0 && StatsHelper.CurrentMissionSuccessful() && flag && !TBFUtils.IsUnsupportedDevice() && OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.EnableKInvite))
			{
				instance.DoKInviteDialogue();
			}
			mMessage = Messaging.AllDone;
			SwrveEventsUI.SwrveTalkTrigger_Results();
		}
	}

	public void ReportSwrveLevels(int levelFrom, int levelTo)
	{
		do
		{
			if (levelFrom == XPManager.Instance.m_XPLevels.Count - 1)
			{
				levelFrom = 0;
			}
			SwrveEventsProgression.PlayerLevelUp(levelFrom + 1);
			levelFrom++;
		}
		while (levelFrom != levelTo);
	}

	public override void EnterScreen()
	{
		base.EnterScreen();
		TitleBarController instance = TitleBarController.Instance;
		string customTitle = string.Empty;
		if (mCurrentStage == Stage.NotStarted)
		{
			SoundManager.Instance.ActivateMissionComplete();
			FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
			if (FrontEndController.Instance.PreviousScreen == ScreenID.None)
			{
				MusicManager.Instance.FadeOutCurrentMusic(2f);
				if (instance != null)
				{
					instance.DisableMTXButton();
				}
			}
			HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
			BlackBarsController.Instance.SetBlackBars(false, false);
			GameSettings instance2 = GameSettings.Instance;
			mPurchasedSoldier = new bool[instance2.Soldiers.Length];
			MissionListings instance3 = MissionListings.Instance;
			ActStructure instance4 = ActStructure.Instance;
			mMissionID = ((instance4.CurrentMissionID == MissionListings.eMissionID.MI_MAX) ? instance4.LastMissionID : instance4.CurrentMissionID);
			mSectionIndex = instance4.CurrentSection;
			mMissionData = instance3.Mission(mMissionID);
			if (FriendsLeaderboard != null)
			{
				int leaderboardId = MissionListings.Instance.LeaderboardIDForMissionSection(mMissionID, mSectionIndex);
				FriendsLeaderboard.RefreshFriendsAndHighScores(leaderboardId, ActStructure.Instance.MissionIsSpecOps(mMissionID, mSectionIndex));
			}
			if (ResultsForFailedMission())
			{
				MusicManager.Instance.PlayMissionFailMusic();
			}
			else
			{
				SecureStorage.Instance.LogMissionCompletedSinceActivateDialog();
				MusicManager.Instance.PlayMissionPassMusic();
			}
			SetupQuitRestartContinue();
			if (ScreenSubTitle != null)
			{
				bool flag = StatsHelper.CurrentMissionSuccessful();
				string text = string.Empty;
				if (!flag)
				{
					ObjectiveManager objectiveManager = Object.FindObjectOfType(typeof(ObjectiveManager)) as ObjectiveManager;
					if (objectiveManager != null)
					{
						for (int i = 0; i < objectiveManager.Objectives.Count; i++)
						{
							MissionObjective missionObjective = objectiveManager.Objectives[i];
							if (missionObjective != null && missionObjective.m_Interface.IsVisible && (missionObjective.State == MissionObjective.ObjectiveState.Failed || missionObjective.State == MissionObjective.ObjectiveState.InProgress))
							{
								text = Language.Get(missionObjective.m_Interface.ObjectiveLabel).ToUpper();
								break;
							}
						}
					}
					if (text == string.Empty)
					{
						text = Language.Get("S_GENERIC_FAIL_REASON");
					}
				}
				ScreenSubTitle.SetColor((!flag && (!mIsSpecOps || mIsFlashpoint)) ? ColourChart.ObjectiveFailed : ColourChart.ObjectiveSuccess);
				string text2 = Language.Get(mIsSpecOps ? "S_RESULTS_GMG_TITLE" : ((!flag) ? "S_RESULT_FAIL" : "S_RESULT_PASS"));
				if (mIsSpecOps)
				{
					GMGData instance5 = GMGData.Instance;
					if (instance5 != null)
					{
						if (instance5.CurrentGameType == GMGData.GameType.Specops || instance5.CurrentGameType == GMGData.GameType.Total)
						{
							int num = instance5.CurrentWave();
							text2 = string.Format(text2, num);
						}
						else if (mIsFlashpoint)
						{
							text2 = Language.Get((!flag) ? "S_FL_RESULTS_TITLE_01_FAIL" : "S_FL_RESULTS_TITLE_01_PASS");
						}
						else
						{
							StatsManager instance6 = StatsManager.Instance;
							if (instance6 != null)
							{
								MissionStat currentMissionStat = instance6.MissionStats().GetCurrentMissionStat(mMissionData.MissionId, mSectionIndex);
								if (currentMissionStat != null)
								{
									text2 = Language.GetFormatString("S_GMG_TIMERESULT", StatsHelper.MinutesToString(currentMissionStat.TimePlayed));
									text2 = text2.ToUpper();
								}
							}
						}
					}
				}
				else if (!flag && text != string.Empty)
				{
					text2 = text2 + ": " + text;
				}
				ScreenSubTitle.Text = text2;
			}
		}
		if (mMissionData != null)
		{
			customTitle = AutoLocalize.Get(mMissionData.NameKey).ToUpper();
		}
		if (instance != null)
		{
			instance.SetCustomTitle(customTitle);
		}
	}

	public override void ExitScreen()
	{
		base.ExitScreen();
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	public override void OnScreen()
	{
		if (FrontEndController.Instance.PreviousScreen != ScreenID.MTXSelect)
		{
			SceneNanny.EndMainGame();
			TimeManager.instance.PauseGame();
			mNextStage = Stage.OverviewPanelAppears;
			NextStage();
			return;
		}
		for (int i = 0; i < base.Blades.Length; i++)
		{
			if (base.Blades[i] != null)
			{
				BoxCollider[] componentsInChildren = base.Blades[i].GetComponentsInChildren<BoxCollider>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].enabled = !componentsInChildren[j].enabled;
					componentsInChildren[j].enabled = !componentsInChildren[j].enabled;
				}
			}
		}
	}

	public void OnReplayButtonPressed()
	{
		if (ResultsForFailedMission())
		{
			FrontEndController.Instance.ConfirmRestart();
		}
		else
		{
			FrontEndController.Instance.ConfirmQuit();
		}
	}

	public void OnShareButtonPressed()
	{
		MessageBoxController instance = MessageBoxController.Instance;
		if (instance != null && mMissionID != MissionListings.MI_MISSION_MAX && mMissionData != null)
		{
			SectionData sectionData = mMissionData.Sections[mSectionIndex];
			string text = Language.Get(sectionData.Name);
			int num = StatsHelper.CurrentMissionScore();
			if (ResultsForFailedMission())
			{
				string formatString = Language.GetFormatString("S_FAILEDMISSION_SOCIAL_MESSAGE", num, text);
				m_DialogHelper.PostMessage(formatString, this, "OnFacebookPost", "OnTwitterPost");
			}
			else
			{
				string formatString2 = Language.GetFormatString("S_PASSEDMISSION_SOCIAL_MESSAGE", text, num);
				m_DialogHelper.PostMessage(formatString2, this, "OnFacebookPost", "OnTwitterPost");
			}
		}
	}

	public void MessageBoxResultShareProPerkUpgrade()
	{
		MessageBoxController instance = MessageBoxController.Instance;
		if (instance != null)
		{
			string message = Language.Get("S_PROPERK_SOCIAL_MESSAGE");
			m_DialogHelper.PostMessage(message, this, "OnFacebookPost", "OnTwitterPost");
		}
	}

	public void OnFacebookPost()
	{
		SwrveEventsMetaGame.SharedFromResults(SwrveEventsMetaGame.ShareType.Facebook, mMissionID);
		SwrveEventsMetaGame.FacebookBroadcast("ResultsScreen", mMissionID);
	}

	public void OnTwitterPost()
	{
		SwrveEventsMetaGame.SharedFromResults(SwrveEventsMetaGame.ShareType.Twitter, mMissionID);
		SwrveEventsMetaGame.TwitterBroadcast("ResultsScreen", mMissionID);
	}

	public void SetupQuitRestartContinue()
	{
		if (!ResultsForFailedMission() && ResultsForFinishedMission() && mMissionID == MissionListings.eMissionID.MI_MISSION_KOWLOON && !ActStructure.Instance.MissionIsSpecOps(mMissionID, mSectionIndex))
		{
			ContinueButton.Text = Language.Get("S_CONTINUE");
			ContinueButton.scriptWithMethodToInvoke = this;
			ContinueButton.methodToInvoke = "ContinueAfterLastMission";
			QuitReplayButton.gameObject.SetActive(false);
		}
		else if (ResultsForFailedMission())
		{
			QuitReplayButton.Text = Language.Get("S_QUIT");
			QuitReplayButton.scriptWithMethodToInvoke = this;
			QuitReplayButton.methodToInvoke = "ReturnToMainMenu";
			ContinueButton.Text = Language.Get("S_REPLAY");
			ContinueButton.scriptWithMethodToInvoke = this;
			ContinueButton.methodToInvoke = "RestartMission";
		}
		else
		{
			QuitReplayButton.Text = Language.Get("S_QUIT");
			QuitReplayButton.scriptWithMethodToInvoke = this;
			QuitReplayButton.methodToInvoke = "ReturnToMainMenu";
			ContinueButton.Text = Language.Get("S_CONTINUE");
			ContinueButton.scriptWithMethodToInvoke = this;
			ContinueButton.methodToInvoke = "ContinueAfterSuccess";
		}
	}

	private void ReturnToMainMenu()
	{
		FrontEndController.Instance.ConfirmContinue();
	}

	private void RestartMission()
	{
		FrontEndController.Instance.ConfirmRestart();
	}

	private void ContinueAfterSuccess()
	{
		GameSettings.LaunchedFromGlobe = false;
		FrontEndController.Instance.ConfirmNextSection();
	}

	private void ContinueAfterLastMission()
	{
		FrontEndController.Instance.ConfirmEpilogue();
	}

	public void SoldierPanel1Pressed()
	{
		PressedSoldier(0);
	}

	public void SoldierPanel2Pressed()
	{
		PressedSoldier(1);
	}

	public void SoldierPanel3Pressed()
	{
		PressedSoldier(2);
	}

	public void SoldierPanel4Pressed()
	{
		PressedSoldier(3);
	}

	private void PressedSoldier(int index)
	{
		GameSettings instance = GameSettings.Instance;
		if ((bool)instance && mCurrentStage == Stage.FinalStage)
		{
			string id = ((!instance.VtolSoldierPresent) ? StatsManager.ConvertSoldierIndexToId(index) : StatsManager.ConvertSoldierIndexToId(4));
			bool flag = StatsHelper.WasCharacterKIA(id);
			int xPFromKillsForCharacter = StatsHelper.GetXPFromKillsForCharacter(id);
			int xPFromBonusesForCharacter = StatsHelper.GetXPFromBonusesForCharacter(id);
			if (flag && (xPFromKillsForCharacter != 0 || xPFromBonusesForCharacter != 0) && !mPurchasedSoldier[index])
			{
				mPurchaseData.SoldierIndex = ((!instance.VtolSoldierPresent) ? index : 4);
				mPurchaseData.SlotIndex = index;
				PurchaseFlowHelper.Instance.Purchase(mPurchaseData);
			}
		}
	}

	private void ConfirmSoldierXpPurchase()
	{
		PurchaseSoldier(mPurchaseData.SlotIndex, mPurchaseData.SoldierIndex);
	}

	private void PurchaseSoldier(int index, int statsIndex)
	{
		mPurchasedSoldier[index] = true;
		int xp = StatsManager.Instance.PlayerStats().PlayerPurchasedSoldierXP(statsIndex);
		SoldierPanels.PlayerPurchasedSoldierXP(index);
		OverviewPanel.UpdateTheTotalXP();
		SwrveEventsPurchase.XPRecover(xp);
	}

	public void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		if (mCurrentStage != Stage.FinalStage)
		{
			mSkipping = true;
			DoAllStagesNow();
		}
		else
		{
			OverviewPanel.OnTap(fingerPos);
		}
	}

	private void NextStage()
	{
		mCurrentStage = mNextStage;
		mTimeInThisStage = 0f;
		if (mCurrentStage != Stage.FinalStage)
		{
			mNextStage++;
		}
		Invoke("Do" + mCurrentStage, 0f);
	}

	private void DoOverviewPanelAppears()
	{
		if (mIsSpecOps)
		{
			MoveBladeOn(Tips, GetTimeForStage(mCurrentStage));
			if (SpecOpsTip != null)
			{
				int num = Random.Range(1, 10);
				SpecOpsTip.Text = Language.Get("S_LOADING_SCREEN_TIP_GMG_" + num.ToString("D2"));
			}
		}
	}

	private void DoOverviewAndObjectives()
	{
		OverviewPanel.BeginSequence(GetTimeForStage(mCurrentStage));
	}

	private void DoSoldierPanelsAppear()
	{
		SoldierPanels.BeginOnSequence(GetTimeForStage(mCurrentStage));
	}

	private void DoKiaMiaStatusAppears()
	{
		SoldierPanels.BeginMiaKiaSequence(GetTimeForStage(mCurrentStage));
	}

	private void DoButtonsAppear()
	{
		float timeForStage = GetTimeForStage(mCurrentStage);
		MoveBladeOn(Buttons, timeForStage);
	}

	private void DoFinalStage()
	{
		AddAllBladesToScreenNow();
		TitleBarController instance = TitleBarController.Instance;
		if (instance != null)
		{
			instance.EnableMTXButton();
		}
	}

	private void DoAllStagesNow()
	{
		while (mCurrentStage != Stage.FinalStage)
		{
			NextStage();
		}
	}

	public void BladeOn(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
		if (blade != null)
		{
			BoxCollider[] componentsInChildren = blade.GetComponentsInChildren<BoxCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = !componentsInChildren[i].enabled;
				componentsInChildren[i].enabled = !componentsInChildren[i].enabled;
			}
		}
	}

	private void MoveBladeOn(MenuScreenBlade blade, float timeInStage)
	{
		if (blade != null)
		{
			blade.Activate(BladeOn, timeInStage);
		}
	}

	private void MoveBladeToOffset(MenuScreenBlade blade, float timeInStage)
	{
		if (blade != null)
		{
			blade.MoveToOffset(timeInStage);
		}
	}

	private float GetTimeForStage(Stage stage)
	{
		float result = 0.15f;
		if ((int)stage < StageTimings.Length && !mSkipping)
		{
			result = StageTimings[(int)stage];
		}
		return result;
	}

	private void MessageBoxResultDismissLevelUp()
	{
		mMessage = Messaging.LevelUpDone;
	}

	private void MessageBoxResultShareLevelUp()
	{
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		XPManager.Instance.ConvertXPToLevel(StatsHelper.PlayerXP(), out level, out prestigeLevel, out xpToNextLevel, out percent);
		string text = Language.Get("S_XPLEVEL_" + level);
		if (prestigeLevel == 0)
		{
			string formatString = Language.GetFormatString("S_LEVELUP_SOCIAL_MESSAGE", text);
			m_LevelUpDialogHelper.PostMessage(formatString, this, "OnLevelUpFacebookPost", "OnLevelUpTwitterPost");
		}
		else
		{
			string formatString2 = Language.GetFormatString("S_PRESTIGE_SOCIAL_MESSAGE", prestigeLevel.ToString(), text);
			m_LevelUpDialogHelper.PostMessage(formatString2, this, "OnLevelUpFacebookPost", "OnLevelUpTwitterPost");
		}
		mMessage = Messaging.LevelUpDone;
	}

	public void OnLevelUpFacebookPost()
	{
		SwrveEventsMetaGame.SharedFromLevelUp(SwrveEventsMetaGame.ShareType.Facebook);
		SwrveEventsMetaGame.FacebookBroadcast("LevelUp", mMissionID);
	}

	public void OnLevelUpTwitterPost()
	{
		SwrveEventsMetaGame.SharedFromLevelUp(SwrveEventsMetaGame.ShareType.Twitter);
		SwrveEventsMetaGame.TwitterBroadcast("LevelUp", mMissionID);
	}

	private void AddAllBladesToScreenNow()
	{
		GameSettings instance = GameSettings.Instance;
		List<MenuScreenBlade> list = new List<MenuScreenBlade>();
		list.AddRange(base.Blades);
		for (int i = 0; i < mAdditionalBlades.Length; i++)
		{
			bool flag = mAdditionalBlades[i].name.Contains("Tip");
			if (mAdditionalBlades[i].name.Contains("Buttons") || (mIsSpecOps && flag))
			{
				list.Add(mAdditionalBlades[i]);
			}
			else if (mAdditionalBlades[i].name.Contains("SoldierPanel") && !mIsSpecOps)
			{
				string s = mAdditionalBlades[i].name.Replace("SoldierPanel", string.Empty);
				int num = int.Parse(s) - 1;
				if ((!instance.VtolSoldierPresent || num == 0) && num >= 0 && num < instance.Soldiers.Length && instance.Soldiers[num].Present)
				{
					list.Add(mAdditionalBlades[i]);
				}
			}
		}
		base.Blades = list.ToArray();
	}

	private void EnableAllAdditionalBlades(bool enable)
	{
		if (mAdditionalBlades == null)
		{
			return;
		}
		for (int i = 0; i < mAdditionalBlades.Length; i++)
		{
			if (mAdditionalBlades[i] != null)
			{
				mAdditionalBlades[i].gameObject.SetActive(enable);
			}
		}
	}
}
