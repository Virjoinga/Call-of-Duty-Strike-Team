using System.Collections;
using UnityEngine;

public class HighlightHudCommand : Command
{
	public enum HUDItems
	{
		FPPADS = 0,
		FPPCrouch = 1,
		FPPFollowMe = 2,
		FPPGrenade = 3,
		FPPSnapTargetLeft = 4,
		FPPSnapTargetRight = 5,
		FPPTrigger = 6,
		FPPUnitSelect = 7,
		FPPWeaponSelect = 8,
		FPPHealth = 9,
		TPPDropBody = 10,
		TPPDropClaymore = 11,
		TPPDropGrenade = 12,
		TPPUnitSelector = 13,
		Pause = 14,
		Zoom = 15,
		FPPStealthTrigger = 16,
		FPPContextMenu = 17,
		FPPADSTrigger = 18,
		FPPMove = 19,
		FPPLook = 20,
		TPPCameraMove = 21,
		TPPCameraRotate = 22,
		TPPCameraZoom = 23,
		WorldGameObject = 24,
		RemoveAllHighlights = 25,
		TPPContextMenu = 26,
		TPPContextMenuButton = 27,
		FPPExitMinigunButton = 28
	}

	public enum ContextType
	{
		Door = 0,
		Window = 1
	}

	public enum HighlightTypes
	{
		Small = 0,
		Medium = 1,
		Arrows = 2,
		Pinch = 3,
		Rotate = 4,
		Swipe = 5
	}

	public HUDItems ItemToHighlight;

	public float TimeToHighlight;

	public HighlightTypes HighlightType = HighlightTypes.Medium;

	public GuidRef ContextMenuObject;

	public ContextType InteriorContextType;

	public int InteriorIndexToReference;

	public ContextMenuIcons CMButtonToHighlight;

	public int TPPUnitNumber = 1;

	public GuidRef GameObjectToHighlight;

	private GameObject mIcon;

	private bool mActive;

	public override bool Blocking()
	{
		return false;
	}

	public override void ResolveGuidLinks()
	{
		if (ContextMenuObject != null)
		{
			ContextMenuObject.ResolveLink();
		}
		if (GameObjectToHighlight != null)
		{
			GameObjectToHighlight.ResolveLink();
		}
	}

	public override IEnumerator Execute()
	{
		StartCoroutine(CleanUp());
		switch (ItemToHighlight)
		{
		case HUDItems.FPPADS:
			yield return StartCoroutine(HighlightFPPADS());
			break;
		case HUDItems.FPPCrouch:
			yield return StartCoroutine(HighlightFPPCrouch());
			break;
		case HUDItems.FPPFollowMe:
			yield return StartCoroutine(HighlightFPPFollowMe());
			break;
		case HUDItems.FPPGrenade:
			yield return StartCoroutine(HighlightFPPGrenade());
			break;
		case HUDItems.FPPSnapTargetLeft:
			yield return StartCoroutine(HighlightFPPSnapTargetLeft());
			break;
		case HUDItems.FPPSnapTargetRight:
			yield return StartCoroutine(HighlightFPPSnapTargetRight());
			break;
		case HUDItems.FPPTrigger:
			yield return StartCoroutine(HighlightFPPTrigger());
			break;
		case HUDItems.FPPUnitSelect:
			yield return StartCoroutine(HighlightFPPUnitSelect());
			break;
		case HUDItems.FPPWeaponSelect:
			yield return StartCoroutine(HighlightFPPWeaponSelect());
			break;
		case HUDItems.FPPHealth:
			yield return StartCoroutine(HighlightFPPHealth());
			break;
		case HUDItems.TPPDropBody:
			yield return StartCoroutine(HighlightTPPDropBody());
			break;
		case HUDItems.TPPDropClaymore:
			yield return StartCoroutine(HighlightTPPDropClaymore());
			break;
		case HUDItems.TPPDropGrenade:
			yield return StartCoroutine(HighlightTPPDropGrenade());
			break;
		case HUDItems.TPPUnitSelector:
			yield return StartCoroutine(HighlightTPPUnitSelector());
			break;
		case HUDItems.Pause:
			yield return StartCoroutine(HighlightPause());
			break;
		case HUDItems.Zoom:
			yield return StartCoroutine(HighlightZoom());
			break;
		case HUDItems.FPPStealthTrigger:
			yield return StartCoroutine(HighlightFPPStealthTrigger());
			break;
		case HUDItems.FPPContextMenu:
			yield return StartCoroutine(HighlightFPPContextMenu());
			break;
		case HUDItems.FPPADSTrigger:
			yield return StartCoroutine(HighlightFPPADSTrigger());
			break;
		case HUDItems.FPPMove:
			yield return StartCoroutine(HighlightFPPMove());
			break;
		case HUDItems.FPPLook:
			yield return StartCoroutine(HighlightFPPLook());
			break;
		case HUDItems.TPPCameraMove:
			yield return StartCoroutine(HighlightTPPCameraMove());
			break;
		case HUDItems.TPPCameraRotate:
			yield return StartCoroutine(HighlightTPPCameraRotate());
			break;
		case HUDItems.TPPCameraZoom:
			yield return StartCoroutine(HighlightTPPCameraZoom());
			break;
		case HUDItems.WorldGameObject:
			yield return StartCoroutine(HighlightWorldGameObject());
			break;
		case HUDItems.RemoveAllHighlights:
			ClearAllHighlights();
			break;
		case HUDItems.TPPContextMenu:
			yield return StartCoroutine(HighlightTPPContextMenu());
			break;
		case HUDItems.TPPContextMenuButton:
			yield return StartCoroutine(HighlightCMButton());
			break;
		case HUDItems.FPPExitMinigunButton:
			yield return StartCoroutine(HighlightExitMinigunButton());
			break;
		}
		StopAllCoroutines();
	}

