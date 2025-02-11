using UnityEngine;

public class ViewModelBlendTree_HAMR : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mReloadEmpty;

	private AnimationState mReloadTactical;

	public ViewModelBlendTree_HAMR(GameObject weapon, WeaponCoreAnims coreAnims, HAMRAnims specificAnims)
	{
		mHipsFire = weapon.animation.AddClipSafe(specificAnims.HipsFire, "HipsFire");
		mSightsFire = weapon.animation.AddClipSafe(specificAnims.SightsFire, "SightsFire");
		mReloadEmpty = weapon.animation.AddClipSafe(specificAnims.ReloadEmpty, "ReloadEmpty");
		mReloadTactical = weapon.animation.AddClipSafe(specificAnims.ReloadTactical, "ReloadTactical");
		mReloadEmpty.wrapMode = WrapMode.Once;
		ViewModelRig.SetupEventData(out mReloadEventTimes, specificAnims.ReloadEmptyEvents, specificAnims.ReloadTacticalEvents);
	}

	public void Update(IWeapon weaponState)
	{
		Weapon_HAMR weapon_HAMR = weaponState as Weapon_HAMR;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReloadEmpty.enabled = false;
		mReloadTactical.enabled = false;
		if (!weapon_HAMR.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (weapon_HAMR.IsReloading())
		{
			AnimationState animationState = ((!weapon_HAMR.IsTacticalReload()) ? mReloadEmpty : mReloadTactical);
			animationState.enabled = true;
			animationState.layer = 2;
			animationState.weight = 1f;
			animationState.normalizedTime = weapon_HAMR.GetReloadingAmount();
			mOldTimeInReloadAnim = weapon_HAMR.PlayReloadSfxFP(mOldTimeInReloadAnim, animationState.normalizedTime * animationState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_HAMR.IsRecoiling())
		{
			float recoilingAmount = weapon_HAMR.GetRecoilingAmount();
			float hipsToSightsBlendAmount = weapon_HAMR.GetHipsToSightsBlendAmount();
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
