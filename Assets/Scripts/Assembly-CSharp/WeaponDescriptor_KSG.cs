using UnityEngine;

public class WeaponDescriptor_KSG : WeaponDescriptor
{
	public float TimeBetweenShots;

	public float TimeForReloadStart;

	public float TimeForReloadSingleShell;

	public float TimeForReloadEnd;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_KSG(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
