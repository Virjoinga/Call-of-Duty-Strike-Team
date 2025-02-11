using UnityEngine;

public class GlobeCamera : MonoBehaviour
{
	private const float MIN_TIME_BEFORE_IDLING = 0.4f;

	private const float MAX_TIME_BEFORE_IDLING = 3f;

	private const float MIN_SPEED_BEFORE_IDLING = 0.2f;

	private const int deltaHistorySize = 8;

	private const float deltaHistoryTime = 0.15f;

	private const float mSpinFadeUpMult = 5f;

	private const float mSpinFadeDownMult = 0.5f;

	public Transform target;

	public float initialDistance = 10f;

	public float minDistance = 1f;

	public float maxDistance = 20f;

	public float YawSensitivity = 2.5f;

	public float PitchSensitivity = 2.5f;

	public float LerpSpeed = 20f;

	public float LerpUpSpeed = 0.1f;

	public float AdjustPositionSpeed = 0.05f;

	public float IdlingSpeed = 1f;

	public float RotateToTargetSpeed = 7f;

	public float PositionDamping = 0.98f;

	public float DefaultHeight = 100f;

	public bool allowPinchZoom = true;

	public float pinchZoomSensitivity = 2f;

	public bool AllowZoomedInMovement;

	public float smoothZoomSpeed = 3f;

	public float smoothOrbitSpeed = 4f;

	public float returnPitchSpeed = 1f;

	private SelectableMissionMarker mFocusMission;

	private float distance = 10f;

	private float idealDistance;

	private Vector3 mDesiredUp;

	private Vector3 mDesiredPosition;

	private Vector3 mHighlightPosition;

	private float mLastKnownDirection;

	private bool mTouching;

	private bool mHighlighting;

	private bool mBlockedInput;

	private float mTimeSinceLastTouch;

	private DragHistoryItem[] m_LastDelta = new DragHistoryItem[8];

	private float mSpinSfxVol;

	public SelectableMissionMarker FocusMission
	{
		get
		{
			return mFocusMission;
		}
	}

	public float Distance
	{
		get
		{
			return distance;
		}
	}

	public float IdealDistance
	{
		get
		{
			return idealDistance;
		}
		set
		{
			idealDistance = Mathf.Clamp(value, minDistance, maxDistance);
		}
	}

	public void ZoomIn()
	{
		if (!IsZoomedIn() && mFocusMission != null)
		{
			FocusAndZoomOnMission(mFocusMission);
		}
	}

	public void ZoomOut()
	{
		if (IsZoomedIn() && IdealDistance != maxDistance)
		{
			GlobeSFX.Instance.ZoomOutWithB.Play2D();
			IdealDistance = maxDistance;
		}
	}

