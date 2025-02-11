using System;
using UnityEngine;

internal class Weapon_Grenade : IWeapon
{
	private float mTime;

	private float mCookedTime;

	private float mTimeToRaiseArm;

	private float mTimeToLowerArm;

	private float mTimeToReleaseGrenade;

	private bool mShouldRelease;

	private bool mGrenadeReleased;

	public Weapon_Grenade()
	{
		mTimeToRaiseArm = 0.5f;
		mTimeToLowerArm = 0.5f;
		mTimeToReleaseGrenade = 0.7f;
	}

	public object QueryInterface(Type t)
	{
		return this;
	}

	public void Reset()
	{
	}

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
		if (mTime < mTimeToRaiseArm)
		{
			mTime += deltaTime;
		}
		else if (mShouldRelease)
		{
			mTime += deltaTime;
		}
		else
		{
			mCookedTime += deltaTime;
		}
		if (mTime > mTimeToReleaseGrenade && mShouldRelease && !mGrenadeReleased)
		{
			GameController.Instance.ReleaseFirstPersonGrenade(mCookedTime, owner.myActor);
			mGrenadeReleased = true;
		}
		if (EndingThrowFraction() == 1f)
		{
			owner.myActor.weapon.SwitchToPrevious();
		}
	}

	public void Release()
	{
		mShouldRelease = true;
	}

	public bool StartingThrow()
	{
		return mTime < mTimeToRaiseArm || !mShouldRelease;
	}

	public bool EndingThrow()
	{
		return !StartingThrow();
	}

	public float StartingThrowFraction()
	{
		return Mathf.Clamp01(mTime / mTimeToRaiseArm);
	}

	public float EndingThrowFraction()
	{
		return Mathf.Clamp01((mTime - mTimeToRaiseArm) / mTimeToLowerArm);
	}

	public void DepressTrigger()
	{
	}

	public void ReleaseTrigger()
	{
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

	public void Drop()
	{
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
		return 0f;
	}

	public string GetAmmoString()
	{
		return string.Empty;
	}

	public WeaponAmmo GetWeaponAmmo()
	{
		return null;
	}

	public float GetFirstPersonAccuracy()
	{
		return 1f;
	}

	public float GetThirdPersonAccuracy()
	{
		return 1f;
	}

	public float GetRunSpeed(bool playerControlled, bool isFirstPerson)
	{
		return 100f;
	}

	public float GetDesiredFieldOfView()
	{
		return WeaponUtils.CalculateStandardFieldOfView(0f, 0f);
	}

	public float GetCrosshairOpacity()
	{
		return 0f;
	}

	public string GetId()
	{
		return "Frag Grenade";
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

	public float GetReloadDuration()
	{
		return 1f;
	}
}
