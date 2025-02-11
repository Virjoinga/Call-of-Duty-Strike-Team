using System;
using UnityEngine;

public class Weapon_Minigun : IWeapon, IWeaponADS, IWeaponAI, IWeaponStats
{
	private IWeaponModel mModel;

	private MinigunDescriptor mDescriptor;

	private bool mTrigger;

	private float mSpinUpAmount;

	private float mHeat;

	private bool mJammed;

	private float mShotTimeAllowance;

	private WeaponADS mWeaponADS;

	private CasingEffect mCasingEffect;

	private bool mWarningTriggered;

	private SoundManager.SoundInstance mFireLoopSoundInst;

	private bool mWasFiringLastUpdate;

	private SoundManager.SoundInstance mOverHeatSFXInst;

	public bool UseFiringSounds { get; set; }

	public Weapon_Minigun(IWeaponModel model, MinigunDescriptor descriptor)
	{
		mModel = model;
		mDescriptor = descriptor;
		mWeaponADS = new WeaponADS(0.2f, 1f);
		if (EffectsController.Instance != null)
		{
			WeaponUtils.CreateMuzzleFlash(mModel.GetMuzzleLocator(), EffectsController.Instance.Effects.SentryGunMuzzleFlash);
			WeaponUtils.CreateMuzzleFlash(mModel.GetMuzzleLocator(), EffectsController.Instance.Effects.SentryGunMuzzleFlash);
			WeaponUtils.CreateMuzzleFlash(mModel.GetMuzzleLocator(), EffectsController.Instance.Effects.SentryGunMuzzleFlash);
			mCasingEffect = WeaponUtils.CreateCasingEffect(mModel.GetCasingsLocator(), EffectsController.Instance.Effects.SentryGunCasingEject);
		}
		UseFiringSounds = true;
		mWasFiringLastUpdate = false;
		Reset();
	}

	public object QueryInterface(Type t)
	{
		return this;
	}

	public void Reset()
	{
		mTrigger = false;
		mSpinUpAmount = 0f;
		mWeaponADS.Reset();
		CleanUpSound();
	}

