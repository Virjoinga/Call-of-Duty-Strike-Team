using UnityEngine;

public class GrenadeThrowerComponent : BaseActorComponent
{
	private enum State
	{
		Inactive = 0,
		Primed = 1,
		Throwing = 2
	}

	public const float kMinDist2dRange = 0.3f;

	private GrenadeThrowMarker mMarker;

	private float mLastKnownMinimumThrowDistanceSq;

	private float mLastKnownDistanceRangeSq;

	private State mState;

	private Vector3 mTarget;

	private Grenade mThrowingArc;

	private int mUId;

	public float LastKnownMinimumThrowDistanceSq
	{
		get
		{
			return mLastKnownMinimumThrowDistanceSq;
		}
	}

	public float LastKnownDistanceRangeSq
	{
		get
		{
			return mLastKnownDistanceRangeSq;
		}
	}

	public Vector3 Target
	{
		get
		{
			return mTarget;
		}
		set
		{
			mTarget = value;
		}
	}

	private void Awake()
	{
		InputManager.Instance.AddOnFingerDragMoveEventHandler(OnFingerDragMove, 0);
		InputManager.Instance.AddOnFingerDownEventHandler(OnFingerDown, 0);
		mState = State.Inactive;
		mUId = 0;
	}

	private void OnDestroy()
	{
		if (InputManager.Instance != null)
		{
			InputManager.Instance.RemoveOnFingerDragMoveEventHandler(OnFingerDragMove);
			InputManager.Instance.RemoveOnFingerDownEventHandler(OnFingerDown);
		}
	}

	private void Start()
	{
		mThrowingArc = null;
	}

	private void Update()
	{
		State state = mState;
		if (state != State.Primed)
		{
			return;
		}
		if (myActor.realCharacter.IsDead() || myActor.realCharacter.IsMortallyWounded() || GameController.Instance.IsFirstPerson || !GameplayController.Instance().IsSelected(myActor))
		{
			Cancel();
			return;
		}
		if (mThrowingArc == null)
		{
			GameObject gameObject = InstantiateGrenade();
			gameObject.name = myActor.realCharacter.name + "_FAKE_" + mUId;
			mThrowingArc = gameObject.GetComponent<Grenade>();
			mLastKnownMinimumThrowDistanceSq = mThrowingArc.MinimumThrowDistanceSquared;
			mLastKnownDistanceRangeSq = mThrowingArc.DistanceRangeSquared;
			float sqrMagnitude = (Target - myActor.transform.position).sqrMagnitude;
			bool flag = sqrMagnitude > mLastKnownMinimumThrowDistanceSq && sqrMagnitude < mLastKnownDistanceRangeSq;
			if (flag)
			{
				float sqrMagnitude2 = (Target.xz() - myActor.transform.position.xz()).sqrMagnitude;
				if (sqrMagnitude2 <= mThrowingArc.MinimumThrowDistanceSquared * 0.3f)
				{
					flag = false;
				}
			}
			mThrowingArc.Launch(myActor, mTarget, true, flag);
		}
		if (CommonHudController.Instance.ActiveGrenadeThrowMarker != null && !CommonHudController.Instance.ActiveGrenadeThrowMarker.positionMoved)
		{
			UpdateMarkerWithCamera();
		}
	}

