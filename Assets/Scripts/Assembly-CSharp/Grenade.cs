using System;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		Fake = 1,
		ImpactBroadcast = 2,
		ThrownBack = 4
	}

	public float DetonationTime;

	public Material Arc;

	public Material Trail;

	public float ArcTailTime;

	public float TrailTailTime;

	public float MinimumThrowDistance = 3f;

	public float ArcAngleMin = 45f;

	public float ArcAngleMax = 80f;

	public float DamageRadius = 7.5f;

	public float DistanceRange = 25f;

	public static List<Grenade> GlobalPoolCache = new List<Grenade>();

	private float mMinimumThrowDistanceSq;

	private float mDamageRadiusSq;

	private float mDistanceRangeSq;

	private Actor mOwner;

	private float mTimeToDetonation;

	private Flags mFlags;

	private Vector3 mOrigin;

	private CharacterType mOwnerCharacterType;

	private bool mOwnerWasPlayerControlled;

	private Vector3 mDebugStart;

	private Vector3 mDebugDirection;

	private HaloEffect mHalo;

	private bool mHasSweepTestCollided;

	public float MinimumThrowDistanceSquared
	{
		get
		{
			return mMinimumThrowDistanceSq;
		}
	}

	public float DamageRadiusSquared
	{
		get
		{
			return mDamageRadiusSq;
		}
	}

	public float DistanceRangeSquared
	{
		get
		{
			return mDistanceRangeSq;
		}
	}

	public bool MarkForDelete { private get; set; }

	public bool ThrownBack
	{
		get
		{
			return (mFlags & Flags.ThrownBack) != 0;
		}
		set
		{
			if (value)
			{
				mFlags |= Flags.ThrownBack;
			}
			else
			{
				mFlags &= ~Flags.ThrownBack;
			}
		}
	}

	public Actor Owner
	{
		get
		{
			return mOwner;
		}
	}

	private void Awake()
	{
		GlobalPoolCache.Add(this);
		mMinimumThrowDistanceSq = MinimumThrowDistance * MinimumThrowDistance;
		mDamageRadiusSq = DamageRadius * DamageRadius;
		mDistanceRangeSq = DistanceRange * DistanceRange;
	}

	private void Start()
	{
		mHalo = GetComponentInChildren<HaloEffect>();
	}

	private void Update()
	{
		if ((mFlags & Flags.Fake) == 0)
		{
			InteractionsManager instance = InteractionsManager.Instance;
			if (MarkForDelete || (instance != null && instance.IsPlayingCutscene()))
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (!mHasSweepTestCollided && base.GetComponent<Rigidbody>() != null && base.GetComponent<Rigidbody>().velocity.sqrMagnitude > 100f)
			{
				Vector3 normalized = base.GetComponent<Rigidbody>().velocity.normalized;
				RaycastHit hitInfo;
				if (base.GetComponent<Rigidbody>().SweepTest(normalized, out hitInfo, base.GetComponent<Rigidbody>().velocity.magnitude * Time.fixedDeltaTime))
				{
					base.GetComponent<Rigidbody>().velocity = hitInfo.normal;
					ProcessCollision();
					mHasSweepTestCollided = true;
				}
			}
			mTimeToDetonation -= Time.deltaTime;
			if (mTimeToDetonation <= 0f)
			{
				Explode();
			}
		}
		if (!mHalo)
		{
		}
	}

	private void OnDestroy()
	{
		GlobalPoolCache.Remove(this);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(mDebugStart, mDebugStart + mDebugDirection * 3f);
	}

	private void DoGrenadeBounceSfx()
	{
		Vector3 position = base.transform.position;
		SurfaceImpact surfaceImpact = ProjectileManager.Trace(position, position + Vector3.down, ProjectileManager.DefaultLayerProjectileMask);
		SoundFXData soundFXData = ((surfaceImpact.material != SurfaceMaterial.Water) ? ExplosivesSFX.Instance.GrenadeBounce : ExplosivesSFX.Instance.GrenadeBounceWater);
		soundFXData.Play(base.gameObject);
	}

	private void OnCollisionEnter(Collision other)
	{
		ProcessCollision();
	}

	private void ProcessCollision()
	{
		if ((mFlags & Flags.Fake) == 0)
		{
			DoGrenadeBounceSfx();
		}
		CommonHudController.Instance.GrenadeLanded(this);
		if ((mFlags & Flags.Fake) != 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if ((mFlags & Flags.ImpactBroadcast) == 0)
		{
			Projectile.BroadcastImpact(this, mDamageRadiusSq, Projectile.ProjectileType.Grenade);
			mFlags |= Flags.ImpactBroadcast;
		}
	}

	public void Launch(Actor owner, Vector3 target, bool isFake, bool isAllowed)
	{
		if (isFake)
		{
			mFlags |= Flags.Fake;
			base.GetComponent<Renderer>().enabled = false;
		}
		TrailRenderer component = GetComponent<TrailRenderer>();
		TBFAssert.DoAssert(component != null, "Grenade prefab should have a Trail Renderer");
		if ((mFlags & Flags.Fake) != 0)
		{
			TBFAssert.DoAssert(Arc != null, "Grenade prefab should have an Arc Material specified");
			Color color = ((!isAllowed) ? ColourChart.GrenadeCancel : ColourChart.GrenadeThrow);
			Arc.SetColor("_Color", color);
			component.material = Arc;
			component.time = ArcTailTime;
		}
		else
		{
			TBFAssert.DoAssert(Trail != null, "Grenade prefab should have a Trail Material specified");
			component.material = Trail;
			component.time = TrailTailTime;
		}
		mTimeToDetonation = DetonationTime;
		mOwner = owner;
		if (mOwner != null)
		{
			mOwnerCharacterType = mOwner.awareness.ChDefCharacterType;
			mOwnerWasPlayerControlled = mOwner.behaviour.PlayerControlled;
			if (mOwner.GetComponent<Collider>() != null)
			{
				Physics.IgnoreCollision(base.GetComponent<Collider>(), mOwner.GetComponent<Collider>());
			}
			if (mOwner.realCharacter.SimpleHitBounds != null)
			{
				Physics.IgnoreCollision(base.GetComponent<Collider>(), mOwner.realCharacter.SimpleHitBounds.GetComponent<Collider>());
			}
		}
		SetThrowingPosition(mOwner);
		mDebugStart = base.transform.position;
		Vector3 vector = target - base.transform.position;
		float magnitude = vector.magnitude;
		if (magnitude > 0f)
		{
			vector.Normalize();
		}
		else
		{
			vector = Vector3.up;
		}
		mDebugDirection = vector;
		Throw(target);
	}

	public void LaunchFromFirstPerson(Actor owner, Vector3 direction, float cookedTime)
	{
		TrailRenderer component = GetComponent<TrailRenderer>();
		TBFAssert.DoAssert(component != null, "Grenade prefab should have a Trail Renderer");
		component.material = Trail;
		component.time = TrailTailTime;
		mTimeToDetonation = DetonationTime - cookedTime;
		mOwner = owner;
		if (mOwner != null)
		{
			mOwnerCharacterType = mOwner.awareness.ChDefCharacterType;
			mOwnerWasPlayerControlled = mOwner.behaviour.PlayerControlled;
			if (mOwner.GetComponent<Collider>() != null)
			{
				Physics.IgnoreCollision(base.GetComponent<Collider>(), mOwner.GetComponent<Collider>());
			}
			if (mOwner.realCharacter.SimpleHitBounds != null)
			{
				Physics.IgnoreCollision(base.GetComponent<Collider>(), mOwner.realCharacter.SimpleHitBounds.GetComponent<Collider>());
			}
		}
		SetThrowingPosition(mOwner);
		mDebugStart = base.transform.position;
		mDebugDirection = direction;
		if (base.GetComponent<Rigidbody>().IsSleeping())
		{
			base.GetComponent<Rigidbody>().WakeUp();
		}
		float num = 20f;
		base.GetComponent<Rigidbody>().velocity = num * direction;
	}

	public void Throw(Vector3 target)
	{
		if (base.GetComponent<Rigidbody>().IsSleeping())
		{
			base.GetComponent<Rigidbody>().WakeUp();
		}
		float num = 1f;
		float num2 = 1f;
		if (mOwner != null && mOwner.awareness.ChDefCharacterType == CharacterType.RiotShieldNPC)
		{
			num = 2f;
			num2 = 2f;
		}
		Vector3 velocity = new Vector3((target.x - base.transform.position.x) * num2 / num, (target.y + 0.5f * (0f - Physics.gravity.y) * (num * num) - base.transform.position.y) / num, (target.z - base.transform.position.z) * num2 / num);
		base.GetComponent<Rigidbody>().velocity = velocity;
	}

	public void SetThrowingPosition(Actor owner)
	{
		if (owner.realCharacter != null && owner.realCharacter.IsFirstPerson)
		{
			mOrigin = owner.realCharacter.FirstPersonCamera.transform.position + owner.realCharacter.FirstPersonCamera.transform.forward * 1f + owner.realCharacter.FirstPersonCamera.transform.right * 0.5f;
			SurfaceImpact surfaceImpact = ProjectileManager.Trace(owner.realCharacter.FirstPersonCamera.transform.position, mOrigin, ProjectileManager.ProjectileMask);
			if (surfaceImpact.gameobject != null)
			{
				mOrigin = owner.realCharacter.FirstPersonCamera.transform.position;
			}
		}
		else
		{
			mOrigin = owner.transform.position;
			mOrigin.y += 1.5f;
		}
		base.transform.position = mOrigin;
	}

	public Vector3 GetFacing()
	{
		Vector3 vector = base.transform.position - mOrigin;
		if (vector.sqrMagnitude == 0f)
		{
			return base.transform.forward;
		}
		return vector.normalized;
	}

	private void Explode()
	{
		TBFAssert.DoAssert((mFlags & Flags.Fake) == 0, "Fake Grenades shouldn't be exploding!");
		float num = 1f;
		float num2 = 1f;
		if (mOwnerCharacterType == CharacterType.RiotShieldNPC)
		{
			num = mOwner.realCharacter.GrenadeDamageMultiplier;
		}
		else if (mOwnerWasPlayerControlled && GameSettings.Instance.PerksEnabled)
		{
			num = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.SonicBoom);
			num2 = num;
		}
		bool flag = !(ActStructure.Instance != null) || ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran;
		RulesSystem.DoAreaOfEffectDamage(base.transform.position, DamageRadius * num2, num, (!mOwner) ? null : mOwner.gameObject, ExplosionManager.ExplosionType.Grenade, "Grenade", mOwnerWasPlayerControlled && flag);
		ExplosionManager.BroadcastNoise(base.transform.position, mOwner);
		Projectile.BroadcastExplosion(this, mOwner);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
