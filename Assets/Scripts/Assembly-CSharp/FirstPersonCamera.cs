using UnityEngine;

public class FirstPersonCamera : CameraBase, PlayCameraInterface
{
	private Transform mTransform;

	public Transform HeadBone;

	public Vector3 Angles;

	public Vector3 RecoilAngles;

	public Vector3 SwayAngles;

	public GameObject model;

	public GameObject owner;

	public bool InitAnglesFromTransform;

	public float HeadHeight;

	private float mShakeIntensity;

	private float mShakeDuration;

	private float mDazeIntensity;

	private float mDazeDuration;

	private float mShakeSampleCoefficient;

	private float mDazedSampleCoefficient;

	private Vector3 mShakeOffsets;

	private Vector3 mConstraintAnglesLower;

	private Vector3 mConstraintAnglesUpper;

	private float mFlinchFOV;

	private Vector3 mFlinchAngles;

	private float mBaseFieldOfView;

	public override float Fov
	{
		get
		{
			return mBaseFieldOfView * (1f - 0.1f * mFlinchFOV);
		}
		set
		{
			mBaseFieldOfView = value;
		}
	}

	public bool LockPitch { get; set; }

	public override Vector3 Position
	{
		get
		{
			return base.transform.position + InputSettings.FirstPersonViewOffset * (Quaternion.Euler(Angles) * Vector3.forward);
		}
		set
		{
			TBFAssert.DoAssert(false, "not allowed");
		}
	}

	public Vector3 PanOffset
	{
		get
		{
			return Vector3.zero;
		}
	}

	public Vector3 PanVelocity
	{
		get
		{
			return Vector3.zero;
		}
	}

	public bool MovingToFocusPoint
	{
		get
		{
			return false;
		}
	}

	public float WorldRotation
	{
		get
		{
			return base.transform.eulerAngles.y;
		}
	}

	public void Awake()
	{
		ClearConstraints();
		mTransform = base.transform;
	}

	public void Start()
	{
		if (InitAnglesFromTransform)
		{
			Angles = base.transform.eulerAngles;
		}
		ClearConstraints();
	}

	public void Flinch(Vector3 origin)
	{
		Flinch(origin, 1f);
	}

