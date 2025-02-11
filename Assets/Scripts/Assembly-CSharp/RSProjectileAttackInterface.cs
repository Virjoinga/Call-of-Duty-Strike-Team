using System;

[Serializable]
public class RSProjectileAttackInterface
{
	public RSProjectileWeaponInterface WeaponInterface;

	private bool mIsRunning;

	public bool IsRunning
	{
		get
		{
			return mIsRunning;
		}
	}

	public RSProjectileAttackInterface()
	{
		WeaponInterface = new RSProjectileWeaponInterface();
	}

	public void Initialise(float weaponBaseAccuracy, bool isRunning)
	{
		WeaponInterface.BaseAccuracy = weaponBaseAccuracy;
		mIsRunning = isRunning;
	}
}