	private IEnumerator HighlightFPPADS()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.ADSButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon);
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if ((weaponADS != null && weaponADS.GetADSState() == ADSState.ADS) || !GameController.Instance.IsFirstPerson)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPCrouch()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.CrouchButton.transform, Vector3.zero);
		BaseCharacter.Stance initialStance = GameController.Instance.mFirstPersonActor.realCharacter.GetStance();
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (!GameController.Instance.IsFirstPerson || GameController.Instance.mFirstPersonActor.realCharacter.GetStance() != initialStance)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPFollowMe()
	{
		if (!GameController.Instance.IsFirstPerson || !CommonHudController.Instance.FollowMeButton.gameObject.activeInHierarchy)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.FollowMeButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (!GameController.Instance.IsFirstPerson || CommonHudController.Instance.FollowMeRecentlyPressed())
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPGrenade()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.GreneadeSelect.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		Weapon_Grenade grenadeWeapon2 = null;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (GameController.Instance.IsFirstPerson)
			{
				grenadeWeapon2 = (GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon as Weapon_Grenade) ?? (GameController.Instance.mFirstPersonActor.weapon.DesiredWeapon as Weapon_Grenade);
				if (grenadeWeapon2 != null)
				{
					quitLoop = true;
				}
			}
			else
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPSnapTargetLeft()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		Transform leftTarget = CommonHudController.Instance.SnapToTargetLeft.transform;
		while (!leftTarget.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		SetupIcon(leftTarget, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			mIcon.transform.localScale = Vector3.one;
			if (!GameController.Instance.IsFirstPerson || CommonHudController.Instance.HasSnappedToTargetLeft)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPSnapTargetRight()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		Transform rightTarget = CommonHudController.Instance.SnapToTargetRight.transform;
		while (!rightTarget.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		SetupIcon(rightTarget, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			mIcon.transform.localScale = Vector3.one;
			if (!GameController.Instance.IsFirstPerson || CommonHudController.Instance.HasSnappedToTargetRight)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPTrigger()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.TriggerButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		Vector3 initialPosition = CommonHudController.Instance.TriggerButton.transform.position;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (CommonHudController.Instance.TriggerButton.transform.position != initialPosition)
			{
				mIcon.transform.position = CommonHudController.Instance.TriggerButton.transform.position;
			}
			if (!GameController.Instance.IsFirstPerson || CommonHudController.Instance.TriggerPressed)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPUnitSelect()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.UnitName.transform, Vector3.zero);
		Actor initialFPPActor = GameController.Instance.mFirstPersonActor;
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (!GameController.Instance.IsFirstPerson || initialFPPActor != GameController.Instance.mFirstPersonActor)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPWeaponSelect()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.WeaponIcon.transform, Vector3.zero);
		IWeapon initialWeapon = GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon;
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (!GameController.Instance.IsFirstPerson || initialWeapon != GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPHealth()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.HeartBeat.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (!GameController.Instance.IsFirstPerson)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightTPPDropBody()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.DropBodyButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (GameController.Instance.IsFirstPerson || GameplayController.instance.PlacingBody)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightTPPDropClaymore()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.DropClaymoreButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (GameController.Instance.IsFirstPerson || GameplayController.instance.PlacingClaymore)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightTPPDropGrenade()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		SetupIcon(CommonHudController.Instance.DropGrenadeButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (GameController.Instance.IsFirstPerson || GameController.Instance.GrenadeThrowingModeActive)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightTPPUnitSelector()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		if (TPPUnitNumber < 1)
		{
			TPPUnitNumber = 1;
		}
		else if (TPPUnitNumber > 4)
		{
			TPPUnitNumber = 4;
		}
		if (!CommonHudController.Instance.TPPUnitSelecter.Units[TPPUnitNumber - 1].GetComponent<Renderer>().enabled)
		{
			yield break;
		}
		Actor actorToHighlight = CommonHudController.Instance.TPPUnitSelecter.Units[TPPUnitNumber - 1].MyActor;
		SetupIcon(CommonHudController.Instance.TPPUnitSelecter.Units[TPPUnitNumber - 1].transform, Vector3.zero);
		GameObject secondIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterSmall);
		secondIcon.transform.position = actorToHighlight.transform.position;
		StartCoroutine("Blink", secondIcon);
		TutorialToggles.ActiveHighlights.Add(secondIcon);
		HudBlipIcon hbi = secondIcon.GetComponent<HudBlipIcon>();
		hbi.Target = actorToHighlight.transform;
		hbi.WorldOffset = Vector3.up;
		float delayTime = 0f;
		bool quitLoop = false;
		HudBlipIcon mIconBlip = mIcon.GetComponent<HudBlipIcon>();
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			mIconBlip.Target = CommonHudController.Instance.TPPUnitSelecter.Units[TPPUnitNumber - 1].transform;
			mIcon.transform.position = CommonHudController.Instance.TPPUnitSelecter.Units[TPPUnitNumber - 1].transform.position;
			mIcon.transform.localScale = Vector3.one;
			if (GameController.Instance.IsFirstPerson)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
		DestroyIcon(secondIcon);
	}

	private IEnumerator HighlightPause()
	{
		SetupIcon(CommonHudController.Instance.PauseButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (!CommonHudController.Instance.PauseButton.gameObject.activeInHierarchy)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightZoom()
	{
		SetupIcon(CommonHudController.Instance.ZoomInButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		bool isFPP = GameController.Instance.IsFirstPerson;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			mIcon.transform.localScale = Vector3.one;
			if (isFPP != GameController.Instance.IsFirstPerson || !CommonHudController.Instance.ZoomInButton.gameObject.activeInHierarchy)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPStealthTrigger()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		while (!CommonHudController.Instance.TriggerButton.gameObject.activeInHierarchy || CommonHudController.Instance.GetTriggerFrame() != 1)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		SetupIcon(CommonHudController.Instance.TriggerButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		Vector3 initialPosition = CommonHudController.Instance.TriggerButton.transform.position;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (CommonHudController.Instance.TriggerButton.transform.position != initialPosition)
			{
				mIcon.transform.position = CommonHudController.Instance.TriggerButton.transform.position;
			}
			if (CommonHudController.Instance.GetTriggerFrame() == 1)
			{
				if (!mIcon.activeInHierarchy && !mActive)
				{
					yield return new WaitForSeconds(0.5f);
					mIcon.SetActive(true);
					mActive = true;
				}
			}
			else if (mIcon.activeInHierarchy && mActive)
			{
				mIcon.SetActive(false);
				mActive = false;
			}
			if (!GameController.Instance.IsFirstPerson || CommonHudController.Instance.StealthKillPressed)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPContextMenu()
	{
		if (ContextMenuObject == null || !GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		InterfaceableObject io = ContextMenuObject.theObject.GetComponentInChildren<InterfaceableObject>();
		if (!(io != null))
		{
			yield break;
		}
		while (!io.IsActionIconVisible() || !CommonHudController.Instance.ContextInteractionButton.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		SetupIcon(io.ContextBlip.Target, io.GetBlipPos());
		HudBlipIcon hbi2 = mIcon.GetComponent<HudBlipIcon>();
		hbi2.WorldOffset = io.ContextBlip.WorldOffset;
		hbi2.IsAllowedInFirstPerson = true;
		GameObject secondIcon = null;
		if (HighlightType == HighlightTypes.Medium)
		{
			secondIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighter);
		}
		else if (HighlightType == HighlightTypes.Small)
		{
			secondIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterSmall);
		}
		secondIcon.transform.position = CommonHudController.Instance.ContextInteractionButton.transform.position;
		TutorialToggles.ActiveHighlights.Add(secondIcon);
		hbi2 = secondIcon.GetComponent<HudBlipIcon>();
		hbi2.Target = CommonHudController.Instance.ContextInteractionButton.transform;
		float delayTime = 0f;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (io == null || io.ContextBlip == null)
			{
				break;
			}
			if (io.ContextBlip.IsOnScreen && CommonHudController.Instance.ContextInteractionButton.gameObject.activeInHierarchy)
			{
				if (io.IsActionIconVisible())
				{
					if (!mIcon.activeInHierarchy && !mActive)
					{
						mIcon.SetActive(true);
						mActive = true;
					}
					else if (mIcon.activeInHierarchy && !mActive)
					{
						mActive = true;
					}
					mIcon.transform.position = io.GetBlipPos();
				}
				else if (mIcon.activeInHierarchy)
				{
					mIcon.SetActive(false);
					mActive = false;
				}
			}
			else
			{
				mActive = false;
				mIcon.SetActive(false);
			}
			secondIcon.transform.localScale = mIcon.transform.localScale;
			secondIcon.SetActive(mIcon.activeInHierarchy);
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightTPPContextMenu()
	{
		if (ContextMenuObject == null || GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		InterfaceableObject io = null;
		InteriorOverride interior = ContextMenuObject.theObject.GetComponentInChildren<InteriorOverride>();
		if ((bool)interior)
		{
			if (InteriorContextType == ContextType.Door)
			{
				if (interior.m_ActiveDoors.Count > InteriorIndexToReference)
				{
					io = interior.m_ActiveDoors[InteriorIndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>();
				}
			}
			else if (interior.m_ActiveWindows.Count > InteriorIndexToReference)
			{
				io = interior.m_ActiveWindows[InteriorIndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>();
			}
		}
		else
		{
			io = ContextMenuObject.theObject.GetComponentInChildren<InterfaceableObject>();
		}
		if (io != null && io.ContextBlip != null)
		{
			while (!io.ContextBlip.IsOnScreen || !io.ContextBlip.IsSwitchedOn)
			{
				yield return null;
			}
			SetupIcon(io.ContextBlip.Target, io.GetBlipPos());
			HudBlipIcon hbi = mIcon.GetComponent<HudBlipIcon>();
			hbi.WorldOffset = io.ContextBlip.WorldOffset;
			float delayTime = 0f;
			while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && mIcon != null)
			{
				delayTime += Time.deltaTime;
				yield return null;
			}
			DestroyIcon(mIcon);
		}
	}

	private IEnumerator HighlightFPPADSTrigger()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(GameController.Instance.mFirstPersonActor.weapon.ActiveWeapon);
		if (weaponADS == null)
		{
			yield break;
		}
		while (weaponADS.GetADSState() != 0)
		{
			yield return null;
		}
		SetupIcon(CommonHudController.Instance.TriggerButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (weaponADS.GetADSState() == ADSState.ADS)
			{
				if (!mIcon.activeInHierarchy)
				{
					yield return new WaitForSeconds(0.5f);
					mIcon.SetActive(true);
					mActive = true;
				}
			}
			else if (mIcon.activeInHierarchy)
			{
				mIcon.SetActive(false);
				mActive = false;
			}
			if (!GameController.Instance.IsFirstPerson || (CommonHudController.Instance.TriggerPressed && weaponADS.GetADSState() == ADSState.ADS))
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPMove()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		Vector3 pos = Vector3.zero;
		pos.y += (float)Screen.height * 0.42f;
		pos.x += (float)Screen.width * 0.15f;
		SetupIcon(altPos: GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(pos), target: CommonHudController.Instance.Move.transform);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (!GameController.Instance.IsFirstPerson || CommonHudController.Instance.MoveAmount != Vector2.zero)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightFPPLook()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		Vector3 pos = Vector3.zero;
		pos.y += (float)Screen.height * 0.42f;
		pos.x += (float)Screen.width * 0.85f;
		pos = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(pos);
		Transform target = CommonHudController.Instance.Move.transform;
		while (!target.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		SetupIcon(target, pos);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			mIcon.transform.localScale = Vector3.one;
			if (!GameController.Instance.IsFirstPerson || CommonHudController.Instance.LookAmountTouch != Vector2.zero || CommonHudController.Instance.LookAmountGamepad != Vector2.zero)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightTPPCameraMove()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		CameraManager cameras = CameraManager.Instance;
		NavMeshCamera nmc = null;
		if (cameras != null)
		{
			nmc = cameras.PlayCameraController.CurrentCameraBase as NavMeshCamera;
		}
		if (nmc == null)
		{
			yield break;
		}
		Vector3 pos = Vector3.zero;
		pos.y += (float)Screen.height * 0.45f;
		pos.x += (float)Screen.width * 0.5f;
		SetupIcon(altPos: GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(pos), target: CommonHudController.Instance.Move.transform);
		float delayTime = 0f;
		bool quitLoop = false;
		Vector3 panVelocity = nmc.PanVelocity;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (GameController.Instance.IsFirstPerson || panVelocity != nmc.PanVelocity)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightTPPCameraRotate()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		CameraManager cameras = CameraManager.Instance;
		NavMeshCamera nmc = null;
		if (cameras != null)
		{
			nmc = cameras.PlayCameraController.CurrentCameraBase as NavMeshCamera;
		}
		if (nmc == null)
		{
			yield break;
		}
		Vector3 pos = Vector3.zero;
		pos.y += (float)Screen.height * 0.425f;
		pos.x += (float)Screen.width * 0.5f;
		SetupIcon(altPos: GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(pos), target: CommonHudController.Instance.Move.transform);
		float delayTime = 0f;
		bool quitLoop = false;
		float orbitVelocity = nmc.IdealYaw;
		float rotAmount = 0f;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			rotAmount += Mathf.Abs(orbitVelocity - nmc.IdealYaw);
			orbitVelocity = nmc.IdealYaw;
			if (GameController.Instance.IsFirstPerson || rotAmount > 30f)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightTPPCameraZoom()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		CameraManager cameras = CameraManager.Instance;
		NavMeshCamera nmc = null;
		if (cameras != null)
		{
			nmc = cameras.PlayCameraController.CurrentCameraBase as NavMeshCamera;
		}
		if (nmc == null)
		{
			yield break;
		}
		Vector3 pos = Vector3.zero;
		pos.y += (float)Screen.height * 0.425f;
		pos.x += (float)Screen.width * 0.5f;
		SetupIcon(altPos: GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(pos), target: CommonHudController.Instance.Move.transform);
		GameObject secondIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterPinchOut);
		pos = Vector3.zero;
		pos.y += (float)Screen.height * 0.425f;
		pos.x += (float)Screen.width * 0.5f;
		pos = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(pos);
		secondIcon.transform.position = pos;
		secondIcon.SetActive(false);
		TutorialToggles.ActiveHighlights.Add(secondIcon);
		HudBlipIcon hbi = secondIcon.GetComponent<HudBlipIcon>();
		hbi.Target = CommonHudController.Instance.Move.transform;
		float delayTime = 0f;
		bool quitLoop = false;
		bool usingFirstIcon = true;
		float fov = nmc.Fov;
		float zoomAmount = 0f;
		float iconSwapTimer = 0f;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			zoomAmount += Mathf.Abs(fov - nmc.Fov);
			fov = nmc.Fov;
			if (GameController.Instance.IsFirstPerson || zoomAmount > 5f)
			{
				quitLoop = true;
			}
			iconSwapTimer += Time.deltaTime;
			if (iconSwapTimer > 2.4f)
			{
				StopCoroutine("Blink");
				mIcon.SetActive(!usingFirstIcon);
				secondIcon.SetActive(usingFirstIcon);
				StartCoroutine("Blink", (!usingFirstIcon) ? mIcon : secondIcon);
				usingFirstIcon = !usingFirstIcon;
				iconSwapTimer = 0f;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
		DestroyIcon(secondIcon);
	}

	private IEnumerator HighlightWorldGameObject()
	{
		Transform target = GameObjectToHighlight.theObject.transform;
		Vector3 worldOffset = Vector3.zero;
		ActorWrapper aw = GameObjectToHighlight.theObject.GetComponentInChildren<ActorWrapper>();
		if (aw != null)
		{
			Actor a = aw.GetActor();
			if (a != null)
			{
				target = a.transform;
				worldOffset = Vector3.up;
			}
		}
		SetupIcon(target, Vector3.zero);
		HudBlipIcon hbi = mIcon.GetComponent<HudBlipIcon>();
		hbi.WorldOffset = worldOffset;
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightCMButton()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		while (CommonHudController.Instance.ContextMenu == null || CommonHudController.Instance.ContextMenu.GetButton(CMButtonToHighlight) == null)
		{
			if (GameController.Instance.IsFirstPerson)
			{
				yield break;
			}
			yield return null;
		}
		MenuButton button = CommonHudController.Instance.ContextMenu.GetButton(CMButtonToHighlight);
		SetupIcon(button.Button.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop)
		{
			if (CommonHudController.Instance.ContextMenu == null || CommonHudController.Instance.ContextMenu.GetButton(CMButtonToHighlight) == null)
			{
				quitLoop = true;
			}
			if (mIcon != null && button != null && button.Button != null)
			{
				mIcon.transform.position = button.Button.transform.position;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private IEnumerator HighlightExitMinigunButton()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			yield break;
		}
		TaskUseFixedGun task2 = GameController.Instance.mFirstPersonActor.tasks.GetRunningTask<TaskUseFixedGun>();
		while (task2 == null || GameController.Instance.IsLockedToFirstPerson || !CommonHudController.Instance.ContextInteractionIcon.gameObject.activeInHierarchy || CommonHudController.Instance.ContextInteractionIcon.GetCurAnim().GetCurPosition() != 1)
		{
			if (!GameController.Instance.IsFirstPerson)
			{
				yield break;
			}
			yield return null;
		}
		SetupIcon(CommonHudController.Instance.ContextInteractionIcon.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop)
		{
			task2 = GameController.Instance.mFirstPersonActor.tasks.GetRunningTask<TaskUseFixedGun>();
			if (task2 == null || !GameController.Instance.IsFirstPerson || CommonHudController.Instance.ContextInteractionButton.controlState == UIButton.CONTROL_STATE.ACTIVE || !CommonHudController.Instance.ContextInteractionIcon.gameObject.activeInHierarchy || CommonHudController.Instance.ContextInteractionIcon.GetCurAnim().GetCurPosition() != 1)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
	}

	private void ClearAllHighlights()
	{
		foreach (GameObject activeHighlight in TutorialToggles.ActiveHighlights)
		{
			Object.Destroy(activeHighlight);
		}
		TutorialToggles.ActiveHighlights.RemoveAll((GameObject item) => item == null);
	}

	private void SetupIcon(Transform target, Vector3 altPos)
	{
		switch (HighlightType)
		{
		case HighlightTypes.Medium:
			mIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighter);
			break;
		case HighlightTypes.Small:
			mIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterSmall);
			break;
		case HighlightTypes.Pinch:
			mIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterPinchIn);
			break;
		case HighlightTypes.Arrows:
			mIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterArrows);
			break;
		case HighlightTypes.Rotate:
			mIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterRotate);
			break;
		case HighlightTypes.Swipe:
			mIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterSwipe);
			break;
		}
		Vector3 zero = Vector3.zero;
		zero = ((!(altPos != Vector3.zero)) ? target.transform.position : altPos);
		zero.z = 0f;
		HudBlipIcon component = mIcon.GetComponent<HudBlipIcon>();
		component.Target = target;
		mIcon.transform.position = zero;
		mActive = true;
		TutorialToggles.ActiveHighlights.Add(mIcon);
		StartCoroutine("Blink", mIcon);
	}

	private void DestroyIcon(GameObject icon)
	{
		if (TutorialToggles.ActiveHighlights.Contains(icon))
		{
			TutorialToggles.ActiveHighlights.Remove(icon);
		}
		Object.Destroy(icon);
	}

	private IEnumerator Blink(GameObject icon)
	{
		float blinkTime = 0.2f;
		float currentTime = 0f;
		while (icon != null)
		{
			if (!mActive)
			{
				currentTime = 0f;
				yield return null;
			}
			if (icon == null)
			{
				break;
			}
			currentTime += Time.deltaTime;
			if (currentTime >= blinkTime)
			{
				icon.SetActive(!icon.activeInHierarchy);
				currentTime = 0f;
			}
			yield return null;
		}
	}

	private IEnumerator CleanUp()
	{
		while (!TutorialToggles.ShouldClearHighlights)
		{
			yield return null;
		}
		StopAllCoroutines();
		ClearAllHighlights();
		TutorialToggles.ShouldClearHighlights = false;
	}
}
