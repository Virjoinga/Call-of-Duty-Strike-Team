public class PauseMenuAudioSettingsController : MenuScreenBlade
{
	public PercentageEditOption SoundMusic;

	public PercentageEditOption SoundSFX;

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
			SettingsUtils.SetUpPercentageEditOption(this, SoundMusic, "ChangedMusicVolume", SecureStorage.Instance.MusicVolume, Language.Get("S_SETTINGS_MENU_PROMPT_MUSIC"));
			SettingsUtils.SetUpPercentageEditOption(this, SoundSFX, "ChangedSfxVolume", SecureStorage.Instance.SoundFXVolume, Language.Get("S_SETTINGS_MENU_PROMPT_SFX"));
		}
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
		InterfaceSFX.Instance.GeneralButtonPress.Play2D();
		if (SecureStorage.Instance.MusicVolume != value)
		{
			SecureStorage.Instance.MusicVolume = value;
			if (value == 0f)
			{
				SwrveEventsUI.MusicTurnedOff();
			}
			else
			{
				SwrveEventsUI.MusicTurnedOn();
			}
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
		InterfaceSFX.Instance.GeneralButtonPress.Play2D();
		if (SecureStorage.Instance.SoundFXVolume != value)
		{
			SecureStorage.Instance.SoundFXVolume = value;
			if (value == 0f)
			{
				SwrveEventsUI.SFXTurnedOff();
			}
			else
			{
				SwrveEventsUI.SFXTurnedOn();
			}
			mDirty = true;
		}
	}
}
