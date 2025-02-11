using System;
using UnityEngine;

public class TaskSetPiece : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		SetPieceInvalidated = 1
	}

	private enum State
	{
		Start = 0,
		MovingToTarget = 1,
		Animating = 2,
		Complete = 3,
		Rescheduling = 4
	}

	public delegate void AboutToStartSetPiece();

	private const float kSetPieceTouchDist = 1.5f;

	private const float kSetPieceFaceDist = 2f;

	private const float kSetPieceTouchDist_Sq = 2.25f;

	private const float kSetPieceFaceDist_Sq = 4f;

	private Transform mStartTransform;

	private CMSetPiece mContextMenu;

	public AboutToStartSetPiece AboutToStartSetPieceCallback;

	private InheritableMovementParams mMoveParams;

	private Flags mFlags;

	private SetPieceLogic mSetPiece;

	private State mState;

	private bool mInPlace;

	private bool mPathTo;

	private bool mRouteTo;

	private float mStartTime;

	private Config mMoveToFlags;

	public bool DoNotInterrupt { get; set; }

	public TaskSetPiece(TaskManager owner, TaskManager.Priority priority, Config flags, SetPieceLogic setPiece, bool InPlace, bool pathTo, bool routeTo, InheritableMovementParams moveParams, float delay)
		: this(owner, priority, flags, setPiece, InPlace, pathTo, routeTo, moveParams, delay, null)
	{
	}

	public TaskSetPiece(TaskManager owner, TaskManager.Priority priority, Config flags, SetPieceLogic setPiece, bool InPlace, bool pathTo, bool routeTo, InheritableMovementParams moveParams, float delay, CMSetPiece contextMenu)
		: base(owner, priority, flags)
	{
		mMoveParams = moveParams;
		mSetPiece = setPiece;
		mState = State.Start;
		mFlags = Flags.Default;
		mInPlace = InPlace;
		mPathTo = pathTo;
		mRouteTo = routeTo;
		mStartTime = Time.time + delay;
		mContextMenu = contextMenu;
		mMoveToFlags = flags & (Config.AbortOnAlert | Config.AbortOnVisibleEnemy | Config.AbortOnUnobstructedEnemy | Config.AbortOnUnobstructedTarget | Config.AbortIfSpotted);
	}

	public override void Update()
	{
		if (mSetPiece == null)
		{
			mState = State.Complete;
			return;
		}
		switch (mState)
		{
		case State.Start:
		{
			if (mStartTime > Time.time)
			{
				break;
			}
			if (mActor != null && mActor.realCharacter.IsAimingDownSights)
			{
				mActor.realCharacter.IsAimingDownSights = false;
			}
			bool flag = mOwner.IsRunningTask<TaskExit>();
			TaskCarry taskCarry = (TaskCarry)mOwner.GetRunningTask(typeof(TaskCarry));
			if (!flag && taskCarry != null && !taskCarry.ForceDropDone(this))
			{
				break;
			}
			if (mInPlace)
			{
				mSetPiece.PlaceSetPiece(mActor.transform);
				StartSetPiece();
			}
			else if (mPathTo)
			{
				mStartTransform = mSetPiece.GetCharacterStartNode(0);
				if (mStartTransform != null)
				{
					mMoveParams.mDestination = mStartTransform.position;
					mMoveParams.FinalLookDirection = WorldHelper.UfM_Forward(mStartTransform);
					mMoveParams.DestinationThreshold = 0.375f;
					mMoveParams.mGunAwayAtEnd = mSetPiece.GunAway(0);
					mMoveParams.forceFaceForwards = true;
					float sqrMagnitude = (mMoveParams.mDestination - mActor.GetPosition()).sqrMagnitude;
					if (sqrMagnitude > 9f / 64f)
					{
						if (mRouteTo)
						{
							mOwner.CancelTasksOfEqualOrHigherPriority(typeof(TaskRouteTo), base.Priority);
							new TaskRouteTo(mOwner, base.Priority, Config.ConsultParent | mMoveToFlags, mMoveParams);
						}
						else
						{
							new TaskMoveTo(mOwner, base.Priority, Config.ClearCurrentTypeEqualOrHigher | mMoveToFlags, mMoveParams);
						}
					}
					mState = State.MovingToTarget;
				}
				else
				{
					StartSetPiece();
				}
			}
			else
			{
				StartSetPiece();
			}
			break;
		}
		case State.MovingToTarget:
		{
			float sqrMagnitude2 = (mMoveParams.mDestination - mActor.GetPosition()).sqrMagnitude;
			if (sqrMagnitude2 <= 2.25f)
			{
				StartSetPiece();
			}
			else
			{
				mFlags |= Flags.SetPieceInvalidated;
			}
			break;
		}
		case State.Animating:
			if (mSetPiece.HasActorFinished(0))
			{
				EndSetPiece();
			}
			break;
		}
	}

	public void Cancel()
	{
		if (mState == State.Start || mState == State.MovingToTarget)
		{
			mOwner.CancelTasks(typeof(TaskMoveTo));
			mState = State.Complete;
		}
	}

	public bool CanReschedule()
	{
		return mState != State.Animating;
	}

	public void Reschedule()
	{
		mOwner.CancelTasks(typeof(TaskMoveTo));
		mState = State.Rescheduling;
	}

	public void AddToMoveFlags(Config newFlag)
	{
		mMoveToFlags |= newFlag;
	}

	public void CleanUpSetPiece()
	{
		InteractionsManager.Instance.FinishedAction(mSetPiece.SPModule.gameObject, this);
		ForceFinish();
	}

	private void ForceFinish()
	{
		mFlags |= Flags.SetPieceInvalidated;
	}

	public override void Finish()
	{
		if (mSetPiece != null && mSetPiece.Mortal)
		{
			UnityEngine.Object.Destroy(mSetPiece.gameObject);
		}
	}

	public override bool HasFinished()
	{
		if (mState == State.Complete)
		{
			mOwner.Feedback(TaskManager.TaskResult.Complete, null);
			return true;
		}
		if (mState == State.Rescheduling)
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "Reschedule");
			return true;
		}
		if ((mFlags & Flags.SetPieceInvalidated) != 0)
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "Failed");
			return true;
		}
		if (mState != State.Animating && CheckConfigFlagsFinished())
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "Enemy");
			return true;
		}
		if ((bool)AlarmManager.Instance)
		{
			TaskUseAlarmPanel taskUseAlarmPanel = (TaskUseAlarmPanel)mOwner.GetRunningTask(typeof(TaskUseAlarmPanel));
			if (taskUseAlarmPanel != null && AlarmManager.Instance.AlarmSounding && mState != State.Animating)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPlayingSetPiece()
	{
		return mState == State.Animating;
	}

	private void StartSetPiece()
	{
		if (AboutToStartSetPieceCallback != null)
		{
			AboutToStartSetPieceCallback();
		}
		mSetPiece.SetActor_IndexOnlyCharacters(0, mActor);
		mSetPiece.PlaySetPiece();
		mSetPiece.ShowHUD(false);
		if (mSetPiece.SPModule != null)
		{
			InteractionsManager.Instance.ChangeActionType(mSetPiece.SPModule.gameObject, this, mSetPiece.SPModule.ActionType);
		}
		mState = State.Animating;
	}

	private void EndSetPiece()
	{
		mState = State.Complete;
		mSetPiece.ShowHUD(true);
		mSetPiece.WasSuccessful = true;
		if (mContextMenu != null)
		{
			mContextMenu.IsComplete = true;
		}
	}

	public void Skip()
	{
		if (mSetPiece != null)
		{
			mSetPiece.SPModule.Skip();
			mSetPiece.SPModule.PostSkip();
		}
	}
}
