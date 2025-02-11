using System;
using UnityEngine;

public class MoveAimModule : BasePoseModule
{
	public enum MoveAimStateEnum
	{
		kStationary = 0,
		kWalking = 1,
		kStealthing = 2,
		kSauntering = 3,
		kRunning = 4,
		kPuppet = 5,
		kThrowingGrenade = 6
	}

	public enum AimType
	{
		None = 0,
		FortyFive = 1,
		Ninety = 2
	}

	protected enum CategoryEnum
	{
		kMovement = 0,
		kAiming = 1,
		kIdles = 2,
		kTurning = 3,
		kGesture = 4,
		kGrenadeThrow = 5,
		kReactions = 6,
		kCount = 7
	}

	protected enum ActionEnum
	{
		kFirstMovementAction = 0,
		kForward = 0,
		kRight = 1,
		kBackward = 2,
		kLeft = 3,
		kStand = 4,
		kStopLeftLegForward = 5,
		kStopRightLegForward = 6,
		kFirstTurningAction = 7,
		kTurnLeft45 = 7,
		kTurnRight45 = 8,
		kStandToWalkLeft130 = 9,
		kStandToWalkLeft90 = 10,
		kStandToWalkLeft60 = 11,
		kStandToWalkRight60 = 12,
		kStandToWalkRight90 = 13,
		kStandToWalkRight130 = 14,
		kStandToWalkBack = 15,
		kFirstAimingAction = 16,
		kAim = 16,
		kFirstIdleAction = 17,
		kIdle1 = 17,
		kIdle2 = 18,
		kIdle3 = 19,
		kLastIdleAction = 20,
		kFirstGestureAction = 21,
		kPrimAway = 21,
		kPrimOut = 22,
		kSecAway = 23,
		kSecOut = 24,
		kKnifeAway = 25,
		kKnifeOut = 26,
		kGrenadeThrow = 27,
		kBashed = 28,
		kCount = 29
	}

	private const float closeThresholdSqr = 0.0001f;

	private const float shuffleDistanceSqr = 2.25f;

	private const float kDefaultBlendTime = 0.25f;

	private const float kMinTurnToTurnTime = 0.4f;

	private const float kMinAboutFaceToTurnTime = 0.6f;

	private const float kCasualStopDistance = 0.5f;

	private const float kLeftLegForwardTime = 4f / 15f;

	private const float kRightLegForwardTime = 19f / 30f;

	private const float kGrenadeThrowAnimDelay = 0.2f;

	private const float kGrenadeThrowCosine = 0.17f;

	private const int kSaunterTypes = 4;

	public float mMaxWalkAnimSpeed;

	public float mLogicalSpeed;

	public float mLastLogicalSpeed;

	public string debuglog;

	public Vector3 mdebugforward;

	public float mWalkAnimTimer;

	public float mWalkAnimLength;

	public float mDebugAnimStartTime;

	public float mTestWalkWeight;

	public float mShuffleWindowStart;

	public float mShuffleWindowEnd;

	protected float forwardBlendRate;

	public Vector3 mLogicalPosition;

	protected Vector3 mLogicalVelocity;

	protected Vector3 mDestination;

	public MoveAimStateEnum mState;

	protected ActionEnum mCurrentWalkAction;

	protected float mWalkAnimSpeed;

	protected float mStartRunningDelay;

	protected bool mPointing;

	protected float mIdleTimer;

	protected bool mShuffle;

	protected float mCantTurnTimer;

	protected Vector3 mDesiredForward;

	protected WorldHelper.Quadrant mDesiredQuadrant;

	protected PoseModuleSharedData.CommandCode newState;

	protected float mEquippedAimWeight = 1f;

	public float mWalkStrideLength;

	public float mLeanLeftRight;

	private bool leftFootOnFloor;

	private bool rightFootOnFloor;

	private int pointOverride;

	protected int shuffleOverride;

	protected int[] saunterOverride;

	protected int runOverride;

	protected int sprintOverride;

	protected int crouchOverride;

	protected int cautiousOverride;

	protected int stealthOverride;

	protected int crouchShuffleOverride;

	protected int saunterSelection;

	protected float saunterChangeTime;

	protected float lastWalkAnimTime;

	public float mTargetWalkAnimTime;

	public float mLastChangeAnimTime;

	private float mGrenadeThrowTime;

	private bool mGrenadeThrowWasWalking;

	private AimType mAimType;

	private bool aimingForbidden;

	private float prevAimCross = 0.5f;

	private Vector3 prevAimDir = Vector3.zero;

	private bool wasOnTarget;

