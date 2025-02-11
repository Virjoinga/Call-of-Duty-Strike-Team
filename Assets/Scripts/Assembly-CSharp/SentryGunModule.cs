using UnityEngine;

public class SentryGunModule : BasePoseModule
{
	public enum SentryGunStateEnum
	{
		kAim = 0,
		kJiggle = 1,
		kDie = 2,
		kFire = 3,
		kIdle = 4
	}

	protected enum CategoryEnum
	{
		kMovement = 0,
		kShooting = 1,
		kCount = 2
	}

	protected enum ActionEnum
	{
		kAim = 0,
		kJiggle = 1,
		kDie = 2,
		kFire = 3,
		kCount = 4
	}

	public SentryGunStateEnum mState;

	private float aimAnimLength;

	protected override void Internal_Init()
	{
		Start_GetActionHandles();
	}

	private void Start_GetActionHandles()
	{
		mCategoryHandle = new int[2];
		mActionHandle = new AnimDirector.ActionHandle[4];
		GetCategoryHandle(CategoryEnum.kMovement, "Movement");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kAim, "Aim");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kJiggle, "Jiggle");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kDie, "Die");
		GetCategoryHandle(CategoryEnum.kShooting, "Shooting");
		GetActionHandle(CategoryEnum.kShooting, ActionEnum.kFire, "Fire");
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
		mState = SentryGunStateEnum.kAim;
		base.mAnimDirector.PlayAction(mActionHandle[0]);
		aimAnimLength = base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]);
	}

	public override PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		base.Model.transform.position = newPos;
		myActor.Pose.offAxisTrans.forward = myActor.realCharacter.mStartForward;
		base.modeltransforward = myActor.realCharacter.mStartForward;
		if (!myActor.realCharacter.IsDead())
		{
			if (!base.mAnimDirector.IsPlayingAction(mActionHandle[0]))
			{
				base.mAnimDirector.PlayAction(mActionHandle[0]);
			}
			if (myActor != null)
			{
				TaskSentryGun taskSentryGun = (TaskSentryGun)myActor.tasks.GetRunningTask(typeof(TaskSentryGun));
				if (taskSentryGun != null)
				{
					switch (mState)
					{
					case SentryGunStateEnum.kAim:
					{
						float num4 = Vector3.Dot(base.Model.transform.right, taskSentryGun.LookDirection.normalized);
						float num5 = Mathf.Acos(num4);
						if (num4 >= 1f)
						{
							num5 = 0.0001f;
						}
						else if (num4 <= -1f)
						{
							num5 = 3.14f;
						}
						float num6 = 57.29578f * num5;
						float num7 = num6 / 180f;
						float animTime2 = (1f - num7) * aimAnimLength;
						base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], animTime2);
						if (taskSentryGun.IsShootingAtTarget())
						{
							if (!base.mAnimDirector.IsPlayingAction(mActionHandle[3]))
							{
								base.mAnimDirector.PlayAction(mActionHandle[3]);
							}
						}
						else
						{
							base.mAnimDirector.StopAction(mActionHandle[3]);
						}
						break;
					}
					case SentryGunStateEnum.kJiggle:
						if (!base.mAnimDirector.IsPlayingAction(mActionHandle[1]))
						{
							base.mAnimDirector.PlayAction(mActionHandle[1]);
						}
						if (base.mAnimDirector.GetCategoryTime(mCategoryHandle[0]) > base.mAnimDirector.GetCategoryLength(mCategoryHandle[0]))
						{
							mState = SentryGunStateEnum.kAim;
						}
						break;
					case SentryGunStateEnum.kIdle:
					{
						float f = Vector3.Dot(base.Model.transform.right, taskSentryGun.LookDirection.normalized);
						float num = Mathf.Acos(f);
						float num2 = 57.29578f * num;
						float num3 = num2 / 180f;
						float animTime = (1f - num3) * aimAnimLength;
						base.mAnimDirector.SetCategoryTime(mCategoryHandle[0], animTime);
						TaskSentryGun runningTask = myActor.tasks.GetRunningTask<TaskSentryGun>();
						if (runningTask != null)
						{
							base.modeltransforward = runningTask.LookDirection;
						}
						break;
					}
					case SentryGunStateEnum.kDie:
						base.mAnimDirector.StopAction(mActionHandle[3]);
						break;
					}
				}
			}
		}
		return PoseModuleSharedData.Modules.SentryGun;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		switch (ParseCommand(com))
		{
		case PoseModuleSharedData.CommandCode.MoveToBegin:
			if (mState != SentryGunStateEnum.kJiggle)
			{
				mState = SentryGunStateEnum.kJiggle;
				base.mAnimDirector.PlayAction(mActionHandle[1]);
			}
			break;
		case PoseModuleSharedData.CommandCode.Melee:
			if (mState != SentryGunStateEnum.kDie)
			{
				mState = SentryGunStateEnum.kDie;
				base.mAnimDirector.AnimationPlayer.Stop();
				base.mAnimDirector.StopAction(mActionHandle[3]);
				base.mAnimDirector.PlayAction(mActionHandle[2]);
				myActor.realCharacter.DestroyHudMarker();
			}
			break;
		case PoseModuleSharedData.CommandCode.Ragdoll:
			mState = SentryGunStateEnum.kIdle;
			break;
		case PoseModuleSharedData.CommandCode.AtEase:
			mState = SentryGunStateEnum.kAim;
			break;
		default:
			Debug.LogWarning(string.Format("SentryGunModule: Unknown Pose Command: {0}", com));
			break;
		}
		return PoseModuleSharedData.Modules.SentryGun;
	}
}