	public void CleanUpSound()
	{
		StopFiringLoop(null);
		StopOverHeatWarning();
		mWasFiringLastUpdate = false;
	}

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
		bool flag = mTrigger && !mJammed;
		mSpinUpAmount = Mathf.Clamp01(mSpinUpAmount + ((!flag) ? ((0f - deltaTime) * mDescriptor.SpinDownAcceleration) : (deltaTime * mDescriptor.SpinUpAcceleration)));
		if (mSpinUpAmount >= 1f && owner != null)
		{
			mShotTimeAllowance += deltaTime;
			float num = Mathf.Max(0.01f, mDescriptor.TimeBetweenShots);
			bool flag2 = false;
			while (mShotTimeAllowance > num)
			{
				flag2 = true;
				SurfaceImpact impact = ProjectileManager.Instance.StartProjectile(owner.myActor, mModel.GetMuzzleLocator().position, mModel.GetMuzzleLocator().position + WeaponUtils.GetFirstPersonShotDirection(mModel.GetMuzzleLocator().forward, mDescriptor.Spread), CalculateDamage, CalculateImpactForce, !(owner != null) || !owner.IsFirstPerson);
				WeaponUtils.TriggerProjectileEffects(owner, mModel.GetMuzzleLocator().position, impact, EffectsController.TracerType.Minigun);
				mShotTimeAllowance -= num;
			}
			if (flag2)
			{
				WeaponUtils.CreateMuzzleFlash(mModel.GetMuzzleLocator(), EffectsController.Instance.Effects.SentryGunMuzzleFlash);
				if (!mDescriptor.NoBulletCasings)
				{
					mCasingEffect.Fire(false);
				}
			}
			DoFiringLoop(owner);
			mWasFiringLastUpdate = true;
			mHeat += deltaTime * mDescriptor.HeatUpSpeed;
		}
		else
		{
			mHeat -= deltaTime * mDescriptor.CoolDownSpeed;
			StopFiringLoop(owner);
			mWasFiringLastUpdate = false;
		}
		if (mHeat > 1f)
		{
			mJammed = true;
			mHeat = 1f;
		}
		else if (mHeat < 0f)
		{
			mJammed = false;
			mHeat = 0f;
		}
		if (mHeat > 0.75f && !mWarningTriggered)
		{
			mWarningTriggered = true;
			if (owner != null && mOverHeatSFXInst == null)
			{
				mOverHeatSFXInst = OverHeatWarningSFX.Instance.OverheatWarning.Play(owner.gameObject);
			}
		}
		else if (mHeat < 0.75f && mWarningTriggered)
		{
			mWarningTriggered = false;
			StopOverHeatWarning();
		}
		if (owner != null && owner.myActor.behaviour.PlayerControlled)
		{
			mWeaponADS.SetModifier(StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.QuickDraw));
		}
		if (owner != null)
		{
			mWeaponADS.Update(deltaTime, owner.IsADSSuppressed);
		}
	}

	public void StopOverHeatWarning()
	{
		mWarningTriggered = false;
		if (mOverHeatSFXInst != null)
		{
			mOverHeatSFXInst.Stop();
			mOverHeatSFXInst = null;
		}
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
	}

	public void ReloadImmediately()
	{
	}

	public void CancelReload()
	{
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
		return true;
	}

	public bool LowAmmo()
	{
		return false;
	}

	public float GetPercentageAmmo()
	{
		return 1f;
	}

	public string GetAmmoString()
	{
		return string.Empty;
	}

	public float GetFirstPersonAccuracy()
	{
		return 2f;
	}

	public float GetThirdPersonAccuracy()
	{
		return 1f;
	}

	public float GetRunSpeed(bool playerControlled, bool isFirstPerson)
	{
		return 0f;
	}

	public float GetDesiredFieldOfView()
	{
		float adsFOV = 30f;
		return WeaponUtils.CalculateStandardFieldOfView(mWeaponADS.BlendAmount, adsFOV);
	}

	public float GetCrosshairOpacity()
	{
		return 1f;
	}

	public string GetId()
	{
		return "Minigun";
	}

	public WeaponDescriptor.WeaponClass GetClass()
	{
		return WeaponDescriptor.WeaponClass.Special;
	}

	public int GetWeaponType()
	{
		return 0;
	}

	public float AccuracyStatAdjustment()
	{
		return mDescriptor.AccuracyStatAdjustment;
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
		return mSpinUpAmount >= 1f;
	}

	public bool CanReload()
	{
		return false;
	}

	public bool IsReloading()
	{
		return false;
	}

	public bool IsLongRangeShot(float distSquared)
	{
		return false;
	}

	public bool IsHeadShotAllowed(float distSquared)
	{
		return true;
	}

	public float GetSpinUpAmount()
	{
		return mSpinUpAmount;
	}

	public float GetHeatLevel()
	{
		return mHeat;
	}

	public bool GetJammed()
	{
		return mJammed;
	}

	public float CalculateDamage(float distance, HitLocation target, bool isPlayer)
	{
		WeaponDescriptor.DamageRange[] ranges = ((!isPlayer) ? mDescriptor.DamageRangesNPC : mDescriptor.DamageRanges);
		return WeaponUtils.CalculateStandardDamage(distance, ranges, GetClass(), isPlayer);
	}

	public float CalculateImpactForce()
	{
		return mDescriptor.ImpactForce;
	}

	public float CalculateUtility(float distance)
	{
		return 1f;
	}

	public bool PointingAtTarget(Vector3 pos, float radius)
	{
		return WorldHelper.PointingAtTarget(mModel.GetMuzzleLocator(), pos, radius);
	}

	public float GetSuppressionRadius()
	{
		return 0f;
	}

	public WeaponAmmo GetWeaponAmmo()
	{
		return null;
	}

	public float GetReloadDuration()
	{
		return 1f;
	}

	private void DoFiringLoop(BaseCharacter owner)
	{
		if (UseFiringSounds && mFireLoopSoundInst == null && owner != null)
		{
			mFireLoopSoundInst = WeaponSFX.Instance.SentryLoop.Play(owner.gameObject);
		}
	}

	private void StopFiringLoop(BaseCharacter owner)
	{
		if (UseFiringSounds)
		{
			if (mFireLoopSoundInst != null)
			{
				mFireLoopSoundInst.Stop();
				mFireLoopSoundInst = null;
			}
			if (mWasFiringLastUpdate && owner != null)
			{
				WeaponSFX.Instance.SentryStop.Play(owner.gameObject);
			}
		}
	}
}
