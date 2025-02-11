using UnityEngine;

public class BundleDescriptor : ScriptableObject
{
	public string Name;

	public int HardCost;

	public WeaponDescriptor Weapon;

	public PerkType Perk;

	public EquipmentIconController.EquipmentType EquipmentIcon;

	public EquipmentDescriptor[] Equipment;

	public int NumItemsPerEquipment;

	public bool CompletePackage;
}
