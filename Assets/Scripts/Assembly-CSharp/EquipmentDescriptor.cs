using UnityEngine;

public class EquipmentDescriptor : ScriptableObject
{
	public enum WeaponClass
	{
		Lethal = 0,
		Tactical = 1
	}

	public enum TriggerType
	{
		Triggered = 0,
		Proximity = 1,
		Fuse = 2,
		Impact = 3
	}

	public enum RadiusType
	{
		Circle = 0,
		Arc = 1
	}

	public enum ArmourType
	{
		None = 0,
		Level1 = 1,
		Level2 = 2,
		Level3 = 3,
		Level4 = 4,
		MaxArmour = 4
	}

	public string Name;

	public float NearDamage;

	public float FarDamage;

	public float EffectAreaMeters;

	public float FuseLengthSeconds;

	public float Protection;

	public float Weight;

	public WeaponClass Class;

	public TriggerType Trigger;

	public RadiusType Radius;

	public ArmourType Armour;

	public int AmmoPackSize;

	public int MaxAmmo;

	public int HardCost;

	public int InGameHardCost;

	public int SoftCost;

	public int UnlockCost;

	public int UnlockLevel;

	public int TotalSlotSize;

	public bool CanOnlyOwnOne;

	public EquipmentIconController.EquipmentType Type;

	public string LongDescription;

	public string ShortDescription;
}
