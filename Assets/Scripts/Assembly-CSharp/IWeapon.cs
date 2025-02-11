using System;

public interface IWeapon
{
	object QueryInterface(Type t);

	void Reset();

	void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs);

	void DepressTrigger();

	void ReleaseTrigger();

	void StartReloading(BaseCharacter owner);

	void ReloadImmediately();

	void Drop();

	bool HasAmmo();

	bool LowAmmo();

	float GetPercentageAmmo();

	string GetAmmoString();

	WeaponAmmo GetWeaponAmmo();

	float GetFirstPersonAccuracy();

	float GetThirdPersonAccuracy();

	float GetRunSpeed(bool playerControlled, bool isFirstPerson);

	float GetDesiredFieldOfView();

	float GetCrosshairOpacity();

	string GetId();

	WeaponDescriptor.WeaponClass GetClass();

	int GetWeaponType();

	bool HasScope();

	bool IsSilenced();

	bool IsFiring();

	bool CanReload();

	bool IsReloading();

	float GetReloadDuration();

	void CancelReload();
}
