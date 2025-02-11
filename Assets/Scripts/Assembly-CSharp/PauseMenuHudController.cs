using System;
using UnityEngine;

public class PauseMenuHudController : FrontEndScreen
{
	private enum Mode
	{
		MissionObjectives = 0,
		MissionMedals = 1,
		GameSettings = 2,
		Challenges = 3,
		DebugSettings = 4
	}

	private const float TIME_TO_FADE = 0.2f;

	private static PauseMenuHudController smInstance;

	public MenuScreenBlade ObjectivesBlade;

	public MenuScreenBlade MedalsBlade;

	public MenuScreenBlade SettingsBlade;

	public MenuScreenBlade ChallengesBlade;

	public MenuScreenBlade DebugBlade;

	public MenuScreenBlade SidePanel;

	public MenuScreenBlade ScreenBackground;

	public FrontEndButton ObjectivesButton;

	public FrontEndButton MedalsButton;

	public FrontEndButton SettingsButton;

	public FrontEndButton ChallengesButton;

	public FrontEndButton ResetSettings;

	public FrontEndButton DebugButton;

	private MenuScreenBlade[] mBlades;

	private Mode mMode;

	private bool mSideMenuNeedsRefresh;

	private bool mQuittingOrRestarting;

	public static PauseMenuHudController Instance
	{
		get
		{
			return smInstance;
		}
	}

