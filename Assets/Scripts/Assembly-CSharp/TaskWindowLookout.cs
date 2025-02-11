using UnityEngine;

public class TaskWindowLookout : Task
{
	private enum State
	{
		CentreAim = 0,
		TrackTarget = 1,
		HoldAim = 2,
		ReactToTarget = 3
	}

	private static float mSpeedToLockOntoTarget = 0.05f;

	private static float mZoneInOnTargetTime = 2.5f;

	private State mState;

	private float mCurrentTrackTime;

	private LineRenderer mLineRenderer;

	private Actor mCurrentTarget;

	private Vector3 mTargetMoveDirection = Vector3.zero;

	private Vector3 mPreviousTargetPosition = Vector3.zero;

	private Vector3 mLookDirection;

	private float mLookDistance;

	private float mMaxLookDistance;

	private Vector3 mLookDirectionBeforeTurnBegan;

	private Vector3 mStartLookDirection;

	public TaskWindowLookout(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mLookDirection = mActor.transform.forward.normalized;
		mLookDirectionBeforeTurnBegan = (mStartLookDirection = mLookDirection);
		mStartLookDirection.y = -0.5f;
		FireAtWillComponent component = base.Owner.GetComponent<FireAtWillComponent>();
		if (component != null)
		{
			component.enabled = false;
		}
		mLookDistance = (mMaxLookDistance = mActor.awareness.VisionRange);
	}

	public override void Update()
	{
		if (mActor == null)
		{
			return;
		}
		if (mCurrentTarget != null)
		{
			if (mPreviousTargetPosition != Vector3.zero && mPreviousTargetPosition != mCurrentTarget.transform.position)
			{
				mTargetMoveDirection = mPreviousTargetPosition - mCurrentTarget.transform.position;
			}
			mPreviousTargetPosition = mCurrentTarget.transform.position;
			mTargetMoveDirection.Normalize();
			if (ShouldMoveIntoCover())
			{
				OrderMoveToCover();
			}
		}
		switch (mState)
		{
		case State.CentreAim:
			CentreAim();
			break;
		case State.TrackTarget:
			if (LOSVisible())
			{
				Vector3 vector = mCurrentTarget.transform.position - mActor.awareness.GetPosition();
				float magnitude = vector.magnitude;
				vector.Normalize();
				float num = Mathf.Acos(Vector3.Dot(mLookDirectionBeforeTurnBegan, vector));
				num *= 57.29578f;
				float num2 = num + magnitude;
				if (ZonedInOnDirection(vector, num2 * mSpeedToLockOntoTarget))
				{
					SetState(State.ReactToTarget);
				}
				if (TooCloseToBeConsideredATarget(mCurrentTarget))
				{
					SetState(State.CentreAim);
				}
			}
			else
			{
				SetState(State.CentreAim);
			}
			break;
		case State.HoldAim:
			HoldAim();
			break;
		case State.ReactToTarget:
			ReactToTarget();
			break;
		}
	}

	public override bool HasFinished()
	{
		return false;
	}

	private void SetState(State newState)
	{
		switch (newState)
		{
		case State.CentreAim:
			if (mCurrentTarget != null)
			{
				mCurrentTarget.awareness.Forget(mActor);
				mCurrentTarget = null;
				CheckForEnemies();
				if (mCurrentTarget == null)
				{
					TimeManager.instance.ResumeNormalTime();
				}
			}
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mCurrentTrackTime = 0f;
			break;
		case State.HoldAim:
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mCurrentTrackTime = 0f;
			break;
		case State.TrackTarget:
			if (mCurrentTarget != null)
			{
				GameController instance = GameController.Instance;
				if (!instance.IsFirstPerson)
				{
					if (TimeManager.instance.CurrentTargetTimeScale != TimeManager.instance.StopTimeModeSpeed)
					{
						CameraController playCameraController = CameraManager.Instance.PlayCameraController;
						if (playCameraController != null)
						{
							PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
							if (playCameraInterface != null)
							{
								playCameraInterface.FocusOnTarget(mCurrentTarget.transform, true);
								instance.StartCountdownToAdreneline(playCameraInterface, mCurrentTarget, 3f, mActor);
							}
						}
					}
					mCurrentTarget.awareness.BecomeAware(mActor);
					TimeManager.instance.SlowDownTime(0f, TimeManager.instance.StopTimeModeSpeed);
				}
			}
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mCurrentTrackTime = 0f;
			break;
		}
		mState = newState;
	}

