using System.Collections.Generic;
using UnityEngine;

public class VtolTestRoutineKM : MonoBehaviour
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

	public float Radius = 10f;

	public float ArrivalOffset = 50f;

	public float ArrivalSpeed = 1f;

	public float MoveToRoutineSpeed = 8f;

	public bool MaintainSpeed;

	public float Speed = 1f;

	public float NewRotationSpeed = 2f;

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

	public GameObject VtolToMessage;

	public bool PlayInteriorSound = true;

	private VtolTestKM VtoltoMessage;

	public bool isSniperVtol;

	public bool lookAlongPath = true;

	private float fPitchLimit = 4f;

	private float fRollLimit = 5f;

	private float fJohnsFactor = 200f;

	private float rotSpeed = 0.1f;

	private bool useTimer;

	private bool isRunning;

	private float mCurrentDirection;

	private float mStrafedDistance;

	private float mWaitTimeRemaining;

	private float mCompleteTimer;

	private float mPauseTime;

	private float DesiredSpeed;

	private float DesiredPitchRoll;

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
		isRunning = false;
		if (CompleteOnTimer > 0f)
		{
			useTimer = true;
			mCompleteTimer = CompleteOnTimer;
		}
		ResetState();
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
		VtolToMessage.SendMessage("Activate");
		ResetState();
	}

	public void TriggerStart(float startSpeed, float startPitchRoll)
	{
		if (PlayInteriorSound)
		{
			VtolTestKM.TurnOnSound();
		}
		if (!PlayInteriorSound)
		{
			VtolTestKM.TurnOffSound();
		}
		DesiredSpeed = startSpeed;
		fPitchLimit = startPitchRoll;
		fRollLimit = startPitchRoll;
		if (mState == State.Idle)
		{
			mState = State.Arriving;
			mCurrentTravelDirection = new Vector3(0f, 1f, 0f);
		}
	}

	private void Update()
	{
		fPitchLimit = Mathf.Lerp(fPitchLimit, DesiredPitchRoll, Time.deltaTime * rotSpeed);
		fRollLimit = Mathf.Lerp(fRollLimit, DesiredPitchRoll, Time.deltaTime * rotSpeed);
		switch (mState)
		{
		case State.Arriving:
			DesiredSpeed = MoveToRoutineSpeed;
			DesiredPitchRoll = 20f;
			fJohnsFactor = 200f;
			PrintDebug();
			UpdateArriving();
			break;
		case State.Approaching:
			DesiredSpeed = MoveToRoutineSpeed;
			DesiredPitchRoll = 20f;
			fJohnsFactor = 200f;
			PrintDebug();
			UpdateApproaching();
			break;
		case State.Pausing:
			DesiredSpeed = 2f;
			DesiredPitchRoll = 2f;
			fJohnsFactor = 200f;
			if (MaintainSpeed)
			{
				DesiredSpeed = MoveToRoutineSpeed;
			}
			PrintDebug();
			UpdatePausing();
			break;
		case State.Engaging:
			if (isSniperVtol)
			{
				DesiredSpeed = 0.5f;
				DesiredPitchRoll = 0.5f;
			}
			else
			{
				DesiredSpeed = 1f;
				DesiredPitchRoll = 1f;
			}
			if (MaintainSpeed)
			{
				DesiredSpeed = MoveToRoutineSpeed;
			}
			fJohnsFactor = 200f;
			PrintDebug();
			UpdateEngaging();
			break;
		case State.ChangingDirection:
			if (isSniperVtol)
			{
				DesiredSpeed = 0.5f;
				DesiredPitchRoll = 0.5f;
			}
			else
			{
				DesiredSpeed = 2f;
				DesiredPitchRoll = 1f;
			}
			if (MaintainSpeed)
			{
				DesiredSpeed = MoveToRoutineSpeed;
			}
			fJohnsFactor = 200f;
			PrintDebug();
			UpdateChangingDirection();
			break;
		case State.Exploding:
			DesiredSpeed = 10f;
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
			Debug.Log("VTOL IS: " + mState);
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

	public GameObject GetCurrentTarget()
	{
		return mCirclingTarget;
	}

	public float GetNewRotationSpeed()
	{
		return NewRotationSpeed;
	}

	public float GetDesiredSpeed()
	{
		return DesiredSpeed;
	}

	public float GetPitch()
	{
		return fPitchLimit;
	}

	public float GetRoll()
	{
		return fRollLimit;
	}

	public float GetJohnsFactor()
	{
		return fJohnsFactor;
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

	public bool IsIdle()
	{
		return mState == State.Idle;
	}

	public bool CanShoot()
	{
		return mState == State.Engaging;
	}

	private void ResetState()
	{
		isRunning = false;
		mCurrentDirection = 1f;
		mStrafedDistance = 0f;
		mCurrentPosition = base.transform.position - new Vector3(0f, ArrivalOffset, 0f);
		if (Style == BehaviourType.Circling && mState == State.Engaging)
		{
			mCurrentPosition += base.transform.forward * Radius;
		}
		mCompleteTimer = CompleteOnTimer;
		mPauseTime = PauseTime;
		mState = State.Idle;
	}

	private void UpdateArriving()
	{
		mCurrentPosition += mCurrentTravelDirection * Time.deltaTime * ArrivalSpeed;
		if (mCurrentPosition.y >= base.transform.position.y)
		{
			mState = State.Approaching;
		}
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
			if (magnitude < 3f)
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
}
