using UnityEngine;

public class WeaponDescriptor_Ballista : WeaponDescriptor
{
	public float RecoilTime;

	public float RechamberTime;

	public float ScopeFieldOfView;

	public float SwayMinimum;

	public float SwayMaximum;

	public float SwayFrequency;

	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Ballista(this, model, adsModifier, ammoModifier, reloadModifier);
	}
}
