using UnityEngine;

public class PlayerWeapon : BaseActorComponent
{
	private const float kAimingRadius = 2f;

	private const float kFirstPersonDelayHideTime = 0f;

	private IWeapon mDesiredWeapon;

	private float mFirstPersonTime;

	private IWeapon mPreviousEquippedWeapon;

	public SoldierFiringState soldierFiringState = new SoldierFiringState();

	public IWeapon PrimaryWeapon { get; private set; }

	public IWeapon SecondaryWeapon { get; private set; }

	public IWeapon ActiveWeapon { get; private set; }

	public bool WeaponSelectionLocked { get; set; }

	public IWeapon DesiredWeapon
	{
		get
		{
			return mDesiredWeapon;
		}
		private set
		{
			mDesiredWeapon = value;
			if (mDesiredWeapon != null && (mDesiredWeapon == PrimaryWeapon || mDesiredWeapon == SecondaryWeapon))
			{
				mPreviousEquippedWeapon = mDesiredWeapon;
			}
		}
	}

	public float RunSpeed
	{
		get
		{
			bool playerControlled = myActor.behaviour != null && myActor.behaviour.PlayerControlled;
			return ActiveWeapon.GetRunSpeed(playerControlled, myActor.baseCharacter.IsFirstPerson);
		}
	}

	public bool Silenced
	{
		get
		{
			return ActiveWeapon.IsSilenced();
		}
	}

	public string Id
	{
		get
		{
			return ActiveWeapon.GetId();
		}
	}

	public WeaponDescriptor.WeaponClass Class
	{
		get
		{
			return ActiveWeapon.GetClass();
		}
	}

	public void Initialise(IWeapon primaryWeapon, IWeapon secondaryWeapon)
	{
		PrimaryWeapon = primaryWeapon;
		SecondaryWeapon = secondaryWeapon;
		IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(PrimaryWeapon);
		if (weaponEquip != null)
		{
			weaponEquip.TakeOut(1f);
		}
		IWeaponEquip weaponEquip2 = WeaponUtils.GetWeaponEquip(SecondaryWeapon);
		if (weaponEquip2 != null)
		{
			weaponEquip2.PutAway(1f);
		}
		ActiveWeapon = PrimaryWeapon;
		DesiredWeapon = PrimaryWeapon;
	}

	public void Pump()
	{
		IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(ActiveWeapon);
		bool flag = weaponEquip != null && weaponEquip.IsPuttingAway();
		bool flag2 = weaponEquip == null || !weaponEquip.HasNoWeapon();
		if (ActiveWeapon != DesiredWeapon && !flag && flag2)
		{
			ActiveWeapon = DesiredWeapon;
			AfterSwitchWeapon();
		}
		if (IsAIUsingPrimaryWeaponWithLowAmmo())
		{
			SwitchToSecondary();
		}
		else if (!ActiveWeapon.HasAmmo())
		{
			if (ActiveWeapon == PrimaryWeapon)
			{
				SwitchToSecondary();
			}
			else
			{
				SwitchToPrimary();
			}
		}
		if (flag2)
		{
			SetADS(myActor.baseCharacter.IsAimingDownSights);
			ActiveWeapon.Update(Time.deltaTime, myActor.baseCharacter, soldierFiringState);
		}
	}

	public void LateUpdate()
	{
		BaseCharacter baseCharacter = myActor.baseCharacter;
		if (ActiveWeapon == null)
		{
			return;
		}
		bool isFirstPerson = baseCharacter.IsFirstPerson;
		if (isFirstPerson)
		{
			mFirstPersonTime += Time.deltaTime;
			if (mFirstPersonTime > 0f)
			{
				baseCharacter.mForceOffscreen = true;
			}
		}
		else
		{
			mFirstPersonTime = 0f;
			baseCharacter.mForceOffscreen = false;
		}
		if (isFirstPerson)
		{
			FirstPersonCamera firstPersonCamera = GameController.Instance.FirstPersonCamera;
			if (firstPersonCamera != null)
			{
				firstPersonCamera.Fov = ((!(ViewModelRig.Instance() != null) || !ViewModelRig.Instance().IsOverrideActive) ? ActiveWeapon.GetDesiredFieldOfView() : InputSettings.FirstPersonFieldOfViewStandard);
			}
			if (!baseCharacter.IsInASetPiece)
			{
				baseCharacter.TurnToFaceDirection(baseCharacter.FirstPersonCamera.transform.forward);
			}
		}
	}

