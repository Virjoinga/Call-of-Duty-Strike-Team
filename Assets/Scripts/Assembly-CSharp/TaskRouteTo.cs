using UnityEngine;

public class TaskRouteTo : Task
{
	private enum State
	{
		Start = 0,
		WaitPath = 1,
		AnalysePath = 2,
		FollowRoute = 3,
		AlertToRescheduling = 4,
		Done = 5
	}

	private const float kMinimumCrawlDistanceSqr = 81f;

	private float mEnemyBreakoutRange;

	protected InheritableMovementParams mMoveParams;

	protected UnityEngine.AI.NavMeshPath mPath;

	private State state;

	protected int mNextCorner;

	protected int mPrevCorner;

	protected int mNavZoneLayerMask;

	protected bool mFailedToExitContainer;

	private TargetWrapper mTargetWrapper;

	public BaseCharacter.MovementStyle MoveSpeed
	{
		get
		{
			return mMoveParams.mMovementStyle;
		}
	}

	public float EnemyBreakoutRange
	{
		set
		{
			mEnemyBreakoutRange = value;
		}
	}

	public TaskRouteTo(TaskManager owner, TaskManager.Priority priority, Config flags, InheritableMovementParams moveParams)
		: base(owner, priority, flags)
	{
		mActor.baseCharacter.StartRouting();
		mMoveParams = moveParams;
		TaskSetPiece taskSetPiece = (TaskSetPiece)mOwner.GetRunningTask(typeof(TaskSetPiece));
		if (taskSetPiece != null && taskSetPiece != mConsultant)
		{
			taskSetPiece.Cancel();
		}
		TBFAssert.DoAssert(mActor.navAgent != null, string.Format("Character '{0}' has no NavMeshAgent. Unable to move, will crash.", owner.name));
		mNavZoneLayerMask = 1 << LayerMask.NameToLayer("NavZones");
		mPath = new UnityEngine.AI.NavMeshPath();
		mFailedToExitContainer = false;
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.FriendsMask(mActor) & GKM.AliveMask & ~mActor.ident);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!(a != null))
			{
				continue;
			}
			TaskFollow runningTask = a.tasks.GetRunningTask<TaskFollow>();
			if (runningTask != null && runningTask.Target == mActor && mActor.realCharacter.NumberOfFollowers > 0)
			{
				a.tasks.CancelTasks(typeof(TaskMoveTo));
			}
			if (a.behaviour.alertState <= BehaviourController.AlertState.Focused)
			{
				TaskDefend runningTask2 = a.tasks.GetRunningTask<TaskDefend>();
				if (runningTask2 != null && runningTask2.Target == mActor && mActor.realCharacter.NumberOfDefenders > 0)
				{
					a.tasks.CancelTasks(typeof(TaskMoveTo));
				}
			}
		}
		float sqrMagnitude = (mMoveParams.mDestination - mActor.GetPosition()).sqrMagnitude;
		if (sqrMagnitude < mMoveParams.mDestinationThresholdSqr)
		{
			state = State.Done;
		}
		mActor.realCharacter.MovementStyleActive = mMoveParams.mMovementStyle;
	}

	public TaskRouteTo(TaskManager owner, TaskManager.Priority priority, Config flags, InheritableMovementParams moveParams, TargetWrapper targetWrapper)
		: this(owner, priority, flags, moveParams)
	{
		mTargetWrapper = targetWrapper;
	}

	public override void Update()
	{
		switch (state)
		{
		case State.Start:
			ConstructRoute();
			break;
		case State.FollowRoute:
			FollowRoute();
			break;
		case State.AlertToRescheduling:
			if (mOwner.lastFeedback.mResult == TaskManager.TaskResult.Aborted)
			{
				if (!(mOwner.lastFeedback.mDetails == "Reschedule"))
				{
					state = State.Done;
					break;
				}
				mNextCorner = mPrevCorner;
			}
			state = State.FollowRoute;
			FollowRoute();
			break;
		case State.WaitPath:
		case State.AnalysePath:
			break;
		}
	}

	public override bool HasFinished()
	{
		if (CheckConfigFlagsFinished())
		{
			return true;
		}
		if (mEnemyBreakoutRange > 0f)
		{
			float num = mEnemyBreakoutRange * mEnemyBreakoutRange;
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(mActor.awareness.EnemiesIKnowAbout);
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				float sqrMagnitude = (a.GetPosition() - mActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					return true;
				}
			}
		}
		if (mMoveParams.abortOnExitTether && !mActor.tether.IsWithinTether())
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

	public override void Finish()
	{
		if (GameController.Instance.PlayerEngagedInCombat)
		{
			QueuedOrder.ExecuteOrders(mActor);
		}
		mActor.baseCharacter.EndRouting();
		RemoveMarker();
	}

	public override void OnSleep()
	{
	}

	public override void OnResume()
	{
		if (mOwner.lastFeedback.mResult == TaskManager.TaskResult.Aborted)
		{
			if (mOwner.lastFeedback.mDetails == "Reschedule")
			{
				mNextCorner = mPrevCorner;
				state = State.FollowRoute;
			}
			else
			{
				RemoveMarker();
				state = State.Done;
			}
		}
		else if (state == State.Done)
		{
			RemoveMarker();
		}
	}

	protected void RemoveMarker()
	{
		if (mActor != null && mActor.behaviour.PlayerControlled)
		{
			WaypointMarkerManager.Instance.RemoveMarker(mActor.gameObject);
		}
	}

	protected virtual void ConstructRoute()
	{
		if (!mActor.navAgent.enabled)
		{
			if (!mActor.realCharacter.NextFrameNavMesh)
			{
				state = State.Done;
			}
			return;
		}
		if (!WorldHelper.CalculatePath_AvoidingMantlesWhenCarrying(mActor, mMoveParams.mDestination, mPath))
		{
			state = State.Done;
			return;
		}
		if (mPath.status == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
		{
			state = State.Done;
			return;
		}
		WaypointMarker marker = WaypointMarkerManager.Instance.GetMarker(mActor.gameObject);
		if (marker != null && marker.type == WaypointMarker.Type.OpenGround)
		{
			Vector3 forward = marker.transform.forward;
			if (mMoveParams.mFinalLookAtValid && mPath.corners.GetLength(0) > 0)
			{
				forward = mMoveParams.mFinalLookAt - mPath.corners[mPath.corners.GetLength(0) - 1];
			}
			else if (mPath.corners.GetLength(0) > 1)
			{
				forward = mPath.corners[mPath.corners.GetLength(0) - 1] - mPath.corners[mPath.corners.GetLength(0) - 2];
			}
			forward.y = 0f;
			marker.transform.forward = forward;
		}
		if (mPath.corners.GetLength(0) <= 2)
		{
			NavigationZoneManager.ClearBookings(mActor);
			if (mActor.gestures != null)
			{
				mActor.gestures.SetValidGestureRequests(HandGestureModule.ForbidPriority.CurrentAction, HandGestureModule.GestureEnum.kAll);
				mActor.gestures.SetValidGestureParticipation(HandGestureModule.ForbidPriority.CurrentAction, HandGestureModule.GestureEnum.kAll);
			}
			if (mMoveParams.forceCrouch)
			{
				mActor.realCharacter.Crouch();
			}
			else if (mPath.corners.GetLength(0) == 2 && (mPath.corners[1] - mPath.corners[0]).sqrMagnitude > 81f && (mMoveParams.mMovementStyle == BaseCharacter.MovementStyle.Run || mActor.behaviour.SafeToStand()))
			{
				mActor.realCharacter.Stand();
			}
			TaskMoveTo taskMoveTo = new TaskMoveTo(mOwner, base.Priority, Config.ClearAllCurrentType | base.ConfigFlags, mMoveParams, mTargetWrapper);
			taskMoveTo.AbortTarget = base.AbortTarget;
			taskMoveTo.EnemyBreakoutRange = mEnemyBreakoutRange;
			if (mOwner.IsRunningTask(typeof(TaskStealthKill)))
			{
				mActor.Command("Stealth");
			}
			state = State.Done;
		}
		else
		{
			mNextCorner = 0;
			mPrevCorner = 0;
			if (mMoveParams.forceCrouch)
			{
				mActor.realCharacter.Crouch();
			}
			else if (mMoveParams.mMovementStyle == BaseCharacter.MovementStyle.Run || mActor.behaviour.SafeToStand())
			{
				mActor.realCharacter.Stand();
			}
			state = State.FollowRoute;
			FollowRoute();
		}
	}

	private void FollowRoute()
	{
		SetPieceLogic setPieceLogic = null;
		float delay = 0f;
		float num = Time.time;
		float gaitSpeed = mActor.baseCharacter.GetGaitSpeed(mMoveParams.mMovementStyle);
		mPrevCorner = mNextCorner;
		Vector3 point = mActor.GetPosition();
		WaypointMarker marker = WaypointMarkerManager.Instance.GetMarker(mActor.gameObject);
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
				if (marker != null)
				{
					marker.SetPathRemainder(mPath, mNextCorner + 1);
				}
				InheritableMovementParams moveParams = new InheritableMovementParams(mMoveParams.mMovementStyle);
				mOwner.CancelTasksOtherThanThisOne<TaskSetPiece>(mConsultant);
				TaskSetPiece taskSetPiece = new TaskSetPiece(mOwner, base.Priority, base.ConfigFlags, setPieceLogic, false, true, false, moveParams, delay);
				taskSetPiece.AbortTarget = base.AbortTarget;
				mNextCorner += 2;
				state = State.AlertToRescheduling;
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
		if (marker != null)
		{
			marker.ClearPathRemainder();
		}
		TaskMoveTo taskMoveTo = new TaskMoveTo(mOwner, base.Priority, Config.ClearAllCurrentType | base.ConfigFlags, mMoveParams, mTargetWrapper);
		taskMoveTo.AbortTarget = base.AbortTarget;
		taskMoveTo.EnemyBreakoutRange = mEnemyBreakoutRange;
		state = State.Done;
	}

	public override void Command(string com)
	{
		switch (com)
		{
		case "Run":
			mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.Run;
			if (mMoveParams.stanceAtEnd == InheritableMovementParams.StanceOrder.CrouchFromStealth)
			{
				mMoveParams.stanceAtEnd = InheritableMovementParams.StanceOrder.Stand;
			}
			break;
		case "Crouch":
			if (mMoveParams.mMovementStyle == BaseCharacter.MovementStyle.Run)
			{
				mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.Walk;
			}
			break;
		case "Stand":
			break;
		}
	}
}
