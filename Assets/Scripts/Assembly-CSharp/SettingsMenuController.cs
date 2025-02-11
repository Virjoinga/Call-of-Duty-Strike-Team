using System;
using UnityEngine;

public class SettingsMenuController : FrontEndScreen
{
	public PercentageEditOption SoundMusic;

	public PercentageEditOption SoundSFX;

	public ToggleEditOption FirstPersonGesturesEnabled;

	public ToggleEditOption FirstPersonGyroscopeEnabled;

	public PercentageEditOption FirstPersonGyroscope;

	public PercentageEditOption FirstPersonXSensitivity;

	public PercentageEditOption FirstPersonYSensitivity;

	public ToggleEditOption FirstPersonInvertTouchX;

	public ToggleEditOption FirstPersonInvertTouchY;

	public ToggleEditOption FirstPersonInvertGyroscopeX;

	public ToggleEditOption FirstPersonInvertGyroscopeY;

	public ToggleEditOption FirstPersonMovableFireButton;

	public PercentageEditOption GamepadSensitivity;

	public ToggleEditOption GamepadInvert;

	public ToggleEditOption NotificationsSale;

	public ToggleEditOption NotificationsFriend;

	public ToggleEditOption NotificationsChallenge;

	public ToggleEditOption EnableTutorials;

	public SpriteText EliteButtonText;

	public SpriteText EliteBenefitText;

	private MenuScreenBlade[] mBlades;

	private MonoBehaviour mReturnMonoBehaviour;

	private string mReturnMethodToCall;

	private bool mDirty;

	private bool mEliteButtonGreyedOut;

	protected override void Awake()
	{
		ID = ScreenID.Settings;
		mEliteButtonGreyedOut = false;
		base.Awake();
	}

	private void OnEnable()
	{
		Bedrock.UserConnectionStatusChanged += HandleConnectionStatusChanged;
		Bedrock.BedrockUIClosed += HandleActivateUIClosed;
		ActivateWatcher.EliteAccountLinked += HandleEliteAccountLinked;
		ActivateWatcher.ActivateUILaunched += OnActivateUILaunched;
	}

	private void OnDisable()
	{
		Bedrock.UserConnectionStatusChanged -= HandleConnectionStatusChanged;
		Bedrock.BedrockUIClosed -= HandleActivateUIClosed;
		ActivateWatcher.EliteAccountLinked -= HandleEliteAccountLinked;
		ActivateWatcher.ActivateUILaunched -= OnActivateUILaunched;
	}

