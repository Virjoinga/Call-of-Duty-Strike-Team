using UnityEngine;

public class ViewModelBlendTree_QBBLSW : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mReloadEmpty;

	private AnimationState mReloadTactical;

	public ViewModelBlendTree_QBBLSW(GameObject weapon, WeaponCoreAnims coreAnims, QBBLSWAnims specificAnims)
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
		Weapon_QBBLSW weapon_QBBLSW = weaponState as Weapon_QBBLSW;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReloadEmpty.enabled = false;
		mReloadTactical.enabled = false;
		if (!weapon_QBBLSW.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (weapon_QBBLSW.IsReloading())
		{
			AnimationState animationState = ((!weapon_QBBLSW.IsTacticalReload()) ? mReloadEmpty : mReloadTactical);
			animationState.enabled = true;
			animationState.layer = 2;
			animationState.weight = 1f;
			animationState.normalizedTime = weapon_QBBLSW.GetReloadingAmount();
			mOldTimeInReloadAnim = weapon_QBBLSW.PlayReloadSfxFP(mOldTimeInReloadAnim, animationState.normalizedTime * animationState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_QBBLSW.IsRecoiling())
		{
			float recoilingAmount = weapon_QBBLSW.GetRecoilingAmount();
			float hipsToSightsBlendAmount = weapon_QBBLSW.GetHipsToSightsBlendAmount();
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
