using UnityEngine;

public class ViewModelBlendTree_KSG : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private float[,] mReloadCockingEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private int mWhichReloadCockingEvent;

	private float mOldTimeInReloadCockingAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mReloadStart;

	private AnimationState mReloadLoop;

	private AnimationState mReloadEnd;

	public ViewModelBlendTree_KSG(GameObject weapon, WeaponCoreAnims coreAnims, KSGAnims specificAnims)
	{
		mHipsFire = weapon.animation.AddClipSafe(specificAnims.HipsFire, "HipsFire");
		mSightsFire = weapon.animation.AddClipSafe(specificAnims.SightsFire, "SightsFire");
		mReloadStart = weapon.animation.AddClipSafe(specificAnims.ReloadStart, "ReloadStart");
		mReloadLoop = weapon.animation.AddClipSafe(specificAnims.ReloadLoop, "ReloadLoop");
		mReloadEnd = weapon.animation.AddClipSafe(specificAnims.ReloadEnd, "ReloadEnd");
		mHipsFire.wrapMode = WrapMode.ClampForever;
		mSightsFire.wrapMode = WrapMode.ClampForever;
		mReloadStart.wrapMode = WrapMode.ClampForever;
		mReloadLoop.wrapMode = WrapMode.ClampForever;
		mReloadEnd.wrapMode = WrapMode.ClampForever;
		ViewModelRig.SetupEventData(out mReloadEventTimes, specificAnims.ReloadEndEvents, specificAnims.ReloadLoopEvents);
		ViewModelRig.SetupEventData(out mReloadCockingEventTimes, specificAnims.ReloadHipCockingEvents, specificAnims.ReloadSightsCockingEvents);
	}

	public void Update(IWeapon weaponState)
	{
		Weapon_KSG weapon_KSG = weaponState as Weapon_KSG;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReloadStart.enabled = false;
		mReloadLoop.enabled = false;
		mReloadEnd.enabled = false;
		if (!weapon_KSG.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (!weapon_KSG.IsRecoiling())
		{
			mOldTimeInReloadCockingAnim = 0f;
			mWhichReloadCockingEvent = 0;
		}
		if (weapon_KSG.IsReloading())
		{
			AnimationState animationForReloadState = GetAnimationForReloadState(weapon_KSG.GetReloadState());
			animationForReloadState.enabled = true;
			animationForReloadState.layer = 2;
			animationForReloadState.weight = 1f;
			animationForReloadState.normalizedTime = weapon_KSG.GetReloadStateAmount(weapon_KSG.GetReloadModifier());
			mOldTimeInReloadAnim = weapon_KSG.PlayReloadSfxFP(mOldTimeInReloadAnim, animationForReloadState.normalizedTime * animationForReloadState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_KSG.IsRecoiling())
		{
			float recoilingAmount = weapon_KSG.GetRecoilingAmount();
			float hipsToSightsBlendAmount = weapon_KSG.GetHipsToSightsBlendAmount();
			mHipsFire.enabled = true;
			mHipsFire.layer = 2;
			mHipsFire.weight = 1f - hipsToSightsBlendAmount;
			mHipsFire.normalizedTime = recoilingAmount;
			mSightsFire.enabled = true;
			mSightsFire.layer = 2;
			mSightsFire.weight = hipsToSightsBlendAmount;
			mSightsFire.normalizedTime = recoilingAmount;
			mOldTimeInReloadCockingAnim = weapon_KSG.PlayReloadSfxFP(mOldTimeInReloadCockingAnim, mHipsFire.normalizedTime * mHipsFire.length, mReloadCockingEventTimes, ref mWhichReloadCockingEvent);
		}
	}

	private AnimationState GetAnimationForReloadState(Weapon_KSG.ReloadState reloadState)
	{
		switch (reloadState)
		{
		case Weapon_KSG.ReloadState.Start:
			return mReloadStart;
		case Weapon_KSG.ReloadState.Loop:
			return mReloadLoop;
		case Weapon_KSG.ReloadState.End:
			return mReloadEnd;
		default:
			return null;
		}
	}
}
