using System;
using UnityEngine;

public class Weapon_OverwatchMachineGun : IWeapon, IWeaponADS, IWeaponAI, IWeaponStats
{
	private const float mStartLoopFrac = 0.5f;

	private const float mOneOvermStartLoopFrac = 2f;

	private static float CONSECUTIVEFIRING_HEATUP = 0.45f;

	private static float CONSECUTIVEFIRING_COOLDOWN = 2f;

	private static float CONSECUTIVEFIRING_MAX = 3f;

	private static float CONSECUTIVEFIRING_OVERHEATWARNING = 2.5f;

	private static int ROCKET_CLIP_SIZE = 5;

	private static float ROCKET_CLIP_RELOAD_TIME = 5f;

	private OverwatchController mOwner;

	private StrategyHudController mHUD;

	private bool mTrigger;

	private float mSpinUpAmount;

	private float mOldSpinUpAmount;

	private float mConsecutiveFiringTime;

	private float mRocketReloadTimer;

	private int mRocketClipRemaining;

	private float mRocketClipReloadTimer;

	private bool mWarningTriggered;

	private bool mPlayingSpinLoop;

	private bool mPlayingFireLoop;

	private bool mCanPlayStart;

	private bool mHasGoneDownOrOff;

	private float mCannonStartVol;

	private float mCannonStartFadeMult = 5f;

	public bool ReloadingRocket
	{
		get
		{
			return mRocketReloadTimer > 0f;
		}
	}

	public float RocketClipReady01
	{
		get
		{
			float num = Mathf.Clamp(mRocketClipReloadTimer, 0f, ROCKET_CLIP_RELOAD_TIME);
			return 1f - num / ROCKET_CLIP_RELOAD_TIME;
		}
	}

	public int NumRocketsRemaining
	{
		get
		{
			return mRocketClipRemaining;
		}
	}

	public Weapon_OverwatchMachineGun(OverwatchController owner)
	{
		mOwner = owner;
		Reset();
	}

	public object QueryInterface(Type t)
	{
		return this;
	}

	public void Reset()
	{
		mTrigger = false;
		mSpinUpAmount = 0f;
		mRocketClipRemaining = ROCKET_CLIP_SIZE;
	}

	public void Update(float deltaTime, BaseCharacter owner, SoldierFiringState sfs)
	{
		if (mHUD == null)
		{
			mHUD = StrategyHudController.Instance;
		}
		if (TimeManager.instance != null && TimeManager.instance.GlobalTimeState == TimeManager.State.IngamePaused)
		{
			return;
		}
		mOldSpinUpAmount = mSpinUpAmount;
		float num = deltaTime * 3f;
		mSpinUpAmount = Mathf.Clamp01(mSpinUpAmount + ((!mTrigger) ? (0f - num) : num));
		DoSpinLoop(mSpinUpAmount, mOldSpinUpAmount);
		if (mSpinUpAmount >= 1f)
		{
			if (mConsecutiveFiringTime < CONSECUTIVEFIRING_MAX)
			{
				mConsecutiveFiringTime += deltaTime * CONSECUTIVEFIRING_HEATUP;
			}
			if (mConsecutiveFiringTime < CONSECUTIVEFIRING_MAX)
			{
				StartFireLoop();
				for (int i = 0; i < 2; i++)
				{
					SurfaceImpact impact = ProjectileManager.Instance.StartProjectile(null, mOwner.OverwatchLC.transform.position - mOwner.OverwatchLC.transform.right * 1f + mOwner.OverwatchLC.transform.up * 0.3f, mOwner.GetTarget() + GetImpactNoise(mOwner.TargetSpread), CalculateDamage, CalculateImpactForce, true);
					WeaponUtils.TriggerProjectileEffects(owner, mOwner.OverwatchLC.transform.position - mOwner.OverwatchLC.transform.right * 1f + mOwner.OverwatchLC.transform.up * 0.3f, impact, true);
				}
			}
			else
			{
				StopFireLoop();
			}
			if (mConsecutiveFiringTime > CONSECUTIVEFIRING_OVERHEATWARNING && !mWarningTriggered)
			{
				mWarningTriggered = true;
				OverwatchSFX.Instance.OverheatWarning2D.Play2D();
			}
		}
		else
		{
			StopFireLoop();
			if (mConsecutiveFiringTime > 0f)
			{
				mConsecutiveFiringTime -= deltaTime * CONSECUTIVEFIRING_COOLDOWN;
			}
			if (mConsecutiveFiringTime < CONSECUTIVEFIRING_OVERHEATWARNING && mWarningTriggered)
			{
				mWarningTriggered = false;
				OverwatchSFX.Instance.OverheatWarning2D.Stop2D();
			}
		}
		float num2 = Mathf.Clamp(mConsecutiveFiringTime, 0f, CONSECUTIVEFIRING_MAX);
		num2 /= CONSECUTIVEFIRING_MAX;
		mHUD.SetChainGunHeat(Mathf.Clamp(num2, 0f, 1f));
		if (mRocketClipReloadTimer > 0f)
		{
			mRocketClipReloadTimer -= deltaTime;
		}
		else if (mRocketReloadTimer > 0f)
		{
			mRocketReloadTimer -= deltaTime;
		}
	}

