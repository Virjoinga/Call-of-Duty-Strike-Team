using System;

internal class Weapon_Claymore : IWeapon
{
	private TaskDropClaymore mTask;

	public Weapon_Claymore(TaskDropClaymore task)
	{
		mTask = task;
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
		if (mTask != null && mTask.HasFinished())
		{
			owner.myActor.weapon.SwitchToPrevious();
		}
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
		return "Claymore";
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
		return 0f;
	}
}
