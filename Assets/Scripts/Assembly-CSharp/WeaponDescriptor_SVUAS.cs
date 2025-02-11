using UnityEngine;

public class WeaponDescriptor_SVUAS : WeaponDescriptor
{
	public float TimeBetweenShots;

	public float ScopeFieldOfView;

	public float SwayMinimum;

	public float SwayMaximum;

	public float SwayFrequency;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_SVUAS(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
