using UnityEngine;

public class PauseMenuTouchSettingsController : MenuScreenBlade
{
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

	private bool mDirty;

	public override void Activate()
	{
		base.Activate();
		Refresh();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		PercentageEditOption[] componentsInChildren = GetComponentsInChildren<PercentageEditOption>();
		PercentageEditOption[] array = componentsInChildren;
		foreach (PercentageEditOption percentageEditOption in array)
		{
			percentageEditOption.Enable();
		}
		mDirty = false;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		PercentageEditOption[] componentsInChildren = GetComponentsInChildren<PercentageEditOption>();
		PercentageEditOption[] array = componentsInChildren;
		foreach (PercentageEditOption percentageEditOption in array)
		{
			percentageEditOption.Disable();
		}
		if (mDirty)
		{
			SecureStorage.Instance.SaveGameSettings();
			mDirty = false;
		}
	}

	private void Refresh()
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null)
		{
			Player player = instance.PlayerGameSettings();
			SettingsUtils.SetUpToggleEditOption(this, FirstPersonGesturesEnabled, "ChangedFirstPersonGesturesEnabled", player.FirstPersonGesturesEnabled, Language.Get("S_SETTINGS_MENU_PROMPT_GESTURE_CONTROLS"));
			SettingsUtils.SetUpToggleEditOption(this, FirstPersonMovableFireButton, "ChangedFirstPersonMovableFireButton", player.FirstPersonMovableFireButton, Language.Get("S_SETTINGS_MENU_PROMPT_MOVABLE_FIRE_BUTTON"));
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
		}
	}

	private void ChangedFirstPersonMovableFireButton()
	{
		bool value = FirstPersonMovableFireButton.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonMovableFireButton != value)
		{
			instance.PlayerGameSettings().FirstPersonMovableFireButton = value;
			FrontEndController.Instance.TransitionTo(ScreenID.HUDEditScreen);
			mDirty = true;
		}
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
			mDirty = true;
		}
	}

	private void ChangedFirstPersonGyroscope()
	{
		float value = FirstPersonGyroscope.Value;
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonGyroscope != value)
		{
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
}
