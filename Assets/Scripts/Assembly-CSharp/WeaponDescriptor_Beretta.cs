using UnityEngine;

public class WeaponDescriptor_Beretta : WeaponDescriptor
{
	public float TimeBetweenBursts;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Beretta(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
