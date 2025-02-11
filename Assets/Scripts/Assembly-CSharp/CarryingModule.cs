using UnityEngine;

public class CarryingModule : MoveAimModule
{
	protected override void Internal_Init()
	{
		Start_GetActionHandles();
		shuffleOverride = -1;
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
		mWalkStrideLength = 42f * base.mDesc.mStandardWalkSpeed;
		mDesiredForward = Vector3.zero;
		mMaxWalkAnimSpeed = 0f;
		if (myActor.realCharacter.IsFirstPerson)
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBodyFP"), true, 0f);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBody"), false, 0f);
		}
		else
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBodyFP"), false, 0f);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBody"), true, 0f);
		}
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		PoseModuleSharedData.CommandCode commandCode = ParseCommand(newStateStr);
		if (commandCode == PoseModuleSharedData.CommandCode.Puppet)
		{
			mState = MoveAimStateEnum.kPuppet;
			base.mAimWeight = 0f;
			base.mAnimDirector.SetCategorySpeed(mCategoryHandle[1], 0f);
			base.mAnimDirector.SetCategoryWeight(mCategoryHandle[1], base.mAimWeight);
			return PoseModuleSharedData.Modules.Carrying;
		}
		base.modeltransposition = newPos;
		newVel /= 60f;
		mLogicalPosition = newPos;
		newVel.y = 0f;
		if (mState != MoveAimStateEnum.kPuppet)
		{
			mLogicalVelocity = newVel;
			mLogicalSpeed = newVel.magnitude;
		}
		mDestination = destination;
		base.mAimDir = WorldHelper.ExpBlend(base.mAimDir, aimDir, 0.1f);
		mCantTurnTimer = Mathf.Max(0f, mCantTurnTimer - Time.deltaTime);
		switch (mState)
		{
		case MoveAimStateEnum.kStationary:
		case MoveAimStateEnum.kPuppet:
			switch (commandCode)
			{
			case PoseModuleSharedData.CommandCode.Idle:
			case PoseModuleSharedData.CommandCode.Stand:
			case PoseModuleSharedData.CommandCode.Crouch:
				Update_Stationary_Stand();
				break;
			case PoseModuleSharedData.CommandCode.Crawl:
			case PoseModuleSharedData.CommandCode.Walk:
				StationaryToWalking();
				break;
			}
			break;
		case MoveAimStateEnum.kWalking:
			switch (commandCode)
			{
			case PoseModuleSharedData.CommandCode.Idle:
			case PoseModuleSharedData.CommandCode.Stand:
			case PoseModuleSharedData.CommandCode.Crouch:
				WalkingToStationary();
				break;
			case PoseModuleSharedData.CommandCode.Crawl:
			case PoseModuleSharedData.CommandCode.Walk:
				Update_Walking();
				break;
			}
			break;
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
		base.mAnimDirector.SetCategorySpeed(mCategoryHandle[1], 0f);
		base.mAnimDirector.SetCategoryWeight(mCategoryHandle[1], base.mAimWeight);
		myActor.weapon.SetAiming(false);
		return PoseModuleSharedData.Modules.Carrying;
	}

	protected override void Update_Walking()
	{
		base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0.5f);
		base.mAnimDirector.EnableCategory(mCategoryHandle[3], false, 0.5f);
		base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("Run"), false);
		base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("Sprint"), false);
		if (myActor.realCharacter.IsFirstPerson)
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBody"), false);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBodyFP"), true);
		}
		else
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBody"), true);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBodyFP"), false);
		}
		base.mAimWeight = WorldHelper.ExpBlend(base.mAimWeight, 0f, 0.2f);
		Update_Walking_Aim();
	}

	protected override void Update_Stationary_Stand()
	{
		mShuffle = false;
		base.mAnimDirector.EnableCategory(mCategoryHandle[2], false, 0.5f);
		if (myActor.realCharacter.IsFirstPerson)
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBodyFP"), true);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBody"), false);
		}
		else
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBodyFP"), false);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBody"), true);
		}
		base.mAimWeight = WorldHelper.ExpBlend(base.mAimWeight, 0f, 0.2f);
		PlayWalkAction(ActionEnum.kStand, ActionEnum.kCount);
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		switch (ParseCommand(com))
		{
		case PoseModuleSharedData.CommandCode.CancelCarrying:
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBody"), false);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarryBodyFP"), false);
			return PoseModuleSharedData.Modules.MoveAim;
		case PoseModuleSharedData.CommandCode.ResetStand:
			PlayWalkAction(ActionEnum.kStand, ActionEnum.kCount, 0.2f);
			base.mAimDir = base.Model.transform.forward;
			break;
		}
		return PoseModuleSharedData.Modules.Carrying;
	}
}
