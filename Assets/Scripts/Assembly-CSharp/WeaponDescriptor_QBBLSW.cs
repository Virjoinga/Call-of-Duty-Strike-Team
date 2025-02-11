using UnityEngine;

public class WeaponDescriptor_QBBLSW : WeaponDescriptor
{
	public float TimeBetweenShots;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_QBBLSW(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
