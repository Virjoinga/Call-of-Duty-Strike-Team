using UnityEngine;

public class BodyDroppingComponent : MonoBehaviour
{
	private enum State
	{
		Start = 0,
		Position = 1,
		Inactive = 2,
		Destroy = 3
	}

	private Vector3 mStartPosition;

	private State mState;

	private Vector3 mPosition;

	private void Awake()
	{
		InputManager.Instance.AddOnFingerDragMoveEventHandler(OnFingerDragMove, 0);
		mState = State.Inactive;
		mStartPosition = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
	}

	private void OnDestroy()
	{
		if (InputManager.Instance != null)
		{
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
			break;
		case State.Position:
			break;
		case State.Destroy:
			TidyUp();
			Object.DestroyImmediate(this);
			break;
		case State.Inactive:
			break;
		}
	}

	public void TidyUp()
	{
		if (GameController.Instance.PlacementModeActive)
		{
			GameController.Instance.EndPlacementMode();
		}
		mState = State.Destroy;
	}

	public void Cancel()
	{
		mState = State.Destroy;
	}

	public void BeginDroppingBody()
	{
		CommonHudController.Instance.AddGrenadeThrowMarker();
		if (CommonHudController.Instance.ActiveGrenadeThrowMarker != null)
		{
			Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(mStartPosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
			{
				CommonHudController.Instance.ActiveGrenadeThrowMarker.UpdatePosition(hitInfo.point);
				CommonHudController.Instance.ActiveGrenadeThrowMarker.SetIconState(GrenadeThrowMarker.IconState.DropBody);
			}
		}
		mState = State.Start;
	}

	public void DropBody(Vector3 position)
	{
		CommonHudController.Instance.RemoveGrenadeThrowMarker();
		if (CanAnyoneGetHere(position))
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderDrop(gameplayController, position);
		}
		else
		{
			Cancel();
		}
		mState = State.Position;
	}

	private void OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (mState == State.Start && CommonHudController.Instance.ActiveGrenadeThrowMarker != null)
		{
			Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(fingerPos);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
			{
				bool flag = CanAnyoneGetHere(hitInfo.point);
				CommonHudController.Instance.ActiveGrenadeThrowMarker.SetIconState((!flag) ? GrenadeThrowMarker.IconState.CancelBody : GrenadeThrowMarker.IconState.DropBody);
				CommonHudController.Instance.ActiveGrenadeThrowMarker.UpdatePosition(hitInfo.point);
			}
		}
	}

	private bool CanAnyoneGetHere(Vector3 point)
	{
		bool result = false;
		GameplayController gameplayController = GameplayController.Instance();
		foreach (Actor item in gameplayController.Selected)
		{
			UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
			if (UnityEngine.AI.NavMesh.CalculatePath(item.navAgent.transform.position, point, item.navAgent.walkableMask, navMeshPath) && navMeshPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
