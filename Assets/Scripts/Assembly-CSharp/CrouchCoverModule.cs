using UnityEngine;

public class CrouchCoverModule : BasePoseModule
{
	private int mAimingHandle;

	private int mEdgeHandle;

	private int mCrouchHandle;

	private int mHunchHandle;

	private int mPeekHandle;

	private int cautiousOverride;

	private int mStepOutLeftOverrideHandle;

	private int mStepOutRightOverrideHandle;

	private AnimDirector.ActionHandle mPopUpHandle;

	private AnimDirector.ActionHandle mAimHandle;

	private AnimDirector.ActionHandle mPopDownHandle;

	private AnimDirector.ActionHandle mIntoHandle;

	private AnimDirector.ActionHandle mHitchLeftHandle;

	private AnimDirector.ActionHandle mHitchRightHandle;

	private AnimDirector.ActionHandle mStepOutLeftHandle;

	private AnimDirector.ActionHandle mStepOutRightHandle;

	private AnimDirector.ActionHandle mStepBackLeftHandle;

	private AnimDirector.ActionHandle mStepBackRightHandle;

	private PoseModuleSharedData.Modules moduleFrom;

	public bool mPoppedUp;

	public bool mSteppedOutLeft;

	public bool mSteppedOutRight;

	public CoverPointCore coverPoint;

	private PoseModuleSharedData.CommandCode newState;

	protected override void Internal_Init()
	{
		Remap(PoseModuleSharedData.CommandCode.Crawl, PoseModuleSharedData.CommandCode.Cancel);
		Remap(PoseModuleSharedData.CommandCode.Walk, PoseModuleSharedData.CommandCode.Cancel);
		Remap(PoseModuleSharedData.CommandCode.Saunter, PoseModuleSharedData.CommandCode.Cancel);
		Remap(PoseModuleSharedData.CommandCode.Run, PoseModuleSharedData.CommandCode.Cancel);
		Remap(PoseModuleSharedData.CommandCode.Puppet, PoseModuleSharedData.CommandCode.Cancel);
		Remap(PoseModuleSharedData.CommandCode.MoveToBegin, PoseModuleSharedData.CommandCode.Cancel);
		mCrouchHandle = base.mAnimDirector.GetOverrideHandle("Crouch");
		mHunchHandle = base.mAnimDirector.GetOverrideHandle("Hunched");
		mPeekHandle = base.mAnimDirector.GetOverrideHandle("Peekover");
		mStepOutLeftOverrideHandle = base.mAnimDirector.GetOverrideHandle("StepOutLowCoverLeft");
		mStepOutRightOverrideHandle = base.mAnimDirector.GetOverrideHandle("StepOutLowCoverRight");
		mEdgeHandle = base.mAnimDirector.GetCategoryHandle("Edging");
		cautiousOverride = base.mAnimDirector.GetOverrideHandle("Cautious");
		mAimingHandle = base.mAnimDirector.GetCategoryHandle("Aiming");
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		SetCoverPoint(myActor.awareness.coverBooked);
		moduleFrom = fr;
		base.mAnimDirector.SetCategorySpeed(mAimingHandle, 0f);
		base.mAnimDirector.SetCategoryWeight(mAimingHandle, base.mAimWeight);
		mPoppedUp = false;
		mSteppedOutLeft = false;
		mSteppedOutRight = false;
		if (myActor.gestures != null)
		{
			myActor.gestures.SetValidGestureParticipation(HandGestureModule.ForbidPriority.CurrentAction, (HandGestureModule.GestureEnum)3);
		}
	}

