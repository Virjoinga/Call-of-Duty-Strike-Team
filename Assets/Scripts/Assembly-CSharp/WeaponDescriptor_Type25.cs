using UnityEngine;

public class WeaponDescriptor_Type25 : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Type25(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