	private bool wasOnScreen;

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
		Reset(mLogicalPosition, Vector3.forward, 0f);
		base.mAnimDirector.PlayAction(mActionHandle[4]);
		base.mAnimDirector.PlayAction(mActionHandle[16]);
		base.mAnimDirector.SetCategorySpeed(mCategoryHandle[1], 0f);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[1], 0.5f);
		mAimType = AimType.None;
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		mLogicalPosition = base.modeltransposition;
		mLogicalVelocity.Set(0f, 0f, 0f);
		mState = MoveAimStateEnum.kStationary;
		mTargetWalkAnimTime = -1f;
		mCurrentWalkAction = ActionEnum.kFirstMovementAction;
		mWalkAnimSpeed = 1f;
		mStartRunningDelay = 0f;
		mShuffleWindowStart = 0.65f;
		mShuffleWindowEnd = 1f;
		mWalkStrideLength = 42f * mDesc.mStandardWalkSpeed;
		mDesiredForward = base.modeltransforward;
		forwardBlendRate = 1f;
		mCurrentWalkAction = ActionEnum.kCount;
		mMaxWalkAnimSpeed = 0f;
		lastWalkAnimTime = Mathf.Repeat(base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]), 0.8f) / 0.8f;
		leftFootOnFloor = true;
		rightFootOnFloor = true;
		wasOnScreen = myActor.OnScreen;
		base.mAnimDirector.EnableCategoryRetainWeight(mCategoryHandle[1]);
	}

	private void Reset(Vector3 pos, Vector3 aim, float debuganimstarttime)
	{
		mState = MoveAimStateEnum.kStationary;
		mCurrentWalkAction = ActionEnum.kCount;
		mDebugAnimStartTime = debuganimstarttime;
		mPointing = false;
		mShuffle = false;
		mCantTurnTimer = 0f;
	}

	protected void Start_GetActionHandles()
	{
		mCategoryHandle = new int[7];
		mActionHandle = new AnimDirector.ActionHandle[29];
		GetCategoryHandle(CategoryEnum.kMovement, "Movement");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kFirstMovementAction, "Forward");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kRight, "Right");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kBackward, "Backward");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kLeft, "Left");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kStand, "Stand");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kStopLeftLegForward, "StopLeftLegForward");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kStopRightLegForward, "StopRightLegForward");
		GetCategoryHandle(CategoryEnum.kTurning, "Turning");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kFirstTurningAction, "TurnLeft45");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kTurnRight45, "TurnRight45");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kStandToWalkLeft60, "StandToWalkLeft60");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kStandToWalkLeft90, "StandToWalkLeft90");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kStandToWalkLeft130, "StandToWalkLeft130");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kStandToWalkRight60, "StandToWalkRight60");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kStandToWalkRight90, "StandToWalkRight90");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kStandToWalkRight130, "StandToWalkRight130");
		GetActionHandle(CategoryEnum.kTurning, ActionEnum.kStandToWalkBack, "StandToWalkBack");
		GetCategoryHandle(CategoryEnum.kAiming, "Aiming");
		GetActionHandle(CategoryEnum.kAiming, ActionEnum.kFirstAimingAction, "Aim");
		GetCategoryHandle(CategoryEnum.kIdles, "Idles");
		GetActionHandle(CategoryEnum.kIdles, ActionEnum.kFirstIdleAction, "Idle1");
		GetActionHandle(CategoryEnum.kIdles, ActionEnum.kIdle2, "Idle2");
		GetActionHandle(CategoryEnum.kIdles, ActionEnum.kIdle3, "Idle3");
		GetCategoryHandle(CategoryEnum.kGesture, "Gesture");
		GetActionHandle(CategoryEnum.kGesture, ActionEnum.kFirstGestureAction, "GunAway");
		GetActionHandle(CategoryEnum.kGesture, ActionEnum.kPrimOut, "GunOut");
		GetActionHandle(CategoryEnum.kGesture, ActionEnum.kSecAway, "SecAway");
		GetActionHandle(CategoryEnum.kGesture, ActionEnum.kSecOut, "SecOut");
		GetActionHandle(CategoryEnum.kGesture, ActionEnum.kKnifeAway, "KnifeAway");
		GetActionHandle(CategoryEnum.kGesture, ActionEnum.kKnifeOut, "KnifeOut");
		GetCategoryHandle(CategoryEnum.kGrenadeThrow, "Throw");
		GetActionHandle(CategoryEnum.kGrenadeThrow, ActionEnum.kGrenadeThrow, "Grenade");
		GetCategoryHandle(CategoryEnum.kReactions, "Reactions");
		GetActionHandle(CategoryEnum.kReactions, ActionEnum.kBashed, "Bashed");
		pointOverride = base.mAnimDirector.GetOverrideHandle("Point");
		saunterOverride = new int[4];
		saunterSelection = 0;
		saunterChangeTime = 0f;
		shuffleOverride = base.mAnimDirector.GetOverrideHandle("Shuffle");
		saunterOverride[0] = base.mAnimDirector.GetOverrideHandle("Stroll");
		saunterOverride[1] = base.mAnimDirector.GetOverrideHandle("Saunter");
		saunterOverride[2] = base.mAnimDirector.GetOverrideHandle("Sashay");
		saunterOverride[3] = base.mAnimDirector.GetOverrideHandle("Swagger");
		runOverride = base.mAnimDirector.GetOverrideHandle("Run");
		sprintOverride = base.mAnimDirector.GetOverrideHandle("Sprint");
		crouchOverride = base.mAnimDirector.GetOverrideHandle("Crouch");
		cautiousOverride = base.mAnimDirector.GetOverrideHandle("Cautious");
		stealthOverride = base.mAnimDirector.GetOverrideHandle("Stealth");
		crouchShuffleOverride = base.mAnimDirector.GetOverrideHandle("CrouchShuffle");
	}

	private void GetCategoryHandle(CategoryEnum cat, string name)
	{
		mCategoryHandle[(int)cat] = base.mAnimDirector.GetCategoryHandle(name);
	}

	private void GetActionHandle(CategoryEnum cat, ActionEnum act, string name)
	{
		mActionHandle[(int)act] = base.mAnimDirector.GetActionHandle(mCategoryHandle[(int)cat], name);
	}

	private PoseModuleSharedData.CommandCode ApplyAlertness(PoseModuleSharedData.CommandCode state)
	{
		switch (myActor.behaviour.alertState)
		{
		case BehaviourController.AlertState.Casual:
			return ApplyAlertness_Casual(state);
		case BehaviourController.AlertState.Focused:
			return ApplyAlertness_Focused(state);
		case BehaviourController.AlertState.Reacting:
			return ApplyAlertness_Reacting(state);
		case BehaviourController.AlertState.Suspicious:
			return ApplyAlertness_Suspicious(state);
		case BehaviourController.AlertState.Alerted:
			return ApplyAlertness_Alerted(state);
		case BehaviourController.AlertState.Combat:
			return ApplyAlertness_Combat(state);
		default:
			return state;
		}
	}

	private PoseModuleSharedData.CommandCode ApplyAlertness_Casual(PoseModuleSharedData.CommandCode state)
	{
		switch (state)
		{
		case PoseModuleSharedData.CommandCode.Stand:
			return PoseModuleSharedData.CommandCode.AtEase;
		case PoseModuleSharedData.CommandCode.Walk:
			return PoseModuleSharedData.CommandCode.Saunter;
		default:
			return state;
		}
	}

	private PoseModuleSharedData.CommandCode ApplyAlertness_Focused(PoseModuleSharedData.CommandCode state)
	{
		switch (state)
		{
		case PoseModuleSharedData.CommandCode.Stand:
			return PoseModuleSharedData.CommandCode.AtEase;
		case PoseModuleSharedData.CommandCode.Walk:
			return PoseModuleSharedData.CommandCode.Saunter;
		default:
			return state;
		}
	}

	private PoseModuleSharedData.CommandCode ApplyAlertness_Reacting(PoseModuleSharedData.CommandCode state)
	{
		if (state == PoseModuleSharedData.CommandCode.Idle)
		{
			return PoseModuleSharedData.CommandCode.Stand;
		}
		return state;
	}

	private PoseModuleSharedData.CommandCode ApplyAlertness_Suspicious(PoseModuleSharedData.CommandCode state)
	{
		if (state == PoseModuleSharedData.CommandCode.Idle)
		{
			return PoseModuleSharedData.CommandCode.Stand;
		}
		return state;
	}

	private PoseModuleSharedData.CommandCode ApplyAlertness_Alerted(PoseModuleSharedData.CommandCode state)
	{
		if (state == PoseModuleSharedData.CommandCode.Idle)
		{
			return PoseModuleSharedData.CommandCode.Stand;
		}
		return state;
	}

	private PoseModuleSharedData.CommandCode ApplyAlertness_Combat(PoseModuleSharedData.CommandCode state)
	{
		if (state == PoseModuleSharedData.CommandCode.Idle)
		{
			return PoseModuleSharedData.CommandCode.Stand;
		}
		return state;
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		if (myActor.OnScreen || expensiveTick)
		{
			base.modeltransposition = newPos;
		}
		leftFootOnFloor = !myActor.OnScreen;
		rightFootOnFloor = leftFootOnFloor;
		newState = ParseCommand(newStateStr);
		newState = ApplyAlertness(newState);
		if (newState == PoseModuleSharedData.CommandCode.Puppet)
		{
			mState = MoveAimStateEnum.kPuppet;
			base.mAimWeight = 0f;
			base.mAnimDirector.SetCategorySpeed(mCategoryHandle[1], 0f);
			base.mAnimDirector.SetCategoryWeight(mCategoryHandle[1], base.mAimWeight);
			return PoseModuleSharedData.Modules.MoveAim;
		}
		newVel /= 60f;
		mLogicalPosition = newPos;
		newVel.y = 0f;
		if (mState != MoveAimStateEnum.kPuppet)
		{
			mLogicalVelocity = newVel;
			mLogicalSpeed = newVel.magnitude;
			myActor.realCharacter.lastVelocity = mLogicalVelocity * 60f;
		}
		mDestination = destination;
		if (newState != PoseModuleSharedData.CommandCode.ThrowGrenade)
		{
			if (Vector3.Dot(base.mAimDir, aimDir) < 0f)
			{
				if (base.mAimDir.z * aimDir.x - base.mAimDir.x * aimDir.z > 0f)
				{
					aimDir.Set(base.mAimDir.z, aimDir.y, 0f - base.mAimDir.x);
				}
				else
				{
					aimDir.Set(0f - base.mAimDir.z, aimDir.y, base.mAimDir.x);
				}
			}
			if (base.mTargetAimWeight > 0f)
			{
				base.mAimDir = WorldHelper.ExpBlend(base.mAimDir, aimDir, 0.2f);
			}
			base.mAimDir = aimDir;
			base.mAimDir = base.mAimDir.normalized;
			if (mCantTurnTimer > 0f)
			{
				mCantTurnTimer -= Time.deltaTime;
				if (mCantTurnTimer <= 0f)
				{
					mCantTurnTimer = 0f;
					expensiveTick = true;
				}
			}
		}
		else
		{
			base.mAimDir = aimDir;
		}
		if (expensiveTick)
		{
			switch (mState)
			{
			case MoveAimStateEnum.kStationary:
			case MoveAimStateEnum.kPuppet:
			case MoveAimStateEnum.kThrowingGrenade:
				switch (newState)
				{
				case PoseModuleSharedData.CommandCode.Stand:
				case PoseModuleSharedData.CommandCode.Crouch:
				case PoseModuleSharedData.CommandCode.AtEase:
				case PoseModuleSharedData.CommandCode.StealthStand:
					Update_Stationary_Stand();
					break;
				case PoseModuleSharedData.CommandCode.Idle:
					Update_Stationary_Idle();
					break;
				case PoseModuleSharedData.CommandCode.Crawl:
				case PoseModuleSharedData.CommandCode.Walk:
				case PoseModuleSharedData.CommandCode.Stealth:
					StationaryToWalking();
					break;
				case PoseModuleSharedData.CommandCode.Saunter:
					StationaryToSauntering();
					break;
				case PoseModuleSharedData.CommandCode.Run:
					StationaryToRunning();
					break;
				case PoseModuleSharedData.CommandCode.ThrowGrenade:
					Update_ThrowGrenade();
					break;
				}
				break;
			case MoveAimStateEnum.kWalking:
			case MoveAimStateEnum.kStealthing:
				switch (newState)
				{
				case PoseModuleSharedData.CommandCode.Idle:
				case PoseModuleSharedData.CommandCode.Stand:
				case PoseModuleSharedData.CommandCode.Crouch:
				case PoseModuleSharedData.CommandCode.AtEase:
				case PoseModuleSharedData.CommandCode.StealthStand:
					WalkingToStationary();
					break;
				case PoseModuleSharedData.CommandCode.Crawl:
				case PoseModuleSharedData.CommandCode.Walk:
				case PoseModuleSharedData.CommandCode.Stealth:
					Update_Walking();
					break;
				case PoseModuleSharedData.CommandCode.Saunter:
					Update_Sauntering();
					break;
				case PoseModuleSharedData.CommandCode.Run:
					WalkingToRunning();
					break;
				case PoseModuleSharedData.CommandCode.ThrowGrenade:
					Update_ThrowGrenade();
					break;
				}
				break;
			case MoveAimStateEnum.kSauntering:
				switch (newState)
				{
				case PoseModuleSharedData.CommandCode.Idle:
				case PoseModuleSharedData.CommandCode.Stand:
				case PoseModuleSharedData.CommandCode.Crouch:
				case PoseModuleSharedData.CommandCode.AtEase:
				case PoseModuleSharedData.CommandCode.StealthStand:
					SaunteringToStationary();
					break;
				case PoseModuleSharedData.CommandCode.Crawl:
				case PoseModuleSharedData.CommandCode.Walk:
				case PoseModuleSharedData.CommandCode.Stealth:
					Update_Walking();
					break;
				case PoseModuleSharedData.CommandCode.Saunter:
					Update_Sauntering();
					break;
				case PoseModuleSharedData.CommandCode.Run:
					WalkingToRunning();
					break;
				case PoseModuleSharedData.CommandCode.ThrowGrenade:
					Update_ThrowGrenade();
					break;
				}
				break;
			case MoveAimStateEnum.kRunning:
				switch (newState)
				{
				case PoseModuleSharedData.CommandCode.Idle:
				case PoseModuleSharedData.CommandCode.Stand:
				case PoseModuleSharedData.CommandCode.Crouch:
				case PoseModuleSharedData.CommandCode.AtEase:
				case PoseModuleSharedData.CommandCode.StealthStand:
					RunningToStationary();
					break;
				case PoseModuleSharedData.CommandCode.Crawl:
				case PoseModuleSharedData.CommandCode.Walk:
				case PoseModuleSharedData.CommandCode.Stealth:
					RunningToWalking();
					break;
				case PoseModuleSharedData.CommandCode.Saunter:
					RunningToSauntering();
					break;
				case PoseModuleSharedData.CommandCode.Run:
					Update_Running();
					break;
				case PoseModuleSharedData.CommandCode.ThrowGrenade:
					Update_ThrowGrenade();
					break;
				}
				break;
			}
		}
		if (forwardBlendRate > 0f)
		{
			Vector3 vector = mDesiredForward;
			if (Vector3.Dot(base.modeltransforward, vector) < 0f)
			{
				if (base.modeltransforward.z * vector.x - base.modeltransforward.x * vector.z > 0f)
				{
					vector.Set(base.modeltransforward.z, 0f, 0f - base.modeltransforward.x);
				}
				else
				{
					vector.Set(0f - base.modeltransforward.z, 0f, base.modeltransforward.x);
				}
			}
			base.modeltransforward = WorldHelper.ExpBlend(base.modeltransforward, vector, forwardBlendRate);
		}
		if (aimingForbidden || newState == PoseModuleSharedData.CommandCode.Stealth || newState == PoseModuleSharedData.CommandCode.StealthStand)
		{
			base.mTargetAimWeight = 0f;
			aimingForbidden = false;
		}
		base.mAimWeight = WorldHelper.ExpBlend(base.mAimWeight, base.mTargetAimWeight, 0.2f);
		DoAiming();
		base.mAnimDirector.EnableOverride(cautiousOverride, myActor.behaviour.alertState == BehaviourController.AlertState.Alerted || myActor.behaviour.alertState == BehaviourController.AlertState.Suspicious);
		base.mAnimDirector.SetCategorySpeed(mCategoryHandle[1], 0f);
		base.mAnimDirector.SetCategoryWeight(mCategoryHandle[1], base.mAimWeight);
		myActor.weapon.SetAiming(base.mAimWeight > 0.95f);
		if (mWalkAnimSpeed > mMaxWalkAnimSpeed)
		{
			mMaxWalkAnimSpeed = mWalkAnimSpeed;
		}
		lastWalkAnimTime = Mathf.Repeat(base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]), 0.8f) / 0.8f;
		return PoseModuleSharedData.Modules.MoveAim;
	}

	public override void OnInactive(PoseModuleSharedData.Modules to)
	{
		switch (to)
		{
		case PoseModuleSharedData.Modules.CrouchCover:
		case PoseModuleSharedData.Modules.HighCornerCover:
		case PoseModuleSharedData.Modules.Carrying:
		case PoseModuleSharedData.Modules.Carried:
		case PoseModuleSharedData.Modules.Corpse:
		case PoseModuleSharedData.Modules.FixedGun:
			EnableSauntering(false);
			StopPointing();
			break;
		case PoseModuleSharedData.Modules.AGR:
			break;
		}
	}

	protected void EnableSauntering(bool val)
	{
		float animTime = 0f;
		if (val)
		{
			animTime = base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]);
		}
		for (int i = 0; i < 4; i++)
		{
			base.mAnimDirector.EnableOverride(saunterOverride[i], val && i == saunterSelection, 1f);
		}
		if (val)
		{
			base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], animTime);
		}
		else
		{
			myActor.Pose.CancelSegue();
		}
	}

	protected virtual void Update_Stationary_Stand()
	{
		leftFootOnFloor = true;
		rightFootOnFloor = true;
		mShuffle = false;
		base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0.25f);
		EnableSauntering(newState == PoseModuleSharedData.CommandCode.AtEase);
		base.mAnimDirector.EnableOverride(crouchOverride, newState == PoseModuleSharedData.CommandCode.Crouch || newState == PoseModuleSharedData.CommandCode.StealthStand);
		base.mAnimDirector.EnableOverride(stealthOverride, newState == PoseModuleSharedData.CommandCode.StealthStand);
		base.mTargetAimWeight = mEquippedAimWeight;
		if (mPointing)
		{
			TurnToPoint_Lower();
			Point_Upper();
		}
		else
		{
			TurnToAim_Lower();
			Aim_Upper();
		}
	}

	protected virtual void Update_Stationary_Idle()
	{
		leftFootOnFloor = true;
		rightFootOnFloor = true;
		base.mAnimDirector.EnableOverride(stealthOverride, false);
		mShuffle = false;
		base.mTargetAimWeight = 0f;
		Vector3 fr = base.modeltransforward;
		fr.y = 0f;
		fr.Normalize();
		EnableSauntering(myActor.behaviour.alertState < BehaviourController.AlertState.Suspicious);
		switch (WorldHelper.GetQuadrant(fr, base.mAimDir, (!myActor.Pose.faceLookDirection) ? 1f : 0.1f))
		{
		case WorldHelper.Quadrant.kFront:
			mDesiredForward = fr;
			if (mIdleTimer <= 0f)
			{
				base.mAnimDirector.EnableCategory(mCategoryHandle[2], true, 0.25f);
				mIdleTimer = UnityEngine.Random.Range(10f, 15f);
				PlayAction((ActionEnum)UnityEngine.Random.Range(17, 20), 1f, 1f);
			}
			break;
		case WorldHelper.Quadrant.kBack:
		case WorldHelper.Quadrant.kLeft:
			mIdleTimer = UnityEngine.Random.Range(10f, 15f);
			mDesiredForward = base.mAimDir;
			base.mAnimDirector.EnableCategory(mCategoryHandle[3], true, 0.25f);
			if (mCantTurnTimer == 0f)
			{
				mCantTurnTimer = 0.25f;
				PlayActionIfNone(ActionEnum.kFirstTurningAction);
				saunterSelection = 0;
			}
			break;
		case WorldHelper.Quadrant.kRight:
			mIdleTimer = UnityEngine.Random.Range(10f, 15f);
			mDesiredForward = base.mAimDir;
			base.mAnimDirector.EnableCategory(mCategoryHandle[3], true, 0.25f);
			if (mCantTurnTimer == 0f)
			{
				mCantTurnTimer = 0.25f;
				PlayActionIfNone(ActionEnum.kTurnRight45);
				saunterSelection = 0;
			}
			break;
		}
		mIdleTimer -= Time.deltaTime;
		mDesiredForward.y = 0f;
		mDesiredForward.Normalize();
		forwardBlendRate = 0.1f;
	}

	protected void TurnToAim_Lower()
	{
		Vector3 fr = base.modeltransforward;
		fr.y = 0f;
		fr.Normalize();
		mDesiredQuadrant = WorldHelper.GetQuadrant(fr, base.mAimDir, 1f);
		switch (mDesiredQuadrant)
		{
		case WorldHelper.Quadrant.kFront:
			mDesiredForward = fr;
			break;
		case WorldHelper.Quadrant.kBack:
			mDesiredForward = base.mAimDir;
			break;
		case WorldHelper.Quadrant.kLeft:
			if (WorldHelper.InFront(fr, base.mAimDir))
			{
				mDesiredForward.x = fr.x - fr.z;
				mDesiredForward.z = fr.x + fr.z;
			}
			else
			{
				mDesiredForward = base.mAimDir;
				mDesiredForward.y = 0f;
			}
			base.mAnimDirector.EnableCategory(mCategoryHandle[3], true, 0.25f);
			if (mCantTurnTimer == 0f)
			{
				mCantTurnTimer = 0.25f;
				PlayActionIfNone(ActionEnum.kFirstTurningAction);
			}
			break;
		case WorldHelper.Quadrant.kRight:
			if (WorldHelper.InFront(fr, base.mAimDir))
			{
				mDesiredForward.x = fr.x + fr.z;
				mDesiredForward.z = fr.z - fr.x;
			}
			else
			{
				mDesiredForward = base.mAimDir;
			}
			base.mAnimDirector.EnableCategory(mCategoryHandle[3], true, 0.25f);
			if (mCantTurnTimer == 0f)
			{
				mCantTurnTimer = 0.25f;
				PlayActionIfNone(ActionEnum.kTurnRight45);
			}
			break;
		}
		mDesiredForward.Normalize();
		forwardBlendRate = 0.1f;
	}

	protected void TurnToPoint_Lower()
	{
		Vector3 vector = base.modeltransforward;
		vector.y = 0f;
		vector.Normalize();
		if (Vector3.Dot(vector, base.mAimDir) > 0f)
		{
			mDesiredForward = vector;
		}
		else
		{
			mDesiredQuadrant = WorldHelper.GetQuadrant(vector, base.mAimDir, 1f);
			switch (mDesiredQuadrant)
			{
			case WorldHelper.Quadrant.kBack:
				mDesiredForward = vector * -1f;
				break;
			case WorldHelper.Quadrant.kLeft:
				mDesiredForward.x = vector.x - vector.z;
				mDesiredForward.z = vector.x + vector.z;
				base.mAnimDirector.EnableCategory(mCategoryHandle[3], true, 0.25f);
				if (mCantTurnTimer == 0f)
				{
					mCantTurnTimer = 0.25f;
					PlayActionIfNone(ActionEnum.kFirstTurningAction);
					saunterSelection = 0;
				}
				break;
			case WorldHelper.Quadrant.kRight:
				mDesiredForward.x = vector.x + vector.z;
				mDesiredForward.z = vector.z - vector.x;
				base.mAnimDirector.EnableCategory(mCategoryHandle[3], true, 0.25f);
				if (mCantTurnTimer == 0f)
				{
					mCantTurnTimer = 0.25f;
					PlayActionIfNone(ActionEnum.kTurnRight45);
					saunterSelection = 0;
				}
				break;
			default:
				mDesiredForward = vector;
				break;
			}
		}
		mDesiredForward.Normalize();
		forwardBlendRate = 0.1f;
	}

	protected virtual void Update_Sauntering()
	{
		base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0.25f);
		base.mAnimDirector.EnableCategory(mCategoryHandle[3], true, 0.25f);
		EnableRunning(false, 0.25f);
		if (WorldHelper.ThisFrameTime > saunterChangeTime)
		{
			saunterChangeTime = WorldHelper.ThisFrameTime + UnityEngine.Random.Range(5f, 15f);
			saunterSelection += UnityEngine.Random.Range(1, 3);
			if (saunterSelection >= 4)
			{
				saunterSelection -= 4;
			}
		}
		if (myActor.behaviour.alertState >= BehaviourController.AlertState.Focused)
		{
			saunterSelection = 0;
		}
		EnableSauntering(true);
		base.mAnimDirector.EnableOverride(crouchOverride, false);
		base.mAnimDirector.EnableOverride(crouchShuffleOverride, false);
		base.mAnimDirector.EnableOverride(stealthOverride, false);
		float mStandardSaunterSpeed = mDesc.mStandardSaunterSpeed;
		Vector3 vector = base.modeltransforward;
		vector.y = 0f;
		vector.Normalize();
		mDesiredForward = mLogicalVelocity;
		mDesiredForward.y = 0f;
		mDesiredForward.Normalize();
		mDesiredForward = mDesiredForward * 3f - vector * 2f;
		mDesiredForward.Normalize();
		mLastLogicalSpeed = mLogicalSpeed;
		forwardBlendRate = mDesc.mWalkingDirectionBlend.Get(mLogicalSpeed);
		if (mStandardSaunterSpeed > 0f)
		{
			mWalkAnimSpeed = mLogicalSpeed / mStandardSaunterSpeed;
		}
		base.mAnimDirector.SetCategorySpeed(mCategoryHandle[0], mWalkAnimSpeed);
		mWalkAnimLength = base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]);
		mWalkAnimTimer = Mathf.Repeat(base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]), mWalkAnimLength);
		mdebugforward = mDesiredForward;
		Point_Upper();
		if (!myActor.Pose.Segueing())
		{
			base.mTargetAimWeight = 1f;
		}
	}

	protected virtual void Update_Walking()
	{
		ApplyFeetOnFloor();
		base.mTargetAimWeight = mEquippedAimWeight;
		base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0.25f);
		base.mAnimDirector.EnableCategory(mCategoryHandle[3], false, 0.25f);
		EnableRunning(false, 0.25f);
		EnableSauntering(false);
		float categoryTime = base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]);
		base.mAnimDirector.EnableOverride(crouchOverride, newState == PoseModuleSharedData.CommandCode.Crawl || newState == PoseModuleSharedData.CommandCode.Stealth);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], categoryTime);
		base.mAnimDirector.EnableOverride(stealthOverride, newState == PoseModuleSharedData.CommandCode.Stealth);
		base.mAnimDirector.EnableCategory(mCategoryHandle[3], false, 0.1f);
		if (newState == PoseModuleSharedData.CommandCode.Stealth)
		{
			mState = MoveAimStateEnum.kStealthing;
		}
		else
		{
			mState = MoveAimStateEnum.kWalking;
		}
		if (mPointing || newState == PoseModuleSharedData.CommandCode.Stealth)
		{
			Update_Walking_Point();
			Point_Upper();
		}
		else
		{
			Update_Walking_Aim();
			Aim_Upper();
		}
	}

	protected void Update_Walking_Aim()
	{
		float num = mDesc.mStandardWalkSpeed;
		bool flag = mCantTurnTimer == 0f && (leftFootOnFloor || rightFootOnFloor);
		if (flag)
		{
			mDesiredQuadrant = WorldHelper.GetQuadrant(mLogicalVelocity, base.mAimDir, 1f);
		}
		if (mLogicalSpeed > mDesc.mStandardWalkSpeed * 1.5f)
		{
			mDesiredQuadrant = WorldHelper.Quadrant.kFront;
		}
		Vector3 vector = base.modeltransforward;
		vector.y = 0f;
		vector.Normalize();
		switch (mDesiredQuadrant)
		{
		case WorldHelper.Quadrant.kFront:
			mDesiredForward = mLogicalVelocity;
			PlayWalkAction(ActionEnum.kFirstMovementAction, ActionEnum.kCount);
			if (mShuffleWindowStart < mWalkAnimTimer && mWalkAnimTimer < mShuffleWindowEnd)
			{
				float sqrMagnitude = (mDestination - mLogicalPosition).sqrMagnitude;
				mShuffle = sqrMagnitude < mWalkStrideLength * mWalkStrideLength;
			}
			break;
		case WorldHelper.Quadrant.kBack:
			if (WorldHelper.GetQuadrant(vector, base.mAimDir, 1f) == WorldHelper.Quadrant.kBack)
			{
				mCantTurnTimer = 0.6f;
				WorldHelper.Quadrant quadrant = WorldHelper.Quadrant.kRight;
				if (Vector3.Dot(mLogicalVelocity, base.mAimDir) < 0f)
				{
					quadrant = WorldHelper.Quadrant.kLeft;
				}
				if (WorldHelper.LeftOrRight(vector, base.mAimDir) == quadrant)
				{
					mDesiredForward.x = 0f - mLogicalVelocity.z;
					mDesiredForward.z = mLogicalVelocity.x;
					PlayWalkAction(ActionEnum.kRight, ActionEnum.kCount);
					mDesiredQuadrant = WorldHelper.Quadrant.kLeft;
				}
				else
				{
					mDesiredForward.x = mLogicalVelocity.z;
					mDesiredForward.z = 0f - mLogicalVelocity.x;
					PlayWalkAction(ActionEnum.kLeft, ActionEnum.kCount);
					mDesiredQuadrant = WorldHelper.Quadrant.kRight;
				}
			}
			else
			{
				mDesiredForward = mLogicalVelocity * -1f;
				PlayWalkAction(ActionEnum.kBackward, ActionEnum.kCount);
			}
			mShuffle = false;
			break;
		case WorldHelper.Quadrant.kLeft:
			if (flag)
			{
				mCantTurnTimer = 0.4f;
			}
			mDesiredForward.x = 0f - mLogicalVelocity.z;
			mDesiredForward.z = mLogicalVelocity.x;
			PlayWalkAction(ActionEnum.kRight, ActionEnum.kCount);
			mShuffle = false;
			break;
		case WorldHelper.Quadrant.kRight:
			if (flag)
			{
				mCantTurnTimer = 0.4f;
			}
			mDesiredForward.x = mLogicalVelocity.z;
			mDesiredForward.z = 0f - mLogicalVelocity.x;
			PlayWalkAction(ActionEnum.kLeft, ActionEnum.kCount);
			mShuffle = false;
			break;
		}
		mDesiredForward.y = 0f;
		mDesiredForward.Normalize();
		mDesiredForward = mDesiredForward * 3f - vector * 2f;
		mDesiredForward.Normalize();
		if (shuffleOverride >= 0)
		{
			if (mShuffle)
			{
				num = mDesc.mStandardShuffleSpeed;
			}
			bool flag2 = newState == PoseModuleSharedData.CommandCode.Crawl || newState == PoseModuleSharedData.CommandCode.Stealth;
			base.mAnimDirector.EnableOverride(shuffleOverride, mShuffle && !flag2);
			base.mAnimDirector.EnableOverride(crouchShuffleOverride, mShuffle && flag2);
		}
		mLastLogicalSpeed = mLogicalSpeed;
		forwardBlendRate = mDesc.mWalkingDirectionBlend.Get(mLogicalSpeed);
		if (num > 0f)
		{
			mWalkAnimSpeed = mLogicalSpeed / num;
		}
		base.mAnimDirector.SetCategorySpeed(mCategoryHandle[0], mWalkAnimSpeed);
		mWalkAnimLength = base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]);
		mWalkAnimTimer = Mathf.Repeat(base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]), mWalkAnimLength);
		mdebugforward = mDesiredForward;
	}

	protected void Update_Walking_Point()
	{
		float num = mDesc.mStandardWalkSpeed;
		if (Vector3.Dot(mLogicalVelocity, base.mAimDir) > 0f)
		{
			mDesiredForward = mLogicalVelocity;
			PlayWalkAction(ActionEnum.kFirstMovementAction, ActionEnum.kCount);
			if (mShuffleWindowStart < mWalkAnimTimer && mWalkAnimTimer < mShuffleWindowEnd)
			{
				float sqrMagnitude = (mDestination - mLogicalPosition).sqrMagnitude;
				mShuffle = sqrMagnitude < mWalkStrideLength * mWalkStrideLength;
			}
		}
		else
		{
			mShuffle = false;
			if (mLogicalVelocity.x * base.mAimDir.z - mLogicalVelocity.z * base.mAimDir.x > 0f)
			{
				mDesiredForward.x = 0f - mLogicalVelocity.z;
				mDesiredForward.z = mLogicalVelocity.x;
				PlayWalkAction(ActionEnum.kRight, ActionEnum.kCount);
			}
			else
			{
				mDesiredForward.x = mLogicalVelocity.z;
				mDesiredForward.z = 0f - mLogicalVelocity.x;
				PlayWalkAction(ActionEnum.kLeft, ActionEnum.kCount);
			}
		}
		mDesiredForward.y = 0f;
		mDesiredForward.Normalize();
		if (shuffleOverride >= 0)
		{
			if (mShuffle)
			{
				num = mDesc.mStandardShuffleSpeed;
			}
			bool flag = newState == PoseModuleSharedData.CommandCode.Crawl || newState == PoseModuleSharedData.CommandCode.Stealth;
			base.mAnimDirector.EnableOverride(shuffleOverride, mShuffle && !flag);
			base.mAnimDirector.EnableOverride(crouchShuffleOverride, mShuffle && flag);
		}
		mLastLogicalSpeed = mLogicalSpeed;
		Vector3 vector = base.modeltransforward;
		vector.y = 0f;
		vector.Normalize();
		mDesiredForward = mDesiredForward * 3f - vector * 2f;
		mDesiredForward.Normalize();
		forwardBlendRate = mDesc.mWalkingDirectionBlend.Get(mLogicalSpeed);
		if (mDesc.mStandardWalkSpeed > 0f)
		{
			mWalkAnimSpeed = mLogicalSpeed / num;
		}
		base.mAnimDirector.SetCategorySpeed(mCategoryHandle[0], mWalkAnimSpeed);
		mWalkAnimLength = base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]);
		mWalkAnimTimer = Mathf.Repeat(base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]), mWalkAnimLength);
		mdebugforward = mDesiredForward;
	}

	protected void Update_Running()
	{
		ApplyFeetOnFloor();
		mShuffle = false;
		base.mAnimDirector.EnableOverride(stealthOverride, false);
		EnableRunning(true, 0.3f);
		EnableSauntering(false);
		base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0.25f);
		base.mAnimDirector.EnableCategory(mCategoryHandle[3], false, 0.1f);
		base.mTargetAimWeight = 0f;
		mDesiredForward = mLogicalVelocity;
		mDesiredForward.Normalize();
		forwardBlendRate = 0.2f;
		if (mDesc.mStandardRunSpeed > 0f)
		{
			mWalkAnimSpeed = mLogicalSpeed / mDesc.mStandardRunSpeed;
		}
		base.mAnimDirector.SetCategorySpeed(mCategoryHandle[0], mWalkAnimSpeed);
	}

	private void ApplyFeetOnFloor()
	{
		float num = Mathf.Repeat(base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]), 0.8f) / 0.8f;
		if (lastWalkAnimTime > num)
		{
			num += 1f;
		}
		if (lastWalkAnimTime < 0.07f)
		{
			leftFootOnFloor = true;
		}
		if (lastWalkAnimTime < 0.55f && num > 0.4f)
		{
			rightFootOnFloor = true;
		}
		if (num > 0.9f)
		{
			leftFootOnFloor = true;
		}
		Vector3 lhs = myActor.navAgent.steeringTarget - myActor.GetPosition();
		float num2 = (lhs.x * lhs.x + lhs.z * lhs.z) / 10f;
		if (num2 < 1f)
		{
			num2 = 1f;
		}
		float num3 = (mDesiredForward.x * lhs.z - mDesiredForward.z * lhs.x) / num2;
		mLeanLeftRight = WorldHelper.ExpBlend(mLeanLeftRight, Mathf.Clamp01(0.5f - num3), 0.1f);
		base.mAnimDirector.SetCategoryBlendTreeBlend(mCategoryHandle[0], mLeanLeftRight);
		if (myActor.navAgent.hasPath && (leftFootOnFloor || rightFootOnFloor))
		{
			float num4 = Vector3.Dot(lhs, mDesiredForward);
			float frac = 0.05f;
			if (num3 > 0f && rightFootOnFloor)
			{
				frac = 0.2f;
			}
			else if (num3 < 0f && leftFootOnFloor)
			{
				frac = 0.2f;
			}
			if (num4 < 0f)
			{
				frac = 0.3f;
			}
			if (Time.fixedDeltaTime > 0.03f && Time.deltaTime > 0.005f)
			{
				myActor.navAgent.velocity = WorldHelper.ExpBlend(myActor.navAgent.velocity, myActor.navAgent.desiredVelocity, frac);
			}
		}
	}

	protected void Update_ThrowGrenade()
	{
		Vector3 lhs = base.modeltransforward;
		lhs.y = 0f;
		lhs.Normalize();
		float num = Vector3.Dot(lhs, base.mAimDir);
		bool flag = num >= 0.17f;
		if (mState != MoveAimStateEnum.kThrowingGrenade)
		{
			mState = MoveAimStateEnum.kThrowingGrenade;
			mGrenadeThrowTime = -1f;
			mGrenadeThrowWasWalking = false;
		}
		if (mLogicalSpeed > 0f)
		{
			Update_Walking();
			mGrenadeThrowWasWalking = true;
		}
		else if (mGrenadeThrowWasWalking)
		{
			WalkingToStationary();
			mGrenadeThrowWasWalking = false;
		}
		else if (!flag)
		{
			Update_Stationary_Stand();
		}
		else if (mGrenadeThrowTime == -1f)
		{
			base.mAnimDirector.PlayAction(mActionHandle[27], true);
			mGrenadeThrowTime = 0f;
		}
		else if (mGrenadeThrowTime >= 0.2f)
		{
			myActor.tasks.Command("ThrowGrenade");
			mState = MoveAimStateEnum.kStationary;
			mGrenadeThrowTime = -1f;
		}
		else
		{
			mGrenadeThrowTime += Time.deltaTime;
		}
	}

	private void EnableRunning(bool val, float dur)
	{
		bool flag = false;
		bool onOff = false;
		if (val)
		{
			flag = myActor.baseCharacter.MovementStyleRequested == BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
			onOff = !flag;
		}
		base.mAnimDirector.EnableOverride(runOverride, flag, dur);
		base.mAnimDirector.EnableOverride(sprintOverride, onOff, dur);
	}

	protected void StationaryToRunning()
	{
		leftFootOnFloor = true;
		rightFootOnFloor = true;
		base.modeltransforward = mLogicalVelocity;
		PlayWalkAction(ActionEnum.kFirstMovementAction, ActionEnum.kCount);
		mState = MoveAimStateEnum.kRunning;
		Update_Running();
	}

	protected void StationaryToSauntering()
	{
		leftFootOnFloor = true;
		rightFootOnFloor = true;
		mCantTurnTimer = 0f;
		mDebugAnimStartTime = 0f;
		saunterChangeTime = Time.time + UnityEngine.Random.Range(2f, 10f);
		float sector = WorldHelper.GetSector(base.modeltransforward, mLogicalVelocity);
		EnableSauntering(true);
		if (myActor.Pose.blend != 0f)
		{
			StartSauntering();
			return;
		}
		if (sector <= -150f || sector >= 150f)
		{
			Segue(ActionEnum.kStandToWalkBack, ActionEnum.kFirstMovementAction, 0f);
		}
		else if (sector <= -115f)
		{
			Segue(ActionEnum.kStandToWalkLeft130, ActionEnum.kFirstMovementAction, 0f);
		}
		else if (sector <= -75f)
		{
			Segue(ActionEnum.kStandToWalkLeft90, ActionEnum.kFirstMovementAction, 0f);
		}
		else if (sector <= -45f)
		{
			Segue(ActionEnum.kStandToWalkLeft60, ActionEnum.kFirstMovementAction, 0f);
		}
		else
		{
			if (sector <= 45f)
			{
				StartSauntering();
				return;
			}
			if (sector <= 75f)
			{
				Segue(ActionEnum.kStandToWalkRight60, ActionEnum.kFirstMovementAction, 0f);
			}
			else if (sector <= 115f)
			{
				Segue(ActionEnum.kStandToWalkRight90, ActionEnum.kFirstMovementAction, 0f);
			}
			else
			{
				Segue(ActionEnum.kStandToWalkRight130, ActionEnum.kFirstMovementAction, 0f);
			}
		}
		base.mTargetAimWeight = 0f;
		mState = MoveAimStateEnum.kSauntering;
		Update_Sauntering();
	}

	protected void StartSauntering()
	{
		PlayWalkAction(ActionEnum.kFirstMovementAction, ActionEnum.kCount);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], mDebugAnimStartTime);
		mState = MoveAimStateEnum.kSauntering;
		Update_Sauntering();
	}

	protected void StationaryToWalking()
	{
		leftFootOnFloor = true;
		rightFootOnFloor = true;
		mCantTurnTimer = 0f;
		mDebugAnimStartTime = 0f;
		WorldHelper.Quadrant quadrant = WorldHelper.GetQuadrant(mLogicalVelocity, base.mAimDir, 1f, 1f);
		if (quadrant == WorldHelper.Quadrant.kFront && (myActor.weapon == null || myActor.weapon.GetTarget() == null))
		{
			quadrant = WorldHelper.GetQuadrant(mLogicalVelocity, base.modeltransforward, 2f, 1f);
		}
		switch (quadrant)
		{
		case WorldHelper.Quadrant.kFront:
			StartWalking();
			break;
		case WorldHelper.Quadrant.kBack:
			StartWalking_AboutFace();
			break;
		case WorldHelper.Quadrant.kRight:
			StartWalking_ToLeft();
			break;
		default:
			StartWalking_ToRight();
			break;
		}
		mState = MoveAimStateEnum.kWalking;
	}

	protected void StartWalking()
	{
		PlayWalkAction(ActionEnum.kFirstMovementAction, ActionEnum.kCount);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], mDebugAnimStartTime);
		mState = MoveAimStateEnum.kWalking;
		mCantTurnTimer = 0.5f;
		mDesiredQuadrant = WorldHelper.Quadrant.kFront;
		Update_Walking();
	}

	protected void StartWalking_AboutFace()
	{
		PlayWalkAction(ActionEnum.kBackward, ActionEnum.kCount);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], mDebugAnimStartTime);
		mState = MoveAimStateEnum.kWalking;
		mCantTurnTimer = 0.5f;
		mDesiredQuadrant = WorldHelper.Quadrant.kBack;
		Update_Walking();
	}

	protected void StartWalking_ToLeft()
	{
		PlayWalkAction(ActionEnum.kLeft, ActionEnum.kCount);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], mDebugAnimStartTime);
		mState = MoveAimStateEnum.kWalking;
		mDesiredQuadrant = WorldHelper.Quadrant.kRight;
		mCantTurnTimer = 0.5f;
		Update_Walking();
	}

	protected void StartWalking_ToRight()
	{
		PlayWalkAction(ActionEnum.kRight, ActionEnum.kCount);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], mDebugAnimStartTime);
		mState = MoveAimStateEnum.kWalking;
		mDesiredQuadrant = WorldHelper.Quadrant.kLeft;
		mCantTurnTimer = 0.5f;
		Update_Walking();
	}

	protected void WalkingToStationary()
	{
		mCantTurnTimer = 0f;
		PlayWalkAction(ActionEnum.kStand, ActionEnum.kCount, 0.2f);
		mState = MoveAimStateEnum.kStationary;
		Update_Stationary_Stand();
	}

	protected void SaunteringToStationary()
	{
		mCantTurnTimer = 0f;
		PlayWalkAction(ActionEnum.kStand, ActionEnum.kCount, 0.2f);
		mState = MoveAimStateEnum.kStationary;
		Update_Stationary_Stand();
	}

	protected void WalkingToRunning()
	{
		mCantTurnTimer = 0f;
		if (mCurrentWalkAction == ActionEnum.kFirstMovementAction)
		{
			if (mStartRunningDelay > Time.time)
			{
				mStartRunningDelay = 0f;
				if (mLogicalSpeed > 0f)
				{
					base.mAimDir = mLogicalVelocity;
					base.mAimDir.Normalize();
				}
				Update_Walking();
			}
			else
			{
				float categoryTime = base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]);
				EnableRunning(true, 0.3f);
				base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], categoryTime + 0.1f);
				mState = MoveAimStateEnum.kRunning;
				Update_Running();
			}
		}
		else
		{
			if (mLogicalSpeed > 0f)
			{
				base.mAimDir = mLogicalVelocity;
				base.mAimDir.Normalize();
			}
			Update_Walking();
			mStartRunningDelay = Time.time + 0.3f;
		}
	}

	protected void RunningToWalking()
	{
		float categoryTime = base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]);
		EnableRunning(false, 0.25f);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], categoryTime);
		mState = MoveAimStateEnum.kWalking;
		Update_Walking();
	}

	protected void RunningToSauntering()
	{
		float categoryTime = base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]);
		EnableRunning(false, 0.25f);
		base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], categoryTime);
		mState = MoveAimStateEnum.kSauntering;
		Update_Sauntering();
	}

	protected void RunningToStationary()
	{
		mCantTurnTimer = 0f;
		PlayWalkAction(ActionEnum.kStand, ActionEnum.kCount, 0.2f);
		mState = MoveAimStateEnum.kStationary;
	}

	protected void Aim_Upper()
	{
		mAimType = AimType.FortyFive;
	}

	protected void Point_Upper()
	{
		mAimType = AimType.Ninety;
	}

	public void DoAiming()
	{
		if (Vector3.Dot(prevAimDir, base.mAimDir) < 0.9f)
		{
			wasOnTarget = false;
		}
		prevAimDir = base.mAimDir;
		if (!myActor.OnScreen)
		{
			return;
		}
		Vector3 forward = myActor.model.transform.forward;
		Vector3 vector = base.mAimDir;
		vector.y = 0f;
		vector.Normalize();
		float num = (Mathf.Asin(Mathf.Clamp(forward.z * vector.x - forward.x * vector.z, -1f, 1f)) - -(float)Math.PI / 4f) / ((float)Math.PI / 2f) * (29f / 30f);
		if (!wasOnScreen)
		{
			prevAimCross = num;
		}
		if (mAimType == AimType.FortyFive)
		{
			if (Vector3.Dot(forward, base.mAimDir) > 0f)
			{
				if ((num >= 0f && num <= 1f) || wasOnTarget)
				{
					WorldHelper.ExpBlend(ref prevAimCross, num, (!wasOnTarget) ? 0.1f : 0.9f);
					if (Mathf.Abs(prevAimCross - num) < 0.05f)
					{
						wasOnTarget = true;
					}
				}
				else
				{
					wasOnTarget = false;
				}
				prevAimCross = Mathf.Clamp(prevAimCross, 0f, 0.99f);
			}
			base.mAnimDirector.SetCategoryTime(mCategoryHandle[1], prevAimCross);
		}
		else
		{
			num = (prevAimCross = Mathf.Clamp(num * 0.5f + 0.25f, 1f / 30f, 0.99f));
			base.mAnimDirector.SetCategoryTime(mCategoryHandle[1], num);
		}
		float sqrMagnitude = base.mAimDir.xz().sqrMagnitude;
		if (sqrMagnitude > 0.01f)
		{
			float btb = Mathf.Clamp01(base.mAimDir.y / sqrMagnitude / 1.3333f + 0.5f);
			base.mAnimDirector.SetCategoryBlendTreeBlend(mCategoryHandle[1], btb);
		}
		else
		{
			base.mAnimDirector.SetCategoryBlendTreeBlend(mCategoryHandle[1], 0.5f);
		}
		wasOnScreen = myActor.OnScreen;
	}

	protected float BlendLooped(float fr, float to, float looplen, float frac)
	{
		if (fr < to)
		{
			if (fr - (to - looplen) < to - fr)
			{
				to -= looplen;
			}
		}
		else if (to + looplen - fr < fr - to)
		{
			to += looplen;
		}
		WorldHelper.ExpBlend(ref fr, to, frac);
		if (fr < 0f)
		{
			fr += looplen;
		}
		else if (fr > looplen)
		{
			fr -= looplen;
		}
		return fr;
	}

	protected bool Close(Vector3 p1, Vector3 p2)
	{
		return Vector3.SqrMagnitude(p1 - p2) < 0.0001f;
	}

	protected void PlayAction(ActionEnum act)
	{
		base.mAnimDirector.PlayAction(mActionHandle[(int)act], -1f);
	}

	protected void PlayAction(ActionEnum act, bool forceRestart)
	{
		base.mAnimDirector.PlayAction(mActionHandle[(int)act], -1f, forceRestart);
	}

	protected void PlayActionIfNone(ActionEnum act)
	{
		if (base.mAnimDirector.HasCurrentActionCompleted(mActionHandle[(int)act].CategoryID))
		{
			base.mAnimDirector.PlayAction(mActionHandle[(int)act], -1f, true);
		}
	}

	protected void PlayAction(ActionEnum act, float xfade)
	{
		base.mAnimDirector.PlayAction(mActionHandle[(int)act], xfade);
	}

	protected void PlayAction(ActionEnum act, float xfade, float fadeOutTime)
	{
		base.mAnimDirector.SetBlendOutTime(base.mAnimDirector.PlayAction(mActionHandle[(int)act], xfade), fadeOutTime);
	}

	protected void Segue(ActionEnum segAction, ActionEnum segTo, float toStartTime)
	{
		myActor.Pose.Segue(mActionHandle[(int)segAction], mActionHandle[(int)segTo], toStartTime, false);
		mCurrentWalkAction = segTo;
	}

	protected void PlayWalkAction(ActionEnum act, ActionEnum chainedfrom)
	{
		if (chainedfrom != ActionEnum.kCount)
		{
			PlayAction(chainedfrom);
			base.mAnimDirector.ChainAction(mActionHandle[(int)chainedfrom].CategoryID, mActionHandle[(int)act], 0f);
			mCurrentWalkAction = act;
			return;
		}
		base.mAnimDirector.CancelChainedActions();
		if (act != mCurrentWalkAction)
		{
			float categoryTime = base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]);
			float categoryLength = base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]);
			categoryTime = (mLastChangeAnimTime = Mathf.Repeat(categoryTime, categoryLength));
			mTargetWalkAnimTime = -1f;
			if (mCurrentWalkAction != ActionEnum.kCount)
			{
				mTargetWalkAnimTime = mDesc.mTweenTimes.fr[(int)mCurrentWalkAction].to[(int)act].Get(categoryTime, categoryLength);
			}
			PlayAction(act, mDesc.mWalkingXFade.Get(mLogicalSpeed));
			if (mTargetWalkAnimTime >= 0f)
			{
				base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], mTargetWalkAnimTime);
			}
			mCurrentWalkAction = act;
		}
	}

	protected void PlayWalkAction(ActionEnum act, ActionEnum chainedfrom, float xfade)
	{
		if (chainedfrom != ActionEnum.kCount)
		{
			PlayAction(chainedfrom);
			base.mAnimDirector.ChainAction(mActionHandle[(int)chainedfrom].CategoryID, mActionHandle[(int)act], xfade);
			mCurrentWalkAction = act;
			return;
		}
		base.mAnimDirector.CancelChainedActions();
		if (act == mCurrentWalkAction)
		{
			return;
		}
		if (myActor.Pose.Segueing())
		{
			myActor.Pose.mSegueToAction = mActionHandle[(int)act];
			mCurrentWalkAction = act;
			return;
		}
		float categoryTime = base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]);
		float categoryLength = base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]);
		categoryTime = (mLastChangeAnimTime = Mathf.Repeat(categoryTime, categoryLength));
		mTargetWalkAnimTime = -1f;
		if (mCurrentWalkAction != ActionEnum.kCount)
		{
			mTargetWalkAnimTime = mDesc.mTweenTimes.fr[(int)mCurrentWalkAction].to[(int)act].Get(categoryTime, categoryLength);
		}
		PlayAction(act, xfade);
		if (mTargetWalkAnimTime >= 0f)
		{
			base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], mTargetWalkAnimTime);
		}
		mCurrentWalkAction = act;
	}

	protected void StartPointing()
	{
		mPointing = true;
		base.mAnimDirector.EnableOverride(pointOverride, true);
	}

	protected void StopPointing()
	{
		mPointing = false;
		base.mAnimDirector.EnableOverride(pointOverride, false);
	}

	protected void StopIdling()
	{
		mIdleTimer = 4f;
		base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0.25f);
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		switch (ParseCommand(com))
		{
		case PoseModuleSharedData.CommandCode.PrimAway:
			base.mAnimDirector.PlayAction(mActionHandle[21]);
			mEquippedAimWeight = 0f;
			break;
		case PoseModuleSharedData.CommandCode.PrimOut:
			base.mAnimDirector.PlayAction(mActionHandle[22]);
			mEquippedAimWeight = 1f;
			break;
		case PoseModuleSharedData.CommandCode.SecAway:
			base.mAnimDirector.PlayAction(mActionHandle[23]);
			mEquippedAimWeight = 0f;
			break;
		case PoseModuleSharedData.CommandCode.SecOut:
			base.mAnimDirector.PlayAction(mActionHandle[24]);
			mEquippedAimWeight = 1f;
			break;
		case PoseModuleSharedData.CommandCode.KnifeAway:
			base.mAnimDirector.PlayAction(mActionHandle[25]);
			mEquippedAimWeight = 0f;
			break;
		case PoseModuleSharedData.CommandCode.KnifeOut:
			base.mAnimDirector.PlayAction(mActionHandle[26]);
			mEquippedAimWeight = 1f;
			break;
		case PoseModuleSharedData.CommandCode.StartPointing:
			StartPointing();
			break;
		case PoseModuleSharedData.CommandCode.StopPointing:
			StopPointing();
			break;
		case PoseModuleSharedData.CommandCode.CancelIdle:
			StopIdling();
			break;
		case PoseModuleSharedData.CommandCode.ResetStand:
			base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0.25f);
			base.mAnimDirector.EnableCategory(mCategoryHandle[3], false, 0.25f);
			EnableRunning(false, 0.25f);
			EnableSauntering(false);
			base.mAnimDirector.EnableOverride(crouchOverride, false);
			base.mAnimDirector.EnableOverride(crouchShuffleOverride, false);
			base.mAnimDirector.EnableOverride(shuffleOverride, false);
			PlayWalkAction(ActionEnum.kStand, ActionEnum.kCount, 0.2f);
			base.mAimDir = myActor.Pose.onAxisTrans.forward;
			mLogicalSpeed = 0f;
			mCantTurnTimer = 0f;
			break;
		case PoseModuleSharedData.CommandCode.NoAim:
			aimingForbidden = true;
			break;
		case PoseModuleSharedData.CommandCode.Bashed:
			PlayAction(ActionEnum.kBashed, 0f, 0f);
			base.mAnimDirector.SetCategoryWeight(mCategoryHandle[6], 1f);
			break;
		}
		return PoseModuleSharedData.Modules.MoveAim;
	}

	public override Vector3 IdealFirstPersonAngles()
	{
		return new Vector3(0f, myActor.Pose.onAxisTrans.eulerAngles.y + CalculateAimYaw(), 0f);
	}

	private float CalculateAimYaw()
	{
		if (mAimType == AimType.FortyFive)
		{
			float num = 29f / 30f;
			float num2 = -45f;
			float num3 = 45f;
			return base.mAnimDirector.GetCategoryTime(mCategoryHandle[1]) / num * (num3 - num2) + num2;
		}
		return (base.mAnimDirector.GetCategoryTime(mCategoryHandle[1]) - 0.5f) * 180f;
	}
}
