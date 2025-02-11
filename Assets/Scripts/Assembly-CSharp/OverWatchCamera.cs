using System.Collections.Generic;
using UnityEngine;

public class OverWatchCamera : CameraBase
{
	public GameObject CentrePointObject;

	public bool allowPanning = true;

	public float smoothPanningSpeed = 8f;

	public float PanVelocityDamping = 0.9f;

	private Vector3 panVelocity = Vector3.zero;

	private float lastPanTime;

	private Vector3 m_CurrentLookAtPoint;

	private Vector3 mPanMove;

	private float mRealTimeLastFrame;

	private float mSpeedToFocusPoint = 0.3f;

	private bool m_bInputEnabled;

	private List<Transform> mWaypoints;

	private int mCurrentWaypointTarget;

	private Vector3 mWaypointMoverPosition;

	private Vector3 mWaypointMoverFacing;

	private Vector3 mWaypointMoverVelocity;

	private Vector2 mWaypointTargetting;

	private ActorWrapper mFocusActorWrapper;

	public List<Transform> Waypoints
	{
		get
		{
			return mWaypoints;
		}
		set
		{
			mWaypoints = new List<Transform>(value);
		}
	}

	public bool IsWaypointMovement
	{
		get
		{
			return mWaypoints != null && mWaypoints.Count > 1;
		}
	}

