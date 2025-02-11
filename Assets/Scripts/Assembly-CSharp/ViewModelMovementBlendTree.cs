using System;
using UnityEngine;

internal class ViewModelMovementBlendTree
{
	private AnimationState mHipsMove;

	private AnimationState mHipsLookUp;

	private AnimationState mHipsLookDown;

	private AnimationState mHipsLookLeft;

	private AnimationState mHipsLookRight;

	private AnimationState mSightsLookUp;

	private AnimationState mSightsLookDown;

	private AnimationState mSightsLookLeft;

	private AnimationState mSightsLookRight;

	private float mMoveTime;

	public Vector3 ViewBob { get; private set; }

	public ViewModelMovementBlendTree(GameObject weapon, WeaponLookAnims hipsAnims, WeaponLookAnims sightsAnims)
	{
		Animation animation = weapon.animation;
		mHipsMove = animation["Move"];
		mHipsLookUp = animation.AddClipSafe(hipsAnims.Up, "Hips_LookUp");
		mHipsLookDown = animation.AddClipSafe(hipsAnims.Down, "Hips_LookDown");
		mHipsLookLeft = animation.AddClipSafe(hipsAnims.Left, "Hips_LookLeft");
		mHipsLookRight = animation.AddClipSafe(hipsAnims.Right, "Hips_LookRight");
		mSightsLookUp = animation.AddClipSafe(sightsAnims.Up, "Sights_LookUp");
		mSightsLookDown = animation.AddClipSafe(sightsAnims.Down, "Sights_LookDown");
		mSightsLookLeft = animation.AddClipSafe(sightsAnims.Left, "Sights_LookLeft");
		mSightsLookRight = animation.AddClipSafe(sightsAnims.Right, "Sights_LookRight");
		if (mHipsMove != null)
		{
			mHipsMove.blendMode = AnimationBlendMode.Additive;
			mHipsMove.enabled = true;
			mHipsMove.weight = 1f;
			mHipsMove.speed = 0f;
		}
		if (mHipsLookUp != null)
		{
			mHipsLookUp.blendMode = AnimationBlendMode.Additive;
			mHipsLookUp.enabled = true;
			mHipsLookUp.weight = 1f;
			mHipsLookUp.speed = 0f;
		}
		if (mHipsLookDown != null)
		{
			mHipsLookDown.blendMode = AnimationBlendMode.Additive;
			mHipsLookDown.enabled = true;
			mHipsLookDown.weight = 1f;
			mHipsLookDown.speed = 0f;
		}
		if (mHipsLookLeft != null)
		{
			mHipsLookLeft.blendMode = AnimationBlendMode.Additive;
			mHipsLookLeft.enabled = true;
			mHipsLookLeft.weight = 1f;
			mHipsLookLeft.speed = 0f;
		}
		if (mHipsLookRight != null)
		{
			mHipsLookRight.blendMode = AnimationBlendMode.Additive;
			mHipsLookRight.enabled = true;
			mHipsLookRight.weight = 1f;
			mHipsLookRight.speed = 0f;
		}
		if (mSightsLookUp != null)
		{
			mSightsLookUp.blendMode = AnimationBlendMode.Additive;
			mSightsLookUp.enabled = true;
			mSightsLookUp.weight = 1f;
			mSightsLookUp.speed = 0f;
		}
		if (mSightsLookDown != null)
		{
			mSightsLookDown.blendMode = AnimationBlendMode.Additive;
			mSightsLookDown.enabled = true;
			mSightsLookDown.weight = 1f;
			mSightsLookDown.speed = 0f;
		}
		if (mSightsLookLeft != null)
		{
			mSightsLookLeft.blendMode = AnimationBlendMode.Additive;
			mSightsLookLeft.enabled = true;
			mSightsLookLeft.weight = 1f;
			mSightsLookLeft.speed = 0f;
		}
		if (mSightsLookRight != null)
		{
			mSightsLookRight.blendMode = AnimationBlendMode.Additive;
			mSightsLookRight.enabled = true;
			mSightsLookRight.weight = 1f;
			mSightsLookRight.speed = 0f;
		}
	}

	public void Update(IWeapon weaponState)
	{
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(weaponState);
		IWeaponMovement weaponMovement = WeaponUtils.GetWeaponMovement(weaponState);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		if (weaponMovement != null)
		{
			num = weaponMovement.GetUpDownLeanAmount();
			num2 = weaponMovement.GetLeftRightLeanAmount();
			num3 = weaponMovement.GetMovementAmount();
		}
		if (weaponADS != null)
		{
			num4 = weaponADS.GetHipsToSightsBlendAmount();
		}
		if (mHipsMove != null)
		{
			float num5 = -2f;
			float num6 = 1f;
			float num7 = num3 * (1f - num4);
			float value = num7 - mHipsMove.weight;
			mHipsMove.enabled = true;
			mHipsMove.weight += Mathf.Clamp(value, num5 * Time.deltaTime, num6 * Time.deltaTime);
			mMoveTime += 2.5f * num3 * Time.deltaTime;
			mHipsMove.time = mMoveTime;
			ViewBob = mHipsMove.weight * InputSettings.FirstPersonViewBobAmount * Vector3.up * Mathf.Sin((float)Math.PI * 4f * mHipsMove.normalizedTime);
		}
		if (mHipsLookUp != null)
		{
			mHipsLookUp.enabled = true;
			mHipsLookUp.normalizedTime = Mathf.Lerp(Mathf.Clamp01(0f - num), 0f, num4);
		}
		if (mHipsLookDown != null)
		{
			mHipsLookDown.enabled = true;
			mHipsLookDown.normalizedTime = Mathf.Lerp(Mathf.Clamp01(num), 0f, num4);
		}
		if (mHipsLookLeft != null)
		{
			mHipsLookLeft.enabled = true;
			mHipsLookLeft.normalizedTime = Mathf.Lerp(Mathf.Clamp01(0f - num2), 0f, num4);
		}
		if (mHipsLookRight != null)
		{
			mHipsLookRight.enabled = true;
			mHipsLookRight.normalizedTime = Mathf.Lerp(Mathf.Clamp01(num2), 0f, num4);
		}
		if (mSightsLookUp != null)
		{
			mSightsLookUp.enabled = true;
			mSightsLookUp.normalizedTime = Mathf.Lerp(0f, Mathf.Clamp01(0f - num), num4);
		}
		if (mSightsLookDown != null)
		{
			mSightsLookDown.enabled = true;
			mSightsLookDown.normalizedTime = Mathf.Lerp(0f, Mathf.Clamp01(num), num4);
		}
		if (mSightsLookLeft != null)
		{
			mSightsLookLeft.enabled = true;
			mSightsLookLeft.normalizedTime = Mathf.Lerp(0f, Mathf.Clamp01(0f - num2), num4);
		}
		if (mSightsLookRight != null)
		{
			mSightsLookRight.enabled = true;
			mSightsLookRight.normalizedTime = Mathf.Lerp(0f, Mathf.Clamp01(num2), num4);
		}
	}
}
