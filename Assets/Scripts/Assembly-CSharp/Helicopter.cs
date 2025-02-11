using System;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
	public HelicopterData m_Interface;

	public Transform GunNozzle;

	public HelicopterRoutine Routine;

	public float FleeReturnSpeedMultiplier = 1f;

	public float GunTrackingSpeed = 1f;

	public float MachineGunDamage = 40f;

	public int BulletsInSpread = 1;

	public float TimeBetweenRockets = 6f;

	public float Health = 20000f;

	public float HealthDamageFleeDelta = 0.33f;

	public bool OverWatchHeli;

	public Animation AnimationPlayer;

	public AnimationClip HoverClip;

	public AnimationClip ForwardClip;

	public AnimationClip BackClip;

	public AnimationClip LeftClip;

	public AnimationClip RightClip;

	public AnimationClip DeathClip;

	public HitBoxDescriptor HitBoxRig;

	public GameObject[] ActivateOnFlee;

	public Transform LightingAnchor;

	public HudBlipIcon HudMarker;

	public GameObject DamageEffect;

	public Vector3 DamageEffectOffset = new Vector3(0f, 3f, 1f);

	public float DamageEffectHealthPercent = 10f;

	public GameObject DamageEffectAfterFlee;

	public Vector3 DamageEffectAfterFleeOffset = new Vector3(0f, 3f, 1f);

	public static List<Helicopter> GlobalPoolCache = new List<Helicopter>();

	private static float VELOCITY_MAX = 5f;

	private static Vector3 ROCKET_FIRE_LOCAL_OFFSET = new Vector3(2.5f, 0.5f, -0.75f);

	private HealthComponent mHealth;

	private List<float> mHealthCallbackThresholds;

	private int ActiveRocketsCount;

	private IWeapon mMachineGun;

	private GameObject mLaserSight;

	private float mTimeToNextShot;

	private Vector3 mAwakePosition;

	private Quaternion mAwakeRotation;

	private Quaternion mAwakeAnimationPlayerRotation;

	private Transform mAwakeAPNodeHind;

	private Vector3 mAwakeAPNodeHindPosition;

	private Quaternion mAwakeAPNodeHindRotation;

	private Vector3 mLastPosition;

	private Vector3 mVelocity;

	private Vector3 mCurrentTargetPosition;

	private AnimationState mHoverAnimation;

	private AnimationState mForwardAnimation;

	private AnimationState mBackAnimation;

	private AnimationState mLeftAnimation;

	private AnimationState mRightAnimation;

	private AnimationState mDeathAnimation;

	private float mForwardAmount;

	private float mRightAmount;

	private float mExplosionTimer;

	private float mExplosionBlend;

	private float mHoverAnimationDeathWeight;

	private float mHoverAnimationForwardWeight;

	private float mHoverAnimationBackWeight;

	private float mHoverAnimationLeftWeight;

	private float mHoverAnimationRightWeight;

	private bool mDamageSFXStarted;

	private CharacterLighting mLighting;

	private SnapTarget mSnapTarget;

	private float mRocketInaccuracy;

	private HudBlipIcon mHudMarker;

	private GameObject mDamageEffect;

	private GameObject mDamageEffectAfterFlee;

	private bool mHasExplosionEffectStarted;

	private bool mFiringFromRight;

	public Vector3 GunTarget { get; private set; }

	private void Awake()
	{
		GlobalPoolCache.Add(this);
		mAwakePosition = base.transform.position;
		mAwakeRotation = base.transform.rotation;
		mAwakeAnimationPlayerRotation = AnimationPlayer.transform.localRotation;
		mAwakeAPNodeHind = AnimationPlayer.transform.GetChild(0);
		mAwakeAPNodeHindPosition = mAwakeAPNodeHind.localPosition;
		mAwakeAPNodeHindRotation = mAwakeAPNodeHind.localRotation;
		if (Routine != null)
		{
			Routine.Owner = this;
		}
		SetupHitBox();
		SetupLighting();
		SetupBlip();
	}

	private void Start()
	{
		mMachineGun = new Weapon_HelicopterMachineGun(this);
		GunNozzle.transform.parent = AnimationPlayer.gameObject.transform;
		mLaserSight = UnityEngine.Object.Instantiate(EffectsController.Instance.Effects.LaserSight) as GameObject;
		mLaserSight.transform.parent = GunNozzle;
		mLaserSight.transform.localPosition = Vector3.zero;
		mLaserSight.transform.forward = GunNozzle.forward;
		if (DamageEffect != null)
		{
			mDamageEffect = UnityEngine.Object.Instantiate(DamageEffect) as GameObject;
			mDamageEffect.transform.parent = AnimationPlayer.gameObject.transform;
			mDamageEffect.transform.localPosition = DamageEffectOffset;
		}
		if (DamageEffectAfterFlee != null)
		{
			mDamageEffectAfterFlee = UnityEngine.Object.Instantiate(DamageEffectAfterFlee) as GameObject;
			mDamageEffectAfterFlee.transform.parent = AnimationPlayer.gameObject.transform;
			mDamageEffectAfterFlee.transform.localPosition = DamageEffectAfterFleeOffset;
		}
		GunTarget = GunNozzle.position + GunNozzle.forward * 100f;
		if (AnimationPlayer != null && HoverClip != null)
		{
			AnimationPlayer.AddClip(HoverClip, "Hover");
			mHoverAnimation = AnimationPlayer["Hover"];
			mHoverAnimation.wrapMode = WrapMode.Loop;
			AnimationPlayer.AddClip(ForwardClip, "Forward");
			mForwardAnimation = AnimationPlayer["Forward"];
			mForwardAnimation.wrapMode = WrapMode.Loop;
			AnimationPlayer.AddClip(BackClip, "Back");
			mBackAnimation = AnimationPlayer["Back"];
			mBackAnimation.wrapMode = WrapMode.Loop;
			AnimationPlayer.AddClip(LeftClip, "Left");
			mLeftAnimation = AnimationPlayer["Left"];
			mLeftAnimation.wrapMode = WrapMode.Loop;
			AnimationPlayer.AddClip(RightClip, "Right");
			mRightAnimation = AnimationPlayer["Right"];
			mRightAnimation.wrapMode = WrapMode.Loop;
			AnimationPlayer.AddClip(DeathClip, "Death");
			mDeathAnimation = AnimationPlayer["Death"];
			mDeathAnimation.wrapMode = WrapMode.Loop;
		}
		if (ActivateOnFlee != null)
		{
			GameObject[] activateOnFlee = ActivateOnFlee;
			foreach (GameObject item in activateOnFlee)
			{
				Routine.ActivateOnFlee.Add(item);
			}
		}
		ResetState();
	}

	private void OnDestroy()
	{
		if (mHealth != null)
		{
			mHealth.OnHealthEmpty -= OnHealthEmpty;
			mHealth.OnHealthChange -= OnHealthChange;
		}
		DestroySnapTarget();
		GlobalPoolCache.Remove(this);
		if (SetPieceSFX.HasInstance)
		{
			SetPieceSFX.Instance.HindEngine.Stop(base.gameObject);
			SetPieceSFX.Instance.HindDamaged.Stop(base.gameObject);
			StopMachineGunSfx();
		}
	}

	private void Update()
	{
		if (!AnimationPlayer.gameObject.activeSelf)
		{
			if (Routine == null || Routine.IsSpawned())
			{
				SetPieceSFX.Instance.HindEngine.Play(base.gameObject);
				AnimationPlayer.gameObject.SetActive(true);
				SetupSnapTarget();
				if (mHudMarker != null)
				{
					mHudMarker.SwitchOn();
				}
			}
		}
		else
		{
			mHoverAnimation.enabled = true;
			mForwardAnimation.enabled = true;
			mBackAnimation.enabled = true;
			mLeftAnimation.enabled = true;
			mRightAnimation.enabled = true;
			mDeathAnimation.enabled = true;
		}
		if (mMachineGun != null)
		{
			mMachineGun.ReleaseTrigger();
			if (Routine != null)
			{
				if (Routine.IsExploding())
				{
					if (!mHasExplosionEffectStarted)
					{
						ExplosionManager.Instance.StartExplosion(AnimationPlayer.transform.position, 500f, ExplosionManager.ExplosionType.Helicopter);
						ExplosivesSFX.Instance.HelicopterExplode.Play(AnimationPlayer.gameObject);
						StopMachineGunSfx();
						mHasExplosionEffectStarted = true;
					}
					mExplosionTimer -= Time.deltaTime;
					if (mExplosionTimer <= 0f)
					{
						base.gameObject.SetActive(false);
						DestroySnapTarget();
						ExplosionManager.Instance.StartExplosion(AnimationPlayer.transform.position, 500f, ExplosionManager.ExplosionType.Helicopter);
						ExplosivesSFX.Instance.HelicopterExplode.Play(AnimationPlayer.gameObject);
						SetPieceSFX.Instance.HindDamaged.Stop(base.gameObject);
					}
				}
				else if (!Routine.IsSpawned())
				{
					Cleanup();
				}
				else
				{
					if (Routine.CanShoot())
					{
						Actor currentTarget = Routine.GetCurrentTarget();
						bool flag = false;
						if (currentTarget != null)
						{
							GunTarget = Vector3.Lerp(GunTarget, currentTarget.GetPosition(), Time.deltaTime * GunTrackingSpeed);
							mLaserSight.transform.forward = (GunTarget - GunNozzle.position).normalized;
							if ((GunTarget - currentTarget.GetPosition()).sqrMagnitude < 25f)
							{
								flag = true;
							}
						}
						if (flag)
						{
							mMachineGun.DepressTrigger();
						}
					}
					if (Routine.CanFireRockets())
					{
						mTimeToNextShot -= Time.deltaTime;
						if (mTimeToNextShot <= 0f)
						{
							FireRocketAtCurrentTarget();
							mTimeToNextShot = TimeBetweenRockets;
						}
					}
				}
			}
			mMachineGun.Update(Time.deltaTime, null, null);
		}
		UpdateAnimation(base.transform.position - mLastPosition);
	}

	private void LateUpdate()
	{
		mLastPosition = base.transform.position;
		mLighting.UpdateMaterials(true);
	}

	private void OnDrawGizmos()
	{
		if (Routine != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, Routine.CurrentPosition);
		}
	}

	public void ReInitialise()
	{
		base.transform.position = mAwakePosition;
		base.transform.rotation = mAwakeRotation;
		AnimationPlayer.transform.localRotation = mAwakeAnimationPlayerRotation;
		mAwakeAPNodeHind.localPosition = mAwakeAPNodeHindPosition;
		mAwakeAPNodeHind.localRotation = mAwakeAPNodeHindRotation;
		if (mSnapTarget == null)
		{
			SetupSnapTarget();
		}
		base.gameObject.SetActive(true);
		mHoverAnimation.weight = 0f;
		mForwardAnimation.weight = 0f;
		mBackAnimation.weight = 0f;
		mLeftAnimation.weight = 0f;
		mRightAnimation.weight = 0f;
		mDeathAnimation.weight = 0f;
		ResetState();
	}

	public void FireRocket(GameObject specifiedTarget)
	{
		GameObject gameObject = AnimationPlayer.gameObject;
		GameObject gameObject2 = SceneNanny.Instantiate(ExplosionManager.Instance.RPG) as GameObject;
		gameObject2.name = base.name + "_RPG";
		Vector3 position = specifiedTarget.transform.position;
		RPGProjectile component = gameObject2.GetComponent<RPGProjectile>();
		component.OverrideLaunchSpeed(m_Interface.RPGProjectileSettings.LaunchSpeed);
		component.OverrideDamageMultiplier(m_Interface.RPGProjectileSettings.DamageMultiplier);
		component.OverrideDistanceToStartCollision(10f);
		Vector3 vector = gameObject.transform.position + gameObject.transform.right * ((!mFiringFromRight) ? (0f - ROCKET_FIRE_LOCAL_OFFSET.x) : ROCKET_FIRE_LOCAL_OFFSET.x) + gameObject.transform.up * ROCKET_FIRE_LOCAL_OFFSET.y + gameObject.transform.forward * ROCKET_FIRE_LOCAL_OFFSET.z;
		component.OverrideLaunchPosition(vector);
		component.Launch(gameObject, (position - vector).normalized.normalized, false);
		WeaponSFX.Instance.RPGFire.Play(base.gameObject);
		ActiveRocketsCount++;
		mFiringFromRight = !mFiringFromRight;
		Routine.TemporarilySuppressFiring();
	}

	public void FireRocketAtCurrentTarget()
	{
		GameObject gameObject = AnimationPlayer.gameObject;
		GameObject gameObject2 = SceneNanny.Instantiate(ExplosionManager.Instance.RPG) as GameObject;
		gameObject2.name = base.name + "_RPG";
		Actor currentTarget = Routine.GetCurrentTarget();
		if (currentTarget != null)
		{
			if (CanSeeTarget(currentTarget))
			{
				Vector3 position = currentTarget.GetPosition();
				position += UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(mRocketInaccuracy - 1f, mRocketInaccuracy + 1f);
				mRocketInaccuracy -= 1f;
				mRocketInaccuracy = Mathf.Max(1f, mRocketInaccuracy);
				RPGProjectile component = gameObject2.GetComponent<RPGProjectile>();
				component.OverrideLaunchSpeed(m_Interface.RPGProjectileSettings.LaunchSpeed);
				component.OverrideDamageMultiplier(m_Interface.RPGProjectileSettings.DamageMultiplier);
				component.OverrideDistanceToStartCollision(10f);
				Vector3 vector = gameObject.transform.position + gameObject.transform.right * ((!mFiringFromRight) ? (0f - ROCKET_FIRE_LOCAL_OFFSET.x) : ROCKET_FIRE_LOCAL_OFFSET.x) + gameObject.transform.up * ROCKET_FIRE_LOCAL_OFFSET.y + gameObject.transform.forward * ROCKET_FIRE_LOCAL_OFFSET.z;
				component.OverrideLaunchPosition(vector);
				component.Launch(gameObject, (position - vector).normalized.normalized, false);
				WeaponSFX.Instance.RPGFire.Play(base.gameObject);
				ActiveRocketsCount++;
				mFiringFromRight = !mFiringFromRight;
				Routine.TemporarilySuppressFiring();
			}
		}
		else
		{
			Debug.Log("FireRocketAtCurrentTarget called on Helicopter but there is no target currently available. Will do nothing.");
		}
	}

	public void RocketCounter(int num)
	{
		ActiveRocketsCount -= num;
		if (ActiveRocketsCount == 0)
		{
			TimeManager.instance.DisableSlomo();
		}
	}

	public void OnReturnFromFlee()
	{
		SetupSnapTarget();
		if (mDamageEffectAfterFlee != null && !mDamageEffectAfterFlee.activeInHierarchy)
		{
			if (mDamageEffect != null && mDamageEffect.activeInHierarchy)
			{
				mDamageEffect.SetActive(false);
			}
			mDamageEffectAfterFlee.SetActive(true);
		}
	}

	private void ResetState()
	{
		SetupHealth();
		mTimeToNextShot = TimeBetweenRockets;
		mLastPosition = mAwakePosition;
		mExplosionTimer = 8f;
		mRocketInaccuracy = 5f;
		mDamageSFXStarted = false;
		mVelocity = Vector3.zero;
		mCurrentTargetPosition = Vector3.zero;
		mForwardAmount = (mRightAmount = 0f);
		mExplosionBlend = 0f;
		mHoverAnimationDeathWeight = 0f;
		mHoverAnimationForwardWeight = 0f;
		mHoverAnimationBackWeight = 0f;
		mHoverAnimationLeftWeight = 0f;
		mHoverAnimationRightWeight = 0f;
		if (mDamageEffect != null)
		{
			mDamageEffect.SetActive(false);
		}
		if (mDamageEffectAfterFlee != null)
		{
			mDamageEffectAfterFlee.SetActive(false);
		}
		mHasExplosionEffectStarted = false;
		StopMachineGunSfx();
	}

	private void SetupHitBox()
	{
		if (HitBoxRig == null)
		{
			return;
		}
		mHealth = base.gameObject.AddComponent<HealthComponent>();
		mHealth.OnHealthEmpty += OnHealthEmpty;
		mHealth.OnHealthChange += OnHealthChange;
		GameObject gameObject = AnimationPlayer.gameObject;
		List<HitLocation> list = new List<HitLocation>();
		foreach (HitBoxDescriptor.HitBox hitBox in HitBoxRig.HitBoxes)
		{
			HitLocation hitLocation = HitBoxUtils.CreateHitLocation(gameObject, hitBox);
			hitLocation.transform.parent = gameObject.transform;
			hitLocation.Owner = gameObject;
			hitLocation.Health = mHealth;
			if (OverWatchHeli)
			{
				hitLocation.gameObject.layer = LayerMask.NameToLayer("SimpleHitBox");
			}
			list.Add(hitLocation);
		}
		foreach (HitLocation item in list)
		{
			Rigidbody rigidbody = item.gameObject.RequireComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.freezeRotation = true;
			rigidbody.mass = item.Mass;
		}
	}

	private void SetupHealth()
	{
		if (mHealth == null)
		{
			return;
		}
		mHealth.Initialise(0f, Health, Health);
		if (!(HealthDamageFleeDelta <= 0f))
		{
			mHealthCallbackThresholds = new List<float>();
			for (float num = 1f - HealthDamageFleeDelta; num > 0.01f; num -= HealthDamageFleeDelta)
			{
				mHealthCallbackThresholds.Add(Health * num);
			}
		}
	}

	private void SetupLighting()
	{
		GameObject gameObject = AnimationPlayer.gameObject;
		CharacterLighting characterLighting = gameObject.AddComponent<CharacterLighting>();
		characterLighting.Renderers = gameObject.GetComponentsInChildren<Renderer>();
		characterLighting.ProbeAnchor = LightingAnchor ?? gameObject.transform;
		characterLighting.FlashAnchor = GunNozzle;
		mLighting = characterLighting;
	}

	public void SetupSnapTarget()
	{
		if (mSnapTarget == null)
		{
			mSnapTarget = ActorGenerator.CreateStandardSnapTarget(base.transform);
			mSnapTarget.SnapPositionOverride = () => base.transform.position;
		}
	}

	public void DestroySnapTarget()
	{
		if (mSnapTarget != null)
		{
			UnityEngine.Object.Destroy(mSnapTarget.gameObject);
			mSnapTarget = null;
		}
	}

	private void UpdateAnimation(Vector3 delta)
	{
		if (Routine != null && Routine.IsExploding())
		{
			mExplosionBlend += Time.deltaTime * 0.001f;
			mExplosionBlend = Mathf.Clamp01(mExplosionBlend);
			mDeathAnimation.weight = mExplosionBlend;
			float num = 1f - mExplosionBlend;
			mHoverAnimation.weight = mHoverAnimationDeathWeight * num;
			mForwardAnimation.weight = mHoverAnimationForwardWeight * num;
			mBackAnimation.weight = mHoverAnimationBackWeight * num;
			mLeftAnimation.weight = mHoverAnimationLeftWeight * num;
			mRightAnimation.weight = mHoverAnimationRightWeight * num;
		}
		else
		{
			float num2 = Vector3.Dot(base.transform.forward, delta);
			float num3 = Vector3.Dot(base.transform.right, delta);
			mForwardAmount = Mathf.Lerp(mForwardAmount, num2, 0.3f * Time.deltaTime);
			mRightAmount = Mathf.Lerp(mRightAmount, num3, 0.3f * Time.deltaTime);
			float num4 = AnimationPlayer.transform.eulerAngles.y;
			if (Routine != null)
			{
				Vector3 to = base.transform.forward;
				Vector3 to2 = Routine.CurrentPosition;
				Vector3 vector = Routine.CurrentPosition - base.transform.position;
				if (vector.sqrMagnitude > 0f)
				{
					to = vector.normalized;
				}
				if (Routine.GetCurrentTarget() != null)
				{
					Vector3 vector2 = Routine.GetCurrentTarget().GetPosition() - base.transform.position;
					if (vector2.sqrMagnitude > 0f)
					{
						to = vector2.normalized;
						to2 = Routine.GetCurrentTarget().GetPosition();
					}
				}
				Vector3 forward = AnimationPlayer.transform.forward;
				float t = 1f * Time.deltaTime;
				Vector3 from = Vector3.Lerp(forward, to, t);
				num4 = Vector3.Angle(from, Vector3.forward);
				if (mCurrentTargetPosition == Vector3.zero)
				{
					mCurrentTargetPosition = to2;
				}
				mCurrentTargetPosition = Vector3.Lerp(mCurrentTargetPosition, to2, Routine.GetSpeedMultiplier() * Time.deltaTime);
				float y = mCurrentTargetPosition.x - base.transform.position.x;
				float x = mCurrentTargetPosition.z - base.transform.position.z;
				num4 = Mathf.Atan2(y, x) * 180f / (float)Math.PI;
				if (num4 < 0f)
				{
					num4 += 360f;
				}
			}
			float num5 = 60f;
			float num6 = 60f;
			float num7 = 200f;
			AnimationPlayer.transform.eulerAngles = new Vector3(Mathf.Clamp(num7 * mForwardAmount, 0f - num5, num5), num4, Mathf.Clamp((0f - num7) * mRightAmount, 0f - num6, num6));
			mHoverAnimation.weight = (mHoverAnimationDeathWeight = Mathf.Clamp01(1f));
			mForwardAnimation.weight = (mHoverAnimationForwardWeight = Mathf.Clamp01(num2));
			mBackAnimation.weight = (mHoverAnimationBackWeight = Mathf.Clamp01(0f - num2));
			mLeftAnimation.weight = (mHoverAnimationLeftWeight = Mathf.Clamp01(0f - num3));
			mRightAnimation.weight = (mHoverAnimationRightWeight = Mathf.Clamp01(num3));
		}
		if (Routine != null)
		{
			mVelocity = Vector3.Lerp(mVelocity, Vector3.zero, Time.deltaTime * 1f);
			Vector3 vector3 = Routine.CurrentPosition - base.transform.position;
			float magnitude = vector3.magnitude;
			if (magnitude > 0f)
			{
				float vELOCITY_MAX = VELOCITY_MAX;
				vELOCITY_MAX *= Routine.GetSpeedMultiplier();
				Vector3 vector4 = vector3.normalized * (magnitude * 0.5f) * Routine.GetSpeedMultiplier();
				mVelocity += vector4 * Time.deltaTime;
				mVelocity.x = Mathf.Clamp(mVelocity.x, 0f - vELOCITY_MAX, vELOCITY_MAX);
				mVelocity.y = Mathf.Clamp(mVelocity.y, 0f - vELOCITY_MAX, vELOCITY_MAX);
				mVelocity.z = Mathf.Clamp(mVelocity.z, 0f - vELOCITY_MAX, vELOCITY_MAX);
			}
			Vector3 vector5 = mVelocity;
			if (Routine.IsReturningFromFlee())
			{
				vector5 *= FleeReturnSpeedMultiplier;
			}
			base.transform.position += vector5 * Time.deltaTime;
		}
	}

	private void OnHealthChange(object sender, EventArgs args)
	{
		if (mDamageEffect != null && !mDamageEffect.activeInHierarchy && (mDamageEffectAfterFlee == null || (mDamageEffectAfterFlee != null && !mDamageEffectAfterFlee.activeInHierarchy)) && mHealth.Health01 * 100f <= DamageEffectHealthPercent)
		{
			mDamageEffect.SetActive(true);
		}
		if (mHealth.HealthLow && !mDamageSFXStarted)
		{
			SetPieceSFX.Instance.HindDamaged.Play(base.gameObject);
			SetPieceSFX.Instance.HindEngine.Stop(base.gameObject);
			mDamageSFXStarted = true;
		}
		if (mHealthCallbackThresholds != null && mHealthCallbackThresholds.Count != 0 && mHealth.Health <= mHealthCallbackThresholds[0])
		{
			mHealthCallbackThresholds.RemoveAt(0);
			Routine.Flee();
		}
	}

	private void OnHealthEmpty(object sender, HealthComponent.HeathChangeEventArgs args)
	{
		StopMachineGunSfx();
		Routine.TriggerDestroy(args);
	}

	private bool CanSeeTarget(Actor target)
	{
		Vector3 collision;
		return WorldHelper.IsClearTrace(GunNozzle.position, target.GetPosition(), out collision);
	}

	private void SetupBlip()
	{
		if (!(HudMarker == null))
		{
			mHudMarker = SceneNanny.Instantiate(HudMarker) as HudBlipIcon;
			mHudMarker.Target = base.transform;
			mHudMarker.SwitchOff();
			EnemyRocketBlip enemyRocketBlip = mHudMarker as EnemyRocketBlip;
			enemyRocketBlip.DoBaseOnOffScreenControl = true;
		}
	}

	public void Cleanup()
	{
		SetPieceSFX.Instance.HindEngine.Stop(base.gameObject);
		SetPieceSFX.Instance.HindDamaged.Stop(base.gameObject);
		AnimationPlayer.gameObject.SetActive(false);
		StopMachineGunSfx();
	}

	private void StopMachineGunSfx()
	{
		if (mMachineGun != null)
		{
			Weapon_HelicopterMachineGun weapon_HelicopterMachineGun = mMachineGun as Weapon_HelicopterMachineGun;
			if (weapon_HelicopterMachineGun != null)
			{
				weapon_HelicopterMachineGun.StopFiringSfxLoop();
			}
		}
	}

	public void StopEngineLoops()
	{
		SetPieceSFX.Instance.HindEngine.Stop(base.gameObject);
		SetPieceSFX.Instance.HindDamaged.Stop(base.gameObject);
	}
}
