using System;
using UnityEngine;

public class HighCornerCoverModule : BasePoseModule
{
	public enum EmergeMode
	{
		EdgeOut = 0,
		AimAround = 1
	}

	protected enum CoverStatus
	{
		kSuppressed = 0,
		kEngaged = 1
	}

	private const float kGrenadeThrowAnimDelay = 0.2f;

	private const float kSuppressedTime = 0f;

	private const float kEdge1Time = 8f / 15f;

	private const float kEdge2Time = 1.0666667f;

	private const float kEdge3Time = 1.6f;

	public EmergeMode mEmergeMode;

	private int mAimingHandle;

	private int mEdgeHandle;

	private int mDiveHandle;

	private int mCrouchHandle;

	private int mCautiousOverride;

	private int mAimAroundOverride;

	private AnimDirector.ActionHandle mEdgeOutHandle;

	private AnimDirector.ActionHandle mEdgeOutCrouchHandle;

	private AnimDirector.ActionHandle mAimHandle;

	private AnimDirector.ActionHandle mDiveBack1Handle;

	private AnimDirector.ActionHandle mDiveBack2Handle;

	private AnimDirector.ActionHandle mThrowGrenade;

	private float mBlendCharacter;

	public PoseModuleSharedData.Modules mModuleFrom;

	private RefNodeResource.Nodes mNode1;

	private RefNodeResource.Nodes mNode2;

	private RefNodeResource.Nodes mNode3;

	private float mDiveTimer;

	private Vector3 mCharblendpos;

	private Quaternion mCharblendrot;

	private Vector3 mSuppressedPos;

	private Quaternion mSuppressedRot;

	private Vector3 mCoverTangent;

	private float mGrenadeThrowTime;

	private bool mGrenadeThrowTriedMoving;

	private float mAimVertical;

	private float mPrevAimAngle;

	private PoseModuleSharedData.CommandCode newState;

	private float edgeAnimTime;

	private float edgeAnimTargetTime;

	protected override void Internal_Init()
	{
		base.mAimDir = base.Model.transform.forward;
		Remap(PoseModuleSharedData.CommandCode.Run, PoseModuleSharedData.CommandCode.Cancel);
		Remap(PoseModuleSharedData.CommandCode.Puppet, PoseModuleSharedData.CommandCode.Cancel);
		Remap(PoseModuleSharedData.CommandCode.MoveToBegin, PoseModuleSharedData.CommandCode.Cancel);
		Remap(PoseModuleSharedData.CommandCode.Shoot, PoseModuleSharedData.CommandCode.Cancel);
		mEdgeHandle = base.mAnimDirector.GetCategoryHandle("Edging");
		mDiveHandle = base.mAnimDirector.GetCategoryHandle("Reactions");
		mCrouchHandle = base.mAnimDirector.GetOverrideHandle("Crouch");
		mAimingHandle = base.mAnimDirector.GetCategoryHandle("Aiming");
		mAimHandle = base.mAnimDirector.GetActionHandle(mAimingHandle, "Aim");
		mCautiousOverride = base.mAnimDirector.GetOverrideHandle("Cautious");
		int categoryHandle = base.mAnimDirector.GetCategoryHandle("Throw");
		mThrowGrenade = base.mAnimDirector.GetActionHandle(categoryHandle, "Grenade");
		mGrenadeThrowTime = -1f;
		mGrenadeThrowTriedMoving = false;
	}

