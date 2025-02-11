using System;
using UnityEngine;

[Serializable]
public class HelicopterData
{
	public float FleeReturnSpeedMultiplier = 1f;

	public float GunTrackingSpeed = 1f;

	public float MachineGunDamage = 40f;

	public int BulletsInSpread = 1;

	public float TimeBetweenRockets = 6f;

	public float Health = 20000f;

	public float HealthDamageFleeDelta = 0.33f;

	public HudBlipIcon HudMarker;

	public GameObject DamageEffect;

	public Vector3 DamageEffectOffset = new Vector3(0f, 3f, 1f);

	public float DamageEffectHealthPercent = 10f;

	public GameObject DamageEffectAfterFlee;

	public Vector3 DamageEffectAfterFleeOffset = new Vector3(0f, 3f, 1f);

	public RPGProjectileData RPGProjectileSettings;

	public void CopyContainerData(Helicopter h)
	{
		h.FleeReturnSpeedMultiplier = FleeReturnSpeedMultiplier;
		h.GunTrackingSpeed = GunTrackingSpeed;
		h.MachineGunDamage = MachineGunDamage;
		h.BulletsInSpread = BulletsInSpread;
		h.TimeBetweenRockets = TimeBetweenRockets;
		h.Health = Health;
		h.HealthDamageFleeDelta = HealthDamageFleeDelta;
		h.HudMarker = HudMarker;
		h.DamageEffect = DamageEffect;
		h.DamageEffectOffset = DamageEffectOffset;
		h.DamageEffectHealthPercent = DamageEffectHealthPercent;
		h.DamageEffectAfterFlee = DamageEffectAfterFlee;
		h.DamageEffectAfterFleeOffset = DamageEffectAfterFleeOffset;
	}
}
