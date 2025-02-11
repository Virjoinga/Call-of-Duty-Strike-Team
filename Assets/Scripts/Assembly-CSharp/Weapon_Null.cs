using System;
using UnityEngine;

internal class Weapon_Null : IWeapon, IWeaponADS, IWeaponAI, IWeaponMovement, IWeaponEquip, IWeaponStats
{
	public object QueryInterface(Type t)
	{
		return this;
	}

	public void Reset()
	{
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

	public void PutAway(float speedModifier)
	{
	}

	public void TakeOut(float speedModifier)
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
		return 0f;
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
		return false;
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
		return "TODO";
	}

	public float GetFirstPersonAccuracy()
	{
		return 0f;
	}

	public float GetThirdPersonAccuracy()
	{
		return 0f;
	}

	public float GetRunSpeed(bool playerControlled, bool isFirstPerson)
	{
		return 100f;
	}

	public float GetDesiredFieldOfView()
	{
		return 45f;
	}

	public float GetCrosshairOpacity()
	{
		return 0f;
	}

	public float CalculateDamage(float distance, HitLocation target, bool isPlayer)
	{
		return 0f;
	}

	public float CalculateUtility(float distance)
	{
		return 0f;
	}

	public string GetId()
	{
		return "NULL";
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

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
	}

	public bool PointingAtTarget(Vector3 pos, float radius)
	{
		return false;
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
