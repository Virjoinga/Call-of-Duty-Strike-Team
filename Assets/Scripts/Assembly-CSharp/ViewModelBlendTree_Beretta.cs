using UnityEngine;

public class ViewModelBlendTree_Beretta : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mReloadEmpty;

	private AnimationState mReloadTactical;

	public ViewModelBlendTree_Beretta(GameObject weapon, WeaponCoreAnims coreAnims, BerettaAnims specificAnims)
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
		Weapon_Beretta weapon_Beretta = weaponState as Weapon_Beretta;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReloadEmpty.enabled = false;
		mReloadTactical.enabled = false;
		if (!weapon_Beretta.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (weapon_Beretta.IsReloading())
		{
			AnimationState animationState = ((!weapon_Beretta.IsTacticalReload()) ? mReloadEmpty : mReloadTactical);
			animationState.enabled = true;
			animationState.layer = 2;
			animationState.weight = 1f;
			animationState.normalizedTime = weapon_Beretta.GetReloadingAmount();
			mOldTimeInReloadAnim = weapon_Beretta.PlayReloadSfxFP(mOldTimeInReloadAnim, animationState.normalizedTime * animationState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_Beretta.IsRecoiling())
		{
			float recoilingAmount = weapon_Beretta.GetRecoilingAmount();
			float hipsToSightsBlendAmount = weapon_Beretta.GetHipsToSightsBlendAmount();
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
