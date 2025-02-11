using System.Collections.Generic;
using UnityEngine;

public class HelicopterRoutine : MonoBehaviour
{
	public enum BehaviourType
	{
		Circling = 0,
		Strafing = 1,
		Pathing = 2
	}

	private enum State
	{
		Unspawned = 0,
		Arriving = 1,
		Pausing = 2,
		Engaging = 3,
		ChangingDirection = 4,
		Exploding = 5,
		Fleeing = 6,
		FleeIdling = 7,
		PathToTarget = 8
	}

	public HelicopterRoutineData m_Interface;

	public float Radius = 10f;

	public float ArrivalSpeed = 1f;

	public float Speed = 1f;

	public float SpeedCrashMultiplier = 3f;

	public float SpeedFleeMultiplier = 3f;

	public float SpeedFleeReturnMultiplier = 3f;

	public BehaviourType Style;

	public bool ChasePlayer;

	public bool DisableTargeting;

	public ActorWrapper PriorityTarget;

	public float ArrivalOffset = 50f;

	public float PauseTime = 3f;

	public float DirectionChangePause = 6f;

	public float FleeAfterTime;

	public float FleeIdleTime = 5f;

	public GameObject ActivateOnDestruction;

	public List<GameObject> ActivateOnFlee;

	public List<Transform> PathPoints;

	public List<Transform> FleePoints;

	public Transform CrashPoint;

	private float mCurrentDirection;

	private float mStrafedDistance;

	private float mWaitTimeRemaining;

	private Vector3 mCurrentPosition;

	private Vector3 mCurrentTravelDirection;

	private int mPathPointIndex;

	private State mState;

	private State mPreviousState;

	private Actor mPriorityTargetActor;

	private ActorIdentIterator mPotentialTargetList;

	private Actor mCirclingTarget;

	private float mFleeIdleTime;

	private bool mIsAllowedToShoot;

	private float mShotSuppressionTimer;

	private bool mReturningToPathFromFlee;

	private GameObject mPathToTarget;

