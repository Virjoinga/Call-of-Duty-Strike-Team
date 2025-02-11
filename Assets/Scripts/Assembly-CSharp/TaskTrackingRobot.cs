using System;
using UnityEngine;

public abstract class TaskTrackingRobot : Task
{
	protected enum State
	{
		Search = 0,
		WarmUp = 1,
		TrackTarget = 2,
		ReactToTarget = 3,
		CentreTurret = 4,
		WarmDown = 5,
		HoldAim = 6,
		Reposition = 7,
		Pause = 8
	}

	public float mTimeToCentre = 0.25f;

	public float mZoneInOnTargetTime = 0.5f;

	private HaloEffect mHalo;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private FactionHelper.Category mPreviousFaction;

	protected State mState;

	protected TrackingRobotTarget mCurrentTarget;

	protected Vector3 mLookDirection;

	protected Vector3 mRepositionLookDirectionStart;

	protected float mCurrentAngleAdjustment;

	protected float mWarmUpTime = 0.5f;

	private float mTrackDelayTime;

	protected float mTrackDelayTimer;

	protected float mCoolDownTime = 0.5f;

	private float mTimeToWarmUp;

	private float mTimeToCoolDown;

	private float mCurrentHoldAimTime;

	private float mHoldAimTime = 2f;

	protected float mSweepSpeed = 0.2f;

	private float mCurrentCentreTurretTime;

	protected Vector3 mLookDirectionBeforeCentreBegan;

	protected Vector3 mLookDirectionAfterCenter;

	protected Vector3 mLookDirectionStart;

	private LaserScanLineEffect mLaserEffect;

	protected float mPauseTimeLeft = 5f;

	protected float mPauseTimeRight = 5f;

	protected float mCurrentPauseTime;

	protected int mRotateCount = -1;

	public Vector3 StartLookDirection
	{
		get
		{
			return mLookDirectionStart;
		}
	}

	public Vector3 LookDirection
	{
		get
		{
			return mLookDirection;
		}
	}

	public float CurrentAngleAdjustment
	{
		get
		{
			return mCurrentAngleAdjustment;
		}
	}

	public float PauseTimeLeft
	{
		get
		{
			return mPauseTimeLeft;
		}
		set
		{
			mPauseTimeLeft = value;
		}
	}

	public float PauseTimeRight
	{
		get
		{
			return mPauseTimeRight;
		}
		set
		{
			mPauseTimeRight = value;
		}
	}

	public float SweepSpeed
	{
		get
		{
			return mSweepSpeed;
		}
		set
		{
			mSweepSpeed = value;
		}
	}

	public float WarmUpTime
	{
		get
		{
			return mTimeToWarmUp;
		}
		set
		{
			mTimeToWarmUp = value;
		}
	}

	public float CoolDownTime
	{
		get
		{
			return mTimeToCoolDown;
		}
		set
		{
			mTimeToCoolDown = value;
		}
	}

	public TaskTrackingRobot(TaskManager owner, TaskManager.Priority priority, Config flags)
		: base(owner, priority, flags)
	{
		mCurrentTarget = null;
		mLookDirection = mActor.transform.forward.normalized;
		mLookDirectionBeforeCentreBegan = mLookDirection;
		mLookDirectionStart = mLookDirection;
		mRepositionLookDirectionStart = mLookDirection;
		mCurrentAngleAdjustment = 0f;
		mCurrentCentreTurretTime = 0f;
		if (mActor.awareness.ChDefCharacterType == CharacterType.SecurityCamera)
		{
			mActor.awareness.visible = false;
		}
		mHalo = base.Owner.GetComponent<HaloEffect>();
		if (mHalo != null)
		{
			mHalo.Size = 1f;
		}
		SetState(State.Search);
		FireAtWillComponent component = base.Owner.GetComponent<FireAtWillComponent>();
		if (component != null)
		{
			component.enabled = false;
		}
		AuditoryAwarenessComponent component2 = base.Owner.GetComponent<AuditoryAwarenessComponent>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
	}

	public override void Destroy()
	{
		CleanUpSound();
		base.Destroy();
	}

