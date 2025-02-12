using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LeaderboardNavigator : FrontEndScreen
{
	private struct AnonynmousData
	{
		public int Rating;

		public int XP;

		public bool Veteran;
	}

	private enum LeaderboardDisplayType
	{
		TopRated = 0,
		PivotAroundAnonymousUser = 1,
		PivotAroundRegisteredUser = 2
	}

	private class SubMode
	{
		public int LeaderboardID;

		public string Title;

		public SubMode(int leaderboardId, string title)
		{
			LeaderboardID = leaderboardId;
			Title = title;
		}
	}

	private class Mode
	{
		public UIButton Button;

		public FrontEndButton ButtonState;

		public MissionData Data;

		public int SectionIndex;

		public bool VeteranAllowed;

		public List<SubMode> SubModes = new List<SubMode>();
	}

	private const int MAX_ROWS = 10;

	public GameObject ButtonPrefab;

	public MenuScreenBlade SidePanel;

	public LeaderboardController LeaderboardController;

	public SpriteText TitleText;

	public GameObject Arrows;

	private LeaderboardController.Panel[] mCachedPanels;

	private LeaderboardController.Panel[] mCachedCurrentPosition;

	private Mode[] mModes;

	private Transform mSideMenuContent;

	private NumberFormatInfo mNFI;

	private int mMode;

	private int mNumModes;

	private int mCachedAbsoluteLevel;

	private LeaderboardDisplayType mDisplayType;

	private AnonynmousData mAnonymousLeaderboardData;

	protected override void Awake()
	{
		mNFI = GlobalizationUtils.GetNumberFormat(0);
		ID = ScreenID.Leaderboards;
		MissionListings instance = MissionListings.Instance;
		mNumModes = 2;
		if (instance != null)
		{
			int num = instance.Missions.Length;
			for (int i = 0; i < num; i++)
			{
				MissionData missionData = instance.Missions[i];
				if (missionData.IncludeInStats && missionData.HasStoryMissions())
				{
					mNumModes++;
				}
			}
			mModes = new Mode[mNumModes];
		}
		if (SidePanel != null)
		{
			AnimateCommonBackgroundBox componentInChildren = SidePanel.GetComponentInChildren<AnimateCommonBackgroundBox>();
			if (componentInChildren != null)
			{
				mSideMenuContent = componentInChildren.transform.Find("Content");
			}
		}
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		RefreshAllData();
	}

	private void RefreshAllData()
	{
		InitialiseModes();
	}

	private void OnEnable()
	{
		ActivateWatcher.ConnectionStatusChange += HandleUserConnectionStatusChange;
	}

	private void OnDisable()
	{
		ActivateWatcher.ConnectionStatusChange -= HandleUserConnectionStatusChange;
		BedrockWorker.Instance.InvalidateGotleaderboardCallback();
	}

	private void HandleUserConnectionStatusChange(object sender, ConnectionStatusChangeEventArgs e)
	{
		if (e.NewStatus == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_OFFLINE || e.NewStatus == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_ANONYMOUSLY_OFFLINE)
		{
			FrontEndController.Instance.ReturnToPrevious();
			return;
		}
		LeaderboardController.Clear();
		RefreshMissionLeaderboard();
	}

	private void OnDataChange(object sender, EventArgs e)
	{
		RefreshAllData();
		FrontEndController.Instance.ReturnToGlobe();
	}

	private void OnActivateLaunched(object sender, EventArgs e)
	{
		FrontEndController.Instance.ReturnToGlobe();
	}

	public override void EnterScreen()
	{
		base.EnterScreen();
		if (Bedrock.isBedrockActive())
		{
			Bedrock.UpdateFriendsList();
		}
		mMode = 0;
		if (Arrows != null)
		{
			Arrows.SetActive(false);
		}
		mCachedAbsoluteLevel = XPManager.Instance.GetXPLevelAbsolute();
		RefreshSideMenu();
		RefreshLeaderboard(LeaderboardController, MenuScreenBlade.BladeTransition.On);
		ActivateWatcher.DataLoadedAfterLogin += OnDataChange;
		ActivateWatcher.ActivateUILaunched += OnActivateLaunched;
		StartCoroutine(LayoutSideMenu());
	}

	public override void ExitScreen()
	{
		base.ExitScreen();
		ActivateWatcher.DataLoadedAfterLogin -= OnDataChange;
		ActivateWatcher.ActivateUILaunched -= OnActivateLaunched;
	}

	protected override void Update()
	{
		base.Update();
	}

	private void ModeButtonPressed()
	{
		int mode = 0;
		for (int i = 0; i < mNumModes; i++)
		{
			if (mModes[i].Button != null && mModes[i].Button.controlState == UIButton.CONTROL_STATE.ACTIVE)
			{
				mode = i;
				break;
			}
		}
		ChangeMode(mode);
	}

	private void TransitionToNext()
	{
		if (mMode != 0 && mModes[mMode] != null)
		{
			int count = mModes[mMode].SubModes.Count;
			mModes[mMode].SectionIndex = (mModes[mMode].SectionIndex + 1) % count;
			RefreshLeaderboard(LeaderboardController, MenuScreenBlade.BladeTransition.On);
		}
	}

	private void TransitionToPrevious()
	{
		if (mMode != 0 && mModes[mMode] != null)
		{
			int count = mModes[mMode].SubModes.Count;
			mModes[mMode].SectionIndex = (mModes[mMode].SectionIndex + (count - 1)) % count;
			RefreshLeaderboard(LeaderboardController, MenuScreenBlade.BladeTransition.On);
		}
	}

	private void ChangeMode(int mode)
	{
		if (mMode != mode && LeaderboardController.IsActive)
		{
			mMode = mode;
			LeaderboardController.Deactivate(RefreshLeaderboard);
			RefreshSideMenu();
		}
	}

	private void RefreshSideMenu()
	{
		for (int i = 0; i < mNumModes; i++)
		{
			if (mModes[i].ButtonState != null)
			{
				mModes[i].ButtonState.CurrentState = ((mMode == i) ? FrontEndButton.State.Selected : FrontEndButton.State.Normal);
			}
		}
		if (!SidePanel.IsActive)
		{
			SidePanel.Activate();
		}
	}

	private void RefreshLeaderboard(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
		LeaderboardController.Clear();
		if (Arrows != null)
		{
			Arrows.SetActive(mMode != 0);
		}
		if (mModes[mMode] != null)
		{
			int sectionIndex = mModes[mMode].SectionIndex;
			string title = mModes[mMode].SubModes[sectionIndex].Title;
			if (TitleText != null)
			{
				TitleText.Text = title;
			}
			mCachedPanels = new LeaderboardController.Panel[0];
			mCachedCurrentPosition = new LeaderboardController.Panel[0];
			RefreshMissionLeaderboard();
		}
		LeaderboardController.Activate();
	}

	private void RefreshMissionLeaderboard()
	{
		bool flag = false;
		int sectionIndex = mModes[mMode].SectionIndex;
		if (mModes[mMode] != null && mModes[mMode].Data != null && sectionIndex < mModes[mMode].SubModes.Count)
		{
			flag = !mModes[mMode].VeteranAllowed;
		}
		mCachedCurrentPosition = new LeaderboardController.Panel[3];
		for (int i = 0; i < 3; i++)
		{
			mCachedCurrentPosition[i] = new LeaderboardController.Panel();
			mCachedCurrentPosition[i].Blank();
		}
		bool showVeteranIcon = flag && mModes[mMode].SubModes[sectionIndex].LeaderboardID != LeaderboardManager.GlobalXPLeaderboardID;
		LeaderboardController.UpdateCurrentPosition(mCachedCurrentPosition, showVeteranIcon);
		if (!Bedrock.isUserConnected())
		{
			return;
		}
		int leaderboardID = mModes[mMode].SubModes[sectionIndex].LeaderboardID;
		if (Bedrock.getUserConnectionStatus().IsAnonymous())
		{
			mAnonymousLeaderboardData.Rating = StatsManager.Instance.LeaderboardManagerInstance.FindScoreForLeaderboard(leaderboardID, out mAnonymousLeaderboardData.Veteran);
			if (mAnonymousLeaderboardData.Rating <= 0)
			{
				mDisplayType = LeaderboardDisplayType.TopRated;
				BedrockWorker.Instance.GetLeaderboardValuesByRank(1uL, (uint)leaderboardID, false, LeaderboardRetrieved);
			}
			else
			{
				mDisplayType = LeaderboardDisplayType.PivotAroundAnonymousUser;
				BedrockWorker.Instance.GetLeaderboardValuesByRating((ulong)mAnonymousLeaderboardData.Rating, (uint)leaderboardID, false, RatingLeaderboardRetrieved);
			}
		}
		else
		{
			mDisplayType = LeaderboardDisplayType.TopRated;
			BedrockWorker.Instance.GetLeaderboardValuesByRank(1uL, (uint)leaderboardID, false, LeaderboardRetrieved);
		}
	}

	private IEnumerator PopulateFakeData(bool isVeteranAllowed, int sectionIndex)
	{
		yield return new WaitForSeconds(1f);
		bool showVeteranIcon = isVeteranAllowed && mModes[mMode].SubModes[sectionIndex].LeaderboardID != LeaderboardManager.GlobalXPLeaderboardID;
		mCachedPanels = GetPanelsForLeaderboard(mModes[mMode].SubModes[sectionIndex].LeaderboardID, 1, 12);
		LeaderboardController.AddPanels(mCachedPanels, showVeteranIcon);
		mCachedCurrentPosition = GetCurrentPositionForLeaderboard(mModes[mMode].SubModes[sectionIndex].LeaderboardID);
		LeaderboardController.UpdateCurrentPosition(mCachedCurrentPosition, showVeteranIcon);
	}

	private void RatingLeaderboardRetrieved(uint leaderboardIndex, List<LeaderboardResult> leaderBoard)
	{
		if (base.enabled)
		{
			int sectionIndex = mModes[mMode].SectionIndex;
			if (leaderboardIndex != mModes[mMode].SubModes[sectionIndex].LeaderboardID)
			{
				RefreshMissionLeaderboard();
			}
			else if (leaderBoard.Count > 0)
			{
				ulong userID = leaderBoard[0].UserID;
				BedrockWorker.Instance.GetLeaderboardValuesByGivenPivot((uint)mModes[mMode].SubModes[sectionIndex].LeaderboardID, userID, false, LeaderboardRetrieved);
			}
			else
			{
				BedrockWorker.Instance.GetLeaderboardValuesByRank(1uL, leaderboardIndex, false, LeaderboardRetrieved);
				mDisplayType = LeaderboardDisplayType.TopRated;
			}
		}
	}

	private void LeaderboardRetrieved(uint leaderboardIndex, List<LeaderboardResult> leaderBoard)
	{
		if (!base.enabled)
		{
			return;
		}
		int sectionIndex = mModes[mMode].SectionIndex;
		if (leaderboardIndex != mModes[mMode].SubModes[sectionIndex].LeaderboardID)
		{
			RefreshMissionLeaderboard();
			return;
		}
		if (leaderBoard.Count == 0)
		{
			BedrockWorker.Instance.GetLeaderboardValuesByRank(1uL, leaderboardIndex, false, LeaderboardRetrieved);
			mDisplayType = LeaderboardDisplayType.TopRated;
			return;
		}
		bool flag = false;
		if (mModes[mMode] != null && mModes[mMode].Data != null && sectionIndex < mModes[mMode].SubModes.Count)
		{
			flag = mModes[mMode].VeteranAllowed;
		}
		int num = 0;
		if (mDisplayType == LeaderboardDisplayType.PivotAroundAnonymousUser)
		{
			for (int i = 0; i < leaderBoard.Count; i++)
			{
				if (mAnonymousLeaderboardData.Rating >= leaderBoard[i].Rating)
				{
					num = i;
					leaderBoard.Insert(i, new LeaderboardResult(Language.Get("S_YOUR_SCORE"), 0uL, mAnonymousLeaderboardData.Rating, 0uL, true, false, 0, 0, 0, mAnonymousLeaderboardData.XP, mAnonymousLeaderboardData.Veteran));
					break;
				}
			}
		}
		else if (mDisplayType == LeaderboardDisplayType.PivotAroundRegisteredUser)
		{
			for (int j = 0; j < leaderBoard.Count; j++)
			{
				if (leaderBoard[j].IsPlayer)
				{
					num = j;
					break;
				}
			}
		}
		else
		{
			while (leaderBoard.Count > 10 && leaderBoard[0].Rating > SwrveServerVariables.Instance.MaxXPForLeaderboards)
			{
				leaderBoard.RemoveAt(0);
			}
			for (int k = 0; k < leaderBoard.Count; k++)
			{
				leaderBoard[k].LeaderboardRank = (ulong)(k + 1);
			}
		}
		num = Mathf.Max(0, num - 5);
		List<LeaderboardController.Panel> list = new List<LeaderboardController.Panel>();
		for (int l = num; l < leaderBoard.Count; l++)
		{
			if (list.Count >= 10)
			{
				continue;
			}
			LeaderboardController.Panel panel = new LeaderboardController.Panel();
			panel.title = leaderBoard[l].Name;
			panel.elite = leaderBoard[l].Elite;
			panel.veteran = leaderBoard[l].Veteran;
			panel.level = leaderBoard[l].Rank;
			if (leaderBoard[l].LeaderboardRank != 0L)
			{
				panel.rank = leaderBoard[l].LeaderboardRank.ToString("N", mNFI) + ".";
			}
			else
			{
				int num2 = 2;
				if (l > 0)
				{
					num2 = leaderBoard[l - 1].LeaderboardRank.ToString().Length + 1;
				}
				panel.rank = string.Empty;
				for (int m = 0; m < num2; m++)
				{
					panel.rank += "-";
				}
			}
			panel.score = leaderBoard[l].Rating.ToString("N", mNFI);
			panel.player = leaderBoard[l].IsPlayer;
			if (leaderBoard[l].IsPlayer)
			{
				panel.level = mCachedAbsoluteLevel;
			}
			list.Add(panel);
		}
		mCachedPanels = list.ToArray();
		bool showVeteranIcon = flag && mModes[mMode].SubModes[sectionIndex].LeaderboardID != LeaderboardManager.GlobalXPLeaderboardID;
		LeaderboardController.AddPanels(mCachedPanels, showVeteranIcon);
		RefreshCurrentPosition();
	}

	private void RefreshCurrentPosition()
	{
		if (Bedrock.isUserConnected() && !Bedrock.isDeviceAnonymouslyLoggedOn())
		{
			int sectionIndex = mModes[mMode].SectionIndex;
			BedrockWorker.Instance.GetLeaderboardValuesByPivot((uint)mModes[mMode].SubModes[sectionIndex].LeaderboardID, false, CurrentPositionRetrieved);
		}
	}

	private void CurrentPositionRetrieved(uint leaderboardIndex, List<LeaderboardResult> leaderBoard)
	{
		if (!base.enabled)
		{
			return;
		}
		int sectionIndex = mModes[mMode].SectionIndex;
		if (leaderboardIndex != mModes[mMode].SubModes[sectionIndex].LeaderboardID)
		{
			RefreshMissionLeaderboard();
			return;
		}
		bool flag = false;
		if (mModes[mMode] != null && mModes[mMode].Data != null && sectionIndex < mModes[mMode].SubModes.Count)
		{
			flag = mModes[mMode].VeteranAllowed;
		}
		mCachedCurrentPosition = new LeaderboardController.Panel[3];
		int num = BedrockWorker.FindPlayerInLeaderboard(leaderBoard);
		if (num == -1)
		{
			return;
		}
		int num2 = num - 1;
		if (num == leaderBoard.Count - 1)
		{
			num2--;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		for (int i = 0; i < 3; i++)
		{
			mCachedCurrentPosition[i] = new LeaderboardController.Panel();
			if (num2 >= 0 && num2 < leaderBoard.Count)
			{
				mCachedCurrentPosition[i].title = leaderBoard[num2].Name;
				mCachedCurrentPosition[i].elite = leaderBoard[num2].Elite;
				mCachedCurrentPosition[i].veteran = leaderBoard[num2].Veteran;
				mCachedCurrentPosition[i].level = leaderBoard[num2].Rank;
				mCachedCurrentPosition[i].rank = leaderBoard[num2].LeaderboardRank.ToString("N", mNFI) + ".";
				mCachedCurrentPosition[i].score = leaderBoard[num2].Rating.ToString("N", mNFI);
				mCachedCurrentPosition[i].player = leaderBoard[num2].IsPlayer;
				if (mCachedCurrentPosition[i].player)
				{
					mCachedCurrentPosition[i].level = mCachedAbsoluteLevel;
				}
			}
			else
			{
				mCachedCurrentPosition[i].Blank();
			}
			num2++;
		}
		bool showVeteranIcon = flag && mModes[mMode].SubModes[sectionIndex].LeaderboardID != LeaderboardManager.GlobalXPLeaderboardID;
		LeaderboardController.UpdateCurrentPosition(mCachedCurrentPosition, showVeteranIcon);
	}

	private void InitialiseModes()
	{
		MissionListings instance = MissionListings.Instance;
		int num = instance.Missions.Length;
		float num2 = 1f / (float)mNumModes;
		float num3 = 0f;
		int num4 = 0;
		mModes[num4] = new Mode();
		mModes[num4].SubModes.Add(new SubMode(LeaderboardManager.GlobalXPLeaderboardID, Language.Get("S_GLOBAL_XP")));
		mModes[num4].VeteranAllowed = false;
		GameObject button = (GameObject)UnityEngine.Object.Instantiate(ButtonPrefab);
		mModes[num4].Button = PlaceButton(button, num3, num2, AutoLocalize.Get("S_GLOBAL_XP"));
		mModes[num4].ButtonState = mModes[num4].Button.GetComponent<FrontEndButton>();
		num3 += num2;
		num4++;
		int num5 = num4;
		mModes[num4] = new Mode();
		mModes[num4].VeteranAllowed = false;
		GameObject button2 = (GameObject)UnityEngine.Object.Instantiate(ButtonPrefab);
		mModes[num4].Button = PlaceButton(button2, num3, num2, AutoLocalize.Get("S_LEADERBOARDS_ARCADE"));
		mModes[num4].ButtonState = mModes[num4].Button.GetComponent<FrontEndButton>();
		for (int i = 0; i < num; i++)
		{
			MissionData missionData = instance.Missions[i];
			if (!missionData.IncludeInStats)
			{
				continue;
			}
			if (missionData.HasStoryMissions())
			{
				num4++;
				num3 += num2;
				mModes[num4] = new Mode();
				mModes[num4].Data = missionData;
				mModes[num4].SectionIndex = 0;
				mModes[num4].VeteranAllowed = true;
				for (int j = 0; j < missionData.Sections.Count; j++)
				{
					SectionData sectionData = missionData.Sections[j];
					if (sectionData.IsValidInCurrentBuild)
					{
						mModes[num4].SubModes.Add(new SubMode(sectionData.LeaderboardID, Language.Get(sectionData.Name)));
					}
				}
				GameObject button3 = (GameObject)UnityEngine.Object.Instantiate(ButtonPrefab);
				mModes[num4].Button = PlaceButton(button3, num3, num2, AutoLocalize.Get(missionData.NameKey).ToUpper());
				if (mModes[num4].Button != null)
				{
					mModes[num4].ButtonState = mModes[num4].Button.GetComponent<FrontEndButton>();
				}
			}
			for (int k = 0; k < missionData.Sections.Count; k++)
			{
				SectionData sectionData2 = missionData.Sections[k];
				if (sectionData2.IsSpecOps && sectionData2.IsValidInCurrentBuild)
				{
					mModes[num5].SubModes.Add(new SubMode(sectionData2.LeaderboardID, Language.Get(sectionData2.Name)));
				}
			}
		}
	}

	private IEnumerator LayoutSideMenu()
	{
		float pixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Vector2 buttonSize = new Vector2(0f, 0f);
		for (int currentMode = 0; currentMode < mNumModes; currentMode++)
		{
			if (mModes[currentMode].Button != null)
			{
				while (mModes[currentMode].Button.width == 0f)
				{
					yield return null;
				}
				if (buttonSize.x == 0f)
				{
					buttonSize.x = mModes[currentMode].Button.width / pixelSize;
					buttonSize.y = mModes[currentMode].Button.height / pixelSize;
				}
			}
		}
		float totalHeight = buttonSize.y * (float)mNumModes;
		if (!(SidePanel != null))
		{
			yield break;
		}
		CommonBackgroundBox box = SidePanel.GetComponentInChildren<CommonBackgroundBox>();
		AnimateCommonBackgroundBox animateBox = SidePanel.GetComponentInChildren<AnimateCommonBackgroundBox>();
		if (box != null)
		{
			if (TBFUtils.IsRetinaHdDevice())
			{
				box.ForegroundHeightInUnits = totalHeight / 64f;
			}
			else
			{
				box.ForegroundHeightInUnits = totalHeight / 32f;
			}
			box.Resize();
		}
		if (animateBox != null)
		{
			animateBox.RecacheVariables();
		}
	}

	private UIButton PlaceButton(GameObject button, float start, float height, string text)
	{
		UIButton uIButton = null;
		if (button != null)
		{
			uIButton = button.GetComponentInChildren<UIButton>();
			if (uIButton != null)
			{
				uIButton.transform.position = new Vector3(-100f, 0f, 0f);
				uIButton.Text = text;
				uIButton.scriptWithMethodToInvoke = this;
				uIButton.methodToInvoke = "ModeButtonPressed";
				AnimateButtonOnInvoke component = uIButton.GetComponent<AnimateButtonOnInvoke>();
				if (component != null)
				{
					component.ReplaceButtonScript();
				}
			}
			CommonBackgroundBoxPlacement commonBackgroundBoxPlacement = button.GetComponentInChildren<CommonBackgroundBoxPlacement>();
			if (commonBackgroundBoxPlacement == null)
			{
				commonBackgroundBoxPlacement = button.gameObject.AddComponent<CommonBackgroundBoxPlacement>();
			}
			commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxWidth = 0f;
			commonBackgroundBoxPlacement.WidthAsPercentageOfBoxWidth = 1f;
			commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight = start;
			commonBackgroundBoxPlacement.HeightAsPercentageOfBoxHeight = height;
			button.transform.parent = mSideMenuContent;
		}
		return uIButton;
	}

	private LeaderboardController.Panel[] GetPanelsForLeaderboard(int LeaderboardIndex, int start, int numToFetch)
	{
		List<LeaderboardController.Panel> list = new List<LeaderboardController.Panel>();
		for (int i = 0; i < numToFetch; i++)
		{
			LeaderboardController.Panel panel = new LeaderboardController.Panel();
			panel.rank = (i + start).ToString("N", mNFI) + ".";
			panel.title = "PANEL " + (i + 1).ToString("N8");
			panel.score = (100000000 - i).ToString("N", mNFI);
			panel.level = 50 - i;
			panel.veteran = true;
			panel.elite = true;
			panel.player = i == 1;
			list.Add(panel);
		}
		return list.ToArray();
	}

	private LeaderboardController.Panel[] GetCurrentPositionForLeaderboard(int LeaderboardIndex)
	{
		List<LeaderboardController.Panel> list = new List<LeaderboardController.Panel>();
		for (int i = 0; i < 3; i++)
		{
			LeaderboardController.Panel panel = new LeaderboardController.Panel();
			panel.rank = (i + 1).ToString("N", mNFI) + ".";
			panel.title = "PANEL " + (i + 1).ToString("N8");
			panel.score = (100000000 - i).ToString("N", mNFI);
			panel.level = 50 - i;
			panel.veteran = true;
			panel.elite = true;
			panel.player = i == 1;
			list.Add(panel);
		}
		return list.ToArray();
	}
}
