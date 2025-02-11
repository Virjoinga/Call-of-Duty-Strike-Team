using System;
using UnityEngine;

internal class Weapon_M1216 : IWeapon, IWeaponADS, IWeaponAI, IWeaponStats
{
	private WeaponDescriptor_M1216 mDescriptor;

	private bool mTrigger;

	private bool mWantsReload;

	private WeaponADS mWeaponADS;

	private WeaponAmmo mAmmo;

	private float mRecoilTimeRemaining;

	private float mRechamberTimeRemaining;

	private float mReloadTimeRemaining;

	private bool mWaitingForAmmo;

	private StandardWeaponEquip mWeaponEquip;

	private StandardWeaponMovement mWeaponMovement;

	private bool mTacticalReload;

	private GameObject mWeaponModel;

	private Transform mMuzzleLocator;

	private CasingEffect mCasingEffect;

	private float mADSModifier;

	private float mAmmoModifier;

	private float mReloadModifier;

	private AudioFilter m_AudioFilter;

	public Weapon_M1216(WeaponDescriptor_M1216 descriptor, GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		mDescriptor = descriptor;
		mADSModifier = adsModifier;
		mAmmoModifier = ammoModifier;
		mReloadModifier = reloadModifier;
		mWeaponModel = WeaponUtils.CreateThirdPersonModel(model, mDescriptor);
		mMuzzleLocator = mWeaponModel.transform.FindInHierarchy("MUZZLE_FLASH");
		mWeaponADS = new WeaponADS(0.2f, mADSModifier);
		mAmmo = new WeaponAmmo(mDescriptor, mAmmoModifier);
		mWeaponEquip = new StandardWeaponEquip(this, mWeaponModel, descriptor);
		mWeaponMovement = new StandardWeaponMovement();
		m_AudioFilter = descriptor.ThirdPersonAudioFilter;
		WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.M1216MuzzleFlash);
		mCasingEffect = WeaponUtils.CreateCasingEffect(mWeaponModel.transform.FindInHierarchy("cartridge"), EffectsController.Instance.Effects.M1216CasingEject);
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
		return WeaponUtils.CalculateStandardFieldOfView(GetHipsToSightsBlendAmount(), mDescriptor.AimDownSightsFOV);
	}

	public float GetCrosshairOpacity()
	{
		return Mathf.Min(1f - mWeaponADS.BlendAmount, mWeaponEquip.GetEquipedAmount());
	}

	public string GetId()
	{
		return "M1216";
	}

	public WeaponDescriptor.WeaponClass GetClass()
	{
		return WeaponDescriptor.WeaponClass.Shotgun;
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
		return false;
	}

	public bool IsSilenced()
	{
		return false;
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
				mRechamberTimeRemaining = 0f;
			}
			if (mWaitingForAmmo && GetReloadingAmount() >= mDescriptor.ValidReloadThreshold)
			{
				mWaitingForAmmo = false;
				mAmmo.Reload();
			}
		}
		else if (IsRechambering())
		{
			mRechamberTimeRemaining -= deltaTime;
		}
		if (mAmmo.NeedsReload)
		{
			StartReloading(owner);
		}
		if (mWantsReload && sfs.animsAllowReload)
		{
			mTacticalReload = mAmmo.Loaded > 0;
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
		}
		mWeaponMovement.Update(deltaTime);
	}

	private bool TryToFire(BaseCharacter owner, Vector3 position, Vector3 target)
	{
		if (mWeaponEquip.IsPuttingAway() || mWeaponEquip.IsTakingOut() || IsReloading() || IsRechambering() || IsRecoiling())
		{
			return false;
		}
		mAmmo.Take(1, owner.IsFirstPerson);
		mRecoilTimeRemaining = mDescriptor.TimeBetweenShots;
		if (GameSettings.Instance.PerksEnabled)
		{
			mRechamberTimeRemaining = ((mAmmo.Loaded == 0 || mAmmo.Loaded % 4 != 0) ? 0f : (mDescriptor.TimeToRechamber * mReloadModifier));
		}
		else
		{
			mRechamberTimeRemaining = ((mAmmo.Loaded == 0 || mAmmo.Loaded % 4 != 0) ? 0f : mDescriptor.TimeToRechamber);
		}
		if (owner != null)
		{
			if (owner.IsFirstPerson || m_AudioFilter == null)
			{
				WeaponSFX.Instance.M1216Fire.Play(owner.gameObject);
			}
			else
			{
				WeaponSFX.Instance.M1216Fire.Play(owner.gameObject, m_AudioFilter);
			}
		}
		Vector3 origin = position;
		if (owner.IsFirstPerson)
		{
			origin = ViewModelRig.Instance().Fire(GetId());
		}
		else
		{
			WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.M1216MuzzleFlash);
			mCasingEffect.Fire(false);
			if (mMuzzleLocator != null)
			{
				origin = mMuzzleLocator.position;
			}
		}
		Vector3 normalized = (target - position).normalized;
		for (int i = 0; i < 8; i++)
		{
			target = position + normalized * 1000f + mDescriptor.ShotSpread * UnityEngine.Random.insideUnitSphere;
			SurfaceImpact impact = ProjectileManager.Instance.StartProjectile(owner.myActor, position, target, CalculateDamage, CalculateImpactForce, !(owner != null) || !owner.IsFirstPerson);
			WeaponUtils.FeelTheWind(owner, impact);
			WeaponUtils.TriggerProjectileEffects(owner, origin, impact, false);
		}
		WeaponUtils.ApplyRecoil(owner, mDescriptor);
		return true;
	}

	public bool IsTacticalReload()
	{
		return mTacticalReload;
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
			return 1f - Mathf.Clamp01(mRechamberTimeRemaining / (mDescriptor.TimeToRechamber * mReloadModifier));
		}
		return 1f - Mathf.Clamp01(mRechamberTimeRemaining / mDescriptor.TimeToRechamber);
	}

	public bool IsRecoiling()
	{
		return mRecoilTimeRemaining > 0f;
	}

	public float GetRecoilingAmount()
	{
		return 1f - Mathf.Clamp01(mRecoilTimeRemaining / mDescriptor.TimeBetweenShots);
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
				if (eventToPlay == 0)
				{
					WeaponSFX.Instance.M1216Charge.Play(gameObject);
				}
			}
			else if (IsTacticalReload())
			{
				switch (eventToPlay)
				{
				case 0:
					WeaponSFX.Instance.M1216ReloadOut.Play(gameObject);
					break;
				case 1:
					WeaponSFX.Instance.M1216ReloadIn.Play(gameObject);
					break;
				}
			}
			else
			{
				switch (eventToPlay)
				{
				case 0:
					WeaponSFX.Instance.M1216ReloadOut.Play(gameObject);
					break;
				case 1:
					WeaponSFX.Instance.M1216ReloadIn.Play(gameObject);
					break;
				case 2:
					WeaponSFX.Instance.M1216Charge.Play(gameObject);
					break;
				case 3:
					WeaponSFX.Instance.M1216Charge.Play(gameObject);
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
}
