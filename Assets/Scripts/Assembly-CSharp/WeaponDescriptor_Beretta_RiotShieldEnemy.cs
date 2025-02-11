using UnityEngine;

public class WeaponDescriptor_Beretta_RiotShieldEnemy : WeaponDescriptor
{
	public override IWeapon Create(GameObject model, float adsModifier, float ammoModifier, float reloadModifier)
	{
		return new Weapon_Beretta_RiotShieldEnemy(this, model);
	}
}
