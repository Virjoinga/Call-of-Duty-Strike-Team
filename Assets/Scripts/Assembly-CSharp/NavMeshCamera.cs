using System;
using UnityEngine;

public class NavMeshCamera : CameraBase, PlayCameraInterface
{
	private const float MIN_FOV = 1f;

	private const float MAX_FOV = 180f;

	private const float PLACEMENT_PAN_FWD_SPEED = 1f;

	private const float PLACEMENT_PAN_SDE_SPEED = 2f;

	private const float PLACEMENT_PAN_BCK_SPEED = 3f;

	public float yawSensitivity = 15f;

	public float pitchSensitivity = 15f;

	public bool clampPitchAngle = true;

	public float minPitch = 50f;

	public float maxPitch = 50f;

	public bool allowPinchZoom;

	public float pinchZoomSensitivity = 0.2f;

	public float WorldMoveSpeed = 0.02f;

	public bool allowOrbit = true;

	public float smoothZoomSpeed = 3f;

	public float smoothOrbitSpeed = 8f;

	private float mOriginalOrbitSpeed = 8f;

	public float OrbitVelocityDamping = 0.9f;

	public bool allowPanning = true;

	public float panningSensitivity = 1f;

	public float smoothPanningSpeed = 8f;

	public float PanVelocityDamping = 0.9f;

	public Vector3 mLastPanDirection;

	private Vector3 mWorldSpacePan;

	private static float mWorldSpacePanScale;

	private static float mTargetWorldSpacePanScale;

	private float lastPanTime;

	private float lastOrbitTime;

	private float yaw;

	private float pitch;

	private float idealYaw;

	private float idealPitch;

	private Vector3 panVelocity = Vector3.zero;

	private Vector2 orbitVelocity = Vector3.zero;

	private Vector3 mLastFingerPos = Vector3.zero;

	private Rect mScreenCentre;

	private int mCameraNavMeshMask;

	private bool mInputEnabled;

	private float mRealTimeLastFrame;

	private NavMeshAgent mNavAgent;

	private float mTargetingDist = 20f;

	private float mTargetingAdjacent;

	private Vector3 mPanMove;

	private Vector3 mIdealFocusPoint;

	private bool mIsFocusingOnPoint;

	private float mSpeedToFocusPoint = 0.3f;

	private float mLastSpeedToFocusPoint;

	private float mLastDistToFocus;

	private bool mBeginRotateMove;

	private Vector3 mRotateWorldPos = default(Vector3);

	private bool mRotate;

	private int mBeginPinch;

	private bool disableFingerDrag;

	private float IdealFov;

	private float MinFov;

	private float MaxFov;

	private bool mIsZooming;

	private bool mPlacementPan;

	private float mNormalisedZoom = 1f;

	private bool mSuppressPanMove;

	public bool OrbitSpeedAltered { get; set; }

	public float Yaw
	{
		get
		{
			return yaw;
		}
	}

	public float IdealYaw
	{
		get
		{
			return idealYaw;
		}
		set
		{
			idealYaw = value;
		}
	}

	public float Pitch
	{
		get
		{
			return pitch;
		}
	}

	public float IdealPitch
	{
		get
		{
			return idealPitch;
		}
		set
		{
			idealPitch = ((!clampPitchAngle) ? value : ClampAngle(value, minPitch, maxPitch));
		}
	}

	public Vector3 PanOffset
	{
		get
		{
			if (!mIsFocusingOnPoint)
			{
				return Vector3.zero;
			}
			Vector3 result = mIdealFocusPoint - (base.transform.position + mTargetingDist * base.transform.forward);
			result.y = 0f;
			return result;
		}
	}

	public Vector3 OrbitVelocity
	{
		get
		{
			return orbitVelocity;
		}
	}

	public Vector3 PanVelocity
	{
		get
		{
			return panVelocity;
		}
	}

