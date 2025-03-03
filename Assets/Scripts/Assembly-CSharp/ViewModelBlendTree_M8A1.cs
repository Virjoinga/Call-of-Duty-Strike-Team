using UnityEngine;

public class ViewModelBlendTree_M8A1 : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mReloadEmpty;

	private AnimationState mReloadTactical;

	public ViewModelBlendTree_M8A1(GameObject weapon, WeaponCoreAnims coreAnims, M8A1Anims specificAnims)
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
		Weapon_M8A1 weapon_M8A = weaponState as Weapon_M8A1;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReloadEmpty.enabled = false;
		mReloadTactical.enabled = false;
		if (!weapon_M8A.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (weapon_M8A.IsReloading())
		{
			AnimationState animationState = ((!weapon_M8A.IsTacticalReload()) ? mReloadEmpty : mReloadTactical);
			animationState.enabled = true;
			animationState.layer = 2;
			animationState.weight = 1f;
			animationState.normalizedTime = weapon_M8A.GetReloadingAmount();
			mOldTimeInReloadAnim = weapon_M8A.PlayReloadSfxFP(mOldTimeInReloadAnim, animationState.normalizedTime * animationState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_M8A.IsRecoiling())
		{
			float recoilingAmount = weapon_M8A.GetRecoilingAmount();
			float hipsToSightsBlendAmount = weapon_M8A.GetHipsToSightsBlendAmount();
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
