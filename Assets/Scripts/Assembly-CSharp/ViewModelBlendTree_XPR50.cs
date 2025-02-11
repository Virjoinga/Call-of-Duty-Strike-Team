using UnityEngine;

public class ViewModelBlendTree_XPR50 : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mReloadEmpty;

	private AnimationState mReloadTactical;

	public ViewModelBlendTree_XPR50(GameObject weapon, WeaponCoreAnims coreAnims, XPR50Anims specificAnims)
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
		Weapon_XPR50 weapon_XPR = weaponState as Weapon_XPR50;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReloadEmpty.enabled = false;
		mReloadTactical.enabled = false;
		if (!weapon_XPR.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (weapon_XPR.IsReloading())
		{
			AnimationState animationState = ((!weapon_XPR.IsTacticalReload()) ? mReloadEmpty : mReloadTactical);
			animationState.enabled = true;
			animationState.layer = 2;
			animationState.weight = 1f;
			animationState.normalizedTime = weapon_XPR.GetReloadingAmount();
			mOldTimeInReloadAnim = weapon_XPR.PlayReloadSfxFP(mOldTimeInReloadAnim, animationState.normalizedTime * animationState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_XPR.IsRecoiling())
		{
			float recoilingAmount = weapon_XPR.GetRecoilingAmount();
			float hipsToSightsBlendAmount = weapon_XPR.GetHipsToSightsBlendAmount();
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
