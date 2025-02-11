using System.Collections.Generic;
using UnityEngine;

public class PathObject_TS : MonoBehaviour
{
	public enum BehaviourType
	{
		Circling = 0,
		Strafing = 1,
		PathThrough = 2,
		FollowPath = 3
	}

	public enum PathType
	{
		CompleteAtEnd = 0,
		Loop = 1,
		PingPong = 2
	}

	public enum State
	{
		Idle = 0,
		Arriving = 1,
		Approaching = 2,
		Pausing = 3,
		Engaging = 4,
		ChangingDirection = 5,
		Exploding = 6
	}

	public TargetRangeEnemyData m_Interface;

	public GameObject moveMe;

	public float Radius = 10f;

	public float Speed = 1f;

	public BehaviourType Style;

	public float PauseTime = 3f;

	public float DirectionChangePause = 6f;

	public GameObject mCirclingTarget;

	public List<GameObject> PathPoints;

	public PathType pathType;

	public bool CompleteOnMessage;

	public float CompleteOnTimer;

	public List<GameObject> MsgOnFinished;

	public List<string> Message;

	public bool lookAlongPath;

	public bool MovingAtStart;

	public bool TargetSnap;

	public GameObject ActivateOnEnemyTargetShot;

	public GameObject ActivateOnFriendlyTargetShot;

	private bool useTimer;

	private bool isRunning;

	private bool isActive;

	public bool TargetRangeRotation;

	public GameObject controlledObject;

	public int WaitTime;

	private float timer;

	private bool isTiming = true;

	private float MyRotationX;

	private float MyRotationZ;

	private float MyRotationY;

	private float rotationAmmount = 90f;

	private float flipTime = 0.5f;

	private bool resetting;

	private float mCurrentDirection;

	private float mStrafedDistance;

	private float mWaitTimeRemaining;

	private float mCompleteTimer;

	private float mPauseTime;

	private float DesiredPitchRoll;

	private SnapTarget mSnapTarget;

	private Vector3 mCurrentPosition;

	private Vector3 mCurrentTravelDirection;

	private int mPathPointIndex;

	[HideInInspector]
	public State mState;

	private State cState = State.Arriving;

	private bool goingForwards = true;

	public Vector3 CurrentPosition
	{
		get
		{
			return mCurrentPosition;
		}
	}

	public Vector3 CurrentTravelDirection
	{
		get
		{
			return mCurrentTravelDirection;
		}
	}

	private void Start()
	{
		timer = WaitTime;
		MyRotationY = base.transform.rotation.y;
		rotationAmmount = MyRotationY + rotationAmmount;
		isRunning = false;
		mCurrentPosition = base.gameObject.transform.position;
		if (CompleteOnTimer > 0f)
		{
			useTimer = true;
			mCompleteTimer = CompleteOnTimer;
		}
		if (MovingAtStart)
		{
			ResetState(State.ChangingDirection);
		}
		else
		{
			ResetState(State.Idle);
		}
		SetupSnapTarget();
	}

	private void Complete()
	{
		if (MsgOnFinished != null)
		{
			int num = 0;
			foreach (GameObject item in MsgOnFinished)
			{
				Container.SendMessage(item, Message[num], base.gameObject);
				num++;
			}
		}
		mState = State.Idle;
	}

	public void TriggerStart()
	{
		if (mState == State.Idle)
		{
			mState = State.ChangingDirection;
			mCurrentTravelDirection = new Vector3(0f, 1f, 0f);
		}
	}

	public void SetupSnapTarget()
	{
		if (mSnapTarget == null && moveMe != null && controlledObject != null)
		{
			mSnapTarget = ActorGenerator.CreateStandardSnapTarget(base.transform);
			mSnapTarget.SnapPositionOverride = () => new Vector3(moveMe.transform.position.x, moveMe.transform.position.y + 1f, moveMe.transform.position.z);
		}
	}

	private void Update()
	{
		if (TargetRangeRotation)
		{
			MyRotationX = controlledObject.transform.rotation.x;
			MyRotationZ = controlledObject.transform.rotation.z;
			if (isTiming)
			{
				timer -= Time.deltaTime;
			}
			if (timer <= 0f)
			{
				doRotation();
			}
			if (!isActive)
			{
			}
			if (moveMe.gameObject == null && TargetSnap && mSnapTarget != null)
			{
				Object.Destroy(mSnapTarget.gameObject);
				mSnapTarget = null;
			}
		}
		if (moveMe != null)
		{
			moveMe.transform.position = mCurrentPosition;
		}
		switch (mState)
		{
		case State.Arriving:
			PrintDebug();
			UpdateArriving();
			break;
		case State.Approaching:
			PrintDebug();
			UpdateApproaching();
			break;
		case State.Pausing:
			PrintDebug();
			UpdatePausing();
			break;
		case State.Engaging:
			PrintDebug();
			UpdateEngaging();
			break;
		case State.ChangingDirection:
			PrintDebug();
			UpdateChangingDirection();
			break;
		case State.Exploding:
			UpdateExploding();
			break;
		}
		if (useTimer && isRunning)
		{
			mCompleteTimer -= Time.deltaTime;
			if (mCompleteTimer <= 0f)
			{
				isRunning = false;
				Complete();
			}
		}
	}

	private void startTimer()
	{
		timer = 0f;
		isTiming = true;
	}

