using System;
using System.Collections;
using UnityEngine;

public class RPGProjectile : MonoBehaviour
{
	[Flags]
	private enum Flags
	{
		Default = 0
	}

	public const float kFireYOffset = 1.6f;

	private const float kParticleWaitTime = 3f;

	public float DetonationTime = 5f;

	public float LaunchSpeed = 50f;

	public float DamageRadius = 7.5f;

	public float Acceleration = 12f;

	public float DistanceToStartCollision = 2f;

	public float DamageMultiplier = 6f;

	public bool CanTakeBulletDamage = true;

	public float RandomProjectileScale = 0.1f;

	public ParticleSystem TrailParticles;

	public HudBlipIcon HudMarker;

	public float Health = 10f;

	public float ToughnessValue = 1f;

	private GameObject mOwner;

	private Helicopter Heli;

	private Flags mFlags;

	private Vector3 mOrigin;

	private HudBlipIcon mHudBlip;

	private float mDistanceFromStartSq;

	private float mTimeToDetonation;

	private float mDamageRadiusSq;

	private float mDistanceToStartCollisionSq;

	private bool mExploded = true;

	private Vector3 mDebugStart;

	private Vector3 mDebugDirection;

	private Vector3 mDebugForce;

	private float mLaunchSpeed;

	private Vector3 mOverriddenLaunchPosition;

	public GameObject Owner
	{
		get
		{
			return mOwner;
		}
	}

	private void Awake()
	{
		mDamageRadiusSq = DamageRadius * DamageRadius;
		OverrideDistanceToStartCollision(DistanceToStartCollision);
		mLaunchSpeed = LaunchSpeed;
		mExploded = false;
	}

	public void OnDestroy()
	{
	}

	private void Update()
	{
		if (mExploded)
		{
			return;
		}
		Vector3 force = GetFacing() * Acceleration * (Time.deltaTime * 50f);
		base.GetComponent<Rigidbody>().AddForce(force);
		if (!base.GetComponent<Rigidbody>().GetComponent<Collider>().enabled)
		{
			float sqrMagnitude = (mOrigin - base.transform.position).sqrMagnitude;
			if (sqrMagnitude >= mDistanceToStartCollisionSq)
			{
				base.GetComponent<Rigidbody>().GetComponent<Collider>().enabled = true;
			}
		}
		mTimeToDetonation -= Time.deltaTime;
		if (mTimeToDetonation <= 0f)
		{
			Explode();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(mDebugStart, mDebugStart + mDebugDirection * 3f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(mDebugStart, mDebugStart + mDebugForce * 3f);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!mExploded)
		{
			Projectile.BroadcastImpact(this, mDamageRadiusSq, Projectile.ProjectileType.RPG);
			Explode();
		}
	}

	public void Launch(GameObject owner, Vector3 aimDir, bool applyFireYOffset)
	{
		mTimeToDetonation = DetonationTime;
		mOwner = owner;
		if (mOwner != null && mOwner.transform.parent != null)
		{
			Heli = mOwner.transform.parent.GetComponentInChildren<Helicopter>();
		}
		AddBlip(base.GetComponent<Rigidbody>().gameObject);
		if (mOwner != null)
		{
		}
		SetThrowingPosition(mOwner, applyFireYOffset);
		base.transform.forward = aimDir;
		mDebugStart = base.transform.position;
		Vector3 vector = aimDir;
		float magnitude = aimDir.magnitude;
		if (magnitude > 0f)
		{
			aimDir.Normalize();
		}
		else
		{
			aimDir = Vector3.left;
		}
		mDebugDirection = vector;
		Throw(aimDir, mLaunchSpeed);
		base.GetComponent<Rigidbody>().GetComponent<Collider>().enabled = false;
		if (TrailParticles != null)
		{
			TrailParticles.gameObject.SetActive(true);
			TrailParticles.Play();
		}
	}

	public void Throw(Vector3 direction, float speed)
	{
		if (base.GetComponent<Rigidbody>().IsSleeping())
		{
			base.GetComponent<Rigidbody>().WakeUp();
		}
		Vector3 force = direction * speed;
		base.GetComponent<Rigidbody>().AddForce(force);
		mDebugForce = force;
	}

	public void SetThrowingPosition(GameObject owner, bool applyFireYOffset)
	{
		if (mOverriddenLaunchPosition != Vector3.zero)
		{
			base.transform.position = (mOrigin = mOverriddenLaunchPosition);
			return;
		}
		mOrigin = owner.transform.position;
		if (applyFireYOffset)
		{
			mOrigin.y += 1.6f;
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

	public void OverrideLaunchSpeed(float speed)
	{
		mLaunchSpeed = speed;
	}

	public void OverrideDamageMultiplier(float mult)
	{
		DamageMultiplier = mult;
	}

	public void OverrideDistanceToStartCollision(float distance)
	{
		mDistanceToStartCollisionSq = distance * distance;
	}

	public void OverrideLaunchPosition(Vector3 position)
	{
		mOverriddenLaunchPosition = position;
	}

	private void Explode()
	{
		if (!mExploded)
		{
			if ((bool)mHudBlip)
			{
				UnityEngine.Object.Destroy(mHudBlip);
			}
			float num = 1f;
			RulesSystem.DoAreaOfEffectDamage(base.transform.position, DamageRadius * num, DamageMultiplier, (!mOwner) ? null : mOwner.gameObject, ExplosionManager.ExplosionType.RPG, "RPG");
			Actor owner = null;
			if (mOwner != null)
			{
				owner = mOwner.GetComponent<Actor>();
			}
			ExplosionManager.BroadcastNoise(base.transform.position, owner);
			Projectile.BroadcastExplosion(this, owner);
			MeshRenderer componentInChildren = GetComponentInChildren<MeshRenderer>();
			if (componentInChildren != null)
			{
				componentInChildren.enabled = false;
			}
			if ((bool)TrailParticles)
			{
				TrailParticles.enableEmission = false;
			}
			base.GetComponent<Rigidbody>().Sleep();
			StartCoroutine(WaitForParticlesThenDestroy(3f));
			mExploded = true;
			if (Heli != null)
			{
				Heli.RocketCounter(1);
			}
		}
	}

	public void DoBulletDamage(float damageToTake, GameObject damageDealer)
	{
		if (CanTakeBulletDamage && damageDealer != null && Health > 0f)
		{
			Health -= damageToTake * ToughnessValue;
			if (Health <= 0f)
			{
				Explode();
			}
		}
	}

	private void AddBlip(GameObject go)
	{
		if (CameraManager.Instance.ActiveCamera != CameraManager.ActiveCameraType.StrategyCamera)
		{
			if (ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran)
			{
				mHudBlip = SceneNanny.Instantiate(HudMarker) as HudBlipIcon;
				mHudBlip.Target = go.transform;
				mHudBlip.OffsetTarget = null;
			}
			else
			{
				mHudBlip = null;
			}
		}
	}

	private IEnumerator WaitForParticlesThenDestroy(float delay)
	{
		yield return new WaitForSeconds(delay);
		UnityEngine.Object.Destroy(base.gameObject);
		yield return null;
	}
}
