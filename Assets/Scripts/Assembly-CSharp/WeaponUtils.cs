using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUtils
{
	private const float TEST_HEIGHT = 4900f;

	public const float shotStartOffset = 0f;

	public static GameObject[] MuzzleList;

	public static IWeaponADS GetWeaponADS(IWeapon weapon)
	{
		return weapon.QueryInterface(typeof(IWeaponADS)) as IWeaponADS;
	}

	public static IWeaponAI GetWeaponAI(IWeapon weapon)
	{
		return weapon.QueryInterface(typeof(IWeaponAI)) as IWeaponAI;
	}

	public static IWeaponControl GetWeaponControl(IWeapon weapon)
	{
		return weapon.QueryInterface(typeof(IWeaponControl)) as IWeaponControl;
	}

	public static IWeaponEquip GetWeaponEquip(IWeapon weapon)
	{
		return weapon.QueryInterface(typeof(IWeaponEquip)) as IWeaponEquip;
	}

	public static IWeaponMovement GetWeaponMovement(IWeapon weapon)
	{
		return weapon.QueryInterface(typeof(IWeaponMovement)) as IWeaponMovement;
	}

	public static IWeaponStats GetWeaponStats(IWeapon weapon)
	{
		return weapon.QueryInterface(typeof(IWeaponStats)) as IWeaponStats;
	}

	public static IWeaponSway GetWeaponSway(IWeapon weapon)
	{
		return weapon.QueryInterface(typeof(IWeaponSway)) as IWeaponSway;
	}

	public static void ApplyRecoil(BaseCharacter owner)
	{
		ApplyRecoil(owner, 2f);
	}

	public static void ApplyRecoil(BaseCharacter owner, WeaponDescriptor descriptor)
	{
		ApplyRecoil(owner, (!(owner != null) || !owner.IsFirstPerson) ? descriptor.ThirdPersonRecoil : descriptor.FirstPersonRecoil);
	}

	public static void ApplyRecoil(BaseCharacter owner, float amount)
	{
		if (owner != null)
		{
			owner.ApplyRecoil(amount);
			if (owner.FirstPersonCamera != null)
			{
				owner.FirstPersonCamera.RecoilAngles += amount * new Vector3(UnityEngine.Random.Range(-0.8f, -1f), UnityEngine.Random.Range(-0.15f, 0.15f), 0f);
			}
		}
	}

	public static GameObject CreateThirdPersonModel(GameObject owner, WeaponDescriptor descriptor)
	{
		return CreateThirdPersonWeaponModel(owner, descriptor, false);
	}

	public static GameObject CreateThirdPersonModelForCutscene(GameObject owner, WeaponDescriptor descriptor)
	{
		return CreateThirdPersonWeaponModel(owner, descriptor, true);
	}

	private static Transform FindCutsceneWeapon(GameObject owner, string tag)
	{
		Transform transform = owner.transform.FindInHierarchyStartsWith(tag);
		if (transform != null && transform.childCount > 1)
		{
			return transform;
		}
		return null;
	}

	private static GameObject CreateThirdPersonWeaponModel(GameObject owner, WeaponDescriptor descriptor, bool forCutscene)
	{
		bool flag = false;
		Transform transform = null;
		bool flag2 = false;
		string name = string.Empty;
		if (forCutscene)
		{
			if (owner != null)
			{
				transform = FindCutsceneWeapon(owner, "Weapon");
				if (transform == null)
				{
					transform = FindCutsceneWeapon(owner, "SpareWeapon");
					if (transform != null)
					{
						flag2 = true;
						Transform child = transform.GetChild(0);
						name = child.name;
					}
				}
				if (transform != null)
				{
					RemoveExistingWeaponFromObject(transform);
					flag = true;
				}
			}
		}
		else
		{
			transform = ((!(owner != null)) ? null : owner.transform.FindInHierarchy("Weapon"));
			flag = true;
		}
		if (flag)
		{
			GameObject gameObject = descriptor.ModelHiResPrefab;
			if (gameObject == null)
			{
				BufferObject bufferObject = BufferObjectHelper.LoadBufferObject(descriptor.ModelHiResName);
				if (bufferObject != null)
				{
					gameObject = bufferObject.GetGameObjects()[0];
				}
			}
			if (transform != null && descriptor != null && gameObject != null)
			{
				GameObject gameObject2 = new GameObject(descriptor.Name);
				gameObject2.transform.parent = owner.transform;
				GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				gameObject3.transform.parent = gameObject2.transform;
				gameObject3.transform.localPosition = descriptor.inHandLocalPosition;
				gameObject3.transform.localRotation = descriptor.inHandLocalRotation;
				if (flag2)
				{
					gameObject2.name = name;
					Transform transform2 = gameObject3.transform;
					int childCount = transform2.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform child2 = transform2.GetChild(i);
						if (child2 != null)
						{
							child2.localRotation = Quaternion.identity;
						}
					}
				}
				ActorAttachment actorAttachment = gameObject2.AddComponent<ActorAttachment>();
				actorAttachment.AttachPoint = transform.gameObject.transform;
				actorAttachment.Model = gameObject3;
				actorAttachment.HitBoxRig = descriptor.HitBoxRig;
				actorAttachment.UpdateAttachment();
				return gameObject2;
			}
		}
		return SceneNanny.NewGameObject("DummyWeapon");
	}

	public static float CalculateHipsToSightsBlend(ADSState state, float blendTime, float blendDuration)
	{
		switch (state)
		{
		case ADSState.Hips:
			return 0f;
		case ADSState.ADS:
			return 1f;
		case ADSState.SwitchingToADS:
			return Mathf.Clamp01(blendTime / blendDuration);
		case ADSState.SwitchingToHips:
			return 1f - Mathf.Clamp01(blendTime / blendDuration);
		default:
			return 1f;
		}
	}

	public static float CalculateStandardUtility(float distance, WeaponDescriptor descriptor)
	{
		WeaponDescriptor.UtilityRange[] utilityRanges = descriptor.UtilityRanges;
		if (utilityRanges == null || utilityRanges.Length == 0)
		{
			return 1f;
		}
		float utility = utilityRanges[0].Utility;
		float range = utilityRanges[0].Range;
		foreach (WeaponDescriptor.UtilityRange utilityRange in utilityRanges)
		{
			float closeDamage = utility;
			float closeRange = range;
			utility = utilityRange.Utility;
			range = utilityRange.Range;
			if (distance < utilityRange.Range)
			{
				return CalculateStandardDamage(distance, closeRange, closeDamage, range, utility, 1f);
			}
		}
		return utility;
	}

	public static float CalculateStandardDamage(float distance, WeaponDescriptor descriptor, WeaponDescriptor.WeaponClass weaponClass, bool isPlayer)
	{
		return CalculateStandardDamage(distance, descriptor.DamageRanges, weaponClass, isPlayer);
	}

	public static float CalculateStandardDamage(float distance, WeaponDescriptor.DamageRange[] ranges, WeaponDescriptor.WeaponClass weaponClass, bool isPlayer)
	{
		if (ranges.Length == 0)
		{
			return 0f;
		}
		float multiplier = 1f;
		if (isPlayer && GameSettings.Instance.PerksEnabled)
		{
			switch (weaponClass)
			{
			case WeaponDescriptor.WeaponClass.SniperRifle:
				multiplier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.Sniper);
				break;
			case WeaponDescriptor.WeaponClass.AssaultRifle:
				multiplier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.Rifleman);
				break;
			case WeaponDescriptor.WeaponClass.LightMachineGun:
				multiplier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.MachineGunner);
				break;
			case WeaponDescriptor.WeaponClass.Shotgun:
				multiplier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.Shotgunner);
				break;
			case WeaponDescriptor.WeaponClass.SubMachineGun:
				multiplier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.SMGOperative);
				break;
			}
		}
		float damage = ranges[0].Damage;
		float range = ranges[0].Range;
		foreach (WeaponDescriptor.DamageRange damageRange in ranges)
		{
			float closeDamage = damage;
			float closeRange = range;
			damage = damageRange.Damage;
			range = damageRange.Range;
			if (distance < damageRange.Range)
			{
				return CalculateStandardDamage(distance, closeRange, closeDamage, range, damage, multiplier);
			}
		}
		return damage;
	}

	public static float CalculateStandardDamage(float distance, float closeRange, float closeDamage, float farRange, float farDamage, float multiplier)
	{
		float t = Mathf.InverseLerp(closeRange, farRange, distance);
		return Mathf.Lerp(closeDamage, farDamage, t) * multiplier;
	}

	public static float CalculateStandardFieldOfView(float blendTime, float adsFOV)
	{
		return Mathf.Lerp(InputSettings.FirstPersonFieldOfView, adsFOV, blendTime);
	}

	public static float CalculateRunSpeed(WeaponDescriptor descriptor, float adsBlend, bool playerControlled)
	{
		float num = ((!playerControlled || !GameSettings.Instance.PerksEnabled || !GameSettings.Instance.HasPerk(PerkType.LightWeight)) ? descriptor.RunSpeed : 100f);
		float to = ((!playerControlled || !GameSettings.Instance.PerksEnabled) ? (1f - descriptor.ADSRunSpeedModifier) : (1f - descriptor.ADSRunSpeedModifier * StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.Stalker)));
		float num2 = Mathf.Lerp(1f, to, adsBlend);
		return num * num2;
	}

	public static string FormatAmmoString(int ammoRemaining, int ammoStash, bool isReloading, bool isUnlimited)
	{
		return string.Format("{0:D2} / {1}", (!isReloading) ? ammoRemaining : 0, (!isUnlimited) ? ammoStash.ToString() : "-");
	}

	public static Vector3 GetFirstPersonShotDirection(Actor firstPersonActor, float spread)
	{
		Vector3 crosshairForward = ViewModelRig.Instance().GetCrosshairForward();
		float accuracy = GetAccuracy(firstPersonActor, 0.5f);
		return 100f * (100f * crosshairForward.normalized + spread * Vector3.Lerp(UnityEngine.Random.insideUnitSphere, Vector3.zero, accuracy));
	}

	public static Vector3 GetFirstPersonShotDirection(Actor firstPersonActor, Vector3 baseDirection, float spread)
	{
		float accuracy = GetAccuracy(firstPersonActor, 0f);
		return 100f * (100f * baseDirection.normalized + spread * Vector3.Lerp(UnityEngine.Random.insideUnitSphere, Vector3.zero, accuracy));
	}

	public static Vector3 GetFirstPersonShotDirection(Vector3 baseDirection, float spread)
	{
		return 100f * (100f * baseDirection.normalized + spread * UnityEngine.Random.insideUnitSphere);
	}

	public static void TriggerProjectileEffects(BaseCharacter owner, Vector3 origin, SurfaceImpact impact, bool isTracer)
	{
		if (isTracer)
		{
			EffectsController.TracerType type = EffectsController.TracerType.Friendly;
			bool allowAudio = true;
			if (owner != null && owner.myActor != null && owner.myActor.realCharacter != null)
			{
				if (!owner.myActor.realCharacter.IsFirstPerson && (owner.myActor.awareness.faction == FactionHelper.Category.Enemy || owner.myActor.awareness.faction == FactionHelper.Category.SoloEnemy))
				{
					type = EffectsController.TracerType.Enemy;
				}
				allowAudio = !owner.IsFirstPerson;
			}
			EffectsController.Instance.GetTracer(origin, impact.position, type, impact, allowAudio);
		}
		else if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.ImpactDecals) || owner.IsFirstPerson)
		{
			EffectsController.Instance.TriggerSurfaceImpact(impact);
		}
	}

	public static void TriggerProjectileEffects(BaseCharacter owner, Vector3 origin, SurfaceImpact impact, EffectsController.TracerType tracerType)
	{
		if (tracerType != 0)
		{
			bool allowAudio = owner == null || !owner.IsFirstPerson;
			EffectsController.Instance.GetTracer(origin, impact.position, tracerType, impact, allowAudio);
		}
		else
		{
			EffectsController.Instance.TriggerSurfaceImpact(impact);
		}
	}

	public static void AddMuzzleToList(GameObject muzzleObj)
	{
		if (MuzzleList != null)
		{
			for (int i = 0; i < MuzzleList.Length; i++)
			{
				if (MuzzleList[i] == null)
				{
					MuzzleList[i] = muzzleObj;
					return;
				}
			}
			GameObject[] array = new GameObject[MuzzleList.Length + 4];
			Array.Copy(MuzzleList, array, MuzzleList.Length);
			array[MuzzleList.Length] = muzzleObj;
			MuzzleList = array;
		}
		else
		{
			MuzzleList = new GameObject[4];
			MuzzleList[0] = muzzleObj;
		}
	}

	public static GameObject GetFreeMuzzle(GameObject effect)
	{
		if (MuzzleList != null)
		{
			for (int i = 0; i < MuzzleList.Length; i++)
			{
				if (MuzzleList[i] != null && !MuzzleList[i].activeSelf && MuzzleList[i].name.GetHashCode() == effect.name.GetHashCode())
				{
					MuzzleList[i].SetActive(true);
					return MuzzleList[i];
				}
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(effect) as GameObject;
		gameObject.name = effect.name;
		gameObject.AddComponent<MuzzleEffectHelper>();
		Behaviour behaviour = gameObject.GetComponent("TimedFxDestroy") as Behaviour;
		if (behaviour != null)
		{
			UnityEngine.Object.Destroy(behaviour);
		}
		AddMuzzleToList(gameObject);
		return gameObject;
	}

	public static void CreateMuzzleFlash(Transform locator, GameObject effect)
	{
		if ((!(locator != null) || !(locator.position.y > 4900f)) && locator != null)
		{
			GameObject freeMuzzle = GetFreeMuzzle(effect);
			freeMuzzle.transform.parent = locator;
			freeMuzzle.transform.localPosition = Vector3.zero;
			freeMuzzle.transform.localRotation = Quaternion.identity;
		}
	}

	public static CasingEffect CreateCasingEffect(Transform locator, GameObject effect)
	{
		ParticleEmitter particleEmitter = null;
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.BulletCasings) && locator != null && effect != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(effect) as GameObject;
			gameObject.transform.parent = locator;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			particleEmitter = gameObject.GetComponent<ParticleEmitter>();
			if (particleEmitter != null)
			{
				particleEmitter.emit = false;
			}
			gameObject.SetActive(true);
		}
		return new CasingEffect(particleEmitter);
	}

	public static bool IsLongRangeShot(WeaponDescriptor descriptor, float distSquared)
	{
		if (descriptor.Range == 0f)
		{
			return false;
		}
		float num = descriptor.Range * descriptor.LongRange;
		return distSquared > num * num;
	}

	public static bool IsHeadShotAllowed(WeaponDescriptor descriptor, float distSquared)
	{
		float num = descriptor.HeadShotMaxRange * descriptor.HeadShotMaxRange;
		return distSquared <= num;
	}

	public static bool IsWeaponSilenced(WeaponDescriptor descriptor)
	{
		return descriptor.name == "Ballista" || descriptor.name == "SVU-AS" || descriptor.name == "XPR-50";
	}

	public static void FeelTheWind(BaseCharacter owner, SurfaceImpact impact)
	{
		if (owner == null || owner.myActor.weapon == null || owner.myActor.weapon.ActiveWeapon == null || !owner.myActor.behaviour.PlayerControlled)
		{
			return;
		}
		bool flag = true;
		float num;
		if (owner.myActor.behaviour.suppressionTarget == null)
		{
			flag = false;
			num = 4f;
		}
		else
		{
			IWeaponAI weaponAI = GetWeaponAI(owner.myActor.weapon.ActiveWeapon);
			num = ((weaponAI == null) ? 0f : weaponAI.GetSuppressionRadius());
			num *= num;
		}
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.EnemiesMask(owner.myActor));
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if ((impact.position - a.GetPosition()).sqrMagnitude > num)
			{
				Vector3 lhs = impact.position - owner.myActor.GetPosition();
				Vector3 rhs = a.GetPosition() - owner.myActor.GetPosition();
				float sqrMagnitude = rhs.sqrMagnitude;
				if (lhs.sqrMagnitude < sqrMagnitude)
				{
					continue;
				}
				float num2 = Vector3.Dot(lhs, rhs);
				if (num2 <= 0f)
				{
					continue;
				}
				float num3 = lhs.x * rhs.z - lhs.z * rhs.x;
				float num4 = num3 / num2;
				float num5 = num4 * num4;
				if (sqrMagnitude * num5 > num)
				{
					continue;
				}
			}
			if (flag)
			{
				a.behaviour.Suppressed = true;
				a.realCharacter.HudMarker.Suppressed();
			}
			TargetScorer.RecordAttack(owner.myActor, a);
			a.realCharacter.AttackedBy(owner.myActor);
		}
	}

	private static float GetAccuracy(Actor actor, float defaultAccuracy)
	{
		if (actor.realCharacter != null)
		{
			if (actor.realCharacter.IsFirstPerson && GameController.Instance.FirstPersonAccuracyCheat)
			{
				return 1f;
			}
			return actor.realCharacter.GetCurrentAccuracy();
		}
		return defaultAccuracy;
	}

	private static void RemoveExistingWeaponFromObject(Transform obj)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < obj.childCount; i++)
		{
			Transform child = obj.GetChild(i);
			if (!child.name.StartsWith("Bone"))
			{
				list.Add(child.gameObject);
			}
		}
		foreach (GameObject item in list)
		{
			UnityEngine.Object.Destroy(item);
		}
	}
}
