using UnityEngine;

public class TaskFollowMovingTarget : TaskRouteTo
{
	private enum State
	{
		Start = 0,
		WaitPath = 1,
		AnalysePath = 2,
		Follow = 3,
		Done = 4
	}

	public const float kMinWalkSpeedRange = 0.95f;

	public const float kMaxWalkSpeedRange = 1.05f;

	public const float kMinRunSpeedRange = 0.95f;

	public const float kMaxRunSpeedRange = 1.05f;

	private bool hasStopped;

	private State state;

	private Actor mMovingTarget;

	private bool mExitUponReachingTarget;

	private bool mInRangeOfTarget;

	private Vector3 mLastFramesTargetPosition = Vector3.zero;

	public TaskFollowMovingTarget(TaskManager owner, TaskManager.Priority priority, Config flags, InheritableMovementParams moveParams, Actor movingTarget, bool exitUponReachingTarget)
		: base(owner, priority, flags, moveParams)
	{
		mMovingTarget = movingTarget;
		mMoveParams.mDestination = mMovingTarget.GetPosition();
		mExitUponReachingTarget = exitUponReachingTarget;
		mActor.Command("MoveToBegin");
		if (owner.IsRunningTask(typeof(TaskStealthKill)))
		{
			mActor.Command("Stealth");
		}
	}

