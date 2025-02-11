using UnityEngine;

public class WeaponDescriptor_Vektor : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Vektor(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