	public void Flinch(Vector3 origin, float scale)
	{
		float from = 5f;
		float to = 50f * scale;
		float modifierForPerk = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.Toughness);
		float from2 = 4f * scale * modifierForPerk;
		float to2 = 0.1f * modifierForPerk;
		float t = Mathf.InverseLerp(from, to, Vector3.Distance(Position, origin));
		float num = Mathf.Lerp(from2, to2, t);
		mFlinchAngles = Vector3.Min(mFlinchAngles, num * new Vector3(-1f, 0f, 0f));
	}

	public override void AddShake(Vector3 origin, float radius, float duration)
	{
		float num = 30f;
		float num2 = 5f;
		float from = 0.3f;
		float to = 0.8f;
		float from2 = 0.8f;
		float to2 = 5f;
		float from3 = 0f;
		float to3 = 0.5f;
		float from4 = 0f;
		float to4 = 1f;
		float num3 = num * num;
		float num4 = num2 * num2;
		Vector3 vector = Position - origin;
		float t = Mathf.Clamp01(1f - vector.sqrMagnitude / num3);
		float t2 = Mathf.Clamp01(1f - vector.sqrMagnitude / num4);
		float b = Mathf.Lerp(from3, to3, t);
		float b2 = Mathf.Lerp(from, to, t);
		float b3 = Mathf.Lerp(from4, to4, t2);
		float b4 = Mathf.Lerp(from2, to2, t2);
		mShakeIntensity = Mathf.Max(mShakeIntensity, b);
		mShakeDuration = Mathf.Max(mShakeDuration, b2);
		mDazeIntensity = Mathf.Max(mDazeIntensity, b3);
		mDazeDuration = Mathf.Max(mDazeDuration, b4);
	}

	public override void AddShake(float scale, float duration)
	{
		mShakeIntensity = Mathf.Max(mShakeIntensity, scale);
		mShakeDuration = Mathf.Max(mShakeDuration, duration);
	}

	private void UpdateShake()
	{
		TimeManager instance = TimeManager.instance;
		if (instance != null && instance.GlobalTimeState != TimeManager.State.IngamePaused)
		{
			float deltaTime = Time.deltaTime;
			if (mShakeDuration > 0f || mDazeDuration > 0f)
			{
				float num = 100f;
				float num2 = 1f;
				mShakeSampleCoefficient += num * deltaTime;
				mDazedSampleCoefficient += num2 * deltaTime;
				float num3 = mShakeIntensity * Mathf.Clamp01(mShakeDuration * 10f);
				float num4 = mDazeIntensity * Mathf.Clamp01(mDazeDuration * 0.5f);
				mShakeOffsets = num3 * 5f * Noise.Smooth(mShakeSampleCoefficient) + num4 * 5f * Noise.Smooth(mDazedSampleCoefficient);
			}
			mShakeDuration -= Mathf.Min(mShakeDuration, deltaTime);
			mDazeDuration -= Mathf.Min(mDazeDuration, deltaTime);
			mShakeIntensity -= Mathf.Min(mShakeIntensity, deltaTime);
			mDazeIntensity -= Mathf.Min(mDazeIntensity, 0.1f * deltaTime);
		}
	}

	public void LateUpdate()
	{
		ViewModelRig viewModelRig = ViewModelRig.Instance();
		if (HeadBone == null)
		{
			if (owner != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}
		else
		{
			mTransform.position = HeadBone.position + HeadHeight * HeadBone.up + viewModelRig.ViewBob;
		}
		float num = Time.deltaTime * -4f;
		if (num > 0f)
		{
			num = 0f;
		}
		if (num < -1f)
		{
			num = -1f;
		}
		Vector3 vector = mFlinchAngles;
		vector.x += vector.x * num;
		vector.y += vector.y * num;
		vector.z += vector.z * num;
		mFlinchAngles = vector;
		num *= 4f;
		if (num > 0f)
		{
			num = 0f;
		}
		if (num < -1f)
		{
			num = -1f;
		}
		mFlinchFOV += mFlinchFOV * num;
		GameController instance = GameController.Instance;
		Actor mFirstPersonActor = instance.mFirstPersonActor;
		Quaternion quaternion = ((!(mFirstPersonActor != null)) ? Quaternion.identity : mFirstPersonActor.realCharacter.GetReferenceQuaternion());
		Rotation = quaternion * Quaternion.Euler(Angles + RecoilAngles + SwayAngles + mFlinchAngles + mShakeOffsets);
		if (this == instance.FirstPersonCamera)
		{
			if (InteractionsManager.Instance.AllowFirstPersonAnims())
			{
				viewModelRig.UpdateForActor(GameController.Instance.mFirstPersonActor);
			}
			else
			{
				viewModelRig.UpdateForActor(null);
			}
			Transform cameraLocator = viewModelRig.GetCameraLocator();
			if (cameraLocator != null)
			{
				mTransform.position = cameraLocator.position;
				mTransform.rotation = Quaternion.Euler(cameraLocator.rotation.eulerAngles + mShakeOffsets);
				Vector3 angles = Angles;
				angles.x = 0f;
				angles.y = cameraLocator.eulerAngles.y;
				angles.z = 0f;
				Angles = angles;
			}
		}
		UpdateShake();
	}

	public void UpdatePosition()
	{
		if (HeadBone != null)
		{
			Vector3 viewBob = ViewModelRig.Instance().ViewBob;
			base.transform.position = HeadBone.position + HeadHeight * HeadBone.up + viewBob;
		}
	}

	public void ClearConstraints()
	{
		mConstraintAnglesLower = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		mConstraintAnglesUpper = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		LockPitch = false;
	}

	public Vector3 GetConstrainedAngles(Vector3 angles)
	{
		Vector3 result = Vector3.Max(Vector3.Min(angles, mConstraintAnglesUpper), mConstraintAnglesLower);
		if (LockPitch)
		{
			result.x = 0f;
			LockPitch = false;
		}
		return result;
	}

	public void SetConstraints(Vector3 centreAngles, float yaw, float pitchUp, float pitchDown)
	{
		mConstraintAnglesLower = new Vector3(centreAngles.x - pitchDown, centreAngles.y - yaw, float.MinValue);
		mConstraintAnglesUpper = new Vector3(centreAngles.x + pitchUp, centreAngles.y + yaw, float.MaxValue);
	}

	public void FocusAndSelectTarget(Transform trans)
	{
	}

	public void SnapToTarget(Transform trans)
	{
	}

	public bool FocusOnTarget(Transform trans, bool forceEvenIfOnScreen)
	{
		return false;
	}
}
