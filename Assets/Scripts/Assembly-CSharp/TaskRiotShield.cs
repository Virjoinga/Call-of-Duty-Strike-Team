using System;
using UnityEngine;

public class TaskRiotShield : Task
{
	private enum BehaviourState
	{
		Spawning = 0,
		Stand = 1,
		TurnToFace = 2,
		Advance = 3,
		Flinch = 4,
		ShieldBash = 5,
		ShootPistol = 6,
		ThrowGrenade = 7,
		FlinchBig = 8
	}

	private RiotShieldDescriptorConfig mSpawnConfig;

	private float mRepathDistanceSq;

	private float mShieldBashDistanceSq;

	private float mGrenadeThrowMinimumDistanceSq;

	private float mGrenadeThrowMaximumDistanceSq;

	private BehaviourState mState;

	private Weapon_Beretta_RiotShieldEnemy mPistol;

	private Actor mCurrentTarget;

	private float mShootTime;

	private int mShotsRemaining;

	private int mGrenadeCount;

	private float mConsiderGrenadeTime;

	private float mFlinchTime;

	public TaskRiotShield(TaskManager owner, TaskManager.Priority priority, Config flags, RiotShieldDescriptorConfig spawnConfig)
		: base(owner, priority, flags)
	{
		mActor.health.OnHealthChangeEnter += OnHealthChangeEnter;
		mSpawnConfig = spawnConfig;
		mRepathDistanceSq = mSpawnConfig.RepathDistance * mSpawnConfig.RepathDistance;
		mShieldBashDistanceSq = mSpawnConfig.ShieldBashDistance * mSpawnConfig.ShieldBashDistance;
		mGrenadeThrowMinimumDistanceSq = mSpawnConfig.GrenadeThrowMinimumDistance * mSpawnConfig.GrenadeThrowMinimumDistance;
		mGrenadeThrowMaximumDistanceSq = mSpawnConfig.GrenadeThrowMaximumDistance * mSpawnConfig.GrenadeThrowMaximumDistance;
		mActor.realCharacter.GrenadeDamageMultiplier = mSpawnConfig.GrenadeDamageMultiplier;
		mState = BehaviourState.Spawning;
		mPistol = mActor.weapon.ActiveWeapon as Weapon_Beretta_RiotShieldEnemy;
		TBFAssert.DoAssert(mPistol != null, "TaskRiotShield requires a Weapon_Beretta_RiotShieldEnemy setup");
		ResetShootTimer();
		mGrenadeCount = mSpawnConfig.GrenadeInventoryCount;
		mConsiderGrenadeTime = UnityEngine.Random.Range(mSpawnConfig.ConsiderGrenadeTimeInitialRange[0], mSpawnConfig.ConsiderGrenadeTimeInitialRange[1]);
	}

	public override void Update()
	{
		if (mActor.navAgent.enabled)
		{
			mActor.realCharacter.Crouch();
			UpdateBehaviourStateMachine();
			mActor.realCharacter.PickSomethingToAimAt(null);
			if (mState != BehaviourState.ShootPistol && mShootTime > 0f)
			{
				mShootTime -= Time.deltaTime;
			}
		}
	}

	public override void Finish()
	{
		mActor.health.OnHealthChangeEnter -= OnHealthChangeEnter;
	}

	public override bool HasFinished()
	{
		return false;
	}

	public override void OnResume()
	{
		if (!mActor.baseCharacter.IsBeingMovedManually())
		{
			SwitchToState(BehaviourState.Stand);
		}
	}

	public void NotifyAnimationStateComplete(RiotShieldNPCModule.ActionEnum action)
	{
		switch (mState)
		{
		case BehaviourState.ShieldBash:
			switch (action)
			{
			case RiotShieldNPCModule.ActionEnum.kShieldBash:
				ShieldBash();
				break;
			case RiotShieldNPCModule.ActionEnum.kShieldBashComplete:
				SwitchToState(BehaviourState.Stand);
				break;
			default:
				Debug.LogWarning(string.Format("RSG BehaviourState.ShieldBash : Unexpected Anim Action={0}", action));
				break;
			}
			break;
		case BehaviourState.ShootPistol:
		{
			RiotShieldNPCModule.ActionEnum actionEnum = action;
			if (actionEnum == RiotShieldNPCModule.ActionEnum.kShootPistol)
			{
				ShootPistol();
			}
			else
			{
				Debug.LogWarning(string.Format("RSG BehaviourState.ShootPistol : Unexpected Anim Action={0}", action));
			}
			break;
		}
		case BehaviourState.ThrowGrenade:
			switch (action)
			{
			case RiotShieldNPCModule.ActionEnum.kThrowGrenade:
				ThrowGrenade();
				break;
			case RiotShieldNPCModule.ActionEnum.kTransGrenadeToWalk:
				SwitchToState(BehaviourState.Advance);
				break;
			default:
				Debug.LogWarning(string.Format("RSG BehaviourState.ThrowGrenade : Unexpected Anim Action={0}", action));
				break;
			}
			break;
		}
	}

