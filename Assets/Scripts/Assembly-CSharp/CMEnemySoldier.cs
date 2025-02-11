using UnityEngine;

public class CMEnemySoldier : CMSoldier
{
	public static bool AllowTapToShoot;

	private MenuButton mShoot;

	private MenuButton mSuppress;

	private MenuButton mMelee;

	private Material mLineMaterial;

	private bool mDeferToCMDM;

	protected override bool validInFirstPerson()
	{
		return UseBlip;
	}

	protected void Start_Old()
	{
		UseBlip = false;
		base.Start();
		target = base.AssociatedObject.GetComponent<Actor>();
		quickType = QuickType.EnemySoldier;
		target.health.OnHealthChange += HandleTargethealthOnHealthChange;
	}

	protected override void Start()
	{
		UseBlip = true;
		base.Start();
		(base.ContextBlip as ContextObjectBlip).StartViewable = true;
		mDeferToCMDM = false;
		target = base.AssociatedObject.GetComponent<Actor>();
		quickType = QuickType.EnemySoldier;
		target.health.OnHealthChange += HandleTargethealthOnHealthChange;
		base.IgnoreTutorialLock = true;
	}

	public override void Update()
	{
		if (mDeferToCMDM)
		{
			Update_Old();
		}
		else if (base.ContextBlip != null)
		{
			base.ContextBlip.renderer.enabled = false;
			mContextBlip.IsAllowedInFirstPerson = false;
			if (target != null && (target.realCharacter.Location == null || target.realCharacter.Location.ShouldShowInteriorObjects()) && base.ContextBlip.IsOnScreen && GameController.Instance != null && !GameController.Instance.IsFirstPerson)
			{
				base.collider.enabled = true;
				base.ContextBlip.SwitchOn();
			}
			else
			{
				base.collider.enabled = false;
				base.ContextBlip.SwitchOff();
				base.ContextBlip.collider.enabled = false;
			}
		}
		if (mSuppress != null)
		{
			mSuppress.SetButtonActive(target.realCharacter.CanBeSuppressed() && TutorialToggles.CMSupressLockState != TutorialToggles.CMButtonLockState.GreyedOut);
		}
		if (mShoot != null)
		{
			mShoot.SetButtonActive(TutorialToggles.CMAimedShotLockState != TutorialToggles.CMButtonLockState.GreyedOut);
		}
		if (mMelee != null)
		{
			mMelee.SetButtonActive(target.awareness.ChDefCharacterType == CharacterType.Human && TutorialToggles.CMMeeleLockState != TutorialToggles.CMButtonLockState.GreyedOut);
		}
		AdjustCollider(target);
		base.Update();
	}

	public void Update_Old()
	{
		if (target.realCharacter.CanBeCarried() && !target.realCharacter.IsBeingCarried && TutorialToggles.CMCarryBodyLockState != TutorialToggles.CMButtonLockState.Hidden)
		{
			if (base.collider != null)
			{
			}
			ShowBlip(true);
		}
		else
		{
			if (base.collider != null)
			{
				base.collider.enabled = false;
			}
			ShowBlip(false);
		}
		base.InvocationType = InvocationTypeEnum.Immediate;
	}

	private void HandleTargethealthOnHealthChange(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		if (target != null && (target.health.IsMortallyWounded() || target.health.Health <= 0f) && !mDeferToCMDM)
		{
			mDeferToCMDM = true;
			ContextMenuDistanceManager contextMenuDistanceManager = base.gameObject.AddComponent<ContextMenuDistanceManager>();
			if (contextMenuDistanceManager != null)
			{
				contextMenuDistanceManager.Radius = 2.1f;
			}
			if (ActStructure.Instance != null && ActStructure.Instance.CurrentMissionSectionIsTutorial() && TutorialToggles.CMCarryBodyLockState != 0)
			{
				Deactivate();
			}
		}
	}

	private void OnDestroy()
	{
		if (target != null)
		{
			target.health.OnHealthChange -= HandleTargethealthOnHealthChange;
		}
	}