	private void DoSpinLoop(float spinAmount, float oldSpinUpAmount)
	{
		if (spinAmount < oldSpinUpAmount || mSpinUpAmount == 0f)
		{
			mHasGoneDownOrOff = true;
		}
		if (spinAmount > oldSpinUpAmount && mHasGoneDownOrOff)
		{
			mCanPlayStart = true;
			mHasGoneDownOrOff = false;
		}
		if (!mPlayingSpinLoop && spinAmount > 0.5f)
		{
			mPlayingSpinLoop = true;
			WeaponSFX.Instance.OverwatchCannonLoop.Play(OverwatchController.Instance.OverwatchLC.gameObject);
		}
		else if (!mPlayingSpinLoop && mCanPlayStart)
		{
			mCannonStartVol = 1f;
			WeaponSFX.Instance.OverwatchCannonStart.Stop(OverwatchController.Instance.OverwatchLC.gameObject);
			WeaponSFX.Instance.OverwatchCannonStart.Play(OverwatchController.Instance.OverwatchLC.gameObject);
			mCanPlayStart = false;
		}
		if (spinAmount < oldSpinUpAmount || spinAmount == 0f)
		{
			mCannonStartVol -= Time.deltaTime * mCannonStartFadeMult;
			Mathf.Clamp01(mCannonStartVol);
			SoundManager.Instance.SetVolume(WeaponSFX.Instance.OverwatchCannonStart, OverwatchController.Instance.OverwatchLC.gameObject, mCannonStartVol);
		}
		if (mPlayingSpinLoop)
		{
			if (spinAmount <= 0.5f)
			{
				StopSpinLoop();
				return;
			}
			float num = mSpinUpAmount - 0.5f;
			num = ((!(num > 0f)) ? 0f : (num * 2f));
			float volume = WeaponSFX.Instance.OverwatchCannonLoop.m_volume * num;
			SoundManager.Instance.SetVolume(WeaponSFX.Instance.OverwatchCannonLoop, OverwatchController.Instance.OverwatchLC.gameObject, volume);
		}
	}

	private void StopSpinLoop()
	{
		if (mPlayingSpinLoop)
		{
			mPlayingSpinLoop = false;
			WeaponSFX.Instance.OverwatchCannonLoop.Stop(OverwatchController.Instance.OverwatchLC.gameObject);
		}
	}

	private void StartFireLoop()
	{
		if (!mPlayingFireLoop)
		{
			mPlayingFireLoop = true;
			WeaponSFX.Instance.OverwatchCannonFire.Play(OverwatchController.Instance.OverwatchLC.gameObject);
		}
	}

	private void StopFireLoop()
	{
		if (mPlayingFireLoop)
		{
			mPlayingFireLoop = false;
			WeaponSFX.Instance.OverwatchCannonFire.Stop(OverwatchController.Instance.OverwatchLC.gameObject);
		}
	}

	public void StopLoops()
	{
		StopSpinLoop();
		StopFireLoop();
		if (mWarningTriggered)
		{
			mWarningTriggered = false;
			OverwatchSFX.Instance.OverheatWarning2D.Stop2D();
		}
	}

	public void DepressTrigger()
	{
		mTrigger = true;
	}

	public void ReleaseTrigger()
	{
		mTrigger = false;
	}

	public void StartReloading(BaseCharacter owner)
	{
	}

	public void ReloadImmediately()
	{
	}

	public void CancelReload()
	{
	}

	public void PutAway()
	{
	}

	public void TakeOut()
	{
	}

	public void SwitchToHips()
	{
	}

	public void SwitchToSights()
	{
	}

