using UnityEngine;

public class TaskRPG : Task
{
	private enum BehaviourState
	{
		Spawning = 0,
		Stand = 1,
		TurnToAim = 2,
		Fire = 3,
		ReloadIn = 4,
		ReloadOut = 5,
		Walk = 6
	}

	private const float kGroundYOffset = 0.5f;

	private const float kDestinationTolerance = 0.5f;

	private const float kAimSpeed = 2f;

	private const float kAimTolerance = 0.1f;

	private const float kShuffleBlendFactor = 2f;

	private const float kTurnToAimTime = 1.5f;

	private const float kReloadAnimTime = 6f;

	private const float kFireTimer = 1f;

	private const float kDestinationToleranceSq = 0.25f;

	public float ShuffleBlend;

	private Vector3 mCurrentAimDir;

	private Vector3 mTargetAimDir;

	private BehaviourState mState;

	private bool mWalkingToSpawn;

	private bool mAimAtGround = true;

	private float mWaitInBetweenShots;

	private float mSpawnWaitTime;

	private float mWaitTime;

	private float mCurrentInaccuracy;

	private float mAccuracyIncrease;

	private float mDamageMultiplier;

	private GameObject mCurrentTarget;

	private GameObject mWalkTarget;

	private float mTestSpin;

	private int mUId;

	private TargetSwitchComponent mTargetSwitcher;

	private int mCurrentMoveCommand;

	public Vector3 CurrentAimDir
	{
		get
		{
			return mCurrentAimDir;
		}
	}

	public TaskRPG(TaskManager owner, TaskManager.Priority priority, Config flags, RPGOverrideData overrideData)
		: base(owner, priority, flags)
	{
		mState = BehaviourState.Spawning;
		mWaitInBetweenShots = overrideData.WaitInbetweenShots;
		mWalkTarget = overrideData.SpawnTarget;
		mCurrentInaccuracy = overrideData.Inaccuracy;
		mAccuracyIncrease = overrideData.AccuracyIncrease;
		mDamageMultiplier = overrideData.DamageMultiplier;
		mSpawnWaitTime = overrideData.TimeToWaitAfterSpawn;
		mTargetSwitcher = overrideData.TargetSwitcher;
		ShuffleBlend = 0f;
		if (overrideData.Target != null)
		{
			mCurrentTarget = overrideData.Target;
		}
		mAimAtGround = overrideData.AimAtGround;
	}

	public override void Update()
	{
		if (mTargetSwitcher != null)
		{
			GameObject currentTarget = mTargetSwitcher.CurrentTarget;
			if (currentTarget != null)
			{
				mCurrentTarget = currentTarget;
			}
		}
		UpdateBehaviourStateMachine();
		mActor.realCharacter.ImposeLookDirection(mCurrentAimDir);
		mActor.realCharacter.PickSomethingToAimAt(null);
	}

	public override void Finish()
	{
	}

	public override bool HasFinished()
	{
		return false;
	}

	public bool IsAiming()
	{
		if (mState == BehaviourState.Stand || mState == BehaviourState.TurnToAim)
		{
			return true;
		}
		return false;
	}

	private bool HasValidTarget()
	{
		return true;
	}

	private void UpdateBehaviourStateMachine()
	{
		switch (mState)
		{
		case BehaviourState.Spawning:
			if (mWalkTarget != null)
			{
				mWalkingToSpawn = true;
				mCurrentAimDir = mActor.transform.forward;
				mCurrentAimDir.Normalize();
				SwitchToState(BehaviourState.Walk);
			}
			else
			{
				SwitchToState(BehaviourState.Stand);
			}
			break;
		case BehaviourState.Stand:
			UpdateStand();
			break;
		case BehaviourState.TurnToAim:
			UpdateTurnToAim();
			break;
		case BehaviourState.Walk:
			UpdateWalking();
			break;
		case BehaviourState.ReloadIn:
			UpdateReload();
			break;
		case BehaviourState.Fire:
			UpdateFire();
			break;
		case BehaviourState.ReloadOut:
			break;
		}
	}

	private void UpdateReload()
	{
		mWaitTime -= Time.deltaTime;
		if (mWaitTime < 0f && mCurrentTarget != null)
		{
			SwitchToState(BehaviourState.Stand);
			mWaitTime = 1.5f;
			mActor.Pose.Command("ExitReload");
		}
	}

	private void UpdateFire()
	{
		mWaitTime -= Time.deltaTime;
		if (mWaitTime < 0f && mCurrentTarget != null)
		{
			SwitchToState(BehaviourState.ReloadIn);
		}
	}

	private void UpdateStand()
	{
		CalcAimDirection();
		TurnToAim();
		if (mWaitTime < 0f && mCurrentTarget != null)
		{
			SwitchToState(BehaviourState.TurnToAim);
		}
		mWaitTime -= Time.deltaTime;
	}

	private void UpdateTurnToAim()
	{
		CalcAimDirection();
		TurnToAim();
		float num = Vector3.Dot(mCurrentAimDir, mTargetAimDir);
		if (num >= 0.9f)
		{
			mCurrentAimDir = mTargetAimDir;
			FireWeapon();
		}
	}

