using UnityEngine;

public class ViewModelBlendTree_Knife : IViewModelWeaponBlendTree
{
	private AnimationState mStab;

	public ViewModelBlendTree_Knife(GameObject weapon, WeaponCoreAnims coreAnims, KnifeAnims specificAnims)
	{
		mStab = weapon.animation.AddClipSafe(specificAnims.Stab, "Stab");
		mStab.wrapMode = WrapMode.ClampForever;
		mStab.speed = 0f;
		mStab.weight = 1f;
	}

	public void Update(IWeapon weaponState)
	{
		Weapon_Knife weapon_Knife = weaponState as Weapon_Knife;
		mStab.enabled = true;
		mStab.normalizedTime = weapon_Knife.StabFraction();
	}
}
