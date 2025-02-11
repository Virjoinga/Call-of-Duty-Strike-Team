using UnityEngine;

public class TaskThrowGrenade : Task
{
	private enum State
	{
		Start = 0,
		Throwing = 1,
		Throw = 2,
		Cancel = 3
	}

	private bool mFinished;

	private bool mWasInCrouchCover;

	private Vector3 mTarget;

	private Vector3 mTargetDir;

	private State mState;

	public TaskThrowGrenade(TaskManager owner, TaskManager.Priority priority, Config flags, Vector3 target)
		: base(owner, priority, flags)
	{
		mTarget = target;
		mState = State.Start;
		CalculateTargetDir();
		mFinished = false;
		mWasInCrouchCover = false;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			BeginThrow();
			break;
		case State.Throwing:
			mActor.grenadeThrower.Throwing();
			break;
		case State.Throw:
			if (!mFinished)
			{
				VocalSFXHelper.GrenadeThrown(mActor);
				if (mActor.realCharacter != null)
				{
					GameplayController gameplayController = GameplayController.Instance();
					gameplayController.BroadcastEventGrenade(mActor);
					mActor.realCharacter.ThrowGrenade(mTarget);
				}
				mFinished = true;
			}
			break;
		case State.Cancel:
			if (!mFinished)
			{
				mActor.realCharacter.CancelThrowGrenade();
				mFinished = true;
			}
			break;
		}
	}

	public override void Command(string com)
	{
		if (com == "ThrowGrenade" && mState == State.Throwing)
		{
			mState = State.Throw;
		}
		else if (com == "CancelGrenade" && mState == State.Throwing)
		{
			mState = State.Cancel;
			mActor.grenadeThrower.Cancel();
		}
	}

	public override bool HasFinished()
	{
		return mFinished;
	}

	public override void Finish()
	{
		base.Finish();
		if (mWasInCrouchCover)
		{
			mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.CrouchCover);
		}
		if (mActor.realCharacter.IsDead() || mActor.realCharacter.IsMortallyWounded())
		{
			mActor.realCharacter.CancelThrowGrenade();
			mActor.grenadeThrower.Cancel();
		}
		mFinished = true;
	}

	private void CalculateTargetDir()
	{
		mTargetDir = (mTarget - mActor.GetPosition()).normalized;
	}

	private void BeginThrow()
	{
		if (mActor.Pose.ActiveModule == PoseModuleSharedData.Modules.CrouchCover)
		{
			mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.MoveAim);
			mWasInCrouchCover = true;
		}
		if (mActor.realCharacter != null)
		{
			mActor.realCharacter.ImposeLookDirection(mTargetDir);
			mActor.realCharacter.Command("ThrowGrenade");
		}
		mState = State.Throwing;
	}
}
