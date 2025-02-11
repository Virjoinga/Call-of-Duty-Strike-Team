using UnityEngine;

public class CameraFingerDrag : CameraBase, PlayCameraInterface
{
	public Transform target;

	private Vector3 mLastTargetPosition;

	public float initialDistance = 10f;

	public float minDistance = 1f;

	public float maxDistance = 20f;

	public float yawSensitivity = 80f;

	public float pitchSensitivity = 80f;

	public bool clampPitchAngle = true;

	public float minPitch = -20f;

	public float maxPitch = 80f;

	public bool clampYawAngle = true;

	public float yawDefault = 90f;

	public float yawClamp = 35f;

	public bool allowPinchZoom = true;

	public float pinchZoomSensitivity = 2f;

	public bool smoothMotion = true;

	public float smoothZoomSpeed = 3f;

	public float smoothOrbitSpeed = 4f;

	public float OrbitVelocityDamping = 0.9f;

	public bool allowPanning;

	public bool invertPanningDirections;

	public float panningSensitivity = 1f;

	public bool smoothPanning = true;

	public float smoothPanningSpeed = 8f;

	public float PanVelocityDamping = 0.9f;

	private float lastPanTime;

	private float lastOrbitTime;

	private float mTwoFingersReleaseTime = -1f;

	private float distance = 10f;

	private float yaw;

	private float pitch;

	private float idealDistance;

	private float idealYaw;

	private float idealPitch;

	private Vector3 idealPanOffset = Vector3.zero;

	private Vector3 panOffset = Vector3.zero;

	private Vector3 panVelocity = Vector3.zero;

	private Vector2 orbitVelocity = Vector3.zero;

	public float LookAheadOffset;

	private GameObject mDummyTarget;

	private bool mInputEnabled;

	private float mRealTimeLastFrame;

	private Vector3 mLastKnownGoodPosition = Vector3.zero;

	private Quaternion mLastKnownGoodRotation = Quaternion.identity;

	private bool mGoToLastKnownGoodWhenPossible;

	private Vector3 mIdleOffset;

	private float mIdleFovRestore;

	private float mIdleStartTime;

	private Transform mDontTargetCheck;

	private float mTimeToSwitchFomTarget;

	public float TimeToStayOnDeadGuy = 2f;

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
			idealYaw = ((!clampYawAngle) ? value : ClampAngle(value, yawDefault - yawClamp, yawDefault + yawClamp));
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

	public Vector3 IdealPanOffset
	{
		get
		{
			return idealPanOffset;
		}
		set
		{
			idealPanOffset = value;
		}
	}

	public Vector3 PanOffset
	{
		get
		{
			return panOffset;
		}
		set
		{
			panOffset = value;
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

	public override Vector3 TargetPoint()
	{
		return target.transform.position;
	}

	private void Start()
	{
		mInputEnabled = false;
		Vector3 eulerAngles = base.transform.eulerAngles;
		float num2 = (IdealDistance = initialDistance);
		distance = num2;
		num2 = (IdealYaw = eulerAngles.y);
		yaw = num2;
		num2 = (IdealPitch = eulerAngles.x);
		pitch = num2;
		mIdleOffset = Vector3.zero;
		mIdleFovRestore = Fov;
		mIdleStartTime = 0f;
		mRealTimeLastFrame = Time.realtimeSinceStartup;
		mDummyTarget = new GameObject("dummyCamTarget");
		if (!target)
		{
			target = mDummyTarget.transform;
		}
		mDontTargetCheck = null;
		mTimeToSwitchFomTarget = 0f;
		Apply();
		AllowInput(true);
		GameplayController gameplayController = GameplayController.Instance();
		gameplayController.OnDeath += OnDeath;
	}

	private void OnDestroy()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController != null)
		{
			gameplayController.OnDeath -= OnDeath;
		}
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
		if (!mInputEnabled)
		{
			mInputEnabled = true;
			InputManager.Instance.AddOnPinchMoveEventHandler(FingerGestures_OnPinchMove, 0);
			InputManager.Instance.AddOnTwoFingerDragBeginEventHandler(FingerGestures_OnTwoFingerDragBegin, 0);
			InputManager.Instance.AddOnTwoFingerDragMoveEventHandler(FingerGestures_OnTwoFingerDragMove, 0);
			InputManager.Instance.AddOnTwoFingerDragEndEventHandler(ObjectSelecter_OnTwoFingerDragEnd, 0);
			InputManager.Instance.AddOnFingerSwipeEventHandler(ObjectSelecter_OnFingerSwipe, 0);
			InputManager.Instance.AddOnFingerDragBeginEventHandler(ObjectSelecter_OnFingerDragBegin, 0);
			InputManager.Instance.AddOnFingerDragEndEventHandler(ObjectSelecter_OnFingerDragEnd, 0);
			InputManager.Instance.AddOnFingerDragMoveEventHandler(ObjectSelecter_OnFingerDragMove, 0);
			InputManager.Instance.AddOnRotateMoveEventHandler(Camera_OnRotateMove, 0);
		}
	}

