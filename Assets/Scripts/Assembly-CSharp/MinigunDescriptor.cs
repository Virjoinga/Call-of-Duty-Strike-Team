using UnityEngine;

public class MinigunDescriptor : ScriptableObject
{
	public float SpinUpAcceleration;

	public float SpinDownAcceleration;

	public WeaponDescriptor.DamageRange[] DamageRanges;

	public WeaponDescriptor.DamageRange[] DamageRangesNPC;

	public float TimeBetweenShots;

	public float HeatUpSpeed;

	public float CoolDownSpeed;

	public float ShakeIntensity;

	public float Spread;

	public float ImpactForce;

	public bool NoBulletCasings;

	public float AccuracyStatAdjustment = 0.1f;
}
