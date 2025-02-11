public class PauseMenuSettingsController : MenuScreenBlade
{
	public MenuScreenBlade AudioBlade;

	public MenuScreenBlade TouchBlade;

	public MenuScreenBlade GamepadBlade;

	public FrontEndButton AudioButton;

	public FrontEndButton TouchButton;

	public FrontEndButton GamepadButton;

	private MenuScreenBlade[] mBlades;

	private FrontEndButton[] mButtons;

	private bool mDirty;

	public override void Awake()
	{
		base.Awake();
		mBlades = new MenuScreenBlade[3] { AudioBlade, TouchBlade, GamepadBlade };
		mButtons = new FrontEndButton[3] { AudioButton, TouchButton, GamepadButton };
		for (int i = 0; i < mBlades.Length; i++)
		{
			mBlades[i].ScreenMoveTime = 0.2f;
		}
	}

	private void ActivateBlade(MenuScreenBlade bladeToActivate)
	{
		if (!CanTransition())
		{
			return;
		}
		MenuScreenBlade currentBlade = GetCurrentBlade();
		if (!(currentBlade != bladeToActivate))
		{
			return;
		}
		if (currentBlade != null)
		{
			MenuScreenBladeTransitionFinishedDelegate callback = delegate(MenuScreenBlade deactivatingBlade, BladeTransition type)
			{
				if (deactivatingBlade != null)
				{
					deactivatingBlade.gameObject.SetActive(false);
				}
				if (CanTransition())
				{
					bladeToActivate.gameObject.SetActive(true);
					bladeToActivate.Activate();
				}
			};
			currentBlade.Deactivate(callback);
		}
		else
		{
			bladeToActivate.gameObject.SetActive(true);
			bladeToActivate.Activate();
		}
	}

	private void OnAudioSettingsButtonPress()
	{
		ActivateBlade(AudioBlade);
		RefreshButtons(AudioButton);
	}

	private void OnGamepadSettingsButtonPress()
	{
		ActivateBlade(GamepadBlade);
		RefreshButtons(GamepadButton);
	}

	private void OnTouchSettingsButtonPress()
	{
		ActivateBlade(TouchBlade);
		RefreshButtons(TouchButton);
	}

	private MenuScreenBlade GetCurrentBlade()
	{
		if (AudioBlade.IsActive)
		{
			return AudioBlade;
		}
		if (TouchBlade.IsActive)
		{
			return TouchBlade;
		}
		if (GamepadBlade.IsActive)
		{
			return GamepadBlade;
		}
		return null;
	}

	public override void Activate()
	{
		base.Activate();
		Refresh();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		mDirty = false;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (mDirty)
		{
			SecureStorage.Instance.SaveGameSettings();
			mDirty = false;
		}
		DeactivateAllBlades();
	}

	private void DeactivateAllBlades()
	{
		MenuScreenBladeTransitionFinishedDelegate callback = delegate(MenuScreenBlade deactivatingBlade, BladeTransition type)
		{
			if (deactivatingBlade != null)
			{
				deactivatingBlade.gameObject.SetActive(false);
			}
		};
		for (int i = 0; i < mBlades.Length; i++)
		{
			if (mBlades[i] != null && (mBlades[i].IsActive || mBlades[i].IsTransitioning))
			{
				mBlades[i].Deactivate(callback);
			}
		}
	}

	private void Refresh()
	{
		AudioBlade.gameObject.SetActive(true);
		AudioBlade.Activate();
		RefreshButtons(AudioButton);
	}

	private void RefreshButtons(FrontEndButton active)
	{
		for (int i = 0; i < mButtons.Length; i++)
		{
			if (mButtons[i] != null)
			{
				mButtons[i].CurrentState = FrontEndButton.State.Normal;
			}
		}
		if (active != null)
		{
			active.CurrentState = FrontEndButton.State.Selected;
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

	private void OnRestoreSettingsButtonPress()
	{
		FrontEndController instance = FrontEndController.Instance;
		MessageBoxController instance2 = MessageBoxController.Instance;
		if (!instance.IsBusy && !instance2.IsAnyMessageActive && instance2 != null)
		{
			instance2.DoConfirmResetGameDialogue(this, "MessageBoxConfirmResetGame");
		}
	}

	private void MessageBoxConfirmResetGame()
	{
		GameSettings.Instance.RestoreDefaultPlayerSettings();
		Refresh();
	}
}
