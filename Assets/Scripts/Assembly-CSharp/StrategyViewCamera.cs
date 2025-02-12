using UnityEngine;

public class StrategyViewCamera : CameraBase
{
	public Transform target;

	public float initialDistance = 10f;

	public float minDistance = 1f;

	public float maxDistance = 20f;

	public float Pitch = 90f;

	public float Orientation;

	public bool allowPinchZoom = true;

	public float pinchZoomSensitivity = 2f;

	public bool smoothMotion = true;

	public float smoothZoomSpeed = 3f;

	public float smoothOrbitSpeed = 4f;

	public bool allowPanning;

	public Transform panningPlane;

	public bool smoothPanning = true;

	public float smoothPanningDecay = 0.9f;

	private bool mIsZooming;

	private float distance = 10f;

	private float idealDistance;

	public Vector3 mLastVelocity = Vector3.zero;

	private Vector3 idealPanOffset = Vector3.zero;

	private Vector3 panOffset = Vector3.zero;

	private bool mInputEnabled;

	private Vector3 centrePinchPos;

	private Vector3 mPanMove;

	private UnityEngine.AI.NavMeshAgent mNavAgent;

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

	public override Vector3 TargetPoint()
	{
		return target.transform.position;
	}

	private void Start()
	{
		if (MissionSetup.Instance != null)
		{
			Orientation = MissionSetup.Instance.MissionBriefingCameraOrientation;
		}
		if (!panningPlane)
		{
			panningPlane = base.transform;
		}
		CameraTarget cameraTarget = Object.FindObjectOfType(typeof(CameraTarget)) as CameraTarget;
		if (cameraTarget != null)
		{
			base.transform.position = cameraTarget.transform.position;
			base.transform.position += new Vector3(0f, initialDistance, 0f);
		}
		int navMeshLayerFromName = UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("StrategyCamera");
		if (!AttachToNavMesh(navMeshLayerFromName))
		{
		}
		mInputEnabled = false;
		float num2 = (IdealDistance = initialDistance);
		distance = num2;
		if ((bool)base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
		mIsZooming = false;
		mLastVelocity = Vector3.zero;
		Apply();
		AllowInput(true);
	}

	private bool AttachToNavMesh(int cameraNavMeshLayer)
	{
		UnityEngine.AI.NavMeshHit navMeshHit = NavMeshUtils.SampleNavMesh(base.transform.position, 1 << cameraNavMeshLayer);
		if (navMeshHit.hit)
		{
			GameObject gameObject = new GameObject("StrategyCamParent");
			gameObject.transform.position = navMeshHit.position;
			gameObject.transform.parent = base.transform.parent;
			base.transform.parent = gameObject.transform;
			base.transform.position = new Vector3(0f, 0f, 0f);
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			mNavAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
			mNavAgent.updateRotation = false;
			mNavAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
			mNavAgent.walkableMask = 1 << cameraNavMeshLayer;
			mNavAgent.autoRepath = false;
			mNavAgent.autoTraverseOffMeshLink = false;
			mNavAgent.radius = 2f;
			return true;
		}
		mNavAgent = null;
		return false;
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
			InputManager.Instance.AddOnPinchBeginEventHandler(FingerGestures_OnPinchBegin, 0);
			InputManager.Instance.AddOnPinchMoveEventHandler(FingerGestures_OnPinchMove, 0);
			InputManager.Instance.AddOnFingerDragMoveEventHandler(ObjectSelecter_OnFingerDragMove, 0);
			InputManager.Instance.AddOnTwoFingerDragBeginEventHandler(FingerGestures_OnTwoFingerDragBegin, 0);
			InputManager.Instance.AddOnTwoFingerDragMoveEventHandler(FingerGestures_OnTwoFingerDragMove, 0);
			InputManager.Instance.AddOnFingerDownEventHandler(ObjectSelecter_OnFingerDown, 0);
		}
	}