	public override float Fov
	{
		get
		{
			return mFov;
		}
		set
		{
			mFov = value;
			mNormalisedZoom = (mFov - MinFov) / (MaxFov - MinFov);
		}
	}

	public bool SuppressPanMove
	{
		get
		{
			return mSuppressPanMove;
		}
		set
		{
			mSuppressPanMove = value;
			mPanMove = Vector3.zero;
		}
	}

	public float SpeedToFocusPoint
	{
		get
		{
			return mSpeedToFocusPoint;
		}
		set
		{
			mLastSpeedToFocusPoint = mSpeedToFocusPoint;
			mSpeedToFocusPoint = value;
		}
	}

	public bool MovingToFocusPoint
	{
		get
		{
			return IsFocusingOnPoint();
		}
	}

	public float WorldRotation
	{
		get
		{
			return base.transform.eulerAngles.y;
		}
	}

	public static void EnablePushing()
	{
		mTargetWorldSpacePanScale = 1f;
	}

	public static void DisablePushing()
	{
		mTargetWorldSpacePanScale = 0f;
	}

	public static void ZeroPush()
	{
		mWorldSpacePanScale = -1f;
	}

	public float GetNormalisedZoom()
	{
		return mNormalisedZoom;
	}

	public override Vector3 TargetPoint()
	{
		return base.transform.position + mTargetingDist * base.transform.forward;
	}

	private bool AttachToNavMesh(int cameraNavMeshLayer)
	{
		mCameraNavMeshMask = 1 << cameraNavMeshLayer;
		NavMeshHit navMeshHit = NavMeshUtils.SampleNavMesh(base.transform.position, mCameraNavMeshMask);
		if (navMeshHit.hit)
		{
			GameObject gameObject = new GameObject("NavCamParent");
			gameObject.transform.position = navMeshHit.position;
			gameObject.transform.parent = base.transform.parent;
			base.transform.parent = gameObject.transform;
			mTargetingAdjacent = mTargetingDist / Mathf.Cos((float)Math.PI / 180f * Pitch);
			base.transform.position = new Vector3(0f, 0f, 0f);
			SetCameraRelativeToTarget();
			mNavAgent = gameObject.AddComponent<NavMeshAgent>();
			mNavAgent.updateRotation = false;
			mNavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
			mNavAgent.walkableMask = 1 << cameraNavMeshLayer;
			mNavAgent.autoRepath = false;
			mNavAgent.autoTraverseOffMeshLink = false;
			mNavAgent.radius = 2f;
			return true;
		}
		mNavAgent = null;
		return false;
	}

	private void Start()
	{
		CameraTarget cameraTarget = UnityEngine.Object.FindObjectOfType(typeof(CameraTarget)) as CameraTarget;
		if (cameraTarget != null)
		{
			cameraTarget.GameCamera = this;
			base.transform.position = cameraTarget.transform.position;
			base.transform.position += new Vector3(0f, 20f, 0f);
		}
		Vector3 eulerAngles = base.transform.eulerAngles;
		float num = (IdealYaw = eulerAngles.y);
		yaw = num;
		num = (IdealPitch = eulerAngles.x);
		pitch = num;
		int navMeshLayerFromName = NavMesh.GetNavMeshLayerFromName("Camera");
		if (!AttachToNavMesh(navMeshLayerFromName))
		{
		}
		mInputEnabled = false;
		mIdealFocusPoint = Vector3.zero;
		mIsFocusingOnPoint = false;
		mLastDistToFocus = 0f;
		mBeginRotateMove = false;
		mBeginPinch = 0;
		disableFingerDrag = false;
		IdealFov = Fov;
		MinFov = Fov - 20f;
		MaxFov = Fov + 10f;
		MissionSetup instance = MissionSetup.Instance;
		if (instance != null)
		{
			if (TBFUtils.IsSmallScreenDevice())
			{
				MinFov = instance.SmallScreenMinFov;
				MaxFov = instance.SmallScreenMaxFov;
				num = (Fov = instance.SmallScreenStartFov);
				IdealFov = num;
			}
			else
			{
				MinFov = instance.LargeScreenMinFov;
				MaxFov = instance.LargeScreenMaxFov;
				num = (Fov = instance.LargeScreenStartFov);
				IdealFov = num;
			}
			IdealFov = Mathf.Clamp(IdealFov, MinFov, MaxFov);
		}
		mRealTimeLastFrame = Time.realtimeSinceStartup;
		Apply();
		AllowInput(true);
		mScreenCentre = new Rect((float)Screen.width * 0.1f, (float)Screen.height * 0.1f, (float)Screen.width * 0.8f, (float)Screen.height * 0.8f);
		mWorldSpacePan = Vector3.zero;
		mLastPanDirection = Vector3.zero;
		mWorldSpacePanScale = 1f;
		mTargetWorldSpacePanScale = 1f;
		mLastSpeedToFocusPoint = mSpeedToFocusPoint;
		mOriginalOrbitSpeed = smoothOrbitSpeed;
	}