	private void DisableInput()
	{
		if (mInputEnabled)
		{
			mInputEnabled = false;
			if ((bool)InputManager.Instance)
			{
				InputManager.Instance.RemoveOnPinchMoveEventHandler(FingerGestures_OnPinchMove);
				InputManager.Instance.RemoveOnTwoFingerDragBeginEventHandler(FingerGestures_OnTwoFingerDragBegin);
				InputManager.Instance.RemoveOnTwoFingerDragMoveEventHandler(FingerGestures_OnTwoFingerDragMove);
				InputManager.Instance.RemoveOnTwoFingerDragEndEventHandler(ObjectSelecter_OnTwoFingerDragEnd);
				InputManager.Instance.RemoveOnFingerSwipeEventHandler(ObjectSelecter_OnFingerSwipe);
				InputManager.Instance.RemoveOnFingerDragBeginEventHandler(ObjectSelecter_OnFingerDragBegin);
				InputManager.Instance.RemoveOnFingerDragEndEventHandler(ObjectSelecter_OnFingerDragEnd);
				InputManager.Instance.RemoveOnFingerDragMoveEventHandler(ObjectSelecter_OnFingerDragMove);
				InputManager.Instance.RemoveOnRotateMoveEventHandler(Camera_OnRotateMove);
			}
		}
	}

	private void DoPan(Vector2 delta)
	{
		if (!allowPanning)
		{
			return;
		}
		panVelocity = delta;
		if (!(panVelocity == Vector3.zero))
		{
			Vector3 vector = base.gameObject.transform.forward + base.gameObject.transform.up;
			vector.y = 0f;
			vector.Normalize();
			Vector3 vector2 = -0.02f * panningSensitivity * (base.gameObject.transform.right * delta.x + vector * delta.y);
			if (invertPanningDirections)
			{
				IdealPanOffset -= vector2;
			}
			else
			{
				IdealPanOffset += vector2;
			}
			lastPanTime = Time.realtimeSinceStartup;
		}
	}

	private void DoOrbit(Vector2 delta)
	{
		if ((bool)target)
		{
			orbitVelocity = delta;
			IdealYaw += delta.x * yawSensitivity * 0.02f;
			IdealPitch -= delta.y * pitchSensitivity * 0.02f;
			lastOrbitTime = Time.realtimeSinceStartup;
		}
	}

