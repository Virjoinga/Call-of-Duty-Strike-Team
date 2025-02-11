using UnityEngine;

public class WeaponDescriptor_M27 : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Null();
	}
}