	private void Refresh()
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null)
		{
			Player player = instance.PlayerGameSettings();
			SettingsUtils.SetUpPercentageEditOption(this, SoundMusic, "ChangedMusicVolume", SecureStorage.Instance.MusicVolume, Language.Get("S_SETTINGS_MENU_PROMPT_MUSIC"));
			SettingsUtils.SetUpPercentageEditOption(this, SoundSFX, "ChangedSfxVolume", SecureStorage.Instance.SoundFXVolume, Language.Get("S_SETTINGS_MENU_PROMPT_SFX"));
			SettingsUtils.SetUpToggleEditOption(this, FirstPersonGesturesEnabled, "ChangedFirstPersonGesturesEnabled", player.FirstPersonGesturesEnabled, Language.Get("S_SETTINGS_MENU_PROMPT_GESTURE_CONTROLS"));
			if (!SystemInfo.supportsGyroscope)
			{
				FirstPersonGyroscopeEnabled.SetModifiable(false);
			}
			FirstPersonInvertGyroscopeX.SetModifiable(player.FirstPersonGyroscopeEnabled);
			FirstPersonInvertGyroscopeY.SetModifiable(player.FirstPersonGyroscopeEnabled);
			FirstPersonGyroscope.SetModifiable(player.FirstPersonGyroscopeEnabled);
			SettingsUtils.SetUpToggleEditOption(this, FirstPersonInvertGyroscopeX, "ChangedFirstPersonInvertGyroscopeX", player.FirstPersonInvertGyroscopeX, Language.Get("S_SETTINGS_MENU_PROMPT_INVERT_GYROSCOPE_X"));
			SettingsUtils.SetUpToggleEditOption(this, FirstPersonInvertGyroscopeY, "ChangedFirstPersonInvertGyroscopeY", player.FirstPersonInvertGyroscopeY, Language.Get("S_SETTINGS_MENU_PROMPT_INVERT_GYROSCOPE_Y"));
			SettingsUtils.SetUpPercentageEditOption(this, FirstPersonGyroscope, "ChangedFirstPersonGyroscope", player.FirstPersonGyroscope, Language.Get("S_SETTINGS_MENU_PROMPT_GYROSCOPE"));
			SettingsUtils.SetUpToggleEditOption(this, FirstPersonGyroscopeEnabled, "ChangedFirstPersonGyroscopeEnabled", player.FirstPersonGyroscopeEnabled, Language.Get("S_SETTINGS_MENU_PROMPT_GYROSCOPE_CONTROLS"));
			SettingsUtils.SetUpPercentageEditOption(this, FirstPersonXSensitivity, "ChangedFirstPersonXSensitivity", player.FirstPersonXSensitivity, Language.Get("S_SETTINGS_MENU_PROMPT_X_SENSITIVITY"));
			SettingsUtils.SetUpPercentageEditOption(this, FirstPersonYSensitivity, "ChangedFirstPersonYSensitivity", player.FirstPersonYSensitivity, Language.Get("S_SETTINGS_MENU_PROMPT_Y_SENSITIVITY"));
			SettingsUtils.SetUpToggleEditOption(this, FirstPersonInvertTouchX, "ChangedFirstPersonInvertTouchX", player.FirstPersonInvertTouchX, Language.Get("S_SETTINGS_MENU_PROMPT_INVERT_TOUCH_X"));
			SettingsUtils.SetUpToggleEditOption(this, FirstPersonInvertTouchY, "ChangedFirstPersonInvertTouchY", player.FirstPersonInvertTouchY, Language.Get("S_SETTINGS_MENU_PROMPT_INVERT_TOUCH_Y"));
			SettingsUtils.SetUpPercentageEditOption(this, GamepadSensitivity, "ChangedGamepadSensitivity", player.FirstPersonGamepadSensitivity, Language.Get("S_SETTINGS_MENU_PROMPT_GAMEPAD_SENSITIVITY"));
			SettingsUtils.SetUpToggleEditOption(this, GamepadInvert, "ChangedGamepadInvert", player.FirstPersonInvertGamepadY, Language.Get("S_SETTINGS_MENU_PROMPT_GAMEPAD_INVERT"));
			SettingsUtils.SetUpToggleEditOption(this, NotificationsSale, "ChangedNotificationsSale", player.NotificationsSale, null);
			SettingsUtils.SetUpToggleEditOption(this, NotificationsFriend, "ChangedNotificationsFriend", player.NotificationsFriend, null);
			SettingsUtils.SetUpToggleEditOption(this, NotificationsChallenge, "ChangedNotificationsChallenge", player.NotificationsChallenge, null);
			Debug.Log("Settings Refreshed");
		}
		else
		{
			Debug.LogWarning("Settings Refresh - Game settings invalid");
		}
		if (EliteBenefitText != null)
		{
			char c = CommonHelper.HardCurrencySymbol();
			string arg = string.Format("{0}{1}", c, SwrveServerVariables.Instance.EliteLinkReward.ToString());
			string format = Language.Get("S_ELITE_BENEFIT");
			format = string.Format(format, arg);
			EliteBenefitText.Text = format;
		}
		UpdateEliteButton();
	}

	private void UpdateEliteButton()
	{
		if (EliteButtonText != null && EliteBenefitText != null)
		{
			if (!mEliteButtonGreyedOut && SecureStorage.Instance.EliteAccountLinked)
			{
				EliteButtonText.Text = Language.Get("S_ELITE_ALREADY_LINKED");
				EliteBenefitText.SetColor(ColourChart.GreyedOut);
				mEliteButtonGreyedOut = true;
			}
			else if (mEliteButtonGreyedOut && !SecureStorage.Instance.EliteAccountLinked)
			{
				EliteButtonText.Text = Language.Get("S_SIGN_IN");
				EliteBenefitText.SetColor(ColourChart.HudWhite);
				mEliteButtonGreyedOut = false;
			}
		}
	}

	private void HandleConnectionStatusChanged(object sender, EventArgs e)
	{
		UpdateEliteButton();
	}

	private void HandleEliteAccountLinked(object sender, EventArgs e)
	{
		UpdateEliteButton();
	}

	private void OnActivateUILaunched(object sender, EventArgs args)
	{
		if (FrontEndController.Instance.ActiveScreen != 0)
		{
			FrontEndController.Instance.ReturnToGlobe();
		}
	}

	private void HandleActivateUIClosed(object sender, Bedrock.brUserInterfaceReasonForCloseEventArgs e)
	{
		UpdateEliteButton();
	}

	private void ChangedMusicVolume()
	{
		float value = SoundMusic.Value;
		if (value == 0f)
		{
			SoundManager.Instance.MuteVolumeGroup(SoundFXData.VolumeGroup.Music, true);
			SoundManager.Instance.MuteVolumeGroup(SoundFXData.VolumeGroup.FrontEnd_Music, true);
		}
		else
		{
			if (SoundManager.Instance.VolumeGroups[3].Mute)
			{
				SoundManager.Instance.MuteVolumeGroup(SoundFXData.VolumeGroup.Music, false);
			}
			if (SoundManager.Instance.VolumeGroups[8].Mute)
			{
				SoundManager.Instance.MuteVolumeGroup(SoundFXData.VolumeGroup.FrontEnd_Music, false);
			}
		}
		SoundManager.Instance.SetDefaultVolumeGroup(SoundFXData.VolumeGroup.Music, value);
		SoundManager.Instance.SetDefaultVolumeGroup(SoundFXData.VolumeGroup.FrontEnd_Music, value);
		MusicManager.Instance.RefreshVolumeAfterSettingsChange();
		if (SecureStorage.Instance.MusicVolume != value)
		{
			if (value == 0f)
			{
				SwrveEventsUI.MusicTurnedOff();
			}
			else
			{
				SwrveEventsUI.MusicTurnedOn();
			}
			SecureStorage.Instance.MusicVolume = value;
			InterfaceSFX.Instance.GeneralButtonPress.Play2D();
			mDirty = true;
		}
	}

	private void ChangedSfxVolume()
	{
		float value = SoundSFX.Value;
		if (value == 0f)
		{
			SoundManager.Instance.MuteVolumeGroup(SoundFXData.VolumeGroup.Sfx, true);
			SoundManager.Instance.MuteVolumeGroup(SoundFXData.VolumeGroup.FrontEnd, true);
		}
		else
		{
			if (SoundManager.Instance.VolumeGroups[0].Mute)
			{
				SoundManager.Instance.MuteVolumeGroup(SoundFXData.VolumeGroup.Sfx, false);
			}
			if (SoundManager.Instance.VolumeGroups[2].Mute)
			{
				SoundManager.Instance.MuteVolumeGroup(SoundFXData.VolumeGroup.FrontEnd, false);
			}
		}
		SoundManager.Instance.SetAllDefaultSFXVolumeGroups(value);
		MusicManager.Instance.RefreshVolumeAfterSettingsChange();
		if (SecureStorage.Instance.SoundFXVolume != value)
		{
			if (value == 0f)
			{
				SwrveEventsUI.SFXTurnedOff();
			}
			else
			{
				SwrveEventsUI.SFXTurnedOn();
			}
			SecureStorage.Instance.SoundFXVolume = value;
			InterfaceSFX.Instance.GeneralButtonPress.Play2D();
			mDirty = true;
		}
	}

	private void ChangedFirstPersonMovableFireButton()
	{
	}

	private void ChangedFirstPersonGesturesEnabled()
	{
		bool value = FirstPersonGesturesEnabled.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonGesturesEnabled != value)
		{
			instance.PlayerGameSettings().FirstPersonGesturesEnabled = value;
			if (value)
			{
				SwrveEventsUI.GesturesTurnedOn();
			}
			else
			{
				SwrveEventsUI.GesturesTurnedOff();
			}
			mDirty = true;
		}
	}

	private void ChangedFirstPersonGyroscopeEnabled()
	{
		bool value = FirstPersonGyroscopeEnabled.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonGyroscopeEnabled != value)
		{
			instance.PlayerGameSettings().FirstPersonGyroscopeEnabled = value;
			mDirty = true;
			FirstPersonInvertGyroscopeX.SetModifiable(value);
			FirstPersonInvertGyroscopeY.SetModifiable(value);
			FirstPersonGyroscope.SetModifiable(value);
			if (value)
			{
				SwrveEventsUI.GyroscopeTurnedOn();
			}
			else
			{
				SwrveEventsUI.GyroscopeTurnedOff();
			}
		}
	}

	private void ChangedFirstPersonGyroscope()
	{
		float value = FirstPersonGyroscope.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonGyroscope != value)
		{
			if (instance.PlayerGameSettings().FirstPersonGyroscope != value)
			{
				InterfaceSFX.Instance.GeneralButtonPress.Play2D();
			}
			instance.PlayerGameSettings().FirstPersonGyroscope = value;
			mDirty = true;
		}
	}

	private void ChangedFirstPersonXSensitivity()
	{
		float value = FirstPersonXSensitivity.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonXSensitivity != value)
		{
			if (instance.PlayerGameSettings().FirstPersonXSensitivity != value)
			{
				InterfaceSFX.Instance.GeneralButtonPress.Play2D();
			}
			instance.PlayerGameSettings().FirstPersonXSensitivity = value;
			mDirty = true;
		}
	}

	private void ChangedFirstPersonYSensitivity()
	{
		float value = FirstPersonYSensitivity.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonYSensitivity != value)
		{
			if (instance.PlayerGameSettings().FirstPersonYSensitivity != value)
			{
				InterfaceSFX.Instance.GeneralButtonPress.Play2D();
			}
			instance.PlayerGameSettings().FirstPersonYSensitivity = value;
			mDirty = true;
		}
	}

	private void ChangedFirstPersonInvertTouchX()
	{
		bool value = FirstPersonInvertTouchX.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonInvertTouchX != value)
		{
			instance.PlayerGameSettings().FirstPersonInvertTouchX = value;
			mDirty = true;
		}
	}

	private void ChangedFirstPersonInvertTouchY()
	{
		bool value = FirstPersonInvertTouchY.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonInvertTouchY != value)
		{
			instance.PlayerGameSettings().FirstPersonInvertTouchY = value;
			mDirty = true;
		}
	}

	private void ChangedFirstPersonInvertGyroscopeX()
	{
		bool value = FirstPersonInvertGyroscopeX.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonInvertGyroscopeX != value)
		{
			instance.PlayerGameSettings().FirstPersonInvertGyroscopeX = value;
			mDirty = true;
		}
	}

	private void ChangedFirstPersonInvertGyroscopeY()
	{
		bool value = FirstPersonInvertGyroscopeY.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonInvertGyroscopeY != value)
		{
			instance.PlayerGameSettings().FirstPersonInvertGyroscopeY = value;
			mDirty = true;
		}
	}

	private void ChangedGamepadSensitivity()
	{
		float value = GamepadSensitivity.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonGamepadSensitivity != value)
		{
			instance.PlayerGameSettings().FirstPersonGamepadSensitivity = value;
			mDirty = true;
		}
	}

	private void ChangedGamepadInvert()
	{
		bool value = GamepadInvert.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonInvertGamepadY != value)
		{
			instance.PlayerGameSettings().FirstPersonInvertGamepadY = value;
			mDirty = true;
		}
	}

	private void ChangedNotificationsSale()
	{
		bool value = NotificationsSale.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().NotificationsSale != value)
		{
			instance.PlayerGameSettings().NotificationsSale = value;
			mDirty = true;
		}
	}

	private void ChangedNotificationsFriend()
	{
		bool value = NotificationsFriend.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().NotificationsFriend != value)
		{
			instance.PlayerGameSettings().NotificationsFriend = value;
			mDirty = true;
		}
	}

	private void ChangedNotificationsChallenge()
	{
		bool value = NotificationsChallenge.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().NotificationsChallenge != value)
		{
			instance.PlayerGameSettings().NotificationsChallenge = value;
			mDirty = true;
		}
	}

	public override void EnterScreen()
	{
		base.EnterScreen();
		PercentageEditOption[] componentsInChildren = GetComponentsInChildren<PercentageEditOption>();
		PercentageEditOption[] array = componentsInChildren;
		foreach (PercentageEditOption percentageEditOption in array)
		{
			percentageEditOption.Enable();
		}
		Refresh();
		mDirty = false;
	}

	public override void ExitScreen()
	{
		base.ExitScreen();
		PercentageEditOption[] componentsInChildren = GetComponentsInChildren<PercentageEditOption>();
		PercentageEditOption[] array = componentsInChildren;
		foreach (PercentageEditOption percentageEditOption in array)
		{
			percentageEditOption.Disable();
		}
		if (mDirty)
		{
			SecureStorage.Instance.SaveGameSettings();
		}
	}

	private void SignInToElite()
	{
		if (!SecureStorage.Instance.EliteAccountLinked)
		{
			if (Bedrock.getUserConnectionStatus() == Bedrock.brUserConnectionStatus.BR_LOGGED_IN_REGISTERED_ONLINE)
			{
				Debug.LogError("RKTODO 07/10/14");
				SwrveEventsUI.LinkedAccountPressed();
			}
			else
			{
				MessageBoxController.Instance.NeedToBeOnline();
			}
		}
	}

	private void CreditsPressed()
	{
	}

	private void ResetGamePressed()
	{
		MessageBoxController.Instance.DoConfirmResetGameDialogue(this, "MessageBoxResultConfirmResetGame");
	}

	private void RestorePurchasesPressed()
	{
	}

	private void MessageBoxResultConfirmResetGame()
	{
		GameSettings.Instance.RestoreDefaultPlayerSettings();
		Refresh();
	}
}
