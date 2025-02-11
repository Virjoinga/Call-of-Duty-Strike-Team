using UnityEngine;

public class RPGModule : BasePoseModule
{
	public enum RPGStateEnum
	{
		kStand = 0,
		kTurnToFace = 1,
		kWalk = 2,
		kFire = 3,
		kReloadIn = 4,
		kReloadLoop = 5,
		kReloadOut = 6,
		kCount = 7
	}

	protected enum CategoryEnum
	{
		kMovement = 0,
		kTurning = 1,
		kAiming = 2,
		kCount = 3
	}

	public enum ActionEnum
	{
		kStand = 0,
		kTurnToFace = 1,
		kWalk = 2,
		kFire = 3,
		kReloadIn = 4,
		kReloadLoop = 5,
		kReloadOut = 6,
		kShuffle = 7,
		kUpDown = 8,
		kCount = 9
	}

	private const float kWalkVel = 0.001f;

	private const float kWalkVelSq = 1.0000001E-06f;

	private const float kShuffleCap = 0.1f;

	private const float kReloadLoopTime = 3f;

	public RPGStateEnum mState;

	private float mShuffleBlend;

	private TaskRPG mRPGTask;

	private bool mWasMoving;

	private float mLoopTimer;

	protected MoveAimDescriptor mDesc
	{
		get
		{
			return myActor.Pose.moveAimDesc;
		}
	}

	protected override void Internal_Init()
	{
		Start_GetActionHandles();
		Reset();
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		mState = RPGStateEnum.kStand;
	}

	private void Reset()
	{
		mState = RPGStateEnum.kStand;
		mShuffleBlend = 0f;
		mWasMoving = false;
	}