	private void UpdateBehaviourStateMachine()
	{
		switch (mState)
		{
		case BehaviourState.Spawning:
			SwitchToState(BehaviourState.Stand);
			break;
		case BehaviourState.Stand:
			UpdateStand();
			break;
		case BehaviourState.TurnToFace:
			break;
		case BehaviourState.Advance:
			UpdateAdvance();
			break;
		case BehaviourState.Flinch:
		case BehaviourState.FlinchBig:
			UpdateFlinch();
			break;
		case BehaviourState.ShieldBash:
			UpdateShieldBash();
			break;
		case BehaviourState.ShootPistol:
			UpdateShootPistol();
			break;
		case BehaviourState.ThrowGrenade:
			UpdateThrowGrenade();
			break;
		}
	}

	private void UpdateStand()
	{
		bool flag = EvaluateNearestEnemy();
		if (mActor.navAgent.isOnOffMeshLink)
		{
			if (!flag)
			{
				return;
			}
			Vector3 rhs = mActor.navAgent.currentOffMeshLinkData.endPos - mActor.GetPosition();
			if (!(rhs.sqrMagnitude > 0f))
			{
				return;
			}
			rhs.Normalize();
			Vector3 normalized = (mCurrentTarget.GetPosition() - mActor.GetPosition()).normalized;
			if (Vector3.Dot(normalized, rhs) < 0f)
			{
				mActor.navAgent.enabled = false;
				mActor.navAgent.enabled = true;
				SwitchToState(BehaviourState.Advance);
				return;
			}
			float sqrMagnitude = (mCurrentTarget.GetPosition() - mActor.GetPosition()).sqrMagnitude;
			if (sqrMagnitude > mGrenadeThrowMinimumDistanceSq && sqrMagnitude < mGrenadeThrowMaximumDistanceSq)
			{
				mGrenadeCount++;
				SwitchToState(BehaviourState.ThrowGrenade);
			}
		}
		else
		{
			if (!flag)
			{
				return;
			}
			float sqrMagnitude2 = (mCurrentTarget.GetPosition() - mActor.GetPosition()).sqrMagnitude;
			if (sqrMagnitude2 <= mShieldBashDistanceSq)
			{
				if (mActor.awareness.CanSee(mCurrentTarget))
				{
					if (CanShieldBash())
					{
						SwitchToState(BehaviourState.ShieldBash);
					}
					else
					{
						SwitchToState(BehaviourState.Advance);
					}
				}
			}
			else
			{
				SwitchToState(BehaviourState.Advance);
			}
		}
	}

	private void StartAdvance()
	{
		TBFAssert.DoAssert(mCurrentTarget != null, "Should not have entered Advance state if we have no Target");
		mActor.navAgent.destination = mCurrentTarget.GetPosition();
		mActor.realCharacter.MovementStyleActive = BaseCharacter.MovementStyle.Walk;
		float speed = mActor.realCharacter.GetGaitSpeed(mActor.realCharacter.MovementStyleActive) * UnityEngine.Random.Range(0.95f, 1.05f);
		mActor.navAgent.speed = speed;
		mActor.realCharacter.SetIsMoving(true, mCurrentTarget.GetPosition());
	}

	private void UpdateAdvance()
	{
		if (mActor.navAgent.isOnOffMeshLink)
		{
			SwitchToState(BehaviourState.Stand);
		}
		else if (EvaluateNearestEnemy())
		{
			float sqrMagnitude = (mCurrentTarget.GetPosition() - mActor.navAgent.destination).sqrMagnitude;
			if (sqrMagnitude > mRepathDistanceSq)
			{
				mActor.navAgent.destination = mCurrentTarget.GetPosition();
			}
			float sqrMagnitude2 = (mCurrentTarget.GetPosition() - mActor.GetPosition()).sqrMagnitude;
			if (sqrMagnitude2 <= mShieldBashDistanceSq)
			{
				mActor.navAgent.Stop();
				if (CanShieldBash())
				{
					SwitchToState(BehaviourState.ShieldBash);
				}
				else
				{
					SwitchToState(BehaviourState.Stand);
				}
				return;
			}
			if (mGrenadeCount > 0)
			{
				mConsiderGrenadeTime -= Time.deltaTime;
				if (mConsiderGrenadeTime <= 0f)
				{
					mConsiderGrenadeTime = UnityEngine.Random.Range(mSpawnConfig.ConsiderGrenadeTimeSubsequentRange[0], mSpawnConfig.ConsiderGrenadeTimeSubsequentRange[1]);
					if (sqrMagnitude2 > mGrenadeThrowMinimumDistanceSq && sqrMagnitude2 < mGrenadeThrowMaximumDistanceSq)
					{
						mActor.navAgent.Stop();
						SwitchToState(BehaviourState.ThrowGrenade);
					}
					return;
				}
			}
			if (mShootTime <= 0f)
			{
				ResetShootTimer();
				mShotsRemaining = UnityEngine.Random.Range(mSpawnConfig.ShotBurst[0], mSpawnConfig.ShotBurst[1]);
				mActor.navAgent.Stop();
				SwitchToState(BehaviourState.ShootPistol);
			}
		}
		else
		{
			mActor.navAgent.Stop();
			SwitchToState(BehaviourState.Stand);
		}
	}