	public void GotoMission(MissionListings.eMissionID missionID)
	{
		SelectableMissionMarker[] array = Object.FindObjectsOfType(typeof(SelectableMissionMarker)) as SelectableMissionMarker[];
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].Data != null && array[i].Data.MissionId == missionID)
			{
				mFocusMission = array[i];
				break;
			}
		}
	}

	public void BlockInput(bool blocked)
	{
		if (blocked != mBlockedInput)
		{
			SelectableMissionMarker.HideAllMissionMarkers(blocked);
		}
		mBlockedInput = blocked;
		if (blocked)
		{
			mTouching = false;
		}
	}

	private void Start()
	{
		float num2 = (IdealDistance = initialDistance);
		distance = num2;
		mTouching = false;
		Vector3 vector = target.position - base.transform.position;
		mDesiredPosition = target.position - distance * vector.normalized;
		mDesiredUp = Vector3.up;
		mLastKnownDirection = -1f;
		if ((bool)base.rigidbody)
		{
			base.rigidbody.freezeRotation = true;
		}
		ClearDeltaHistory();
		Apply();
	}

	private void ClearDeltaHistory()
	{
		for (int i = 0; i < 8; i++)
		{
			m_LastDelta[i] = new DragHistoryItem(0f, 0f);
		}
	}

	private void OnEnable()
	{
		FingerGestures.OnDragMove += FingerGestures_OnDragMove;
		FingerGestures.OnDragEnd += FingerGestures_OnDragEnd;
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
	}

	private void OnDisable()
	{
		FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
		FingerGestures.OnDragEnd -= FingerGestures_OnDragEnd;
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	private void FingerGestures_OnFingerDown(int index, Vector2 fingerPos)
	{
		ClearDeltaHistory();
		smoothOrbitSpeed = 24f;
		if (index == 0)
		{
			mTimeSinceLastTouch = 0f;
			mTouching = true;
		}
	}

	private void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
	{
		if (!mBlockedInput)
		{
			mTimeSinceLastTouch = 0f;
			if (!IsZoomedIn() || AllowZoomedInMovement)
			{
				mDesiredPosition -= base.transform.right * (delta.x * YawSensitivity);
				mDesiredPosition -= base.transform.up * (delta.y * PitchSensitivity);
				mLastKnownDirection = ((!(delta.x > 0f)) ? 1f : (-1f));
				mTouching = true;
				SpinSFXTickUp();
			}
		}
	}

	private void FingerGestures_OnDragEnd(Vector2 fingerPos)
	{
		if (!mBlockedInput)
		{
			mTimeSinceLastTouch = 0f;
			mTouching = false;
		}
	}

	public bool IsZoomedIn()
	{
		return IdealDistance != maxDistance;
	}

	public bool IsZooming()
	{
		return maxDistance - distance > 0.001f;
	}

	public void ClearFocusMission()
	{
		mFocusMission = null;
	}

	private void FocusAndZoomOnMission(SelectableMissionMarker mission)
	{
		GlobeSFX.Instance.ZoomInWithC.Play2D();
		Vector3 vector = target.position - mission.transform.position;
		mDesiredPosition = target.position - distance * vector.normalized;
		IdealDistance = SelectableMissionMarker.Radius + mission.FrameCamRadiusOffs;
	}

	public void HighlightMission(SelectableMissionMarker mission)
	{
		Vector3 vector = target.position - mission.transform.position;
		mHighlightPosition = target.position - distance * vector.normalized;
		mTimeSinceLastTouch = 3f;
		mHighlighting = true;
	}

	private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		if (mBlockedInput || ActivateWatcher.Instance.ActivateUIOpen)
		{
			return;
		}
		mTimeSinceLastTouch = 0f;
		Ray ray = base.camera.ScreenPointToRay(fingerPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity) && (bool)hitInfo.collider.gameObject)
		{
			SelectableMissionMarker component = hitInfo.collider.gameObject.GetComponent<SelectableMissionMarker>();
			if (component != null && component.IsBlipActive() && component.Data != null && (component.Data.Type != MissionData.eType.MT_STORY || !component.Data.IsLocked()))
			{
				FocusAndZoomOnMission(component);
				mFocusMission = component;
				return;
			}
			if (component != null && component.IsBlipActive() && component.Data != null && component.Data.IsLocked())
			{
				MenuSFX.Instance.MissionLocked.Play2D();
			}
		}
		if (fingerIndex == 0)
		{
			mTouching = false;
		}
	}

	private void Apply()
	{
		distance = Mathf.Lerp(distance, IdealDistance, Time.deltaTime * smoothZoomSpeed);
		if (IsZooming() || IsZoomedIn())
		{
			mDesiredUp = Vector3.Lerp(mDesiredUp, Vector3.up, Time.deltaTime * RotateToTargetSpeed);
			distance = Mathf.Lerp(distance, IdealDistance + Mathf.Sin(Time.time) * 10f, Time.deltaTime * smoothZoomSpeed);
			Vector3 vector = mDesiredPosition;
			mDesiredPosition.x += Mathf.Sin(Time.time) * 15f;
			mDesiredPosition.y += Mathf.Cos(Time.time * 0.4f) * 15f;
			mDesiredPosition.z += Mathf.Sin(Time.time * 1.2f) * 15f;
			UpdatePositionAndOrientation(RotateToTargetSpeed);
			mDesiredPosition = vector;
			return;
		}
		Vector3 position = base.transform.position;
		UpdatePositionAndOrientation(LerpSpeed);
		if (!mTouching)
		{
			Vector3 vector2 = base.transform.position - position;
			mDesiredPosition += vector2 * PositionDamping;
			if (mDesiredPosition.y != DefaultHeight && mTimeSinceLastTouch >= 3f)
			{
				mDesiredUp = Vector3.Lerp(mDesiredUp, Vector3.up, Time.deltaTime * LerpUpSpeed);
				Vector3 to = mDesiredPosition;
				to.y = DefaultHeight;
				mDesiredPosition = Vector3.Lerp(mDesiredPosition, to, Time.deltaTime * AdjustPositionSpeed);
			}
			if (mHighlighting && mTimeSinceLastTouch >= 3f)
			{
				mDesiredPosition = Vector3.Lerp(mDesiredPosition, mHighlightPosition, Time.deltaTime * AdjustPositionSpeed);
			}
			else if (mTimeSinceLastTouch >= 3f || (mTimeSinceLastTouch >= 0.4f && vector2.sqrMagnitude < 0.2f))
			{
				mDesiredPosition += base.transform.right * (mLastKnownDirection * IdlingSpeed * Time.deltaTime);
			}
		}
	}

	private void UpdatePositionAndOrientation(float lerpSpeed)
	{
		Vector3 vector = target.position - mDesiredPosition;
		mDesiredPosition = target.position - distance * vector.normalized;
		Vector3 vector2 = Vector3.Lerp(base.transform.position, mDesiredPosition, Time.deltaTime * lerpSpeed);
		vector = target.position - vector2;
		base.transform.position = target.position - distance * vector.normalized;
		vector = target.position - base.transform.position;
		base.transform.rotation = Quaternion.LookRotation(vector.normalized, mDesiredUp);
		mDesiredUp = base.transform.up;
	}

	private void LateUpdate()
	{
		mTimeSinceLastTouch += Time.deltaTime;
		Apply();
		SpinSFXTickDown();
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	private static float ClampPitch(float pitch)
	{
		while (pitch < -90f)
		{
			pitch += 360f;
		}
		while (pitch >= 270f)
		{
			pitch -= 360f;
		}
		return pitch;
	}

	private void SpinSFXTickUp()
	{
		if (mSpinSfxVol == 0f && !ActivateWatcher.Instance.ActivateUIOpen)
		{
			GlobeSFX.Instance.GlobeSpin.Play2D();
		}
		mSpinSfxVol += Time.deltaTime * 5f;
		mSpinSfxVol = Mathf.Clamp(mSpinSfxVol, 0f, 1f);
		float volume = GlobeSFX.Instance.GlobeSpin.m_volume * mSpinSfxVol;
		SoundManager.Instance.SetVolume(GlobeSFX.Instance.GlobeSpin, SoundManager.Instance.gameObject, volume);
	}

	private void SpinSFXTickDown()
	{
		if (mSpinSfxVol != 0f)
		{
			mSpinSfxVol -= Time.deltaTime * 0.5f;
			if (mSpinSfxVol <= 0f)
			{
				StopSpinSFX();
				return;
			}
			float volume = GlobeSFX.Instance.GlobeSpin.m_volume * mSpinSfxVol;
			SoundManager.Instance.SetVolume(GlobeSFX.Instance.GlobeSpin, SoundManager.Instance.gameObject, volume);
		}
	}

	public void StopSpinSFX()
	{
		mSpinSfxVol = 0f;
		GlobeSFX.Instance.GlobeSpin.Stop2D();
	}
}
