using System;
using UnityEngine;

internal class Weapon_Legacy : IWeapon
{
	private WeaponDescriptor mDescriptor;

	private bool mTrigger;

	private bool mWantsReload;

	private bool mWantsSights;

	private ADSState mADSState;

	private float mAdsBlendTime;

	private string mId;

	private float mTimeBetweenBullets;

	private float mReloadTime;

	private float mPutAwayDuration;

	private float mTakeOutDuration;

	private int mClipCapacity;

	private float mAimDownSightsFOV;

	private int mBulletStash;

	private bool mUnlimitedAmmo;

	private float mBulletGapCooldown;

	private float mReloadTimeRemaining;

	private float mTakeOutTime;

	private float mPutAwayTime;

	private int mBulletsRemaining;

	private int mLifetimeBulletsFired;

	private bool mIsSentryGun;

	private float mLeftRightLean;

	private float mUpDownLean;

	private float mMovementAmount;

	private Actor mTarget;

	public Weapon_Legacy(WeaponDescriptor descriptor, GameObject model)
	{
		mDescriptor = descriptor;
		mId = descriptor.name;
		mTimeBetweenBullets = 0.060000002f;
		mReloadTime = descriptor.ReloadTime;
		mPutAwayDuration = descriptor.PutAwayTime;
		mTakeOutDuration = descriptor.TakeOutTime;
		mClipCapacity = (int)descriptor.Capacity;
		mBulletStash = descriptor.BulletStartQuantity;
		mUnlimitedAmmo = descriptor.UnlimitedAmmo;
		mAimDownSightsFOV = descriptor.AimDownSightsFOV;
		WeaponUtils.CreateThirdPersonModel(model, mDescriptor);
		mIsSentryGun = descriptor.Name.ToLowerInvariant().Contains("sentrygun");
		Reset();
	}

	public object QueryInterface(Type t)
	{
		return this;
	}

	public void Reset()
	{
		mADSState = ADSState.Hips;
		mBulletGapCooldown = 0f;
		mReloadTimeRemaining = 0f;
		mBulletsRemaining = mClipCapacity;
		mLifetimeBulletsFired = 0;
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
	}

	public void ReloadImmediately()
	{
		if (mUnlimitedAmmo)
		{
			mBulletsRemaining = mClipCapacity;
			return;
		}
		int num = Mathf.Min(mBulletStash, mClipCapacity - mBulletsRemaining);
		mBulletsRemaining += num;
		mBulletStash -= num;
	}

	public void CancelReload()
	{
		mReloadTimeRemaining = 0f;
	}

	public void PutAway()
	{
		SwitchToHips();
		if (!IsPuttingAway())
		{
			mPutAwayTime = Time.time;
			if (IsTakingOut())
			{
				mPutAwayTime -= Mathf.Lerp(mPutAwayDuration, 0f, GetEquipedBlendAmount());
			}
			mTakeOutTime = 0f;
		}
	}

	public void TakeOut()
	{
		if (!IsTakingOut())
		{
			mTakeOutTime = Time.time;
			if (IsPuttingAway())
			{
				mTakeOutTime -= Mathf.Lerp(mTakeOutDuration, 0f, GetEquipedBlendAmount());
			}
			mPutAwayTime = 0f;
		}
	}

	public void SwitchToHips()
	{
		mWantsSights = false;
	}

	public void SwitchToSights()
	{
		mWantsSights = true;
	}

	public void Drop()
	{
	}

	public ADSState GetADSState()
	{
		return mADSState;
	}

	public float GetEquipedBlendAmount()
	{
		if (IsPuttingAway())
		{
			return Mathf.Clamp01((Time.time - mPutAwayTime) / mPutAwayDuration);
		}
		if (IsTakingOut())
		{
			return Mathf.Clamp01((Time.time - mTakeOutTime) / mTakeOutDuration);
		}
		return 0f;
	}

	public float GetHipsToSightsBlendAmount()
	{
		return WeaponUtils.CalculateHipsToSightsBlend(mADSState, mAdsBlendTime, ADSTransitionDuration());
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
		return mBulletStash > 0 || mBulletsRemaining > 0;
	}

	public bool LowAmmo()
	{
		return mBulletStash < (int)mDescriptor.Capacity;
	}

	public float GetPercentageAmmo()
	{
		return (float)mBulletsRemaining / (float)mClipCapacity;
	}

	public string GetAmmoString()
	{
		return string.Format("{0:D2} / {1}", (!IsReloading()) ? mBulletsRemaining : 0, mBulletStash);
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
		return WeaponUtils.CalculateRunSpeed(mDescriptor, (!isFirstPerson) ? 0f : GetHipsToSightsBlendAmount(), playerControlled);
	}

	public float GetDesiredFieldOfView()
	{
		float hipsToSightsBlendAmount = GetHipsToSightsBlendAmount();
		float firstPersonFieldOfView = InputSettings.FirstPersonFieldOfView;
		float to = mAimDownSightsFOV;
		return Mathf.Lerp(firstPersonFieldOfView, to, hipsToSightsBlendAmount);
	}

	public float GetCrosshairOpacity()
	{
		return 1f - GetHipsToSightsBlendAmount();
	}

	public string GetId()
	{
		return mId;
	}

