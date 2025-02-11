using System;
using UnityEngine;

public class TaskKnockedDown : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		Invalidated = 1
	}

	private enum State
	{
		Start = 0,
		OnTheFloor = 1,
		Recovered = 2
	}

	private static float TIME_ON_FLOOR = 5f;

	private Flags mFlags;

	private State mState;

	private float mKnockedDownTimeRemaining;

	public TaskKnockedDown(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mFlags = Flags.Default;
		mState = State.Start;
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			mKnockedDownTimeRemaining = TIME_ON_FLOOR;
			mState = State.OnTheFloor;
			break;
		case State.OnTheFloor:
			mKnockedDownTimeRemaining -= Time.deltaTime;
			if (mKnockedDownTimeRemaining <= 0f)
			{
				mState = State.Recovered;
			}
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.Invalidated) != 0)
		{
			return true;
		}
		return mState == State.Recovered;
	}
}
