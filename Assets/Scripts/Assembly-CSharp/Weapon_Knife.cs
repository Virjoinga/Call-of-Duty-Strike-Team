using System;
using UnityEngine;

internal class Weapon_Knife : IWeapon
{
	private float mTime;

	private float mTimeToStab;

	private bool mStarted;

	private bool mHit;

	public static float KnifeRange
	{
		get
		{
			return 2f;
		}
	}

	public Weapon_Knife()
	{
		Reset();
	}

	public object QueryInterface(Type t)
	{
		return this;
	}

	public void Reset()
	{
		mTime = 0f;
		mTimeToStab = 0.333f;
		mStarted = false;
		mHit = false;
	}

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
		mTime += deltaTime;
		if (owner.IsFirstPerson)
		{
			if (!mStarted)
			{
				WeaponSFX.Instance.MeleeSwing.Play2D();
				mStarted = true;
			}
			if (!mHit)
			{
				SurfaceImpact surfaceImpact = Trace(owner);
				if (surfaceImpact != null)
				{
					surfaceImpact.noDecal = true;
					EffectsController.Instance.TriggerSurfaceImpact(surfaceImpact);
					WeaponSFX.Instance.MeleeHit.Play2D();
					mHit = true;
				}
			}
		}
		if (StabFraction() == 1f)
		{
			owner.myActor.weapon.SwitchToPrevious();
		}
	}

	public float StabFraction()
	{
		return Mathf.Clamp01(mTime / mTimeToStab);
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
		return "Knife";
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

	private SurfaceImpact Trace(BaseCharacter owner)
	{
		Transform transform = owner.FirstPersonCamera.transform;
		Vector3 position = transform.position;
		Vector3 target = position + transform.forward;
		Actor owner2 = ((!(owner != null)) ? null : owner.myActor);
		return ProjectileManager.Instance.KnifeAttack(owner2, position, target, KnifeRange, 1000f);
	}
}