	public void Drop()
	{
	}

	public ADSState GetADSState()
	{
		return ADSState.Hips;
	}

	public float GetEquipedBlendAmount()
	{
		return 1f;
	}

	public float GetHipsToSightsBlendAmount()
	{
		return 0f;
	}

	public float GetLeftRightLeanAmount()
	{
		return 0f;
	}

	public float GetUpDownLeanAmount()
	{
		return 0f;
	}

	public float GetMovementAmount()
	{
		return 0f;
	}

	public bool HasAmmo()
	{
		return true;
	}

	public bool LowAmmo()
	{
		return false;
	}

	public float GetPercentageAmmo()
	{
		return 1f;
	}

	public string GetAmmoString()
	{
		return string.Empty;
	}

	public float GetFirstPersonAccuracy()
	{
		return 1f;
	}

	public float GetThirdPersonAccuracy()
	{
		return 1f;
	}

	public float GetRunSpeed(bool playerControlled, bool isFirstPerson)
	{
		return 0f;
	}

	public float GetDesiredFieldOfView()
	{
		return InputSettings.FirstPersonFieldOfView;
	}

	public float GetCrosshairOpacity()
	{
		return 0f;
	}

	public string GetId()
	{
		return "HelicopterMachineGun";
	}

	public WeaponDescriptor.WeaponClass GetClass()
	{
		return WeaponDescriptor.WeaponClass.Special;
	}

	public int GetWeaponType()
	{
		return 0;
	}

	public float AccuracyStatAdjustment()
	{
		return 0f;
	}

	public bool HasScope()
	{
		return false;
	}

	public bool IsSilenced()
	{
		return false;
	}

	public bool IsFiring()
	{
		return false;
	}

	public bool CanReload()
	{
		return false;
	}

	public bool IsReloading()
	{
		return false;
	}

	public bool IsPuttingAway()
	{
		return false;
	}

	public bool IsTakingOut()
	{
		return false;
	}

	public bool IsLongRangeShot(float distSquared)
	{
		return false;
	}

	public bool IsHeadShotAllowed(float distSquared)
	{
		return false;
	}

	public float CalculateDamage(float distance, HitLocation target, bool isPlayer)
	{
		if (target != null && target.Actor != null)
		{
			if (target.Actor.behaviour.PlayerControlled)
			{
				return 300f;
			}
			return 800f;
		}
		return 100f;
	}

	public float CalculateImpactForce()
	{
		return 140f;
	}

	public float CalculateUtility(float distance)
	{
		return 1f;
	}

	public bool PointingAtTarget(Vector3 pos, float radius)
	{
		return true;
	}

	public float GetSuppressionRadius()
	{
		return 0f;
	}

	public void FireCannon()
	{
		if (!ReloadingRocket && !(mRocketClipReloadTimer > 0f))
		{
			GameObject gameObject = mOwner.OverwatchLC.gameObject;
			GameObject gameObject2 = SceneNanny.Instantiate(ExplosionManager.Instance.RPGOverwatch) as GameObject;
			gameObject2.name = gameObject.name + "_RPG";
			Vector3 target = mOwner.GetTarget();
			RPGProjectile component = gameObject2.GetComponent<RPGProjectile>();
			component.OverrideLaunchSpeed(600f);
			component.OverrideDistanceToStartCollision(0.1f);
			component.OverrideLaunchPosition(mOwner.OverwatchLC.transform.position + mOwner.OverwatchLC.transform.right * 1f + mOwner.OverwatchLC.transform.up * 0.3f);
			component.Launch(gameObject, (target - gameObject.transform.position).normalized.normalized, false);
			mRocketReloadTimer = mOwner.RocketReloadTime;
			WeaponSFX.Instance.OverwatchRocketFire.Play(OverwatchController.Instance.OverwatchLC.gameObject);
			mRocketClipRemaining--;
			if (mRocketClipRemaining <= 0)
			{
				mRocketClipRemaining = ROCKET_CLIP_SIZE;
				mRocketClipReloadTimer = ROCKET_CLIP_RELOAD_TIME;
			}
		}
	}

	private Vector3 GetImpactNoise(float range)
	{
		Vector3 result = UnityEngine.Random.insideUnitSphere * range;
		result.y = 0f;
		return result;
	}

	public WeaponAmmo GetWeaponAmmo()
	{
		return null;
	}

	public float GetReloadDuration()
	{
		return 1f;
	}
}