	private void OnDestroy()
	{
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

	public override void EnablePlacementInput(bool enable)
	{
		if (enable)
		{
			DisableInput();
			InputManager.Instance.AddOnFingerDragMoveEventHandler(PlacementCamera_OnFingerDragMove, 0);
			InputManager.Instance.AddOnFingerDragEndEventHandler(PlacementCamera_OnFingerDragEnd, 0);
		}
		else
		{
			EnableInput();
			InputManager.Instance.RemoveOnFingerDragMoveEventHandler(PlacementCamera_OnFingerDragMove);
			InputManager.Instance.RemoveOnFingerDragEndEventHandler(PlacementCamera_OnFingerDragEnd);
			mPlacementPan = false;
		}
	}

	private void EnableInput()
	{
		if (!mInputEnabled)
		{
			mInputEnabled = true;
			InputManager.Instance.AddOnPinchBeginEventHandler(Camera_OnPinchBegin, 0);
			InputManager.Instance.AddOnPinchMoveEventHandler(Camera_OnPinchMove, 0);
			InputManager.Instance.AddOnTwoFingerDragBeginEventHandler(Camera_OnTwoFingerDragBegin, 0);
			InputManager.Instance.AddOnTwoFingerDragMoveEventHandler(Camera_OnTwoFingerDragMove, 0);
			InputManager.Instance.AddOnTwoFingerDragEndEventHandler(Camera_OnTwoFingerDragEnd, 0);
			InputManager.Instance.AddOnFingerDragMoveEventHandler(Camera_OnFingerDragMove, 0);
			InputManager.Instance.AddOnFingerDragEndEventHandler(Camera_OnFingerDragEnd, 0);
			InputManager.Instance.AddOnRotateBeginEventHandler(Camera_OnRotateBegin, 0);
			InputManager.Instance.AddOnRotateMoveEventHandler(Camera_OnRotateMove, 0);
			InputManager.Instance.AddOnRotateEndEventHandler(Camera_OnRotateEnd, 0);
			InputManager.Instance.AddOnFingerDownEventHandler(Camera_OnFingerDown, 0);
		}
	}

	private void DisableInput()
	{
		if (mInputEnabled)
		{
			mInputEnabled = false;
			if ((bool)InputManager.Instance)
			{
				InputManager.Instance.RemoveOnPinchBeginEventHandler(Camera_OnPinchBegin);
				InputManager.Instance.RemoveOnPinchMoveEventHandler(Camera_OnPinchMove);
				InputManager.Instance.RemoveOnTwoFingerDragBeginEventHandler(Camera_OnTwoFingerDragBegin);
				InputManager.Instance.RemoveOnTwoFingerDragMoveEventHandler(Camera_OnTwoFingerDragMove);
				InputManager.Instance.RemoveOnTwoFingerDragEndEventHandler(Camera_OnTwoFingerDragEnd);
				InputManager.Instance.RemoveOnFingerDragMoveEventHandler(Camera_OnFingerDragMove);
				InputManager.Instance.RemoveOnFingerDragEndEventHandler(Camera_OnFingerDragEnd);
				InputManager.Instance.RemoveOnRotateBeginEventHandler(Camera_OnRotateBegin);
				InputManager.Instance.RemoveOnRotateMoveEventHandler(Camera_OnRotateMove);
				InputManager.Instance.RemoveOnRotateEndEventHandler(Camera_OnRotateEnd);
				InputManager.Instance.RemoveOnFingerDownEventHandler(Camera_OnFingerDown);
			}
		}
	}

	private void DoPan(Vector2 fingerPos, Vector2 delta)
	{
		if (!allowPanning)
		{
			return;
		}
		panVelocity = delta;
		mLastFingerPos = fingerPos;
		if (panVelocity == Vector3.zero)
		{
			mPanMove = Vector3.zero;
			return;
		}
		Vector3 vector = base.gameObject.transform.forward + base.gameObject.transform.up;
		vector.y = 0f;
		vector.Normalize();
		Vector3 vector2 = ScreenToWorldPos(fingerPos - delta);
		Vector3 vector3 = ScreenToWorldPos(fingerPos);
		Vector3 vector4 = (vector3 - vector2).normalized * (delta.magnitude * WorldMoveSpeed * (1f + GetNormalisedZoom()));
		if (!SuppressPanMove)
		{
			mPanMove = -vector4;
		}
		if (TutorialToggles.LockCameraMoveX)
		{
			mPanMove.x = 0f;
		}
		if (TutorialToggles.LockCameraMoveZ)
		{
			mPanMove.z = 0f;
		}
		float magnitude = mPanMove.magnitude;
		if (magnitude > 0.1f)
		{
			mLastPanDirection = mPanMove / magnitude;
		}
		lastPanTime = Time.realtimeSinceStartup;
	}

	private void DoPlacementPan(Vector2 fingerPos, Vector2 delta)
	{
		if (!allowPanning)
		{
			return;
		}
		if (mPlacementPan)
		{
			delta.x = 0f;
			delta.y = 0f;
		}
		if (fingerPos.x < mScreenCentre.xMin)
		{
			delta.x -= 2f;
		}
		else if (mScreenCentre.xMax < fingerPos.x)
		{
			delta.x += 2f;
		}
		if (fingerPos.y < mScreenCentre.yMin)
		{
			delta.y -= 3f;
		}
		else if (mScreenCentre.yMax < fingerPos.y)
		{
			delta.y += 1f;
		}
		panVelocity = delta;
		mLastFingerPos = fingerPos;
		if (panVelocity == Vector3.zero)
		{
			mPanMove = Vector3.zero;
			return;
		}
		Vector3 vector = ScreenToWorldPos(fingerPos - delta);
		Vector3 vector2 = ScreenToWorldPos(fingerPos);
		Vector3 vector3 = vector2 - vector;
		if (!SuppressPanMove)
		{
			mPanMove = vector3;
		}
		float magnitude = mPanMove.magnitude;
		if (magnitude > 0.1f)
		{
			mLastPanDirection = mPanMove / magnitude;
		}
		lastPanTime = Time.realtimeSinceStartup;
	}

	public void WorldSpacePan(Vector2 delta)
	{
		mWorldSpacePan = new Vector3(delta.x, 0f, delta.y);
	}

	private void DoOrbit(Vector2 delta)
	{
		if (allowOrbit)
		{
			orbitVelocity = delta;
			float current = IdealYaw + delta.x * yawSensitivity * 0.02f;
			float num = Mathf.DeltaAngle(current, yaw);
			if (num > 4f)
			{
				allowPinchZoom = false;
				IdealYaw = current;
			}
			else
			{
				allowPinchZoom = true;
			}
			lastOrbitTime = Time.realtimeSinceStartup;
		}
	}

	private void Camera_OnRotateBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		if (allowOrbit)
		{
			if (OrbitSpeedAltered)
			{
				smoothOrbitSpeed = mOriginalOrbitSpeed;
				OrbitSpeedAltered = false;
			}
			mBeginRotateMove = true;
			disableFingerDrag = true;
			CalculateWorldRotatePos(fingerPos1, fingerPos2);
			mRotate = true;
		}
	}