	private void StartFlinch(float flinchTime)
	{
		if (mActor.navAgent.enabled)
		{
			mActor.navAgent.Stop();
		}
		mFlinchTime = flinchTime;
	}

	private void UpdateFlinch()
	{
		mFlinchTime -= Time.deltaTime;
		if (mFlinchTime <= 0f)
		{
			if (mCurrentTarget == null)
			{
				SwitchToState(BehaviourState.Stand);
			}
			else
			{
				SwitchToState(BehaviourState.Advance);
			}
		}
	}

	private void UpdateShieldBash()
	{
		if (!HasValidTarget())
		{
			SwitchToState(BehaviourState.Stand);
		}
		else if (EvaluateNearestEnemy())
		{
			float sqrMagnitude = (mCurrentTarget.GetPosition() - mActor.GetPosition()).sqrMagnitude;
			if (sqrMagnitude > mShieldBashDistanceSq)
			{
				SwitchToState(BehaviourState.Advance);
			}
			else if (!mActor.awareness.CanSee(mCurrentTarget))
			{
				SwitchToState(BehaviourState.Advance);
			}
		}
		else
		{
			mActor.navAgent.Stop();
			SwitchToState(BehaviourState.Stand);
		}
	}

	private void StartShootPistol()
	{
	}

	private void UpdateShootPistol()
	{
		if (!HasValidTarget())
		{
			SwitchToState(BehaviourState.Stand);
			return;
		}
		Vector3 forward = mActor.transform.forward;
		Vector3 position = mActor.GetPosition();
		Vector3 position2 = mCurrentTarget.GetPosition();
		Vector3 rhs = position2 - position;
		if (rhs.sqrMagnitude > 0f)
		{
			rhs.Normalize();
			if (Vector3.Dot(forward, rhs) < 0f)
			{
				SwitchToState(BehaviourState.Advance);
				return;
			}
		}
		if (!mActor.awareness.CanSee(mCurrentTarget))
		{
			SwitchToState(BehaviourState.Advance);
		}
	}

	private void StartThrowGrenade()
	{
	}

	private void UpdateThrowGrenade()
	{
		if (!HasValidTarget())
		{
			SwitchToState(BehaviourState.Stand);
			return;
		}
		Vector3 forward = mActor.transform.forward;
		Vector3 rhs = mCurrentTarget.GetPosition() - mActor.GetPosition();
		if (rhs.sqrMagnitude <= mShieldBashDistanceSq)
		{
			SwitchToState(BehaviourState.ShieldBash);
		}
		else if (rhs.sqrMagnitude > 0f)
		{
			rhs.Normalize();
			if (Vector3.Dot(forward, rhs) < 0f)
			{
				SwitchToState(BehaviourState.Stand);
			}
		}
	}

	private void SwitchToState(BehaviourState nextState)
	{
		OutputDebugLog(string.Format("Switching from state {0} to {1}", mState.ToString(), nextState.ToString()));
		mState = nextState;
		mActor.Pose.Command(nextState.ToString());
		switch (mState)
		{
		case BehaviourState.Spawning:
			break;
		case BehaviourState.Stand:
			break;
		case BehaviourState.TurnToFace:
			break;
		case BehaviourState.Advance:
			StartAdvance();
			break;
		case BehaviourState.Flinch:
			StartFlinch(mSpawnConfig.FlinchDuration);
			break;
		case BehaviourState.ShieldBash:
			break;
		case BehaviourState.ShootPistol:
			StartShootPistol();
			break;
		case BehaviourState.ThrowGrenade:
			StartThrowGrenade();
			break;
		case BehaviourState.FlinchBig:
			StartFlinch(mSpawnConfig.FlinchBigDuration);
			break;
		}
	}

