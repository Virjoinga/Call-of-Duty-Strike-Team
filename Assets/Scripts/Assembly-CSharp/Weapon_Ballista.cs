using System;
using UnityEngine;

internal class Weapon_Ballista : IWeapon, IWeaponADS, IWeaponAI, IWeaponControl, IWeaponSway, IWeaponStats
{
	private WeaponDescriptor_Ballista mDescriptor;

	private bool mTrigger;

	private bool mHasDepressed;

	private bool mWantsReload;

	private WeaponADS mWeaponADS;

	private WeaponAmmo mAmmo;

	private float mRecoilTimeRemaining;

	private float mRechamberTimeRemaining;

	private float mReloadTimeRemaining;

	private bool mWaitingForAmmo;

	private bool mCasingToEject;

	private StandardWeaponEquip mWeaponEquip;

	private StandardWeaponMovement mWeaponMovement;

	private GameObject mWeaponModel;

	private Transform mMuzzleLocator;

	private CasingEffect mCasingEffect;

	private float mADSModifier;

	private float mAmmoModifier;

	private float mReloadModifier;

	public Weapon_Ballista(WeaponDescriptor_Ballista descriptor, GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		mDescriptor = descriptor;
		mADSModifier = adsModifier;
		mAmmoModifier = ammoModifier;
		mReloadModifier = reloadModifier;
		mWeaponModel = WeaponUtils.CreateThirdPersonModel(model, mDescriptor);
		mMuzzleLocator = mWeaponModel.transform.FindInHierarchy("muzzle_flash");
		mWeaponADS = new WeaponADS(0.2f, mADSModifier);
		mAmmo = new WeaponAmmo(mDescriptor, mAmmoModifier);
		mWeaponEquip = new StandardWeaponEquip(this, mWeaponModel, descriptor);
		mWeaponMovement = new StandardWeaponMovement();
		WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.BallistaMuzzleFlash);
		mCasingEffect = WeaponUtils.CreateCasingEffect(mWeaponModel.transform.FindInHierarchy("cartridge"), EffectsController.Instance.Effects.BallistaCasingEject);
		Reset();
	}

	public object QueryInterface(Type t)
	{
		if (t == typeof(IWeaponMovement))
		{
			return mWeaponMovement;
		}
		if (t == typeof(IWeaponEquip))
		{
			return mWeaponEquip;
		}
		return this;
	}

	public void Reset()
	{
		mRecoilTimeRemaining = 0f;
		mRechamberTimeRemaining = 0f;
		mReloadTimeRemaining = 0f;
		mCasingToEject = false;
		mWeaponADS.Reset();
		mAmmo.Reset();
		mWeaponEquip.Reset();
		mWeaponMovement.Reset();
	}

	public void DepressTrigger()
	{
		mTrigger = true;
	}

	public void ReleaseTrigger()
	{
		mTrigger = false;
	}

	public bool NeedsTriggerRelease()
	{
		return mHasDepressed;
	}

	public Vector2 GetSemiAutoFireRates()
	{
		return mDescriptor.SemiAutoFireRates;
	}

	public void StartReloading(BaseCharacter owner)
	{
		if (!IsReloading() && !mWeaponEquip.IsTakingOut() && !mWeaponEquip.IsPuttingAway())
		{
			if (owner != null && owner.myActor != null && owner.myActor.speech != null)
			{
				owner.myActor.speech.Reload(owner);
			}
			mWantsReload = true;
		}
	}

	public void ReloadImmediately()
	{
		mAmmo.Reload();
	}

	public void CancelReload()
	{
		mReloadTimeRemaining = 0f;
	}

	public void SwitchToHips()
	{
		mWeaponADS.WantsSights = false;
	}

	public void SwitchToSights()
	{
		mWeaponADS.WantsSights = true;
	}

	public void Drop()
	{
		if (mWeaponModel != null)
		{
			ActorAttachment component = mWeaponModel.GetComponent<ActorAttachment>();
			if (component != null)
			{
				component.Drop();
			}
		}
	}

	public ADSState GetADSState()
	{
		return mWeaponADS.State;
	}

	public float GetHipsToSightsBlendAmount()
	{
		return mWeaponADS.BlendAmount;
	}

	public bool HasAmmo()
	{
		return mAmmo.Available;
	}

	public bool LowAmmo()
	{
		return mAmmo.LowAmmo;
	}

	public float GetPercentageAmmo()
	{
		return mAmmo.PercentageLoaded;
	}

	public string GetAmmoString()
	{
		return WeaponUtils.FormatAmmoString(mAmmo.Loaded, mAmmo.Stashed, IsReloading(), mAmmo.UnlimitedAmmo);
	}

	public float GetFirstPersonAccuracy()
	{
		return mDescriptor.FirstPersonAccuracy;
	}

	public float GetThirdPersonAccuracy()
	{
		return mDescriptor.ThirdPersonAccuracy;
	}

	public float GetRunSpeed(bool playerControlled, bool isFirstPerson)
	{
		return WeaponUtils.CalculateRunSpeed(mDescriptor, (!isFirstPerson) ? 0f : mWeaponADS.BlendAmount, playerControlled);
	}

	public float GetDesiredFieldOfView()
	{
		if (mWeaponADS.State == ADSState.ADS)
		{
			return mDescriptor.ScopeFieldOfView;
		}
		float hipsToSightsBlendAmount = GetHipsToSightsBlendAmount();
		float firstPersonFieldOfView = InputSettings.FirstPersonFieldOfView;
		float aimDownSightsFOV = mDescriptor.AimDownSightsFOV;
		return Mathf.Lerp(firstPersonFieldOfView, aimDownSightsFOV, hipsToSightsBlendAmount);
	}

	public float GetCrosshairOpacity()
	{
		return Mathf.Min(1f - mWeaponADS.BlendAmount, mWeaponEquip.GetEquipedAmount());
	}

	public string GetId()
	{
		return "Ballista";
	}

	public WeaponDescriptor.WeaponClass GetClass()
	{
		return WeaponDescriptor.WeaponClass.SniperRifle;
	}

	public int GetWeaponType()
	{
		return (int)mDescriptor.Type;
	}

	public float AccuracyStatAdjustment()
	{
		return 1f;
	}

	public bool HasScope()
	{
		return true;
	}

	public bool IsSilenced()
	{
		return true;
	}

	public bool IsFiring()
	{
		return mRecoilTimeRemaining > 0f && !IsReloading();
	}

	public bool CanReload()
	{
		return mAmmo.CanReload;
	}

	public bool IsReloading()
	{
		return !IsRecoiling() && mReloadTimeRemaining > 0f;
	}

	public bool IsLongRangeShot(float distSquared)
	{
		return WeaponUtils.IsLongRangeShot(mDescriptor, distSquared);
	}

	public bool IsHeadShotAllowed(float distSquared)
	{
		return WeaponUtils.IsHeadShotAllowed(mDescriptor, distSquared);
	}

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
		sfs = sfs ?? new SoldierFiringState();
		mWeaponADS.Update(deltaTime, owner.IsADSSuppressed);
		if (IsRecoiling())
		{
			mRecoilTimeRemaining -= deltaTime;
		}
		else if (IsReloading())
		{
			mReloadTimeRemaining -= deltaTime;
			if (mReloadTimeRemaining <= 0f)
			{
				mRecoilTimeRemaining = 0f;
			}
			if (mWaitingForAmmo && GetReloadingAmount() >= mDescriptor.ValidReloadThreshold)
			{
				mWaitingForAmmo = false;
				mAmmo.Reload();
			}
		}
		else if (IsRechambering())
		{
			if (mCasingToEject)
			{
				if (owner.IsFirstPerson)
				{
					ViewModelRig.Instance().Fire(GetId(), ViewModelRig.FireEffects.CasingEject);
				}
				else
				{
					mCasingEffect.Fire(false);
				}
				mCasingToEject = false;
			}
			mRechamberTimeRemaining -= deltaTime;
		}
		if (mAmmo.NeedsReload)
		{
			StartReloading(owner);
		}
		if (mWantsReload && sfs.animsAllowReload)
		{
			mWaitingForAmmo = true;
			mReloadTimeRemaining = GetReloadDuration();
			if (!owner.IsFirstPerson)
			{
				WeaponSFX.Instance.AK47Reload.Play(owner.gameObject);
			}
			if ((bool)owner)
			{
				owner.PlayReload();
			}
			mWantsReload = false;
		}
		Vector3 shotPosition = Vector3.zero;
		Vector3 shotTarget = Vector3.zero;
		if (owner != null)
		{
			Actor target;
			if (owner.myActor.aiGunHandler.Process(deltaTime, owner, sfs, this, out target))
			{
				owner.myActor.aiGunHandler.Fire(owner as RealCharacter, target, this, out shotPosition, out shotTarget);
			}
			else if (owner.IsFirstPerson)
			{
				shotPosition = ViewModelRig.Instance().GetCrosshairCentre();
				shotTarget = shotPosition + WeaponUtils.GetFirstPersonShotDirection(owner.myActor, GetFirstPersonAccuracy());
			}
			owner.myActor.aiGunHandler.PostProcess(deltaTime);
		}
		if (mTrigger)
		{
			TryToFire(owner, shotPosition, shotTarget);
			mHasDepressed = true;
		}
		else
		{
			mHasDepressed = false;
		}
		mWeaponMovement.Update(deltaTime);
	}

	private bool TryToFire(BaseCharacter owner, Vector3 position, Vector3 target)
	{
		if (mWeaponEquip.IsPuttingAway() || mWeaponEquip.IsTakingOut() || IsReloading() || IsRechambering() || IsRecoiling() || mHasDepressed)
		{
			return false;
		}
		mAmmo.Take(1, owner.IsFirstPerson);
		mRecoilTimeRemaining = mDescriptor.RecoilTime;
		if (GameSettings.Instance.PerksEnabled)
		{
			mRechamberTimeRemaining = mDescriptor.RechamberTime * mReloadModifier;
		}
		else
		{
			mRechamberTimeRemaining = mDescriptor.RechamberTime;
		}
		mCasingToEject = true;
		if (owner != null)
		{
			WeaponSFX.Instance.BallistaSilencedFire.Play(owner.gameObject);
		}
		SurfaceImpact impact = ProjectileManager.Instance.StartProjectile((!(owner != null)) ? null : owner.myActor, position, target, CalculateDamage, CalculateImpactForce, !(owner != null) || !owner.IsFirstPerson);
		WeaponUtils.FeelTheWind(owner, impact);
		if (owner.IsFirstPerson)
		{
			Vector3 origin = ViewModelRig.Instance().Fire(GetId(), ViewModelRig.FireEffects.MuzzleFlash);
			WeaponUtils.TriggerProjectileEffects(owner, origin, impact, false);
		}
		else
		{
			WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.BallistaMuzzleFlash);
			WeaponUtils.TriggerProjectileEffects(owner, (!(mMuzzleLocator != null)) ? position : mMuzzleLocator.position, impact, true);
		}
		WeaponUtils.ApplyRecoil(owner, mDescriptor);
		return true;
	}

	public float GetReloadingAmount()
	{
		if (GameSettings.Instance.PerksEnabled)
		{
			return 1f - Mathf.Clamp01(mReloadTimeRemaining / (mDescriptor.ReloadTime * mReloadModifier));
		}
		return 1f - Mathf.Clamp01(mReloadTimeRemaining / mDescriptor.ReloadTime);
	}

	public bool IsRechambering()
	{
		return !IsRecoiling() && !IsReloading() && mRechamberTimeRemaining > 0f;
	}

	public float GetRechamberingAmount()
	{
		if (GameSettings.Instance.PerksEnabled)
		{
			return 1f - Mathf.Clamp01(mRechamberTimeRemaining / (mDescriptor.RechamberTime * mReloadModifier));
		}
		return 1f - Mathf.Clamp01(mRechamberTimeRemaining / mDescriptor.RechamberTime);
	}

	public bool IsRecoiling()
	{
		return mRecoilTimeRemaining > 0f;
	}

	public float GetRecoilingAmount()
	{
		return 1f - Mathf.Clamp01(mRecoilTimeRemaining / mDescriptor.RecoilTime);
	}

	public float CalculateDamage(float distance, HitLocation target, bool isPlayer)
	{
		return WeaponUtils.CalculateStandardDamage(distance, mDescriptor, GetClass(), isPlayer);
	}

	public float CalculateImpactForce()
	{
		return mDescriptor.ImpactForce;
	}

	public float CalculateUtility(float distance)
	{
		return WeaponUtils.CalculateStandardUtility(distance, mDescriptor);
	}

	public bool PointingAtTarget(Vector3 pos, float radius)
	{
		return WorldHelper.PointingAtTarget(mMuzzleLocator, pos, radius);
	}

	public float GetSuppressionRadius()
	{
		return mDescriptor.SuppressionRadius;
	}

	public WeaponAmmo GetWeaponAmmo()
	{
		return mAmmo;
	}

	public float PlayReloadSfxFP(float oldTimeInAnim, float newTimeInAnim, float[,] eventTimes, ref int whichEvent)
	{
		int eventToPlay = -1;
		newTimeInAnim = mAmmo.CalcReloadSfx(oldTimeInAnim, newTimeInAnim, IsRechambering(), eventTimes, ref whichEvent, ref eventToPlay);
		if (eventToPlay != -1 && GameController.Instance.mFirstPersonActor != null)
		{
			GameObject gameObject = GameController.Instance.mFirstPersonActor.baseCharacter.gameObject;
			if (IsRechambering())
			{
				switch (eventToPlay)
				{
				case 0:
					WeaponSFX.Instance.BallistaBoltBack.Play(gameObject);
					break;
				case 1:
					WeaponSFX.Instance.BallistaBoltForward.Play(gameObject);
					break;
				}
			}
			else
			{
				switch (eventToPlay)
				{
				case 0:
					WeaponSFX.Instance.BallistaMagOut.Play(gameObject);
					break;
				case 1:
					WeaponSFX.Instance.BallistaMagIn.Play(gameObject);
					break;
				}
			}
		}
		return newTimeInAnim;
	}

	public float GetReloadDuration()
	{
		if (GameSettings.Instance.PerksEnabled)
		{
			return mDescriptor.ReloadTime * mReloadModifier;
		}
		return mDescriptor.ReloadTime;
	}

	public float GetSwayMinimum()
	{
		return mDescriptor.SwayMinimum;
	}

	public float GetSwayMaximum()
	{
		return mDescriptor.SwayMaximum;
	}

	public float GetSwayFrequency()
	{
		return mDescriptor.SwayFrequency;
	}
}