	public WeaponDescriptor.WeaponClass GetClass()
	{
		return WeaponDescriptor.WeaponClass.Special;
	}

	public int GetWeaponType()
	{
		return (int)mDescriptor.Type;
	}

	public bool HasScope()
	{
		return false;
	}

	public bool IsSilenced()
	{
		return false;
	}

	public bool IsFiring()
	{
		return mBulletGapCooldown > 0f && !IsReloading();
	}

	public bool CanReload()
	{
		return mBulletsRemaining < mClipCapacity;
	}

	public bool IsReloading()
	{
		return mReloadTimeRemaining > 0f;
	}

	public bool IsPuttingAway()
	{
		return Time.time < mPutAwayTime + mPutAwayDuration;
	}

	public bool IsTakingOut()
	{
		return Time.time < mTakeOutTime + mTakeOutDuration;
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
		if (IsReloading())
		{
			mWantsSights = false;
		}
		if (mWantsSights && mADSState != 0)
		{
			mAdsBlendTime = Mathf.Lerp(0f, ADSTransitionDuration(), GetHipsToSightsBlendAmount());
			mADSState = ADSState.SwitchingToADS;
		}
		else if (!mWantsSights && mADSState != ADSState.Hips)
		{
			mAdsBlendTime = Mathf.Lerp(ADSTransitionDuration(), 0f, GetHipsToSightsBlendAmount());
			mADSState = ADSState.SwitchingToHips;
		}
		if (IsInADSTransition() && mAdsBlendTime >= ADSTransitionDuration())
		{
			mADSState = ((mADSState != ADSState.SwitchingToADS) ? ADSState.Hips : ADSState.ADS);
		}
		mAdsBlendTime += deltaTime;
		if (IsReloading())
		{
			mReloadTimeRemaining -= deltaTime;
			if (mReloadTimeRemaining <= 0f)
			{
				mBulletGapCooldown = 0f;
			}
		}
		if (mBulletGapCooldown > 0f)
		{
			mBulletGapCooldown -= deltaTime;
		}
		if (mWantsReload && sfs.animsAllowReload)
		{
			if (mUnlimitedAmmo)
			{
				mBulletsRemaining = mClipCapacity;
			}
			else
			{
				int num = Mathf.Min(mBulletStash, mClipCapacity - mBulletsRemaining);
				mBulletsRemaining += num;
				mBulletStash -= num;
			}
			mReloadTimeRemaining = mReloadTime;
			WeaponSFX.Instance.AK47Reload.Play(owner.gameObject);
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
		}
		float num2 = (1f - Mathf.Abs(mLeftRightLean)) * 1f;
		mLeftRightLean = Mathf.Clamp(mLeftRightLean + deltaTime * num2 * GameController.Instance.LastViewRotation.y, -1f, 1f);
		mLeftRightLean = Mathf.Lerp(mLeftRightLean, 0f, deltaTime);
		float num3 = (1f - Mathf.Abs(mUpDownLean)) * 1f;
		mUpDownLean = Mathf.Clamp(mUpDownLean + deltaTime * num3 * GameController.Instance.LastViewRotation.x, -1f, 1f);
		mUpDownLean = Mathf.Lerp(mUpDownLean, 0f, deltaTime);
		mMovementAmount = Mathf.Lerp(0f, 0.4f, Mathf.Clamp01(GameController.Instance.LastVelocity.magnitude));
	}

	private bool IsTracerRound()
	{
		return mBulletsRemaining % 3 == 0;
	}

	private bool TryToFire(BaseCharacter owner, Vector3 position, Vector3 target)
	{
		if (IsPuttingAway() || IsTakingOut() || IsReloading())
		{
			return false;
		}
		if (mBulletGapCooldown > 0f)
		{
			return false;
		}
		if (!mIsSentryGun)
		{
			mBulletsRemaining--;
		}
		if (mBulletsRemaining <= 0 && (mBulletStash > 0 || mUnlimitedAmmo))
		{
			StartReloading(owner);
		}
		else
		{
			mBulletGapCooldown = mTimeBetweenBullets;
		}
		mLifetimeBulletsFired++;
		if (owner != null)
		{
			WeaponSFX.Instance.AK47Fire.Play(owner.gameObject);
		}
		SurfaceImpact impact = ProjectileManager.Instance.StartProjectile((!(owner != null)) ? null : owner.myActor, position, target, CalculateDamage, CalculateImpactForce, !(owner != null) || !owner.IsFirstPerson);
		WeaponUtils.FeelTheWind(owner, impact);
		WeaponUtils.TriggerProjectileEffects(owner, position, impact, IsTracerRound());
		WeaponUtils.ApplyRecoil(owner, mDescriptor);
		return true;
	}

	private bool IsInADSTransition()
	{
		return mADSState == ADSState.SwitchingToADS || mADSState == ADSState.SwitchingToHips;
	}

	private float ADSTransitionDuration()
	{
		return 0.2f;
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
		return true;
	}

	public float GetSuppressionRadius()
	{
		return mDescriptor.SuppressionRadius;
	}

	public WeaponAmmo GetWeaponAmmo()
	{
		return null;
	}

	public float GetReloadDuration()
	{
		return mDescriptor.ReloadTime;
	}
}
