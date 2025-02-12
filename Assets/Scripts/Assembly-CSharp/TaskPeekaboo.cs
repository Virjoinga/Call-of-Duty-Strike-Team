using System;
using UnityEngine;

public class TaskPeekaboo : Task
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
		MovingToTarget = 1,
		PeekingOn = 2,
		Peeking = 3,
		PeekingOff = 4,
		PeekComplete = 5
	}

	private BuildingWindow mWindow;

	private Flags mFlags;

	private State mState;

	private float mPeekDuration;

	private float mCurrentTimeValue;

	private bool mCachedActorInvulnerability;

	private bool mCachedActorInvulnerabilityToExplosions;

	private SetPieceLogic mSetPieceLogic;

	public TaskPeekaboo(TaskManager owner, TaskManager.Priority priority, BuildingWindow window, Config flags)
		: base(owner, priority, flags)
	{
		mWindow = window;
		mState = State.Start;
		mFlags = Flags.Default;
		mPeekDuration = 8f;
		mCurrentTimeValue = 0f;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			Vector3 position = mWindow.transform.position;
			Vector3 vector = position - mActor.GetPosition();
			vector.y = 0f;
			if (vector.sqrMagnitude > 0f)
			{
				position -= vector.normalized * 1f;
				SetPieceModule actionSetPieceModule2 = mActor.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kPeekEnter);
				if (actionSetPieceModule2 != null)
				{
					mSetPieceLogic = mActor.realCharacter.CreateSetPieceLogic();
					mSetPieceLogic.SetModule(actionSetPieceModule2);
					mSetPieceLogic.Mortal = false;
					mSetPieceLogic.PlaceSetPiece(mWindow.transform);
					InheritableMovementParams moveParams2 = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
					new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType, mSetPieceLogic, false, true, false, moveParams2, 0f);
					mCachedActorInvulnerability = mActor.health.Invulnerable;
					mCachedActorInvulnerabilityToExplosions = mActor.health.InvulnerableToExplosions;
					mActor.health.Invulnerable = true;
					mActor.health.InvulnerableToExplosions = true;
				}
			}
			mState = State.MovingToTarget;
			break;
		}
		case State.MovingToTarget:
			if (!mOwner.IsRunningTask(typeof(TaskSetPiece)))
			{
				mState = State.PeekingOn;
			}
			break;
		case State.PeekingOn:
			mWindow.Open();
			if (mWindow.GetComponent<Renderer>() != null)
			{
				mWindow.GetComponent<Renderer>().enabled = false;
			}
			InteractionsManager.Instance.ChangeActionType(mActor.gameObject, this, ActionComponent.ActionType.MustSee);
			mState = State.Peeking;
			break;
		case State.Peeking:
			mCurrentTimeValue += Time.deltaTime;
			if (mCurrentTimeValue >= mPeekDuration)
			{
				mState = State.PeekingOff;
				mCurrentTimeValue = 0f;
				InteractionsManager.Instance.ChangeActionType(mActor.gameObject, this, ActionComponent.ActionType.Ignore);
				SetPieceModule actionSetPieceModule = mActor.realCharacter.mNavigationSetPiece.GetActionSetPieceModule(NavigationSetPieceLogic.ActionSetPieceType.kPeekExit);
				if (actionSetPieceModule != null)
				{
					mSetPieceLogic.SetModule(actionSetPieceModule);
					mSetPieceLogic.Mortal = false;
					mSetPieceLogic.PlaceSetPiece(mWindow.transform);
					InheritableMovementParams moveParams = new InheritableMovementParams();
					new TaskSetPiece(mOwner, base.Priority, Config.ClearAllCurrentType, mSetPieceLogic, false, true, false, moveParams, 0f);
				}
			}
			break;
		case State.PeekingOff:
			if (!mOwner.IsRunningTask(typeof(TaskSetPiece)))
			{
				Debug.Log("StopActivePeeking triggered");
				StopActivePeeking();
			}
			break;
		}
	}

	public override void Finish()
	{
		if (mActor != null)
		{
			mActor.health.Invulnerable = mCachedActorInvulnerability;
			mActor.health.InvulnerableToExplosions = mCachedActorInvulnerabilityToExplosions;
		}
		StopActivePeeking();
		UnityEngine.Object.Destroy(mSetPieceLogic);
	}

	public override void OnSleep()
	{
		StopActivePeeking();
	}

	public override bool HasFinished()
	{
		if (mState == State.Start)
		{
			return false;
		}
		if ((mFlags & Flags.TargetInvalidated) != 0)
		{
			return true;
		}
		return mState == State.PeekComplete;
	}

	private void StopActivePeeking()
	{
		Debug.Log("StopActivePeeking");
		if (mState >= State.PeekingOn && mState != State.PeekComplete)
		{
			mState = State.PeekComplete;
			mWindow.Close();
			if (mWindow.GetComponent<Renderer>() != null)
			{
				mWindow.GetComponent<Renderer>().enabled = true;
			}
		}
	}

	public bool AtTarget()
	{
		Vector3 position = mWindow.transform.position;
		Vector3 vector = position - mActor.GetPosition();
		vector.y = 0f;
		if (vector.sqrMagnitude < 1.5f && !mActor.tasks.IsRunningTask(typeof(TaskMoveTo)))
		{
			return true;
		}
		return false;
	}

	public override void Command(string com)
	{
		if (com == "DoScreenStaticEffect")
		{
			AnimatedScreenBackground.Instance.Deactivate();
		}
	}
}
