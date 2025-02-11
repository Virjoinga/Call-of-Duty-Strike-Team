using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskSniper : Task
{
	private enum State
	{
		Search = 0,
		TrackTarget = 1,
		ReactToTarget = 2,
		HoldAim = 3,
		WaverOverPosition = 4,
		ReactToLosingATarget = 5,
		Reloading = 6
	}

	public List<KillZone> mKillZones = new List<KillZone>();

	private static float mSpeedToLockOntoTarget = 0.05f;

	private static float mWaverTime = 1f;

	private static float mFurtiveMovementTime = 0.5f;

	private static float mZoneInOnTargetTime = 2f;

	private int mCollisionMask;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private State mState;

	private float mCurrentHoldTime;

	private float mMaxHoldTime = 3f;

	private float mCurrentTrackTime;

	private Actor mCurrentTarget;

	private Vector3 mTargetMoveDirection = Vector3.zero;

	private Vector3 mPreviousTargetPosition = Vector3.zero;

	private int mCurrentTargetIndex;

	private int mPreviousTargetIndex;

	private Vector3 mLookDirection;

	private float mLookDistance;

	private float mMaxLookDistance;

	private Vector3 mLookDirectionBeforeTurnBegan;

	private Vector3 mStartLookDirection;

	private List<Vector3> mTargets = new List<Vector3>();

	private LineRenderer mLineRenderer;

	private bool mFastSweep;

	private float mNumFastSweepChecks;

	private Vector3 mCurrentPositionToWaverAt = Vector3.zero;

	private static float mMinWaverOffset = -0.3f;

	private static float mMaxWaverOffset = 0.3f;

	private Vector3 mCurrentWaverOffset = Vector3.zero;

	private int mWaverIndex;

	private static int mMaxWavers = 2;

	private int mNumExtendedSearches;

	private static int mMaxExtendedSearches = 2;

	public TaskSniper(TaskManager owner, TaskManager.Priority priority, Config flags, SniperOverrideData overrideData)
		: base(owner, priority, flags)
	{
		SetUpSniper(mActor.transform.forward.normalized, overrideData);
	}

	public TaskSniper(TaskManager owner, TaskManager.Priority priority, Config flags, SniperOverrideData overrideData, Vector3 startLookAt)
		: base(owner, priority, flags)
	{
		SetUpSniper(startLookAt, overrideData);
	}

	private void SetUpSniper(Vector3 positionToStartLookingAt, SniperOverrideData overrideData)
	{
		mOwner.VisualiseTasks += Visualise;
		Type[] taskTypes = new Type[2]
		{
			typeof(TaskRoutine),
			typeof(TaskSniper)
		};
		mActor.tasks.CancelTasksExcluding(taskTypes);
		mActor.realCharacter.IsSniper = true;
		mActor.animDirector.DefaultCulling = AnimationCullingType.AlwaysAnimate;
		mActor.animDirector.AnimationPlayer.cullingType = AnimationCullingType.AlwaysAnimate;
		mCurrentTargetIndex = (mPreviousTargetIndex = 0);
		FireAtWillComponent component = base.Owner.GetComponent<FireAtWillComponent>();
		if (component != null)
		{
			component.enabled = false;
		}
		mLookDistance = (mMaxLookDistance = mActor.awareness.VisionRange);
		mNumExtendedSearches = 0;
		mState = State.Search;
		mZoneInOnTargetTime += UnityEngine.Random.Range(0f, 0.5f);
		if (overrideData != null)
		{
			if (overrideData.KillZones != null)
			{
				mKillZones = overrideData.KillZones;
			}
			if (overrideData.DesiredTargets != null)
			{
				foreach (GuidRef desiredTarget in overrideData.DesiredTargets)
				{
					if (desiredTarget != null && desiredTarget.theObject != null)
					{
						mTargets.Add(desiredTarget.theObject.transform.position);
					}
				}
			}
			if (mTargets.Count == 0)
			{
				mTargets = overrideData.Targets;
			}
			mZoneInOnTargetTime = overrideData.ZoneInOnTargetSpeed;
			mSpeedToLockOntoTarget = overrideData.SpeedModifierToLockEnemy;
		}
		mLineRenderer = (LineRenderer)mActor.gameObject.AddComponent(typeof(LineRenderer));
		if (mLineRenderer != null)
		{
			mLineRenderer.material = EffectsController.Instance.Effects.SniperBeamMaterial;
			mLineRenderer.SetWidth(0.05f, 0.05f);
			int vertexCount = 2;
			mLineRenderer.SetVertexCount(vertexCount);
		}
		Vector3 vector = positionToStartLookingAt - mActor.awareness.GetPosition();
		mActor.realCharacter.ImposeLookDirection(vector.normalized);
		mLookDirection = vector.normalized;
		mLookDirectionBeforeTurnBegan = (mStartLookDirection = mLookDirection);
		mCollisionMask = (1 << LayerMask.NameToLayer("Default")) | (1 << LayerMask.NameToLayer("ConstantHitBox"));
		mActor.behaviour.nextAlertState = BehaviourController.AlertState.Combat;
	}

	public override bool HasFinished()
	{
		return false;
	}

	public override void Finish()
	{
		if (mLineRenderer != null)
		{
			mLineRenderer.enabled = false;
		}
		mOwner.VisualiseTasks -= Visualise;
	}

	public override void Update()
	{
		if (mActor.realCharacter == null)
		{
			return;
		}
		mActor.behaviour.nextAlertState = BehaviourController.AlertState.Combat;
		mMaxLookDistance = mActor.awareness.VisionRange;
		if (mCurrentTarget != null)
		{
			if (mPreviousTargetPosition != Vector3.zero && mPreviousTargetPosition != mCurrentTarget.transform.position)
			{
				mTargetMoveDirection = mPreviousTargetPosition - mCurrentTarget.transform.position;
			}
			mTargetMoveDirection.Normalize();
		}
		switch (mState)
		{
		case State.Search:
			Search();
			break;
		case State.TrackTarget:
			if (mCurrentTarget != null)
			{
				Vector3 vector = mCurrentTarget.transform.position - mActor.awareness.GetPosition();
				float magnitude = vector.magnitude;
				vector.Normalize();
				float num = Mathf.Acos(Vector3.Dot(mLookDirectionBeforeTurnBegan, vector));
				num *= 57.29578f;
				if (float.IsNaN(num))
				{
					num = 0f;
				}
				float num2 = num + magnitude;
				if (ZonedInOnDirection(vector, num2 * mSpeedToLockOntoTarget))
				{
					SetState(State.ReactToTarget);
				}
				if (TooCloseToBeConsideredATarget())
				{
					SetState(State.Search);
				}
			}
			break;
		case State.ReactToTarget:
			ReactToTarget();
			break;
		case State.HoldAim:
			if (!Holding())
			{
				SetState(State.Search);
			}
			break;
		case State.WaverOverPosition:
			WaverInPosition();
			break;
		case State.ReactToLosingATarget:
			ReactToLosingTarget();
			break;
		case State.Reloading:
			if (!IsReloading())
			{
				SetState(State.Search);
			}
			break;
		}
		if (mCurrentTarget != null)
		{
			mPreviousTargetPosition = mCurrentTarget.transform.position;
		}
	}

	private void SetState(State newState)
	{
		switch (newState)
		{
		case State.Search:
			mCurrentTarget = null;
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mCurrentTrackTime = 0f;
			break;
		case State.TrackTarget:
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mCurrentTrackTime = 0f;
			break;
		case State.HoldAim:
			mCurrentTarget = null;
			mCurrentHoldTime = 0f;
			break;
		case State.WaverOverPosition:
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mCurrentTrackTime = 0f;
			GetRandomWaverPositions();
			mWaverIndex = 0;
			break;
		case State.ReactToLosingATarget:
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mCurrentTrackTime = 0f;
			GetRandomWaverPositionsOnVector(mTargetMoveDirection);
			mNumExtendedSearches = 0;
			mWaverIndex = 0;
			break;
		case State.Reloading:
			mCurrentTarget = null;
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mCurrentTrackTime = 0f;
			break;
		}
		mState = newState;
	}

	private Vector3 GetCurrentTargetPosition(int index)
	{
		if (mTargets.Count <= 0)
		{
			return mActor.awareness.GetPosition() + mStartLookDirection * mLookDistance;
		}
		return mTargets[index];
	}

	private void Search()
	{
		Vector3 currentTargetPosition = GetCurrentTargetPosition(mCurrentTargetIndex);
		if (ZonedInOnDirection(currentTargetPosition - mActor.awareness.GetPosition(), mZoneInOnTargetTime))
		{
			mCurrentTrackTime = 0f;
			mPreviousTargetIndex = mCurrentTargetIndex;
			mCurrentTargetIndex++;
			if (mCurrentTargetIndex > mTargets.Count - 1)
			{
				mCurrentTargetIndex = 0;
			}
			mNumFastSweepChecks += 1f;
			if (mFastSweep && mNumFastSweepChecks > (float)(mTargets.Count - 1))
			{
				mFastSweep = false;
			}
			if (mFastSweep)
			{
				SetState(State.Search);
			}
			else
			{
				mCurrentPositionToWaverAt = GetCurrentTargetPosition(mPreviousTargetIndex);
				SetState(State.WaverOverPosition);
			}
		}
		CheckForEnemies();
	}

	private bool Holding()
	{
		mCurrentHoldTime += Time.deltaTime;
		SetLookAndDistance();
		if (mCurrentHoldTime > mMaxHoldTime)
		{
			return false;
		}
		CheckForEnemies();
		return true;
	}

	private void WaverInPosition()
	{
		Vector3 vector = mCurrentPositionToWaverAt + mCurrentWaverOffset;
		if (ZonedInOnDirection(vector - mActor.awareness.GetPosition(), mWaverTime))
		{
			mCurrentTrackTime = 0f;
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mWaverIndex++;
			if (mWaverIndex > mMaxWavers)
			{
				mWaverIndex = 0;
				SetState(State.Search);
			}
			else
			{
				GetRandomWaverPositions();
			}
		}
		CheckForEnemies();
	}

	private void ReactToLosingTarget()
	{
		Vector3 vector = mCurrentPositionToWaverAt + mCurrentWaverOffset;
		if (ZonedInOnDirection(vector - mActor.awareness.GetPosition(), mFurtiveMovementTime))
		{
			mCurrentTrackTime = 0f;
			mLookDirectionBeforeTurnBegan = mLookDirection;
			mWaverIndex++;
			if (mWaverIndex > mMaxWavers)
			{
				mWaverIndex = 0;
				if (mNumExtendedSearches >= mMaxExtendedSearches)
				{
					mNumExtendedSearches = 0;
					SetState(State.Search);
				}
				else
				{
					mNumExtendedSearches++;
				}
				GetRandomWaverPositionsOnVector(mTargetMoveDirection);
			}
			else
			{
				GetRandomWaverPositionsOnVector(mTargetMoveDirection);
			}
		}
		CheckForEnemies();
	}

	private void GetRandomWaverPositions()
	{
		mCurrentWaverOffset = new Vector3(UnityEngine.Random.Range(mMinWaverOffset, mMaxWaverOffset), UnityEngine.Random.Range(mMinWaverOffset, mMaxWaverOffset), UnityEngine.Random.Range(mMinWaverOffset, mMaxWaverOffset));
	}

	private void GetRandomWaverPositionsOnVector(Vector3 direction)
	{
		float max;
		if (mNumExtendedSearches > 0)
		{
			max = 2f;
			direction *= UnityEngine.Random.Range(-1f, 1f);
		}
		else
		{
			max = 3f;
			direction *= -1f;
		}
		direction *= UnityEngine.Random.Range(1f, max);
		mCurrentWaverOffset = direction;
	}

	private void CheckForEnemies()
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.EnemiesMask(mActor) & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			mCurrentTarget = a;
			if (LOSVisible() || mCurrentTarget.realCharacter.IsShootingAt(mActor))
			{
				SetState(State.TrackTarget);
				break;
			}
			mCurrentTarget = null;
		}
	}

	private bool ZonedInOnDirection(Vector3 direction, float time)
	{
		mCurrentTrackTime += Time.deltaTime;
		float num = mCurrentTrackTime / time;
		direction.Normalize();
		mLookDirection = mLookDirectionBeforeTurnBegan + (direction - mLookDirectionBeforeTurnBegan) * num;
		SetLookAndDistance();
		return num > 0.9f || mLookDirection == direction;
	}

	private void SetLookAndDistance()
	{
		mActor.awareness.LookDirection = mStartLookDirection;
		Vector3 position = mActor.awareness.GetPosition();
		Vector3 normalized = mLookDirection.normalized;
		Vector3 end = position + normalized * mMaxLookDistance;
		position += normalized * 2f;
		RaycastHit hitInfo;
		if (Physics.Linecast(position, end, out hitInfo, mCollisionMask))
		{
			mLookDistance = hitInfo.distance + 2f;
		}
		mActor.realCharacter.TurnToFaceDirection(mLookDirection);
		Vector3 forward = mLookDirection;
		forward.y = 0f;
		mActor.transform.forward = forward;
	}

	private void ForceFollow()
	{
		if (LOSVisible())
		{
			if (mPreviousTargetPosition != mCurrentTarget.transform.position)
			{
				mCurrentTrackTime = 1f;
				ZonedInOnDirection(mCurrentTarget.transform.position - mActor.awareness.GetPosition(), 1.1f);
			}
		}
		else if (mCurrentTarget != null)
		{
			RemoveTarget();
			mCurrentPositionToWaverAt = mCurrentTarget.transform.position;
			SetState(State.ReactToLosingATarget);
		}
		else
		{
			RemoveTarget();
			mPreviousTargetPosition = Vector3.zero;
			SetState(State.Search);
		}
	}

	private bool LOSVisible()
	{
		if (mCurrentTarget == null)
		{
			return false;
		}
		if (mCurrentTarget.realCharacter.IsDead() || mCurrentTarget.realCharacter.IsMortallyWounded())
		{
			return false;
		}
		if (mKillZones.Count > 0)
		{
			bool flag = false;
			for (int i = 0; i < mKillZones.Count; i++)
			{
				if (mKillZones[i].Contains(mCurrentTarget))
				{
					flag = true;
					break;
				}
			}
			if (!flag && mState != State.TrackTarget && mState != State.ReactToTarget)
			{
				return false;
			}
		}
		if (TooCloseToBeConsideredATarget())
		{
			return false;
		}
		if (!mActor.awareness.CanSee(mCurrentTarget))
		{
			return false;
		}
		if (mCurrentTarget.awareness.IsInCover() && !mCurrentTarget.weapon.IsFiring() && mCurrentTarget.realCharacter.IsCrouching())
		{
			return false;
		}
		return true;
	}

	private bool TooCloseToBeConsideredATarget()
	{
		if (mActor != null && mCurrentTarget != null)
		{
			float num = 49f;
			Vector2 vector = new Vector2(mCurrentTarget.transform.position.x - mActor.transform.position.x, mCurrentTarget.transform.position.z - mActor.transform.position.z);
			if (vector.sqrMagnitude < num)
			{
				return true;
			}
		}
		return false;
	}

	private void ReactToTarget()
	{
		if (LOSVisible())
		{
			if (mActor.weapon != null)
			{
				FireAtTarget();
			}
		}
		else
		{
			mFastSweep = true;
			mNumFastSweepChecks = 0f;
		}
		if (mState == State.ReactToTarget)
		{
			ForceFollow();
		}
	}

	private void FireAtTarget()
	{
		if (!(mActor != null) || !(mActor.weapon != null))
		{
			return;
		}
		if (!mActor.weapon.IsReloading())
		{
			mActor.weapon.SetTarget(mCurrentTarget);
			if (mCurrentTarget != null)
			{
				mActor.weapon.SetAiming(true);
			}
			if (mActor.realCharacter != null)
			{
				mActor.realCharacter.ShootAtTarget(mCurrentTarget);
			}
		}
		else
		{
			RemoveTarget();
			SetState(State.Reloading);
		}
	}

	private void RemoveTarget()
	{
		if (mActor != null && mActor.weapon != null)
		{
			mActor.weapon.SetAiming(false);
			mActor.weapon.SetTarget(null);
		}
	}

	private bool IsReloading()
	{
		if (mActor != null && mActor.weapon != null)
		{
			return mActor.weapon.IsReloading();
		}
		return false;
	}

	private bool IsRecoiling()
	{
		if (mActor != null && mActor.weapon != null && mActor.weapon.ActiveWeapon != null)
		{
			Weapon_Sniper weapon_Sniper = mActor.weapon.ActiveWeapon as Weapon_Sniper;
			return weapon_Sniper != null && weapon_Sniper.IsRecoiling();
		}
		return false;
	}

	public virtual void Visualise()
	{
		if (mActor.realCharacter.IsDead() || !(mLineRenderer != null))
		{
			return;
		}
		mLineRenderer.enabled = true;
		Vector3 position = mActor.awareness.GetPosition();
		Vector3 vector = mLookDirection;
		Vector3 vector2 = mLookDirection;
		Vector3 vector3 = mLookDirection;
		Vector3 vector4 = mLookDirection;
		if (mActor.weapon != null && mActor.weapon.ActiveWeapon != null && mActor.weapon.ActiveWeapon.GetType() == typeof(Weapon_Sniper))
		{
			position = (mActor.weapon.ActiveWeapon as Weapon_Sniper).GetMuzzleLocator().position;
			vector4 = (mActor.weapon.ActiveWeapon as Weapon_Sniper).GetMuzzleDir();
			vector2 = vector4;
			if (mCurrentTarget != null && !IsReloading() && !IsRecoiling())
			{
				Vector3 position2 = mCurrentTarget.transform.position;
				if (mCurrentTarget.realCharacter.IsCrouching())
				{
					position2.y += 0.45f;
				}
				else
				{
					position2.y += 0.9f;
				}
				vector3 = position2 - position;
				vector3.Normalize();
				float num = Vector3.Dot(vector3, mLookDirection);
				if (num > 0.98f)
				{
					vector2 = Vector3.Lerp(vector4, vector3, num);
				}
			}
		}
		vector = position + vector2 * mMaxLookDistance;
		Vector3 start = position + vector2 * 2f;
		RaycastHit hitInfo;
		if (Physics.Linecast(start, vector, out hitInfo, mCollisionMask))
		{
			vector = hitInfo.point;
		}
		mLineRenderer.SetPosition(0, position);
		mLineRenderer.SetPosition(1, vector);
	}
}
