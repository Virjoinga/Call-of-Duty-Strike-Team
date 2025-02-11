using UnityEngine;

public class TaskMoveTo : Task
{
	private enum State
	{
		Start = 0,
		MovingToDestination = 1,
		NearingDestination = 2,
		WaitingForDoor = 3,
		GoingThroughDoor = 4,
		Done = 5
	}

	public const float kCheckForDoorDistance = 9f;

	public const float kSlowForDoorDistance = 5f;

	public const float kArrivedAtDoorDistance = 2f;

	public const float kMinWalkSpeedRange = 0.95f;

	public const float kMaxWalkSpeedRange = 1.05f;

	public const float kMinRunSpeedRange = 0.95f;

	public const float kMaxRunSpeedRange = 1.05f;

	public const float kRotateTowardsDistSqr = 6.25f;

	public const float kNearDestinationDistSqrWalking = 2.25f;

	public const float kNearDestinationDistSqrRunning = 6.25f;

	public const float kMinStopGestureJourneySqr = 16f;

	private float mEnemyBreakoutRange;

	private int waypointAccessKey;

	private int mPOITimer;

	private State mState;

	private HandGestureModule mHandGestureModule;

	private TargetWrapper mTargetWrapper;

	private int advance_handle;

	private int stop_handle;

	private float mNearDestinationDistSqr;

	private float mNearDestinationMinSpeed;

	private bool mDoStopGesture;

	private DoorNavigator pendingDoor;

	private DoorNavigator doorToIgnore;

	private float targetMovementSpeed;

	private InheritableMovementParams mMoveParams;

	public float EnemyBreakoutRange
	{
		set
		{
			mEnemyBreakoutRange = value;
		}
	}

	public BaseCharacter.MovementStyle MoveSpeed
	{
		get
		{
			return mMoveParams.mMovementStyle;
		}
	}

	public TaskMoveTo(TaskManager owner, TaskManager.Priority priority, Config flags, InheritableMovementParams moveParams, TargetWrapper targetWrapper)
		: this(owner, priority, flags, moveParams)
	{
		mTargetWrapper = targetWrapper;
	}

	public TaskMoveTo(TaskManager owner, TaskManager.Priority priority, Config flags, InheritableMovementParams moveParams)
		: base(owner, priority, flags)
	{
		mMoveParams = moveParams;
		TBFAssert.DoAssert(mActor.navAgent, string.Format("Character '{0}' has no NavMeshAgent. Unable to move, will crash.", owner.name));
		mActor.realCharacter.TargetPosition = mMoveParams.mDestination;
		mTargetWrapper = null;
		if (mActor.realCharacter.Carried != null)
		{
			mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.Walk;
		}
		if (!CharacterTypeHelper.IsAllowedToOpenDoors(mActor.awareness.ChDefCharacterType))
		{
			mMoveParams.navigateDoors = false;
		}
		mActor.Command("MoveToBegin");
		mState = State.Start;
		advance_handle = -1;
		stop_handle = -1;
		BaseCharacter.MovementStyle mMovementStyle = mMoveParams.mMovementStyle;
		if (mMovementStyle == BaseCharacter.MovementStyle.Run)
		{
			mNearDestinationDistSqr = 6.25f;
		}
		else
		{
			mNearDestinationDistSqr = 2.25f;
		}
		mHandGestureModule = mOwner.GetComponent<HandGestureModule>();
		mDoStopGesture = (mMoveParams.mDestination - mActor.GetPosition()).sqrMagnitude > 16f;
		if (mHandGestureModule != null && mDoStopGesture)
		{
			advance_handle = mHandGestureModule.RequestGesture(HandGestureModule.GestureEnum.kAdvance);
		}
		float sqrMagnitude = (mMoveParams.mDestination - mActor.GetPosition()).sqrMagnitude;
		if (sqrMagnitude < mMoveParams.mDestinationThresholdSqr)
		{
			mState = State.Done;
		}
		if (mActor.behaviour.PlayerControlled)
		{
			WaypointMarker marker = WaypointMarkerManager.Instance.GetMarker(mOwner.gameObject);
			if (marker != null)
			{
				waypointAccessKey = marker.accessKey;
			}
		}
	}