	protected override void Awake()
	{
		ID = ScreenID.Pause;
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple PauseMenuHudController");
		}
		smInstance = this;
		ChallengesButton.gameObject.SetActive(false);
		Transform transform = base.transform.Find("SideBlade/MenuPanel");
		if ((bool)transform)
		{
			Transform transform2 = transform.Find("Content");
			if (transform2 != null)
			{
				foreach (Transform item in transform2)
				{
					item.localPosition = Vector3.zero;
				}
			}
			CommonBackgroundBox componentInChildren = transform.GetComponentInChildren<CommonBackgroundBox>();
			componentInChildren.ForegroundHeightInUnits = 4.6f;
			CommonBackgroundBoxPlacement component = ObjectivesButton.GetComponent<CommonBackgroundBoxPlacement>();
			component.StartPositionAsPercentageOfBoxHeight = 0.04f;
			CommonBackgroundBoxPlacement component2 = MedalsButton.GetComponent<CommonBackgroundBoxPlacement>();
			component2.StartPositionAsPercentageOfBoxHeight = 0.3733333f;
			CommonBackgroundBoxPlacement component3 = SettingsButton.GetComponent<CommonBackgroundBoxPlacement>();
			component3.StartPositionAsPercentageOfBoxHeight = 53f / 75f;
		}
		if (DebugButton != null)
		{
			DebugButton.gameObject.SetActive(false);
			ResetSettings.gameObject.SetActive(false);
		}
		if (DebugBlade != null)
		{
			DebugBlade.gameObject.SetActive(false);
			DebugBlade = null;
		}
		mBlades = new MenuScreenBlade[4] { ObjectivesBlade, MedalsBlade, SettingsBlade, ChallengesBlade };
		base.Awake();
	}

	protected override void Update()
	{
		base.Update();
		if (mSideMenuNeedsRefresh)
		{
			AnimateCommonBackgroundBox[] componentsInChildren = SidePanel.GetComponentsInChildren<AnimateCommonBackgroundBox>();
			bool flag = true;
			AnimateCommonBackgroundBox[] array = componentsInChildren;
			foreach (AnimateCommonBackgroundBox animateCommonBackgroundBox in array)
			{
				if (!animateCommonBackgroundBox.IsOpen)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				RefreshSideMenu();
				mSideMenuNeedsRefresh = false;
			}
		}
		UpdatePadInput();
	}

	public override void EnterScreen()
	{
		base.EnterScreen();
		if (ScreenBackground != null && !ScreenBackground.IsActive)
		{
			ScreenBackground.Activate();
		}
		if (FrontEndController.Instance.PreviousScreen == ScreenID.None)
		{
			TimeManager.instance.PauseGame();
		}
		mMode = Mode.MissionObjectives;
		if (ObjectivesBlade != null)
		{
			ObjectivesBlade.DelayedActivate(0.25f);
		}
		TitleBarController instance = TitleBarController.Instance;
		MissionListings instance2 = MissionListings.Instance;
		ActStructure instance3 = ActStructure.Instance;
		MissionListings.eMissionID id = ((instance3.CurrentMissionID == MissionListings.eMissionID.MI_MAX) ? instance3.LastMissionID : instance3.CurrentMissionID);
		MissionData missionData = instance2.Mission(id);
		if (missionData != null)
		{
			instance.SetCustomTitle(AutoLocalize.Get(missionData.NameKey).ToUpper());
		}
		mSideMenuNeedsRefresh = true;
	}

	public override void ExitScreen()
	{
		base.ExitScreen();
		DeactivateAllScreens();
	}

	public override void OnScreen()
	{
		base.OnScreen();
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController != null)
		{
			gameplayController.CancelAnyPrimedGrenade();
			gameplayController.CancelAnyPlacement();
		}
	}

	public override void OffScreen()
	{
		if (!mQuittingOrRestarting && FrontEndController.Instance.ActiveScreen == ScreenID.None)
		{
			GameController.Instance.TogglePause();
			TimeManager.instance.UnpauseGame();
		}
		if (ScreenBackground != null && ScreenBackground.IsActive)
		{
			ScreenBackground.Deactivate();
		}
	}

	private void OnEnable()
	{
		EnableAllScreens(true);
	}

	private void OnDisable()
	{
		EnableAllScreens(false);
	}

	public void GotoObjectives()
	{
		OnObjectivesButtonPress();
	}

	private void OnObjectivesButtonPress()
	{
		if (mMode != 0 && CanTransition())
		{
			MenuScreenBlade currentScreen = GetCurrentScreen();
			if (currentScreen != null && currentScreen.IsActive)
			{
				currentScreen.Deactivate(ShowObjectivesScreen);
			}
			mMode = Mode.MissionObjectives;
			mSideMenuNeedsRefresh = true;
		}
	}

	private void OnMedalsButtonPress()
	{
		if (mMode != Mode.MissionMedals && CanTransition())
		{
			MenuScreenBlade currentScreen = GetCurrentScreen();
			if (currentScreen != null && currentScreen.IsActive)
			{
				currentScreen.Deactivate(ShowMedalsScreen);
			}
			mMode = Mode.MissionMedals;
			mSideMenuNeedsRefresh = true;
		}
	}

	private void OnSettingsButtonPress()
	{
		if (mMode != Mode.GameSettings && CanTransition())
		{
			MenuScreenBlade currentScreen = GetCurrentScreen();
			if (currentScreen != null && currentScreen.IsActive)
			{
				currentScreen.Deactivate(ShowSettingsScreen);
			}
			mMode = Mode.GameSettings;
			mSideMenuNeedsRefresh = true;
			SwrveEventsUI.ViewedOptions();
		}
	}

	private void OnChallengesButtonPress()
	{
		if (mMode != Mode.Challenges && CanTransition())
		{
			MenuScreenBlade currentScreen = GetCurrentScreen();
			if (currentScreen != null && currentScreen.IsActive)
			{
				currentScreen.Deactivate(ShowChallengeScreen);
			}
			mMode = Mode.Challenges;
			mSideMenuNeedsRefresh = true;
			SwrveEventsUI.ViewedChallenges();
		}
	}

	private void OnDebugButtonPress()
	{
	}

	private void OnQuitButtonPress()
	{
		FrontEndController instance = FrontEndController.Instance;
		MessageBoxController instance2 = MessageBoxController.Instance;
		if (!instance.IsBusy && !instance2.IsAnyMessageActive && instance2 != null)
		{
			instance2.DoConfirmQuitDialogue(this, "MessageBoxResultConfirmQuit", "MessageBoxCancelled");
		}
	}

	private void OnReplayButtonPress()
	{
		FrontEndController instance = FrontEndController.Instance;
		MessageBoxController instance2 = MessageBoxController.Instance;
		if (!instance.IsBusy && !instance2.IsAnyMessageActive && instance2 != null)
		{
			instance2.DoConfirmRestartDialogue(this, "MessageBoxResultConfirmRestart", "MessageBoxCancelled");
		}
	}

	private void OnContinueButtonPress()
	{
		FrontEndController instance = FrontEndController.Instance;
		MessageBoxController instance2 = MessageBoxController.Instance;
		if (!instance.IsBusy && !instance2.IsAnyMessageActive)
		{
			mQuittingOrRestarting = false;
			FrontEndController.Instance.TransitionTo(ScreenID.None);
		}
	}

	private void MessageBoxResultConfirmQuit()
	{
		mQuittingOrRestarting = true;
		FrontEndController.Instance.ConfirmQuit();
	}

	private void MessageBoxCancelled()
	{
	}

	private void MessageBoxResultConfirmRestart()
	{
		mQuittingOrRestarting = true;
		FrontEndController.Instance.ConfirmRestart();
	}

	private void RefreshSideMenu()
	{
		UpdateButton(ObjectivesButton, mMode == Mode.MissionObjectives);
		UpdateButton(MedalsButton, mMode == Mode.MissionMedals);
		UpdateButton(SettingsButton, mMode == Mode.GameSettings);
		UpdateButton(ChallengesButton, mMode == Mode.Challenges);
	}

	private void UpdateButton(FrontEndButton button, bool selected)
	{
		if (button != null)
		{
			button.CurrentState = (selected ? FrontEndButton.State.Selected : FrontEndButton.State.Normal);
		}
	}

	private bool CanTransition()
	{
		for (int i = 0; i < mBlades.Length; i++)
		{
			if (mBlades[i] != null && mBlades[i].IsTransitioning)
			{
				return false;
			}
		}
		if (FrontEndController.Instance.IsBusy || FrontEndController.Instance.ActiveScreen != ScreenID.Pause)
		{
			return false;
		}
		return true;
	}

	private void ShowObjectivesScreen(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
		if (CanTransition() && ObjectivesBlade != null)
		{
			ObjectivesBlade.Activate();
		}
	}

	private void ShowMedalsScreen(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
		if (CanTransition())
		{
			PauseMenuMedalsController pauseMenuMedalsController = MedalsBlade as PauseMenuMedalsController;
			if (pauseMenuMedalsController != null)
			{
				pauseMenuMedalsController.Activate();
				pauseMenuMedalsController.BeginMedalSequence(0.5f);
			}
		}
	}

	private void ShowSettingsScreen(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
		if (CanTransition())
		{
			SettingsBlade.Activate();
		}
	}

	private void ShowChallengeScreen(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
		if (CanTransition())
		{
			ChallengesBlade.Activate();
		}
	}

	private void ShowDebugScreen(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
	}

	private MenuScreenBlade GetCurrentScreen()
	{
		MenuScreenBlade result = null;
		if (mMode == Mode.MissionObjectives)
		{
			result = ObjectivesBlade;
		}
		else if (mMode == Mode.MissionMedals)
		{
			result = MedalsBlade;
		}
		else if (mMode == Mode.GameSettings)
		{
			result = SettingsBlade;
		}
		else if (mMode == Mode.Challenges)
		{
			result = ChallengesBlade;
		}
		return result;
	}

	private void DeactivateAllScreens()
	{
		for (int i = 0; i < mBlades.Length; i++)
		{
			if (mBlades[i] != null && (mBlades[i].IsActive || mBlades[i].IsTransitioning))
			{
				mBlades[i].Deactivate();
			}
		}
	}

	private void EnableAllScreens(bool enable)
	{
		for (int i = 0; i < mBlades.Length; i++)
		{
			if (mBlades[i] != null)
			{
				mBlades[i].gameObject.SetActive(enable);
			}
		}
	}

	private void UpdatePadInput()
	{
		Controller.State state = Controller.GetState();
		if (state.connected && SwrveServerVariables.Instance.AllowGCController && state.pause)
		{
			OnContinueButtonPress();
		}
	}
}
