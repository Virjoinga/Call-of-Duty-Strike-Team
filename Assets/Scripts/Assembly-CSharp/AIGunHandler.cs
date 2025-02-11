using UnityEngine;

public class AIGunHandler : BaseActorComponent
{
	private bool mPausingBetweenBursts;

	private Actor mTarget;

	private static int ACCURACY_DELTA_AVERAGE_SAMPLE = 8;

	private float[] mAccuracyDeltas;

	private int mAccuracySampleIndex;

	private int mInaccuracyCount;

	private float mDebugBurstTime;

	private float mDebugPauseTime;

	private int mReloadCount;

	private float mExtraBurstPauseCoolnessWait;

	private float mNextSemiAutoFireTime;

	public bool WeaponsHold { get; set; }

	private void Awake()
	{
		mAccuracyDeltas = new float[ACCURACY_DELTA_AVERAGE_SAMPLE];
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public bool Process(float deltaTime, BaseCharacter owner, SoldierFiringState sfs, IWeapon weapon, out Actor target)
	{
		if (owner.IsFirstPerson)
		{
			target = null;
			return false;
		}
		weapon.ReleaseTrigger();
		target = mTarget;
		RealCharacter realCharacter = owner as RealCharacter;
		if (weapon.IsReloading())
		{
			mReloadCount++;
		}
		else if (mReloadCount > 0)
		{
			mInaccuracyCount = 0;
			mPausingBetweenBursts = false;
			mReloadCount = 0;
		}
		if (mPausingBetweenBursts)
		{
			if (!weapon.IsReloading())
			{
				mDebugPauseTime += deltaTime;
			}
			if (realCharacter.GetRecoil() > 0f)
			{
				mExtraBurstPauseCoolnessWait = GlobalBalanceTweaks.GetExtraBurstPause(owner);
				return false;
			}
			if (mExtraBurstPauseCoolnessWait > 0f)
			{
				mExtraBurstPauseCoolnessWait -= deltaTime;
				return false;
			}
			mDebugPauseTime = 0f;
			mPausingBetweenBursts = false;
		}
		if ((float)mInaccuracyCount > GlobalBalanceTweaks.kTolerableInaccuracyCount && sfs.desiredFireType != SoldierFiringState.FireType.FullAuto)
		{
			mPausingBetweenBursts = true;
			mInaccuracyCount = 0;
			mDebugBurstTime = 0f;
			return false;
		}
		if (!WeaponsHold && sfs.ShootingIsDesiredAndAllowed() && IsValidTarget(weapon, sfs.desiredTarget) && !weapon.IsReloading())
		{
			bool flag = true;
			mTarget = sfs.desiredTarget;
			target = mTarget;
			if (flag && realCharacter != null && mTarget.realCharacter != null)
			{
				float sqrMagnitude = (mTarget.GetPosition() - realCharacter.myActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude > realCharacter.FiringRange * realCharacter.FiringRange)
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	public void Fire(RealCharacter realOwner, Actor target, IWeapon weapon, out Vector3 shotPosition, out Vector3 shotTarget)
	{
		shotPosition = Vector3.zero;
		shotTarget = Vector3.zero;
		if (target == null)
		{
			return;
		}
		Vector3 vector = target.realCharacter.GetBulletOrigin();
		if (realOwner.myActor.behaviour != null && realOwner.myActor.behaviour.aimedShotTarget == target)
		{
			vector = target.realCharacter.GetHeadshotSpot();
		}
		Vector3 normalized = (vector - realOwner.GetBulletOrigin()).normalized;
		shotPosition = realOwner.GetBulletOrigin() + 0f * normalized;
		shotTarget = shotPosition + WeaponUtils.GetFirstPersonShotDirection(realOwner.myActor, normalized, weapon.GetThirdPersonAccuracy());
		realOwner.SetTarget(target);
		if (mAccuracySampleIndex == ACCURACY_DELTA_AVERAGE_SAMPLE - 1)
		{
			for (int i = 0; i < ACCURACY_DELTA_AVERAGE_SAMPLE - 1; i++)
			{
				mAccuracyDeltas[i] = mAccuracyDeltas[i + 1];
			}
		}
		mAccuracyDeltas[mAccuracySampleIndex] = realOwner.GetCurrentAccuracyDelta();
		if (mAccuracySampleIndex < ACCURACY_DELTA_AVERAGE_SAMPLE - 1)
		{
			mAccuracySampleIndex++;
		}
		float num = 0f;
		for (int j = 0; j < ACCURACY_DELTA_AVERAGE_SAMPLE; j++)
		{
			num += mAccuracyDeltas[j];
		}
		if (mAccuracySampleIndex == ACCURACY_DELTA_AVERAGE_SAMPLE - 1)
		{
			float num2 = num / (float)ACCURACY_DELTA_AVERAGE_SAMPLE;
			if (num2 == 0f)
			{
				mInaccuracyCount++;
			}
			else
			{
				mInaccuracyCount = 0;
			}
		}
		IWeaponControl weaponControl = WeaponUtils.GetWeaponControl(weapon);
		bool flag = false;
		if (weaponControl != null)
		{
			if (weaponControl.NeedsTriggerRelease())
			{
				if (Time.time < mNextSemiAutoFireTime)
				{
					flag = true;
				}
			}
			else if (Time.time > mNextSemiAutoFireTime)
			{
				flag = true;
				Vector2 semiAutoFireRates = weaponControl.GetSemiAutoFireRates();
				mNextSemiAutoFireTime = Time.time + Random.Range(semiAutoFireRates.x, semiAutoFireRates.y);
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			weapon.DepressTrigger();
		}
		mDebugBurstTime += Time.deltaTime;
	}

	public void PostProcess(float deltaTime)
	{
		mTarget = null;
	}

	public bool OnCooldown()
	{
		return mPausingBetweenBursts;
	}

	public bool IsValidTarget(IWeapon weapon, Actor desiredTarget)
	{
		if (desiredTarget == null)
		{
			return false;
		}
		if (desiredTarget.realCharacter.IsDead())
		{
			return false;
		}
		if (desiredTarget.realCharacter.IsMortallyWounded())
		{
			return false;
		}
		return true;
	}

	public void ClearSemiAutoDelay()
	{
		mNextSemiAutoFireTime = 0f;
	}

	public void DeadEye()
	{
		mInaccuracyCount = 0;
		mPausingBetweenBursts = false;
		mNextSemiAutoFireTime = 0f;
	}
}
