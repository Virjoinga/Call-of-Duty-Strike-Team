using System;
using UnityEngine;

public class WeaponDescriptor : ScriptableObject
{
	public enum WeaponClass
	{
		Pistol = 0,
		Shotgun = 1,
		SniperRifle = 2,
		LightMachineGun = 3,
		SubMachineGun = 4,
		AssaultRifle = 5,
		Special = 6,
		None = 7
	}

	[Serializable]
	public class LoadoutData
	{
		public float Accuracy;

		public float Damage;

		public float RateOfFire;

		public float Mobility;

		public float Range;
	}

	[Serializable]
	public class DamageRange
	{
		public float Damage;

		public float Range;
	}

	[Serializable]
	public class UtilityRange
	{
		public float Utility;

		public float Range;
	}

	public string Name;

	public LoadoutData Loadout;

	public int UnlockLevel;

	public float Capacity;

	public int BulletStartQuantity;

	public bool UnlimitedAmmo;

	public int LowAmmoPercentage = 50;

	public float ReloadTime;

	public float ValidReloadThreshold;

	public float PutAwayTime;

	public float TakeOutTime;

	public DamageRange[] DamageRanges;

	public UtilityRange[] UtilityRanges;

	public float ImpactForce;

	public float FirstPersonAccuracy = 3f;

	public float ThirdPersonAccuracy = 3f;

	public float FirstPersonRecoil;

	public float ThirdPersonRecoil;

	public float Range;

	public float LongRange = 0.5f;

	public float HeadShotMaxRange = 100f;

	public float RunSpeed;

	public float ADSRunSpeedModifier;

	public float AimDownSightsFOV;

	public Vector2 SemiAutoFireRates = new Vector2(1f, 2f);

	public float SuppressionRadius = 3f;

	public WeaponIconController.WeaponType Type;

	public GameObject ModelHiResPrefab;

	public string ModelHiResName;

	public string LongDescription;

	public string ShortDescription;

	public int HardCost;

	public int SoftCost;

	public int UnlockCost;

	public Quaternion inHandLocalRotation;

	public Vector3 inHandLocalPosition;

	public HitBoxDescriptor HitBoxRig;

	public AudioFilter ThirdPersonAudioFilter;

	public virtual IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Legacy(this, model);
	}
}
