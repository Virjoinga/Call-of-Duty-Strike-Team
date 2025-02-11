using UnityEngine;

public class ClaymoreLayingComponent : MonoBehaviour
{
	private enum State
	{
		Start = 0,
		Position = 1,
		Setting = 2,
		Set = 3,
		Inactive = 4,
		Destroy = 5
	}

	private const float kRingElevation = 0.5f;

	private Vector3 mStartPosition;

	private GameObject claymorePlacementObj;

	private State mState;

	private Vector3 mPosition;

	private Vector3 mFacing;

	private bool mDropped;

	private bool mSet;

	public Vector3 Position
	{
		get
		{
			return mPosition;
		}
		set
		{
			mPosition = value;
		}
	}

	public Vector3 Facing
	{
		get
		{
			return mFacing;
		}
		set
		{
			mFacing = value;
		}
	}

	public bool ClaymoreDropped
	{
		get
		{
			return mDropped;
		}
		set
		{
			mDropped = value;
		}
	}

	public bool ClaymoreSet
	{
		get
		{
			return mSet;
		}
	}

	private void Awake()
	{
		InputManager.Instance.AddOnFingerDownEventHandler(OnFingerDown, 0);
		InputManager.Instance.AddOnFingerUpEventHandler(OnFingerUp, 0);
		InputManager.Instance.AddOnFingerDragMoveEventHandler(OnFingerDragMove, 0);
		mState = State.Inactive;
		mStartPosition = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		mDropped = false;
		mSet = false;
	}

