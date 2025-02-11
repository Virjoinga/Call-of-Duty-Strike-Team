using System;
using UnityEngine;

public class Weapon_Beretta_RiotShieldEnemy : IWeapon, IWeaponADS, IWeaponAI, IWeaponEquip, IWeaponStats
{
	private WeaponDescriptor_Beretta_RiotShieldEnemy mDescriptor;

	private GameObject mWeaponModel;

	private bool mTrigger;

	public Actor Target { private get; set; }

	public Weapon_Beretta_RiotShieldEnemy(WeaponDescriptor_Beretta_RiotShieldEnemy descriptor, GameObject model)
	{
		mDescriptor = descriptor;
		mWeaponModel = WeaponUtils.CreateThirdPersonModel(model, mDescriptor);
		Reset();
	}

	public object QueryInterface(Type t)
	{
		return this;
	}

	public void Reset()
	{
		mTrigger = false;
	}

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
		if (mTrigger)
		{
			TBFAssert.DoAssert(Target != null, "Weapon_Beretta_RiotShieldEnemy should not have it's trigger pulled without a target set");
			RealCharacter realCharacter = owner as RealCharacter;
			float num = 1f;
			Vector3 normalized = (Target.realCharacter.GetBulletOrigin() - realCharacter.GetBulletOrigin()).normalized;
			Vector3 origin = realCharacter.GetBulletOrigin() + num * normalized;
			origin += realCharacter.transform.right * 0.5f;
			Vector3 target = Target.GetPosition() + Vector3.up;
			WeaponSFX.Instance.BerettaBurst.Play(owner.gameObject);
			SurfaceImpact impact = ProjectileManager.Instance.StartProjectile(owner.myActor, origin, target, CalculateDamage, CalculateImpactForce, !owner.IsFirstPerson);
			WeaponUtils.TriggerProjectileEffects(owner, origin, impact, true);
			ReleaseTrigger();
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

	public void PutAway(float speedModifier)
	{
		mWeaponModel.SetActive(false);
	}

	public void TakeOut(float speedModifier)
	{
		mWeaponModel.SetActive(true);
	}

	public void SwitchToHips()
	{
	}

	public void SwitchToSights()
	{
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
		return 1f;
	}

	public float GetThirdPersonAccuracy()
	{
		return 1f;
	}

	public float GetRunSpeed(bool playerControlled, bool isFirstPerson)
	{
		return mDescriptor.RunSpeed;
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
		return "Beretta_RiotShieldEnemy";
	}

	public WeaponDescriptor.WeaponClass GetClass()
	{
		return WeaponDescriptor.WeaponClass.Special;
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

	public bool HasNoWeapon()
	{
		return false;
	}

	public void HasNoWeapon(bool noWeapon)
	{
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
		return 500f;
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
		return true;
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