	public void doRotation()
	{
		if (!resetting)
		{
			controlledObject.transform.rotation = Quaternion.Slerp(controlledObject.transform.rotation, Quaternion.Euler(MyRotationX, rotationAmmount, MyRotationZ), Time.deltaTime * 3.5f);
			flipTime -= Time.deltaTime;
		}
		if (resetting)
		{
			controlledObject.transform.rotation = Quaternion.Slerp(controlledObject.transform.rotation, Quaternion.Euler(MyRotationX, 0f, MyRotationZ), Time.deltaTime * 3.5f);
			flipTime -= Time.deltaTime;
		}
		if (flipTime <= 0f)
		{
			timer = WaitTime;
			flipTime = 0.5f;
			resetting = !resetting;
		}
	}

	private void Activate()
	{
		if (CompleteOnMessage)
		{
			Complete();
		}
	}

	private void Explode()
	{
		mState = State.Exploding;
	}

	private void PrintDebug()
	{
		if (cState != mState)
		{
			cState = mState;
		}
	}

	private void RunTimer()
	{
		isRunning = true;
	}

	public Vector3 GetCurrentFacing()
	{
		if (Style == BehaviourType.Circling && mState == State.Engaging)
		{
			return -base.transform.forward;
		}
		return base.transform.forward;
	}

	public bool ShouldTeleport()
	{
		if (mState != State.Engaging)
		{
			return true;
		}
		return false;
	}

	public bool IsExploding()
	{
		return mState == State.Exploding;
	}

	public void TriggerDestroy()
	{
		mState = State.Exploding;
	}

	private void ResetState(State newState)
	{
		isRunning = false;
		mCurrentDirection = 1f;
		mStrafedDistance = 0f;
		if (Style == BehaviourType.Circling && mState == State.Engaging)
		{
			mCurrentPosition += base.transform.forward * Radius;
		}
		mCompleteTimer = CompleteOnTimer;
		mPauseTime = PauseTime;
		mState = newState;
	}

	private void UpdateArriving()
	{
	}

	private void UpdateApproaching()
	{
		if (isRunning)
		{
			mState = State.Pausing;
		}
	}

	private void UpdatePausing()
	{
		if (isRunning)
		{
			mPauseTime -= Time.deltaTime;
		}
		if (mPauseTime <= 0f)
		{
			mState = State.Engaging;
		}
	}

	private void UpdateEngaging()
	{
		switch (Style)
		{
		case BehaviourType.Circling:
			base.transform.Rotate(0f, Time.deltaTime * Speed, 0f);
			mCurrentPosition = base.transform.position + base.transform.forward * Radius;
			break;
		case BehaviourType.Strafing:
			mCurrentTravelDirection = base.transform.right * mCurrentDirection;
			mStrafedDistance += Time.deltaTime * Speed * mCurrentDirection;
			mCurrentPosition = base.transform.position + base.transform.right * mStrafedDistance;
			if (Mathf.Abs(mStrafedDistance) >= Radius)
			{
				mWaitTimeRemaining = DirectionChangePause;
				mState = State.ChangingDirection;
			}
			break;
		case BehaviourType.PathThrough:
			if (isRunning)
			{
				Complete();
			}
			break;
		case BehaviourType.FollowPath:
		{
			Vector3 vector = PathPoints[mPathPointIndex].transform.position - mCurrentPosition;
			float magnitude = vector.magnitude;
			if (magnitude < 0.1f)
			{
				if (goingForwards)
				{
					mPathPointIndex++;
				}
				else if (!goingForwards)
				{
					mPathPointIndex--;
				}
				if (mPathPointIndex >= PathPoints.Count && pathType == PathType.CompleteAtEnd)
				{
					Complete();
				}
				if (mPathPointIndex >= PathPoints.Count && pathType == PathType.Loop)
				{
					mPathPointIndex = 0;
				}
				if ((mPathPointIndex >= PathPoints.Count && pathType == PathType.PingPong && goingForwards) || (mPathPointIndex <= 0 && pathType == PathType.PingPong && !goingForwards))
				{
					if (!goingForwards)
					{
						mPathPointIndex = 0;
					}
					if (goingForwards)
					{
						mPathPointIndex = PathPoints.Count - 2;
					}
					goingForwards = !goingForwards;
				}
				if (lookAlongPath && mPathPointIndex != PathPoints.Count)
				{
					mCirclingTarget = PathPoints[mPathPointIndex];
				}
			}
			if (magnitude > 0f)
			{
				mCurrentPosition += vector.normalized * Speed * Time.deltaTime;
			}
			break;
		}
		}
	}

	private void UpdateChangingDirection()
	{
		if (TargetSnap)
		{
			isActive = true;
		}
		if (mWaitTimeRemaining > 0f)
		{
			mWaitTimeRemaining -= Time.deltaTime;
			return;
		}
		mStrafedDistance = Radius * mCurrentDirection;
		mCurrentDirection = 0f - mCurrentDirection;
		mState = State.Engaging;
	}

	private void UpdateExploding()
	{
		float num = Speed * 1f;
		mCurrentPosition -= Vector3.up * num * Time.deltaTime;
	}

	public void DestroySnapTarget()
	{
		if (mSnapTarget != null)
		{
			Object.Destroy(mSnapTarget.gameObject);
			mSnapTarget = null;
		}
	}

	public void EnemyTargetDestroyed()
	{
		if (ActivateOnEnemyTargetShot != null)
		{
			Container.SendMessage(ActivateOnEnemyTargetShot, "Activate");
		}
		DestroySnapTarget();
	}

	public void FriendlyTargetDestroyed()
	{
		if (ActivateOnFriendlyTargetShot != null)
		{
			Container.SendMessage(ActivateOnFriendlyTargetShot, "Activate");
		}
		DestroySnapTarget();
	}
}