	public void Reset()
	{
		PrimaryWeapon.Reset();
		SecondaryWeapon.Reset();
		ActiveWeapon.Reset();
		DesiredWeapon.Reset();
		mFirstPersonTime = 0f;
	}

	public void SetADS(bool enabled)
	{
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(ActiveWeapon);
		if (weaponADS != null)
		{
			if (enabled)
			{
				weaponADS.SwitchToSights();
			}
			else
			{
				weaponADS.SwitchToHips();
			}
		}
	}

	public void SetTrigger(bool depressed)
	{
		if (ActiveWeapon != null)
		{
			if (depressed)
			{
				ActiveWeapon.DepressTrigger();
			}
			else
			{
				ActiveWeapon.ReleaseTrigger();
			}
		}
	}

	public void SwapTo(IWeapon temporaryWeapon, float speedModifier)
	{
		bool flag = temporaryWeapon != DesiredWeapon && temporaryWeapon != ActiveWeapon;
		if (temporaryWeapon != null)
		{
			DesiredWeapon = temporaryWeapon;
		}
		if (flag)
		{
			if (myActor.baseCharacter != null && myActor.baseCharacter.IsFirstPerson)
			{
				InterfaceSFX.Instance.WeaponSwitch.Play2D();
			}
			StartSwitchingWeapon(speedModifier);
		}
	}

	public void SwapToImmediately(IWeapon temporaryWeapon)
	{
		if (temporaryWeapon != null)
		{
			DesiredWeapon = temporaryWeapon;
			ActiveWeapon = DesiredWeapon;
		}
	}

	public void Drop()
	{
		if (ActiveWeapon != null)
		{
			ActiveWeapon.Drop();
		}
	}

	public void Toggle()
	{
		if (!WeaponSelectionLocked && !myActor.realCharacter.IsUsingFixedGun)
		{
			if (DesiredWeapon == PrimaryWeapon)
			{
				SwitchToSecondary();
			}
			else
			{
				SwitchToPrimary();
			}
		}
	}

	public void SwitchToSecondary()
	{
		if (!(myActor.baseCharacter == null) && myActor.baseCharacter.CanChangeWeapon)
		{
			DesiredWeapon = SecondaryWeapon;
			StartSwitchingWeapon(1f);
		}
	}

	public void SwitchToPrimary()
	{
		if (!(myActor.baseCharacter == null) && (myActor.behaviour.PlayerControlled || myActor.baseCharacter.CanChangeWeapon))
		{
			if (!PrimaryWeapon.HasAmmo())
			{
				SwitchToSecondary();
			}
			else
			{
				DesiredWeapon = PrimaryWeapon;
			}
			StartSwitchingWeapon(1f);
		}
	}

	public void SwitchToPrevious()
	{
		if (mPreviousEquippedWeapon != null && mPreviousEquippedWeapon == SecondaryWeapon)
		{
			SwitchToSecondary();
		}
		else
		{
			SwitchToPrimary();
		}
	}

	public bool IsUnarmed()
	{
		return ActiveWeapon == null || !ActiveWeapon.HasAmmo();
	}

	public bool IsFiring()
	{
		return ActiveWeapon.IsFiring();
	}

	public bool CanReload()
	{
		return ActiveWeapon.CanReload();
	}

	public bool CanZoomFurther()
	{
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(ActiveWeapon);
		if (weaponADS == null)
		{
			return false;
		}
		return weaponADS.GetADSState() == ADSState.Hips || weaponADS.GetADSState() == ADSState.SwitchingToHips;
	}

	public void Reload()
	{
		ActiveWeapon.StartReloading(myActor.baseCharacter);
	}

	public void ReloadAllImmediately()
	{
		PrimaryWeapon.ReloadImmediately();
		SecondaryWeapon.ReloadImmediately();
	}

	public bool IsReloading()
	{
		return ActiveWeapon.IsReloading();
	}

	public void ReadyForBreach()
	{
		if (DesiredWeapon.HasScope())
		{
			Toggle();
		}
		DesiredWeapon.ReloadImmediately();
	}

