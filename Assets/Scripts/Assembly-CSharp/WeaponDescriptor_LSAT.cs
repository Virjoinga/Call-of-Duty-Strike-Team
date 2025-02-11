using UnityEngine;

public class WeaponDescriptor_LSAT : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_LSAT(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
