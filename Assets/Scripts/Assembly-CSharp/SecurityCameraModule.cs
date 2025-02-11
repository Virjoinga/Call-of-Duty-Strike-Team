using System;
using UnityEngine;

public class SecurityCameraModule : BasePoseModule
{
	public enum SecurityCameraStateEnum
	{
		kAim = 0,
		kPowerDown = 1,
		kPoweringDown = 2,
		kPoweredDown = 3,
		kPowerUp = 4,
		kPoweringUp = 5,
		kPoweredUp = 6
	}

	protected enum CategoryEnum
	{
		kMovement = 0,
		kCount = 1
	}

	protected enum ActionEnum
	{
		kAim = 0,
		kPowerUp = 1,
		kPowerDown = 2,
		kCount = 3
	}

	public SecurityCameraStateEnum mState;

	private float aimAnimLength;

	private Vector3 positionOffset = Vector3.zero;

	private bool on;

	protected override void Internal_Init()
	{
		Start_GetActionHandles();
	}

	private void Start_GetActionHandles()
	{
		mCategoryHandle = new int[1];
		mActionHandle = new AnimDirector.ActionHandle[3];
		GetCategoryHandle(CategoryEnum.kMovement, "Movement");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kAim, "Aim");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kPowerDown, "PowerDown");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kPowerUp, "PowerUp");
	}

	private void GetCategoryHandle(CategoryEnum cat, string name)
	{
		mCategoryHandle[(int)cat] = base.mAnimDirector.GetCategoryHandle(name);
	}

	private void GetActionHandle(CategoryEnum cat, ActionEnum act, string name)
	{
		mActionHandle[(int)act] = base.mAnimDirector.GetActionHandle(mCategoryHandle[(int)cat], name);
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		mState = SecurityCameraStateEnum.kAim;
		base.mAnimDirector.PlayAction(mActionHandle[0]);
		aimAnimLength = base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]);
		Transform transform = base.Model.transform.Find("SecurityCam_Base");
		if (transform != null)
		{
			Transform transform2 = transform.Find("SecurityCam_Head");
			if (transform2 != null)
			{
				positionOffset = transform.position - transform2.position;
			}
		}
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		base.Model.transform.position = newPos + positionOffset;
		myActor.Pose.offAxisTrans.forward = myActor.realCharacter.mStartForward;
		BaseCharacter component = GetComponent<BaseCharacter>();
		if (component != null && myActor != null)
		{
			TaskSecurityCamera taskSecurityCamera = (TaskSecurityCamera)myActor.tasks.GetRunningTask(typeof(TaskSecurityCamera));
			if (taskSecurityCamera != null)
			{
				if (!taskSecurityCamera.Enabled && mState == SecurityCameraStateEnum.kAim)
				{
					mState = SecurityCameraStateEnum.kPowerDown;
				}
				else if (taskSecurityCamera.Enabled && mState == SecurityCameraStateEnum.kPoweredDown)
				{
					mState = SecurityCameraStateEnum.kPowerUp;
				}
				switch (mState)
				{
				case SecurityCameraStateEnum.kAim:
				{
					float num = Vector3.Dot(base.Model.transform.right, taskSecurityCamera.LookDirection.normalized);
					float num2 = Mathf.Acos(num);
					if (num >= 1f)
					{
						num2 = 0.0001f;
					}
					else if (num <= -1f)
					{
						num2 = (float)Math.PI;
					}
					float num3 = 57.29578f * num2;
					float num4 = num3 / 180f;
					float animTime = num4 * aimAnimLength;
					base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], animTime);
					break;
				}
				case SecurityCameraStateEnum.kPowerDown:
					base.mAnimDirector.PlayAction(mActionHandle[2]);
					if (myActor.realCharacter.HudMarker != null)
					{
						myActor.realCharacter.HudMarker.SwitchOff();
					}
					mState = SecurityCameraStateEnum.kPoweringDown;
					break;
				case SecurityCameraStateEnum.kPoweringDown:
					if (base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]) > base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]))
					{
						mState = SecurityCameraStateEnum.kPoweredDown;
					}
					break;
				case SecurityCameraStateEnum.kPowerUp:
					base.mAnimDirector.PlayAction(mActionHandle[1]);
					if (myActor.realCharacter.HudMarker != null)
					{
						myActor.realCharacter.HudMarker.SwitchOn();
					}
					mState = SecurityCameraStateEnum.kPoweringUp;
					break;
				case SecurityCameraStateEnum.kPoweringUp:
					if (base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]) > base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]))
					{
						mState = SecurityCameraStateEnum.kPoweredUp;
					}
					break;
				case SecurityCameraStateEnum.kPoweredUp:
					mState = SecurityCameraStateEnum.kAim;
					base.mAnimDirector.PlayAction(mActionHandle[0]);
					break;
				}
			}
		}
		return PoseModuleSharedData.Modules.SecurityCamera;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		return PoseModuleSharedData.Modules.SecurityCamera;
	}
}