	public void SetCoverPoint(CoverPointCore cp)
	{
		if (cp.type == CoverPointCore.Type.HighCornerLeft)
		{
			mEdgeOutHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "EdgeOutLeft");
			mEdgeOutCrouchHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "EdgeOutLeftCrouch");
			mDiveBack1Handle = base.mAnimDirector.GetActionHandle(mDiveHandle, "DiveCoverLeft1");
			mDiveBack2Handle = base.mAnimDirector.GetActionHandle(mDiveHandle, "DiveCoverLeft2");
			mAimAroundOverride = base.mAnimDirector.GetOverrideHandle("CornerAimLeft");
			mNode1 = RefNodeResource.Nodes.AimWallLeft1;
			mNode2 = RefNodeResource.Nodes.AimWallLeft2;
			mNode3 = RefNodeResource.Nodes.AimWallLeft3;
			mEmergeMode = ((!myActor.behaviour.PlayerControlled) ? EmergeMode.AimAround : EmergeMode.EdgeOut);
		}
		else
		{
			mEdgeOutHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "EdgeOutRight");
			mEdgeOutCrouchHandle = base.mAnimDirector.GetActionHandle(mEdgeHandle, "EdgeOutRightCrouch");
			mDiveBack1Handle = base.mAnimDirector.GetActionHandle(mDiveHandle, "DiveCoverRight1");
			mDiveBack2Handle = base.mAnimDirector.GetActionHandle(mDiveHandle, "DiveCoverRight2");
			mAimAroundOverride = base.mAnimDirector.GetOverrideHandle("CornerAimRight");
			mNode1 = RefNodeResource.Nodes.AimWallRight1;
			mNode2 = RefNodeResource.Nodes.AimWallRight2;
			mNode3 = RefNodeResource.Nodes.AimWallRight3;
			mEmergeMode = EmergeMode.EdgeOut;
		}
		mAimVertical = 0f;
		base.mAnimDirector.PlayAction(mEdgeOutHandle, 0.5f);
		base.mAnimDirector.SetCategorySpeed(mEdgeHandle, 0f);
		base.mAnimDirector.SetCategoryTime(mEdgeHandle, 0f);
		myActor.Pose.idealOnAxisPos = cp.gamePos;
		myActor.Pose.idealOnAxisForward = cp.snappedNormal;
		myActor.Pose.offAxisPos = cp.gamePos;
		myActor.Pose.offAxisForward = cp.snappedNormal;
		mCoverTangent = cp.snappedTangent;
		myActor.Pose.BlendOffAxis(0.5f, AnimDirector.BlendEasing.Sharp, AnimDirector.BlendEasing.Soft, true);
		mCharblendpos = cp.gamePos;
		mCharblendrot = Quaternion.FromToRotation(Vector3.forward, cp.snappedNormal);
		if (mCharblendrot.x != 0f)
		{
			mCharblendrot = new Quaternion(0f, 1f, 0f, 0f);
		}
		mSuppressedPos = cp.gamePos;
		mSuppressedRot = mCharblendrot;
		mBlendCharacter = 0f;
	}

	public override void OnInactive(PoseModuleSharedData.Modules to)
	{
		base.mAnimDirector.EnableOverride(mAimAroundOverride, false);
		myActor.Pose.idealOnAxisPos = myActor.Pose.onAxisTrans.position;
		myActor.Pose.idealOnAxisRot = myActor.Pose.onAxisTrans.rotation;
		myActor.Pose.BlendOntoAxis(0.25f, AnimDirector.BlendEasing.Soft, AnimDirector.BlendEasing.Soft);
		base.mAnimDirector.EnableCategory(mEdgeHandle, false, 0.25f);
		GetComponent<MoveAimModule>().Command("ResetStand");
		if (myActor.gestures != null)
		{
			myActor.gestures.SetValidGestureParticipation(HandGestureModule.ForbidPriority.CurrentAction, HandGestureModule.GestureEnum.kAll);
		}
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		SetCoverPoint(myActor.awareness.coverBooked);
		mModuleFrom = fr;
		base.mAnimDirector.PlayAction(mAimHandle);
		base.mAnimDirector.SetCategorySpeed(mAimingHandle, 0f);
		base.mAnimDirector.SetCategoryWeight(mAimingHandle, base.mAimWeight);
		base.mAnimDirector.EnableOverride(mCrouchHandle, false);
		edgeAnimTime = 0f;
		edgeAnimTargetTime = 0f;
		mPrevAimAngle = -1f;
		mDiveTimer = -1f;
		if (myActor.gestures != null)
		{
			myActor.gestures.SetValidGestureParticipation(HandGestureModule.ForbidPriority.CurrentAction, (HandGestureModule.GestureEnum)3);
		}
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		if (!myActor.OnScreen && !expensiveTick)
		{
			return PoseModuleSharedData.Modules.HighCornerCover;
		}
		PoseModuleSharedData.CommandCode commandCode = ParseCommand(newStateStr);
		base.mAimDir = WorldHelper.ExpBlend(base.mAimDir, aimDir, 0.1f);
		if (edgeAnimTargetTime == 0f && mEmergeMode == EmergeMode.EdgeOut)
		{
			base.mAimWeight = WorldHelper.ExpBlend(base.mAimWeight, 0f, 0.2f);
		}
		else
		{
			base.mAimWeight = WorldHelper.ExpBlend(base.mAimWeight, 1f, 0.2f);
		}
		WorldHelper.CappedExpBlend(ref edgeAnimTime, edgeAnimTargetTime, 0.5f, 0.016666f);
		if (mDiveTimer > 0f)
		{
			mDiveTimer -= Time.deltaTime;
			if (mDiveTimer <= 0f)
			{
				edgeAnimTargetTime = 0f;
				edgeAnimTime = 0f;
			}
		}
		base.mAnimDirector.SetCategoryTime(mEdgeHandle, edgeAnimTime);
		base.mAnimDirector.SetCategorySpeed(mEdgeHandle, 0f);
		if (mBlendCharacter > 0.25f)
		{
			if (myActor.navAgent.enabled)
			{
				Vector3 offAxisPos = myActor.Pose.offAxisPos;
				offAxisPos.y = myActor.GetPosition().y;
				myActor.Pose.offAxisPos = offAxisPos;
			}
			myActor.Pose.onAxisTrans.position = (myActor.Pose.onAxisTrans.position * 4f + mCharblendpos) * 0.2f;
			if (myActor.OnScreen)
			{
				myActor.Pose.onAxisTrans.rotation = Quaternion.Slerp(myActor.Pose.onAxisTrans.rotation, mCharblendrot, 0.2f);
			}
			myActor.SetPosition(myActor.Pose.onAxisTrans.position);
		}
		else
		{
			mBlendCharacter += Time.deltaTime;
		}
		DoAiming();
		if (commandCode == PoseModuleSharedData.CommandCode.ThrowGrenade)
		{
			base.mAimDir = aimDir;
			Update_ThrowGrenade();
		}
		return PoseModuleSharedData.Modules.HighCornerCover;
	}

	private void DoAiming()
	{
		if (mEmergeMode == EmergeMode.EdgeOut)
		{
			DoAiming_EdgeOut();
		}
		else
		{
			DoAiming_AimAround();
		}
	}

	private void DoAiming_EdgeOut()
	{
		if (myActor.OnScreen)
		{
			if (mAimAroundOverride != -1)
			{
				base.mAnimDirector.EnableOverride(mAimAroundOverride, false);
			}
			Vector3 forward = myActor.Pose.onAxisTrans.forward;
			Vector3 vector = base.mAimDir;
			vector.y = 0f;
			vector.Normalize();
			float num = (Mathf.Asin(Mathf.Clamp(forward.z * vector.x - forward.x * vector.z, -1f, 1f)) - -(float)Math.PI / 4f) / ((float)Math.PI / 2f) * (29f / 30f);
			num = ((!(Vector3.Dot(forward, base.mAimDir) > 0f)) ? ((!(num > 0.5f)) ? 0.25f : 0.74f) : Mathf.Clamp(num, 0.25f, 0.74f));
			float sqrMagnitude = base.mAimDir.xz().sqrMagnitude;
			if (sqrMagnitude > 0.01f)
			{
				mAimVertical = Mathf.Clamp01(base.mAimDir.y / sqrMagnitude / 1.3333f + 0.5f);
			}
			base.mAnimDirector.SetCategoryTimeSpeedWeightBlendTreeBlend(mAimingHandle, num, 0f, base.mAimWeight, mAimVertical);
			base.mAnimDirector.EnableOverride(mCautiousOverride, myActor.behaviour.alertState == BehaviourController.AlertState.Alerted || myActor.Pose.restrictAiming);
		}
		myActor.weapon.SetAiming(base.mAimWeight > 0.95f && Mathf.Abs(edgeAnimTime - edgeAnimTargetTime) < 0.05f);
	}

	private void DoAiming_AimAround()
	{
		if (mBlendCharacter > 0.25f)
		{
			base.mAnimDirector.EnableOverride(mCautiousOverride, false);
			base.mAnimDirector.EnableOverride(mAimAroundOverride, true);
			Vector3 offAxisForward = myActor.Pose.offAxisForward;
			float value = 0f - Vector2.Dot(offAxisForward.xz(), base.mAimDir.xz().normalized);
			float num = Mathf.Min(0.9f, Mathf.Asin(Mathf.Clamp01(value)) * (2f / (float)Math.PI));
			float sqrMagnitude = base.mAimDir.xz().sqrMagnitude;
			if (sqrMagnitude > 0.01f)
			{
				mAimVertical = Mathf.Clamp01(base.mAimDir.y / sqrMagnitude / 1.3333f + 0.5f);
			}
			if (mPrevAimAngle >= 0f)
			{
				WorldHelper.ExpBlend(ref mPrevAimAngle, num, 0.1f);
			}
			else
			{
				mPrevAimAngle = num;
			}
			base.mAnimDirector.SetCategoryTimeSpeedWeightBlendTreeBlend(mAimingHandle, mPrevAimAngle, 0f, base.mAimWeight, mAimVertical);
			mCharblendpos = myActor.Pose.offAxisPos + mCoverTangent * (0.6f * num);
			myActor.weapon.SetAiming(base.mAimWeight > 0.95f);
		}
	}

	private void Update_Suppressed()
	{
	}

	private void Update_Engaged()
	{
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		if (mEmergeMode == EmergeMode.EdgeOut)
		{
			return Command_EdgeOut(com);
		}
		return Command_AimAround(com);
	}

	private PoseModuleSharedData.Modules Command_AimAround(string com)
	{
		PoseModuleSharedData.CommandCode commandCode = ParseCommand(com);
		PoseModuleSharedData.CommandCode commandCode2 = commandCode;
		if (commandCode2 == PoseModuleSharedData.CommandCode.CoverInvalid || commandCode2 == PoseModuleSharedData.CommandCode.Cancel)
		{
			return mModuleFrom;
		}
		return PoseModuleSharedData.Modules.HighCornerCover;
	}

	private PoseModuleSharedData.Modules Command_EdgeOut(string com)
	{
		switch (ParseCommand(com))
		{
		case PoseModuleSharedData.CommandCode.Dive:
			mCharblendpos = mSuppressedPos;
			mCharblendrot = mSuppressedRot;
			if (edgeAnimTargetTime == 8f / 15f)
			{
				edgeAnimTargetTime = 0f;
				break;
			}
			if (edgeAnimTargetTime == 1.0666667f)
			{
				base.mAnimDirector.PlayAction(mDiveBack1Handle, 0.1f, true);
			}
			if (edgeAnimTargetTime == 1.6f)
			{
				base.mAnimDirector.PlayAction(mDiveBack2Handle, 0.1f, true);
			}
			mDiveTimer = 0.2f;
			break;
		case PoseModuleSharedData.CommandCode.Edge1:
			edgeAnimTargetTime = 8f / 15f;
			PlayAppropriateEdgeAnim();
			RefNodeResource.GetRefLocalTo(mNode1, myActor.Pose.offAxisPos, myActor.Pose.offAxisRot, out mCharblendpos, out mCharblendrot);
			break;
		case PoseModuleSharedData.CommandCode.Edge2:
			edgeAnimTargetTime = 1.0666667f;
			PlayAppropriateEdgeAnim();
			RefNodeResource.GetRefLocalTo(mNode2, myActor.Pose.offAxisPos, myActor.Pose.offAxisRot, out mCharblendpos, out mCharblendrot);
			break;
		case PoseModuleSharedData.CommandCode.Edge3:
			edgeAnimTargetTime = 1.6f;
			PlayAppropriateEdgeAnim();
			RefNodeResource.GetRefLocalTo(mNode3, myActor.Pose.offAxisPos, myActor.Pose.offAxisRot, out mCharblendpos, out mCharblendrot);
			break;
		case PoseModuleSharedData.CommandCode.CoverInvalid:
		case PoseModuleSharedData.CommandCode.Cancel:
			return mModuleFrom;
		}
		return PoseModuleSharedData.Modules.HighCornerCover;
	}

	private void PlayAppropriateEdgeAnim()
	{
		if (myActor.realCharacter.IsCrouching())
		{
			base.mAnimDirector.PlayAction(mEdgeOutCrouchHandle, 0.5f);
			base.mAnimDirector.SetCategorySpeed(mEdgeHandle, 0f);
			base.mAnimDirector.SetCategoryTime(mEdgeHandle, edgeAnimTime);
		}
		else
		{
			base.mAnimDirector.PlayAction(mEdgeOutHandle, 0.5f);
			base.mAnimDirector.SetCategorySpeed(mEdgeHandle, 0f);
			base.mAnimDirector.SetCategoryTime(mEdgeHandle, edgeAnimTime);
		}
	}

	private void Update_ThrowGrenade()
	{
		Vector3 lhs = base.modeltransforward;
		lhs.y = 0f;
		lhs.Normalize();
		float num = Vector3.Dot(lhs, base.mAimDir);
		bool flag = num > 0f;
		if (!Mathf.Approximately(edgeAnimTime, edgeAnimTargetTime) || !(mDiveTimer <= 0f))
		{
			return;
		}
		if (flag)
		{
			if (mGrenadeThrowTime == -1f)
			{
				base.mAnimDirector.PlayAction(mThrowGrenade);
				mGrenadeThrowTime = 0f;
			}
			else if (mGrenadeThrowTime >= 0.2f)
			{
				myActor.tasks.Command("ThrowGrenade");
				mGrenadeThrowTime = -1f;
				mGrenadeThrowTriedMoving = false;
			}
			else
			{
				mGrenadeThrowTime += Time.deltaTime;
			}
		}
		else if (Mathf.Approximately(edgeAnimTime, 0f) && !mGrenadeThrowTriedMoving)
		{
			Command("Edge3");
			mGrenadeThrowTriedMoving = true;
		}
		else if (!mGrenadeThrowTriedMoving)
		{
			Command("Dive");
			mGrenadeThrowTriedMoving = true;
		}
		else
		{
			myActor.tasks.Command("CancelGrenade");
		}
	}
}
