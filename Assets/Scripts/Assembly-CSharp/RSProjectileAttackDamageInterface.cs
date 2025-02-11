using System;

[Serializable]
public class RSProjectileAttackDamageInterface
{
	private bool mIsCritical;

	private bool mIsExplosion;

	private bool mIsFlanking;

	private float mWeaponDamageMultiplier;

	public bool IsCritical
	{
		get
		{
			return mIsCritical;
		}
	}

	public bool IsExplosion
	{
		get
		{
			return mIsExplosion;
		}
	}

	public bool IsFlanking
	{
		get
		{
			return mIsFlanking;
		}
	}

	public float WeaponDamageMultiplier
	{
		get
		{
			return mWeaponDamageMultiplier;
		}
	}

	public void Initialise(bool isExplosion, bool isCritical, bool isFlanking, float weaponDamageMultiplier)
	{
		mIsExplosion = isExplosion;
		mIsCritical = isCritical;
		mIsFlanking = isFlanking;
		mWeaponDamageMultiplier = weaponDamageMultiplier;
	}
}
