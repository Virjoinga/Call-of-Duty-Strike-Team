using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MissionOverviewController : MenuScreenBlade
{
	private const float TIME_BETWEEN_ADDS = 0.1f;

	private const float ITEM_WIDTH_OFFSET = 60f;

	private const float ITEM_HEIGHT = 180f;

	private const float SCROLL_TIME = 0.5f;

	public SectionSelectPanel SectionSelectPanelPrefab;

	public MiniLeaderboard FriendsLeaderboard;

	public SpriteText StatusMessage;

	public SpriteText MissionNameText;

	public SpriteText MissionDescText;

	public SpriteText FlashpointTimerText;

	public SpriteText FlashpointScoreText;

	public SpriteText FlashpointScoreValueText;

	public LoadoutSideMenu DifficultySideMenu;

	public MenuScreenBlade SectionDataPanel;

	public FrontEndButton DeployButton;

	public FrontEndButton ShareButton;

	private List<UIListItemContainer> mPanelsToAdd;

	private SectionSelectPanel[] mSectionPanels;

	private UIScrollList mScrollList;

	private SelectableMissionMarker mData;

	private SelectableMissionMarker mLoadData;

	private SocialBroadcastDialogHelper m_DialogHelper;

	private AnimateCommonBackgroundBox mAnimation;

	private SectionData mCurrentlySelectedData;

	private CommonBackgroundBoxPlacement mSectionListPlacement;

	private NumberFormatInfo mNfi;

	private PurchaseFlowHelper.PurchaseData mPurchaseData;

	private float mTimeSinceLastAdd;

	private float mItemWidthOffset;

	private float mWidth;

	private float mHeight;

	private int mSelectedSection;

	private int mLoadSection;

	private DifficultyMode mDifficulty;

	private bool mLoading;

	private bool mBlipWasHighlighted;

	public DifficultyMode Difficulty
	{
		get
		{
			if (mDifficulty != 0)
			{
				MissionData data = mData.Data;
				SectionData sectionData = data.Sections[mSelectedSection];
				return (!sectionData.IsSpecOps && !sectionData.IsTutorial) ? mDifficulty : DifficultyMode.Regular;
			}
			return mDifficulty;
		}
		private set
		{
			mDifficulty = value;
		}
	}

	public void SetData(SelectableMissionMarker data)
	{
		mData = data;
		if (GameSettings.Instance.HighlightMissionID == mData.Data.MissionId)
		{
			mSelectedSection = GameSettings.Instance.HighlightSectionIndex;
			mSelectedSection = Mathf.Min(mSelectedSection, data.Data.Sections.Count - 1);
		}
		else
		{
			mSelectedSection = 0;
		}
		if (data.Data.Sections.Count > mSelectedSection)
		{
			mCurrentlySelectedData = data.Data.Sections[mSelectedSection];
		}
		mSectionPanels = new SectionSelectPanel[mData.Data.NumValidSections()];
		SwrveEventsUI.Viewed(data.Data.MissionId);
		PopulatePanelsToAdd();
	}

	private int CountSinglePlayerSectionsForMission(MissionData.eSectionImages sectionImageSet)
	{
		MissionListings instance = MissionListings.Instance;
		int num = 0;
		if (instance != null)
		{
			for (int i = 0; i < instance.Missions.Length; i++)
			{
				MissionData missionData = instance.Missions[i];
				if (missionData != null && missionData.Type == MissionData.eType.MT_STORY && missionData.SectionSelectImage == sectionImageSet)
				{
					num += missionData.Sections.Count;
				}
			}
		}
		return num;
	}

	private void PopulatePanelsToAdd()
	{
		if (!(mData != null) || mPanelsToAdd.Count != 0)
		{
			return;
		}
		List<SectionSelectPanel> list = new List<SectionSelectPanel>();
		int numSinglePlayerSections = CountSinglePlayerSectionsForMission(mData.Data.SectionSelectImage);
		DifficultyMode difficulty = Difficulty;
		for (int i = 0; i < mData.Data.Sections.Count; i++)
		{
			SectionData sectionData = mData.Data.Sections[i];
			if (sectionData.IsValidInCurrentBuild)
			{
				SectionSelectPanel item = AddSection(i, numSinglePlayerSections, difficulty, mData.Data, sectionData);
				list.Add(item);
			}
		}
		mSectionPanels = list.ToArray();
		if (mSelectedSection < mSectionPanels.Length && mSectionPanels[mSelectedSection] != null)
		{
			mSectionPanels[mSelectedSection].SetSelected(true);
		}
	}

	private SectionSelectPanel AddSection(int section, int numSinglePlayerSections, DifficultyMode difficulty, MissionData md, SectionData sd)
	{
		SectionSelectPanel sectionSelectPanel = (SectionSelectPanel)UnityEngine.Object.Instantiate(SectionSelectPanelPrefab);
		sectionSelectPanel.LayoutComponents(mWidth, mHeight, mSectionListPlacement);
		sectionSelectPanel.Setup(section, numSinglePlayerSections, md, difficulty, SectionSelectedChanged);
		sectionSelectPanel.gameObject.transform.position = new Vector3(-200f, 0f, 0f);
		UIListItemContainer component = sectionSelectPanel.GetComponent<UIListItemContainer>();
		mPanelsToAdd.Add(component);
		return sectionSelectPanel;
	}

	public override void Awake()
	{
		base.Awake();
		Difficulty = GameSettings.Instance.LastKnownDifficultyMode;
		mLoading = false;
		mNfi = GlobalizationUtils.GetNumberFormat(0);
		mPurchaseData = new PurchaseFlowHelper.PurchaseData();
		mPurchaseData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Mission;
		mPurchaseData.NumItems = 1;
		mPurchaseData.ScriptToCallWithResult = this;
		mPurchaseData.MethodToCallWithResult = "RefreshSectionData";
		int num = 1;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num = 2;
		}
		mItemWidthOffset = 60f * (float)num;
		mHeight = 180f * (float)num;
		mPanelsToAdd = new List<UIListItemContainer>();
		mAnimation = GetComponentInChildren<AnimateCommonBackgroundBox>();
		mScrollList = GetComponentInChildren<UIScrollList>();
		if (mScrollList != null)
		{
			mScrollList.scriptWithMethodToInvoke = this;
			mScrollList.methodToInvokeOnSelect = "SectionPanelPressed";
			SoundManager.Instance.SetUIScrollListScrollSFX(mScrollList);
			if (mScrollList.transform.parent != null)
			{
				mSectionListPlacement = mScrollList.transform.parent.GetComponent<CommonBackgroundBoxPlacement>();
			}
		}
		if (mSectionListPlacement != null)
		{
			mWidth = mSectionListPlacement.BoundingRect.width - mItemWidthOffset;
		}
		if (m_DialogHelper == null)
		{
			m_DialogHelper = base.gameObject.AddComponent<SocialBroadcastDialogHelper>();
		}
		m_DialogHelper.AllowedToPost = true;
		if (!TBFUtils.UseAlternativeLayout())
		{
			return;
		}
		GameObject gameObject = GameObject.Find("FriendsInviteButton");
		if (gameObject != null)
		{
			CommonBackgroundBoxPlacement componentInChildren = gameObject.GetComponentInChildren<CommonBackgroundBoxPlacement>();
			if (componentInChildren != null)
			{
				componentInChildren.StartPositionAsPercentageOfBoxHeight = 0.72f;
				componentInChildren.StartPositionAsPercentageOfBoxWidth = 0.55f;
			}
		}
	}

	private void OnEnable()
	{
		ActivateWatcher.DataLoadedAfterLogin += HandleDataLoadedAfterLogin;
		ActivateWatcher.ActivateUILaunched += OnActivateUILaunched;
	}

	private void OnDisable()
	{
		BedrockWorker.Instance.InvalidateGotleaderboardCallback();
		ActivateWatcher.DataLoadedAfterLogin -= HandleDataLoadedAfterLogin;
		ActivateWatcher.ActivateUILaunched -= OnActivateUILaunched;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		PopulatePanelsToAdd();
		RefreshNameAndDescription();
		RefreshFriendsAndHighScores();
		if (DifficultySideMenu != null)
		{
			DifficultySideMenu.SetSelected((int)Difficulty);
			DifficultySideMenu.SetDisabled(XPManager.Instance.GetXPLevelAbsolute() < XPManager.Instance.XPLevelForVeteranModeUnlock, 1, "S_VETERAN_MODE_LOCKED_BODY");
			SetDifficultyButtonVisibility();
		}
		if (mData != null)
		{
			mBlipWasHighlighted = mData.IsBlipHighlighted();
			mData.SetBlipHighlighted(false);
		}
		TitleBarController instance = TitleBarController.Instance;
		if (mData != null && mData.Data != null)
		{
			instance.SetCustomTitle(AutoLocalize.Get(mData.Data.NameKey).ToUpper());
		}
		mLoading = false;
		SwrveEventsUI.SwrveTalkTrigger_MissionSelect();
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (mData != null)
		{
			mData.SetBlipHighlighted(mBlipWasHighlighted);
		}
		Clear();
	}

	public override void OnScreen()
	{
		base.OnScreen();
		if (mCurrentlySelectedData != null && mCurrentlySelectedData.IsSpecOps && mCurrentlySelectedData.GMGGameType == GMGData.GameType.Flashpoint && !SecureStorage.Instance.HasSeenFlashpointTutorial)
		{
			SecureStorage.Instance.HasSeenFlashpointTutorial = true;
			MessageBoxController instance = MessageBoxController.Instance;
			if (instance != null)
			{
				instance.DoFrontendHintDialogue("S_FL_INTRO_MAIN_TITLE", "S_FL_INTRO_MAIN_BODY", HintMessageBox.ImageLayout.Left, "flashpoint/flashpoint_intro", null, string.Empty);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (FlashpointTimerText != null && mCurrentlySelectedData != null)
		{
			if (mCurrentlySelectedData.IsSpecOps && mCurrentlySelectedData.GMGGameType == GMGData.GameType.Flashpoint)
			{
				if (mData != null && mData.Data != null)
				{
					long num = GlobalUnrestController.Instance.GetTimeRemainingInSeconds(mData.Data.FlashPointIndex);
					FlashpointTimerText.Text = TimeUtils.GetShortTimeStringFromSeconds(num);
					if (num < 3600)
					{
						FlashpointTimerText.SetColor(ColourChart.HudRed);
					}
					else
					{
						FlashpointTimerText.SetColor(ColourChart.HudBlue);
					}
				}
			}
			else
			{
				FlashpointTimerText.Text = string.Empty;
			}
		}
		if (!(mScrollList != null) || !(mAnimation != null) || !mAnimation.IsOpen)
		{
			return;
		}
		mTimeSinceLastAdd += TimeManager.DeltaTime;
		if (mTimeSinceLastAdd >= 0.1f && mPanelsToAdd.Count > 0)
		{
			mScrollList.InsertItem(mPanelsToAdd[0], mScrollList.Count);
			mPanelsToAdd.RemoveAt(0);
			mTimeSinceLastAdd = 0f;
			if (mSelectedSection < mScrollList.Count)
			{
				mScrollList.ScrollToItem(mSelectedSection, 0.5f);
			}
		}
	}

	public void RefreshScreenAfterSectionChange(MenuScreenBlade blade, BladeTransition type)
	{
		if (!base.IsActive || base.IsTransitioning)
		{
			return;
		}
		RefreshNameAndDescription();
		RefreshHighScores();
		SectionDataPanel.Activate();
		if (DifficultySideMenu != null)
		{
			DifficultySideMenu.SetSelected((int)Difficulty);
		}
		for (int i = 0; i < mSectionPanels.Length; i++)
		{
			if (mSectionPanels[i] != null)
			{
				mSectionPanels[i].UpdateDifficulty(Difficulty);
			}
		}
		SetDifficultyButtonVisibility();
	}

	private void SetDifficultyButtonVisibility()
	{
		if (!mData.Data.Sections[mSelectedSection].IsSpecOps && !mData.Data.Sections[mSelectedSection].IsTutorial)
		{
			DifficultySideMenu.HideButton(0, false);
			DifficultySideMenu.HideButton(1, false);
		}
		else
		{
			DifficultySideMenu.HideButton(0, true);
			DifficultySideMenu.HideButton(1, true);
		}
	}

	public void SectionSelectedChanged(SectionData data, bool forceRefresh)
	{
		if (!SectionDataPanel || (data == mCurrentlySelectedData && !forceRefresh) || !SectionDataPanel.IsActive)
		{
			return;
		}
		mCurrentlySelectedData = data;
		if (mSelectedSection < mSectionPanels.Length && mSectionPanels[mSelectedSection] != null)
		{
			mSectionPanels[mSelectedSection].SetSelected(false);
		}
		for (int i = 0; i < mData.Data.Sections.Count; i++)
		{
			if (mData.Data.Sections[i] == data)
			{
				mSelectedSection = i;
				break;
			}
		}
		if (mSelectedSection < mSectionPanels.Length && mSectionPanels[mSelectedSection] != null)
		{
			mSectionPanels[mSelectedSection].SetSelected(true);
			MenuSFX.Instance.SoftSelect.Play2D();
		}
		if (mScrollList != null && mSelectedSection < mScrollList.Count)
		{
			mScrollList.ScrollToItem(mSelectedSection, 0.5f);
		}
		if (SectionDataPanel != null)
		{
			SectionDataPanel.Deactivate(RefreshScreenAfterSectionChange);
		}
	}

	private void Clear()
	{
		if (mScrollList != null)
		{
			mScrollList.ClearList(true);
		}
		mPanelsToAdd.Clear();
	}

	private void OnActivateUILaunched(object sender, EventArgs args)
	{
		if (FrontEndController.Instance.ActiveScreen != 0)
		{
			FrontEndController.Instance.ReturnToGlobe();
		}
	}

	private void RefreshSectionData()
	{
		int specOpsImageStartIndex = CountSinglePlayerSectionsForMission(mData.Data.SectionSelectImage);
		DifficultyMode difficulty = Difficulty;
		SectionSelectPanel sectionSelectPanel = mSectionPanels[mSelectedSection];
		sectionSelectPanel.Setup(mSelectedSection, specOpsImageStartIndex, mData.Data, difficulty, SectionSelectedChanged);
		SectionSelectedChanged(mCurrentlySelectedData, true);
	}

	private void LoadMission()
	{
		if (!FrontEndController.Instance.IsBusy && mData.Data.Type != 0 && !mLoading)
		{
			if (mCurrentlySelectedData.Locked)
			{
				mPurchaseData.Section = mCurrentlySelectedData;
				PurchaseFlowHelper.Instance.Purchase(mPurchaseData);
				return;
			}
			mLoading = true;
			mLoadData = mData;
			mLoadSection = mSelectedSection;
			FrontEndController.Instance.StartCoroutine(DelayedLoadMission());
		}
	}

	private IEnumerator DelayedLoadMission()
	{
		AnimatedScreenBackground background = AnimatedScreenBackground.Instance;
		if (background != null)
		{
			background.Activate();
		}
		FrontEndController.Instance.TransitionTo(ScreenID.None);
		SoundManager.Instance.StartLoadMission();
		while (FrontEndController.Instance.IsBusy)
		{
			yield return null;
		}
		GameSettings settings = GameSettings.Instance;
		if (settings != null)
		{
			settings.LastPlayedID = mLoadData.Data.MissionId;
			settings.LastPlayedSectionIndex = mLoadSection;
			settings.LastKnownDifficultyMode = Difficulty;
		}
		GameSettings.LaunchedFromGlobe = true;
		mLoadData.LoadMission(Difficulty, mLoadSection);
	}

	private void SwitchToRegular()
	{
		if (Difficulty == DifficultyMode.Regular)
		{
			return;
		}
		Difficulty = DifficultyMode.Regular;
		if (DifficultySideMenu != null)
		{
			DifficultySideMenu.SetSelected((int)Difficulty);
		}
		for (int i = 0; i < mSectionPanels.Length; i++)
		{
			if (mSectionPanels[i] != null)
			{
				mSectionPanels[i].UpdateDifficulty(Difficulty);
			}
		}
	}

	private void SwitchToVeteran()
	{
		if (Difficulty == DifficultyMode.Veteran)
		{
			return;
		}
		Difficulty = DifficultyMode.Veteran;
		if (DifficultySideMenu != null)
		{
			DifficultySideMenu.SetSelected((int)Difficulty);
		}
		for (int i = 0; i < mSectionPanels.Length; i++)
		{
			if (mSectionPanels[i] != null)
			{
				mSectionPanels[i].UpdateDifficulty(Difficulty);
			}
		}
	}

	private void Share()
	{
		string text = Language.Get(mData.Data.Sections[mSelectedSection].Name);
		int num = StatsHelper.HighestScoreOverall(mData.Data.MissionId, mSelectedSection);
		string formatString = Language.GetFormatString("S_GLOBE_SOCIAL_MESSAGE", num, text);
		m_DialogHelper.PostMessage(formatString, this, "OnFacebookPost", "OnTwitterPost");
	}

	public void OnFacebookPost()
	{
		SwrveEventsMetaGame.SharedFromMission(SwrveEventsMetaGame.ShareType.Facebook, mData.Data.MissionId);
		SwrveEventsMetaGame.FacebookBroadcast("MissionOverview", mData.Data.MissionId);
	}

	public void OnTwitterPost()
	{
		SwrveEventsMetaGame.SharedFromMission(SwrveEventsMetaGame.ShareType.Twitter, mData.Data.MissionId);
		SwrveEventsMetaGame.TwitterBroadcast("MissionOverview", mData.Data.MissionId);
	}

	private void HandleDataLoadedAfterLogin(object sender, EventArgs e)
	{
		Debug.Log("DATA LOADED AFTER LOGIN! Refreshing high scores");
		RefreshHighScores();
	}

	private void HandleAddFriendButtonPressed()
	{
		ActivateWatcher.Instance.LaunchActivate(Bedrock.brUserInterfaceScreen.BR_ADD_FRIENDS_UI);
	}

	private void RefreshHighScores()
	{
		if (mCurrentlySelectedData != null && mCurrentlySelectedData.LeaderboardID > 0)
		{
			if (FriendsLeaderboard != null)
			{
				FriendsLeaderboard.RefreshHighScores(mCurrentlySelectedData.LeaderboardID, mCurrentlySelectedData.IsSpecOps);
			}
		}
		else if (FriendsLeaderboard != null)
		{
			FriendsLeaderboard.HideAllElements();
		}
	}

	private void RefreshFriendsAndHighScores()
	{
		if (mCurrentlySelectedData != null && mCurrentlySelectedData.LeaderboardID > 0)
		{
			if (FriendsLeaderboard != null)
			{
				FriendsLeaderboard.RefreshFriendsAndHighScores(mCurrentlySelectedData.LeaderboardID, mCurrentlySelectedData.IsSpecOps);
			}
		}
		else if (FriendsLeaderboard != null)
		{
			FriendsLeaderboard.HideAllElements();
		}
	}

	private void RefreshNameAndDescription()
	{
		if (mCurrentlySelectedData == null)
		{
			return;
		}
		if (MissionNameText != null)
		{
			string text = ((!(mCurrentlySelectedData.Name != string.Empty)) ? mCurrentlySelectedData.SceneName : Language.Get(mCurrentlySelectedData.Name));
			string text2 = (mCurrentlySelectedData.Locked ? Language.Get("S_LOCKED") : ((!mCurrentlySelectedData.IsSpecOps) ? string.Format("{0}. {1}", mSelectedSection + 1, text) : text));
			MissionNameText.Text = text2.ToUpper();
		}
		if (MissionDescText != null)
		{
			if (mCurrentlySelectedData.Locked)
			{
				MissionDescText.Text = Language.Get("S_GENERIC_SECTION_NODESC");
			}
			else
			{
				string text3 = string.Empty;
				string text4 = ((!(mCurrentlySelectedData.Description != string.Empty)) ? string.Format("Loads level {0} (No Description Given)", mCurrentlySelectedData.SceneName) : Language.Get(mCurrentlySelectedData.Description));
				if (mCurrentlySelectedData.IsSpecOps)
				{
					if (mCurrentlySelectedData.GMGGameType == GMGData.GameType.TimeAttack)
					{
						text3 = "\n" + Language.Get("S_TT_QUICKTUTORIAL");
					}
					else if (mCurrentlySelectedData.GMGGameType == GMGData.GameType.Domination)
					{
						text3 = "\n" + Language.Get("S_DOMINATION_QUICKTUTORIAL");
					}
					else if (mCurrentlySelectedData.GMGGameType == GMGData.GameType.Flashpoint && mCurrentlySelectedData.Name != "S_CAR_GMG_01_FL")
					{
						string text5 = (1 + DateTime.Now.Day % 3).ToString("00");
						switch (mCurrentlySelectedData.FlashpointObjective)
						{
						case MissionListings.FlashpointData.Objective.Destroy:
							text3 = "\n" + Language.Get("S_GUF_DEST_DESC_" + text5);
							break;
						case MissionListings.FlashpointData.Objective.Survive:
							text3 = "\n" + Language.Get("S_GUF_SURVIVE_DESC_" + text5);
							break;
						case MissionListings.FlashpointData.Objective.Collect:
							text3 = "\n" + Language.Get("S_GUF_COLLECT_DESC_" + text5);
							break;
						case MissionListings.FlashpointData.Objective.Clear:
							text3 = "\n" + Language.Get("S_GUF_CLEAR_DESC_" + text5);
							break;
						}
					}
				}
				MissionDescText.Text = text4 + text3;
			}
		}
		if (FlashpointScoreText != null)
		{
			if (mCurrentlySelectedData.IsSpecOps && mCurrentlySelectedData.GMGGameType == GMGData.GameType.Flashpoint)
			{
				FlashpointScoreText.Text = Language.Get("S_SCORE");
			}
			else
			{
				FlashpointScoreText.Text = string.Empty;
			}
		}
		if (FlashpointScoreValueText != null)
		{
			if (mCurrentlySelectedData.IsSpecOps && mCurrentlySelectedData.GMGGameType == GMGData.GameType.Flashpoint)
			{
				FlashpointScoreValueText.Text = GlobalUnrestController.Instance.CurrentScore.ToString("000000000");
			}
			else
			{
				FlashpointScoreValueText.Text = string.Empty;
			}
		}
		if (DeployButton != null)
		{
			bool locked = mCurrentlySelectedData.Locked;
			bool flag = mCurrentlySelectedData.UnlockedAtXpLevel != -1;
			DeployButton.CurrentState = ((!locked || flag) ? FrontEndButton.State.Highlighted : FrontEndButton.State.Disabled);
			DeployButton.Text = Language.Get((locked || !flag) ? "S_DEPLOY" : string.Empty);
			if (locked && flag)
			{
				char c = CommonHelper.HardCurrencySymbol();
				string arg = string.Format("{0}{1}", c, mCurrentlySelectedData.UnlockEarlyCost.ToString("N", mNfi));
				DeployButton.Text = string.Format(Language.Get("S_UNLOCK_EARLY_COST"), arg);
			}
			else
			{
				DeployButton.Text = Language.Get("S_DEPLOY");
			}
		}
		if (ShareButton != null)
		{
			ShareButton.CurrentState = ((StatsHelper.HighestScore(mData.Data.MissionId, mSelectedSection) <= 0) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
		}
	}
}
