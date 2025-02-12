using UnityEngine;

public class ViewModelBlendTree_Skorpion : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mReloadEmpty;

	private AnimationState mReloadTactical;

	public ViewModelBlendTree_Skorpion(GameObject weapon, WeaponCoreAnims coreAnims, SkorpionAnims specificAnims)
	{
		mHipsFire = weapon.GetComponent<Animation>().AddClipSafe(specificAnims.HipsFire, "HipsFire");
		mSightsFire = weapon.GetComponent<Animation>().AddClipSafe(specificAnims.SightsFire, "SightsFire");
		mReloadEmpty = weapon.GetComponent<Animation>().AddClipSafe(specificAnims.ReloadEmpty, "ReloadEmpty");
		mReloadTactical = weapon.GetComponent<Animation>().AddClipSafe(specificAnims.ReloadTactical, "ReloadTactical");
		mReloadEmpty.wrapMode = WrapMode.Once;
		ViewModelRig.SetupEventData(out mReloadEventTimes, specificAnims.ReloadEmptyEvents, specificAnims.ReloadTacticalEvents);
	}

	public void Update(IWeapon weaponState)
	{
		Weapon_Skorpion weapon_Skorpion = weaponState as Weapon_Skorpion;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReloadEmpty.enabled = false;
		mReloadTactical.enabled = false;
		if (!weapon_Skorpion.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (weapon_Skorpion.IsReloading())
		{
			AnimationState animationState = ((!weapon_Skorpion.IsTacticalReload()) ? mReloadEmpty : mReloadTactical);
			animationState.enabled = true;
			animationState.layer = 2;
			animationState.weight = 1f;
			animationState.normalizedTime = weapon_Skorpion.GetReloadingAmount();
			mOldTimeInReloadAnim = weapon_Skorpion.PlayReloadSfxFP(mOldTimeInReloadAnim, animationState.normalizedTime * animationState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_Skorpion.IsRecoiling())
		{
			float recoilingAmount = weapon_Skorpion.GetRecoilingAmount();
			float hipsToSightsBlendAmount = weapon_Skorpion.GetHipsToSightsBlendAmount();
			mHipsFire.enabled = true;
			mHipsFire.layer = 2;
			mHipsFire.weight = 1f - hipsToSightsBlendAmount;
			mHipsFire.normalizedTime = recoilingAmount;
			mSightsFire.enabled = true;
			mSightsFire.layer = 2;
			mSightsFire.weight = hipsToSightsBlendAmount;
			mSightsFire.normalizedTime = recoilingAmount;
		}
	}
}
