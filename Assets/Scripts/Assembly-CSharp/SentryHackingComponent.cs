using UnityEngine;

public class SentryHackingComponent : BaseActorComponent
{
	private enum PlacementState
	{
		Start = 0,
		Inactive = 1,
		Setting = 2,
		Set = 3,
		Destroy = 4
	}

	private GameObject claymorePlacementObj;

	private TrackingRobotRealCharacter mSentryGunBeingHacked;

	private Actor mHackingActor;

	private PlacementState mPlacementState;

	private Vector3 mPosition;

	private Vector3 mFacing;

	private bool mPlacementComponentActive;

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

	public Actor HackingActor
	{
		get
		{
			return mHackingActor;
		}
	}

	private void Awake()
	{
		InputManager.Instance.AddOnFingerDownEventHandler(OnFingerDown, 0);
		InputManager.Instance.AddOnFingerUpEventHandler(OnFingerUp, 0);
		InputManager.Instance.AddOnFingerDragMoveEventHandler(OnFingerDragMove, 0);
		mPlacementState = PlacementState.Inactive;
		mPlacementComponentActive = false;
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
		if (mPlacementComponentActive)
		{
			if (mSentryGunBeingHacked.myActor.realCharacter != null && mSentryGunBeingHacked != null && (mSentryGunBeingHacked.myActor.realCharacter.IsDead() || mSentryGunBeingHacked.IsDead()))
			{
				Cancel();
			}
			switch (mPlacementState)
			{
			case PlacementState.Start:
				break;
			case PlacementState.Setting:
				break;
			case PlacementState.Set:
				mPlacementState = PlacementState.Inactive;
				break;
			case PlacementState.Inactive:
				mPlacementComponentActive = false;
				break;
			case PlacementState.Destroy:
				break;
			}
		}
	}

	public void TidyUp()
	{
		if (GameController.Instance.SentryHackingModeActive)
		{
			GameController.Instance.EndSentryHackingMode();
		}
		mPlacementState = PlacementState.Destroy;
		mPlacementComponentActive = false;
		if (claymorePlacementObj != null)
		{
			Object.DestroyImmediate(claymorePlacementObj);
		}
	}

	public void Cancel()
	{
		mPlacementState = PlacementState.Inactive;
		CommonHudController.Instance.RemovePlaceFingerHereMarker();
	}

	public void HackSentry(Actor sentryGun)
	{
	}

	public void PrepareHacking(Actor sentryGun)
	{
		if (mSentryGunBeingHacked != sentryGun.realCharacter)
		{
			mSentryGunBeingHacked = sentryGun.realCharacter as TrackingRobotRealCharacter;
		}
		mPosition = mSentryGunBeingHacked.myActor.GetPosition();
		mFacing = mSentryGunBeingHacked.transform.forward;
		mFacing.y = 0f;
		mPlacementComponentActive = true;
		claymorePlacementObj = (GameObject)Object.Instantiate(Resources.Load("ClaymorePlacement"));
		if (claymorePlacementObj != null)
		{
			claymorePlacementObj.transform.position = mPosition;
			claymorePlacementObj.transform.forward = mFacing;
		}
		mPlacementState = PlacementState.Set;
		if (GameController.Instance.SentryHackingModeActive)
		{
			GameController.Instance.EndSentryHackingMode();
		}
		TidyUp();
	}

	private bool OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		return true;
	}

	private void OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
	}

	private bool OnFingerUp(int fingerIndex, Vector2 fingerPos, float duration)
	{
		return true;
	}

	private void SetNewFacing(Vector2 fingerPos)
	{
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(fingerPos);
		Vector3 vector = Maths.ClosestPointOnLineSegment(claymorePlacementObj.transform.position, ray.origin, ray.origin + ray.direction * 100f);
		Vector3 forward = mFacing;
		forward.x = vector.x - mPosition.x;
		forward.z = vector.z - mPosition.z;
		forward.Normalize();
		if (claymorePlacementObj != null)
		{
			claymorePlacementObj.transform.forward = forward;
		}
	}

	public void StartHackingTimer()
	{
		TBFAssert.DoAssert(!mPlacementComponentActive, string.Format("Shouldn't be starting hack if placement is still active"));
	}

	public void StopHackingTimer()
	{
		TBFAssert.DoAssert(!mPlacementComponentActive, string.Format("Shouldn't be ending hack if placement is still active"));
	}

	public void SetHackingActor(Actor hacker)
	{
		mHackingActor = hacker;
	}

	public void ClearHackingActor()
	{
		mHackingActor = null;
	}
}