	public override Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			TBFAssert.DoAssert(false, "not allowed");
		}
	}

	public Transform FocusPoint { get; set; }

	public float DistanceFromCentre { get; set; }

	public float Height { get; set; }

	public float RotateSpeed { get; set; }

	public float TranslateSpeed { get; set; }

	public float PanningSensitivity { get; set; }

	public float FovDefault { get; set; }

	public float MaxLookRadius { get; set; }

	public float TargetRecentreRate { get; set; }

	public void Awake()
	{
		DistanceFromCentre = 50f;
		Height = 200f;
		RotateSpeed = 5f;
		TranslateSpeed = 2f;
		PanningSensitivity = 10f;
		FovDefault = 45f;
		MaxLookRadius = 30f;
		TargetRecentreRate = 0.1f;
	}

	public void Start()
	{
		if (IsWaypointMovement)
		{
			mCurrentWaypointTarget = 0;
			mWaypointMoverPosition = mWaypoints[mCurrentWaypointTarget].position;
			mWaypointMoverFacing = mWaypoints[mCurrentWaypointTarget].forward;
			m_CurrentLookAtPoint = mWaypointMoverPosition + base.transform.forward * 500f;
		}
		else
		{
			if (CentrePointObject == null)
			{
				CentrePointObject = new GameObject();
				CentrePointObject.name = "CentrePointObjectForOverWatch";
			}
			m_CurrentLookAtPoint = CentrePointObject.transform.position;
		}
		if (FocusPoint != null)
		{
			mFocusActorWrapper = FocusPoint.gameObject.GetComponentInChildren<ActorWrapper>();
		}
		EnableInput();
	}

	public override void AllowInput(bool allow)
	{
		if (allow)
		{
			EnableInput();
		}
		else
		{
			DisableInput();
		}
	}

	private void EnableInput()
	{
		if (!m_bInputEnabled)
		{
			m_bInputEnabled = true;
			InputManager.Instance.AddOnFingerTapEventHandler(ObjectSelecter_OnFingerTap, 0);
		}
	}

	private void DisableInput()
	{
		if (m_bInputEnabled)
		{
			m_bInputEnabled = false;
			InputManager.Instance.RemoveOnFingerTapEventHandler(ObjectSelecter_OnFingerTap);
		}
	}

	private void LateUpdate()
	{
		if (IsWaypointMovement)
		{
			if (TimeManager.instance != null && TimeManager.instance.GlobalTimeState == TimeManager.State.IngamePaused)
			{
				mWaypointMoverVelocity = Vector3.zero;
				return;
			}
			mWaypointMoverVelocity = Vector3.Lerp(mWaypointMoverVelocity, Vector3.zero, 0.2f * Time.deltaTime);
			Vector3 vector = mWaypoints[mCurrentWaypointTarget].position - mWaypointMoverPosition;
			mWaypointMoverVelocity += vector.normalized * 4f * Time.deltaTime;
			mWaypointMoverVelocity = Vector3.ClampMagnitude(mWaypointMoverVelocity, TranslateSpeed);
			mWaypointMoverPosition += mWaypointMoverVelocity * Time.deltaTime;
			Vector3 to = mWaypoints[mCurrentWaypointTarget].forward;
			if (FocusPoint != null)
			{
				Transform focusPoint = FocusPoint;
				if (mFocusActorWrapper != null && mFocusActorWrapper.GetActor() != null)
				{
					focusPoint = mFocusActorWrapper.GetActor().transform;
				}
				to = (focusPoint.position - mWaypointMoverPosition).normalized;
			}
			mWaypointMoverFacing = Vector3.Lerp(mWaypointMoverFacing, to, RotateSpeed * 0.05f * Time.deltaTime);
			if (vector.sqrMagnitude < 9f)
			{
				mCurrentWaypointTarget++;
				if (mCurrentWaypointTarget >= mWaypoints.Count)
				{
					mCurrentWaypointTarget = 0;
				}
			}
			base.transform.position = mWaypointMoverPosition;
			base.transform.forward = mWaypointMoverFacing;
			Quaternion quaternion = Quaternion.AngleAxis(0f - mWaypointTargetting.x, Vector3.up);
			Quaternion quaternion2 = Quaternion.AngleAxis(mWaypointTargetting.y, base.transform.right);
			Quaternion quaternion3 = quaternion * quaternion2;
			base.transform.forward = quaternion3 * base.transform.forward;
			RecentreTargeting();
		}
		else
		{
			if (CentrePointObject == null)
			{
				return;
			}
			Apply(CentrePointObject.transform.position);
			base.transform.position = m_CurrentLookAtPoint;
			Vector3 vector2 = new Vector3(DistanceFromCentre, Height, 0f);
			base.transform.localPosition += vector2;
			base.transform.Rotate(Vector3.forward * (RotateSpeed * Time.deltaTime));
		}
		Fov = FovDefault;
	}

	private void ObjectSelecter_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
	}

	public void DoPan(Vector2 delta)
	{
		if (IsWaypointMovement)
		{
			if (!(TimeManager.instance != null) || TimeManager.instance.GlobalTimeState != TimeManager.State.IngamePaused)
			{
				delta *= PanningSensitivity * 0.2f;
				mWaypointTargetting += delta;
				float num = MaxLookRadius;
				float num2 = MaxLookRadius;
				float max = MaxLookRadius;
				Transform transform = mWaypoints[mCurrentWaypointTarget];
				OverwatchWaypoint component = transform.GetComponent<OverwatchWaypoint>();
				if (component != null)
				{
					num = component.MaxLookExtentsHorizontal;
					num2 = component.MaxLookExtentsVerticalFwds;
					max = component.MaxLookExtentsVerticalBack;
				}
				mWaypointTargetting.x = Mathf.Clamp(mWaypointTargetting.x, 0f - num, num);
				mWaypointTargetting.y = Mathf.Clamp(mWaypointTargetting.y, 0f - num2, max);
				float x = transform.transform.eulerAngles.x;
				if (x + mWaypointTargetting.y >= 70f)
				{
					mWaypointTargetting.y = 70f - x;
				}
			}
		}
		else if (allowPanning)
		{
			panVelocity = delta;
			if (panVelocity == Vector3.zero)
			{
				mPanMove = Vector3.zero;
				return;
			}
			Vector3 vector = base.gameObject.transform.forward + base.gameObject.transform.up;
			vector.y = 0f;
			vector.Normalize();
			Vector3 vector2 = -0.02f * PanningSensitivity * (base.gameObject.transform.right * delta.x + vector * delta.y);
			mPanMove = vector2;
			lastPanTime = Time.realtimeSinceStartup;
		}
	}

	private void Apply(Vector3 centrePointObjectPosition)
	{
		if (lastPanTime != Time.realtimeSinceStartup)
		{
			Vector3 currentLookAtPoint = m_CurrentLookAtPoint;
			Vector3 vector = m_CurrentLookAtPoint + mPanMove;
			if ((vector - centrePointObjectPosition).sqrMagnitude > MaxLookRadius * MaxLookRadius)
			{
				vector = m_CurrentLookAtPoint;
			}
			Vector3 vector2 = currentLookAtPoint - vector;
			vector2.y = 0f;
			vector2 *= mSpeedToFocusPoint;
			panVelocity = new Vector2(vector2.x, vector2.z);
			mPanMove = -vector2;
			panVelocity *= PanVelocityDamping;
			m_CurrentLookAtPoint += mPanMove;
		}
	}

	private void RecentreTargeting()
	{
		TBFAssert.DoAssert(IsWaypointMovement, "Should not be called if we're not in Waypoint Movement mode");
		mWaypointTargetting = Vector2.Lerp(mWaypointTargetting, Vector2.zero, Time.deltaTime * 0.02f);
	}

	private void DebugLogOutput()
	{
		if (IsWaypointMovement)
		{
			OverwatchWaypoint component = mWaypoints[mCurrentWaypointTarget].GetComponent<OverwatchWaypoint>();
			if (component != null)
			{
				Debug.Log(string.Format("OverWatchCamera -> Heading for Waypoint {0}. Overriding MaxLookExtentsHorizontal={1} MaxLookExtentsVerticalFwds={2} MaxLookExtentsVerticalBack={3}", mCurrentWaypointTarget, component.MaxLookExtentsHorizontal, component.MaxLookExtentsVerticalFwds, component.MaxLookExtentsVerticalBack));
			}
			else
			{
				Debug.Log(string.Format("OverWatchCamera -> Heading for Waypoint {0}. No Script metadata. Using global MaxLookRadius={1}", mCurrentWaypointTarget, MaxLookRadius));
			}
		}
	}
}
