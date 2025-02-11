using UnityEngine;

public class RiotShieldNPCModule : BasePoseModule
{
	public enum RiotShieldNPCStateEnum
	{
		kStand = 0,
		kTurnToFace = 1,
		kAdvance = 2,
		kFlinch = 3,
		kShieldBash = 4,
		kShootPistol = 5,
		kThrowGrenade = 6,
		kFlinchBig = 7
	}

	protected enum CategoryEnum
	{
		kMovement = 0,
		kCount = 1
	}

	public enum ActionEnum
	{
		kStand = 0,
		kTurnToFace = 1,
		kAdvance = 2,
		kFlinch = 3,
		kShieldBash = 4,
		kShootPistol = 5,
		kThrowGrenade = 6,
		kFlinchBig = 7,
		kShieldBashComplete = 8,
		kTransWalkToGrenade = 9,
		kGrenadeIdle = 10,
		kTransGrenadeToWalk = 11,
		kCount = 12
	}

	public RiotShieldNPCStateEnum mState;

	private ActionEnum mAction;

	private int mGrenadeAnimationSequence;

	private TaskRiotShield mRiotShieldTask;

	private float mBashTime;

	private float mThrowGrenadeTimer;

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
		base.mAnimDirector.PlayAction(mActionHandle[0]);
	}

	public override void OnActive(PoseModuleSharedData.Modules fr)
	{
		mState = RiotShieldNPCStateEnum.kStand;
	}

	private void Reset()
	{
		mState = RiotShieldNPCStateEnum.kStand;
	}

	protected void Start_GetActionHandles()
	{
		mCategoryHandle = new int[1];
		mActionHandle = new AnimDirector.ActionHandle[12];
		GetCategoryHandle(CategoryEnum.kMovement, "Movement");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kStand, "Stand");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kTurnToFace, "TurnToFace");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kAdvance, "Advance");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kFlinch, "Flinch");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kShieldBash, "ShieldBash");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kShootPistol, "ShootPistol");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kThrowGrenade, "ThrowGrenade");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kFlinchBig, "FlinchBig");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kTransWalkToGrenade, "TransWalkToGrenade");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kGrenadeIdle, "GrenadeIdle");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kTransGrenadeToWalk, "TransGrenadeToWalk");
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
		mRiotShieldTask = myActor.tasks.GetRunningTask<TaskRiotShield>();
		if (mRiotShieldTask != null && mThrowGrenadeTimer > 0f)
		{
			mThrowGrenadeTimer -= Time.deltaTime;
			if (mThrowGrenadeTimer <= 0f)
			{
				mRiotShieldTask.NotifyAnimationStateComplete(ActionEnum.kThrowGrenade);
			}
		}
		switch (mState)
		{
		case RiotShieldNPCStateEnum.kShootPistol:
			if (base.mAnimDirector.HasCurrentActionCompleted(mCategoryHandle[0]))
			{
				mRiotShieldTask.NotifyAnimationStateComplete(ActionEnum.kShootPistol);
			}
			break;
		case RiotShieldNPCStateEnum.kThrowGrenade:
			if (base.mAnimDirector.HasCurrentActionCompleted(mCategoryHandle[0]))
			{
				if (mGrenadeAnimationSequence == 0)
				{
					base.mAnimDirector.PlayAction(mActionHandle[10], true);
				}
				else if (mGrenadeAnimationSequence == 1)
				{
					base.mAnimDirector.PlayAction(mActionHandle[6], true);
					mThrowGrenadeTimer = 0.5f;
				}
				else if (mGrenadeAnimationSequence == 2)
				{
					base.mAnimDirector.PlayAction(mActionHandle[10], true);
				}
				else if (mGrenadeAnimationSequence == 3)
				{
					base.mAnimDirector.PlayAction(mActionHandle[11], true);
				}
				else
				{
					mRiotShieldTask.NotifyAnimationStateComplete(ActionEnum.kTransGrenadeToWalk);
				}
				mGrenadeAnimationSequence++;
			}
			break;
		case RiotShieldNPCStateEnum.kShieldBash:
			if (mBashTime > 0f)
			{
				mBashTime -= Time.deltaTime;
				if (mBashTime <= 0f)
				{
					mRiotShieldTask.NotifyAnimationStateComplete(ActionEnum.kShieldBash);
				}
			}
			if (base.mAnimDirector.HasCurrentActionCompleted(mCategoryHandle[0]))
			{
				mRiotShieldTask.NotifyAnimationStateComplete(ActionEnum.kShieldBashComplete);
			}
			break;
		}
		return PoseModuleSharedData.Modules.RiotShieldNPC;
	}

	public override PoseModuleSharedData.Modules Command(string com)
	{
		ActionEnum actionEnum = ActionEnum.kCount;
		for (int i = 0; i < 12; i++)
		{
			ActionEnum actionEnum2 = (ActionEnum)i;
			if (actionEnum2.ToString().Contains(com))
			{
				actionEnum = actionEnum2;
				break;
			}
		}
		if (actionEnum == ActionEnum.kCount)
		{
			if (!(com == "MoveToBegin"))
			{
				return PoseModuleSharedData.Modules.RiotShieldNPC;
			}
			actionEnum = ActionEnum.kAdvance;
		}
		mAction = actionEnum;
		switch (actionEnum)
		{
		case ActionEnum.kStand:
			mState = RiotShieldNPCStateEnum.kStand;
			break;
		case ActionEnum.kTurnToFace:
			mState = RiotShieldNPCStateEnum.kTurnToFace;
			break;
		case ActionEnum.kAdvance:
			mState = RiotShieldNPCStateEnum.kAdvance;
			break;
		case ActionEnum.kFlinch:
			mState = RiotShieldNPCStateEnum.kFlinch;
			break;
		case ActionEnum.kShieldBash:
			mState = RiotShieldNPCStateEnum.kShieldBash;
			mBashTime = 0.2f;
			break;
		case ActionEnum.kShootPistol:
			mState = RiotShieldNPCStateEnum.kShootPistol;
			break;
		case ActionEnum.kThrowGrenade:
			mState = RiotShieldNPCStateEnum.kThrowGrenade;
			mAction = ActionEnum.kTransWalkToGrenade;
			mGrenadeAnimationSequence = 0;
			break;
		case ActionEnum.kFlinchBig:
			mState = RiotShieldNPCStateEnum.kFlinchBig;
			break;
		}
		base.mAnimDirector.PlayAction(mActionHandle[(int)mAction], true);
		return PoseModuleSharedData.Modules.RiotShieldNPC;
	}
}