	public override void Update()
	{
		mMoveParams.mDestination = mMovingTarget.GetPosition();
		CheckIfReachedDestination();
		switch (state)
		{
		case State.Start:
			state = State.Follow;
			break;
		case State.Follow:
			ConstructRoute();
			Follow();
			WorldHelper.AvoidObstacles(mActor, mMovingTarget);
			mLastFramesTargetPosition = mMoveParams.mDestination;
			break;
		}
		Vector3 vector = mMoveParams.mDestination - mActor.GetPosition();
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude < mMoveParams.mLookTowardsDistSqr)
		{
			mActor.realCharacter.ImposeLookDirection(vector.normalized);
		}
		else if (mMoveParams.forceFaceForwards)
		{
			mActor.realCharacter.ImposeLookDirection(mActor.navAgent.velocity);
		}
	}

	public override bool HasFinished()
	{
		if (CheckConfigFlagsFinished())
		{
			return true;
		}
		bool flag = mFailedToExitContainer || state == State.Done;
		float distanceSquared;
		if (mActor.realCharacter != null && (base.ConfigFlags & Config.AbortOnVisibleEnemy) != 0 && mActor.awareness.GetNearestVisibleEnemy(out distanceSquared) != null)
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "Enemy");
			flag = true;
		}
		if (flag)
		{
			RemoveMarker();
		}
		return flag;
	}

	private void CheckIfReachedDestination()
	{
		mInRangeOfTarget = false;
		if (!mExitUponReachingTarget)
		{
			return;
		}
		float sqrMagnitude = (mMoveParams.mDestination - mActor.GetPosition()).sqrMagnitude;
		if (sqrMagnitude <= mMoveParams.mDestinationThresholdSqr)
		{
			mInRangeOfTarget = true;
			if (!WaitForSync(TaskSynchroniser.SyncState.InPosition))
			{
				state = State.Done;
			}
		}
	}

	protected override void ConstructRoute()
	{
		if (mMoveParams.mDestination == mLastFramesTargetPosition)
		{
			return;
		}
		if (!mActor.navAgent.enabled)
		{
			if (!mActor.realCharacter.NextFrameNavMesh)
			{
				state = State.Done;
			}
			return;
		}
		mActor.navAgent.CalculatePath(mMoveParams.mDestination, mPath);
		if (mPath.status == NavMeshPathStatus.PathInvalid)
		{
			state = State.Done;
			return;
		}
		WaypointMarker marker = WaypointMarkerManager.Instance.GetMarker(mActor.gameObject);
		if (marker != null && marker.type == WaypointMarker.Type.OpenGround)
		{
			if (mMoveParams.mFinalLookAtValid && mPath.corners.GetLength(0) > 0)
			{
				marker.transform.forward = mMoveParams.mFinalLookAt - mPath.corners[mPath.corners.GetLength(0) - 1];
			}
			else if (mPath.corners.GetLength(0) > 1)
			{
				marker.transform.forward = mPath.corners[mPath.corners.GetLength(0) - 1] - mPath.corners[mPath.corners.GetLength(0) - 2];
			}
		}
		mNextCorner = 0;
		mPrevCorner = 0;
		mActor.realCharacter.Stand();
	}

	private void Follow()
	{
		if (mMovingTarget == null)
		{
			state = State.Done;
		}
		SetPieceLogic setPieceLogic = null;
		float delay = 0f;
		float num = Time.time;
		float gaitSpeed = mActor.realCharacter.GetGaitSpeed(mMoveParams.mMovementStyle);
		mPrevCorner = mNextCorner;
		Vector3 point = mActor.GetPosition();
		while (mNextCorner < mPath.corners.GetLength(0) - 2)
		{
			Collider[] array = Physics.OverlapSphere(mPath.corners[mNextCorner], 0.5f, mNavZoneLayerMask);
			for (int i = 0; i < array.GetLength(0); i++)
			{
				NavZoneRef component = array[i].GetComponent<NavZoneRef>();
				if (component != null)
				{
					if (mActor.gestures != null)
					{
						mActor.gestures.SetValidGestureRequests(HandGestureModule.ForbidPriority.CurrentAction, HandGestureModule.GestureEnum.kCount);
						mActor.gestures.SetValidGestureParticipation(HandGestureModule.ForbidPriority.CurrentAction, HandGestureModule.GestureEnum.kCount);
					}
					setPieceLogic = mActor.realCharacter.mNavigationSetPiece.CreateSetPiece(mActor, point, mPath.corners[mNextCorner], mPath.corners[mNextCorner + 1], mPath.corners[mNextCorner + 2], component.NavZone, mMoveParams.mMovementStyle, num, out delay);
					if (setPieceLogic != null)
					{
						break;
					}
				}
			}
			if (setPieceLogic != null)
			{
				InheritableMovementParams moveParams = new InheritableMovementParams(mMoveParams.mMovementStyle);
				TaskSetPiece taskSetPiece = new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType | base.ConfigFlags, setPieceLogic, false, true, false, moveParams, delay);
				taskSetPiece.AbortTarget = base.AbortTarget;
				mNextCorner += 2;
				return;
			}
			if (mNextCorner > 0)
			{
				num += Vector3.Distance(mPath.corners[mNextCorner - 1], mPath.corners[mNextCorner]) / gaitSpeed;
			}
			point = mPath.corners[mNextCorner];
			mNextCorner++;
		}
		if (mActor.gestures != null)
		{
			mActor.gestures.SetValidGestureRequests(HandGestureModule.ForbidPriority.CurrentAction, HandGestureModule.GestureEnum.kAll);
			mActor.gestures.SetValidGestureParticipation(HandGestureModule.ForbidPriority.CurrentAction, HandGestureModule.GestureEnum.kAll);
		}
		if (mOwner.IsRunningTask(typeof(TaskStealthKill)))
		{
			mActor.Command("Stealth");
		}
		Move();
	}

	private void Move()
	{
		if (!mActor.realCharacter.IsUsingNavMesh())
		{
			return;
		}
		if (mInRangeOfTarget)
		{
			if (!hasStopped)
			{
				mActor.navAgent.ResetPath();
				if (mMovingTarget.navAgent.velocity.sqrMagnitude < 0.01f)
				{
					mActor.navAgent.destination = mActor.GetPosition();
					mActor.navAgent.Stop();
					mActor.navAgent.speed = 0f;
				}
				hasStopped = true;
			}
			mActor.realCharacter.SetIsMoving(false, mMoveParams.mDestination);
		}
		else
		{
			hasStopped = false;
			mActor.navAgent.destination = mMoveParams.mDestination;
			mActor.realCharacter.MovementStyleActive = mMoveParams.mMovementStyle;
			mActor.navAgent.speed = mActor.baseCharacter.GetGaitSpeed(mActor.realCharacter.MovementStyleActive) * Random.Range(0.95f, 1.05f);
			mActor.realCharacter.SetIsMoving(true, mMoveParams.mDestination);
		}
	}

	public override void Finish()
	{
		mActor.realCharacter.SetIsMoving(false, mMoveParams.mDestination);
	}
}
