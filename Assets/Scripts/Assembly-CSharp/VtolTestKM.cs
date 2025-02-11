using System;
using System.Collections.Generic;
using UnityEngine;

public class VtolTestKM : MonoBehaviour
{
	public enum RoutineBehavior
	{
		Loop = 0,
		WaitAtEnd = 1
	}

	private const float EngineSoundVelMin = 0.5f;

	private const float EngineSoundVelMax = 4f;

	private const float EngineSoundRange = 3.5f;

	private const float OneOverEngineSoundRange = 0.2857143f;

	private const float BaseVolMovingSoundVol = 0.1f;

	public Transform GunNozzle;

	private VtolTestRoutineKM Routine;

	public List<VtolTestRoutineKM> Routines;

	public RoutineBehavior Style;

	public float RotationSpeed = 2f;

	public float GunTrackingSpeed = 1f;

	public float Health = 20000f;

	public GameObject Engine;

	public GameObject EngineFlap1;

	public GameObject EngineFlap2;

	public GameObject EngineFlap3;

	public GameObject EngineFlap4;

	public HitBoxDescriptor HitBoxRig;

	public bool FaceOtherDirection = true;

	public bool DropRotationSpeed;

	public GameObject LinkedActor;

	public bool isDressing;

	private int currentRoutine;

	private int routineSize;

	private int runTimerOnce;

	private float pitchLimit;

	private float rollLimit;

	private static bool interiorSoundOn;

	private Actor mLinkedActor;

	private float VELOCITY_MAX_X = 6f;

	private float VELOCITY_MAX_Y = 6f;

	private float VELOCITY_MAX_Z = 6f;

	private HealthComponent mHealth;

	private GameObject mLaserSight;

	private Vector3 mLastPosition;

	private Vector3 mVelocity;

	private float mForwardAmount;

	private float mRightAmount;

	private float mExplosionTimer;

	private float mExplosionBlend;

	private bool mEngineLoopPlaying;

	private float DesiredSpeed;

	public Vector3 GunTarget { get; private set; }

	private void Awake()
	{
		SetupHitBox();
	}