	private void OnDestroy()
	{
		if (InputManager.Instance != null)
		{
			InputManager.Instance.RemoveOnFingerDownEventHandler(OnFingerDown);
			InputManager.Instance.RemoveOnFingerUpEventHandler(OnFingerUp);
			InputManager.Instance.RemoveOnFingerDragMoveEventHandler(OnFingerDragMove);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		switch (mState)
		{
		case State.Start:
			mDropped = false;
			mSet = false;
			if (CommonHudController.Instance.ActiveGrenadeThrowMarker != null && !CommonHudController.Instance.ActiveGrenadeThrowMarker.positionMoved)
			{
				UpdateMarkerWithCamera();
			}
			break;
		case State.Position:
			mDropped = true;
			mSet = false;
			break;
		case State.Setting:
			break;
		case State.Set:
			mSet = true;
			mState = State.Inactive;
			break;
		case State.Inactive:
			FreezeCamera(false);
			break;
		case State.Destroy:
			FreezeCamera(false);
			Object.DestroyImmediate(this);
			break;
		}
	}

	public void TidyUp()
	{
		if (GameController.Instance.ClaymoreDroppingModeActive)
		{
			GameController.Instance.EndClaymoreDroppingMode();
		}
		mState = State.Destroy;
		CommonHudController.Instance.RemovePlaceFingerHereMarker();
		if (claymorePlacementObj != null)
		{
			Object.DestroyImmediate(claymorePlacementObj);
		}
	}

	public void Cancel()
	{
		mState = State.Destroy;
	}

	public void BeginPlacingClaymore()
	{
		CommonHudController.Instance.AddGrenadeThrowMarker();
		if (CommonHudController.Instance.ActiveGrenadeThrowMarker != null)
		{
			Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(mStartPosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
			{
				CommonHudController.Instance.ActiveGrenadeThrowMarker.UpdatePosition(hitInfo.point);
				CommonHudController.Instance.ActiveGrenadeThrowMarker.SetIconState(GrenadeThrowMarker.IconState.PlaceClaymore);
			}
		}
		mState = State.Start;
		FreezeCamera(true);
	}

	public void PrepareClaymore(Vector3 position)
	{
		CommonHudController.Instance.RemoveGrenadeThrowMarker();
		if (CanAnyoneGetHere(position))
		{
			mPosition = position;
			position.y += 0.5f;
			mFacing = new Vector3(0f, 0f, -1f);
			claymorePlacementObj = (GameObject)Object.Instantiate(Resources.Load("ClaymorePlacement"));
			if (claymorePlacementObj != null)
			{
				claymorePlacementObj.transform.position = position;
				claymorePlacementObj.transform.forward = mFacing;
			}
			mState = State.Position;
			Vector3 vector = position + -mFacing * 3f;
			float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
			float num2 = (float)Screen.height * num;
			float num3 = num2 - num2 * 0.25f;
			if (CameraManager.Instance.CurrentCamera.WorldToScreenPoint(vector).y < num3)
			{
				vector = position + mFacing * 3f;
			}
			CommonHudController.Instance.AddPlaceFingerHereMarker(vector, true);
		}
		else
		{
			Cancel();
		}
	}

	private void FreezeCamera(bool freeze)
	{
		CameraManager instance = CameraManager.Instance;
		if (instance != null)
		{
			NavMeshCamera navMeshCamera = instance.PlayCameraController.CurrentCameraBase as NavMeshCamera;
			if (navMeshCamera != null)
			{
				navMeshCamera.allowPanning = !freeze;
			}
		}
	}

	private bool OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		if (mState == State.Position)
		{
			if (!GameController.Instance.ClaymoreDroppingModeActive)
			{
				GameController.Instance.StartClaymoreDroppingMode();
			}
			mState = State.Setting;
			if (CommonHudController.Instance.ActiveGrenadeThrowMarker != null && !CommonHudController.Instance.ActiveGrenadeThrowMarker.positionMoved)
			{
				CommonHudController.Instance.ActiveGrenadeThrowMarker.positionMoved = true;
			}
			Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(fingerPos);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1) && (mPosition - hitInfo.point).sqrMagnitude > 15f && SelectableObject.PickSelectableObject(fingerPos) != null)
			{
				TidyUp();
			}
		}
		return false;
	}

	private void OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (mState == State.Start)
		{
			if (CommonHudController.Instance.ActiveGrenadeThrowMarker != null)
			{
				Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(fingerPos);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
				{
					bool flag = CanAnyoneGetHere(hitInfo.point);
					CommonHudController.Instance.ActiveGrenadeThrowMarker.SetIconState((!flag) ? GrenadeThrowMarker.IconState.CancelClaymore : GrenadeThrowMarker.IconState.PlaceClaymore);
					CommonHudController.Instance.ActiveGrenadeThrowMarker.UpdatePosition(hitInfo.point);
				}
			}
		}
		else if (mState == State.Setting)
		{
			Ray ray2 = CameraManager.Instance.CurrentCamera.ScreenPointToRay(fingerPos);
			Vector3 vector = Maths.ClosestPointOnLineSegment(claymorePlacementObj.transform.position, ray2.origin, ray2.origin + ray2.direction * 100f);
			Vector3 forward = mFacing;
			forward.x = vector.x - mPosition.x;
			forward.z = vector.z - mPosition.z;
			forward.Normalize();
			mFacing = forward;
			if (claymorePlacementObj != null)
			{
				claymorePlacementObj.transform.forward = forward;
			}
		}
		if (CommonHudController.Instance.ActiveGrenadeThrowMarker != null && !CommonHudController.Instance.ActiveGrenadeThrowMarker.positionMoved)
		{
			CommonHudController.Instance.ActiveGrenadeThrowMarker.positionMoved = true;
		}
	}

	public void UpdateMarkerWithCamera()
	{
		Vector3 position = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(position);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
		{
			CommonHudController.Instance.ActiveGrenadeThrowMarker.UpdatePosition(hitInfo.point);
		}
	}

	private bool CanAnyoneGetHere(Vector3 point)
	{
		bool result = false;
		GameplayController gameplayController = GameplayController.Instance();
		foreach (Actor item in gameplayController.Selected)
		{
			NavMeshPath navMeshPath = new NavMeshPath();
			if (NavMesh.CalculatePath(item.navAgent.transform.position, point, item.navAgent.walkableMask, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool OnFingerUp(int fingerIndex, Vector2 fingerPos, float duration)
	{
		if (mState == State.Setting)
		{
			GameplayController gameplayController = GameplayController.Instance();
			TidyUp();
			mSet = true;
			OrdersHelper.OrderDropClaymore(gameplayController, mPosition, mFacing);
			return false;
		}
		return true;
	}
}
