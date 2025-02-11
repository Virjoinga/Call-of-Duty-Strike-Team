using UnityEngine;

public class WeaponDescriptor_Skorpion : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Skorpion(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
