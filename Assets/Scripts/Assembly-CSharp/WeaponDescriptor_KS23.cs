using UnityEngine;

public class WeaponDescriptor_KS23 : WeaponDescriptor
{
	public float TimeBetweenShots;

	public float TimeForReloadStart;

	public float TimeForReloadSingleShell;

	public float TimeForReloadEnd;

	public float TimeToRechamberHips;

	public float TimeToRechamberSights;

	public float ShotSpread;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_KS23(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