	private void FireWeapon()
	{
		GameObject gameObject = InstantiateRPG(mActor.gameObject.transform.position);
		gameObject.name = mActor.realCharacter.name + "_" + mUId;
		RPGProjectile component = gameObject.GetComponent<RPGProjectile>();
		component.OverrideDamageMultiplier(mDamageMultiplier);
		component.Launch(mActor.gameObject, mTargetAimDir, true);
		if (EffectsController.Instance != null)
		{
			WeaponUtils.CreateMuzzleFlash(mActor.transform, EffectsController.Instance.Effects.AN94MuzzleFlash);
		}
		SwitchToState(BehaviourState.Fire);
		if (mCurrentInaccuracy > 0f)
		{
			mCurrentInaccuracy -= mAccuracyIncrease;
			if (mCurrentInaccuracy <= 0f)
			{
				mCurrentInaccuracy = 0f;
			}
		}
	}

	private GameObject InstantiateRPG(Vector3 pos)
	{
		GameObject result = Object.Instantiate(ExplosionManager.Instance.RPG, pos, Quaternion.identity) as GameObject;
		mUId++;
		return result;
	}

	private void SwitchToState(BehaviourState nextState)
	{
		mState = nextState;
		mActor.Pose.Command(nextState.ToString());
		switch (mState)
		{
		case BehaviourState.ReloadIn:
			ResetFireTimer();
			break;
		case BehaviourState.Walk:
			StartWalking();
			break;
		}
		if (nextState == BehaviourState.Fire)
		{
			mWaitTime = 1f;
		}
	}

	public Vector3 GetTargetPosition()
	{
		if (mCurrentTarget == null)
		{
			Debug.Log("Warning - No target set on RPG guy");
			return Vector3.zero;
		}
		Vector3 vector = mCurrentTarget.transform.position;
		if (mCurrentInaccuracy > 0f)
		{
			Vector3 vector2 = mCurrentTarget.transform.position - mActor.transform.position;
			vector2.Normalize();
			vector -= vector2 * mCurrentInaccuracy;
		}
		RaycastHit hitInfo;
		if (mAimAtGround && Physics.Raycast(vector, -Vector3.up, out hitInfo, float.PositiveInfinity, 1))
		{
			vector = hitInfo.point + Vector3.up * 0.5f;
		}
		return vector;
	}

	public void CalcAimDirection()
	{
		Vector3 position = mActor.transform.position;
		position.y += 1.6f;
		mTargetAimDir = GetTargetPosition() - position;
		mTargetAimDir.Normalize();
	}

	public void NotifyAnimationStateComplete(RPGModule.ActionEnum action)
	{
		switch (mState)
		{
		case BehaviourState.ReloadIn:
			SwitchToState(BehaviourState.Stand);
			break;
		case BehaviourState.Fire:
			SwitchToState(BehaviourState.ReloadIn);
			break;
		}
	}

	private void StartWalking()
	{
		mActor.navAgent.destination = mWalkTarget.transform.position;
		mActor.realCharacter.MovementStyleActive = BaseCharacter.MovementStyle.Run;
		float speed = mActor.realCharacter.GetGaitSpeed(mActor.realCharacter.MovementStyleActive) * Random.Range(0.95f, 1.05f);
		mActor.navAgent.speed = speed;
		mActor.realCharacter.SetIsMoving(true, mWalkTarget.transform.position);
		ResetFireTimer();
		mCurrentAimDir = mActor.transform.forward;
		mCurrentAimDir.Normalize();
		mTargetAimDir = mWalkTarget.transform.position - mActor.transform.position;
		mTargetAimDir.Normalize();
	}

	private void UpdateWalking()
	{
		TurnToAim();
		float sqrMagnitude = (mWalkTarget.transform.position - mActor.transform.position).sqrMagnitude;
		if (sqrMagnitude <= 0.25f)
		{
			if (mWalkingToSpawn)
			{
				mWaitTime = mSpawnWaitTime;
				mWalkingToSpawn = false;
			}
			else
			{
				ResetFireTimer();
			}
			SwitchToState(BehaviourState.Stand);
		}
		else
		{
			ResetFireTimer();
		}
	}

	private void ResetFireTimer()
	{
		float num = mWaitInBetweenShots;
		if (num > 1.5f)
		{
			num -= 1.5f;
		}
		mWaitTime = num + 6f;
	}

	private void TurnToAim()
	{
		mCurrentAimDir = Vector3.Lerp(mCurrentAimDir, mTargetAimDir, Time.deltaTime * 2f);
		ShuffleBlend = (1f - Vector3.Dot(mCurrentAimDir, mTargetAimDir)) * 2f;
		if (ShuffleBlend > 1f)
		{
			ShuffleBlend = 1f;
		}
		mCurrentAimDir.Normalize();
		if (mState != BehaviourState.Walk)
		{
			mActor.Pose.Command("BlendShuffle");
		}
	}
}