	private void ForceFollow()
	{
		if (LOSVisible())
		{
			mCurrentTrackTime = 1f;
			ZonedInOnDirection(mCurrentTarget.transform.position - mActor.awareness.GetPosition(), 1.1f);
		}
		else
		{
			SetState(State.CentreAim);
		}
	}

	private bool ZonedInOnDirection(Vector3 direction, float time)
	{
		mCurrentTrackTime += Time.deltaTime;
		float num = mCurrentTrackTime / time;
		direction.Normalize();
		mLookDirection = mLookDirectionBeforeTurnBegan + (direction - mLookDirectionBeforeTurnBegan) * num;
		SetLookAndDistance();
		return num > 1f || mLookDirection == direction;
	}

	private void SetLookAndDistance()
	{
		mActor.transform.forward = mStartLookDirection;
		mActor.awareness.LookDirection = mStartLookDirection;
		Vector3 position = mActor.awareness.GetPosition();
		Vector3 normalized = mLookDirection.normalized;
		Vector3 end = position + normalized * mMaxLookDistance;
		position += normalized * 2f;
		RaycastHit hitInfo;
		if (Physics.Linecast(position, end, out hitInfo))
		{
			mLookDistance = hitInfo.distance + 2f;
		}
		mActor.awareness.VisionRange = mLookDistance;
	}

	private bool TooCloseToBeConsideredATarget(Actor target)
	{
		if (mActor.realCharacter != null && target != null)
		{
			float num = 7f;
			Vector2 vector = new Vector2(target.transform.position.x - mActor.transform.position.x, target.transform.position.z - mActor.transform.position.z);
			if (vector.magnitude < num)
			{
				return true;
			}
		}
		return false;
	}

	private bool LOSVisible()
	{
		return LOSVisible(mCurrentTarget);
	}

	private bool LOSVisible(Actor target)
	{
		if (target == null)
		{
			return false;
		}
		if (target.realCharacter.IsDead())
		{
			return false;
		}
		if (!mActor.awareness.CanSee(target))
		{
			return false;
		}
		if (target.awareness.IsInCover() && !target.weapon.IsFiring() && target.realCharacter.IsCrouching())
		{
			return false;
		}
		if (TooCloseToBeConsideredATarget(target))
		{
			return false;
		}
		return true;
	}

	private void CentreAim()
	{
		if (ZonedInOnDirection(mStartLookDirection, mZoneInOnTargetTime))
		{
			SetState(State.HoldAim);
		}
		CheckForEnemies();
	}

	private void HoldAim()
	{
		CheckForEnemies();
	}

	private void ReactToTarget()
	{
		if (LOSVisible())
		{
			if (mActor.weapon != null)
			{
				mActor.weapon.SetTarget(mCurrentTarget);
			}
			ForceFollow();
		}
		else
		{
			SetState(State.CentreAim);
		}
	}

	private void CheckForEnemies()
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.EnemiesMask(mActor) & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (LOSVisible(a))
			{
				mCurrentTarget = a;
				SetState(State.TrackTarget);
				return;
			}
		}
		mActor.awareness.ForgetAllEnemies();
	}

	private void OrderMoveToCover()
	{
		InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Run);
		inheritableMovementParams.mDestination = mActor.GetPosition();
		new TaskMoveToCover(mOwner, TaskManager.Priority.LONG_TERM, Config.ClearAllCurrentType, inheritableMovementParams);
	}

	private bool ShouldMoveIntoCover()
	{
		if (mCurrentTarget.realCharacter.IsShootingAt(mActor) && !mActor.behaviour.IsWinning(mCurrentTarget))
		{
			return true;
		}
		return false;
	}
}
