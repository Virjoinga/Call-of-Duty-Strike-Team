using System;
using UnityEngine;

public class HudStateController : MonoBehaviour
{
	public enum HudState
	{
		Invalid = 0,
		Hidden = 1,
		FPP = 2,
		FPPLockedMountedWeapon = 3,
		TPP = 4,
		Strategy = 5,
		Overwatch = 6
	}

	private GameObject mCommonHud;

	private GameObject mStrategyHud;

	private GameObject mFirstPersonHud;

	private HudState mCachedState;

	private static HudStateController smInstance;

	public HudState State { get; private set; }

	public static HudStateController Instance
	{
		get
		{
			return smInstance;
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple HudStateController");
		}
		smInstance = this;
	}

	public void SetCommonHudCtrl(GameObject go)
	{
		mCommonHud = go;
	}

	public void SetFirstPeronHudCtrl(GameObject go)
	{
		mFirstPersonHud = go;
	}

	public void SetStrategyHudCtrl(GameObject go)
	{
		mStrategyHud = go;
	}

	private void Start()
	{
	}

	public void SetState(HudState state)
	{
		if (OverwatchController.Instance != null && OverwatchController.Instance.Active && state != HudState.Overwatch && state != HudState.Hidden)
		{
			Debug.LogWarning("Overwatch has been requested to have its HUD state changed whilst active - This will potentially leave the game unplayable!");
			return;
		}
		if (state != State)
		{
			switch (state)
			{
			default:
				TBFAssert.DoAssert(false, "invalid hud state to set - " + state);
				break;
			case HudState.Hidden:
				HideAllHud();
				break;
			case HudState.FPP:
				SetupHudForFPP();
				break;
			case HudState.FPPLockedMountedWeapon:
				SetupHudForFPPLockedMountedWeapon();
				break;
			case HudState.TPP:
				SetupHudForTPP();
				break;
			case HudState.Strategy:
				SetupHudForStrategy();
				break;
			case HudState.Overwatch:
				SetupHudForOverwatch();
				break;
			}
		}
		State = state;
	}

	private void Update()
	{
	}

	private void HideAllHud()
	{
		CommonHudController.Instance.ClearContextMenu();
		mFirstPersonHud.SetActive(false);
		mStrategyHud.SetActive(false);
		mCommonHud.SetActive(false);
		ToggleHighlights(false);
	}

	private void SetupHudForFPP()
	{
		CommonHudController.Instance.ClearContextMenu();
		mStrategyHud.SetActive(false);
		mFirstPersonHud.SetActive(true);
		mCommonHud.SetActive(true);
		CommonHudController.Instance.SetHoldBreathUI(false, 0f);
		CommonHudController.Instance.HideCrouchButton(false);
		CommonHudController.Instance.HideADSButton(false);
		CommonHudController.Instance.HideFollowMe(false);
		CommonHudController.Instance.HideScreenEdging(true);
		CommonHudController.Instance.HideOnScreenContextMenu(true);
		CommonHudController.Instance.HideTPPUnitSelecter(true);
		CommonHudController.Instance.HideContextInteractionButton(true);
		CommonHudController.Instance.SetFPPThrowWeaponButton(CommonHudController.Instance.CurrentFPPThrowingWeapon);
		ToggleHighlights(true);
	}

	private void SetupHudForFPPLockedMountedWeapon()
	{
		SetupHudForFPP();
		CommonHudController.Instance.HideFollowMe(true);
		CommonHudController.Instance.HideCrouchButton(true);
		ToggleHighlights(true);
	}

	private void SetupHudForTPP()
	{
		mStrategyHud.SetActive(false);
		mFirstPersonHud.SetActive(false);
		mCommonHud.SetActive(true);
		CommonHudController.Instance.HideScreenEdging(false);
		CommonHudController.Instance.HideOnScreenContextMenu(false);
		CommonHudController.Instance.HideTPPUnitSelecter(false);
		ToggleHighlights(true);
	}

	private void SetupHudForStrategy()
	{
		CommonHudController.Instance.ClearContextMenu();
		mFirstPersonHud.SetActive(false);
		mStrategyHud.SetActive(true);
		mCommonHud.SetActive(true);
		CommonHudController.Instance.HideScreenEdging(false);
		CommonHudController.Instance.HideOnScreenContextMenu(true);
		CommonHudController.Instance.HideTPPUnitSelecter(true);
	}

	private void SetupHudForOverwatch()
	{
		CommonHudController.Instance.ClearContextMenu();
		mFirstPersonHud.SetActive(false);
		mStrategyHud.SetActive(true);
		mCommonHud.SetActive(true);
		CommonHudController.Instance.HideScreenEdging(false);
		CommonHudController.Instance.HideOnScreenContextMenu(true);
		CommonHudController.Instance.HideTPPUnitSelecter(true);
		StrategyHudController instance = StrategyHudController.Instance;
		instance.ShowOverwatchShootingHUD = true;
	}

	public void ToggleHighlights(bool OnOff)
	{
		if (TutorialToggles.ActiveHighlights != null && TutorialToggles.ActiveHighlights.Count > 0)
		{
			if (OnOff && TutorialToggles.ActiveHighlightStates != null && TutorialToggles.ActiveHighlightStates.Length > 0)
			{
				for (int i = 0; i < TutorialToggles.ActiveHighlights.Count; i++)
				{
					if (TutorialToggles.ActiveHighlights[i] != null)
					{
						TutorialToggles.ActiveHighlights[i].SetActive(TutorialToggles.ActiveHighlightStates[i]);
					}
				}
				TutorialToggles.ActiveHighlightStates = null;
			}
			else
			{
				if (OnOff)
				{
					return;
				}
				TutorialToggles.ActiveHighlightStates = new bool[TutorialToggles.ActiveHighlights.Count];
				for (int j = 0; j < TutorialToggles.ActiveHighlights.Count; j++)
				{
					if (TutorialToggles.ActiveHighlights[j] != null)
					{
						TutorialToggles.ActiveHighlightStates[j] = TutorialToggles.ActiveHighlights[j].activeInHierarchy;
						TutorialToggles.ActiveHighlights[j].SetActive(false);
					}
				}
			}
		}
		else
		{
			TutorialToggles.ActiveHighlightStates = null;
		}
	}

	public void StoreStateAndHide()
	{
		mCachedState = State;
		SetState(HudState.Hidden);
	}

	public void RestoreState()
	{
		if (mCachedState != 0)
		{
			SetState(mCachedState);
			mCachedState = HudState.Invalid;
		}
	}
}