	protected override void ClearMenuItems()
	{
		if (CommonHudController.Instance.ContextMenu != null)
		{
			ContextMenuBase contextMenu = CommonHudController.Instance.ContextMenu;
			mShoot = contextMenu.GetButton(ContextMenuIcons.Shoot);
			mSuppress = contextMenu.GetButton(ContextMenuIcons.Supress);
			mMelee = contextMenu.GetButton(ContextMenuIcons.Melee);
		}
		else
		{
			mShoot = (mSuppress = (mMelee = null));
		}
		base.ClearMenuItems();
	}

	protected override void PopulateMenuItems(bool fromTap)
	{
		mShoot = null;
		mSuppress = null;
		mMelee = null;
		GameplayController gameplayController = GameplayController.Instance();
		if (AllowTapToShoot && fromTap)
		{
			if (!target.realCharacter.IsDead() && !target.realCharacter.IsMortallyWounded() && gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Shoot) && target.awareness.ChDefCharacterType != CharacterType.SentryGun)
			{
				S_CMShoot();
			}
			return;
		}
		if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Carry) && target.realCharacter.CanBeCarried() && TutorialToggles.CMCarryBodyLockState != TutorialToggles.CMButtonLockState.Hidden)
		{
			AddCallableMethod("S_CMCarry", ContextMenuIcons.PickupBody);
		}
		else if (!target.realCharacter.IsDead() && !target.realCharacter.IsMortallyWounded())
		{
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Shoot) && target.awareness.ChDefCharacterType != CharacterType.SentryGun && TutorialToggles.CMAimedShotLockState != TutorialToggles.CMButtonLockState.Hidden)
			{
				AddCallableMethod("S_CMShoot", ContextMenuIcons.Shoot);
			}
			if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Suppress) && TutorialToggles.CMSupressLockState != TutorialToggles.CMButtonLockState.Hidden)
			{
				AddCallableMethod("S_CMSuppress", ContextMenuIcons.Supress);
			}
			if (IsTargetNotAlert(target) && gameplayController.AnySelectedAllowedCMOptionAndCanReach(ContextMenuOptionType.MeleeAttack, target) && TutorialToggles.CMMeeleLockState != TutorialToggles.CMButtonLockState.Hidden)
			{
				AddCallableMethod("S_CMMeleeAttack", ContextMenuIcons.Melee);
			}
		}
		if (ContextMenuVisualiser.ContextMenuDebugOptions)
		{
			AddCallableMethod("S_CMKill", ContextMenuIcons.Debug);
			AddCallableMethod("S_CMMortallyWound", ContextMenuIcons.Debug);
		}
	}

	protected override void SetIconForDefaultOption()
	{
		base.SetIconForDefaultOption();
	}

	public void S_CMCarry()
	{
		if (target != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderCarry(gameplayController, target);
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMShoot()
	{
		if (target != null && TutorialToggles.CMAimedShotLockState == TutorialToggles.CMButtonLockState.Active)
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderShootAtTarget(gameplayController, target, false);
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMSuppress()
	{
		if (target != null && mSuppress.IsActive())
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderShootAtTarget(gameplayController, target, true);
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMThrowGrenade()
	{
		if (target != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderPrimeGrenade(gameplayController, target.GetPosition());
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMMeleeAttack()
	{
		if (target != null && TutorialToggles.CMMeeleLockState == TutorialToggles.CMButtonLockState.Active && mMelee.IsActive())
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderMeleeAttack(gameplayController, target);
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMKill()
	{
		if (target != null)
		{
			target.realCharacter.Kill("MeleeDeath");
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMMortallyWound()
	{
		if (target != null)
		{
			target.health.ForceCriticallyInjured();
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public override int OnSelected(Vector2 selectedScreenPos, bool fromTap)
	{
		if (TutorialToggles.enableEnemyContextMenu)
		{
			return base.OnSelected(selectedScreenPos, fromTap);
		}
		return 0;
	}

	private bool IsTargetNotAlert(Actor target)
	{
		if (target == null)
		{
			return false;
		}
		return target.behaviour.alertState == BehaviourController.AlertState.Casual || target.behaviour.alertState == BehaviourController.AlertState.Focused;
	}
}
