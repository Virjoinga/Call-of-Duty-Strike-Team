using System;
using UnityEngine;

public class TaskStopAndStare : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		InDirection = 1,
		ForceFacing = 2
	}

	private Flags mFlags;

	private float mSeconds;

	private Transform mStareAt;

	public bool UseForwardDirection
	{
		get
		{
			return (mFlags & Flags.InDirection) != 0;
		}
		set
		{
			if (value)
			{
				mFlags |= Flags.InDirection;
			}
		}
	}

	public TaskStopAndStare(TaskManager owner, TaskManager.Priority priority, Config flags, float seconds, Transform stareAt, bool forceFacing)
		: base(owner, priority, flags)
	{
		mFlags = Flags.Default;
		if (forceFacing)
		{
			mFlags |= Flags.ForceFacing;
		}
		mSeconds = seconds;
		mStareAt = stareAt;
		SetupState();
	}

	public override void OnResume()
	{
		SetupState();
	}

	public override void OnSleep()
	{
		TearDownState();
	}

	public override void Finish()
	{
		TearDownState();
	}

	public override void Update()
	{
		mSeconds -= Time.deltaTime;
		if (UseForwardDirection)
		{
			mActor.realCharacter.TurnToFaceDirection(mStareAt.forward);
		}
		else
		{
			mActor.realCharacter.TurnToFacePosition(mStareAt.position);
		}
		if ((mFlags & Flags.ForceFacing) != 0)
		{
			mActor.Pose.FaceLookDirection();
		}
	}

	public override bool HasFinished()
	{
		if (mActor.realCharacter != null && (base.ConfigFlags & Config.AbortOnAlert) != 0 && mActor.behaviour.InActiveAlertState())
		{
			mOwner.Feedback(TaskManager.TaskResult.Aborted, "Alert");
			return true;
		}
		return mSeconds <= 0f;
	}

	private void SetupState()
	{
	}

	private void TearDownState()
	{
	}
}
