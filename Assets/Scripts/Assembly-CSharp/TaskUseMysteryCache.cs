using System;

public class TaskUseMysteryCache : Task
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

	private CMMysteryCache mTarget;

	private MysteryCache mTargetMysteryCache;

	private State mState;

	private Flags mFlags;

	private bool mWasSuccessful;

	private TaskSetPiece mTaskSetPiece;

	private bool mCachedInvulnerableFlag;

	public TaskUseMysteryCache(TaskManager owner, TaskManager.Priority priority, Config flags, CMMysteryCache target)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mState = State.Start;
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
			if (mTarget != null && mTarget.UseSetPiece != null)
			{
				mTargetMysteryCache = mTarget.GetComponent<MysteryCache>();
				if (mTargetMysteryCache != null)
				{
					SetPieceLogic setPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
					setPieceLogic.SetModule(mTarget.UseSetPiece);
					setPieceLogic.PlaceSetPiece(mTargetMysteryCache.transform);
					setPieceLogic.SetObject(1, mTarget.CrateModel);
					InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible, setPieceLogic.GetCharacterStartNode(0).position);
					inheritableMovementParams.FinalLookAtObject = mTargetMysteryCache.gameObject;
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
			PickUpMystery();
		}
		if (mActor != null)
		{
			mActor.health.Invulnerable = mCachedInvulnerableFlag;
		}
	}

	public override void Command(string com)
	{
		if (com == "PickedUpMystery")
		{
			PickUpMystery();
		}
		else if (com == "PlayPickUpSound")
		{
			GMGSFX.Instance.MysteryCacheUsed.Play2D();
		}
	}

	private void PickUpMystery()
	{
		if (mTarget != null)
		{
			MysteryCache componentInChildren = mTarget.GetComponentInChildren<MysteryCache>();
			if (componentInChildren != null)
			{
				componentInChildren.PickUpMystery();
			}
			mWasSuccessful = true;
		}
	}
}