	private bool EvaluateNearestEnemy()
	{
		Vector3 lastKnownPosition;
		Actor nearestKnownEnemy = mActor.awareness.GetNearestKnownEnemy(out lastKnownPosition, false, CharacterType.SentryGun);
		if (nearestKnownEnemy != null)
		{
			bool flag = true;
			if (nearestKnownEnemy.health.Invulnerable && !nearestKnownEnemy.behaviour.PlayerControlled)
			{
				flag = false;
			}
			if (flag)
			{
				mCurrentTarget = nearestKnownEnemy;
				return true;
			}
		}
		return false;
	}

	private void ResetShootTimer()
	{
		mShootTime = UnityEngine.Random.Range(mSpawnConfig.TimeToNextShooting[0], mSpawnConfig.TimeToNextShooting[1]);
	}

	private void ShootPistol()
	{
		mPistol.Target = mCurrentTarget;
		mPistol.DepressTrigger();
		mShotsRemaining--;
		if (mShotsRemaining <= 0)
		{
			SwitchToState(BehaviourState.Advance);
		}
		else
		{
			SwitchToState(BehaviourState.ShootPistol);
		}
	}

	private void ThrowGrenade()
	{
		mActor.grenadeThrower.Target = mCurrentTarget.GetPosition() + UnityEngine.Random.insideUnitSphere * mSpawnConfig.GrenadeTargetRandomOffset;
		if (mSpawnConfig.GrenadeTargetFallshortOffset != 0f)
		{
			Vector3 normalized = (mActor.GetPosition() - mCurrentTarget.GetPosition()).normalized;
			mActor.grenadeThrower.Target += normalized * mSpawnConfig.GrenadeTargetFallshortOffset;
		}
		GameplayController gameplayController = GameplayController.Instance();
		gameplayController.BroadcastEventGrenade(mActor);
		mActor.realCharacter.ThrowGrenade(mActor.grenadeThrower.Target);
		mGrenadeCount--;
	}

	private void ShieldBash()
	{
		if (!(mCurrentTarget == null))
		{
			Vector3 vector = mCurrentTarget.GetPosition() - mActor.GetPosition();
			float num = mCurrentTarget.health.GetActualHealthFromDelta(mSpawnConfig.ShieldBashDamage);
			if (mCurrentTarget.behaviour.PlayerControlled && mCurrentTarget.health.Health - num < mCurrentTarget.health.MortallyWoundedThreshold)
			{
				num = Mathf.Max(0f, mCurrentTarget.health.Health - (mCurrentTarget.health.MortallyWoundedThreshold - 1f));
			}
			mCurrentTarget.health.ModifyHealth(mActor.gameObject, 0f - num, "Shield Bash", vector.normalized, false);
			WeaponSFX.Instance.ShieldBash.Play(mActor.gameObject);
			mCurrentTarget.Command("Bashed");
		}
	}

	private void OutputDebugLog(string message)
	{
	}

	private bool CanShieldBash()
	{
		if (mCurrentTarget == null)
		{
			return false;
		}
		if (!mActor.awareness.CanSee(mCurrentTarget))
		{
			return false;
		}
		Vector3 forward = mActor.transform.forward;
		Vector3 position = mActor.GetPosition();
		Vector3 position2 = mCurrentTarget.GetPosition();
		Vector3 rhs = position2 - position;
		if (rhs.sqrMagnitude > 0f)
		{
			rhs.Normalize();
			float f = Vector3.Dot(forward, rhs);
			float num = Mathf.Acos(f);
			if (num > (float)Math.PI / 6f)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private bool IsInSuitableStateForFlinch()
	{
		return mState == BehaviourState.Stand || mState == BehaviourState.Advance || mState == BehaviourState.ShootPistol;
	}

	private bool HasValidTarget()
	{
		if (mCurrentTarget == null || ((bool)mCurrentTarget && (mCurrentTarget.realCharacter.IsDead() || mCurrentTarget.realCharacter.IsMortallyWounded())))
		{
			return false;
		}
		return true;
	}

	private void OnHealthChangeEnter(object sender, EventArgs args)
	{
		if (!IsInSuitableStateForFlinch())
		{
			return;
		}
		HealthComponent.HealthChangeEnterEventArgs healthChangeEnterEventArgs = args as HealthComponent.HealthChangeEnterEventArgs;
		if (healthChangeEnterEventArgs.HitLocation != null && healthChangeEnterEventArgs.HitLocation.name == "Shield")
		{
			if (healthChangeEnterEventArgs.DamageType == "Grenade" || healthChangeEnterEventArgs.DamageType == "Claymore" || healthChangeEnterEventArgs.DamageType == "Explosion")
			{
				SwitchToState(BehaviourState.FlinchBig);
			}
			else
			{
				SwitchToState(BehaviourState.Flinch);
			}
		}
	}
}