	public void CreateScanEffect()
	{
		if (mLaserEffect == null)
		{
			float f = mActor.awareness.FoV * 0.5f * ((float)Math.PI / 180f);
			float num = Mathf.Cos(f);
			float num2 = Mathf.Sin(f);
			Vector3 vector = new Vector3(0f, 0f, 1f);
			Vector3 vector2 = new Vector3(vector.x * num - vector.z * num2, 0f, vector.x * num2 + vector.z * num);
			vector2 *= mActor.awareness.VisionRange;
			GameObject gameObject = new GameObject();
			gameObject.name = "ScanLineEffectObject";
			gameObject.transform.parent = mActor.gameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			mLaserEffect = (LaserScanLineEffect)gameObject.AddComponent(typeof(LaserScanLineEffect));
			bool animate = false;
			if (mActor.awareness.ChDefCharacterType == CharacterType.SentryGun)
			{
				vector2.x = -0.5f;
			}
			Vector3 offset = Vector3.zero;
			Vector3 vector3 = new Vector3(0f, 1.2f, 0f);
			Vector3 vector4 = new Vector3(0f, 0.05f, 0.3f);
			if (mActor.awareness.ChDefCharacterType == CharacterType.SentryGun)
			{
				offset = vector3;
			}
			else if (mActor.awareness.ChDefCharacterType == CharacterType.SecurityCamera)
			{
				offset = vector4;
			}
			if (mLaserEffect != null)
			{
				mLaserEffect.Setup(vector2.x, mActor.awareness.VisionRange, offset, animate);
			}
			mPreviousFaction = mActor.awareness.faction;
		}
	}

	protected void SetFactionColor()
	{
		if (mActor.awareness.faction == FactionHelper.Category.Enemy && (bool)mActor.realCharacter)
		{
			EnemyBlip enemyBlip = mActor.realCharacter.HudMarker as EnemyBlip;
			if (enemyBlip != null)
			{
				enemyBlip.SetToSentryGunBlip();
			}
		}
		if (mActor.awareness.faction == mPreviousFaction || !(mLaserEffect != null))
		{
			return;
		}
		mPreviousFaction = mActor.awareness.faction;
		if (mPreviousFaction == FactionHelper.Category.Player)
		{
			if ((bool)mActor.realCharacter)
			{
				EnemyBlip enemyBlip2 = mActor.realCharacter.HudMarker as EnemyBlip;
				if (enemyBlip2 != null)
				{
					enemyBlip2.SetToFriendlyBlip();
				}
			}
			if ((bool)mLaserEffect)
			{
				mLaserEffect.SetLaserColour(ColourChart.FriendlyBlip);
			}
			if (mHalo != null)
			{
				mHalo.SetColour(HaloEffect.HaloColour.Green);
			}
		}
		else
		{
			if ((bool)mLaserEffect)
			{
				mLaserEffect.SetLaserColour(ColourChart.EnemyBlip);
			}
			if (mHalo != null)
			{
				mHalo.SetColour(HaloEffect.HaloColour.Red);
			}
		}
	}

	protected void DestroyScanEffect()
	{
		if (mLaserEffect != null)
		{
			UnityEngine.Object.Destroy(mLaserEffect.gameObject);
		}
	}

	public void ResetDirection()
	{
		mCurrentAngleAdjustment = 0f;
		mCurrentCentreTurretTime = 0f;
		GameObject gameObject = new GameObject();
		gameObject.name = "ScanLineEffectObject";
		gameObject.transform.parent = mActor.gameObject.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		mLookDirection = mLookDirectionStart;
		mLookDirectionBeforeCentreBegan = mLookDirectionStart;
	}

	public override bool HasFinished()
	{
		return false;
	}

