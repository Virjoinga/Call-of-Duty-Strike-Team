using UnityEngine;

public class ViewModelBlendTree_Ballista : IViewModelWeaponBlendTree
{
	private float[,] mReloadEventTimes;

	private int mWhichReloadEvent;

	private float mOldTimeInReloadAnim;

	private int mWhichRechamberingEvent;

	private float mOldTimeInRechamberingAnim;

	private AnimationState mHipsFire;

	private AnimationState mSightsFire;

	private AnimationState mHipsChamber;

	private AnimationState mSightsChamber;

	private AnimationState mReload;

	public ViewModelBlendTree_Ballista(GameObject weapon, WeaponCoreAnims coreAnims, BallistaAnims specificAnims)
	{
		mHipsFire = weapon.animation.AddClipSafe(specificAnims.HipsFire, "HipsFire");
		mSightsFire = weapon.animation.AddClipSafe(specificAnims.SightsFire, "SightsFire");
		mHipsChamber = weapon.animation.AddClipSafe(specificAnims.HipsChamber, "HipsChamber");
		mSightsChamber = weapon.animation.AddClipSafe(specificAnims.SightsChamber, "SightsChamber");
		mReload = weapon.animation.AddClipSafe(specificAnims.Reload, "Reload");
		mReload.wrapMode = WrapMode.Once;
		ViewModelRig.SetupEventData(out mReloadEventTimes, specificAnims.ReloadEmptyEvents, specificAnims.RechamberingEvents);
	}

	public void Update(IWeapon weaponState)
	{
		Weapon_Ballista weapon_Ballista = weaponState as Weapon_Ballista;
		mHipsFire.enabled = false;
		mSightsFire.enabled = false;
		mReload.enabled = false;
		if (!weapon_Ballista.IsReloading())
		{
			mOldTimeInReloadAnim = 0f;
			mWhichReloadEvent = 0;
		}
		if (weapon_Ballista.IsRechambering())
		{
			float rechamberingAmount = weapon_Ballista.GetRechamberingAmount();
			float hipsToSightsBlendAmount = weapon_Ballista.GetHipsToSightsBlendAmount();
			mHipsChamber.enabled = true;
			mHipsChamber.layer = 2;
			mHipsChamber.weight = 1f - hipsToSightsBlendAmount;
			mHipsChamber.normalizedTime = rechamberingAmount;
			mSightsChamber.enabled = true;
			mSightsChamber.layer = 2;
			mSightsChamber.weight = hipsToSightsBlendAmount;
			mSightsChamber.normalizedTime = rechamberingAmount;
			mOldTimeInRechamberingAnim = weapon_Ballista.PlayReloadSfxFP(mOldTimeInRechamberingAnim, mHipsChamber.normalizedTime * mHipsChamber.length, mReloadEventTimes, ref mWhichRechamberingEvent);
		}
		else
		{
			mWhichRechamberingEvent = 0;
			mOldTimeInRechamberingAnim = 0f;
		}
		if (weapon_Ballista.IsReloading())
		{
			AnimationState animationState = mReload;
			animationState.enabled = true;
			animationState.layer = 2;
			animationState.weight = 1f;
			animationState.normalizedTime = weapon_Ballista.GetReloadingAmount();
			mOldTimeInReloadAnim = weapon_Ballista.PlayReloadSfxFP(mOldTimeInReloadAnim, animationState.normalizedTime * animationState.length, mReloadEventTimes, ref mWhichReloadEvent);
		}
		else if (weapon_Ballista.IsRecoiling())
		{
			float recoilingAmount = weapon_Ballista.GetRecoilingAmount();
			float hipsToSightsBlendAmount2 = weapon_Ballista.GetHipsToSightsBlendAmount();
			mHipsFire.enabled = true;
			mHipsFire.layer = 2;
			mHipsFire.weight = 1f - hipsToSightsBlendAmount2;
			mHipsFire.normalizedTime = recoilingAmount;
			mSightsFire.enabled = true;
			mSightsFire.layer = 2;
			mSightsFire.weight = hipsToSightsBlendAmount2;
			mSightsFire.normalizedTime = recoilingAmount;
		}
	}
}
