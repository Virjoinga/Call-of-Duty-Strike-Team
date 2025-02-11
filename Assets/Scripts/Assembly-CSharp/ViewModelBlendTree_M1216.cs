using UnityEngine;

public class ViewModelBlendTree_M1216 : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private int mWhichRechamberingEvent;

	private float mOldTimeInRechamberingAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mHipsRechamber;

	private AnimationState mSightsRechamber;

	private AnimationState mReloadEmpty;

	private AnimationState mReloadTactical;

	public ViewModelBlendTree_M1216(GameObject weapon, WeaponCoreAnims coreAnims, M1216Anims specificAnims)
	{
		mHipsFire = weapon.animation.AddClipSafe(specificAnims.HipsFire, "HipsFire");
		mSightsFire = weapon.animation.AddClipSafe(specificAnims.SightsFire, "SightsFire");
		mHipsRechamber = weapon.animation.AddClipSafe(specificAnims.HipsRechamber, "HipsRechamber");
		mSightsRechamber = weapon.animation.AddClipSafe(specificAnims.SightsRechamber, "SightsRechamber");
		mReloadEmpty = weapon.animation.AddClipSafe(specificAnims.ReloadEmpty, "ReloadEmpty");
		mReloadTactical = weapon.animation.AddClipSafe(specificAnims.ReloadTactical, "ReloadTactical");
		mReloadEmpty.wrapMode = WrapMode.Once;
		ViewModelRig.SetupEventData(out mReloadEventTimes, specificAnims.ReloadEmptyEvents, specificAnims.RechamberingEvents);
	}

	public void Update(IWeapon weaponState)
	{
		Weapon_M1216 weapon_M = weaponState as Weapon_M1216;
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(weaponState);
		float num = ((weaponADS == null) ? 0f : weaponADS.GetHipsToSightsBlendAmount());
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mHipsRechamber.enabled = false;
		mSightsRechamber.enabled = false;
		mReloadEmpty.enabled = false;
		mReloadTactical.enabled = false;
		if (!weapon_M.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (!weapon_M.IsRechambering())
		{
			mWhichRechamberingEvent = 0;
			mOldTimeInRechamberingAnim = 0f;
		}
		if (weapon_M.IsReloading())
		{
			AnimationState animationState = ((!weapon_M.IsTacticalReload()) ? mReloadEmpty : mReloadTactical);
			animationState.enabled = true;
			animationState.layer = 2;
			animationState.weight = 1f;
			animationState.normalizedTime = weapon_M.GetReloadingAmount();
			mOldTimeInReloadAnim = weapon_M.PlayReloadSfxFP(mOldTimeInReloadAnim, animationState.normalizedTime * animationState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_M.IsRechambering())
		{
			float rechamberingAmount = weapon_M.GetRechamberingAmount();
			mHipsRechamber.enabled = true;
			mHipsRechamber.layer = 2;
			mHipsRechamber.weight = 1f - num;
			mHipsRechamber.normalizedTime = rechamberingAmount;
			mSightsRechamber.enabled = true;
			mSightsRechamber.layer = 2;
			mSightsRechamber.weight = num;
			mSightsRechamber.normalizedTime = rechamberingAmount;
			mOldTimeInRechamberingAnim = weapon_M.PlayReloadSfxFP(mOldTimeInRechamberingAnim, mHipsRechamber.normalizedTime * mHipsRechamber.length, mReloadEventTimes, ref mWhichRechamberingEvent);
		}
		else if (weapon_M.IsRecoiling())
		{
			float recoilingAmount = weapon_M.GetRecoilingAmount();
			mHipsFire.enabled = true;
			mHipsFire.layer = 2;
			mHipsFire.weight = 1f - num;
			mHipsFire.normalizedTime = recoilingAmount;
			mSightsFire.enabled = true;
			mSightsFire.layer = 2;
			mSightsFire.weight = num;
			mSightsFire.normalizedTime = recoilingAmount;
		}
	}
}
