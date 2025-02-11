using System;
using UnityEngine;

[Serializable]
public class WeaponCoreAnims
{
	public Vector3 PositionOffset;

	public Vector3 RotationOffset;

	public AnimationClip HipsIdle;

	public AnimationClip SightsIdle;

	public AnimationClip TakeOut;

	public AnimationClip PutAway;

	public AnimationClip Move;

	public AnimationClip HipsToSights;

	public WeaponLookAnims HipsLook;

	public WeaponLookAnims SightsLook;
}
