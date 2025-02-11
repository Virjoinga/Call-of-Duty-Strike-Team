using UnityEngine;

public class WeaponDescriptor_PDW : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_PDW(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