	private void Camera_OnRotateMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
		IdealYaw += rotationAngleDelta;
	}

	private void FingerGestures_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
		if (allowPinchZoom)
		{
			IdealDistance -= delta * pinchZoomSensitivity;
		}
	}

	private void FingerGestures_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
		mTwoFingersReleaseTime = -1f;
	}

	private void FingerGestures_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
	}

	private void ObjectSelecter_OnTwoFingerDragEnd(Vector2 fingerPos)
	{
		mTwoFingersReleaseTime = Time.realtimeSinceStartup;
	}

	private void ObjectSelecter_OnFingerSwipe(int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
		if (fingerIndex == 0 && !IsInTwoFingerDrag())
		{
			GameplayController gameplayController = GameplayController.Instance();
			switch (direction)
			{
			case FingerGestures.SwipeDirection.Right:
			case FingerGestures.SwipeDirection.Left:
				OrdersHelper.OrderHoldPosition(gameplayController);
				break;
			case FingerGestures.SwipeDirection.Down:
				OrdersHelper.OrderCrouch(gameplayController);
				break;
			case FingerGestures.SwipeDirection.Up:
				OrdersHelper.OrderStand(gameplayController);
				break;
			}
		}
	}

	private void ObjectSelecter_OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
	{
	}

	private void ObjectSelecter_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (InputManager.Instance.NumFingersActive <= 1)
		{
			if (fingerIndex == 0)
			{
				DoPan(delta);
				FocusOnDummy(false);
			}
			if (fingerIndex == 1 && Input.GetKey(KeyCode.LeftShift))
			{
				DoPan(delta);
				FocusOnDummy(false);
			}
			else if (fingerIndex == 1)
			{
				DoOrbit(delta);
			}
		}
	}

	private void ObjectSelecter_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
	}

	public void FocusOnDummy(bool keepPanPos)
	{
		Transform transform = mDummyTarget.transform;
		if (transform != target && target != null)
		{
			Vector3 vector = target.position;
			RealCharacter component = target.gameObject.GetComponent<RealCharacter>();
			if (component != null)
			{
				vector = component.GetCameraTargetPosition();
			}
			mDummyTarget.transform.position = vector;
			mDummyTarget.transform.rotation = target.rotation;
			panOffset -= transform.position - vector;
			target = transform;
			if (keepPanPos)
			{
				IdealPanOffset = Vector3.zero;
			}
			mLastTargetPosition = target.position;
		}
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

	public void SnapToTarget(Transform trans)
	{
		FocusOnTarget(trans, true);
	}

	public bool FocusOnTarget(Transform trans, bool now)
	{
		if (target == null)
		{
			target = trans;
		}
		Actor component = trans.gameObject.GetComponent<Actor>();
		if (!WorldHelper.IsPlayerControlledActor(component))
		{
			return false;
		}
		Vector3 vector = trans.position;
		if (component != null)
		{
			vector = component.realCharacter.GetCameraTargetPosition();
		}
		Vector3 vector2 = target.position;
		component = target.gameObject.GetComponent<Actor>();
		if (component != null)
		{
			vector2 = component.realCharacter.GetCameraTargetPosition();
		}
		bool flag = false;
		Vector3 vector3 = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(trans.position);
		if (vector3.x > 0f && vector3.x < 1f && vector3.y > 0f && vector3.y < 1f)
		{
			flag = true;
		}
		if (trans != target)
		{
			panOffset -= vector - vector2;
		}
		target = trans;
		if (!flag || now)
		{
			IdealPanOffset = Vector3.zero;
		}
		else
		{
			IdealPanOffset = panOffset;
		}
		mLastTargetPosition = vector;
		FocusOnDummy(false);
		return true;
	}

	public void AdditionalySelectTarget(GameObject selection, bool toggle)
	{
		Actor component = selection.GetComponent<Actor>();
		if (!WorldHelper.IsPlayerControlledActor(component))
		{
			return;
		}
		GameplayController gameplayController = GameplayController.Instance();
		if (toggle && gameplayController.IsSelected(component))
		{
			if (gameplayController.Selected.Count != 1)
			{
				bool flag = false;
				if (gameplayController.IsSelectedLeader(component))
				{
					flag = true;
				}
				gameplayController.RemoveFromSelected(component);
				if (flag)
				{
					FocusOnTarget(gameplayController.Selected[0].transform);
				}
			}
		}
		else
		{
			gameplayController.AddToSelected(component);
		}
	}

	private bool IsInTwoFingerDrag()
	{
		if (mTwoFingersReleaseTime < 0f || Time.realtimeSinceStartup - mTwoFingersReleaseTime <= 0.25f)
		{
			return true;
		}
		return false;
	}

	private void Apply()
	{
		if (mGoToLastKnownGoodWhenPossible && InputManager.Instance.NumFingersActive == 0)
		{
			idealPanOffset = mLastKnownGoodPosition;
			IdealPitch = mLastKnownGoodRotation.eulerAngles.x;
			IdealYaw = mLastKnownGoodRotation.eulerAngles.y;
		}
		float num = Time.realtimeSinceStartup - mRealTimeLastFrame;
		mRealTimeLastFrame = Time.realtimeSinceStartup;
		if (lastPanTime != Time.realtimeSinceStartup)
		{
			DoPan(panVelocity);
			panVelocity *= PanVelocityDamping;
		}
		if (lastOrbitTime != Time.realtimeSinceStartup)
		{
			DoOrbit(orbitVelocity);
			orbitVelocity *= OrbitVelocityDamping;
		}
		if (target != null && (mLastTargetPosition - target.position).sqrMagnitude > 0f && !IsInTwoFingerDrag())
		{
			idealPanOffset = Vector3.zero;
		}
		if (smoothMotion)
		{
			distance = Mathf.Lerp(distance, IdealDistance, num * smoothZoomSpeed);
			yaw = Mathf.Lerp(yaw, IdealYaw, num * smoothOrbitSpeed);
			pitch = Mathf.Lerp(pitch, IdealPitch, num * smoothOrbitSpeed);
		}
		else
		{
			distance = IdealDistance;
			yaw = IdealYaw;
			pitch = IdealPitch;
		}
		if (smoothPanning)
		{
			panOffset = Vector3.Lerp(panOffset, idealPanOffset, num * smoothPanningSpeed);
		}
		else
		{
			panOffset = idealPanOffset;
		}
		UpdateIdleMovement(num);
		Vector3 to = mLastTargetPosition;
		if (target != null)
		{
			to = target.position;
			RealCharacter component = target.gameObject.GetComponent<RealCharacter>();
			if (component != null)
			{
				to = component.GetCameraTargetPosition();
			}
		}
		mLastTargetPosition = Vector3.Lerp(mLastTargetPosition, to, num * smoothPanningSpeed);
		base.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
		base.transform.position = mLastTargetPosition + panOffset + mIdleOffset - distance * base.transform.forward;
	}

	private void UpdateIdleMovement(float realDeltaTime)
	{
		if (panOffset == idealPanOffset && distance == IdealDistance && yaw == IdealYaw && pitch == IdealPitch)
		{
			if (mIdleStartTime < 0f)
			{
				mIdleStartTime = Time.time;
			}
			float num = 0.2f;
			float num2 = 5f;
			float value = Time.time - mIdleStartTime;
			float num3 = Mathf.Clamp(value, 0f, num2);
			float currentVelocity = 0f;
			Fov = Mathf.SmoothDamp(Fov, mIdleFovRestore + Mathf.Sin(Time.time * 0.5f) * 1f, ref currentVelocity, (num2 - num3) * (1f - num) + num);
			Vector3 currentVelocity2 = Vector3.zero;
			mIdleOffset = Vector3.SmoothDamp(mIdleOffset, base.transform.right * Mathf.Sin(Time.time * 0.1f), ref currentVelocity2, (num2 - num3) * (1f - num) + num);
		}
		else
		{
			mIdleStartTime = -1f;
			Fov = Mathf.Lerp(Fov, mIdleFovRestore, realDeltaTime * smoothPanningSpeed);
			mIdleOffset = Vector3.Lerp(mIdleOffset, Vector3.zero, realDeltaTime * smoothPanningSpeed);
		}
	}

	private void Update()
	{
		if (mDontTargetCheck != null && Time.realtimeSinceStartup > mTimeToSwitchFomTarget)
		{
			if (target.Equals(mDontTargetCheck))
			{
				SelectNearestCharacter();
			}
			mDontTargetCheck = null;
		}
	}

	private void LateUpdate()
	{
		Apply();
	}

	private void SelectNearestCharacter()
	{
		float num = float.MaxValue;
		GameObject gameObject = null;
		Vector3 position = base.transform.position;
		if (target != null)
		{
			position = target.position;
		}
		GameplayController gameplayController = GameplayController.Instance();
		Actor[] array = gameplayController.Selected.ToArray();
		Actor[] array2 = array;
		foreach (Actor actor in array2)
		{
			GameObject gameObject2 = actor.gameObject;
			if (!actor.realCharacter.IsDead() && actor.behaviour != null && actor.behaviour.PlayerControlled)
			{
				float sqrMagnitude = (gameObject2.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					gameObject = gameObject2;
				}
			}
		}
		if (gameObject != null)
		{
			FocusAndSelectTarget(gameObject.transform);
		}
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

	public void ResetPanning()
	{
		IdealPanOffset = Vector3.zero;
	}

	public override void OnTriggerEnter(Collider other)
	{
		mGoToLastKnownGoodWhenPossible = false;
		mLastKnownGoodPosition = PanOffset;
		mLastKnownGoodRotation = base.transform.rotation;
		panVelocity = Vector3.zero;
	}

	public override void OnTriggerExit(Collider other)
	{
		mGoToLastKnownGoodWhenPossible = true;
	}

	public override void OnTriggerStay(Collider other)
	{
		mGoToLastKnownGoodWhenPossible = false;
		mLastKnownGoodPosition = PanOffset;
		mLastKnownGoodRotation = base.transform.rotation;
	}

	private void OnDeath(object sender, HealthComponent.HeathChangeEventArgs hce)
	{
		RealCharacter realCharacter = sender as RealCharacter;
		if (!(realCharacter == null) && realCharacter.transform == target)
		{
			SwitchAwayFromTargetAtTime(target, Time.realtimeSinceStartup + TimeToStayOnDeadGuy);
		}
	}

	private void SwitchAwayFromTargetAtTime(Transform cachedTarget, float time)
	{
		mDontTargetCheck = cachedTarget;
		mTimeToSwitchFomTarget = time;
	}
}