	public void SetCoverPoint(CoverPointCore cp)
	{
		coverPoint = cp;
		mPopUpHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "PopUp");
		mPopDownHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "PopDown");
		mIntoHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "IntoLowCover");
		mStepOutLeftHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "StepOutLeft");
		mStepOutRightHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "StepOutRight");
		mStepBackLeftHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "StepBackLeft");
		mStepBackRightHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "StepBackRight");
		mHitchLeftHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "HitchLeft");
		mHitchRightHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "HitchRight");
		base.mAnimDirector.PlayAction(mIntoHandle, 0.5f);
		myActor.Pose.offAxisPos = coverPoint.gamePos;
		myActor.Pose.offAxisForward = coverPoint.StandFacing;
		myActor.Pose.idealOnAxisPos = coverPoint.gamePos;
		myActor.Pose.idealOnAxisForward = coverPoint.StandFacing;
		myActor.Pose.BlendOffAxis(0.25f, AnimDirector.BlendEasing.Linear, AnimDirector.BlendEasing.Soft, true);
	}

	public override void OnInactive(PoseModuleSharedData.Modules to)
	{
		myActor.Pose.BlendOntoAxis(0.25f, AnimDirector.BlendEasing.Soft, AnimDirector.BlendEasing.Linear);
		base.mAnimDirector.EnableOverride(mHunchHandle, false);
		base.mAnimDirector.EnableOverride(mPeekHandle, false);
		base.mAnimDirector.EnableOverride(mStepOutLeftOverrideHandle, false);
		base.mAnimDirector.EnableOverride(mStepOutRightOverrideHandle, false);
		base.mAnimDirector.EnableCategory(mEdgeHandle, false, 0.25f, RawAnimation.AnimBlendType.kSoftLinear);
		Vector3 vector = base.mAimDir;
		GetComponent<MoveAimModule>().Command("ResetStand");
		base.mAimDir = vector;
		if (myActor.gestures != null)
		{
			myActor.gestures.SetValidGestureParticipation(HandGestureModule.ForbidPriority.CurrentAction, HandGestureModule.GestureEnum.kAll);
		}
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		if (!myActor.OnScreen && !expensiveTick)
		{
			return PoseModuleSharedData.Modules.CrouchCover;
		}
		base.mAimDir = WorldHelper.ExpBlend(base.mAimDir, aimDir, 0.1f);
		if (mPoppedUp || mSteppedOutLeft || mSteppedOutRight)
		{
			base.mAimWeight = WorldHelper.ExpBlend(base.mAimWeight, 1f, 0.2f);
			base.mAnimDirector.EnableOverride(mCrouchHandle, false);
		}
		else
		{
			base.mAimWeight = WorldHelper.ExpBlend(base.mAimWeight, 0f, 0.2f);
		}
		((MoveAimModule)myActor.Pose.GetModule(PoseModuleSharedData.Modules.MoveAim)).DoAiming();
		base.mAnimDirector.EnableOverride(cautiousOverride, false);
		base.mAnimDirector.SetCategorySpeed(mAimingHandle, 0f);
		base.mAnimDirector.SetCategoryWeight(mAimingHandle, base.mAimWeight);
		myActor.weapon.SetAiming(base.mAimWeight > 0.95f);
		return PoseModuleSharedData.Modules.CrouchCover;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		switch (ParseCommand(com))
		{
		case PoseModuleSharedData.CommandCode.PopUp:
			if (!mPoppedUp)
			{
				base.mAnimDirector.EnableOverride(mHunchHandle, false);
				base.mAnimDirector.EnableOverride(mPeekHandle, false);
				base.mAnimDirector.PlayAction(mPopUpHandle, true);
				mPoppedUp = true;
			}
			break;
		case PoseModuleSharedData.CommandCode.Hunch:
			if (!mPoppedUp)
			{
				base.mAnimDirector.EnableOverride(mHunchHandle, true);
				base.mAnimDirector.EnableOverride(mPeekHandle, false);
				base.mAnimDirector.PlayAction(mPopUpHandle, true);
				mPoppedUp = true;
			}
			break;
		case PoseModuleSharedData.CommandCode.PeekOver:
			if (!mPoppedUp)
			{
				base.mAnimDirector.EnableOverride(mHunchHandle, false);
				base.mAnimDirector.EnableOverride(mPeekHandle, true);
				base.mAnimDirector.PlayAction(mPopUpHandle, true);
				mPoppedUp = true;
			}
			break;
		case PoseModuleSharedData.CommandCode.PopDown:
			if (mPoppedUp)
			{
				base.mAnimDirector.PlayAction(mPopDownHandle, true);
				mPoppedUp = false;
			}
			if (mSteppedOutLeft)
			{
				base.mAnimDirector.EnableOverride(mStepOutLeftOverrideHandle, false);
				base.mAnimDirector.PlayAction(mStepBackLeftHandle, true);
				mSteppedOutLeft = false;
			}
			if (mSteppedOutRight)
			{
				base.mAnimDirector.EnableOverride(mStepOutRightOverrideHandle, false);
				base.mAnimDirector.PlayAction(mStepBackRightHandle, true);
				mSteppedOutRight = false;
			}
			break;
		case PoseModuleSharedData.CommandCode.HitchLeft:
			base.mAnimDirector.PlayAction(mHitchLeftHandle, true);
			mPoppedUp = false;
			break;
		case PoseModuleSharedData.CommandCode.HitchRight:
			base.mAnimDirector.PlayAction(mHitchRightHandle, true);
			mPoppedUp = false;
			break;
		case PoseModuleSharedData.CommandCode.StepLeft:
			if (!mSteppedOutLeft)
			{
				base.mAnimDirector.EnableOverride(mStepOutLeftOverrideHandle, true);
				base.mAnimDirector.PlayAction(mStepOutLeftHandle, true);
				mSteppedOutLeft = true;
			}
			break;
		case PoseModuleSharedData.CommandCode.StepRight:
			if (!mSteppedOutRight)
			{
				base.mAnimDirector.EnableOverride(mStepOutRightOverrideHandle, true);
				base.mAnimDirector.PlayAction(mStepOutRightHandle, true);
				mSteppedOutRight = true;
			}
			break;
		case PoseModuleSharedData.CommandCode.StartPointing:
			return PoseModuleSharedData.Modules.CrouchCover;
		case PoseModuleSharedData.CommandCode.StopPointing:
			return PoseModuleSharedData.Modules.CrouchCover;
		case PoseModuleSharedData.CommandCode.Cancel:
			return moduleFrom;
		case PoseModuleSharedData.CommandCode.CoverInvalid:
			coverPoint = null;
			return moduleFrom;
		}
		return PoseModuleSharedData.Modules.CrouchCover;
	}

	public override Vector3 IdealFirstPersonAngles()
	{
		float num = 0f;
		return new Vector3(0f, myActor.Pose.onAxisTrans.eulerAngles.y + num, 0f);
	}
}
