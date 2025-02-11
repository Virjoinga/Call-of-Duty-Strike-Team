using System;

public class TaskUseAmmoCache : Task
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
		UsingCache = 1,
		UseComplete = 2
	}

	private CMAmmoCache mTarget;

	private AmmoCache mTargetAmmoCache;

	private State mState;

	private Flags mFlags;

	private bool mWasSuccessful;

	private TaskSetPiece mTaskSetPiece;

	private bool mCachedInvulnerableFlag;

	private bool mCachedMysteryBoxInvulnerability;

	public TaskUseAmmoCache(TaskManager owner, TaskManager.Priority priority, Config flags, CMAmmoCache target)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mState = State.Start;
		if (mActor != null)
		{
			mCachedInvulnerableFlag = mActor.health.Invulnerable;
			mCachedMysteryBoxInvulnerability = mActor.MysteryBoxInvulnerability;
			mActor.health.Invulnerable = true;
		}
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			if (mTarget != null && mTarget.UseSetPiece != null)
			{
				mTargetAmmoCache = mTarget.GetComponent<AmmoCache>();
				if (mTargetAmmoCache != null)
				{
					SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
					setPieceLogic.SetModule(mTarget.UseSetPiece);
					setPieceLogic.PlaceSetPiece(mTargetAmmoCache.transform);
					setPieceLogic.SetObject(1, mTarget.CrateModel);
					InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible, setPieceLogic.GetCharacterStartNode(0).position);
					inheritableMovementParams.FinalLookAtObject = mTargetAmmoCache.gameObject;
					inheritableMovementParams.forceFaceForwards = true;
					mTaskSetPiece = new TaskSetPiece(mOwner, base.Priority, base.ConfigFlags, setPieceLogic, false, false, false, inheritableMovementParams, 0f);
					mTarget.Deactivate();
					mState = State.UsingCache;
				}
				else
				{
					mFlags |= Flags.TargetInvalidated;
				}
			}
			else
			{
				mFlags |= Flags.TargetInvalidated;
			}
			break;
		case State.UsingCache:
			if (!mOwner.IsRunningTask<TaskSetPiece>())
			{
				if (mWasSuccessful)
				{
					mState = State.UseComplete;
				}
				else
				{
					mFlags |= Flags.TargetInvalidated;
				}
			}
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.TargetInvalidated) != 0)
		{
			return true;
		}
		return mState == State.UseComplete;
	}

	public override void Finish()
	{
		if (mTaskSetPiece != null && !mTaskSetPiece.HasFinished())
		{
			mTaskSetPiece.Skip();
			PickUpAmmo();
		}
		if (mActor != null)
		{
			if (mCachedInvulnerableFlag && mCachedMysteryBoxInvulnerability && !mActor.health.Invulnerable)
			{
				mCachedInvulnerableFlag = false;
			}
			mActor.health.Invulnerable = mCachedInvulnerableFlag;
		}
	}

	public override void Command(string com)
	{
		if (com == "PickedUpAmmo")
		{
			PickUpAmmo();
		}
		else if (com == "PlayPickUpSound")
		{
			GMGSFX.Instance.AmmoCacheUsed.Play2D();
		}
	}

	private void PickUpAmmo()
	{
		if (mTarget != null)
		{
			AmmoCache componentInChildren = mTarget.GetComponentInChildren<AmmoCache>();
			if (componentInChildren != null)
			{
				componentInChildren.PickUpAmmo();
			}
			mWasSuccessful = true;
		}
	}
}
