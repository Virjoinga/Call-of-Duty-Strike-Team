using UnityEngine;

public static class RulesSystem
{
	public delegate void OnExplosionEventHandler(Vector3 origin, float radius, GameObject damageDealer, ExplosionManager.ExplosionType explosionType, string damageType);

	public static event OnExplosionEventHandler OnExplosion;

	public static void Log(string message)
	{
		if (RulesSystemVisualiser.Instance() != null)
		{
			RulesSystemVisualiser.Instance().AddLogEntry(message);
		}
	}

	public static bool HasHitTargetWithProjectile(RSProjectileAttackInterface attacker, RSProjectileDefenceInterface defender)
	{
		if (!HasHitTargetWithProjectileAttackRoll(attacker))
		{
			Log("Rules System: HasHitTargetWithProjectile -> Attack Roll FAILED");
			return false;
		}
		if (HasTargetAvoidedWithProjectileDefenceRoll(defender))
		{
			Log("Rules System: HasHitTargetWithProjectile -> Defence Roll SUCCESS");
			return false;
		}
		Log("Rules System: HasHitTargetWithProjectile -> Attack Roll SUCCESS, Defence Roll FAILED -> HIT!");
		return true;
	}

	public static float CalculateProjectileDamage(RSProjectileAttackDamageInterface attacker, RSProjectileDamageDefenceInterface defender)
	{
		float num = RulesSystemSettings.ProjectileDamageDefault;
		if (attacker.IsCritical)
		{
			num = Mathf.Max(num, RulesSystemSettings.ProjectileDamageCritical);
		}
		if (attacker.IsExplosion)
		{
			num = Mathf.Max(num, RulesSystemSettings.ProjectileDamageExplosion);
		}
		if (attacker.IsFlanking)
		{
			num = Mathf.Max(num, RulesSystemSettings.ProjectileDamageFlanking);
		}
		num *= attacker.WeaponDamageMultiplier;
		if (defender.IsAGR)
		{
			num *= RulesSystemSettings.AgainstAGRPenaltyModifier;
		}
		else if (!defender.IsHuman && !attacker.IsFlanking && !attacker.IsExplosion)
		{
			num *= RulesSystemSettings.AgainstNoneHumanNoFlankingPenaltyModifier;
		}
		Log(string.Format("CalculateProjectileDamage: damage={0}", num));
		return num;
	}

	public static float CalculateMeleeDamage(RSMeleeAttackDamageInterface attacker, RSMeleeDamageDefenceInterface defender)
	{
		return 1f;
	}

	public static void DoAreaOfEffectDamage(Vector3 origin, float radius, float damageMultiplier, GameObject damageDealer, ExplosionManager.ExplosionType explosionType, string damageType)
	{
		DoAreaOfEffectDamage(origin, radius, damageMultiplier, damageDealer, explosionType, damageType, false);
	}

	public static void DoAreaOfEffectDamage(Vector3 origin, float radius, float damageMultiplier, GameObject damageDealer, ExplosionManager.ExplosionType explosionType, string damageType, bool ignoreFriendlies)
	{
		DoAOEDamage(origin, radius, damageMultiplier, damageDealer, damageType, true, ignoreFriendlies);
		if (RulesSystem.OnExplosion != null)
		{
			RulesSystem.OnExplosion(origin, radius, damageDealer, explosionType, damageType);
		}
		ExplosionManager.Instance.StartExplosion(origin, radius, explosionType);
	}

