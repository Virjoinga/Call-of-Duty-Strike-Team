using UnityEngine;

public class WeaponDescriptor_HAMR : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_HAMR(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