	private void CalculateWorldRotatePos(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		Vector2 pos = (fingerPos1 + fingerPos2) * 0.5f;
		mRotateWorldPos = ScreenToWorldPos(pos);
	}

	private Vector3 ScreenToWorldPos(Vector2 pos)
	{
		Ray ray = CameraManager.Instance.PlayCamera.ScreenPointToRay(new Vector3(pos.x, pos.y, mTargetingDist));
		float distance = Mathf.Abs(ray.origin.y / ray.direction.y);
		return ray.GetPoint(distance);
	}

	private void Camera_OnRotateMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
		if (mBeginRotateMove)
		{
			mBeginRotateMove = false;
		}
		else if (allowOrbit)
		{
			IdealYaw += rotationAngleDelta;
			panVelocity = Vector3.zero;
		}
	}

	private void Camera_OnRotateEnd(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
		panVelocity = Vector3.zero;
		mRotate = false;
	}

	private Vector3 CurrentFocusPoint()
	{
		return base.transform.position + mTargetingDist * base.transform.forward;
	}

	private void Camera_OnPinchBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		mBeginPinch = 2;
	}

	private void Camera_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
		if (mBeginPinch > 0)
		{
			mBeginPinch--;
		}
		else if (allowPinchZoom)
		{
			mIdealFocusPoint = CurrentFocusPoint();
			mLastDistToFocus = 0f;
			mIsZooming = true;
			float num = IdealFov - delta * pinchZoomSensitivity;
			float num2 = pinchZoomSensitivity;
			float num3 = 10f;
			if (num > MaxFov)
			{
				float value = num - MaxFov;
				value = Mathf.Clamp(value, 0f, num3);
				num2 *= (num3 - value) / num3;
			}
			else if (num < MinFov)
			{
				float value2 = MinFov - num;
				value2 = Mathf.Clamp(value2, 0f, num3);
				num2 *= (num3 - value2) / num3;
			}
			IdealFov -= delta * num2;
			IdealFov = Mathf.Clamp(IdealFov, 1f, 180f);
		}
	}

	private void Camera_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
	}

	private void Camera_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
	}

	private void Camera_OnTwoFingerDragEnd(Vector2 fingerPos)
	{
	}

	private bool Camera_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		if (!IsFocusingOnPoint())
		{
			panVelocity = Vector3.zero;
		}
		return true;
	}

	private void Camera_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (!(InputManager.Instance == null) && !(CommonHudController.Instance == null) && InputManager.Instance.NumFingersActive <= 1 && !disableFingerDrag && !CommonHudController.Instance.ContextMenuActive && fingerIndex == 0 && !disableFingerDrag)
		{
			DoPan(fingerPos, delta);
		}
	}

	private void Camera_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
		mRotate = false;
	}

	private void PlacementCamera_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (InputManager.Instance.NumFingersActive <= 1 && !disableFingerDrag && !CommonHudController.Instance.ContextMenuActive && fingerIndex == 0 && !disableFingerDrag)
		{
			mPlacementPan = true;
			DoPlacementPan(fingerPos, delta);
		}
	}

	private void PlacementCamera_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
		mPlacementPan = false;
	}

	public void FocusAndSelectTarget(Transform trans)
	{
		if (FocusOnTarget(trans))
		{
			GameplayController gameplayController = GameplayController.Instance();
			gameplayController.SelectOnlyThis(trans.gameObject.GetComponent<Actor>());
		}
	}

	public bool FocusOnTarget(Transform trans)
	{
		return FocusOnTarget(trans, false);
	}

	public bool FocusOnTarget(Transform trans, bool forceEvenIfOnScreen)
	{
		if (TutorialToggles.LockCameraFocus)
		{
			return false;
		}
		Actor component = trans.GetComponent<Actor>();
		if (!WorldHelper.IsPlayerControlledActor(component))
		{
			return false;
		}
		bool flag = false;
		if (!forceEvenIfOnScreen)
		{
			Vector3 vector = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(trans.position);
			if (vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			FocusOnPoint(trans.position);
		}
		return true;
	}

	public void FocusOnPoint(Vector3 point)
	{
		if (!TutorialToggles.LockCameraFocus)
		{
			mIdealFocusPoint = point;
			mIsFocusingOnPoint = true;
			mLastDistToFocus = 0f;
		}
	}

	public void FocusOnPoint(Vector3 point, float heading)
	{
		if (!TutorialToggles.LockCameraFocus)
		{
			mIdealFocusPoint = point;
			mIsFocusingOnPoint = true;
			mLastDistToFocus = 0f;
			yaw = heading;
			IdealYaw = heading;
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, heading, base.transform.localEulerAngles.z);
			SetCameraRelativeToTarget();
		}
	}

	public void FocusOnPoint(Vector3 point, float heading, float fov)
	{
		if (!TutorialToggles.LockCameraFocus)
		{
			mIdealFocusPoint = point;
			mIsFocusingOnPoint = true;
			mLastDistToFocus = 0f;
			IdealYaw = heading;
			IdealFov = fov;
		}
	}

	public void SnapToTarget(Transform trans)
	{
		if (!TutorialToggles.LockCameraFocus)
		{
			mIdealFocusPoint = trans.position;
			mIsFocusingOnPoint = true;
			Vector3 vector = CurrentFocusPoint();
			vector = trans.position - vector;
			vector.y = 0f;
			base.transform.position = base.transform.position + vector;
		}
	}

	public void SnapToTarget(Transform trans, float heading, float fov)
	{
		if (!TutorialToggles.LockCameraFocus)
		{
			yaw = heading;
			IdealYaw = heading;
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, heading, base.transform.localEulerAngles.z);
			Fov = fov;
			IdealFov = fov;
			mIdealFocusPoint = trans.position;
			mLastDistToFocus = 0f;
			Vector3 vector = CurrentFocusPoint();
			vector = trans.position - vector;
			vector.y = 0f;
			base.transform.position = base.transform.position + vector;
		}
	}

	public bool IsFocusingOnPoint()
	{
		return mIsFocusingOnPoint;
	}

	public void ClearFocusingOnPoint()
	{
		mIsFocusingOnPoint = false;
		mLastDistToFocus = 0f;
	}

	private void SetCameraRelativeToTarget()
	{
		Quaternion quaternion = Quaternion.AngleAxis(base.transform.localEulerAngles.y, Vector3.up);
		Vector3 vector = new Vector3(0f, 0f, 0f - mTargetingAdjacent);
		base.transform.localPosition = quaternion * vector;
	}

	private void Apply()
	{
		float num = Time.realtimeSinceStartup - mRealTimeLastFrame;
		mRealTimeLastFrame = Time.realtimeSinceStartup;
		if (TimeManager.instance != null && TimeManager.instance.GlobalTimeState == TimeManager.State.IngamePaused)
		{
			panVelocity = Vector3.zero;
			orbitVelocity = Vector3.zero;
			return;
		}
		if (!allowPanning || SuppressPanMove)
		{
			panVelocity = Vector3.zero;
			orbitVelocity = Vector3.zero;
		}
		if (lastPanTime != Time.realtimeSinceStartup)
		{
			if (IsFocusingOnPoint())
			{
				float distanceToPoint = new Plane(base.transform.forward, base.transform.position).GetDistanceToPoint(mIdealFocusPoint);
				Vector3 vector = mIdealFocusPoint - base.transform.forward * distanceToPoint;
				Vector3 vector2 = base.transform.position - vector;
				float sqrMagnitude = vector2.sqrMagnitude;
				if ((double)sqrMagnitude < 0.01 || Mathf.Abs(mLastDistToFocus - sqrMagnitude) < 0.0001f)
				{
					ClearFocusingOnPoint();
					mSpeedToFocusPoint = mLastSpeedToFocusPoint;
				}
				else
				{
					mLastDistToFocus = sqrMagnitude;
					vector2 *= mSpeedToFocusPoint;
					panVelocity = new Vector2(vector2.x, vector2.z);
					mPanMove = -vector2;
				}
			}
			else if (mPlacementPan)
			{
				DoPlacementPan(mLastFingerPos, panVelocity);
			}
			else
			{
				DoPan(mLastFingerPos, panVelocity);
			}
			panVelocity *= PanVelocityDamping;
		}
		if (lastOrbitTime != Time.realtimeSinceStartup)
		{
			DoOrbit(orbitVelocity);
			orbitVelocity *= OrbitVelocityDamping;
		}
		float num2 = yaw;
		float num3 = pitch;
		yaw = Mathf.LerpAngle(yaw, IdealYaw, num * smoothOrbitSpeed);
		pitch = Mathf.Lerp(pitch, IdealPitch, num * smoothOrbitSpeed);
		Vector3 vector3 = ((!mRotate) ? (base.transform.position + mTargetingDist * base.transform.forward) : mRotateWorldPos);
		if (mNavAgent != null)
		{
			vector3 = mNavAgent.transform.position;
		}
		base.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
		Vector3 vector4 = base.transform.position - vector3;
		Matrix4x4 matrix4x = Matrix4x4.TRS(q: (!(mNavAgent != null)) ? Quaternion.Euler(pitch - num3, yaw - num2, 0f) : Quaternion.Euler(0f, yaw - num2, 0f), pos: Vector3.zero, s: new Vector3(1f, 1f, 1f));
		Vector3 vector5 = matrix4x * vector4;
		Vector3 vector6 = vector5 - vector4;
		mWorldSpacePanScale = mWorldSpacePanScale * 0.98f + ((!mInputEnabled) ? 0f : mTargetWorldSpacePanScale) * 0.02f;
		float num4 = Mathf.Max(0f, mWorldSpacePanScale);
		if (!mIsZooming && InputManager.Instance.NumFingersActive < 2)
		{
			IdealFov = Mathf.Clamp(IdealFov, MinFov, MaxFov);
		}
		Fov = Mathf.Lerp(Fov, IdealFov, 0.5f);
		if (InputManager.Instance.NumFingersActive == 0)
		{
			disableFingerDrag = false;
		}
		mIsZooming = false;
		if (mNavAgent != null)
		{
			mNavAgent.Move(mPanMove + mWorldSpacePan * num4);
			base.transform.position += vector6;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(base.transform.position, out hit, mTargetingDist * 2f, mCameraNavMeshMask))
			{
				float to = hit.position.y - mNavAgent.transform.position.y;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, Mathf.Lerp(base.transform.localPosition.y, to, 0.5f), base.transform.localPosition.z);
			}
		}
		else
		{
			base.transform.position += mPanMove + vector6 + mWorldSpacePan * num4;
		}
		mWorldSpacePan = Vector3.zero;
	}

	private void LateUpdate()
	{
		Apply();
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

	public override void AddShake(Vector3 origin, float radius, float duration)
	{
		Vector3 vector = Position - origin;
		vector.y = 0f;
		if (vector.sqrMagnitude < 625f)
		{
			AddShake(1.5f, duration);
		}
	}

	public override void AddShake(float scale, float duration)
	{
		Vector3 amount = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
		amount.Normalize();
		amount *= scale;
		base.gameObject.transform.parent.gameObject.ShakePosition(amount, duration, 0f);
	}

	public void ZoomOut()
	{
		IdealFov = 180f;
	}

	public void SetZoom(float fov)
	{
		IdealFov = fov;
	}
}