	public string GetAmmoString()
	{
		return ActiveWeapon.GetAmmoString();
	}

	public float GetPercentageAmmoInClip()
	{
		return ActiveWeapon.GetPercentageAmmo();
	}

	public float ReloadTime()
	{
		return 0f;
	}

	private void StartSwitchingWeapon(float speedModifier)
	{
		if (ActiveWeapon == null)
		{
			return;
		}
		ActiveWeapon.CancelReload();
		IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(ActiveWeapon);
		if (weaponEquip == null)
		{
			return;
		}
		if (ActiveWeapon == DesiredWeapon)
		{
			if (weaponEquip.IsPuttingAway())
			{
				weaponEquip.TakeOut(speedModifier);
			}
		}
		else
		{
			weaponEquip.PutAway(speedModifier);
		}
	}

	public void LowerWeapon()
	{
		if (ActiveWeapon != null)
		{
			IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(ActiveWeapon);
			if (weaponEquip != null && !weaponEquip.HasNoWeapon())
			{
				weaponEquip.PutAway(1f);
				weaponEquip.HasNoWeapon(true);
			}
		}
	}

	public void RaiseWeapon()
	{
		if (ActiveWeapon != null)
		{
			IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(ActiveWeapon);
			if (weaponEquip != null && weaponEquip.HasNoWeapon())
			{
				weaponEquip.HasNoWeapon(false);
				weaponEquip.TakeOut(1f);
			}
		}
	}

	private void AfterSwitchWeapon()
	{
		if (ActiveWeapon != null)
		{
			IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(ActiveWeapon);
			if (weaponEquip != null)
			{
				weaponEquip.TakeOut(1f);
			}
		}
	}

	public virtual bool IsShootingSilenced()
	{
		return Silenced & IsFiring();
	}

	public void PutAway()
	{
		IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(ActiveWeapon);
		if (weaponEquip != null)
		{
			weaponEquip.PutAway(1f);
		}
	}

	public void TakeOut()
	{
		TakeOut(1f);
	}

	public void TakeOut(float speedMultiplier)
	{
		IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(ActiveWeapon);
		if (weaponEquip != null)
		{
			weaponEquip.TakeOut(speedMultiplier);
		}
	}

	public void SetTarget(Actor target)
	{
		soldierFiringState.desiredTarget = target;
	}

	public Actor GetTarget()
	{
		return soldierFiringState.desiredTarget;
	}

	public void ClearTarget()
	{
		soldierFiringState.desiredTarget = null;
	}

	public void SetDesiredFiringBehaviour(SoldierFiringState.FireType ft)
	{
		soldierFiringState.desiredFireType = ft;
	}

	public bool ShootingIsAllowed()
	{
		return soldierFiringState.ShootingIsAllowed();
	}

	public bool ShootingIsDesiredAndAllowed()
	{
		return soldierFiringState.ShootingIsDesiredAndAllowed();
	}

	public SoldierFiringState.FireType DesiredFiringBehaviour()
	{
		return soldierFiringState.desiredFireType;
	}

	public void SetAiming(bool a)
	{
		IWeaponAI weaponAI = WeaponUtils.GetWeaponAI(ActiveWeapon);
		if (!a)
		{
			soldierFiringState.aimingAtTarget = false;
			return;
		}
		if (soldierFiringState.desiredTarget == null)
		{
			soldierFiringState.aimingAtTarget = false;
			return;
		}
		if (myActor.OnScreen)
		{
			a &= weaponAI != null && weaponAI.PointingAtTarget(soldierFiringState.desiredTarget.baseCharacter.GetBulletOrigin(), 2f);
		}
		soldierFiringState.aimingAtTarget = a;
	}

	public void AllowReload(bool a)
	{
		soldierFiringState.animsAllowReload = a & !myActor.animDirector.AnimsPreventReloading;
	}

	public void CharacterPermitsFiring(bool p)
	{
		soldierFiringState.characterPermitsFiring = p;
	}

	private bool IsAIUsingPrimaryWeaponWithLowAmmo()
	{
		return myActor.baseCharacter != null && !myActor.baseCharacter.IsFirstPerson && ActiveWeapon == PrimaryWeapon && PrimaryWeapon.LowAmmo();
	}
}
