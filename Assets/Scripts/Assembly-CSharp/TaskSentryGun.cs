using UnityEngine;

public class TaskSentryGun : TaskTrackingRobot
{
	private const float kMaxEnemyDistSq = 400f;

	private Actor mPreviousTarget;

	private float mLastFovValue;

	public TaskSentryGun(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mLastFovValue = mActor.awareness.FoV;
	}

	public override void Update()
	{
		if (mActor.realCharacter == null)
		{
			return;
		}
		switch (mState)
		{
		case State.Search:
			DecrementWarmUpTime();
			RotateTurret();
			mCurrentTarget = FindTarget();
			if (mCurrentTarget != null)
			{
				if (AbleToTrackTarget())
				{
					SetState(State.WarmUp);
				}
			}
			else
			{
				ResetTrackDelay();
			}
			break;
		case State.WarmUp:
			if (WarmedUp())
			{
				SetState(State.TrackTarget);
			}
			break;
		case State.TrackTarget:
			if (mCurrentTarget != null)
			{
				if (ZonedInOnDirection(mCurrentTarget.Position - mActor.GetPosition(), mZoneInOnTargetTime))
				{
					SetState(State.ReactToTarget);
				}
			}
			else
			{
				SetState(State.Search);
			}
			break;
		case State.ReactToTarget:
			ReactToTarget();
			break;
		case State.CentreTurret:
			if (ZonedInOnDirection(mLookDirectionStart, mTimeToCentre))
			{
				SetState(State.Search);
			}
			break;
		case State.WarmDown:
			if (CooledDown())
			{
				mCurrentAngleAdjustment = 0f;
				SetState(State.CentreTurret);
			}
			break;
		case State.Reposition:
			if (ZonedInOnDirection(mRepositionLookDirectionStart, 1f))
			{
				mActor.transform.forward = mRepositionLookDirectionStart;
				mActor.awareness.LookDirection = mRepositionLookDirectionStart;
				mActor.realCharacter.mStartForward = mRepositionLookDirectionStart;
				mLookDirectionStart = mRepositionLookDirectionStart;
				mLookDirection = mRepositionLookDirectionStart;
				mLookDirectionBeforeCentreBegan = mRepositionLookDirectionStart;
				mCurrentAngleAdjustment = 0f;
				mActor.Command("AtEase");
				SetState(State.CentreTurret);
			}
			break;
		case State.HoldAim:
			if (!HoldingAim())
			{
				SetState(State.Search);
			}
			break;
		default:
			SetState(State.Search);
			break;
		}
		SetFactionColor();
	}

	protected override void RotateTurret()
	{
		if (mLastFovValue != mActor.awareness.FoV)
		{
			float f = Mathf.Sin(mCurrentAngleAdjustment) * (mLastFovValue * 0.5f) / (mActor.awareness.FoV * 0.5f);
			mCurrentAngleAdjustment = Mathf.Asin(f);
		}
		Quaternion quaternion = Quaternion.AngleAxis(Mathf.Sin(mCurrentAngleAdjustment) * (mActor.awareness.FoV * 0.5f), Vector3.up);
		mLookDirection = quaternion * mLookDirectionStart;
		mLookDirection.Normalize();
		mActor.transform.forward = mLookDirection;
		mActor.realCharacter.TurnToFaceDirection(mLookDirection);
		mActor.awareness.LookDirection = mLookDirectionStart;
		mCurrentAngleAdjustment += mSweepSpeed * Time.deltaTime;
		mLastFovValue = mActor.awareness.FoV;
	}

	protected override TrackingRobotTarget FindTarget()
	{
		Actor actor = mPreviousTarget;
		int num = GetScoreFor(mPreviousTarget);
		uint num2 = uint.MaxValue;
		if (mPreviousTarget != null)
		{
			num2 = ~mPreviousTarget.ident;
		}
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(mActor.awareness.EnemiesICanSee & GKM.AliveMask & num2 & GKM.CharacterTypeMask(CharacterType.Human));
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!a.realCharacter.IsMortallyWounded())
			{
				int scoreFor = GetScoreFor(a);
				if (scoreFor > num)
				{
					actor = a;
					num = scoreFor;
				}
			}
		}
		if (num <= -1000)
		{
			actor = null;
		}
		mPreviousTarget = actor;
		if (actor != null)
		{
			return new TrackingRobotTarget(actor);
		}
		return null;
	}

	private int GetScoreFor(Actor actor)
	{
		if (actor == null || actor.realCharacter.IsDead() || (mActor.awareness.EnemiesICanSee & actor.ident) == 0 || actor.realCharacter.IsMortallyWounded())
		{
			return -1000;
		}
		int num = 100;
		int num2 = 12;
		int num3 = 10;
		int num4 = -5;
		float num5 = Mathf.Clamp((actor.GetPosition().xz() - mActor.GetPosition().xz()).sqrMagnitude, 0f, 400f);
		float num6 = num5 / 400f;
		num += (int)((1f - num6) * 100f);
		if (actor.realCharacter.IsMortallyWounded())
		{
			num = 0;
		}
		if (actor.realCharacter.IsShootingAt(mActor))
		{
			num += num2;
		}
		if (!actor.awareness.IsInCover())
		{
			num += num3;
		}
		if (actor.awareness.IsInCover() && !actor.weapon.IsFiring())
		{
			num += num4;
		}
		return num;
	}

	public void ResetStartDirection(Vector3 direction)
	{
		direction.Normalize();
		mLookDirectionStart = direction;
	}
}
