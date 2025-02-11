using UnityEngine;

public class AGRModule : BasePoseModule
{
	public enum AGRStateEnum
	{
		kBreak = 0,
		kBreakIdle = 1,
		kDiffuse = 2,
		kIdle = 3,
		kRepair = 4,
		kRollForward = 5,
		kRollLeft = 6,
		kRollRight = 7
	}

	protected enum CategoryEnum
	{
		kMovement = 0,
		kCount = 1
	}

	protected enum ActionEnum
	{
		kBreak = 0,
		kBreakIdle = 1,
		kDiffuse = 2,
		kIdle = 3,
		kRepair = 4,
		kRollForward = 5,
		kRollLeft = 6,
		kRollRight = 7,
		kCount = 8
	}

	public float mMaxWalkAnimSpeed;

	public float mDebugAnimStartTime;

	public float mShuffleWindowStart;

	public float mShuffleWindowEnd;

	public Vector3 mLogicalPosition;

	protected Vector3 mLogicalVelocity;

	public AGRStateEnum mState;

	protected ActionEnum mCurrentWalkAction;

	protected float mWalkAnimSpeed;

	protected int mStartRunningDelay;

	protected bool mPointing;

	protected float mCantTurnTimer;

	protected Vector3 mDesiredForward;

	protected PoseModuleSharedData.CommandCode newState;

	public float mWalkStrideLength;

	public float mTargetWalkAnimTime;

	private float mIdleTime;

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
		SkinnedMeshRenderer[] componentsInChildren = base.Model.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
		{
			if (skinnedMeshRenderer.gameObject.name == "Arm02Mesh" || skinnedMeshRenderer.gameObject.name == "ArmHeadMesh")
			{
				skinnedMeshRenderer.updateWhenOffscreen = true;
			}
		}
		Reset(mLogicalPosition, Vector3.forward, 0f);
		base.mAnimDirector.PlayAction(mActionHandle[3]);
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		mLogicalPosition = base.modeltransposition;
		mLogicalVelocity.Set(0f, 0f, 0f);
		mState = AGRStateEnum.kIdle;
		mTargetWalkAnimTime = -1f;
		mWalkAnimSpeed = 1f;
		mStartRunningDelay = 0;
		mShuffleWindowStart = 0.65f;
		mShuffleWindowEnd = 1f;
		mWalkStrideLength = 42f * mDesc.mStandardWalkSpeed;
		mDesiredForward = Vector3.zero;
		mCurrentWalkAction = ActionEnum.kCount;
		mMaxWalkAnimSpeed = 0f;
	}

	private void Reset(Vector3 pos, Vector3 aim, float debuganimstarttime)
	{
		mState = AGRStateEnum.kIdle;
		mCurrentWalkAction = ActionEnum.kCount;
		mDebugAnimStartTime = debuganimstarttime;
		mPointing = false;
		mCantTurnTimer = 0f;
	}

	protected void Start_GetActionHandles()
	{
		mCategoryHandle = new int[1];
		mActionHandle = new AnimDirector.ActionHandle[8];
		GetCategoryHandle(CategoryEnum.kMovement, "Movement");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kBreak, "Break");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kBreakIdle, "BreakIdle");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kDiffuse, "Diffuse");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kIdle, "Idle");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kRepair, "Repair");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kRollForward, "RollForward");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kRollLeft, "RollLeft");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kRollRight, "RollRight");
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
		base.Model.transform.position = newPos;
		base.Model.transform.forward = aimDir;
		if (newVel.sqrMagnitude == 0f)
		{
			mIdleTime += Time.deltaTime;
		}
		else
		{
			mIdleTime = 0f;
		}
		switch (mState)
		{
		case AGRStateEnum.kRollForward:
			if (mIdleTime > 0.5f)
			{
				mState = AGRStateEnum.kIdle;
				base.mAnimDirector.PlayAction(mActionHandle[3]);
			}
			break;
		case AGRStateEnum.kBreak:
			if (base.mAnimDirector.HasCurrentActionCompleted(mCategoryHandle[0]))
			{
				mState = AGRStateEnum.kBreakIdle;
				base.mAnimDirector.PlayAction(mActionHandle[1]);
			}
			break;
		case AGRStateEnum.kRepair:
			if (base.mAnimDirector.HasCurrentActionCompleted(mCategoryHandle[0]))
			{
				mState = AGRStateEnum.kIdle;
				base.mAnimDirector.PlayAction(mActionHandle[3]);
			}
			break;
		}
		return PoseModuleSharedData.Modules.AGR;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		switch (ParseCommand(com))
		{
		case PoseModuleSharedData.CommandCode.MoveToBegin:
			if (mState != AGRStateEnum.kRollForward)
			{
				mState = AGRStateEnum.kRollForward;
				base.mAnimDirector.PlayAction(mActionHandle[5]);
				mIdleTime = 0f;
			}
			break;
		case PoseModuleSharedData.CommandCode.Melee:
			if (mState != 0 && mState != AGRStateEnum.kBreakIdle)
			{
				mState = AGRStateEnum.kBreak;
				base.mAnimDirector.PlayAction(mActionHandle[0]);
			}
			break;
		case PoseModuleSharedData.CommandCode.Stand:
			if (mState == AGRStateEnum.kBreak || mState == AGRStateEnum.kBreakIdle)
			{
				mState = AGRStateEnum.kRepair;
				base.mAnimDirector.PlayAction(mActionHandle[4]);
			}
			break;
		case PoseModuleSharedData.CommandCode.PrimOut:
			mState = AGRStateEnum.kDiffuse;
			base.mAnimDirector.PlayAction(mActionHandle[2]);
			break;
		case PoseModuleSharedData.CommandCode.PrimAway:
			mState = AGRStateEnum.kIdle;
			base.mAnimDirector.PlayAction(mActionHandle[3]);
			break;
		}
		return PoseModuleSharedData.Modules.AGR;
	}
}
