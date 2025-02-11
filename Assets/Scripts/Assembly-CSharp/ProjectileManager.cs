using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
	public delegate float DamageCallback(float distance, HitLocation target, bool isPlayer);

	public delegate float ForceCallback();

	private static ProjectileManager smInstance = null;

	public static List<Actor> UnitList = new List<Actor>();

	private int mHitLayerMask;

	private int mSimpleHitLayerMask;

	public static ProjectileManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	public static int ProjectileMask
	{
		get
		{
			return (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("ProjectileCharacterCollider")) | (1 << LayerMask.NameToLayer("HitBox")) | (1 << LayerMask.NameToLayer("ConstantHitBox")) | (1 << LayerMask.NameToLayer("RoofCollider")) | (1 << LayerMask.NameToLayer("Glass")) | (1 << LayerMask.NameToLayer("Physical"));
		}
	}

	public static int SimpleHitBoxProjectileMask
	{
		get
		{
			return (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("SimpleHitBox")) | (1 << LayerMask.NameToLayer("ConstantHitBox")) | (1 << LayerMask.NameToLayer("RoofCollider")) | (1 << LayerMask.NameToLayer("Glass")) | (1 << LayerMask.NameToLayer("Physical"));
		}
	}

	public static int SimpleUnitProjectileMask
	{
		get
		{
			return 1 << LayerMask.NameToLayer("ActorGameObject");
		}
	}

	public static int DefaultLayerProjectileMask
	{
		get
		{
			return (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("RoofCollider")) | (1 << LayerMask.NameToLayer("Glass")) | (1 << LayerMask.NameToLayer("Physical"));
		}
	}

	public int HitLayerMask
	{
		get
		{
			return mHitLayerMask;
		}
	}

	public static SurfaceImpact Trace(Vector3 traceOrigin, Vector3 target, int hitLayerMask)
	{
		return Trace(traceOrigin, target, float.MaxValue, hitLayerMask);
	}

	public static SurfaceImpact Trace(Vector3 traceOrigin, Vector3 target, float maxDistance, int hitLayerMask)
	{
		GameObject gameobject = null;
		SurfaceMaterial material = SurfaceMaterial.None;
		Vector3 normal = Vector3.zero;
		Vector3 normalized = (target - traceOrigin).normalized;
		bool noDecal = false;
		RaycastHit hitInfo;
		if (Physics.Raycast(traceOrigin, normalized, out hitInfo, maxDistance, hitLayerMask))
		{
			target = hitInfo.point;
			gameobject = ((!(hitInfo.collider != null)) ? null : hitInfo.collider.gameObject);
			material = SurfaceMaterial.Default;
			material = ((!(hitInfo.collider is MeshCollider)) ? HitBoxUtils.GetSurfaceMaterial(hitInfo.collider.gameObject, out noDecal) : ((SurfaceMaterial)Mathf.RoundToInt(hitInfo.textureCoord.x)));
			normal = hitInfo.normal;
		}
		return new SurfaceImpact(gameobject, material, target, -normalized, normal, noDecal);
	}

	public static void TraceFast(Vector3 traceOrigin, Vector3 target, int hitLayerMask, out GameObject outGo, out Vector3 outPosition)
	{
		Vector3 normalized = (target - traceOrigin).normalized;
		RaycastHit hitInfo;
		if (Physics.Raycast(traceOrigin, normalized, out hitInfo, float.MaxValue, hitLayerMask))
		{
			outPosition = hitInfo.point;
			outGo = ((!(hitInfo.collider != null)) ? null : hitInfo.collider.gameObject);
		}
		else
		{
			outGo = null;
			outPosition = Vector3.zero;
		}
	}

	public static Actor[] TraceAllUnits(Vector3 traceOrigin, Vector3 target, float maxDistance, int hitLayerMask)
	{
		int num = LayerMask.NameToLayer("ActorGameObject");
		UnitList.Clear();
		Vector3 normalized = (target - traceOrigin).normalized;
		if (!Physics.Raycast(traceOrigin, normalized, maxDistance, hitLayerMask))
		{
			return null;
		}
		RaycastHit[] array = Physics.RaycastAll(traceOrigin, normalized, maxDistance, hitLayerMask);
		Collider[] array2 = Physics.OverlapSphere(traceOrigin, 0.5f, hitLayerMask);
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].collider.gameObject.layer == num)
				{
					Actor component = array[i].collider.gameObject.GetComponent<Actor>();
					if (component != null)
					{
						UnitList.Add(component);
					}
					continue;
				}
				ActorLink component2 = array[i].collider.gameObject.GetComponent<ActorLink>();
				if (component2 != null)
				{
					Actor linkedActor = component2.linkedActor;
					if (linkedActor != null)
					{
						UnitList.Add(linkedActor);
					}
				}
			}
		}
		if (array2 != null)
		{
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].gameObject.layer == num)
				{
					Actor component3 = array2[j].gameObject.GetComponent<Actor>();
					if (component3 != null)
					{
						UnitList.Add(component3);
					}
				}
			}
		}
		if (UnitList.Count == 0)
		{
			return null;
		}
		return UnitList.ToArray();
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple ProjectileManager");
		}
		smInstance = this;
		mHitLayerMask = ProjectileMask;
		mSimpleHitLayerMask = SimpleHitBoxProjectileMask;
	}

	private void OnDestroy()
	{
		smInstance = null;
		UnitList.Clear();
	}

	public SurfaceImpact StartProjectile(Actor owner, Vector3 origin, Vector3 target, DamageCallback damage, ForceCallback force, bool simpleHitBoxes)
	{
		SurfaceImpact impact;
		if (simpleHitBoxes)
		{
			impact = Trace(origin, target, mSimpleHitLayerMask);
			HitBoxUtils.DelegateImpact(ref impact, owner);
		}
		else
		{
			impact = Trace(origin, target, mHitLayerMask);
		}
		RegisterImpact(impact, origin, owner, damage, force, "Shot");
		return impact;
	}

	public SurfaceImpact KnifeAttack(Actor owner, Vector3 origin, Vector3 target, float range, float damage)
	{
		SurfaceImpact surfaceImpact = Trace(origin, target, range, mHitLayerMask | mSimpleHitLayerMask);
		float num = 0.5f * Vector3.Distance(origin, target);
		for (int num2 = 8; num2 > 0; num2--)
		{
			surfaceImpact = PickBestImpactForMelee(Trace(origin, target + num * UnityEngine.Random.insideUnitSphere, range, mHitLayerMask), surfaceImpact);
		}
		HitLocation hitLocation = ((!(surfaceImpact.gameobject != null)) ? null : surfaceImpact.gameobject.GetComponent<HitLocation>());
		if (hitLocation != null && hitLocation != null && hitLocation.Actor != null && hitLocation.Actor.awareness.ChDefCharacterType == CharacterType.RiotShieldNPC)
		{
			float num3 = Vector3.Dot(owner.realCharacter.FirstPersonCamera.transform.forward, hitLocation.Actor.transform.forward);
			if (num3 < 0f)
			{
				damage = 0f;
				if (surfaceImpact.material == SurfaceMaterial.Flesh)
				{
					surfaceImpact.material = SurfaceMaterial.None;
				}
			}
		}
		DamageCallback damage2 = (float P_0, HitLocation P_1, bool P_2) => damage;
		ForceCallback force = () => 100f;
		RegisterImpact(surfaceImpact, origin, owner, damage2, force, "Knife");
		return surfaceImpact;
	}

	private void RegisterImpact(SurfaceImpact impact, Vector3 projectileOrigin, Actor owner, DamageCallback damage, ForceCallback force, string damageType)
	{
		float magnitude = (projectileOrigin - impact.position).magnitude;
		bool flag = false;
		Actor actor = null;
		if (impact.gameobject != null)
		{
			HitLocation component = impact.gameobject.GetComponent<HitLocation>();
			bool flag2 = false;
			if (owner != null && owner.behaviour.PlayerControlled)
			{
				flag2 = true;
			}
			float num = damage(magnitude, component, flag2);
			float num2 = force();
			if (owner != null)
			{
				num *= owner.realCharacter.DamageModifier;
			}
			if (num > 0f)
			{
				Destructible component2 = impact.gameobject.GetComponent<Destructible>();
				if (component2 != null)
				{
					GameObject damageDealer = null;
					if (owner != null)
					{
						damageDealer = owner.gameObject;
					}
					component2.DoBulletDamage(num, damageDealer);
				}
				RPGProjectile component3 = impact.gameobject.GetComponent<RPGProjectile>();
				if (component3 != null && flag2)
				{
					GameObject gameObject = null;
					gameObject = owner.gameObject;
					component3.DoBulletDamage(num, gameObject);
					impact.noDecal = true;
				}
				FakeGunman component4 = impact.gameobject.GetComponent<FakeGunman>();
				if (component4 != null)
				{
					impact.material = SurfaceMaterial.None;
					Container.SendMessage(impact.gameobject, "Damage");
				}
				if (component != null)
				{
					actor = component.Actor;
				}
				if (actor != null)
				{
					bool flag3 = false;
					if (owner != null && owner.behaviour != null && (owner.awareness.faction == actor.awareness.faction || (actor.awareness.faction == FactionHelper.Category.Neutral && owner.awareness.faction == FactionHelper.Category.Player)))
					{
						if (actor == owner)
						{
							Debug.LogWarning("AC: " + owner.name + " just shot himself in the " + component.name + "!");
						}
						flag3 = true;
						actor = null;
						impact.material = SurfaceMaterial.None;
					}
					if (!flag3 && actor.awareness.ChDefCharacterType != CharacterType.SentryGun)
					{
						object obj;
						if (owner != null)
						{
							IWeaponStats weaponStats = WeaponUtils.GetWeaponStats(owner.weapon.ActiveWeapon);
							obj = weaponStats;
						}
						else
						{
							obj = null;
						}
						IWeaponStats weaponStats2 = (IWeaponStats)obj;
						float sqrMagnitude = (projectileOrigin - impact.position).sqrMagnitude;
						bool longShot = weaponStats2 != null && weaponStats2.IsLongRangeShot(sqrMagnitude);
						float num3 = component.DamageMultiplier;
						bool flag4 = weaponStats2 != null && weaponStats2.IsHeadShotAllowed(sqrMagnitude);
						bool flag5 = false;
						if (component.DamageMultiplier > 1f)
						{
							if (flag4)
							{
								flag5 = true;
							}
							else
							{
								num3 = 1f;
							}
						}
						if (owner != null && owner.realCharacter != null && owner.realCharacter.IsFirstPerson)
						{
							if (!actor.baseCharacter.IsDead() && num3 > 0f)
							{
								flag = true;
							}
							if (flag5)
							{
								FirstPersonPenaliser.EventOccurred(FirstPersonPenaliser.EventEnum.FirstPersonHeadshot);
							}
							else
							{
								FirstPersonPenaliser.EventOccurred(FirstPersonPenaliser.EventEnum.FirstPersonShotEnemy);
							}
						}
						else if (owner != null && owner.behaviour != null && owner.behaviour.aimedShotTarget == actor)
						{
							FirstPersonPenaliser.EventOccurred(FirstPersonPenaliser.EventEnum.ThirdPersonAimedShot);
						}
						if (actor.realCharacter != null && actor.realCharacter.IsFirstPerson && CommonHudController.Instance != null)
						{
							CommonHudController.Instance.TakeDamage(projectileOrigin);
							actor.realCharacter.Flinch(projectileOrigin);
							if ((bool)owner && owner.realCharacter != null && owner.realCharacter.SnapTarget != null)
							{
								owner.realCharacter.SnapTarget.LastDamageTime = Time.realtimeSinceStartup;
							}
						}
						if (actor.health != null)
						{
							actor.health.ModifyHealth((!(owner != null)) ? null : owner.gameObject, (0f - num) * num3, damageType, -impact.direction, (0f - num2) * impact.direction, flag5, longShot, component, impact, SpeechComponent.SpeechMode.Normal);
						}
					}
				}
				else if (component != null && component.Health != null)
				{
					object obj2;
					if (owner != null)
					{
						IWeaponStats weaponStats = WeaponUtils.GetWeaponStats(owner.weapon.ActiveWeapon);
						obj2 = weaponStats;
					}
					else
					{
						obj2 = null;
					}
					IWeaponStats weaponStats3 = (IWeaponStats)obj2;
					float sqrMagnitude2 = (projectileOrigin - impact.position).sqrMagnitude;
					bool longShot2 = weaponStats3 != null && weaponStats3.IsLongRangeShot(sqrMagnitude2);
					if (!component.Health.HealthEmpty && owner != null && owner.realCharacter != null && owner.realCharacter.IsFirstPerson)
					{
						flag = true;
					}
					component.Health.ModifyHealth((!(owner != null)) ? null : owner.gameObject, 0f - num, "Shot", -impact.direction, num2 * impact.direction, component.DamageMultiplier > 1f, longShot2, component, impact, SpeechComponent.SpeechMode.Normal);
				}
				if (flag && CommonHudController.Instance != null && CommonHudController.Instance.Crosshair != null)
				{
					CommonHudController.Instance.Crosshair.DidDamage();
				}
				if (owner != null)
				{
					if (impact.material == SurfaceMaterial.Flesh)
					{
						AuditoryAwarenessManager.Instance.RegisterEventInWorld(impact.position, AudioResponseRanges.BulletHittingFlesh, owner);
					}
					else
					{
						AuditoryAwarenessManager.Instance.RegisterEventInWorld(impact.position, AudioResponseRanges.Gunshot, owner);
					}
				}
			}
		}
		if (owner != null && EventHub.Instance != null)
		{
			EventHub.Instance.Report(new Events.WeaponFired(owner.EventActor(), actor.EventActor(), actor != null || flag));
		}
	}

	private SurfaceImpact PickBestImpactForMelee(SurfaceImpact a, SurfaceImpact b)
	{
		return (a.material != SurfaceMaterial.Flesh) ? b : a;
	}
}