	public override void Update()
	{
		if (mMoveParams.mFinalLookAtObject != null)
		{
			mMoveParams.FinalLookAt = mMoveParams.mFinalLookAtObject.transform.position;
			mMoveParams.mFinalLookAtValid = true;
		}
		bool flag = OptimisationManager.Update(OptType.TaskMoveTo, mActor);
		float sqrMagnitude = (mMoveParams.mDestination - mActor.GetPosition()).sqrMagnitude;
		float gaitSpeed = mActor.baseCharacter.GetGaitSpeed(mActor.baseCharacter.MovementStyleActive);
		if (Mathf.Abs(targetMovementSpeed - gaitSpeed) > 1f)
		{
			targetMovementSpeed = gaitSpeed;
		}
		if (mActor.behaviour.PlayerControlled && mState != 0)
		{
			WaypointMarker marker = WaypointMarkerManager.Instance.GetMarker(mOwner.gameObject);
			if (marker != null && (marker.accessKey == waypointAccessKey || marker.accessKey == 0))
			{
				marker.RefreshPaths = true;
				marker.accessKey = waypointAccessKey;
			}
			if ((base.ConfigFlags & Config.AbortIfDestinationBlocked) != 0 && sqrMagnitude < 0.46474996f)
			{
				base.Owner.mVolatileIterator.ResetWithMask(GKM.FactionMask(FactionHelper.Category.Player) & ~mActor.ident);
				Actor a;
				while (base.Owner.mVolatileIterator.NextActor(out a))
				{
					if ((a.GetPosition() - mMoveParams.mDestination).sqrMagnitude < 0.46474996f)
					{
						mMoveParams.mDestinationThresholdSqr = 100f;
					}
				}
			}
		}
		switch (mState)
		{
		case State.Start:
			StartMoving(mMoveParams.mDestination);
			mState = State.MovingToDestination;
			break;
		case State.MovingToDestination:
			if (flag)
			{
				if (sqrMagnitude < mNearDestinationDistSqr)
				{
					if (mHandGestureModule != null && mDoStopGesture)
					{
						stop_handle = mHandGestureModule.RequestGesture(HandGestureModule.GestureEnum.kStop);
					}
					mState = State.NearingDestination;
				}
				DoDoorCheck();
			}
			WorldHelper.AvoidObstacles(mActor, null);
			break;
		case State.NearingDestination:
			if (flag)
			{
				DoDoorCheck();
			}
			break;
		case State.WaitingForDoor:
		{
			bool canRun = true;
			SetPieceModule spm;
			if (pendingDoor.TellMeWhatToDo(mActor, out spm, ref canRun))
			{
				StartDoorSetPiece(spm, canRun);
			}
			break;
		}
		case State.GoingThroughDoor:
		{
			int num = 0;
			num |= 1 << (NavMesh.GetNavMeshLayerFromName("Default") & 0x1F);
			NavMeshHit hit;
			if (mActor.navAgent.SamplePathPosition(num, 9f, out hit))
			{
				pendingDoor = DoorNavigator.GetNavigatorFromLayerMask(hit.mask, hit.position);
				if (pendingDoor != doorToIgnore)
				{
					mState = State.MovingToDestination;
				}
			}
			else
			{
				mState = State.MovingToDestination;
			}
			break;
		}
		}
		bool flag2 = sqrMagnitude < mNearDestinationDistSqr;
		bool flag3 = mMoveParams.mFinalLookAtValid && sqrMagnitude < mMoveParams.mLookTowardsDistSqr;
		bool flag4 = mMoveParams.mFinalLookAtValid && sqrMagnitude < mMoveParams.mLookTowardsDistSqr * 4f;
		if (flag2)
		{
			if (mMoveParams.CrouchAtEnd())
			{
				mActor.realCharacter.Crouch();
			}
			if (mMoveParams.stanceAtEnd == InheritableMovementParams.StanceOrder.CrouchInCover || mMoveParams.stanceAtEnd == InheritableMovementParams.StanceOrder.StandInCover)
			{
				mActor.Command("NoAim");
			}
		}
		else if ((mActor.realCharacter == null || !mActor.realCharacter.IsFirstPerson) && mActor.baseCharacter.IsCrouching() && mMoveParams.mMovementStyle == BaseCharacter.MovementStyle.AsFastAsSafelyPossible && (mActor.behaviour == null || mActor.behaviour.SafeToStand()))
		{
			mActor.realCharacter.Stand();
		}
		Vector3 lhs = mMoveParams.mFinalLookAt - mMoveParams.mDestination;
		float sqrMagnitude2 = lhs.sqrMagnitude;
		Vector3 rhs = mMoveParams.mDestination - mActor.transform.position;
		float sqrMagnitude3 = rhs.sqrMagnitude;
		bool flag5 = false;
		if (flag4 && sqrMagnitude3 > 1f && sqrMagnitude2 > 0.1f)
		{
			float num2 = Vector3.Dot(lhs, rhs);
			flag5 = num2 * Mathf.Abs(num2) < sqrMagnitude3 * sqrMagnitude2 * 0.5f;
		}
		float speed = mActor.navAgent.speed;
		if (flag2 || flag5)
		{
			speed = Mathf.Max(speed - Time.deltaTime * 2f, mActor.realCharacter.Settings.WalkSpeed);
			mActor.navAgent.speed = speed;
		}
		else
		{
			mActor.navAgent.speed = WorldHelper.ExpBlend(mActor.navAgent.speed, targetMovementSpeed, 0.05f);
		}
		if (flag3)
		{
			Vector3 vector = mMoveParams.mFinalLookAt - mMoveParams.mDestination;
			mActor.realCharacter.ImposeLookDirection(vector.normalized);
			mActor.realCharacter.PickSomethingToAimAt(null);
			mActor.realCharacter.ImposeLookDirection(vector.normalized);
		}
		else if (mMoveParams.forceFaceForwards)
		{
			mActor.realCharacter.ImposeLookDirection(mActor.navAgent.velocity);
			mActor.realCharacter.PickSomethingToAimAt(null);
			mActor.realCharacter.ImposeLookDirection(mActor.navAgent.velocity);
		}
		if (mActor.realCharacter.FirstPersonCamera != null)
		{
			mActor.realCharacter.FirstPersonCamera.Angles = mActor.model.transform.rotation.eulerAngles;
		}
	}

