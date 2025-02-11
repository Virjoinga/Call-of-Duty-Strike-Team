using UnityEngine;

public class WeaponDescriptor_XPR50 : WeaponDescriptor
{
	public float TimeBetweenShots;

	public float ScopeFieldOfView;

	public float SwayMinimum;

	public float SwayMaximum;

	public float SwayFrequency;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_XPR50(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
