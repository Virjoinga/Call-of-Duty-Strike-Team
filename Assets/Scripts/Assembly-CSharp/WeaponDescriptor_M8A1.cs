using UnityEngine;

public class WeaponDescriptor_M8A1 : WeaponDescriptor
{
	public float TimeBetweenBurst;

	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_M8A1(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