	private void DoDoorCheck()
	{
		if (!mMoveParams.navigateDoors)
		{
			return;
		}
		int num = 0;
		num |= 1 << (NavMesh.GetNavMeshLayerFromName("Default") & 0x1F);
		NavMeshHit hit;
		if (!mActor.navAgent.SamplePathPosition(num, 9f, out hit))
		{
			return;
		}
		pendingDoor = DoorNavigator.GetNavigatorFromLayerMask(hit.mask, hit.position);
		if (pendingDoor != null)
		{
			if (!pendingDoor.NavigableBy(mActor) || !(pendingDoor != doorToIgnore) || !pendingDoor.Manipulable())
			{
				return;
			}
			if (hit.distance <= 2f)
			{
				pendingDoor.Arrive(mActor);
				bool canRun = true;
				SetPieceModule spm;
				if (pendingDoor.TellMeWhatToDo(mActor, out spm, ref canRun))
				{
					StartDoorSetPiece(spm, canRun);
					return;
				}
				mState = State.WaitingForDoor;
				mActor.navAgent.Stop();
			}
			else
			{
				if (hit.distance <= 5f && !pendingDoor.CanRunThrough(hit.position))
				{
					targetMovementSpeed = mActor.realCharacter.GetGaitSpeed(BaseCharacter.MovementStyle.Walk);
				}
				pendingDoor.Approach(mActor, hit.distance / mActor.navAgent.speed + WorldHelper.ThisFrameTime, hit.position);
			}
		}
		else
		{
			doorToIgnore = null;
		}
	}

