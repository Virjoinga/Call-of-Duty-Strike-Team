using UnityEngine;

public class TaskSecurityCamera : TaskTrackingRobot
{
	private enum TurnDir
	{
		Static = 0,
		Positive = 1,
		Negative = 2
	}

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private bool mEnabled = true;

	private bool mPaused;

	private TurnDir mPreviousTurnDir;

	private SoundManager.SoundInstance mRotateSoundLoopInst;

	public bool Enabled
	{
		get
		{
			return mEnabled;
		}
	}

	public bool Paused
	{
		get
		{
			return mPaused;
		}
	}

	public TaskSecurityCamera(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mTimeToCentre = 0.5f;
	}

	public override void Update()
	{
		if (mEnabled)
		{
			base.Update();
		}
		DoSound();
	}

	protected override void CleanUpSound()
	{
		StopRotateSoundLoop();
	}

	public void DisableCamera()
	{
		mActor.awareness.CanLook = false;
		mEnabled = false;
		HaloEffect component = base.Owner.GetComponent<HaloEffect>();
		if (component != null)
		{
			component.SetBlinkPattern(HaloEffect.BlinkPattern.Off);
		}
		DisableScanEffect();
		ResetDirection();
	}

	public void EnableCamera()
	{
		mActor.awareness.CanLook = true;
		mEnabled = true;
		HaloEffect component = base.Owner.GetComponent<HaloEffect>();
		if (component != null)
		{
			component.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkSlow);
		}
		EnableScanEffect();
		ResetDirection();
	}

	protected override void RotateTurret()
	{
		Quaternion quaternion = Quaternion.AngleAxis(Mathf.Sin(mCurrentAngleAdjustment) * (mActor.realCharacter.RangeDegree + 1f), Vector3.up);
		mLookDirection = quaternion * mLookDirectionStart;
		mLookDirection.Normalize();
		mActor.transform.forward = mLookDirection;
		mActor.realCharacter.TurnToFaceDirection(mLookDirection);
		mActor.awareness.LookDirection = mLookDirection;
		mCurrentAngleAdjustment += mSweepSpeed * Time.deltaTime;
		float num = Vector3.Dot(mLookDirectionStart, mLookDirection);
		TurnDir turnDir = TurnDir.Static;
		turnDir = ((num < mActor.realCharacter.Range) ? TurnDir.Positive : TurnDir.Negative);
		if (turnDir != mPreviousTurnDir)
		{
			if (!mPaused)
			{
				SetState(State.Pause);
				mPaused = true;
			}
		}
		else if (mPaused && turnDir == TurnDir.Negative)
		{
			mPaused = false;
		}
		mPreviousTurnDir = turnDir;
	}

	protected override void ReactToTarget()
	{
		TrackingRobotTarget trackingRobotTarget = FindTarget();
		if (trackingRobotTarget != null && !mCurrentTarget.SameAs(trackingRobotTarget))
		{
			mCurrentTarget = trackingRobotTarget;
			SetState(State.TrackTarget);
		}
		else if (mCurrentTarget != null)
		{
			SoundAlarm(null, mCurrentTarget.ActorTargeted.GetPosition());
			ForceFollow();
			mActor.awareness.LookDirection = mLookDirectionAfterCenter;
		}
		else
		{
			SetState(State.CentreTurret);
		}
	}

	public void SoundAlarm(Actor targetActor, Vector3 position)
	{
		if (!mEnabled)
		{
			return;
		}
		if (targetActor != null)
		{
			Vector3 rhs = position - mActor.transform.position;
			float num = Vector3.Dot(mLookDirectionStart, rhs);
			if (num >= mActor.realCharacter.Range && (mState != State.ReactToTarget || mState != State.TrackTarget))
			{
				mCurrentTarget = new TrackingRobotTarget(targetActor, position);
				SetState(State.TrackTarget);
			}
		}
		if ((bool)AlarmManager.Instance && !AlarmManager.Instance.AlarmSounding)
		{
			AlarmManager.Instance.Activate(position);
			if (!GameplayController.Instance().CameraAlarmSounded)
			{
				GameplayController.Instance().CameraAlarmSounded = true;
			}
		}
	}

	protected override TrackingRobotTarget FindTarget()
	{
		float num = float.MaxValue;
		Actor actor = null;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(mActor.awareness.EnemiesICanSee & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!a.awareness.IsInCover() || a.awareness.ChDefCharacterType != CharacterType.SecurityCamera)
			{
				float sqrMagnitude = (a.GetPosition().xz() - mActor.GetPosition().xz()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					actor = a;
				}
			}
		}
		if (actor == null)
		{
			return null;
		}
		return new TrackingRobotTarget(actor);
	}

	private void DoSound()
	{
		if (!mEnabled || mState != 0)
		{
			StopRotateSoundLoop();
		}
		else
		{
			PlayRotateSoundLoop();
		}
	}

	private void PlayRotateSoundLoop()
	{
		if (mActor != null && mRotateSoundLoopInst == null)
		{
			mRotateSoundLoopInst = SecurityCameraSFX.Instance.SecurityCamMoveLoop.Play(mActor.gameObject);
		}
	}

	private void StopRotateSoundLoop()
	{
		if (mRotateSoundLoopInst != null)
		{
			mRotateSoundLoopInst.Stop();
			mRotateSoundLoopInst = null;
		}
	}
}
