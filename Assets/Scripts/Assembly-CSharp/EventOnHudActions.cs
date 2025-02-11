using System;
using System.Collections;
using UnityEngine;

public class EventOnHudActions : EventDescriptor
{
	public enum HudEvents
	{
		Fire = 0,
		CharacterSwitch = 1,
		AiTargetSwitch = 2,
		Reload = 3,
		GoIntoOverWatch = 4,
		GoIntoSniper = 5,
		Crouch = 6,
		DraggedRoute = 7,
		ThrowGrenade = 8,
		SwitchWeapon = 9
	}

	public HudEvents HudEvent;

	public void Awake()
	{
		StartCoroutine(WaitForInit());
	}

	public IEnumerator WaitForInit()
	{
		while (CommonHudController.Instance == null)
		{
			yield return new WaitForSeconds(1f);
		}
		InitialiseHudActionEvents();
	}

	public void InitialiseHudActionEvents()
	{
		switch (HudEvent)
		{
		case HudEvents.Fire:
			EventHub.Instance.OnWeaponFired += OnWeaponFired;
			break;
		case HudEvents.GoIntoOverWatch:
			CommonHudController.Instance.OnZoomOutTriggered += OnOverWatch;
			break;
		case HudEvents.GoIntoSniper:
			CommonHudController.Instance.OnZoomInTriggered += OnSniper;
			break;
		case HudEvents.Crouch:
			CommonHudController.Instance.OnDuckClick += CrouchTest;
			break;
		case HudEvents.Reload:
			CommonHudController.Instance.OnReloadClick += OnFirstPersonReloadClick;
			break;
		case HudEvents.AiTargetSwitch:
			CommonHudController.Instance.OnSnapEnemyLeftClick += OnAIFirstPersonSwitch;
			CommonHudController.Instance.OnSnapEnemyRightClick += OnAIFirstPersonSwitch;
			break;
		case HudEvents.CharacterSwitch:
			CommonHudController.Instance.OnSnapUnitLeftClick += OnUnitFirstPersonSwitch;
			CommonHudController.Instance.OnSnapUnitRightClick += OnUnitFirstPersonSwitch;
			break;
		case HudEvents.ThrowGrenade:
			CommonHudController.Instance.OnThrowGrenadeClick += OnGrenadeFired;
			break;
		case HudEvents.SwitchWeapon:
			CommonHudController.Instance.OnSwitchWeapon += OnWeaponSwitched;
			break;
		case HudEvents.DraggedRoute:
			GameplayController.Instance().OnFingerDragEndClick += OnDraggedRoute;
			break;
		}
	}

	public void CrouchTest(object sender, EventArgs args)
	{
		if (GameController.Instance.mFirstPersonActor.realCharacter.GetStance() == BaseCharacter.Stance.Standing)
		{
			FireEvent();
		}
	}

	public void OnWeaponFired(object sender, Events.WeaponFired args)
	{
		FireEvent();
	}

	public void OnDraggedRoute(object sender)
	{
		FireEvent();
	}

	public void OnGrenadeFired(object sender, EventArgs args)
	{
		FireEvent();
	}

	public void OnWeaponSwitched(object sender, EventArgs args)
	{
		FireEvent();
	}

	private void OnAIFirstPersonSwitch(object sender, EventArgs args)
	{
		FireEvent();
	}

	private void OnUnitFirstPersonSwitch(object sender, EventArgs args)
	{
		FireEvent();
	}

	private void OnFirstPersonReloadClick(object sender, EventArgs args)
	{
		if (GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.weapon != null && GameController.Instance.mFirstPersonActor.weapon.CanReload())
		{
			FireEvent();
		}
	}

	public void OnOverWatch(object sender, EventArgs args)
	{
		if (CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.PlayCamera && GameController.Instance.mFirstPersonActor == null)
		{
			FireEvent();
		}
	}

	public void OnSniper(object sender, EventArgs args)
	{
		if (GameController.Instance.mFirstPersonActor != null)
		{
			FireEvent();
		}
	}

	public void OnDestroy()
	{
		switch (HudEvent)
		{
		case HudEvents.Fire:
			EventHub.Instance.OnWeaponFired -= OnWeaponFired;
			break;
		case HudEvents.GoIntoOverWatch:
			CommonHudController.Instance.OnZoomOutTriggered -= OnOverWatch;
			break;
		case HudEvents.GoIntoSniper:
			CommonHudController.Instance.OnZoomInTriggered -= OnSniper;
			break;
		case HudEvents.Crouch:
			CommonHudController.Instance.OnDuckClick -= CrouchTest;
			break;
		case HudEvents.Reload:
			CommonHudController.Instance.OnReloadClick -= OnFirstPersonReloadClick;
			break;
		case HudEvents.AiTargetSwitch:
			CommonHudController.Instance.OnSnapEnemyLeftClick -= OnAIFirstPersonSwitch;
			CommonHudController.Instance.OnSnapEnemyRightClick -= OnAIFirstPersonSwitch;
			break;
		case HudEvents.CharacterSwitch:
			CommonHudController.Instance.OnSnapUnitLeftClick -= OnUnitFirstPersonSwitch;
			CommonHudController.Instance.OnSnapUnitRightClick -= OnUnitFirstPersonSwitch;
			break;
		case HudEvents.ThrowGrenade:
			CommonHudController.Instance.OnThrowGrenadeClick -= OnGrenadeFired;
			break;
		case HudEvents.SwitchWeapon:
			CommonHudController.Instance.OnSwitchWeapon -= OnWeaponSwitched;
			break;
		case HudEvents.DraggedRoute:
			GameplayController.Instance().OnFingerDragEndClick -= OnDraggedRoute;
			break;
		}
	}
}
