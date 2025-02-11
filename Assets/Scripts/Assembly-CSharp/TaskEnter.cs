using System;

public class TaskEnter : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		TargetInvalidated = 1
	}

	private enum State
	{
		Start = 0,
		ExecutingSetPiece = 1,
		Docking = 2,
		Docked = 3,
		EnteringComplete = 4
	}

	private bool mWasSuccessful;

	private bool mCachedInvulnerableFlag;

	private HidingPlace mTarget;

	private Flags mFlags;

	private State mState;

	public HidingPlace Container
	{
		get
		{
			return mTarget;
		}
	}

	public bool HasFinishedEntering { get; private set; }

	public TaskEnter(TaskManager owner, TaskManager.Priority priority, Config flags, HidingPlace hidingPlace)
		: base(owner, priority, flags)
	{
		mTarget = hidingPlace;
		mState = State.Start;
		mFlags = Flags.Default;
		mWasSuccessful = false;
		if (mActor != null)
		{
			mCachedInvulnerableFlag = mActor.health.Invulnerable;
			mActor.health.Invulnerable = true;
		}
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			GameController instance = GameController.Instance;
			if (instance != null && instance.IsFirstPerson && mActor == instance.mFirstPersonActor)
			{
				instance.ExitFirstPerson(true, true);
			}
			SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
			setPieceLogic.SetModule(mTarget.EnterSetPiece);
			setPieceLogic.PlaceSetPiece(mTarget.SetPieceLocation.transform);
			setPieceLogic.SetObject(1, mTarget.Model);
			InheritableMovementParams moveParams = new InheritableMovementParams();
			new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType | Config.AbortIfSpotted | Config.ConsultParent, setPieceLogic, false, true, true, moveParams, 0f);
			mTarget.SetOccupier(mActor, false);
			mState = State.ExecutingSetPiece;
			break;
		}
		case State.ExecutingSetPiece:
			if (!mOwner.IsRunningTask(typeof(TaskSetPiece)))
			{
				if (mWasSuccessful)
				{
					mState = State.Docking;
				}
				else
				{
					mFlags |= Flags.TargetInvalidated;
				}
			}
			break;
		case State.Docking:
			mState = State.Docked;
			mActor.realCharacter.Docked = true;
			mActor.realCharacter.DockedContainer = mTarget;
			break;
		case State.Docked:
			if (mTarget.Occupier != mActor)
			{
				mState = State.EnteringComplete;
			}
			break;
		}
	}

	public override bool Consult(Task child)
	{
		return mActor.awareness.CanAnyEnemiesSeeMe();
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.TargetInvalidated) != 0)
		{
			return true;
		}
		return mState == State.EnteringComplete;
	}

	public override void Finish()
	{
		if (mTarget != null && mTarget.IsOccupied)
		{
			mTarget.ClearOccupier();
		}
		if (!(mActor != null))
		{
			return;
		}
		mActor.health.Invulnerable = mCachedInvulnerableFlag;
		mActor.baseCharacter.EnableNavMesh(true);
		mActor.tether.TetherToSelf();
		mActor.Command("ResetStand");
		mActor.realCharacter.Docked = false;
		mActor.realCharacter.DockedContainer = null;
		if (GameplayController.instance.IsSelectedLeader(mActor))
		{
			GameController.Instance.ZoomInAvailable = true;
			GameController.Instance.ZoomOutAvailable = false;
		}
		if (mActor.weapon != null)
		{
			IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(mActor.weapon.ActiveWeapon);
			if (weaponEquip != null && !weaponEquip.IsPuttingAway() && !weaponEquip.IsTakingOut())
			{
				weaponEquip.TakeOut(1f);
			}
		}
	}

	public override void OnSleep()
	{
		if (mState == State.Docking || mState == State.Docked)
		{
			new TaskExit(mOwner, base.Priority, Config.Default, mTarget);
			mFlags |= Flags.TargetInvalidated;
		}
	}

	public override void Command(string com)
	{
		if (com == "EnterOk")
		{
			mWasSuccessful = true;
		}
		else if (com == "Hide")
		{
			mActor.ShowHide(false);
			HasFinishedEntering = true;
		}
	}
}