	private void StartDoorSetPiece(SetPieceModule spm, bool canRun)
	{
		if (spm != null)
		{
			spm = ((GameObject)Object.Instantiate(spm.gameObject)).GetComponent<SetPieceModule>();
			SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
			setPieceLogic.SetModule(spm);
			setPieceLogic.PlaceSetPiece(pendingDoor.setPieceTransform);
			setPieceLogic.SetActor_IndexOnlyCharacters(0, mActor);
			setPieceLogic.SetObject(1, pendingDoor.DoorMesh);
			InheritableMovementParams inheritableMovementParams = mMoveParams.Clone();
			TaskSetPiece taskSetPiece = null;
			if (mActor == GameController.Instance.mFirstPersonActor)
			{
				inheritableMovementParams.mDestination = Vector3.zero;
				inheritableMovementParams.FinalLookDirection = pendingDoor.transform.forward;
				inheritableMovementParams.navigateDoors = false;
				mActor.tasks.CancelTasks<TaskRouteTo>();
				mActor.tasks.CancelTasks<TaskMoveTo>();
				taskSetPiece = new TaskSetPiece(mOwner, TaskManager.Priority.REACTIVE, Config.Default, setPieceLogic, true, false, false, inheritableMovementParams, 0f);
			}
			else
			{
				inheritableMovementParams.navigateDoors = false;
				inheritableMovementParams.mMovementStyle = BaseCharacter.MovementStyle.Walk;
				inheritableMovementParams.stanceAtEnd = InheritableMovementParams.StanceOrder.Stand;
				taskSetPiece = new TaskSetPiece(mOwner, TaskManager.Priority.REACTIVE, Config.Default, setPieceLogic, false, true, false, inheritableMovementParams, 0f);
			}
			if (taskSetPiece != null)
			{
				taskSetPiece.AddToMoveFlags(Config.DenyPlayerInput);
			}
		}
		else
		{
			StartMoving(mMoveParams.mDestination);
			if (!canRun)
			{
				mActor.navAgent.speed = mActor.realCharacter.GetGaitSpeed(BaseCharacter.MovementStyle.Walk);
			}
			doorToIgnore = pendingDoor;
		}
		mState = State.GoingThroughDoor;
	}

