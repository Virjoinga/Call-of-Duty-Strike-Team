using System;
using System.Collections;
using UnityEngine;

public class GlobeSelectNavigator : MonoBehaviour
{
	public GlobeCamera GlobeSelectCamera;

	public MissionOverviewController MissionOverviewCtrl;

	public MissionSelectController MissionSelectCtrl;

	private CreditsController mCredits;

	private SelectableMissionMarker m_CurrentSelectedMission;

	private bool mStartingMissionHighlighted;

	private static GlobeSelectNavigator mInstance;

	private bool mCheckingEliteApp;

	public static GlobeSelectNavigator Instance
	{
		get
		{
			return mInstance;
		}
	}

	public static event EventHandler<EventArgs> BackToMissionSelect;

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
		}
		else
		{
			Debug.LogError("Should not have two GlobeSelectNavigator components");
		}
	}

	private void Start()
	{
		if (GlobeSelectCamera == null)
		{
			GlobeSelectCamera = UnityEngine.Object.FindObjectOfType(typeof(GlobeCamera)) as GlobeCamera;
		}
		mCredits = UnityEngine.Object.FindObjectOfType(typeof(CreditsController)) as CreditsController;
		CheckEliteApp();
	}

	private void Update()
	{
		if (MissionOverviewCtrl != null && MissionSelectCtrl != null && GlobeSelectCamera != null)
		{
			if (GlobeSelectCamera.FocusMission != null && m_CurrentSelectedMission != GlobeSelectCamera.FocusMission)
			{
				ShowMissionDetail(GlobeSelectCamera.FocusMission);
				m_CurrentSelectedMission = GlobeSelectCamera.FocusMission;
			}
			bool flag = true;
			switch (FrontEndController.Instance.ActiveScreen)
			{
			case ScreenID.MissionSelect:
				if (GlobeSelectCamera.IsZoomedIn())
				{
					GlobeSelectCamera.ZoomOut();
					m_CurrentSelectedMission = null;
				}
				flag = FrontEndController.Instance.IsBusy;
				break;
			case ScreenID.MissionOverview:
				if (m_CurrentSelectedMission != null && !GlobeSelectCamera.IsZoomedIn())
				{
					GlobeSelectCamera.ZoomIn();
				}
				break;
			}
			if (!flag)
			{
				flag = MessageBoxController.Instance.IsAnyMessageActive;
			}
			GlobeSelectCamera.BlockInput(flag);
			if (!mStartingMissionHighlighted)
			{
				if (StatsHelper.AllCampaignMissionsComplete())
				{
					mStartingMissionHighlighted = true;
					return;
				}
				MessageBoxController instance = MessageBoxController.Instance;
				if (instance != null && !instance.IsAnyMessageActive && GameSettings.Instance != null)
				{
					MissionListings.eMissionID highlightMissionID = GameSettings.Instance.HighlightMissionID;
					SelectableMissionMarker selectableMissionMarker = null;
					SelectableMissionMarker[] array = UnityEngine.Object.FindObjectsOfType(typeof(SelectableMissionMarker)) as SelectableMissionMarker[];
					if (array != null && highlightMissionID != MissionListings.eMissionID.MI_MAX)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i] != null && array[i].Data != null && array[i].Data.MissionId == highlightMissionID)
							{
								selectableMissionMarker = array[i];
								break;
							}
						}
					}
					if (selectableMissionMarker != null)
					{
						GlobeSelectCamera.HighlightMission(selectableMissionMarker);
						selectableMissionMarker.SetBlipHighlighted(true);
						mStartingMissionHighlighted = true;
					}
				}
			}
		}
		bool flag2 = false;
		bool flag3 = StatsHelper.CurrentMissionTimesSucceeded() == 1;
		if (!FrontEndController.Instance.IsBusy && ActStructure.Instance.GameJustCompleted && flag3)
		{
			flag2 = true;
		}
		if (flag2 && mCredits != null)
		{
			mCredits.BeginSequence();
			GlobeSelectCamera.ClearFocusMission();
			ActStructure.Instance.GameJustCompleted = false;
		}
	}

	public void ShowMissionSelect()
	{
		FrontEndController.Instance.TransitionTo(ScreenID.MissionSelect);
	}

	public void GotoMission(MissionListings.eMissionID missionId)
	{
		if (GlobeSelectCamera != null)
		{
			GlobeSelectCamera.GotoMission(missionId);
		}
	}

	public void ShowMissionDetail(SelectableMissionMarker data)
	{
		if (data.Data.Type == MissionData.eType.MT_STORY && data.Data.IsLocked())
		{
			return;
		}
		if (data.Data.Type == MissionData.eType.MT_DEMO)
		{
			if (mCredits != null && GlobeSelectCamera != null)
			{
				mCredits.BeginSequence();
				GlobeSelectCamera.ClearFocusMission();
			}
		}
		else if (data.Data.Type == MissionData.eType.MT_KINVITE)
		{
			if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.EnableKInvite) && SwrveServerVariables.Instance.KInviteEnabled)
			{
				TBFUtils.PressKInvite();
				GlobeSelectCamera.ClearFocusMission();
			}
		}
		else if (FrontEndController.Instance.ActiveScreen != ScreenID.MissionOverview)
		{
			if (FrontEndController.Instance.TransitionTo(ScreenID.MissionOverview))
			{
				MissionOverviewCtrl.SetData(data);
			}
		}
		else
		{
			MissionOverviewCtrl.SetData(data);
		}
	}

	public void StatsButtonPressed()
	{
		SwrveEventsUI.ViewedStats();
		FrontEndController.Instance.TransitionTo(ScreenID.Statistics);
	}

	public void SettingsButtonPressed()
	{
		SwrveEventsUI.ViewedOptions();
		FrontEndController.Instance.TransitionTo(ScreenID.Settings);
	}

	public void ChallengesButtonPressed()
	{
		SwrveEventsUI.ViewedChallenges();
		FrontEndController.Instance.TransitionTo(ScreenID.ChallengeSelect);
	}

	public void LeaderboardButtonPressed()
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

	public void FAQButtonPressed()
	{
		SwrveEventsUI.ViewedFAQ();
		ActivateWatcher.Instance.LaunchActivate(Bedrock.brUserInterfaceScreen.BR_CUSTOMER_SERVICE_UI);
	}

	private void OnEnable()
	{
		ActivateWatcher.UserLoggedOn += HandleUserLoggedOn;
		ActivateWatcher.UserLoggedOff += HandleUserLoggedOff;
		ActivateWatcher.ConnectionStatusChange += HandleConnectionStatusChange;
		ActivateWatcher.ActivateUILaunched += OnActivateLaunched;
		ActStructure.OnLoad += OnBlipsUpdated;
		Bedrock.UserResourcesChanged += HandleBedrockUserResourcesChanged;
		MessageBoxController instance = MessageBoxController.Instance;
		if (instance != null)
		{
			instance.SeeIfDeviceMessageShouldBeDisplayed();
		}
	}

	private void HandleBedrockUserResourcesChanged(object sender, EventArgs e)
	{
		MessageBoxController.Instance.SeeIfDeviceMessageShouldBeDisplayed();
	}

	private void OnDisable()
	{
		ActivateWatcher.UserLoggedOn -= HandleUserLoggedOn;
		ActivateWatcher.UserLoggedOff -= HandleUserLoggedOff;
		ActivateWatcher.ConnectionStatusChange -= HandleConnectionStatusChange;
		ActivateWatcher.ActivateUILaunched -= OnActivateLaunched;
		ActStructure.OnLoad -= OnBlipsUpdated;
		Bedrock.UserResourcesChanged -= HandleBedrockUserResourcesChanged;
	}

	private void HandleUserLoggedOn(object sender, LogOnEventArgs e)
	{
	}

	private void HandleUserLoggedOff(object sender, EventArgs e)
	{
	}

	private void OnBlipsUpdated(object sender, EventArgs e)
	{
		SelectableMissionMarker.RefreshAllAfterUnlock();
		RefreshBlipHighlight();
	}

	private void OnActivateLaunched(object sender, EventArgs args)
	{
		if (FrontEndController.Instance.ActiveScreen == ScreenID.MissionSelect)
		{
			Time.timeScale = 0f;
		}
	}

	private void HandleConnectionStatusChange(object sender, ConnectionStatusChangeEventArgs e)
	{
	}

	public void RefreshBlipHighlight()
	{
		MissionListings.eMissionID eMissionID = GameSettings.Instance.HighlightMissionID;
		FrontEndController instance = FrontEndController.Instance;
		if (instance != null && instance.IsFlashpointBannerShowing)
		{
			int flashpointBannerShowingIndex = instance.FlashpointBannerShowingIndex;
			GlobalUnrestController instance2 = GlobalUnrestController.Instance;
			if (instance2 != null)
			{
				MissionData missionData = instance2.GetMissionData(flashpointBannerShowingIndex);
				if (missionData != null)
				{
					eMissionID = missionData.MissionId;
				}
			}
		}
		SelectableMissionMarker[] array = UnityEngine.Object.FindObjectsOfType(typeof(SelectableMissionMarker)) as SelectableMissionMarker[];
		if (array == null || eMissionID == MissionListings.eMissionID.MI_MAX)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].Data != null && array[i].Data.MissionId == eMissionID)
			{
				GlobeSelectCamera.HighlightMission(array[i]);
				array[i].SetBlipHighlighted(true);
			}
			else if (array[i] != null)
			{
				array[i].SetBlipHighlighted(false);
			}
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			CheckEliteApp();
		}
	}

	private void CheckEliteApp()
	{
		if (!mCheckingEliteApp)
		{
			Debug.Log("Checking Elite App");
			if (SecureStorage.Instance.EliteAppRewarded)
			{
				Debug.Log("Elite app reward already given");
				return;
			}
			if (!TBFUtils.CanOpenURL(SwrveServerVariables.Instance.EliteAppURL))
			{
				Debug.Log("Elite app is not installed");
				return;
			}
			Debug.Log("Elite app is installed");
			mCheckingEliteApp = true;
			StartCoroutine(DoCheckEliteApp());
		}
	}

	private IEnumerator DoCheckEliteApp()
	{
		yield return new WaitForSeconds(2f);
		while (MessageBoxController.Instance.IsAnyMessageActive)
		{
			yield return new WaitForEndOfFrame();
		}
		MessageBoxController.Instance.DoEliteAppInstalledDialogue(this, "OnOkPressed");
	}

	private void OnOkPressed()
	{
		mCheckingEliteApp = false;
		SecureStorage.Instance.EliteAppRewarded = true;
		SwrveEventsUI.EliteAppInstalled();
		GameSettings.Instance.PlayerCash().AwardHardCashFreebie(SwrveServerVariables.Instance.EliteAppInstalledReward, "EliteAppInstalled");
	}

	private void MoreGamesButtonPressed()
	{
		TBFUtils.LaunchURL(SwrveServerVariables.Instance.MoreGamesURL);
		SwrveEventsUI.ViewedMoreGames();
	}

	private void CancelledTBFInfoMessage()
	{
		SwrveEventsUI.CancelledTBFInfoMessage();
	}
}