	public Helicopter Owner { private get; set; }

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
		mPotentialTargetList = new ActorIdentIterator(0u);
		ResetState();
	}

	private void Update()
	{
		if (PriorityTarget != null && mPriorityTargetActor == null)
		{
			mPriorityTargetActor = PriorityTarget.GetActor();
		}
		float num = float.MaxValue;
		mCirclingTarget = null;
		mPotentialTargetList.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (mPotentialTargetList.NextActor(out a))
		{
			if (a != null && a == mPriorityTargetActor)
			{
				mCirclingTarget = a;
				break;
			}
			if (!a.realCharacter.IsMortallyWounded() && !a.health.IsReviving)
			{
				float sqrMagnitude = (a.GetPosition() - mCurrentPosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					mCirclingTarget = a;
				}
			}
		}
		switch (mState)
		{
		case State.Arriving:
			if (!UpdateTimeFlee())
			{
				UpdateArriving();
			}
			break;
		case State.Pausing:
			if (!UpdateTimeFlee())
			{
				UpdatePausing();
			}
			break;
		case State.Engaging:
			if (!UpdateTimeFlee())
			{
				UpdateEngaging();
			}
			break;
		case State.ChangingDirection:
			if (!UpdateTimeFlee())
			{
				UpdateChangingDirection();
			}
			break;
		case State.Exploding:
			UpdateExploding();
			break;
		case State.Fleeing:
			UpdateFleeing();
			break;
		case State.FleeIdling:
			UpdateFleeIdling();
			break;
		case State.PathToTarget:
			UpdatePathToTarget();
			break;
		}
		if (mShotSuppressionTimer > 0f)
		{
			mShotSuppressionTimer -= Time.deltaTime;
		}
	}

	private bool UpdateTimeFlee()
	{
		if (m_Interface.UseTimedFlee)
		{
			FleeAfterTime -= Time.deltaTime;
			if (FleeAfterTime <= 0f)
			{
				Flee();
				FleeAfterTime = m_Interface.FleeAfterTime;
				return true;
			}
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, CurrentPosition);
	}

	public Actor GetCurrentTarget()
	{
		if (DisableTargeting)
		{
			return null;
		}
		if (mState == State.Fleeing)
		{
			return null;
		}
		return mCirclingTarget;
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

	public bool IsReturningFromFlee()
	{
		return mReturningToPathFromFlee;
	}

	public void Activate()
	{
		TriggerStart();
	}

	public void Deactivate()
	{
		TriggerDestroy(null);
	}

	public void Restart()
	{
		ReInitialise();
		TriggerStart();
	}

	public void ChangeDestructionNotification(GameObject obj)
	{
		m_Interface.ActivateOnDestruction = obj;
		ActivateOnDestruction = obj;
	}

	public void TriggerStart()
	{
		if (mState == State.Unspawned)
		{
			mState = State.Arriving;
			mCurrentTravelDirection = new Vector3(0f, 1f, 0f);
		}
	}

	public void TriggerDestroy(HealthComponent.HeathChangeEventArgs args)
	{
		mState = State.Exploding;
		if (args != null && args.From != null && EventHub.Instance != null)
		{
			EventHub.Instance.Report(new Events.XPHeliDestroyed());
			Actor component = args.From.GetComponent<Actor>();
			if (component != null)
			{
				Events.EventActor eventActor = new Events.EventActor();
				eventActor.CharacterType = CharacterType.Human;
				eventActor.Faction = FactionHelper.Category.Enemy;
				eventActor.Id = "Helicopter";
				eventActor.Name = "Helicopter";
				eventActor.WeaponClass = WeaponDescriptor.WeaponClass.Special;
				eventActor.WeaponId = string.Empty;
				EventHub.Instance.Report(new Events.Kill(component.EventActor(), eventActor, args.DamageType, args.HeadShot, args.OneShotKill, args.LongShotKill));
			}
		}
		if (ActivateOnDestruction != null)
		{
			Container.SendMessage(ActivateOnDestruction, "Activate");
		}
	}

	public bool IsSpawned()
	{
		return mState != State.Unspawned;
	}

	public bool CanShoot()
	{
		if (mShotSuppressionTimer > 0f)
		{
			return false;
		}
		return mIsAllowedToShoot && mState == State.Engaging;
	}

	public bool CanFireRockets()
	{
		return mIsAllowedToShoot && mState == State.Engaging;
	}

	public void TemporarilySuppressFiring()
	{
		mShotSuppressionTimer = 3f;
	}

	public void Flee()
	{
		if (ActivateOnFlee != null)
		{
			foreach (GameObject item in ActivateOnFlee)
			{
				Container.SendMessage(item, "Activate");
			}
		}
		if (Owner != null)
		{
			Owner.DestroySnapTarget();
		}
		mState = State.Fleeing;
	}

	public float GetSpeedMultiplier()
	{
		switch (mState)
		{
		case State.Engaging:
			if (mReturningToPathFromFlee)
			{
				return SpeedFleeReturnMultiplier;
			}
			break;
		case State.Exploding:
			return SpeedCrashMultiplier;
		case State.Fleeing:
			return SpeedFleeMultiplier;
		}
		return 1f;
	}

	public void EnableShooting()
	{
		mIsAllowedToShoot = true;
	}

	public void DisableShooting()
	{
		mIsAllowedToShoot = false;
	}

	public void CancelFlee()
	{
		if (mState != State.FleeIdling)
		{
			Debug.Log(string.Format("WARNING - Not intended to be called when not in the Flee-Idle state. Will do nothing. Current State = {0}", mState.ToString()));
			return;
		}
		mState = State.Engaging;
		if (Style != BehaviourType.Pathing)
		{
			return;
		}
		DisableShooting();
		mReturningToPathFromFlee = true;
		TBFAssert.DoAssert(PathPoints != null && PathPoints.Count > 0, "Shouldn't be pathing without a path");
		int num = 0;
		float num2 = float.MaxValue;
		for (int i = 0; i < PathPoints.Count; i++)
		{
			Transform transform = PathPoints[i];
			float sqrMagnitude = (transform.position - mCurrentPosition).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				num2 = sqrMagnitude;
				num = i;
			}
		}
		mPathPointIndex = num;
	}

	public void DoPathToTarget(GameObject target)
	{
		TBFAssert.DoAssert(target != null, "HelicopterRoutine:DoPathToTarget called with null target reference");
		mPreviousState = mState;
		mState = State.PathToTarget;
		mPathToTarget = target;
	}

	private void ResetState()
	{
		mCurrentDirection = 1f;
		mStrafedDistance = 0f;
		mPathPointIndex = 0;
		mCurrentPosition = base.transform.position - new Vector3(0f, ArrivalOffset, 0f);
		if (Style == BehaviourType.Circling && mState == State.Engaging)
		{
			mCurrentPosition += base.transform.forward * Radius;
		}
		if (ArrivalSpeed == 0f)
		{
			mState = State.Engaging;
		}
		else
		{
			mState = State.Unspawned;
		}
		mIsAllowedToShoot = true;
	}

	private void UpdateArriving()
	{
		if (ArrivalSpeed == 0f)
		{
			mState = State.Pausing;
			return;
		}
		mCurrentPosition = base.transform.position + mCurrentTravelDirection * Time.deltaTime * ArrivalSpeed;
		if (mCurrentPosition.y >= base.transform.position.y)
		{
			mState = State.Pausing;
		}
	}

	private void UpdatePausing()
	{
		PauseTime -= Time.deltaTime;
		if (PauseTime <= 0f)
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
		case BehaviourType.Pathing:
		{
			Vector3 vector = PathPoints[mPathPointIndex].position - mCurrentPosition;
			float magnitude = vector.magnitude;
			if (magnitude < 3f)
			{
				mPathPointIndex++;
				if (mPathPointIndex >= PathPoints.Count)
				{
					mPathPointIndex = 0;
				}
				if (mReturningToPathFromFlee)
				{
					if (Owner != null)
					{
						Owner.OnReturnFromFlee();
					}
					EnableShooting();
					mReturningToPathFromFlee = false;
				}
			}
			if (magnitude > 0f)
			{
				mCurrentPosition += vector.normalized * Speed * GetSpeedMultiplier() * Time.deltaTime;
			}
			break;
		}
		}
	}

	private void UpdatePathToTarget()
	{
		Vector3 vector = mPathToTarget.transform.position - mCurrentPosition;
		float magnitude = vector.magnitude;
		if (magnitude < 3f)
		{
			mState = mPreviousState;
		}
		if (magnitude > 0f)
		{
			mCurrentPosition += vector.normalized * Speed * GetSpeedMultiplier() * Time.deltaTime;
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
		float num = Speed * GetSpeedMultiplier();
		if (CrashPoint == null)
		{
			mCurrentPosition -= Vector3.up * num * Time.deltaTime;
			return;
		}
		Vector3 vector = CrashPoint.position - mCurrentPosition;
		if (vector.sqrMagnitude > 0f)
		{
			mCurrentPosition += vector.normalized * num * Time.deltaTime;
		}
	}

	private void UpdateFleeing()
	{
		if (FleePoints == null || FleePoints.Count == 0)
		{
			mState = State.Engaging;
			if (Owner != null)
			{
				Owner.SetupSnapTarget();
			}
			return;
		}
		Transform transform = null;
		float num = float.MaxValue;
		foreach (Transform fleePoint in FleePoints)
		{
			float sqrMagnitude = (fleePoint.position - mCurrentPosition).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				transform = fleePoint;
			}
		}
		TBFAssert.DoAssert(transform != null, "Something unexpectedly bad has happened");
		Vector3 vector = transform.position - mCurrentPosition;
		float magnitude = vector.magnitude;
		if (magnitude < 3f)
		{
			if (m_Interface.FinishOnFlee)
			{
				mState = State.Unspawned;
				return;
			}
			mState = State.FleeIdling;
			mFleeIdleTime = FleeIdleTime;
		}
		else
		{
			mCurrentPosition += vector.normalized * (Speed * GetSpeedMultiplier()) * Time.deltaTime;
		}
	}

	private void UpdateFleeIdling()
	{
		if (mFleeIdleTime > 0f)
		{
			mFleeIdleTime -= Time.deltaTime;
		}
		else
		{
			CancelFlee();
		}
	}

	private void ReInitialise()
	{
		ResetState();
		if (Owner != null)
		{
			Owner.ReInitialise();
		}
	}

	public void StopEngineLoops()
	{
		if (Owner != null)
		{
			Owner.StopEngineLoops();
		}
	}
}
