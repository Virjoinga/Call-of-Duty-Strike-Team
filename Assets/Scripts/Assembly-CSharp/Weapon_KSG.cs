using System;
using UnityEngine;

internal class Weapon_KSG : IWeapon, IWeaponADS, IWeaponAI, IWeaponStats
{
	public enum ReloadState
	{
		Ready = 0,
		Start = 1,
		Loop = 2,
		End = 3
	}

	private WeaponDescriptor_KSG mDescriptor;

	private bool mTrigger;

	private bool mWantsReload;

	private WeaponADS mWeaponADS;

	private WeaponAmmo mAmmo;

	private float mBulletGapCooldown;

	private ReloadState mReloadState;

	private float mReloadStateTime;

	private bool mCasingToEject;

	private StandardWeaponEquip mWeaponEquip;

	private StandardWeaponMovement mWeaponMovement;

	private GameObject mWeaponModel;

	private Transform mMuzzleLocator;

	private CasingEffect mCasingEffect;

	private float mADSModifier;

	private float mAmmoModifier;

	private float mReloadModifier;

	private AudioFilter m_AudioFilter;

	public Weapon_KSG(WeaponDescriptor_KSG descriptor, GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
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
		m_AudioFilter = descriptor.ThirdPersonAudioFilter;
		WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.KSGMuzzleFlash);
		mCasingEffect = WeaponUtils.CreateCasingEffect(mWeaponModel.transform.FindInHierarchy("cartridge"), EffectsController.Instance.Effects.KSGCasingEject);
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
		mBulletGapCooldown = 0f;
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
		mWantsReload = false;
		mReloadState = ReloadState.Ready;
		mReloadStateTime = 0f;
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
		return "KSG";
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
		return mBulletGapCooldown > 0f && !IsReloading();
	}

	public bool CanReload()
	{
		return mAmmo.CanReload;
	}

	public bool IsReloading()
	{
		return !IsRecoiling() && mReloadState != ReloadState.Ready;
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
			mBulletGapCooldown -= deltaTime;
			if (mCasingToEject && GetRecoilingAmount() > 0.5f)
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
		}
		else if (IsReloading())
		{
			mReloadStateTime += deltaTime;
			if (GameSettings.Instance.PerksEnabled)
			{
				ReloadUpdate(mReloadModifier, owner);
			}
			else
			{
				ReloadUpdate(1f, owner);
			}
		}
		if (!IsReloading())
		{
			if (mAmmo.NeedsReload)
			{
				StartReloading(owner);
			}
			if (mWantsReload && sfs.animsAllowReload)
			{
				mReloadState = ReloadState.Start;
				mReloadStateTime = 0f;
				if ((bool)owner)
				{
					owner.PlayReload();
				}
			}
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

	private void ReloadUpdate(float modifier, BaseCharacter owner)
	{
		switch (mReloadState)
		{
		case ReloadState.Start:
			if (mReloadStateTime > mDescriptor.TimeForReloadStart * modifier)
			{
				mReloadStateTime -= mDescriptor.TimeForReloadStart * modifier;
				mReloadState = ReloadState.Loop;
				if (owner != null && !owner.IsFirstPerson)
				{
					WeaponSFX.Instance.AK47Reload.Play(owner.gameObject);
				}
			}
			break;
		case ReloadState.Loop:
			if (mAmmo.Stashed == 0)
			{
				mReloadState = ReloadState.End;
			}
			else if (mReloadStateTime > mDescriptor.TimeForReloadSingleShell * modifier)
			{
				mReloadStateTime -= mDescriptor.TimeForReloadSingleShell * modifier;
				mAmmo.Reload(1);
				if (!mWantsReload || !mAmmo.CanReload)
				{
					mReloadState = ReloadState.End;
				}
			}
			if (mTrigger && GetReloadStateAmount(modifier) > 0.5f)
			{
				mWantsReload = false;
			}
			break;
		case ReloadState.End:
			mWantsReload = false;
			if (mReloadStateTime > mDescriptor.TimeForReloadEnd * modifier)
			{
				mReloadState = ReloadState.Ready;
			}
			break;
		}
	}

	private bool TryToFire(BaseCharacter owner, Vector3 position, Vector3 target)
	{
		if (mWeaponEquip.IsPuttingAway() || mWeaponEquip.IsTakingOut() || IsReloading())
		{
			return false;
		}
		if (mBulletGapCooldown > 0f)
		{
			return false;
		}
		mAmmo.Take(1, owner.IsFirstPerson);
		mBulletGapCooldown = mDescriptor.TimeBetweenShots;
		mCasingToEject = true;
		if (owner != null)
		{
			if (owner.IsFirstPerson || m_AudioFilter == null)
			{
				WeaponSFX.Instance.KSGFire.Play(owner.gameObject);
			}
			else
			{
				WeaponSFX.Instance.KSGFire.Play(owner.gameObject, m_AudioFilter);
			}
		}
		SurfaceImpact impact = ProjectileManager.Instance.StartProjectile((!(owner != null)) ? null : owner.myActor, position, target, CalculateDamage, CalculateImpactForce, owner != null && !owner.IsFirstPerson);
		WeaponUtils.FeelTheWind(owner, impact);
		if (owner.IsFirstPerson)
		{
			Vector3 origin = ViewModelRig.Instance().Fire(GetId(), ViewModelRig.FireEffects.MuzzleFlash | ViewModelRig.FireEffects.Lighting);
			WeaponUtils.TriggerProjectileEffects(owner, origin, impact, false);
		}
		else
		{
			WeaponUtils.CreateMuzzleFlash(mMuzzleLocator, EffectsController.Instance.Effects.KSGMuzzleFlash);
			WeaponUtils.TriggerProjectileEffects(owner, (!(mMuzzleLocator != null)) ? position : mMuzzleLocator.position, impact, false);
		}
		WeaponUtils.ApplyRecoil(owner, mDescriptor);
		return true;
	}

	public ReloadState GetReloadState()
	{
		return mReloadState;
	}

	public float GetReloadStateAmount(float modifier)
	{
		switch (mReloadState)
		{
		case ReloadState.Start:
			return Mathf.Clamp01(mReloadStateTime / (mDescriptor.TimeForReloadStart * modifier));
		case ReloadState.Loop:
			return Mathf.Clamp01(mReloadStateTime / (mDescriptor.TimeForReloadSingleShell * modifier));
		case ReloadState.End:
			return Mathf.Clamp01(mReloadStateTime / (mDescriptor.TimeForReloadEnd * modifier));
		default:
			return 0f;
		}
	}

	public bool IsRecoiling()
	{
		float num = mBulletGapCooldown;
		return num > 0f;
	}

	public float GetRecoilingAmount()
	{
		float num = mBulletGapCooldown;
		return 1f - Mathf.Clamp01(num / mDescriptor.TimeBetweenShots);
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

	public float GetReloadModifier()
	{
		if (GameSettings.Instance.PerksEnabled)
		{
			return mReloadModifier;
		}
		return 1f;
	}

	public float PlayReloadSfxFP(float oldTimeInAnim, float newTimeInAnim, float[,] eventTimes, ref int whichEvent)
	{
		int eventToPlay = -1;
		bool flag = GetReloadState() == ReloadState.End;
		if (GetReloadState() == ReloadState.Loop || flag || IsRecoiling())
		{
			newTimeInAnim = ((!IsRecoiling()) ? mAmmo.CalcReloadSfx(oldTimeInAnim, newTimeInAnim, !flag, eventTimes, ref whichEvent, ref eventToPlay) : mAmmo.CalcReloadSfx(oldTimeInAnim, newTimeInAnim, false, eventTimes, ref whichEvent, ref eventToPlay));
			if (eventToPlay != -1 && GameController.Instance.mFirstPersonActor != null)
			{
				GameObject gameObject = GameController.Instance.mFirstPersonActor.baseCharacter.gameObject;
				if (IsRecoiling())
				{
					switch (eventToPlay)
					{
					case 0:
						WeaponSFX.Instance.KSGPumpBack.Play(gameObject);
						break;
					case 1:
						WeaponSFX.Instance.KSGPumpForward.Play(gameObject);
						break;
					}
				}
				else if (!flag)
				{
					if (eventToPlay == 0)
					{
						WeaponSFX.Instance.KSGReload.Play(gameObject);
					}
				}
				else
				{
					switch (eventToPlay)
					{
					case 0:
						WeaponSFX.Instance.KSGPumpBack.Play(gameObject);
						break;
					case 1:
						WeaponSFX.Instance.KSGPumpForward.Play(gameObject);
						break;
					}
				}
			}
		}
		return newTimeInAnim;
	}

	public float GetReloadDuration()
	{
		float num = 1f;
		if (GameSettings.Instance.PerksEnabled)
		{
			num = mReloadModifier;
		}
		return (mDescriptor.TimeForReloadStart + mDescriptor.TimeForReloadSingleShell * (mDescriptor.Capacity - (float)mAmmo.Loaded) + mDescriptor.TimeForReloadEnd) * num;
	}
}
