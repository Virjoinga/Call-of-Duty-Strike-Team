using UnityEngine;

public class ViewModelBlendTree_KS23 : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private float[,] mReloadCockingEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private int mWhichRechamberingEvent;

	private float mOldTimeInRechamberingAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mReloadStart;

	private AnimationState mReloadLoop;

	private AnimationState mReloadEnd;

	private AnimationState mHipsRechamber;

	private AnimationState mSightsRechamber;

	public ViewModelBlendTree_KS23(GameObject weapon, WeaponCoreAnims coreAnims, KS23Anims specificAnims)
	{
		mHipsFire = weapon.animation.AddClipSafe(specificAnims.HipsFire, "HipsFire");
		mSightsFire = weapon.animation.AddClipSafe(specificAnims.SightsFire, "SightsFire");
		mReloadStart = weapon.animation.AddClipSafe(specificAnims.ReloadStart, "ReloadStart");
		mReloadLoop = weapon.animation.AddClipSafe(specificAnims.ReloadLoop, "ReloadLoop");
		mReloadEnd = weapon.animation.AddClipSafe(specificAnims.ReloadEnd, "ReloadEnd");
		mHipsRechamber = weapon.animation.AddClipSafe(specificAnims.HipsCocking, "HipsCocking");
		mSightsRechamber = weapon.animation.AddClipSafe(specificAnims.SightsCocking, "SightsCocking");
		mHipsFire.wrapMode = WrapMode.ClampForever;
		mSightsFire.wrapMode = WrapMode.ClampForever;
		mReloadStart.wrapMode = WrapMode.ClampForever;
		mReloadLoop.wrapMode = WrapMode.ClampForever;
		mReloadEnd.wrapMode = WrapMode.ClampForever;
		ViewModelRig.SetupEventData(out mReloadEventTimes, specificAnims.ReloadEmptyEvents, specificAnims.RechamberingEvents);
		ViewModelRig.SetupEventData(out mReloadCockingEventTimes, specificAnims.ReloadHipCockingEvents, specificAnims.ReloadSightsCockingEvents);
	}

	public void Update(IWeapon weaponState)
	{
		Weapon_KS23 weapon_KS = weaponState as Weapon_KS23;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReloadStart.enabled = false;
		mReloadLoop.enabled = false;
		mReloadEnd.enabled = false;
		mHipsRechamber.enabled = false;
		mSightsRechamber.enabled = false;
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(weaponState);
		float num = ((weaponADS == null) ? 0f : weaponADS.GetHipsToSightsBlendAmount());
		if (!weapon_KS.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (!weapon_KS.IsRechambering())
		{
			mWhichRechamberingEvent = 0;
			mOldTimeInRechamberingAnim = 0f;
		}
		if (weapon_KS.IsReloading())
		{
			AnimationState animationForReloadState = GetAnimationForReloadState(weapon_KS.GetReloadState());
			animationForReloadState.enabled = true;
			animationForReloadState.layer = 2;
			animationForReloadState.weight = 1f;
			animationForReloadState.normalizedTime = weapon_KS.GetReloadStateAmount(weapon_KS.GetReloadModifier());
			mOldTimeInReloadAnim = weapon_KS.PlayReloadSfxFP(mOldTimeInReloadAnim, animationForReloadState.normalizedTime * animationForReloadState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_KS.IsRechambering())
		{
			float rechamberingAmount = weapon_KS.GetRechamberingAmount();
			mHipsRechamber.enabled = true;
			mHipsRechamber.layer = 2;
			mHipsRechamber.weight = 1f - num;
			mHipsRechamber.normalizedTime = rechamberingAmount;
			mSightsRechamber.enabled = true;
			mSightsRechamber.layer = 2;
			mSightsRechamber.weight = num;
			mSightsRechamber.normalizedTime = rechamberingAmount;
			mOldTimeInRechamberingAnim = weapon_KS.PlayReloadSfxFP(mOldTimeInRechamberingAnim, mHipsRechamber.normalizedTime * mHipsRechamber.length, mReloadCockingEventTimes, ref mWhichRechamberingEvent);
		}
		else if (weapon_KS.IsRecoiling())
		{
			float recoilingAmount = weapon_KS.GetRecoilingAmount();
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

	private AnimationState GetAnimationForReloadState(Weapon_KS23.ReloadState reloadState)
	{
		switch (reloadState)
		{
		case Weapon_KS23.ReloadState.Start:
			return mReloadStart;
		case Weapon_KS23.ReloadState.Loop:
			return mReloadLoop;
		case Weapon_KS23.ReloadState.End:
			return mReloadEnd;
		default:
			return null;
		}
	}
}