	private static void DoAOEDamage(Vector3 origin, float radius, float damageMultiplier, GameObject damageDealer, string damageType, bool applyRagdollForce, bool ignoreFriendlies)
	{
		float num = radius * radius;
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (ignoreFriendlies && a.awareness.faction == FactionHelper.Category.Player)
			{
				continue;
			}
			Vector3 vector = origin - a.GetPosition();
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude < num)
			{
				Vector3 position = a.GetPosition();
				if (a.realCharacter != null)
				{
					if (a.realCharacter.LowProfile())
					{
						position += Vector3.up * 0.7f;
					}
					else
					{
						position += Vector3.up * 1.08f;
					}
				}
				else
				{
					position += Vector3.up;
				}
				SurfaceImpact impact = ProjectileManager.Trace(origin, position, ProjectileManager.SimpleHitBoxProjectileMask);
				HitBoxUtils.DelegateImpact(ref impact, null);
				if (impact.gameobject != null)
				{
					HitLocation component = impact.gameobject.GetComponent<HitLocation>();
					if (component != null && component.Actor == a)
					{
						a.rules.RefreshProjectileDamageDefenceInterface();
						float projectileDamageExplosion = RulesSystemSettings.ProjectileDamageExplosion;
						float actualHealthFromDelta = a.health.GetActualHealthFromDelta(projectileDamageExplosion);
						float damageMultiplier2 = component.DamageMultiplier;
						actualHealthFromDelta *= damageMultiplier2;
						actualHealthFromDelta *= damageMultiplier;
						float takeDamageModifier = a.health.TakeDamageModifier;
						a.health.TakeDamageModifier = 1f;
						a.health.ModifyHealth(damageDealer, 0f - actualHealthFromDelta, damageType, vector.normalized, false, false, component, impact);
						a.health.TakeDamageModifier = takeDamageModifier;
						Log(string.Format("DoAreaOfEffectDamage - deal {0} damage to {1}", actualHealthFromDelta, a.realCharacter.name));
					}
				}
			}
			if (!applyRagdollForce || !(a.realCharacter.Ragdoll != null) || !a.health.HealthEmpty)
			{
				continue;
			}
			HitLocation[] bones = a.realCharacter.Ragdoll.Bones;
			foreach (HitLocation hitLocation in bones)
			{
				if (hitLocation.GetComponent<Rigidbody>() != null)
				{
					hitLocation.GetComponent<Rigidbody>().AddExplosionForce(25f, origin, radius * 2f, 0.5f * radius, ForceMode.Impulse);
				}
			}
		}
		foreach (Helicopter item in Helicopter.GlobalPoolCache)
		{
			if (item == null || !item.gameObject.activeInHierarchy)
			{
				continue;
			}
			Vector3 vector2 = origin - item.transform.position;
			float num2 = vector2.magnitude - 4f;
			if (!(num2 < radius))
			{
				continue;
			}
			Vector3 target = item.transform.position + Vector3.up * 2f;
			SurfaceImpact surfaceImpact = null;
			surfaceImpact = ((!item.OverWatchHeli) ? ProjectileManager.Trace(origin, target, ProjectileManager.ProjectileMask) : ProjectileManager.Trace(origin, target, ProjectileManager.SimpleHitBoxProjectileMask));
			if (surfaceImpact.gameobject != null)
			{
				HitLocation component2 = surfaceImpact.gameobject.GetComponent<HitLocation>();
				if (component2 != null && component2.Health != null && component2.Health.GetComponent<Helicopter>() == item)
				{
					float projectileDamageExplosion2 = RulesSystemSettings.ProjectileDamageExplosion;
					float actualHealthFromDelta2 = component2.Health.GetActualHealthFromDelta(projectileDamageExplosion2);
					float damageMultiplier3 = component2.DamageMultiplier;
					actualHealthFromDelta2 *= damageMultiplier3;
					actualHealthFromDelta2 *= damageMultiplier;
					component2.Health.ModifyHealth(damageDealer, 0f - actualHealthFromDelta2, damageType, vector2.normalized, false, false, component2, surfaceImpact);
					Log(string.Format("DoAreaOfEffectDamage - deal {0} damage to {1}", actualHealthFromDelta2, item.name));
				}
			}
		}
		foreach (Tank item2 in Tank.GlobalPoolCache)
		{
			if (item2 == null || !item2.gameObject.activeInHierarchy)
			{
				continue;
			}
			Vector3 vector3 = origin - item2.transform.position;
			float num3 = vector3.magnitude - 4f;
			if (!(num3 < radius))
			{
				continue;
			}
			Vector3 target2 = item2.transform.position + Vector3.up * 2f;
			SurfaceImpact surfaceImpact2 = ProjectileManager.Trace(origin, target2, ProjectileManager.ProjectileMask);
			if (surfaceImpact2.gameobject != null)
			{
				HitLocation component3 = surfaceImpact2.gameobject.GetComponent<HitLocation>();
				if (component3 != null && component3.Health != null && component3.Health.GetComponent<Tank>() == item2)
				{
					float projectileDamageExplosion3 = RulesSystemSettings.ProjectileDamageExplosion;
					float actualHealthFromDelta3 = component3.Health.GetActualHealthFromDelta(projectileDamageExplosion3);
					float damageMultiplier4 = component3.DamageMultiplier;
					actualHealthFromDelta3 *= damageMultiplier4;
					actualHealthFromDelta3 *= damageMultiplier;
					component3.Health.ModifyHealth(damageDealer, 0f - actualHealthFromDelta3, damageType, vector3.normalized, false, false, component3, surfaceImpact2);
					Log(string.Format("DoAreaOfEffectDamage - deal {0} damage to {1}", actualHealthFromDelta3, item2.name));
				}
			}
		}
		foreach (HackableObjectClaymore item3 in HackableObjectClaymore.GlobalPoolCache)
		{
			if (item3 == null || !item3.gameObject.activeInHierarchy)
			{
				continue;
			}
			Vector3 vector4 = origin - item3.transform.position;
			float num4 = vector4.magnitude - 0.5f;
			if (!(num4 < radius))
			{
				continue;
			}
			Vector3 target3 = item3.transform.position + Vector3.up * 0.2f;
			SurfaceImpact surfaceImpact3 = ProjectileManager.Trace(origin, target3, ProjectileManager.ProjectileMask);
			if (surfaceImpact3.gameobject != null)
			{
				HitLocation component4 = surfaceImpact3.gameobject.GetComponent<HitLocation>();
				if (component4 != null && component4.Health != null && component4.Health.GetComponent<HackableObjectClaymore>() == item3)
				{
					float projectileDamageExplosion4 = RulesSystemSettings.ProjectileDamageExplosion;
					float actualHealthFromDelta4 = component4.Health.GetActualHealthFromDelta(projectileDamageExplosion4);
					float damageMultiplier5 = component4.DamageMultiplier;
					actualHealthFromDelta4 *= damageMultiplier5;
					actualHealthFromDelta4 *= damageMultiplier;
					component4.Health.ModifyHealth(damageDealer, 0f - actualHealthFromDelta4, damageType, vector4.normalized, false, false, component4, surfaceImpact3);
					Log(string.Format("DoAreaOfEffectDamage - deal {0} damage to {1}", actualHealthFromDelta4, item3.name));
				}
			}
		}
	}

	private static bool HasHitTargetWithProjectileAttackRoll(RSProjectileAttackInterface attacker)
	{
		float num = 0f;
		RSProjectileWeaponInterface weaponInterface = attacker.WeaponInterface;
		num = weaponInterface.BaseAccuracy;
		if (attacker.IsRunning)
		{
			num *= RulesSystemSettings.RunningAccuracyPenaltyMultiplier;
		}
		Log(string.Format("Rules System: HasHitTargetWithProjectileAttackRoll -> hitChance={0}", num));
		return RulesSystemDice.Delta() <= num;
	}

	private static bool HasTargetAvoidedWithProjectileDefenceRoll(RSProjectileDefenceInterface defender)
	{
		float num = 0f;
		if (defender.InCoverHiding)
		{
			num = Mathf.Max(num, RulesSystemSettings.AvoidChanceInCoverWhenHiding);
		}
		else if (defender.InCoverShooting)
		{
			num = Mathf.Max(num, RulesSystemSettings.AvoidChanceInCoverWhenShooting);
		}
		else if (defender.IsMovingIntoCover)
		{
			num = Mathf.Max(num, RulesSystemSettings.AvoidChanceMovingIntoCover);
		}
		if (defender.IsDodging)
		{
			num = Mathf.Max(num, RulesSystemSettings.AvoidChanceDodging);
		}
		Log(string.Format("Rules System: HasTargetAvoidedWithProjectileDefenceRoll -> hitChance={0}", num));
		return RulesSystemDice.Delta() <= num;
	}
}
