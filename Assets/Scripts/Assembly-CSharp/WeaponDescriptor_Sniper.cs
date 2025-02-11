using UnityEngine;

public class WeaponDescriptor_Sniper : WeaponDescriptor
{
	public float RecoilTime;

	public float RechamberTime;

	public float DamagePercentage = 100f;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Sniper(this, model);
	}
}