	public override bool HasFinished()
	{
		if ((bool)AlarmManager.Instance)
		{
			TaskUseAlarmPanel taskUseAlarmPanel = (TaskUseAlarmPanel)mOwner.GetRunningTask(typeof(TaskUseAlarmPanel));
			if (taskUseAlarmPanel != null && AlarmManager.Instance.AlarmSounding)
			{
				return true;
			}
		}
		if (mState == State.Start || mState == State.GoingThroughDoor)
		{
			return false;
		}
		if (CheckConfigFlagsFinished())
		{
			if (mTargetWrapper != null)
			{
				mTargetWrapper.ClearSearch();
				mTargetWrapper = null;
			}
			return true;
		}
		float sqrMagnitude = (mMoveParams.mDestination - mActor.GetPosition()).sqrMagnitude;
		if (sqrMagnitude < mMoveParams.mDestinationThresholdSqr)
		{
			if (mTargetWrapper != null)
			{
				mTargetWrapper.Search();
			}
			mOwner.Feedback(TaskManager.TaskResult.Complete, "Close");
			return true;
		}
		if (mTargetWrapper != null && (mTargetWrapper.HasBeenSearched() || !mTargetWrapper.HasBeenReserved()))
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
				float sqrMagnitude2 = (a.GetPosition() - mActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude2 < num)
				{
					return true;
				}
			}
		}
		if (mMoveParams.abortOnExitTether && !mActor.tether.IsWithinTether())
		{
			return true;
		}
		bool flag = !mActor.navAgent.pathPending && !mActor.navAgent.hasPath;
		if (flag)
		{
			if (mTargetWrapper != null)
			{
				mTargetWrapper.Search();
			}
			if (mActor.behaviour.PlayerControlled)
			{
				TargetWrapperManager.Instance().AutoClearNearbyVisibleTargetWrappers(mActor.GetPosition());
			}
			mOwner.Feedback(TaskManager.TaskResult.Complete, null);
		}
		return flag;
	}

	public override void Finish()
	{
		if ((bool)mHandGestureModule)
		{
			mHandGestureModule.CancelGesture(advance_handle);
			mHandGestureModule.CancelGesture(stop_handle);
		}
		if (mActor.realCharacter.IsUsingNavMesh())
		{
			mActor.navAgent.Stop();
			if (mMoveParams.stopDead)
			{
				mActor.navAgent.speed = 0f;
			}
		}
		mActor.realCharacter.SetIsMoving(false, Vector3.zero);
	}

	public override void OnSleep()
	{
		if (mActor.realCharacter.IsUsingNavMesh())
		{
			mActor.navAgent.Stop();
			if (mMoveParams.stopDead)
			{
				mActor.navAgent.speed = 0f;
			}
		}
		mActor.realCharacter.SetIsMoving(false, Vector3.zero);
	}

	public override void OnResume()
	{
		if (mActor.realCharacter.IsUsingNavMesh())
		{
			if (mState == State.GoingThroughDoor)
			{
				mState = State.MovingToDestination;
				doorToIgnore = pendingDoor;
				StartMoving(mMoveParams.mDestination);
			}
			else
			{
				mActor.navAgent.Resume();
			}
			mActor.realCharacter.SetIsMoving(true, Vector3.zero);
		}
	}

	private void StartMoving(Vector3 destination)
	{
		mActor.realCharacter.MovementStyleActive = mMoveParams.mMovementStyle;
		targetMovementSpeed = mActor.realCharacter.GetGaitSpeed(mActor.realCharacter.MovementStyleActive) * Random.Range(0.95f, 1.05f);
		if (mActor.realCharacter.MovementStyleActive == BaseCharacter.MovementStyle.Run)
		{
			mActor.realCharacter.Stand();
		}
		if (mActor.realCharacter.IsUsingNavMesh())
		{
			mActor.navAgent.destination = destination;
			mActor.navAgent.speed = targetMovementSpeed;
			mActor.realCharacter.SetIsMoving(true, destination);
		}
	}

	public override void Command(string com)
	{
		switch (com)
		{
		case "Run":
			mActor.realCharacter.Stand();
			mActor.realCharacter.MovementStyleActive = BaseCharacter.MovementStyle.Run;
			mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.Run;
			if (mMoveParams.stanceAtEnd == InheritableMovementParams.StanceOrder.CrouchFromStealth)
			{
				mMoveParams.stanceAtEnd = InheritableMovementParams.StanceOrder.Stand;
			}
			mActor.navAgent.speed = mActor.realCharacter.GetGaitSpeed(mMoveParams.mMovementStyle) * Random.Range(0.95f, 1.05f);
			targetMovementSpeed = mActor.navAgent.speed;
			break;
		case "Crouch":
			mActor.realCharacter.Crouch();
			if (mActor.realCharacter.MovementStyleActive == BaseCharacter.MovementStyle.Run)
			{
				mActor.realCharacter.MovementStyleActive = BaseCharacter.MovementStyle.Walk;
				mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.Walk;
			}
			mActor.navAgent.speed = mActor.realCharacter.GetGaitSpeed(mMoveParams.mMovementStyle) * Random.Range(0.95f, 1.05f);
			targetMovementSpeed = mActor.navAgent.speed;
			break;
		case "Stand":
			if (mActor.realCharacter.GetStance() == BaseCharacter.Stance.Crouched)
			{
				mActor.realCharacter.Stand();
				mActor.navAgent.speed = mActor.realCharacter.GetGaitSpeed(mMoveParams.mMovementStyle) * Random.Range(0.95f, 1.05f);
				targetMovementSpeed = mActor.navAgent.speed;
			}
			break;
		}
	}

	public void CancelFinalLookAt()
	{
		mMoveParams.mFinalLookAtValid = false;
	}
}