	private void OnDrawGizmos()
	{
		if (mState == State.Primed)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, mTarget);
		}
	}

	public bool Prime(Vector3 target)
	{
		TBFAssert.DoAssert(mState == State.Inactive, string.Format("Shouldn't be priming grenade from state {0}", mState.ToString()));
		if (myActor.behaviour.PlayerControlled)
		{
			PlayerSquadManager instance = PlayerSquadManager.Instance;
			if (instance.GrenadeCount <= 0)
			{
				return false;
			}
		}
		mTarget = target;
		mState = State.Primed;
		CommonHudController.Instance.AddGrenadeThrowMarker();
		RefocusCamera();
		UpdateThrowMarker();
		return true;
	}

	public bool IsPrimed()
	{
		return mState == State.Primed;
	}

	public void Cancel()
	{
		if (GameController.Instance.GrenadeThrowingModeActive)
		{
			GameController.Instance.EndGrenadeThrowingMode();
		}
		CommonHudController.Instance.RemoveGrenadeThrowMarker();
		mState = State.Inactive;
	}

	public void Throwing()
	{
		if (mThrowingArc != null)
		{
			Object.Destroy(mThrowingArc.gameObject);
		}
		mState = State.Throwing;
	}

	public void Throw()
	{
		if (mThrowingArc != null)
		{
			Object.Destroy(mThrowingArc.gameObject);
		}
		GameObject gameObject = InstantiateGrenade();
		gameObject.name = myActor.realCharacter.name + "_" + mUId;
		Grenade component = gameObject.GetComponent<Grenade>();
		component.Launch(myActor, mTarget, false, true);
		if (myActor != null && myActor.behaviour.PlayerControlled && GameController.Instance.GrenadeThrowingModeActive)
		{
			GameController.Instance.EndGrenadeThrowingMode();
			CommonHudController.Instance.RemoveGrenadeThrowMarker();
		}
		mState = State.Inactive;
	}

	public void UpdateThrowMarker()
	{
		if (mState != State.Primed)
		{
			return;
		}
		CommonHudController.Instance.ActiveGrenadeThrowMarker.UpdatePosition(Target);
		if (mThrowingArc != null)
		{
			float sqrMagnitude = (Target - myActor.transform.position).sqrMagnitude;
			float sqrMagnitude2 = (Target.xz() - myActor.transform.position.xz()).sqrMagnitude;
			if (sqrMagnitude > mThrowingArc.DistanceRangeSquared)
			{
				CommonHudController.Instance.ActiveGrenadeThrowMarker.SetIconState(GrenadeThrowMarker.IconState.CancelThrow);
			}
			else if (sqrMagnitude < mThrowingArc.MinimumThrowDistanceSquared || sqrMagnitude2 <= mThrowingArc.MinimumThrowDistanceSquared * 0.3f)
			{
				CommonHudController.Instance.ActiveGrenadeThrowMarker.SetIconState(GrenadeThrowMarker.IconState.CancelThrow);
			}
			else
			{
				CommonHudController.Instance.ActiveGrenadeThrowMarker.SetIconState(GrenadeThrowMarker.IconState.Normal);
			}
		}
	}

	public void UpdateMarkerWithCamera()
	{
		Vector3 position = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(position);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
		{
			Target = hitInfo.point;
		}
		UpdateThrowMarker();
	}

	private bool OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		if (GameController.Instance.GrenadeThrowingModeActive)
		{
			if (myActor.tasks.IsRunningTask<TaskThrowGrenade>())
			{
				return false;
			}
			OnFingerDragMove(fingerIndex, fingerPos, Vector2.zero);
			if (CommonHudController.Instance.ActiveGrenadeThrowMarker != null && !CommonHudController.Instance.ActiveGrenadeThrowMarker.positionMoved)
			{
				CommonHudController.Instance.ActiveGrenadeThrowMarker.positionMoved = true;
			}
		}
		return true;
	}

	private void OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (!GameController.Instance.GrenadeThrowingModeActive || !(CommonHudController.Instance.ActiveGrenadeThrowMarker != null) || myActor.tasks.IsRunningTask<TaskThrowGrenade>())
		{
			return;
		}
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(fingerPos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
		{
			mTarget = hitInfo.point;
			if (mThrowingArc != null)
			{
				Object.Destroy(mThrowingArc.gameObject);
			}
		}
		UpdateThrowMarker();
	}

	private GameObject InstantiateGrenade()
	{
		GameObject result = SceneNanny.Instantiate(ExplosionManager.Instance.Grenade) as GameObject;
		mUId++;
		return result;
	}

	private void RefocusCamera()
	{
		CameraManager instance = CameraManager.Instance;
		if (instance != null)
		{
			NavMeshCamera navMeshCamera = instance.PlayCameraController.CurrentCameraBase as NavMeshCamera;
			if (navMeshCamera != null)
			{
				navMeshCamera.FocusOnTarget(myActor.transform, true);
				navMeshCamera.ZoomOut();
			}
		}
	}
}