	private void Start()
	{
		Routine = Routines[currentRoutine];
		routineSize = Routines.Count - 1;
		Routine.TriggerStart(VELOCITY_MAX_X, pitchLimit);
		if (mHealth != null)
		{
			mHealth.OnHealthChange += OnHealthChange;
		}
		GunNozzle.transform.parent = base.gameObject.transform;
		mLaserSight = UnityEngine.Object.Instantiate(EffectsController.Instance.Effects.LaserSight) as GameObject;
		mLaserSight.transform.parent = GunNozzle;
		mLaserSight.transform.localPosition = Vector3.zero;
		mLaserSight.transform.forward = GunNozzle.forward;
		GunTarget = GunNozzle.position + GunNozzle.forward * 100f;
		mLastPosition = base.transform.position;
		mExplosionTimer = 8f;
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			if (collider.gameObject.rigidbody == null)
			{
				Rigidbody rigidbody = collider.gameObject.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
			}
		}
	}

	private void OnDestroy()
	{
		if (VTOLInGameSFX.HasInstance && VTOLInGameSFX.Instance.AmbVTOLInterior != null)
		{
			VTOLInGameSFX.Instance.AmbVTOLInterior.Stop2D();
		}
		if (mHealth != null)
		{
			mHealth.OnHealthChange -= OnHealthChange;
		}
		StopEngineLoop();
	}

	public static void TurnOnSound()
	{
		if (!interiorSoundOn)
		{
			interiorSoundOn = true;
		}
	}

	public static void TurnOffSound()
	{
		interiorSoundOn = false;
	}

	private void Activate()
	{
		if (currentRoutine >= Routines.Count - 1)
		{
			return;
		}
		currentRoutine++;
		runTimerOnce = 0;
		if (Routines[currentRoutine] == null)
		{
			currentRoutine++;
		}
		if (Routines[currentRoutine] != null)
		{
			if (currentRoutine <= routineSize)
			{
				Routine = Routines[currentRoutine];
				Routine.TriggerStart(VELOCITY_MAX_X, pitchLimit);
			}
			if (currentRoutine > routineSize && Style == RoutineBehavior.Loop)
			{
				currentRoutine = 0;
				Routine = Routines[currentRoutine];
				Routine.TriggerStart(VELOCITY_MAX_X, pitchLimit);
			}
			if (currentRoutine > routineSize && Style != RoutineBehavior.WaitAtEnd)
			{
			}
		}
	}

	public float ReturnCurrentSpeed()
	{
		return VELOCITY_MAX_X;
	}

	private void Update()
	{
		if (!base.gameObject.activeSelf && !Routine.IsIdle())
		{
			base.gameObject.SetActive(true);
		}
		Move();
		if (Routine != null && Routine != null)
		{
			if (Routine.IsExploding())
			{
				if (UnityEngine.Random.Range(0, 4) == 1)
				{
					ExplosionManager.Instance.StartExplosion(base.transform.position + UnityEngine.Random.insideUnitSphere * 5f, 50f);
				}
				mExplosionTimer -= Time.deltaTime;
				if (mExplosionTimer <= 0f)
				{
					base.gameObject.SetActive(false);
				}
			}
			else if (Routine.CanShoot())
			{
				GameObject currentTarget = Routine.GetCurrentTarget();
				bool flag = false;
				if (currentTarget != null)
				{
					GunTarget = Vector3.Lerp(GunTarget, currentTarget.transform.position, Time.deltaTime * GunTrackingSpeed);
					mLaserSight.transform.forward = (GunTarget - GunNozzle.position).normalized;
					if ((GunTarget - currentTarget.transform.position).sqrMagnitude < 25f)
					{
						flag = true;
					}
				}
				if (!flag)
				{
				}
			}
		}
		UpdateAnimation(base.transform.position - mLastPosition);
	}

	private void LateUpdate()
	{
		mLastPosition = base.transform.position;
	}

	private void SetupHitBox()
	{
		if (HitBoxRig == null)
		{
			return;
		}
		SetupHealth();
		GameObject gameObject = base.gameObject;
		List<HitLocation> list = new List<HitLocation>();
		foreach (HitBoxDescriptor.HitBox hitBox in HitBoxRig.HitBoxes)
		{
			HitLocation hitLocation = HitBoxUtils.CreateHitLocation(gameObject, hitBox);
			hitLocation.gameObject.layer = LayerMask.NameToLayer("ConstantHitBox");
			hitLocation.transform.parent = gameObject.transform;
			hitLocation.Owner = gameObject;
			hitLocation.Health = mHealth;
			list.Add(hitLocation);
		}
		foreach (HitLocation item in list)
		{
			Rigidbody rigidbody = item.gameObject.RequireComponent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.isKinematic = true;
				rigidbody.freezeRotation = true;
				rigidbody.mass = item.Mass;
			}
		}
	}

	private void SetupHealth()
	{
		mHealth = base.gameObject.AddComponent<HealthComponent>();
		mHealth.Initialise(0f, Health, Health);
	}

	private void UpdateAnimation(Vector3 delta)
	{
		if (Routine.IsExploding())
		{
			mExplosionBlend += Time.deltaTime * 0.1f;
			mExplosionBlend = Mathf.Clamp01(mExplosionBlend);
			return;
		}
		float to = Vector3.Dot(base.transform.forward, delta);
		float to2 = Vector3.Dot(base.transform.right, delta);
		mForwardAmount = Mathf.Lerp(mForwardAmount, to, 0.3f * Time.deltaTime);
		mRightAmount = Mathf.Lerp(mRightAmount, to2, 0.3f * Time.deltaTime);
		pitchLimit = Mathf.Lerp(pitchLimit, Routine.GetPitch(), Time.deltaTime * 1f);
		rollLimit = Mathf.Lerp(pitchLimit, Routine.GetRoll(), Time.deltaTime * 1f);
		if (isDressing)
		{
			pitchLimit = 25f;
			rollLimit = 30f;
		}
		float johnsFactor = Routine.GetJohnsFactor();
		base.transform.eulerAngles = new Vector3(Mathf.Clamp(johnsFactor * mForwardAmount, 0f - pitchLimit, pitchLimit), base.transform.eulerAngles.y, Mathf.Clamp((0f - johnsFactor) * mRightAmount, 0f - rollLimit, rollLimit));
	}

	private void Move()
	{
		DoEngineLoop();
		float num = 1f;
		DesiredSpeed = Mathf.Lerp(DesiredSpeed, Routine.GetDesiredSpeed(), Time.deltaTime * num);
		float newRotationSpeed = Routine.GetNewRotationSpeed();
		if (isDressing)
		{
			DesiredSpeed = 8f;
			num = 1f;
			newRotationSpeed = Routine.GetNewRotationSpeed();
		}
		VELOCITY_MAX_X = Mathf.Lerp(VELOCITY_MAX_X, DesiredSpeed, Time.deltaTime * num);
		VELOCITY_MAX_Y = Mathf.Lerp(VELOCITY_MAX_Y, DesiredSpeed, Time.deltaTime * num);
		VELOCITY_MAX_Z = Mathf.Lerp(VELOCITY_MAX_Z, DesiredSpeed, Time.deltaTime * num);
		RotationSpeed = Mathf.Lerp(RotationSpeed, newRotationSpeed, Time.deltaTime * num);
		if (!(Routine != null))
		{
			return;
		}
		mVelocity = Vector3.Lerp(mVelocity, Vector3.zero, Time.deltaTime * 0.1f);
		Vector3 vector = Routine.CurrentPosition - base.transform.position;
		float magnitude = vector.magnitude;
		if (magnitude < 6f && runTimerOnce == 0)
		{
			Routine.SendMessage("RunTimer");
			runTimerOnce = 1;
		}
		if (magnitude > 0f)
		{
			Vector3 normalized = vector.normalized;
			normalized += new Vector3(UnityEngine.Random.Range(-0.01f, 0.01f), UnityEngine.Random.Range(-0.01f, 0.01f), UnityEngine.Random.Range(-0.01f, 0.01f));
			Vector3 currentTravelDirection = Routine.CurrentTravelDirection;
			Vector3 vector2 = (normalized * (magnitude * 1f) + currentTravelDirection) * 2f;
			mVelocity += vector2 * Time.deltaTime;
			mVelocity.x = Mathf.Clamp(mVelocity.x, 0f - VELOCITY_MAX_X, VELOCITY_MAX_X);
			mVelocity.y = Mathf.Clamp(mVelocity.y, 0f - VELOCITY_MAX_Y, VELOCITY_MAX_Y);
			mVelocity.z = Mathf.Clamp(mVelocity.z, 0f - VELOCITY_MAX_Z, VELOCITY_MAX_Z);
			if (Engine != null && mVelocity.x != 0f)
			{
				float num2 = Mathf.Clamp(DesiredSpeed, 8f, DesiredSpeed);
				float num3 = 30f;
				float num4 = 55f;
				float z = num3 / 100f * (mVelocity.x / num2 * 100f);
				float num5 = num4 / 100f * (mVelocity.x / num2 * 100f);
				Quaternion to = Quaternion.Euler(0f, 0f, z);
				Quaternion to2 = Quaternion.Euler(0f, 0f - num5, 0f);
				Quaternion to3 = Quaternion.Euler(num5, 0f, 0f);
				float f = Engine.transform.localRotation.x - to.z;
				f = Mathf.Abs(f) * 5f;
				Engine.transform.localRotation = Quaternion.Slerp(Engine.transform.localRotation, to, Time.deltaTime * f);
				if (EngineFlap1 != null && EngineFlap2 != null && EngineFlap3 != null && EngineFlap4 != null)
				{
					EngineFlap1.transform.localRotation = Quaternion.Slerp(EngineFlap1.transform.localRotation, to2, Time.deltaTime * f);
					EngineFlap2.transform.localRotation = Quaternion.Slerp(EngineFlap2.transform.localRotation, to3, Time.deltaTime * f);
					EngineFlap3.transform.localRotation = Quaternion.Slerp(EngineFlap3.transform.localRotation, to2, Time.deltaTime * f);
					EngineFlap4.transform.localRotation = Quaternion.Slerp(EngineFlap4.transform.localRotation, to3, Time.deltaTime * f);
				}
			}
		}
		if (Routine.IsExploding())
		{
			mVelocity.y = -4f;
			VELOCITY_MAX_Y = 5f;
		}
		base.transform.position += mVelocity * Time.deltaTime;
		Vector3 vector3 = Routine.GetCurrentFacing();
		float num6 = Vector3.Angle(base.transform.forward, vector3);
		if (Routine.GetCurrentTarget() != null)
		{
			vector3 = (Routine.GetCurrentTarget().transform.position - base.transform.position).normalized;
		}
		if (DropRotationSpeed && magnitude < 1.5f && num6 < 5f)
		{
			RotationSpeed = 0.1f;
		}
		base.transform.forward = Vector3.RotateTowards(base.transform.forward, vector3, RotationSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
	}

	private void OnHealthChange(object sender, EventArgs args)
	{
		if (LinkedActor != null && mLinkedActor == null)
		{
			ActorWrapper componentInChildren = LinkedActor.GetComponentInChildren<ActorWrapper>();
			if (componentInChildren != null)
			{
				mLinkedActor = componentInChildren.GetActor();
			}
		}
		if (mLinkedActor != null)
		{
			HealthComponent.HeathChangeEventArgs heathChangeEventArgs = args as HealthComponent.HeathChangeEventArgs;
			if (heathChangeEventArgs.From != mLinkedActor.gameObject)
			{
				mLinkedActor.health.ModifyHealth(heathChangeEventArgs.From, heathChangeEventArgs.Amount, heathChangeEventArgs.DamageType, heathChangeEventArgs.Direction, heathChangeEventArgs.HeadShot, SpeechComponent.SpeechMode.Occasional);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		int i = 1;
		int count = Routines.Count;
		Gizmos.color = Color.white;
		for (; i < count && Routines[i] != null; i++)
		{
			Gizmos.DrawLine(Routines[i - 1].transform.position, Routines[i].transform.position);
		}
	}

	private void DoEngineLoop()
	{
		if (!interiorSoundOn)
		{
			StopEngineLoop();
		}
		else if (Routine != null && Routine.PlayInteriorSound)
		{
			if (Engine != null && !mEngineLoopPlaying)
			{
				VTOLInGameSFX.Instance.AmbVtolSteadyLoop.Play(Engine);
				VTOLInGameSFX.Instance.AmbVtolExtLoop.Play(Engine);
				mEngineLoopPlaying = true;
			}
			if (mEngineLoopPlaying)
			{
				float value = mVelocity.magnitude - 0.5f;
				value = Mathf.Clamp(value, 0f, 3.5f);
				value *= 0.2857143f;
				value += 0.1f;
				value = Mathf.Clamp01(value);
				SoundManager.Instance.SetVolume(VTOLInGameSFX.Instance.AmbVtolSteadyLoop, Engine, 0.6f);
				SoundManager.Instance.SetVolume(VTOLInGameSFX.Instance.AmbVtolExtLoop, Engine, value);
			}
		}
	}

	private void StopEngineLoop()
	{
		if (Engine != null && mEngineLoopPlaying)
		{
			VTOLInGameSFX.Instance.AmbVtolSteadyLoop.Stop(Engine);
			VTOLInGameSFX.Instance.AmbVtolExtLoop.Stop(Engine);
			mEngineLoopPlaying = false;
		}
	}
}