	private void DisableInput()
	{
		if (mInputEnabled)
		{
			mInputEnabled = false;
			if ((bool)InputManager.Instance)
			{
				InputManager.Instance.RemoveOnPinchBeginEventHandler(FingerGestures_OnPinchBegin);
				InputManager.Instance.RemoveOnPinchMoveEventHandler(FingerGestures_OnPinchMove);
				InputManager.Instance.RemoveOnFingerDragMoveEventHandler(ObjectSelecter_OnFingerDragMove);
				InputManager.Instance.RemoveOnTwoFingerDragBeginEventHandler(FingerGestures_OnTwoFingerDragBegin);
				InputManager.Instance.RemoveOnTwoFingerDragMoveEventHandler(FingerGestures_OnTwoFingerDragMove);
				InputManager.Instance.RemoveOnFingerDownEventHandler(ObjectSelecter_OnFingerDown);
			}
		}
	}

	private void DoPan(Vector2 delta)
	{
		if (allowPanning)
		{
			Vector3 position = new Vector3(0f, 0f, CameraManager.Instance.StrategyCamera.transform.position.y);
			Vector3 position2 = new Vector3(delta.x, delta.y, CameraManager.Instance.StrategyCamera.transform.position.y);
			Vector3 vector = CameraManager.Instance.StrategyCamera.ScreenToWorldPoint(position);
			Vector3 vector2 = CameraManager.Instance.StrategyCamera.ScreenToWorldPoint(position2);
			Vector2 vector3 = new Vector2(vector2.x - vector.x, vector2.z - vector.z);
			mPanMove = panningPlane.right * (0f - vector3.x) + panningPlane.up * (0f - vector3.y);
			IdealPanOffset += mPanMove;
		}
	}

	private void FingerGestures_OnPinchBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		centrePinchPos = (fingerPos1 + fingerPos2) * 0.5f;
		centrePinchPos = new Vector3(centrePinchPos.x, centrePinchPos.y, CameraManager.Instance.CurrentCamera.transform.position.y);
	}

	private void FingerGestures_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
		if (allowPinchZoom)
		{
			IdealDistance -= delta * pinchZoomSensitivity;
			mIsZooming = true;
		}
	}

	private void FingerGestures_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
	}

	private void FingerGestures_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
	}

	private void ObjectSelecter_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (InputManager.Instance.NumFingersActive <= 1 && !mIsZooming)
		{
			DoPan(delta);
		}
	}

	private bool ObjectSelecter_OnFingerDown(int fingerIndex, Vector2 startPos)
	{
		mPanMove = Vector2.zero;
		return true;
	}

	public void FocusOnTarget(Transform trans)
	{
		Actor component = trans.GetComponent<Actor>();
		if (WorldHelper.IsPlayerControlledActor(component))
		{
			if (trans != target)
			{
				panOffset -= trans.position - target.position;
			}
			target = trans;
			IdealPanOffset = Vector3.zero;
			GameplayController gameplayController = GameplayController.Instance();
			gameplayController.SelectOnlyThis(component);
		}
	}

	private void Apply()
	{
		if (!target)
		{
			CameraTarget cameraTarget = Object.FindObjectOfType(typeof(CameraTarget)) as CameraTarget;
			if (cameraTarget != null)
			{
				target = cameraTarget.gameObject.transform;
			}
			else
			{
				GameObject gameObject = new GameObject("dummyCamTarget");
				target = gameObject.transform;
			}
		}
		if (smoothMotion)
		{
			distance = Mathf.Lerp(distance, IdealDistance, TimeManager.DeltaTime * smoothZoomSpeed);
		}
		else
		{
			distance = IdealDistance;
		}
		base.transform.rotation = Quaternion.Euler(Pitch, 0f, Orientation);
		if (smoothPanning)
		{
			if (InputManager.Instance.NumFingersActive != 1)
			{
				mLastVelocity *= Mathf.Clamp(smoothPanningDecay, 0f, 0.95f);
				panOffset += mLastVelocity;
				idealPanOffset = panOffset;
			}
			else
			{
				mLastVelocity = idealPanOffset - panOffset;
				panOffset = idealPanOffset;
			}
		}
		else
		{
			panOffset = idealPanOffset;
		}
		if (mNavAgent != null)
		{
			mNavAgent.Move(mPanMove);
			mNavAgent.baseOffset = distance - initialDistance;
			mPanMove *= 0.9f;
		}
		else
		{
			base.transform.position = target.position + panOffset - distance * base.transform.forward;
		}
		mIsZooming = false;
	}

	private void LateUpdate()
	{
		Apply();
	}

	public void ResetPanning()
	{
		IdealPanOffset = Vector3.zero;
	}
}
