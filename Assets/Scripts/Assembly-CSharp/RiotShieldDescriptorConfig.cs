using System;

[Serializable]
public class RiotShieldDescriptorConfig
{
	public float GrenadeDamageMultiplier = 0.5f;

	public int GrenadeInventoryCount = 2;

	public float[] ConsiderGrenadeTimeInitialRange = new float[2] { 0.5f, 2f };

	public float[] ConsiderGrenadeTimeSubsequentRange = new float[2] { 10f, 20f };

	public float GrenadeTargetRandomOffset = 3f;

	public float GrenadeTargetFallshortOffset;

	public float GrenadeThrowMinimumDistance = 10f;

	public float GrenadeThrowMaximumDistance = 20f;

	public float RepathDistance = 1f;

	public float ShieldBashDistance = 2f;

	public float ShieldBashDamage = 0.5f;

	public float TimeBetweenMeleeStrikes = 2f;

	public int[] ShotBurst = new int[2] { 1, 6 };

	public float[] TimeToNextShooting = new float[2] { 3f, 6f };

	public float FlinchDuration = 1f;

	public float FlinchBigDuration = 1.5f;
}
