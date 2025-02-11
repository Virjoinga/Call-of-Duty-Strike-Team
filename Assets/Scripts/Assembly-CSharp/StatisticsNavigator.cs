using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class StatisticsNavigator : FrontEndScreen
{
	private enum Mode
	{
		CareerStatistics = 0,
		CombatStatistics = 1,
		SquadStatistics = 2,
		MissionStatistics = 3
	}

	public MenuScreenBlade SidePanel;

	public StatisticsController StatsController;

	public SpriteText TitleText;

	public FrontEndButton CareerButton;

	public FrontEndButton CombatButton;

	public FrontEndButton SquadButton;

	public FrontEndButton MissionsButton;

	public FrontEndButton AchievementsButton;

	private string[] mModeTitles = new string[4] { "S_CAREER_OVERVIEW", "S_STATISTICS_TITLE", "S_SQUAD_OVERVIEW", "S_MISSIONS" };

	private StatisticsController.Panel[] mCareerPanels;

	private StatisticsController.Panel[] mCombatPanels;

	private StatisticsController.Panel[] mSquadPanels;

	private StatisticsController.Panel[] mMissionPanels;

	private NumberFormatInfo mNFI;

	private Mode mMode;

	private SocialBroadcastDialogHelper m_DialogHelper;

	protected override void Awake()
	{
		AchievementsButton.gameObject.SetActive(false);
		Transform transform = base.transform.Find("StatisticsPanel/Buttons");
		if (transform != null)
		{
			CommonBackgroundBoxPlacement componentInChildren = transform.GetComponentInChildren<CommonBackgroundBoxPlacement>();
			componentInChildren.StartPositionAsPercentageOfBoxHeight = 0.25f;
			CommonBackgroundBox componentInChildren2 = transform.GetComponentInChildren<CommonBackgroundBox>();
			componentInChildren2.ForegroundHeightInUnits = 1.25f;
			AnimateCommonBackgroundBox componentInChildren3 = transform.GetComponentInChildren<AnimateCommonBackgroundBox>();
			componentInChildren3.RecacheVariables();
		}
		mNFI = GlobalizationUtils.GetNumberFormat(0);
		ID = ScreenID.Statistics;
		base.Awake();
		if (m_DialogHelper == null)
		{
			m_DialogHelper = base.gameObject.AddComponent<SocialBroadcastDialogHelper>();
		}
		m_DialogHelper.AllowedToPost = true;
	}

	protected override void Start()
	{
		base.Start();
		RefreshAllData();
	}

	private void RefreshAllData()
	{
		InitialiseCareerPanels();
		InitialiseCombatPanels();
		InitialiseSquadPanels();
		InitialiseMissionPanels();
	}

	private void OnDataChange(object sender, EventArgs e)
	{
		RefreshAllData();
		FrontEndController.Instance.ReturnToPrevious();
	}

	public override void EnterScreen()
	{
		base.EnterScreen();
		mMode = Mode.CareerStatistics;
		RefreshSideMenu();
		RefreshAllData();
		RefreshStatistics(StatsController, MenuScreenBlade.BladeTransition.On);
		ActivateWatcher.DataLoadedAfterLogin += OnDataChange;
		ActivateWatcher.ActivateUILaunched += OnActivateUILaunched;
	}

	public override void ExitScreen()
	{
		base.ExitScreen();
		ActivateWatcher.DataLoadedAfterLogin -= OnDataChange;
		ActivateWatcher.ActivateUILaunched -= OnActivateUILaunched;
	}

	private void OnActivateUILaunched(object sender, EventArgs args)
	{
		if (FrontEndController.Instance.ActiveScreen != 0)
		{
			FrontEndController.Instance.ReturnToGlobe();
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	private void CareerStatisticsPressed()
	{
		SwrveEventsUI.ViewedCareer();
		if (mMode != 0 && StatsController.IsActive)
		{
			StatsController.Deactivate(RefreshStatistics);
			mMode = Mode.CareerStatistics;
			RefreshSideMenu();
		}
	}

	private void CombatStatisticsPressed()
	{
		SwrveEventsUI.ViewedCombat();
		if (mMode != Mode.CombatStatistics && StatsController.IsActive)
		{
			StatsController.Deactivate(RefreshStatistics);
			mMode = Mode.CombatStatistics;
			RefreshSideMenu();
		}
	}

	private void SquadStatisticsPressed()
	{
		SwrveEventsUI.ViewedSquad();
		if (mMode != Mode.SquadStatistics && StatsController.IsActive)
		{
			StatsController.Deactivate(RefreshStatistics);
			mMode = Mode.SquadStatistics;
			RefreshSideMenu();
		}
	}

	private void MissionStatisticsPressed()
	{
		SwrveEventsUI.ViewedMissions();
		if (mMode != Mode.MissionStatistics && StatsController.IsActive)
		{
			StatsController.Deactivate(RefreshStatistics);
			mMode = Mode.MissionStatistics;
			RefreshSideMenu();
		}
	}

	private void OnLeaderboardButtonPressed()
	{
		bool flag = true;
		if (Bedrock.getUserConnectionStatus() != Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE && Bedrock.getUserConnectionStatus() != Bedrock.brUserConnectionStatus.BR_LOGGING_IN_REGISTERED && Bedrock.getUserConnectionStatus() != Bedrock.brUserConnectionStatus.BR_LOGGED_IN_ANONYMOUSLY_ONLINE && Bedrock.getUserConnectionStatus() != Bedrock.brUserConnectionStatus.BR_LOGGING_IN_ANONYMOUSLY)
		{
			flag = false;
		}
		if (flag)
		{
			SwrveEventsUI.ViewedLeaderboards();
			FrontEndController.Instance.TransitionTo(ScreenID.Leaderboards);
		}
		else
		{
			MessageBoxController.Instance.DoNoLeaderboardsDialog();
		}
	}

	private void OnAchievementsButtonPressed()
	{
		SwrveEventsUI.ViewedAchievements();
		MobileNetworkManager.Instance.showAchievements();
	}

	private void RefreshSideMenu()
	{
		CareerButton.CurrentState = ((mMode == Mode.CareerStatistics) ? FrontEndButton.State.Selected : FrontEndButton.State.Normal);
		CombatButton.CurrentState = ((mMode == Mode.CombatStatistics) ? FrontEndButton.State.Selected : FrontEndButton.State.Normal);
		SquadButton.CurrentState = ((mMode == Mode.SquadStatistics) ? FrontEndButton.State.Selected : FrontEndButton.State.Normal);
		MissionsButton.CurrentState = ((mMode == Mode.MissionStatistics) ? FrontEndButton.State.Selected : FrontEndButton.State.Normal);
		if (!SidePanel.IsActive)
		{
			SidePanel.Activate();
		}
	}

	private void RefreshStatistics(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
		if (TitleText != null)
		{
			TitleText.Text = GetCurrentTitleText();
		}
		StatsController.Clear();
		switch (mMode)
		{
		case Mode.CareerStatistics:
			StatsController.AddPanels(mCareerPanels);
			break;
		case Mode.CombatStatistics:
			StatsController.AddPanels(mCombatPanels);
			break;
		case Mode.SquadStatistics:
			StatsController.AddPanels(mSquadPanels);
			break;
		case Mode.MissionStatistics:
			StatsController.AddPanels(mMissionPanels);
			break;
		}
		StatsController.Activate();
	}

	private void InitialiseCareerPanels()
	{
		int num = 2;
		mCareerPanels = new StatisticsController.Panel[num];
		string text = Language.Get("S_RESULT_XP");
		int xp = StatsHelper.PlayerXP();
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		XPManager.Instance.ConvertXPToLevel(xp, out level, out prestigeLevel, out xpToNextLevel, out percent);
		string text2 = string.Format("{0}{1}", CommonHelper.HardCurrencySymbol(), StatsHelper.TotalHardCurrencyEarned().ToString("N", mNFI));
		string[] subtitles = new string[4]
		{
			AutoLocalize.Get("S_LEVEL"),
			AutoLocalize.Get("S_TOTAL_XP"),
			AutoLocalize.Get("S_XP_TO_NEXT_LEVEL"),
			AutoLocalize.Get("S_TOTAL_EARNINGS")
		};
		string[] values = new string[4]
		{
			level.ToString(),
			xp.ToString("N", mNFI) + text,
			xpToNextLevel.ToString("N", mNFI) + text,
			text2
		};
		string[] subtitles2 = new string[3]
		{
			AutoLocalize.Get("S_TOTAL_TIME_PLAYED"),
			AutoLocalize.Get("S_MISSIONS_COMPLETED"),
			AutoLocalize.Get("S_MEDALS_EARNED")
		};
		string[] values2 = new string[3]
		{
			StatsHelper.MinutesToString(StatsHelper.TotalTimePlayed()),
			StatsHelper.MissionsSuccessful().ToString(),
			StatsHelper.TotalMedalsEarned() + "/" + StatsHelper.NumberOfMedalsAvailable()
		};
		mCareerPanels[0] = NewPanel(XPManager.Instance.XPLevelName(level), subtitles, values, StatisticsPanel.Type.Rank);
		mCareerPanels[1] = NewPanel(string.Empty, subtitles2, values2, StatisticsPanel.Type.Medals);
	}

	private void InitialiseCombatPanels()
	{
		int num = 3;
		mCombatPanels = new StatisticsController.Panel[num];
		string[] subtitles = new string[5]
		{
			AutoLocalize.Get("S_KILLS"),
			AutoLocalize.Get("S_DEATHS"),
			AutoLocalize.Get("S_KILLS_FPS"),
			AutoLocalize.Get("S_DEATHS_FPS"),
			AutoLocalize.Get("S_KD_RATIO")
		};
		string[] values = new string[5]
		{
			StatsHelper.NumKillsByPlayer().ToString("N", mNFI),
			StatsHelper.NumDeathsForPlayer().ToString("N", mNFI),
			StatsHelper.NumKillsByPlayerFP().ToString("N", mNFI),
			StatsHelper.NumDeathsForPlayerFP().ToString("N", mNFI),
			StatsHelper.KDRToString(StatsHelper.KDRForPlayer())
		};
		string[] subtitles2 = new string[3]
		{
			AutoLocalize.Get("S_SHOTS_FIRED"),
			AutoLocalize.Get("S_SHOTS_HIT"),
			AutoLocalize.Get("S_ACCURACY")
		};
		string[] values2 = new string[3]
		{
			StatsHelper.ShotsFiredByPlayer().ToString("N", mNFI),
			StatsHelper.ShotsHitByPlayer().ToString("N", mNFI),
			StatsHelper.AccuracyToString(StatsHelper.AccuracyForPlayer())
		};
		string[] subtitles3 = new string[2]
		{
			AutoLocalize.Get("S_FAVOURITE_WEAPON"),
			AutoLocalize.Get("S_HEAD_SHOTS")
		};
		string[] values3 = new string[2]
		{
			FindWeaponName(StatsHelper.MostFiredWeaponByPlayer()),
			StatsHelper.HeadShotsForPlayer().ToString("N", mNFI)
		};
		mCombatPanels[0] = NewPanel(string.Empty, subtitles, values, StatisticsPanel.Type.Kills);
		mCombatPanels[1] = NewPanel(string.Empty, subtitles2, values2, StatisticsPanel.Type.Accuracy);
		mCombatPanels[2] = NewPanel(string.Empty, subtitles3, values3, StatisticsPanel.Type.Weapon);
	}

	private void InitialiseSquadPanels()
	{
		GameSettings instance = GameSettings.Instance;
		int num = instance.Soldiers.Length;
		int num2 = num + 1;
		mSquadPanels = new StatisticsController.Panel[num2];
		string title = AutoLocalize.Get("S_OVERVIEW");
		string[] subtitles = new string[3]
		{
			AutoLocalize.Get("S_SQUAD_KILLS"),
			AutoLocalize.Get("S_SQUAD_DEATHS"),
			AutoLocalize.Get("S_SQUAD_KD_RATIO")
		};
		string[] values = new string[3]
		{
			StatsHelper.NumKillsBySquad().ToString("N", mNFI),
			StatsHelper.NumDeathsForSquad().ToString("N", mNFI),
			StatsHelper.KDRToString(StatsHelper.KDRForSquad())
		};
		mSquadPanels[0] = NewPanel(title, subtitles, values, StatisticsPanel.Type.Squad);
		for (int i = 0; i < num; i++)
		{
			string id = StatsManager.ConvertSoldierIndexToId(i);
			string title2 = instance.Soldiers[i].Name;
			string[] subtitles2 = new string[3]
			{
				AutoLocalize.Get("S_KILLS"),
				AutoLocalize.Get("S_DEATHS"),
				AutoLocalize.Get("S_KD_RATIO")
			};
			string[] values2 = new string[3]
			{
				StatsHelper.GetTotalKillsForCharacter(id).ToString("N", mNFI),
				StatsHelper.GetTotalDeathsForCharacter(id).ToString("N", mNFI),
				StatsHelper.KDRToString(StatsHelper.GetTotalKDRForCharacter(id))
			};
			mSquadPanels[i + 1] = NewPanel(title2, subtitles2, values2, (StatisticsPanel.Type)i);
		}
	}

	private void InitialiseMissionPanels()
	{
		MissionListings instance = MissionListings.Instance;
		int num = instance.Missions.Length;
		List<StatisticsController.Panel> list = new List<StatisticsController.Panel>();
		for (int i = 0; i < num; i++)
		{
			MissionData missionData = instance.Missions[i];
			if (!missionData.IncludeInStats)
			{
				continue;
			}
			for (int j = 0; j < missionData.Sections.Count; j++)
			{
				if (!missionData.Sections[j].IsValidInCurrentBuild)
				{
					continue;
				}
				PersonalBestStat gameTotalStat = StatsManager.Instance.PersonalBestStats().GetGameTotalStat(StatsManager.MissionStatId(missionData.MissionId, j));
				string empty = string.Empty;
				empty = ((!missionData.Sections[j].Locked) ? Language.Get(missionData.Sections[j].Name).ToUpper() : Language.Get("S_LOCKED"));
				string text = gameTotalStat.MostKills.ToString("N", mNFI);
				string text2 = ((!(gameTotalStat.BestKDR >= 0f)) ? "--" : gameTotalStat.BestKDR.ToString());
				string text3 = gameTotalStat.MostHeadShots.ToString("N", mNFI);
				string text4 = Mathf.Max(gameTotalStat.HighestScoreAsVeteran, gameTotalStat.HighestScore).ToString("N", mNFI);
				string text5 = "--";
				if (gameTotalStat.BestTimeValid())
				{
					text5 = StatsHelper.MinutesToString(gameTotalStat.BestTimeToComplete);
				}
				string[] subtitles = new string[5]
				{
					AutoLocalize.Get("S_MISSION_KILLS"),
					AutoLocalize.Get("S_MISSION_KD_RATIO"),
					AutoLocalize.Get("S_MISSION_HEAD_SHOTS"),
					AutoLocalize.Get("S_MISSION_SCORE"),
					AutoLocalize.Get("S_MISSION_TIME")
				};
				string[] values = new string[5] { text, text2, text3, text4, text5 };
				StatisticsPanel.Type type = StatisticsPanel.Type.MissionLocked;
				if (j >= 0 && j < missionData.Sections.Count)
				{
					SectionData sectionData = missionData.Sections[j];
					if (sectionData != null && !sectionData.Locked)
					{
						type = sectionData.ImageType;
					}
				}
				list.Add(NewPanel(empty, subtitles, values, type));
			}
		}
		mMissionPanels = list.ToArray();
	}

	private string GetCurrentTitleText()
	{
		string textKey = mModeTitles[(int)mMode];
		return AutoLocalize.Get(textKey);
	}

	private StatisticsController.Panel NewPanel(string title, string[] subtitles, string[] values, StatisticsPanel.Type type)
	{
		StatisticsController.Panel panel = new StatisticsController.Panel();
		panel.stats = new string[subtitles.Length];
		panel.values = new string[values.Length];
		panel.title = title;
		int num = Mathf.Max(subtitles.Length, values.Length);
		for (int i = 0; i < num; i++)
		{
			if (i < subtitles.Length)
			{
				panel.stats[i] = subtitles[i];
			}
			if (i < values.Length)
			{
				panel.values[i] = values[i];
			}
		}
		panel.type = type;
		return panel;
	}

	private void Share()
	{
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		XPManager.Instance.ConvertXPToLevel(StatsHelper.PlayerXP(), out level, out prestigeLevel, out xpToNextLevel, out percent);
		string text = Language.Get("S_XPLEVEL_" + level);
		string message = ((prestigeLevel != 0) ? Language.GetFormatString("S_STATS_SOCIAL_MESSAGE_PRESTIGE", text, prestigeLevel) : Language.GetFormatString("S_STATS_SOCIAL_MESSAGE", text));
		m_DialogHelper.PostMessage(message, this, "OnFacebookPost", "OnTwitterPost");
	}

	public void OnFacebookPost()
	{
		SwrveEventsMetaGame.SharedFromStats(SwrveEventsMetaGame.ShareType.Facebook);
		SwrveEventsMetaGame.FacebookBroadcast("Stats", MissionListings.eMissionID.MI_MAX);
	}

	public void OnTwitterPost()
	{
		SwrveEventsMetaGame.SharedFromStats(SwrveEventsMetaGame.ShareType.Twitter);
		SwrveEventsMetaGame.TwitterBroadcast("Stats", MissionListings.eMissionID.MI_MAX);
	}

	private string FindWeaponName(string id)
	{
		string result = id;
		WeaponDescriptor weaponDescriptor = null;
		WeaponManager instance = WeaponManager.Instance;
		if (instance != null)
		{
			weaponDescriptor = FindWeaponDescriptor(id, instance.AssaultRifles);
			if (weaponDescriptor == null)
			{
				weaponDescriptor = FindWeaponDescriptor(id, instance.Shotguns);
			}
			if (weaponDescriptor == null)
			{
				weaponDescriptor = FindWeaponDescriptor(id, instance.SniperRifles);
			}
			if (weaponDescriptor == null)
			{
				weaponDescriptor = FindWeaponDescriptor(id, instance.LightMachineGuns);
			}
			if (weaponDescriptor == null)
			{
				weaponDescriptor = FindWeaponDescriptor(id, instance.SMGs);
			}
		}
		if (weaponDescriptor != null)
		{
			result = weaponDescriptor.Name;
		}
		return result;
	}

	private WeaponDescriptor FindWeaponDescriptor(string id, WeaponDescriptor[] weapons)
	{
		WeaponDescriptor result = null;
		for (int i = 0; i < weapons.Length; i++)
		{
			if (weapons[i].name == id)
			{
				result = weapons[i];
				break;
			}
		}
		return result;
	}
}
