using System;
using UnityEngine;

public class TaskOpen : Task
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
		Opening = 2,
		OpeningComplete = 3
	}

	private BuildingDoor mTarget;

	private Flags mFlags;

	private State mState;

	public TaskOpen(TaskManager owner, TaskManager.Priority priority, Config flags, BuildingDoor door)
		: base(owner, priority, flags)
	{
		mTarget = door;
		mState = State.Start;
		mFlags = Flags.Default;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			mActor.navAgent.walkableMask |= mTarget.m_NavGate.m_Interface.NavLayerID;
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible, mTarget.transform.position);
			new TaskRouteTo(mActor.tasks, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType, moveParams, null);
			mState = State.Opening;
			break;
		}
		case State.Opening:
			mState = State.OpeningComplete;
			break;
		case State.MovingToTarget:
			break;
		}
	}

	public override void Finish()
	{
		if (mTarget.m_Interface.State != BuildingDoor.DoorSate.Open)
		{
			mActor.navAgent.walkableMask &= ~mTarget.m_NavGate.m_Interface.NavLayerID;
		}
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
		return mState == State.OpeningComplete;
	}

	public bool AtTarget()
	{
		Vector3 position = mTarget.DoorMesh.transform.position;
		Vector3 vector = position - mActor.GetPosition();
		vector.y = 0f;
		if (vector.sqrMagnitude < 1.5f && !mActor.tasks.IsRunningTask(typeof(TaskMoveTo)))
		{
			return true;
		}
		return false;
	}
}
