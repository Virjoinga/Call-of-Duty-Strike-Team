using UnityEngine;

public class WeaponDescriptor_M1216 : WeaponDescriptor
{
	public float TimeBetweenShots;

	public float TimeToRechamber;

	public float ShotSpread;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_M1216(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