	public override void Finish()
	{
		DestroyScanEffect();
		if (mHalo != null)
		{
			mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.Off);
		}
		CleanUpSound();
	}

	protected virtual void CleanUpSound()
	{
		if (mActor != null && mActor.weapon != null && mActor.weapon.PrimaryWeapon != null)
		{
			Weapon_Minigun weapon_Minigun = mActor.weapon.PrimaryWeapon as Weapon_Minigun;
			if (weapon_Minigun != null)
			{
				weapon_Minigun.CleanUpSound();
			}
		}
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
		case State.Pause:
			if (PauseComplete())
			{
				SetState(State.Search);
			}
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
		case State.TrackTarget:
			if (mCurrentTarget != null)
			{
				if (ZonedInOnDirection(mCurrentTarget.Position - mActor.GetPosition(), mZoneInOnTargetTime))
				{
					mLookDirectionAfterCenter = mLookDirection;
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
				SetState(State.WarmDown);
			}
			break;
		case State.WarmDown:
			if (CooledDown())
			{
				mCurrentAngleAdjustment = 0f;
				SetState(State.Search);
			}
			break;
		case State.Reposition:
			if (ZonedInOnDirection(mRepositionLookDirectionStart, 1f))
			{
				mActor.realCharacter.mStartForward = mRepositionLookDirectionStart;
				mLookDirectionStart = mRepositionLookDirectionStart;
				mLookDirection = mRepositionLookDirectionStart;
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

	protected void SetState(State newState)
	{
		mState = newState;
		switch (mState)
		{
		case State.Search:
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.On);
			}
			break;
		case State.WarmUp:
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkSlow);
			}
			break;
		case State.ReactToTarget:
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkMedium);
			}
			break;
		case State.TrackTarget:
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkMedium);
			}
			mLookDirectionBeforeCentreBegan = mActor.transform.forward;
			mCurrentCentreTurretTime = 0f;
			StopFiring();
			break;
		case State.CentreTurret:
		case State.Pause:
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkSlow);
			}
			mLookDirectionBeforeCentreBegan = mActor.transform.forward;
			mCurrentCentreTurretTime = 0f;
			StopFiring();
			mCurrentPauseTime = 0f;
			mRotateCount++;
			break;
		case State.WarmDown:
			mActor.Command("MoveToBegin");
			ResetCoolDownTime();
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.BlinkSlow);
			}
			StopFiring();
			break;
		case State.Reposition:
			mActor.Command("Ragdoll");
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.On);
			}
			mCurrentCentreTurretTime = 0f;
			break;
		case State.HoldAim:
			mActor.Command("MoveToBegin");
			ResetHoldAimTime();
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.On);
			}
			break;
		default:
			if (mHalo != null)
			{
				mHalo.SetBlinkPattern(HaloEffect.BlinkPattern.Off);
			}
			break;
		}
	}

	protected void StopFiring()
	{
		if (mActor != null)
		{
			if (mActor.realCharacter != null)
			{
				mActor.realCharacter.SetTarget(null);
			}
			if (mActor.weapon != null)
			{
				mActor.weapon.SetTarget(null);
				mActor.weapon.SetAiming(false);
				mActor.weapon.SetTrigger(false);
			}
		}
	}

	protected void StartFiring()
	{
		if (mActor != null && mActor.weapon != null)
		{
			if (mCurrentTarget.ActorTargeted != null)
			{
				mActor.weapon.SetAiming(true);
			}
			if (mActor.realCharacter != null)
			{
				mActor.realCharacter.ShootAtTarget(mCurrentTarget.ActorTargeted);
			}
			mActor.weapon.SetTarget(mCurrentTarget.ActorTargeted);
			mActor.weapon.SetTrigger(true);
		}
	}

	public void ForceFaceForward()
	{
		SetState(State.CentreTurret);
	}

	public void ForceRepositionForwardFacing(Vector3 direction)
	{
		direction.Normalize();
		mRepositionLookDirectionStart = direction;
		mActor.awareness.LookDirection = mRepositionLookDirectionStart;
		SetState(State.Reposition);
	}

	protected void DecrementWarmUpTime()
	{
		mTimeToWarmUp -= Time.deltaTime;
		if (mTimeToWarmUp < 0f)
		{
			mTimeToWarmUp = 0f;
		}
	}

	protected virtual void RotateTurret()
	{
		mActor.transform.forward = mLookDirection;
		mActor.realCharacter.TurnToFaceDirection(mLookDirection);
		mActor.awareness.LookDirection = mLookDirection;
	}

	protected virtual TrackingRobotTarget FindTarget()
	{
		float num = float.MaxValue;
		Actor actor = null;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(mActor.awareness.EnemiesICanSee & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			float sqrMagnitude = (a.GetPosition().xz() - mActor.GetPosition().xz()).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				actor = a;
			}
		}
		if (actor == null)
		{
			return null;
		}
		return new TrackingRobotTarget(actor);
	}

	protected bool ZonedInOnDirection(Vector3 direction, float time)
	{
		mCurrentCentreTurretTime += Time.deltaTime;
		float num = mCurrentCentreTurretTime / time;
		direction.Normalize();
		mLookDirection = mLookDirectionBeforeCentreBegan + (direction - mLookDirectionBeforeCentreBegan) * num;
		IgnoreAllEnemies();
		mActor.transform.forward = mLookDirection;
		mActor.realCharacter.TurnToFaceDirection(mLookDirection);
		if (!(this is TaskSecurityCamera))
		{
			mActor.awareness.LookDirection = mLookDirectionStart;
		}
		return num > 1f || mLookDirection == direction;
	}

	protected bool WarmedUp()
	{
		if (mActor.baseCharacter.IsDead())
		{
			return false;
		}
		mTimeToWarmUp += Time.deltaTime;
		if (mTimeToWarmUp >= mWarmUpTime)
		{
			mCurrentTarget = FindTarget();
			if (mCurrentTarget != null)
			{
				return true;
			}
			SetState(State.Search);
			return false;
		}
		IgnoreAllEnemies();
		return false;
	}

	protected bool PauseComplete()
	{
		if (mActor.baseCharacter.IsDead())
		{
			return false;
		}
		mCurrentPauseTime += Time.deltaTime;
		float num = 0f;
		if (mRotateCount != 0)
		{
			num = ((mRotateCount % 2 != 0) ? mPauseTimeRight : mPauseTimeLeft);
		}
		if (mCurrentPauseTime >= num)
		{
			SetState(State.Search);
			return true;
		}
		return false;
	}

	protected bool CooledDown()
	{
		mTimeToCoolDown += Time.deltaTime;
		if (mTimeToCoolDown > mCoolDownTime)
		{
			mCurrentAngleAdjustment = Vector3.Dot(Vector3.right, mActor.awareness.LookDirection);
			mCurrentAngleAdjustment *= -1f;
			mCurrentAngleAdjustment *= 2f;
			return true;
		}
		IgnoreAllEnemies();
		return false;
	}

	private void IgnoreAllEnemies()
	{
	}

	protected virtual void ReactToTarget()
	{
		TrackingRobotTarget trackingRobotTarget = FindTarget();
		if (trackingRobotTarget != null)
		{
			if (!mCurrentTarget.SameAs(trackingRobotTarget))
			{
				mCurrentTarget = trackingRobotTarget;
				SetState(State.TrackTarget);
				return;
			}
			if (mCurrentTarget.ActorTargeted != null)
			{
				if (mActor.weapon != null && mCurrentTarget.ActorTargeted != null)
				{
					Vector3 position = mCurrentTarget.Position;
					if (mCurrentTarget.ActorTargeted.realCharacter.LowProfile())
					{
						position.y -= 0.9f;
					}
					float sqrMagnitude = (mCurrentTarget.ActorTargeted.GetPosition() - mActor.GetPosition()).sqrMagnitude;
					float num = Vector3.Dot(mLookDirection, (position - mActor.transform.position).normalized);
					if (num > 0.95f && sqrMagnitude > 1f)
					{
						StartFiring();
					}
					else
					{
						StopFiring();
					}
				}
				ForceFollow();
				return;
			}
		}
		SetState(State.WarmDown);
	}

	protected void ForceFollow()
	{
		if (mCurrentTarget.ActorTargeted != null && mActor.awareness.CanSee(mCurrentTarget.ActorTargeted) && !mCurrentTarget.ActorTargeted.realCharacter.IsDead() && !mCurrentTarget.ActorTargeted.realCharacter.IsMortallyWounded())
		{
			mLookDirectionBeforeCentreBegan = mActor.awareness.LookDirection;
			mCurrentCentreTurretTime = 1f - Time.deltaTime;
			Vector3 position = mCurrentTarget.Position;
			if (mCurrentTarget.ActorTargeted.realCharacter.LowProfile())
			{
				position.y -= 0.9f;
			}
			ZonedInOnDirection(position - mActor.GetPosition(), 1f);
		}
		else
		{
			StopFiring();
			mCurrentTarget = null;
			SetState(State.HoldAim);
		}
	}

	protected bool AbleToTrackTarget()
	{
		if (mTrackDelayTimer == 0f && mCurrentTarget.ActorTargeted.behaviour.PlayerControlled && GameSettings.Instance.PerksEnabled && GameSettings.Instance.HasPerk(PerkType.ColdBlooded))
		{
			if (this is TaskSecurityCamera)
			{
				mTrackDelayTime = 1.5f;
			}
			else if (this is TaskSentryGun)
			{
				mTrackDelayTime = 2.5f;
			}
			else
			{
				mTrackDelayTime = 0f;
			}
			mTrackDelayTime *= StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.ColdBlooded);
		}
		if (mTrackDelayTimer >= mTrackDelayTime)
		{
			return true;
		}
		mTrackDelayTimer += Time.deltaTime;
		return false;
	}

	protected void ResetTrackDelay()
	{
		mTrackDelayTime = (mTrackDelayTimer = 0f);
	}

	protected bool HoldingAim()
	{
		mCurrentHoldAimTime += Time.deltaTime;
		return mCurrentHoldAimTime < mHoldAimTime;
	}

	public void ResetCoolDownTime()
	{
		mTimeToCoolDown = 0f;
	}

	public void ResetAngleAdjustment()
	{
		mCurrentAngleAdjustment = 0f;
	}

	public void ResetHoldAimTime()
	{
		mCurrentHoldAimTime = 0f;
	}

	public void SetTimeToHoldAim(float timeToHold)
	{
		mHoldAimTime = timeToHold;
	}

	public void EnableScanEffect()
	{
		if ((bool)mLaserEffect)
		{
			mLaserEffect.Enable(true);
		}
	}

	public void DisableScanEffect()
	{
		if ((bool)mLaserEffect)
		{
			mLaserEffect.Enable(false);
		}
	}

	public bool IsShootingAtTarget()
	{
		if (mCurrentTarget != null)
		{
			if (mActor.realCharacter.IsShootingAt(mCurrentTarget.ActorTargeted))
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
