using UnityEngine;

public class FixedGunModule : BasePoseModule
{
	private AnimDirector.ActionHandle mIdleAction;

	private FixedGun mFixedGun;

	public void SetFixedGun(FixedGun fixedGun)
	{
		mFixedGun = fixedGun;
	}

	protected override void Internal_Init()
	{
		mIdleAction = base.mAnimDirector.GetActionHandle(base.mAnimDirector.GetCategoryHandle("Movement"), "Stand");
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("FixedGun"), false);
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Turning"), false, 0f);
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Aiming"), false, 0f);
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Shooting"), false, 0f);
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Gesture"), false, 0f);
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Idles"), false, 0f);
		base.mAnimDirector.PlayAction(mIdleAction);
	}

	public override void OnInactive(PoseModuleSharedData.Modules to)
	{
		if (mFixedGun != null)
		{
			myActor.Pose.offAxisPos = mFixedGun.GunnerLocator.position;
			myActor.Pose.offAxisForward = mFixedGun.GunnerLocator.position;
			myActor.Pose.onAxisTrans.position = mFixedGun.GunnerLocator.position;
			myActor.Pose.onAxisTrans.rotation = mFixedGun.GunnerLocator.rotation;
			myActor.Pose.idealOnAxisForward = mFixedGun.GunnerLocator.forward;
			myActor.Pose.idealOnAxisPos = mFixedGun.GunnerLocator.position;
			myActor.Pose.idealOnAxisForward = mFixedGun.GunnerLocator.forward;
			myActor.Pose.BlendOntoAxis(1f, AnimDirector.BlendEasing.Soft, AnimDirector.BlendEasing.Soft);
			GetComponent<MoveAimModule>().Command("ResetStand");
		}
		mFixedGun = null;
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		newStateStr = "FixedGun";
		return PoseModuleSharedData.Modules.FixedGun;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		return PoseModuleSharedData.Modules.FixedGun;
	}
}
