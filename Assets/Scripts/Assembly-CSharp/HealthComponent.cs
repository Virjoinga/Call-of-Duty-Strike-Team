using System;
using System.Collections;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
	public enum HealAmount
	{
		Completely = 0,
		ToMortallyWounded = 1
	}

	public class HeathChangeEventArgs : EventArgs
	{
		public GameObject From;

		public float Amount;

		public string DamageType;

		public Vector3 Direction;

		public Vector3 Force;

		public bool HeadShot;

		public bool OneShotKill;

		public bool LongShotKill;

		public HitLocation HitLocation;

		public SurfaceImpact Impact;

		public SpeechComponent.SpeechMode SpeechMode;

		public HeathChangeEventArgs(GameObject from, float amount, string damageType, Vector3 direction, bool headShot, bool oneShotKill, bool longShotKill)
		{
			From = from;
			Amount = amount;
			DamageType = damageType;
			Direction = direction;
			HeadShot = headShot;
			OneShotKill = oneShotKill;
			LongShotKill = longShotKill;
			HitLocation = null;
			Impact = null;
		}

		public HeathChangeEventArgs(GameObject from, float amount, string damageType, Vector3 direction, Vector3 force, bool headShot, bool oneShotKill, bool longShotKill, HitLocation hitLocation, SurfaceImpact impact)
		{
			From = from;
			Amount = amount;
			DamageType = damageType;
			Direction = direction;
			Force = force;
			HeadShot = headShot;
			OneShotKill = oneShotKill;
			LongShotKill = longShotKill;
			HitLocation = hitLocation;
			Impact = impact;
		}

		public HeathChangeEventArgs(GameObject from, float amount, string damageType, Vector3 direction)
			: this(from, amount, damageType, direction, false, false, false)
		{
		}
	}

	public class HealthChangeEnterEventArgs : EventArgs
	{
		public GameObject From;

		public string DamageType;

		public HitLocation HitLocation;

		public HealthChangeEnterEventArgs(GameObject from, string damageType, HitLocation hitLocation)
		{
			From = from;
			DamageType = damageType;
			HitLocation = hitLocation;
		}
	}

	public delegate void OnHealthChangeEnterEventHandler(object sender, HealthChangeEnterEventArgs args);

	public delegate void OnHeathChangeEventHandler(object sender, HeathChangeEventArgs args);

	public delegate void OnHeathMaxedEventHandler(object sender, HeathChangeEventArgs args);

	public delegate void OnHealthEmptyEventHandler(object sender, HeathChangeEventArgs args);

	public delegate void OnHealthOverTimeCompleteEventHandler(object sender, HeathChangeEventArgs args);

	public bool Invulnerable;

	public bool InvulnerableToExplosions;

	public bool OnlyDamagedByPlayer;

	public bool Rechargeable;

	public bool CanBeMortallyWounded;

	public static float DEFAULT_HEALTH = 400f;

	private static float RECHARGE_PREP_TIME = 3.5f;

	private static float HEALTH_REGENERATION_SPEED = 10f;

	private static float HEALTH_REGENERATION_SCOPE = 1f;

	private static float LOW_HEALTH_PERCENTAGE = 0.5f;

	private static float MORTALLY_WOUNDED_THRESHOLD = 0.25f;

	private float mMinHealth;

	private float mMinHealthClamped;

	private float mMaxHealth;

	private float mCurrentHealth;

	private float mCurrentHealth01;

	private float mRechargeMonitor;

	private bool mCanRecharge;

	private bool mCachedCanRecharge;

	private float mRechargeSpeed;

	private float mRechargeMultiplier;

	private bool mCachedInvulnerable;

	public Actor Owner;

	private float mTimeHeathLastReduced;

	private ModelSwapComponent m_ModelSwapComp;

	private bool mQuickFinishHealOverTime;

	public bool IsReviving { get; private set; }

	public float Health
	{
		get
		{
			return mCurrentHealth;
		}
	}

	public float HealthMax
	{
		get
		{
			return mMaxHealth;
		}
	}

	public float HealthMin
	{
		get
		{
			return mMinHealth;
		}
	}

	public float HealthMinClamped
	{
		set
		{
			mMinHealthClamped = Mathf.Max(value, mMinHealth);
		}
	}

	public float HealthDelta
	{
		get
		{
			return mCurrentHealth / mMaxHealth;
		}
	}

	public float Health01
	{
		get
		{
			return mCurrentHealth01;
		}
	}

	public bool HealthEmpty
	{
		get
		{
			return mCurrentHealth <= mMinHealth;
		}
	}

	public bool HealthLow
	{
		get
		{
			return mCurrentHealth / mMaxHealth < LOW_HEALTH_PERCENTAGE;
		}
	}

	public bool CanRecharge
	{
		set
		{
			mCanRecharge = value;
		}
	}

	public float TakeDamageModifier { get; set; }

	public bool DeferMortallyWounded { private get; set; }

	public float MortallyWoundedThreshold
	{
		get
		{
			return mMaxHealth * MORTALLY_WOUNDED_THRESHOLD;
		}
	}

	public float TimeHeathLastReduced
	{
		get
		{
			return mTimeHeathLastReduced;
		}
	}

	public event OnHealthChangeEnterEventHandler OnHealthChangeEnter;

	public event OnHeathChangeEventHandler OnHealthChange;

	public event OnHeathMaxedEventHandler OnHealthMaxed;

	public event OnHealthEmptyEventHandler OnHealthEmpty;

	public event OnHealthOverTimeCompleteEventHandler OnHealthOverTimeComplete;

	public void Initialise(float minHealth, float maxHealth, float startHealth)
	{
		mMinHealth = (mMinHealthClamped = minHealth);
		mMaxHealth = maxHealth;
		mCurrentHealth = startHealth;
		mCurrentHealth01 = GetRangeCoefficient(mCurrentHealth, mMinHealth, mMaxHealth);
		mRechargeSpeed = maxHealth / HEALTH_REGENERATION_SPEED;
		TakeDamageModifier = 1f;
	}

	public void QuickFinishHealOverTime()
	{
		mQuickFinishHealOverTime = true;
		StopCoroutine("DelayReenableAwareness");
		ReenableAwareness();
	}

	private IEnumerator DelayReenableAwareness(float timeInvulnerableFor)
	{
		yield return new WaitForSeconds(timeInvulnerableFor);
		ReenableAwareness();
	}

	private void ReenableAwareness()
	{
		Owner.awareness.CanBeLookedAt = true;
		Owner.awareness.FlushAllAwareness();
		if (CommonHudController.Instance != null)
		{
			CommonHudController.Instance.HUDInvulnerabilityEffect = false;
		}
	}

	private IEnumerator GiveHealthOverTime(GameObject from, float amount, string damageType, Vector3 direction, bool headshot, int framesToTake, float delayTime)
	{
		float endDelayTime = Time.time + delayTime;
		while (!mQuickFinishHealOverTime && Time.time < endDelayTime)
		{
			yield return 0;
		}
		int i = 0;
		while (!mQuickFinishHealOverTime && i < framesToTake)
		{
			ModifyHealth(from, amount, damageType, direction, headshot, false, null, null);
			yield return new WaitForSeconds(0.1f);
			i++;
		}
		mQuickFinishHealOverTime = false;
		IsReviving = false;
		RestoreCanRecharge();
		if (mCurrentHealth != mMaxHealth)
		{
			ModifyHealth(from, mMaxHealth - mCurrentHealth, damageType, direction, headshot, false, null, null);
		}
		if (this.OnHealthOverTimeComplete != null)
		{
			this.OnHealthOverTimeComplete(this, new HeathChangeEventArgs(from, amount, damageType, direction, headshot, false, false));
		}
	}

	public void ModifyHealthOverTime(GameObject from, float amount, string damageType, Vector3 direction, bool headshot, float timeToTake, bool isInvulnerable, float timeInvulnerableFor)
	{
		ModifyHealthOverTime(from, amount, damageType, direction, headshot, timeToTake, isInvulnerable, timeInvulnerableFor, 0f);
	}

	public void ModifyHealthOverTime(GameObject from, float amount, string damageType, Vector3 direction, bool headshot, float timeToTake, bool isInvulnerable, float timeInvulnerableFor, float delayTime)
	{
		if (timeToTake <= 0f)
		{
			return;
		}
		int num = (int)(timeToTake * 10f);
		float amount2 = amount / (float)num;
		if (num <= 0)
		{
			return;
		}
		StartCoroutine(GiveHealthOverTime(from, amount2, damageType, direction, headshot, num, delayTime));
		if (!isInvulnerable || !(Owner != null))
		{
			return;
		}
		IsReviving = true;
		Owner.awareness.CanBeLookedAt = false;
		Owner.awareness.FlushAllAwareness();
		StartCoroutine("DelayReenableAwareness", delayTime + timeToTake);
		if (!SectionTypeHelper.IsAGMG())
		{
			SoldierMarker selectedMarkerObj = Owner.behaviour.SelectedMarkerObj;
			if (selectedMarkerObj != null)
			{
				selectedMarkerObj.FlashWhileHealing(delayTime + timeInvulnerableFor);
			}
		}
	}

	public void ModifyHealth(GameObject from, float amount, string damageType, Vector3 direction, bool headshot, SpeechComponent.SpeechMode speechMode)
	{
		ModifyHealth(from, amount, damageType, direction, Vector3.zero, headshot, false, null, null, speechMode);
	}

	public void ModifyHealth(GameObject from, float amount, string damageType, Vector3 direction, bool headshot)
	{
		ModifyHealth(from, amount, damageType, direction, headshot, false, null, null);
	}

	public void ModifyHealth(GameObject from, float amount, string damageType, Vector3 direction, bool headshot, bool longShot)
	{
		ModifyHealth(from, amount, damageType, direction, headshot, longShot, null, null);
	}

	public void ModifyHealth(GameObject from, float amount, string damageType, Vector3 direction, bool headshot, bool longShot, HitLocation hitLocation, SurfaceImpact impact)
	{
		ModifyHealth(from, amount, damageType, direction, Vector3.zero, headshot, longShot, hitLocation, impact, SpeechComponent.SpeechMode.Normal);
	}

	public static bool DamageIsFromExplosion(string damageType)
	{
		return damageType == "Explosion" || damageType == "Claymore" || damageType == "Grenade";
	}

	public void ModifyHealth(GameObject from, float amount, string damageType, Vector3 direction, Vector3 force, bool headshot, bool longShot, HitLocation hitLocation, SurfaceImpact impact, SpeechComponent.SpeechMode speechMode)
	{
		HealthChangeEnterEventArgs args = new HealthChangeEnterEventArgs(from, damageType, hitLocation);
		if (this.OnHealthChangeEnter != null)
		{
			this.OnHealthChangeEnter(this, args);
		}
		bool isFromExplosion = DamageIsFromExplosion(damageType);
		if (amount < 0f)
		{
			amount = TakeDamageModifier * amount;
		}
		amount = ApplyInvulnerabilityModifier(amount, isFromExplosion);
		if (amount == 0f)
		{
			return;
		}
		if (IsAllowedToTakeDamage(from, damageType))
		{
			GlobalBalanceTweaks.ApplyDamageTweaks(damageType, base.gameObject, from, ref amount);
			float num = mCurrentHealth;
			mCurrentHealth += amount;
			mCurrentHealth = Mathf.Clamp(mCurrentHealth, mMinHealthClamped, mMaxHealth);
			mCurrentHealth01 = GetRangeCoefficient(mCurrentHealth, mMinHealth, mMaxHealth);
			bool oneShotKill = mCurrentHealth == 0f && num == mMaxHealth;
			float num2 = mCurrentHealth;
			HeathChangeEventArgs heathChangeEventArgs = new HeathChangeEventArgs(from, amount, damageType, direction, force, headshot, oneShotKill, longShot, hitLocation, impact);
			heathChangeEventArgs.SpeechMode = speechMode;
			if (amount < 0f)
			{
				mTimeHeathLastReduced = Time.time;
			}
			if (this.OnHealthChange != null)
			{
				this.OnHealthChange(this, heathChangeEventArgs);
			}
			if (num < mMaxHealth && num2 >= mMaxHealth && this.OnHealthMaxed != null)
			{
				this.OnHealthMaxed(this, heathChangeEventArgs);
			}
			if (num > mMinHealth && num2 <= mMinHealth && this.OnHealthEmpty != null)
			{
				this.OnHealthEmpty(this, heathChangeEventArgs);
			}
		}
		if (amount < 0f)
		{
			mRechargeMonitor = 0f;
		}
		if (m_ModelSwapComp != null)
		{
			m_ModelSwapComp.UpdateHealth(mCurrentHealth);
		}
	}

	private float ApplyInvulnerabilityModifier(float amount, bool isFromExplosion)
	{
		if (amount < 0f && (IsReviving || Invulnerable || (InvulnerableToExplosions && isFromExplosion)))
		{
			return 0f;
		}
		return amount;
	}

	public void ForceCriticallyInjured()
	{
		bool flag = false;
		if (Invulnerable)
		{
			Invulnerable = false;
			flag = true;
		}
		float num = (1f - MORTALLY_WOUNDED_THRESHOLD) * mMaxHealth;
		num += 1f;
		ModifyHealth(null, 0f - num, "InstaCripple", Vector3.up, false);
		if (flag)
		{
			Invulnerable = true;
		}
	}

	public bool IsMortallyWounded()
	{
		return CanBeMortallyWounded && HealthDelta < MORTALLY_WOUNDED_THRESHOLD;
	}

	public void Kill(string killTag, GameObject killer)
	{
		float takeDamageModifier = 1f;
		TakeDamageModifier = 1f;
		Invulnerable = false;
		ModifyHealth(killer, 0f - Health, killTag, Vector3.zero, false);
		TakeDamageModifier = takeDamageModifier;
	}

	private void Awake()
	{
		mCanRecharge = true;
		mRechargeMultiplier = 1f;
		mTimeHeathLastReduced = -1f;
		Initialise(0f, 100f, 100f);
		m_ModelSwapComp = GetComponent(typeof(ModelSwapComponent)) as ModelSwapComponent;
	}

	private void Update()
	{
		if (Rechargeable && mCanRecharge && !HealthEmpty)
		{
			mRechargeMonitor += Time.deltaTime;
			if (mRechargeMonitor >= RECHARGE_PREP_TIME && Health < mMaxHealth * HEALTH_REGENERATION_SCOPE)
			{
				if (GameSettings.Instance.PerksEnabled)
				{
					ModifyHealth(base.gameObject, mRechargeSpeed * mRechargeMultiplier * Time.deltaTime, "Recharge", Vector3.zero, false);
				}
				else
				{
					ModifyHealth(base.gameObject, mRechargeSpeed * Time.deltaTime, "Recharge", Vector3.zero, false);
				}
			}
		}
		if (DeferMortallyWounded)
		{
			CanBeMortallyWounded = true;
			ForceCriticallyInjured();
			DeferMortallyWounded = false;
		}
	}

	public void SetRechargeMultiplier(float multiplier)
	{
		mRechargeMultiplier = multiplier;
	}

	public float GetActualHealthFromDelta(float delta)
	{
		return HealthMax * delta;
	}

	public bool IsAllowedToTakeDamage(GameObject damageDealer, string damageType)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		Actor component = GetComponent<Actor>();
		if (component != null && component.realCharacter != null && component.realCharacter.IsBeingCarried)
		{
			return false;
		}
		if (!OnlyDamagedByPlayer || damageType == "ActOfGod" || damageType == "InstaCripple")
		{
			return true;
		}
		if (OnlyDamagedByPlayer && damageDealer != null && WorldHelper.IsPlayerControlledActor(damageDealer))
		{
			return true;
		}
		return false;
	}

	private float GetRangeCoefficient(float val, float min, float max)
	{
		return (val - min) / (max - min);
	}

	public void CacheAndSetCanRecharge(bool canRecharge)
	{
		mCachedCanRecharge = mCanRecharge;
		mCanRecharge = canRecharge;
	}

	public void RestoreCanRecharge()
	{
		mCanRecharge = mCachedCanRecharge;
	}

	public void CacheAndSetInvulnerable(bool isInvulnerable)
	{
		mCachedInvulnerable = Invulnerable;
		Invulnerable = isInvulnerable;
	}

	public void RestoreInvulnerable()
	{
		Invulnerable = mCachedInvulnerable;
	}
}