	protected void Start_GetActionHandles()
	{
		mCategoryHandle = new int[3];
		mActionHandle = new AnimDirector.ActionHandle[9];
		GetCategoryHandle(CategoryEnum.kMovement, "Movement");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kStand, "Stand");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kTurnToFace, "TurnToFace");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kWalk, "Run");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kFire, "Fire");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kReloadIn, "ReloadIn");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kReloadLoop, "ReloadLoop");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kReloadOut, "ReloadOut");
		GetCategoryHandle(CategoryEnum.kTurning, "Turning");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kShuffle, "Shuffle");
		GetCategoryHandle(CategoryEnum.kAiming, "Aiming");
		GetActionHandle(CategoryEnum.kAiming, ActionEnum.kUpDown, "UpDown");
		base.mAnimDirector.PlayAction(mActionHandle[8]);
		base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0f);
		base.mAnimDirector.SetCategorySpeed(mCategoryHandle[2], 0f);
	}

	private void GetCategoryHandle(CategoryEnum cat, string name)
	{
		mCategoryHandle[(int)cat] = base.mAnimDirector.GetCategoryHandle(name);
	}

	private void GetActionHandle(CategoryEnum cat, ActionEnum act, string name)
	{
		mActionHandle[(int)act] = base.mAnimDirector.GetActionHandle(mCategoryHandle[(int)cat], name);
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		base.modeltransposition = newPos;
		base.modeltransforward = aimDir;
		TaskRPG rPGTask = GetRPGTask();
		if (rPGTask != null)
		{
			switch (mState)
			{
			case RPGStateEnum.kFire:
				if (base.mAnimDirector.HasCurrentActionCompleted(mCategoryHandle[0]))
				{
					rPGTask.NotifyAnimationStateComplete(ActionEnum.kFire);
					base.mAnimDirector.PlayAction(mActionHandle[4]);
					mState = RPGStateEnum.kReloadIn;
				}
				break;
			case RPGStateEnum.kReloadIn:
				if (base.mAnimDirector.HasCurrentActionCompleted(mCategoryHandle[0]))
				{
					base.mAnimDirector.PlayAction(mActionHandle[5]);
					mLoopTimer = 3f;
					mState = RPGStateEnum.kReloadLoop;
				}
				break;
			case RPGStateEnum.kReloadLoop:
				mLoopTimer -= Time.deltaTime;
				if (mLoopTimer <= 0f)
				{
					base.mAnimDirector.StopAction(mActionHandle[5]);
					base.mAnimDirector.PlayAction(mActionHandle[6]);
					mState = RPGStateEnum.kReloadOut;
				}
				break;
			case RPGStateEnum.kReloadOut:
				if (base.mAnimDirector.HasCurrentActionCompleted(mCategoryHandle[0]))
				{
					rPGTask.NotifyAnimationStateComplete(ActionEnum.kReloadOut);
					mState = RPGStateEnum.kStand;
				}
				break;
			}
			int categoryHandle = mCategoryHandle[2];
			if (rPGTask.IsAiming())
			{
				base.mAnimDirector.EnableCategory(categoryHandle, true, 1f);
				float num = Mathf.Clamp(rPGTask.CurrentAimDir.y + 0.5f, 0f, 1f);
				base.mAnimDirector.SetCategoryTime(categoryHandle, 1f - num);
			}
			else
			{
				base.mAnimDirector.EnableCategory(categoryHandle, false, 0.1f);
			}
		}
		else
		{
			int categoryHandle2 = mCategoryHandle[0];
			if (newVel.sqrMagnitude >= 1.0000001E-06f)
			{
				if (!mWasMoving)
				{
					base.mAnimDirector.PlayAction(mActionHandle[2]);
					base.mAnimDirector.SetCategoryWeight(categoryHandle2, 1f);
					base.mAnimDirector.SetCategorySpeed(categoryHandle2, 1f);
					mWasMoving = true;
				}
			}
			else if (mWasMoving)
			{
				base.mAnimDirector.PlayAction(mActionHandle[0]);
				base.mAnimDirector.SetCategoryWeight(categoryHandle2, 1f);
				base.mAnimDirector.SetCategorySpeed(categoryHandle2, 1f);
				mWasMoving = false;
			}
		}
		return PoseModuleSharedData.Modules.RPG;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		if (com.StartsWith("BlendShuffle"))
		{
			if (myActor != null && myActor.navAgent != null && myActor.navAgent.velocity.sqrMagnitude > float.Epsilon)
			{
				return PoseModuleSharedData.Modules.RPG;
			}
			float shuffleBlend = mRPGTask.ShuffleBlend;
			if (mShuffleBlend < 0.1f && shuffleBlend > 0.1f)
			{
				base.mAnimDirector.PlayAction(mActionHandle[7]);
			}
			else if (shuffleBlend <= 0.1f && mShuffleBlend >= 0.1f)
			{
				base.mAnimDirector.StopAction(mActionHandle[7]);
			}
			mShuffleBlend = shuffleBlend;
			base.mAnimDirector.SetCategoryWeight(mCategoryHandle[1], mShuffleBlend);
			return PoseModuleSharedData.Modules.RPG;
		}
		if (com.StartsWith("ExitReload"))
		{
			base.mAnimDirector.StopAction(mActionHandle[5]);
			mState = RPGStateEnum.kStand;
		}
		ActionEnum actionEnum = ActionEnum.kCount;
		for (int i = 0; i < 9; i++)
		{
			ActionEnum actionEnum2 = (ActionEnum)i;
			if (actionEnum2.ToString().Contains(com))
			{
				actionEnum = actionEnum2;
				break;
			}
		}
		switch (actionEnum)
		{
		case ActionEnum.kCount:
			return PoseModuleSharedData.Modules.RPG;
		default:
			base.mAnimDirector.SetCategoryWeight(mCategoryHandle[1], 0f);
			base.mAnimDirector.StopAction(mActionHandle[7]);
			mShuffleBlend = 0f;
			break;
		case ActionEnum.kStand:
		case ActionEnum.kTurnToFace:
			break;
		}
		TaskRPG rPGTask = GetRPGTask();
		if (rPGTask != null && rPGTask.IsAiming())
		{
			base.mAnimDirector.PlayAction(mActionHandle[8]);
		}
		switch (actionEnum)
		{
		case ActionEnum.kStand:
			mState = RPGStateEnum.kStand;
			break;
		case ActionEnum.kTurnToFace:
			mState = RPGStateEnum.kTurnToFace;
			break;
		case ActionEnum.kWalk:
			mState = RPGStateEnum.kWalk;
			break;
		case ActionEnum.kReloadIn:
			mState = RPGStateEnum.kReloadIn;
			WeaponSFX.Instance.RPGReload.Play(myActor.gameObject);
			break;
		case ActionEnum.kReloadOut:
			mState = RPGStateEnum.kReloadOut;
			break;
		case ActionEnum.kFire:
			mState = RPGStateEnum.kFire;
			WeaponSFX.Instance.RPGFire.Play(myActor.gameObject);
			break;
		}
		base.mAnimDirector.PlayAction(mActionHandle[(int)actionEnum], true);
		return PoseModuleSharedData.Modules.RPG;
	}

	private TaskRPG GetRPGTask()
	{
		if (mRPGTask == null)
		{
			mRPGTask = myActor.tasks.GetRunningTask<TaskRPG>();
		}
		return mRPGTask;
	}
}
