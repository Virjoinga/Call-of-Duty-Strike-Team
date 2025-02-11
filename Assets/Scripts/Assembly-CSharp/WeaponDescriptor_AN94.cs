using UnityEngine;

public class WeaponDescriptor_AN94 : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_AN94(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
