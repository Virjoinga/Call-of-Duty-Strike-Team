public class PauseMenuGamepadSettingsController : MenuScreenBlade
{
	public PercentageEditOption GamepadSensitivity;

	public ToggleEditOption GamepadInvert;

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
			SettingsUtils.SetUpPercentageEditOption(this, GamepadSensitivity, "ChangedGamepadSensitivity", player.FirstPersonGamepadSensitivity, Language.Get("S_SETTINGS_MENU_PROMPT_GAMEPAD_SENSITIVITY"));
			SettingsUtils.SetUpToggleEditOption(this, GamepadInvert, "ChangedGamepadInvert", player.FirstPersonInvertGamepadY, Language.Get("S_SETTINGS_MENU_PROMPT_GAMEPAD_INVERT"));
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
}
