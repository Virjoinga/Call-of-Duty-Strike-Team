using System;
using UnityEngine;

internal class Weapon_Sniper : IWeapon, IWeaponADS, IWeaponAI, IWeaponEquip, IWeaponStats
{
	private WeaponDescriptor_Sniper mDescriptor;

	private bool mTrigger;

	private bool mWantsReload;

	private WeaponADS mWeaponADS;

	private WeaponAmmo mAmmo;

	private float mRecoilTimeRemaining;

	private float mRechamberTimeRemaining;

	private float mReloadTimeRemaining;

	private float mTakeOutTime;

	private float mPutAwayTime;

	private float mLeftRightLean;

	private float mUpDownLean;

	private float mMovementAmount;

	private GameObject mWeaponModel;

	private Transform mMuzzleLocator;

	private CasingEffect mCasingEffect;

	private float mReloadAnimTime;

	private float mCurrentReloadAnimTime;

	private bool mAnimsInitialised;

	private float mADSModifier;

	private float mAmmoModifier;

	private AudioFilter m_AudioFilter;

	public Weapon_Sniper(WeaponDescriptor_Sniper descriptor, GameObject model)
	{
		mDescriptor = descriptor;
		mADSModifier = 1f;
		mAmmoModifier = 1f;
		mWeaponModel = WeaponUtils.CreateThirdPersonModel(model, mDescriptor);
		mMuzzleLocator = mWeaponModel.transform.FindInHierarchy("muzzle_flash");
		mWeaponADS = new WeaponADS(0.2f, mADSModifier);
		mAmmo = new WeaponAmmo(mDescriptor, mAmmoModifier);
		m_AudioFilter = descriptor.ThirdPersonAudioFilter;
		WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.BallistaMuzzleFlash);
		mCasingEffect = WeaponUtils.CreateCasingEffect(mWeaponModel.transform.FindInHierarchy("cartridge"), EffectsController.Instance.Effects.BallistaCasingEject);
		Reset();
	}

	public object QueryInterface(Type t)
	{
		return this;
	}

	public void Reset()
	{
		mRecoilTimeRemaining = 0f;
		mRechamberTimeRemaining = 0f;
		mReloadTimeRemaining = 0f;
		mPutAwayTime = float.MinValue;
		mTakeOutTime = float.MinValue;
		mWeaponADS.Reset();
		mAmmo.Reset();
	}

	public void DepressTrigger()
	{
		mTrigger = true;
	}

	public void ReleaseTrigger()
	{
		mTrigger = false;
	}

	public void StartReloading(BaseCharacter owner)
	{
		mWantsReload = true;
		mCurrentReloadAnimTime = 0f;
	}

	public void ReloadImmediately()
	{
		mAmmo.Reload();
	}

	public void CancelReload()
	{
		mReloadTimeRemaining = 0f;
	}

	public void PutAway(float speedModifier)
	{
		SwitchToHips();
		if (!IsPuttingAway())
		{
			mPutAwayTime = Time.time;
			if (IsTakingOut())
			{
				mPutAwayTime -= Mathf.Lerp(mDescriptor.PutAwayTime, 0f, GetEquipedBlendAmount());
			}
			mTakeOutTime = 0f;
			mWeaponModel.SetActive(false);
		}
	}

	public void TakeOut(float speedModifier)
	{
		if (!IsTakingOut())
		{
			mTakeOutTime = Time.time;
			if (IsPuttingAway())
			{
				mTakeOutTime -= Mathf.Lerp(mDescriptor.TakeOutTime, 0f, GetEquipedBlendAmount());
			}
			mPutAwayTime = 0f;
			mWeaponModel.SetActive(true);
		}
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

	public float GetEquipedBlendAmount()
	{
		if (IsPuttingAway())
		{
			return Mathf.Clamp01((Time.time - mPutAwayTime) / mDescriptor.PutAwayTime);
		}
		if (IsTakingOut())
		{
			return Mathf.Clamp01((Time.time - mTakeOutTime) / mDescriptor.TakeOutTime);
		}
		return 0f;
	}

	public float GetHipsToSightsBlendAmount()
	{
		return mWeaponADS.BlendAmount;
	}

	public float GetLeftRightLeanAmount()
	{
		return mLeftRightLean;
	}

	public float GetUpDownLeanAmount()
	{
		return mUpDownLean;
	}

	public float GetMovementAmount()
	{
		return mMovementAmount;
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
		return WeaponUtils.FormatAmmoString(mAmmo.Loaded, mAmmo.Stashed, IsReloading(), mDescriptor.UnlimitedAmmo);
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
			return 5f;
		}
		float hipsToSightsBlendAmount = GetHipsToSightsBlendAmount();
		float firstPersonFieldOfView = InputSettings.FirstPersonFieldOfView;
		float aimDownSightsFOV = mDescriptor.AimDownSightsFOV;
		return Mathf.Lerp(firstPersonFieldOfView, aimDownSightsFOV, hipsToSightsBlendAmount);
	}

	public float GetCrosshairOpacity()
	{
		return 1f - mWeaponADS.BlendAmount;
	}

	public string GetId()
	{
		return "Sniper";
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

	public bool IsPuttingAway()
	{
		return Time.time < mPutAwayTime + mDescriptor.PutAwayTime;
	}

	public bool IsTakingOut()
	{
		return Time.time < mTakeOutTime + mDescriptor.TakeOutTime;
	}

	public bool HasNoWeapon()
	{
		return false;
	}

	public void HasNoWeapon(bool noWeapon)
	{
	}

	public bool IsLongRangeShot(float distSquared)
	{
		return WeaponUtils.IsLongRangeShot(mDescriptor, distSquared);
	}

	public bool IsHeadShotAllowed(float distSquared)
	{
		return WeaponUtils.IsHeadShotAllowed(mDescriptor, distSquared);
	}

	public Transform GetMuzzleLocator()
	{
		return mMuzzleLocator;
	}

	public Vector3 GetMuzzleDir()
	{
		return mMuzzleLocator.transform.forward;
	}

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
		sfs = sfs ?? new SoldierFiringState();
		if (!mAnimsInitialised)
		{
			owner.myActor.animDirector.EnableOverride(owner.myActor.animDirector.GetOverrideHandle("Sniper"), true);
			mAnimsInitialised = true;
		}
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
		}
		else if (IsRechambering())
		{
			mRechamberTimeRemaining -= deltaTime;
		}
		if (mWantsReload && sfs.animsAllowReload)
		{
			mAmmo.Reload();
			mReloadTimeRemaining = mDescriptor.ReloadTime;
			WeaponSFX.Instance.AK47Reload.Play(owner.gameObject);
			mWantsReload = false;
		}
		Vector3 shotPosition = Vector3.zero;
		Vector3 shotTarget = Vector3.zero;
		if (owner != null)
		{
			if (!IsReloading())
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
			}
			else
			{
				mCurrentReloadAnimTime += deltaTime;
				if (mCurrentReloadAnimTime <= mReloadAnimTime && owner.myActor.baseCharacter.reloadActionHandle != null)
				{
					owner.myActor.animDirector.PlayAction(owner.myActor.baseCharacter.reloadActionHandle);
				}
			}
			owner.myActor.aiGunHandler.PostProcess(deltaTime);
		}
		if (mTrigger)
		{
			TryToFire(owner, shotPosition, shotTarget);
		}
		float num = (1f - Mathf.Abs(mLeftRightLean)) * 1f;
		mLeftRightLean = Mathf.Clamp(mLeftRightLean + deltaTime * num * GameController.Instance.LastViewRotation.y, -1f, 1f);
		mLeftRightLean = Mathf.Lerp(mLeftRightLean, 0f, deltaTime);
		float num2 = (1f - Mathf.Abs(mUpDownLean)) * 1f;
		mUpDownLean = Mathf.Clamp(mUpDownLean + deltaTime * num2 * GameController.Instance.LastViewRotation.x, -1f, 1f);
		mUpDownLean = Mathf.Lerp(mUpDownLean, 0f, deltaTime);
		mMovementAmount = Mathf.Lerp(0f, 0.4f, Mathf.Clamp01(GameController.Instance.LastVelocity.magnitude));
	}

	private bool IsTracerRound()
	{
		return true;
	}

	private bool TryToFire(BaseCharacter owner, Vector3 position, Vector3 target)
	{
		if (IsPuttingAway() || IsTakingOut() || IsReloading() || IsRechambering() || IsRecoiling())
		{
			return false;
		}
		mAmmo.Take(1, owner.IsFirstPerson);
		mRecoilTimeRemaining = mDescriptor.RecoilTime;
		mRechamberTimeRemaining = mDescriptor.RechamberTime;
		if (mAmmo.NeedsReload)
		{
			StartReloading(owner);
			RawAnimation rawAnimation = owner.myActor.animDirector.PlayAction(owner.myActor.baseCharacter.reloadActionHandle);
			mReloadAnimTime = rawAnimation.AnimClip.length;
		}
		if (owner != null)
		{
			if (owner.IsFirstPerson || m_AudioFilter == null)
			{
				WeaponSFX.Instance.BallistaSilencedFire.Play(owner.gameObject);
			}
			else
			{
				WeaponSFX.Instance.BallistaSilencedFire.Play(owner.gameObject, m_AudioFilter);
			}
		}
		SurfaceImpact impact = ProjectileManager.Instance.StartProjectile((!(owner != null)) ? null : owner.myActor, position, target, CalculateDamage, CalculateImpactForce, !(owner != null) || !owner.IsFirstPerson);
		WeaponUtils.FeelTheWind(owner, impact);
		if (owner.IsFirstPerson)
		{
			Vector3 origin = ViewModelRig.Instance().Fire(GetId());
			WeaponUtils.TriggerProjectileEffects(owner, origin, impact, IsTracerRound());
		}
		else
		{
			WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.BallistaMuzzleFlash);
			WeaponUtils.TriggerProjectileEffects(owner, (!(mMuzzleLocator != null)) ? position : mMuzzleLocator.position, impact, IsTracerRound());
			mCasingEffect.Fire(false);
		}
		WeaponUtils.ApplyRecoil(owner, mDescriptor);
		return true;
	}

	public float GetReloadingAmount()
	{
		return 1f - Mathf.Clamp01(mReloadTimeRemaining / mDescriptor.ReloadTime);
	}

	public bool IsRechambering()
	{
		return !IsRecoiling() && !IsReloading() && mRechamberTimeRemaining > 0f;
	}

	public float GetRechamberingAmount()
	{
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
		if (target != null && target.Actor != null && target.Actor.health != null)
		{
			return target.Actor.health.HealthMax / 100f * mDescriptor.DamagePercentage;
		}
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
		return WorldHelper.PointingAtTarget3D(mMuzzleLocator, pos, radius);
	}

	public float GetSuppressionRadius()
	{
		return mDescriptor.SuppressionRadius;
	}

	public WeaponAmmo GetWeaponAmmo()
	{
		return mAmmo;
	}

	public float GetReloadDuration()
	{
		return mDescriptor.ReloadTime;
	}
}
