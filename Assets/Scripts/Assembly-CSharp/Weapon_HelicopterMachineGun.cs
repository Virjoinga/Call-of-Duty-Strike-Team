using System;
using UnityEngine;

public class Weapon_HelicopterMachineGun : IWeapon, IWeaponADS, IWeaponAI, IWeaponStats
{
	private Helicopter mOwner;

	private bool mTrigger;

	private float mSpinUpAmount;

	private static float TIME_BETWEEN_BULLETS = 0.05f;

	private float mTimeToNextBullet = TIME_BETWEEN_BULLETS;

	private SoundManager.SoundInstance mFiringSfxLoopInst;

	public Weapon_HelicopterMachineGun(Helicopter owner)
	{
		mOwner = owner;
		WeaponUtils.CreateMuzzleFlash(mOwner.GunNozzle, EffectsController.Instance.Effects.SentryGunMuzzleFlash);
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
		StopFiringSfxLoop();
	}

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
		mSpinUpAmount = Mathf.Clamp01(mSpinUpAmount + ((!mTrigger) ? (0f - deltaTime) : deltaTime));
		if (mSpinUpAmount >= 1f)
		{
			DoFiringSfxLoop();
			if (mTimeToNextBullet <= 0f)
			{
				int bulletsInSpread = mOwner.BulletsInSpread;
				for (int i = 0; i < bulletsInSpread; i++)
				{
					SurfaceImpact impact = ProjectileManager.Instance.StartProjectile(null, mOwner.GunNozzle.position, mOwner.GunTarget + UnityEngine.Random.insideUnitSphere * 0.5f, CalculateDamage, CalculateImpactForce, true);
					WeaponUtils.TriggerProjectileEffects(owner, mOwner.GunNozzle.position, impact, true);
				}
				WeaponUtils.CreateMuzzleFlash(mOwner.GunNozzle, EffectsController.Instance.Effects.SentryGunMuzzleFlash);
				mTimeToNextBullet = TIME_BETWEEN_BULLETS;
			}
			else
			{
				mTimeToNextBullet -= deltaTime;
			}
		}
		else
		{
			StopFiringSfxLoop();
		}
	}

	public void StopFiringSfxLoop()
	{
		if (mFiringSfxLoopInst != null)
		{
			mFiringSfxLoopInst.Stop();
			mFiringSfxLoopInst = null;
		}
	}

	public void DoFiringSfxLoop()
	{
		if (mFiringSfxLoopInst == null && mOwner != null)
		{
			mFiringSfxLoopInst = SoundManager.Instance.Play(WeaponSFX.Instance.HeliMachineGunFireLoop, mOwner.gameObject);
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

	public void PutAway()
	{
	}

	public void TakeOut()
	{
	}

	public void SwitchToHips()
	{
	}

	public void SwitchToSights()
	{
	}

	public void Drop()
	{
	}

	public ADSState GetADSState()
	{
		return ADSState.Hips;
	}

	public float GetEquipedBlendAmount()
	{
		return 1f;
	}

	public float GetHipsToSightsBlendAmount()
	{
		return 0f;
	}

	public float GetLeftRightLeanAmount()
	{
		return 0f;
	}

	public float GetUpDownLeanAmount()
	{
		return 0f;
	}

	public float GetMovementAmount()
	{
		return 0f;
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
		return 0.1f;
	}

	public float GetThirdPersonAccuracy()
	{
		return 0.1f;
	}

	public float GetRunSpeed(bool playerControlled, bool isFirstPerson)
	{
		return 0f;
	}

	public float GetDesiredFieldOfView()
	{
		return InputSettings.FirstPersonFieldOfView;
	}

	public float GetCrosshairOpacity()
	{
		return 0f;
	}

	public string GetId()
	{
		return "HelicopterMachineGun";
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
		return 0f;
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
		return false;
	}

	public bool CanReload()
	{
		return false;
	}

	public bool IsReloading()
	{
		return false;
	}

	public bool IsPuttingAway()
	{
		return false;
	}

	public bool IsTakingOut()
	{
		return false;
	}

	public bool IsLongRangeShot(float distSquared)
	{
		return false;
	}

	public bool IsHeadShotAllowed(float distSquared)
	{
		return false;
	}

	public float CalculateDamage(float distance, HitLocation target, bool isPlayer)
	{
		return mOwner.MachineGunDamage;
	}

	public float CalculateImpactForce()
	{
		return 140f;
	}

	public float CalculateUtility(float distance)
	{
		return 1f;
	}

	public bool PointingAtTarget(Vector3 pos, float radius)
	{
		return WorldHelper.PointingAtTarget(mOwner.GunNozzle.transform, pos, radius);
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
}
