using UnityEngine;

public class ViewModelBlendTree_Grenade : IViewModelWeaponBlendTree
{
	private AnimationState mStart;

	private AnimationState mEnd;

	public ViewModelBlendTree_Grenade(GameObject weapon, WeaponCoreAnims coreAnims, GrenadeAnims specificAnims)
	{
		mStart = weapon.GetComponent<Animation>().AddClipSafe(specificAnims.Start, "Start");
		mStart.wrapMode = WrapMode.ClampForever;
		mStart.speed = 0f;
		mStart.weight = 1f;
		mEnd = weapon.GetComponent<Animation>().AddClipSafe(specificAnims.End, "End");
		mEnd.wrapMode = WrapMode.ClampForever;
		mEnd.speed = 0f;
		mEnd.weight = 1f;
	}

	public void Update(IWeapon weaponState)
	{
		Weapon_Grenade weapon_Grenade = weaponState as Weapon_Grenade;
		mStart.enabled = weapon_Grenade.StartingThrow();
		mStart.normalizedTime = weapon_Grenade.StartingThrowFraction();
		mEnd.enabled = weapon_Grenade.EndingThrow();
		mEnd.normalizedTime = weapon_Grenade.EndingThrowFraction();
	}
}
