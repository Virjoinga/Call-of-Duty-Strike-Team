using UnityEngine;

public class CarriedModule : BasePoseModule
{
	private const int kCancelCarried = 1;

	private AnimDirector.ActionHandle mIdleAction;

	private PoseModuleSharedData.Modules moduleFrom;

	protected override void Internal_Init()
	{
		mIdleAction = base.mAnimDirector.GetActionHandle(base.mAnimDirector.GetCategoryHandle("Movement"), "Stand");
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		moduleFrom = fr;
		if (myActor.realCharacter.CarriedBy.realCharacter.IsFirstPerson)
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarriedFP"), true, 0f);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("Carried"), false, 0f);
		}
		else
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarriedFP"), false, 0f);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("Carried"), true, 0f);
		}
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Aiming"), false, 0.1f);
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Wounded"), false, 0.1f);
		base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Movement"), true, 0.1f);
		base.mAnimDirector.PlayAction(mIdleAction);
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		if (myActor.realCharacter.CarriedBy.Pose.blend == 0f && myActor.realCharacter.CarriedBy.Pose.direction != PoseModuleSharedData.BlendDirection.OffAxis)
		{
			myActor.Pose.offAxisPos = newPos;
			myActor.Pose.offAxisForward = aimDir;
			base.modeltransforward = aimDir;
			base.modeltransposition = newPos;
		}
		myActor.weapon.SetAiming(false);
		if (myActor.realCharacter.CarriedBy != null)
		{
			base.mAnimDirector.SetCategoryTime(0, myActor.realCharacter.CarriedBy.animDirector.GetCategoryTime(0));
		}
		newStateStr = "Carried";
		return PoseModuleSharedData.Modules.Carried;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		if (ParseCommand(com) == PoseModuleSharedData.CommandCode.CancelCarried)
		{
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("Carried"), false);
			base.mAnimDirector.EnableOverride(base.mAnimDirector.GetOverrideHandle("CarriedFP"), false);
			base.mAnimDirector.EnableCategory(base.mAnimDirector.GetCategoryHandle("Wounded"), true, 0.1f);
			if (myActor.realCharacter.IsDead())
			{
				base.mAnimDirector.PlayAction(mIdleAction);
			}
			return moduleFrom;
		}
		return PoseModuleSharedData.Modules.Carried;
	}
}
