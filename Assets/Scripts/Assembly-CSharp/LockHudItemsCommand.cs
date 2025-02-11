using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockHudItemsCommand : Command
{
	public enum HUDItems
	{
		FPPADS = 0,
		FPPCrouch = 1,
		FPPFollowMe = 2,
		FPPGrenade = 3,
		FPPSnapTargetLeft = 4,
		FPPTrigger = 5,
		FPPUnitSelect = 6,
		FPPWeaponSelect = 7,
		TPPDropBody = 8,
		TPPDropClaymore = 9,
		TPPDropGrenade = 10,
		TPPUnitSelector = 11,
		Zoom = 12,
		FPPCMButton = 13,
		FPPSnapTargetRight = 14,
		INVALID = 15,
		FPPExitMinigun = 16,
		FPPHoldBreath = 17,
		FPPStealthKill = 18
	}

	public List<HUDItems> ButtonsToHide = new List<HUDItems>();

	public static List<HUDItems> PreviousButtonState = new List<HUDItems>();

	public bool UnhideSelected;

	public bool UnhideAll;

	public bool HideAll;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		DoLockHUDItems(ButtonsToHide, UnhideSelected, UnhideAll, HideAll);
		yield break;
	}

	public static void RestoreHUDItems()
	{
		if (PreviousButtonState != null && PreviousButtonState.Count > 0)
		{
			DoLockHUDItems(PreviousButtonState, true, false, false);
		}
		else
		{
			DoLockHUDItems(null, false, true, false);
		}
		CommonHudController.Instance.PauseButton.gameObject.SetActive(true);
		CommonHudController.Instance.HeartBeat.gameObject.SetActive(true);
	}

	public static List<HUDItems> GetCurrentButtonStates()
	{
		List<HUDItems> list = new List<HUDItems>();
		if (!CommonHudController.Instance.FPPADSLocked)
		{
			list.Add(HUDItems.FPPADS);
		}
		if (!CommonHudController.Instance.FPPCrouchLocked)
		{
			list.Add(HUDItems.FPPCrouch);
		}
		if (!CommonHudController.Instance.FPPFollowMeLocked)
		{
			list.Add(HUDItems.FPPFollowMe);
		}
		if (CommonHudController.Instance.GreneadeSelect.activeInHierarchy)
		{
			list.Add(HUDItems.FPPGrenade);
		}
		if (CommonHudController.Instance.SnapToTargetLeft.gameObject.activeInHierarchy)
		{
			list.Add(HUDItems.FPPSnapTargetLeft);
		}
		if (CommonHudController.Instance.SnapToTargetRight.gameObject.activeInHierarchy)
		{
			list.Add(HUDItems.FPPSnapTargetRight);
		}
		if (!CommonHudController.Instance.TriggerLocked)
		{
			list.Add(HUDItems.FPPTrigger);
		}
		Transform parent = CommonHudController.Instance.UnitName.transform.parent;
		if (parent == null)
		{
			list.Add(HUDItems.FPPUnitSelect);
		}
		parent = CommonHudController.Instance.AmmoProgressBar.transform.parent;
		if (parent == null)
		{
			list.Add(HUDItems.FPPWeaponSelect);
		}
		if (!CommonHudController.Instance.TPPDropBodyLocked)
		{
			list.Add(HUDItems.TPPDropBody);
		}
		if (!CommonHudController.Instance.TPPDropClaymoreLocked)
		{
			list.Add(HUDItems.TPPDropClaymore);
		}
		if (!CommonHudController.Instance.TPPDropGrenadeLocked)
		{
			list.Add(HUDItems.TPPDropGrenade);
		}
		if (!CommonHudController.Instance.TPPUnitSelectorLocked)
		{
			list.Add(HUDItems.TPPUnitSelector);
		}
		if (!TutorialToggles.LockZoom)
		{
			list.Add(HUDItems.Zoom);
		}
		if (!CommonHudController.Instance.ContextInteractionLocked)
		{
			list.Add(HUDItems.FPPCMButton);
		}
		if (!CommonHudController.Instance.ExitMinigunButtonLocked)
		{
			list.Add(HUDItems.FPPExitMinigun);
		}
		if (!CommonHudController.Instance.HoldBreathLocked)
		{
			list.Add(HUDItems.FPPHoldBreath);
		}
		return list;
	}

	public static void DoLockHUDItems(List<HUDItems> ButtonsToHide, bool UnhideSelected, bool UnhideAll, bool HideAll)
	{
		if (HideAll)
		{
			PreviousButtonState = GetCurrentButtonStates();
		}
		if (UnhideAll)
		{
			CommonHudController.Instance.TPPDropGrenadeLocked = false;
			CommonHudController.Instance.TPPDropBodyLocked = false;
			CommonHudController.Instance.TPPDropClaymoreLocked = false;
			CommonHudController.Instance.TPPUnitSelectorLocked = false;
			CommonHudController.Instance.TriggerLocked = false;
			CommonHudController.Instance.FPPStealthKillLocked = false;
			CommonHudController.Instance.FPPADSLocked = false;
			CommonHudController.Instance.FPPCrouchLocked = false;
			CommonHudController.Instance.FPPFollowMeLocked = false;
			CommonHudController.Instance.ExitMinigunButtonLocked = false;
			CommonHudController.Instance.HoldBreathLocked = false;
			TutorialToggles.LockZoom = false;
			CommonHudController.Instance.SetZoomButtons(GameController.Instance.ZoomInAvailable, GameController.Instance.ZoomOutAvailable);
			if (HudStateController.Instance.State == HudStateController.HudState.TPP)
			{
				CommonHudController.Instance.DropGrenadeButton.gameObject.SetActive(true);
				CommonHudController.Instance.DropBodyButton.gameObject.SetActive(true);
				CommonHudController.Instance.DropClaymoreButton.gameObject.SetActive(true);
				CommonHudController.Instance.HideTPPUnitSelecter(false);
			}
			CommonHudController.Instance.HeartBeat.gameObject.SetActive(true);
			CommonHudController.Instance.PauseButton.gameObject.SetActive(true);
			CommonHudController.Instance.TriggerLocked = false;
			CommonHudController.Instance.FPPStealthKillLocked = false;
			CommonHudController.Instance.HideADSButton(false);
			CommonHudController.Instance.HideCrouchButton(false);
			CommonHudController.Instance.HideFollowMe(false);
			CommonHudController.Instance.GreneadeSelect.SetActive(true);
			CommonHudController.Instance.SnapToTargetLeft.gameObject.SetActive(true);
			CommonHudController.Instance.SnapToTargetRight.gameObject.SetActive(true);
			Transform parent = CommonHudController.Instance.AmmoProgressBar.transform.parent;
			if (parent != null)
			{
				parent.gameObject.SetActive(true);
			}
			parent = CommonHudController.Instance.UnitName.transform.parent;
			if (parent != null)
			{
				Transform parent2 = parent.parent;
				if (parent2 != null)
				{
					parent2.gameObject.SetActive(true);
					if (GKM.UnitCount(GKM.PlayerControlledMask & GKM.UpAndAboutMask) > 1 && GameController.Instance.IsFirstPerson)
					{
						CommonHudController.Instance.HideFPPUntChangeButtons(false);
					}
				}
			}
			CommonHudController.Instance.ContextInteractionLocked = false;
			return;
		}
		if (UnhideSelected)
		{
			using (List<HUDItems>.Enumerator enumerator = ButtonsToHide.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case HUDItems.FPPADS:
					{
						bool fPPCrouchLocked = CommonHudController.Instance.FPPCrouchLocked;
						if (fPPCrouchLocked)
						{
							CommonHudController.Instance.FPPCrouchLocked = false;
							CommonHudController.Instance.HideCrouchButton(false);
						}
						CommonHudController.Instance.FPPADSLocked = false;
						CommonHudController.Instance.HideADSButton(false);
						if (fPPCrouchLocked)
						{
							CommonHudController.Instance.HideCrouchButton(true);
							CommonHudController.Instance.FPPCrouchLocked = true;
						}
						break;
					}
					case HUDItems.FPPCrouch:
						CommonHudController.Instance.FPPCrouchLocked = false;
						CommonHudController.Instance.HideCrouchButton(false);
						break;
					case HUDItems.FPPFollowMe:
						CommonHudController.Instance.FPPFollowMeLocked = false;
						CommonHudController.Instance.HideFollowMe(false);
						break;
					case HUDItems.FPPGrenade:
						CommonHudController.Instance.GreneadeSelect.SetActive(true);
						break;
					case HUDItems.FPPSnapTargetLeft:
						CommonHudController.Instance.SnapToTargetLeft.gameObject.SetActive(true);
						break;
					case HUDItems.FPPSnapTargetRight:
						CommonHudController.Instance.SnapToTargetRight.gameObject.SetActive(true);
						break;
					case HUDItems.FPPTrigger:
						CommonHudController.Instance.TriggerLocked = false;
						break;
					case HUDItems.FPPStealthKill:
						CommonHudController.Instance.FPPStealthKillLocked = false;
						break;
					case HUDItems.FPPUnitSelect:
					{
						Transform parent4 = CommonHudController.Instance.UnitName.transform.parent;
						if (parent4 != null)
						{
							Transform parent5 = parent4.parent;
							if (parent5 != null)
							{
								parent5.gameObject.SetActive(true);
								if (GKM.UnitCount(GKM.PlayerControlledMask & GKM.UpAndAboutMask) > 1 && GameController.Instance.IsFirstPerson)
								{
									CommonHudController.Instance.HideFPPUntChangeButtons(false);
								}
							}
						}
						break;
					}
					case HUDItems.FPPWeaponSelect:
					{
						Transform parent3 = CommonHudController.Instance.AmmoProgressBar.transform.parent;
						if (parent3 != null)
						{
							parent3.gameObject.SetActive(true);
						}
						break;
					}
					case HUDItems.TPPDropBody:
						CommonHudController.Instance.TPPDropBodyLocked = false;
						CommonHudController.Instance.DropBodyButton.gameObject.SetActive(true);
						break;
					case HUDItems.TPPDropClaymore:
						CommonHudController.Instance.TPPDropClaymoreLocked = false;
						CommonHudController.Instance.DropClaymoreButton.gameObject.SetActive(true);
						break;
					case HUDItems.TPPDropGrenade:
						CommonHudController.Instance.TPPDropGrenadeLocked = false;
						CommonHudController.Instance.DropGrenadeButton.gameObject.SetActive(true);
						break;
					case HUDItems.TPPUnitSelector:
						CommonHudController.Instance.TPPUnitSelectorLocked = false;
						CommonHudController.Instance.HideTPPUnitSelecter(false);
						break;
					case HUDItems.Zoom:
						TutorialToggles.LockZoom = false;
						CommonHudController.Instance.SetZoomButtons(GameController.Instance.ZoomInAvailable, GameController.Instance.ZoomOutAvailable);
						break;
					case HUDItems.FPPCMButton:
						CommonHudController.Instance.ContextInteractionLocked = false;
						break;
					case HUDItems.FPPExitMinigun:
						CommonHudController.Instance.ExitMinigunButtonLocked = false;
						break;
					case HUDItems.FPPHoldBreath:
						CommonHudController.Instance.HoldBreathLocked = false;
						break;
					}
				}
				return;
			}
		}
		if (HideAll)
		{
			CommonHudController.Instance.HideADSButton(true);
			CommonHudController.Instance.HideCrouchButton(true);
			CommonHudController.Instance.HideFollowMe(true);
			CommonHudController.Instance.HeartBeat.gameObject.SetActive(false);
			CommonHudController.Instance.PauseButton.gameObject.SetActive(false);
			if (!GameController.Instance.IsFirstPerson)
			{
				CommonHudController.Instance.DropGrenadeButton.gameObject.SetActive(false);
				CommonHudController.Instance.DropBodyButton.gameObject.SetActive(false);
				CommonHudController.Instance.DropClaymoreButton.gameObject.SetActive(false);
				CommonHudController.Instance.HideTPPUnitSelecter(true);
			}
			TutorialToggles.LockZoom = true;
			CommonHudController.Instance.SetZoomButtons(false, false);
			CommonHudController.Instance.HideADSButton(true);
			CommonHudController.Instance.HideCrouchButton(true);
			CommonHudController.Instance.HideFollowMe(true);
			CommonHudController.Instance.GreneadeSelect.SetActive(false);
			CommonHudController.Instance.SnapToTargetLeft.gameObject.SetActive(false);
			CommonHudController.Instance.SnapToTargetRight.gameObject.SetActive(false);
			Transform parent6 = CommonHudController.Instance.AmmoProgressBar.transform.parent;
			if (parent6 != null)
			{
				parent6.gameObject.SetActive(false);
			}
			parent6 = CommonHudController.Instance.UnitName.transform.parent;
			if (parent6 != null)
			{
				Transform parent7 = parent6.parent;
				if (parent7 != null)
				{
					parent7.gameObject.SetActive(false);
				}
			}
			CommonHudController.Instance.TriggerLocked = true;
			CommonHudController.Instance.TPPDropGrenadeLocked = true;
			CommonHudController.Instance.TPPDropBodyLocked = true;
			CommonHudController.Instance.TPPDropClaymoreLocked = true;
			CommonHudController.Instance.TPPUnitSelectorLocked = true;
			CommonHudController.Instance.TriggerLocked = true;
			CommonHudController.Instance.FPPStealthKillLocked = true;
			CommonHudController.Instance.FPPADSLocked = true;
			CommonHudController.Instance.FPPCrouchLocked = true;
			CommonHudController.Instance.FPPFollowMeLocked = true;
			CommonHudController.Instance.HideContextInteractionButton(true);
			CommonHudController.Instance.ContextInteractionLocked = true;
			CommonHudController.Instance.ExitMinigunButtonLocked = true;
			CommonHudController.Instance.HoldBreathLocked = true;
			return;
		}
		using (List<HUDItems>.Enumerator enumerator2 = ButtonsToHide.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				switch (enumerator2.Current)
				{
				case HUDItems.FPPADS:
					CommonHudController.Instance.HideADSButton(true);
					CommonHudController.Instance.FPPADSLocked = true;
					break;
				case HUDItems.FPPCrouch:
					CommonHudController.Instance.HideCrouchButton(true);
					CommonHudController.Instance.FPPCrouchLocked = true;
					break;
				case HUDItems.FPPFollowMe:
					CommonHudController.Instance.HideFollowMe(true);
					CommonHudController.Instance.FPPFollowMeLocked = true;
					break;
				case HUDItems.FPPGrenade:
					CommonHudController.Instance.GreneadeSelect.SetActive(false);
					break;
				case HUDItems.FPPSnapTargetLeft:
					CommonHudController.Instance.SnapToTargetLeft.gameObject.SetActive(false);
					break;
				case HUDItems.FPPSnapTargetRight:
					CommonHudController.Instance.SnapToTargetRight.gameObject.SetActive(false);
					break;
				case HUDItems.FPPTrigger:
					CommonHudController.Instance.TriggerLocked = true;
					break;
				case HUDItems.FPPStealthKill:
					CommonHudController.Instance.FPPStealthKillLocked = true;
					break;
				case HUDItems.FPPUnitSelect:
				{
					Transform parent9 = CommonHudController.Instance.UnitName.transform.parent;
					if (parent9 != null)
					{
						Transform parent10 = parent9.parent;
						if (parent10 != null)
						{
							parent10.gameObject.SetActive(false);
						}
					}
					break;
				}
				case HUDItems.FPPWeaponSelect:
				{
					Transform parent8 = CommonHudController.Instance.AmmoProgressBar.transform.parent;
					if (parent8 != null)
					{
						parent8.gameObject.SetActive(false);
					}
					break;
				}
				case HUDItems.TPPDropBody:
					CommonHudController.Instance.TPPDropBodyLocked = true;
					CommonHudController.Instance.DropBodyButton.gameObject.SetActive(false);
					break;
				case HUDItems.TPPDropClaymore:
					CommonHudController.Instance.TPPDropClaymoreLocked = true;
					CommonHudController.Instance.DropClaymoreButton.gameObject.SetActive(false);
					break;
				case HUDItems.TPPDropGrenade:
					CommonHudController.Instance.TPPDropGrenadeLocked = true;
					CommonHudController.Instance.DropGrenadeButton.gameObject.SetActive(false);
					break;
				case HUDItems.TPPUnitSelector:
					CommonHudController.Instance.HideTPPUnitSelecter(true);
					CommonHudController.Instance.TPPUnitSelectorLocked = true;
					break;
				case HUDItems.Zoom:
					TutorialToggles.LockZoom = true;
					CommonHudController.Instance.SetZoomButtons(false, false);
					break;
				case HUDItems.FPPCMButton:
					CommonHudController.Instance.HideContextInteractionButton(true);
					CommonHudController.Instance.ContextInteractionLocked = true;
					break;
				case HUDItems.FPPExitMinigun:
					CommonHudController.Instance.ExitMinigunButtonLocked = true;
					break;
				case HUDItems.FPPHoldBreath:
					CommonHudController.Instance.HoldBreathLocked = true;
					break;
				}
			}
		}
	}
}
