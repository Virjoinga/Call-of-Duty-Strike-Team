using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealCharacter : BaseCharacter, ICharacterDefinition
{
	public const float kNonHumanNavAcceleration = 8f;

	public const float kDefaultNavAcceleration = 4f;

	public const float kSprintingNavAcceleration = 1f;

	public const float kRunningNavAcceleration = 5f;

	public const float kCrouchedHumanHeight = 0.9f;

	public const float kStandingHumanHeight = 1.8f;

	public const float kHalfCrouchedHumanHeight = 1.3499999f;

	private const float kCrouchingBulletHeight = 0.7f;

	private const float kStandingBulletHeight = 1.4f;

	private const float kKeepWatchingEnemyFor = 1f;

	private const float kKeepWatchingThreatFor = 1f;

	private const float kKeepWatchingUnawareEnemyFor = 1f;

	private const float kKeepWatchingOutOfSightEnemyFor = 1f;

	private const float kKeepLookingDefaultDirectionFor = 1f;

	private const float kKeepLookingMovementDirectionFor = 2f;

	public AssaultParams mPersistentAssaultParams = new AssaultParams();

	public AuditoryAwarenessComponent AuditoryAwareness;

	public string Id;

	public bool ForceOnScreen;

	public bool Selectable;

	public int ShowHUD;

	public bool ScriptHUDControl;

	public Vector3 lastVelocity = Vector3.zero;

	private int mBreatheCategory;

	private AnimDirector.ActionHandle mBreatheAction;

	private float targetBreathingRate = 1f;

	private float breathingRate = 1f;

	private float keepBreathingSameUntil;

	private int mPistolOverride;

	private static int mPreviousGrenadeCount = -1;

	private static int mPreviousClaymoreCount = -1;

	private static int mPreviousAmmoCount = -1;

	private static bool mPreviousReloadState;

	private static string mPreviousWeaponId = string.Empty;

	private static bool UpdateFPPHudText = true;

	private float currentColliderHeight;

	public HandGestureModule handGestureMod;

	public SearchArea mSearchArea;

	private Transform hitBone;

	private Transform mLockLocatorA;

	private Transform mLockLocatorB;

	private Vector3 mGrenadeDirection;

	private float mCrouchTransition;

	private float mHoldBreathStartTime;

	private float mFlinch;

	private float mRecoil;

	private bool mWantsToThrowGrenade;

	private float mHoldBreathModifier = 1f;

	private float mHipFireAccuracyModifier = 1f;

	private bool mHoldingBreathSFXPlaying;

	private float mDamageModifier = 1f;

	private CharacterLighting mCharacterLightingRef;

	private Actor mTarget;

	public List<ForceOnDeathTriggerVolume> ForceOnDeathTriggers = new List<ForceOnDeathTriggerVolume>();

	private static float NPC_CROUCH_ACCURACY_HIGH = 0.2f;

	private static float NPC_NOCROUCH_ACCURACY_HIGH = 0.1f;

	private static float NPC_CROUCH_ACCURACY_LOW = 0.05f;

	private static float NPC_NOCROUCH_ACCURACY_LOW;

	private bool mWaitingForBlood;

	private bool mIsRegisteredWithWorld;

	private float watchEnemyUntil;

	private float watchThreatUntil;

	private float watchUnawareEnemyUntil;

	private float watchOutOfSightEnemyUntil;

	private float watchDefaultDirectionUntil;

	private float watchMovementDirectionUntil;

	private GameObject POIProp;

	private bool mIsSniper;

	private float mRange;

	private float mRangeDegree;

	private float mFiringRange;

	private ContextMenuOptionType mCMRules;

	public Vector3 mStartForward = Vector3.zero;

	public float AliveTime { get; private set; }

	public float AliveTimeReal { get; private set; }

	public bool WasFirstPersonWhenMortallyWounded { get; set; }

	public float NoiseRadiusModifier { get; private set; }

	public float DamageModifier
	{
		get
		{
			return mDamageModifier;
		}
		set
		{
			mDamageModifier = value;
		}
	}

	public bool DontDropAmmo { get; set; }

	private float HoldBreathDuration
	{
		get
		{
			if (GameSettings.Instance.PerksEnabled)
			{
				return 4f * mHoldBreathModifier;
			}
			return 4f;
		}
	}

	private float HoldBreathRecoveryDuration
	{
		get
		{
			return 4f;
		}
	}

	private float HoldBreathTime
	{
		get
		{
			return Time.time - mHoldBreathStartTime;
		}
	}

	public bool CanHoldBreath
	{
		get
		{
			if (mHoldingBreathSFXPlaying != IsHoldingBreath && !IsHoldingBreath)
			{
				mHoldingBreathSFXPlaying = false;
				CharacterSFX.Instance.HoldBreathLoop.Stop2D();
				CharacterSFX.Instance.HoldBreathEnd.Play2D();
			}
			return IsHoldingBreath || HoldBreathTime > HoldBreathDuration + HoldBreathRecoveryDuration;
		}
	}

	public bool IsHoldingBreath
	{
		get
		{
			return HoldBreathTime < HoldBreathDuration;
		}
	}

	public float HoldingBreathFraction
	{
		get
		{
			return (!IsHoldingBreath) ? 0f : Mathf.Clamp01(HoldBreathTime / HoldBreathDuration);
		}
	}

	public float SwayAmount
	{
		get
		{
			float num = 0.5f;
			float num2 = 1f;
			if (myActor.weapon != null && myActor.weapon.ActiveWeapon != null)
			{
				IWeaponSway weaponSway = WeaponUtils.GetWeaponSway(myActor.weapon.ActiveWeapon);
				if (weaponSway != null)
				{
					num = weaponSway.GetSwayMinimum();
					num2 = weaponSway.GetSwayMaximum();
				}
			}
			if (IsHoldingBreath)
			{
				return num;
			}
			float num3 = 4f;
			float num4 = num3 * (HoldBreathTime - HoldBreathDuration);
			if (num4 < 1f)
			{
				return Mathf.Lerp(num, num2, Mathf.Clamp01(num4));
			}
			return Mathf.Lerp(num2, num, Mathf.Clamp01(0.1f * (num4 - 1f)));
		}
	}

	public float SwayFrequency
	{
		get
		{
			float num = 0.5f;
			float to = 0.05f;
			float from = 2f;
			if (myActor.weapon != null && myActor.weapon.ActiveWeapon != null)
			{
				IWeaponSway weaponSway = WeaponUtils.GetWeaponSway(myActor.weapon.ActiveWeapon);
				if (weaponSway != null)
				{
					num = weaponSway.GetSwayFrequency();
				}
			}
			if (IsHoldingBreath)
			{
				return Mathf.Lerp(num, to, Mathf.Clamp01(HoldingBreathFraction * 10f));
			}
			return Mathf.Lerp(from, num, Mathf.Clamp01(0.5f * (HoldBreathTime - HoldBreathDuration)));
		}
	}

	public CharacterLighting Lighting
	{
		get
		{
			return mCharacterLightingRef;
		}
	}

	public bool RespottedAfterDeath { get; set; }

	public bool WaitingForBlood
	{
		get
		{
			return mWaitingForBlood;
		}
		set
		{
			mWaitingForBlood = value;
		}
	}

	public float GrenadeDamageMultiplier { get; set; }

	public bool IsSniper
	{
		get
		{
			return mIsSniper;
		}
		set
		{
			mIsSniper = value;
		}
	}

	public bool IsWindowLookout
	{
		get
		{
			return myActor.tasks.GetRunningTask(typeof(TaskWindowLookout)) != null;
		}
	}

	public float Range
	{
		get
		{
			return mRange;
		}
		set
		{
			mRangeDegree = value;
			float f = (float)Math.PI / 180f * value;
			mRange = Mathf.Cos(f);
		}
	}

	public float RangeDegree
	{
		get
		{
			return mRangeDegree;
		}
	}

	public float FiringRange
	{
		get
		{
			return mFiringRange;
		}
		set
		{
			mFiringRange = value;
		}
	}

	public ContextMenuOptionType CMRules
	{
		get
		{
			if (mCMRules == ContextMenuOptionType.Undefined)
			{
				throw new NotImplementedException();
			}
			return mCMRules;
		}
		set
		{
			mCMRules = value;
		}
	}

	public static void SetUpdateFPPHudText()
	{
		UpdateFPPHudText = true;
	}

	public void AttackedBy(Actor a)
	{
		if (!myActor.behaviour.PlayerControlled)
		{
			SetBreathingRate(3.5f);
		}
	}

	public void SetBreathingRate(float rate)
	{
		if (rate >= targetBreathingRate)
		{
			targetBreathingRate = rate;
			keepBreathingSameUntil = Time.time + targetBreathingRate * 3f;
		}
		else if (keepBreathingSameUntil < Time.time)
		{
			targetBreathingRate = rate;
		}
	}

	public void StartHoldingBreath()
	{
		SoundManager.SoundInstance soundInstance = CharacterSFX.Instance.HoldBreathStart.Play2D();
		if (soundInstance != null)
		{
			StartCoroutine(DoBreathing(soundInstance.SampleLength));
			mHoldingBreathSFXPlaying = true;
			mHoldBreathStartTime = Time.time;
		}
	}

	public IEnumerator DoBreathing(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (mHoldingBreathSFXPlaying)
		{
			CharacterSFX.Instance.HoldBreathLoop.Play2D();
		}
	}

	public void HandleForcedExhale()
	{
		if (mHoldingBreathSFXPlaying)
		{
			mHoldingBreathSFXPlaying = false;
			CharacterSFX.Instance.HoldBreathLoop.Stop2D();
			CharacterSFX.Instance.HoldBreathEndForced.Play2D();
			mHoldBreathStartTime = Time.time - HoldBreathDuration;
		}
	}

	public bool IsShootingAt(Actor actor)
	{
		if (actor == null || mTarget == null)
		{
			return false;
		}
		if (actor == mTarget && myActor.weapon.IsFiring())
		{
			return true;
		}
		return false;
	}

	private void Awake()
	{
		mIsRegisteredWithWorld = false;
		Settings = Settings ?? new CharacterSettings();
		forcedCrouch = false;
		mWander = 0f;
		mWanderRate = 0.1f;
		Selectable = true;
		ShowHUD = 0;
		mRootBone = null;
		mNextFrameNavMesh = false;
		mTranslationTable = new Dictionary<string, int>();
		mTranslationTable.Add("Kill", 0);
		mTranslationTable.Add("SilentKill", 1);
		mTranslationTable.Add("SilentKillNeckSnap", 23);
		mTranslationTable.Add("Stealth", 2);
		mTranslationTable.Add("Unstealth", 3);
		mTranslationTable.Add("DisableShadow", 4);
		mTranslationTable.Add("EnableShadow", 5);
		mTranslationTable.Add("CoverInvalid", 6);
		mTranslationTable.Add("PhysAnim", 7);
		mTranslationTable.Add("PhysAnimOff", 8);
		mTranslationTable.Add("PopUp", 9);
		mTranslationTable.Add("Hunch", 9);
		mTranslationTable.Add("PeekOver", 9);
		mTranslationTable.Add("StepLeft", 9);
		mTranslationTable.Add("StepRight", 9);
		mTranslationTable.Add("PopDown", 10);
		mTranslationTable.Add("Stand", 13);
		mTranslationTable.Add("Crouch", 12);
		mTranslationTable.Add("ThrowGrenade", 11);
		mTranslationTable.Add("Bashed", 14);
		mTranslationTable.Add("DeRagdoll", 15);
		mTranslationTable.Add("Left Upper Arm", 16);
		mTranslationTable.Add("Left Forearm", 16);
		mTranslationTable.Add("Right Upper Arm", 18);
		mTranslationTable.Add("Right Forearm", 18);
		mTranslationTable.Add("Left Thigh", 17);
		mTranslationTable.Add("Left Calf", 17);
		mTranslationTable.Add("Right Thigh", 19);
		mTranslationTable.Add("Right Calf", 19);
		mTranslationTable.Add("Head", 20);
		mTranslationTable.Add("Pelvis", 21);
		mTranslationTable.Add("Torso", 22);
		mForceOffscreen = false;
		mStealth = false;
		mWantsToThrowGrenade = false;
		GrenadeDamageMultiplier = 1f;
		lastPosition = base.transform.position;
		targetPosition = base.transform.position;
		roundRobinIndex = BaseCharacter.roundRobinCounter++;
		SetUpTransForm();
		mCrouchTransition = ((!IsCrouching()) ? 1f : 0f);
	}

	private void OnDestroy()
	{
		if (GameplayController.Instance() != null)
		{
			GameplayController.Instance().DeregisterActor(myActor);
		}
		if (myActor.health != null)
		{
			myActor.health.OnHealthEmpty -= OnHealthEmpty;
			myActor.health.OnHealthChange -= OnHealthChange;
			myActor.health.OnHealthOverTimeComplete -= OnHealthOverTimeComplete;
		}
		mNavigationSetPiece = null;
		myActor = null;
	}

	public override void SetSelectable(bool OnOff, bool HUDOnOff)
	{
		SetSelectable(OnOff, HUDOnOff, false, false);
	}

	public void SetSelectable(bool OnOff, bool HUDOnOff, bool forceSelectChange)
	{
		SetSelectable(OnOff, HUDOnOff, forceSelectChange, false);
	}

	public void SetSelectable(bool OnOff, bool HUDOnOff, bool forceSelectChange, bool ignoreHUD)
	{
		if (ignoreHUD || ChangeHUDStatus(HUDOnOff) || forceSelectChange)
		{
			Selectable = OnOff;
		}
	}

	public bool ChangeHUDStatus(bool HUDOnOff)
	{
		if (HudMarker != null)
		{
			if (HUDOnOff)
			{
				ShowHUD--;
				if (ShowHUD == 0 || HudBlipIcon.AreAllSetForCutscene)
				{
					if (ScriptHUDControl)
					{
						SoldierMarker soldierMarker = HudMarker as SoldierMarker;
						if ((bool)soldierMarker)
						{
							soldierMarker.ToggleScriptOverrideMarker(true);
							return true;
						}
					}
					HudMarker.SwitchOn();
					return true;
				}
			}
			else
			{
				ShowHUD++;
				if (ShowHUD == 1 || HudBlipIcon.AreAllSetForCutscene)
				{
					if (ScriptHUDControl)
					{
						SoldierMarker soldierMarker2 = HudMarker as SoldierMarker;
						if ((bool)soldierMarker2)
						{
							soldierMarker2.ToggleScriptOverrideMarker(false);
							return true;
						}
					}
					HudMarker.SwitchOff();
					return true;
				}
			}
		}
		return false;
	}

	private void Start()
	{
		myActor.OnScreen = true;
		if (!IsDead())
		{
			SetupAnimationHandles();
			myActor.health.OnHealthEmpty += OnHealthEmpty;
			myActor.health.OnHealthChange += OnHealthChange;
			myActor.health.OnHealthOverTimeComplete += OnHealthOverTimeComplete;
			mIdleTimer = 0f;
			WasFirstPersonWhenMortallyWounded = false;
			if (myActor.awareness.ChDefCharacterType == CharacterType.Human)
			{
				handGestureMod = base.gameObject.AddComponent<HandGestureModule>().Connect(myActor) as HandGestureModule;
			}
			mAimBone = myActor.model.transform.FindInHierarchy("Bone027");
			mImposedLookDirectionValid = LookType.None;
			if (myActor.awareness.ChDefCharacterType == CharacterType.SentryGun)
			{
				hitBone = myActor.model.transform.FindInHierarchy("Sentrygunbarrel_LOD00") ?? myActor.model.transform;
				mLockLocatorA = hitBone;
				mLockLocatorB = hitBone;
			}
			else
			{
				hitBone = myActor.model.transform.FindInHierarchy("Bip002 Spine2") ?? myActor.model.transform;
				mLockLocatorA = hitBone;
				mLockLocatorB = myActor.model.transform.FindInHierarchy("Bip002 Head") ?? myActor.model.transform;
			}
			SetupDefaultTask();
			SetupForThirdPerson();
			ResetState();
			mCharacterLightingRef = GetComponentInChildren<CharacterLighting>();
			CachePositionGproj();
			mStartForward = myActor.transform.forward;
			CheckForAwarenessZone();
			if (myActor != null && myActor.behaviour != null && myActor.behaviour.PlayerControlled)
			{
				myActor.health.SetRechargeMultiplier(StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.Medic));
				mHoldBreathModifier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.IronLungs);
				NoiseRadiusModifier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.DeadSilence);
				mHipFireAccuracyModifier = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.SteadyAim);
			}
			CacheAnimHandles();
		}
	}

	private void CacheAnimHandles()
	{
		mBreatheCategory = myActor.animDirector.GetCategoryHandle("Breathing");
		if (mBreatheCategory != -1)
		{
			mBreatheAction = myActor.animDirector.GetActionHandle(mBreatheCategory, "Breathe");
			myActor.animDirector.PlayAction(mBreatheAction);
		}
		mPistolOverride = myActor.animDirector.GetOverrideHandle("Pistol");
	}

	public void CheckForAwarenessZone()
	{
		Collider component = base.gameObject.GetComponent<Collider>();
		if (component == null)
		{
			uint awarenessMembership = AwarenessZone.GetAwarenessMembership(base.transform.position);
			if (awarenessMembership != 2147483648u)
			{
				myActor.awareness.awarenessZonesImIn = awarenessMembership;
				AwarenessZone.AddToAwarenessZonesManually(awarenessMembership, myActor);
			}
		}
	}

	public void SetupForFirstPerson()
	{
		myActor.animDirector.AnimationPlayer.cullingType = AnimationCullingType.AlwaysAnimate;
		myActor.animDirector.AnimationPlayer.enabled = false;
		myActor.animDirector.AnimationPlayer.enabled = true;
		myActor.Pose.mAlwaysUpdateOrientation = true;
		myActor.Pose.DirtyInternalModelPosition();
		myActor.Pose.mWasOnScreen = true;
	}

	public void SetupForThirdPerson()
	{
		myActor.animDirector.AnimationPlayer.cullingType = AnimationCullingType.BasedOnRenderers;
		if ((bool)myActor.Pose)
		{
			myActor.Pose.mAlwaysUpdateOrientation = false;
		}
	}

	public void SetSearchArea(bool bDynamic)
	{
		if (!myActor.behaviour.PlayerControlled)
		{
			if (mSearchArea == null)
			{
				mSearchArea = new SearchArea(myActor.GetPosition(), bDynamic);
			}
			else
			{
				mSearchArea.SetNewSearchArea(myActor.GetPosition(), bDynamic);
			}
		}
	}

	public void SetSearchArea(Vector3 position, bool bDynamic)
	{
		if (!myActor.behaviour.PlayerControlled)
		{
			if (mSearchArea == null)
			{
				mSearchArea = new SearchArea(position, bDynamic);
			}
			else
			{
				mSearchArea.SetNewSearchArea(position, bDynamic);
			}
		}
	}

	public void SetSearchArea(Vector3 position, float sqrRadius, bool bDynamic)
	{
		if (!myActor.behaviour.PlayerControlled)
		{
			if (mSearchArea == null)
			{
				mSearchArea = new SearchArea(position, sqrRadius, bDynamic);
			}
			else
			{
				mSearchArea.SetNewSearchArea(position, sqrRadius, bDynamic);
			}
		}
	}

	public virtual void SetupDefaultTask()
	{
		if (myActor.awareness.ChDefCharacterType == CharacterType.RiotShieldNPC)
		{
			new TaskRiotShield(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default, new RiotShieldDescriptorConfig());
		}
		else if (myActor.awareness.ChDefCharacterType == CharacterType.RPG)
		{
			new TaskRoutine(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default, new List<TaskDescriptor>(), true);
			myActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.RPG);
		}
		else
		{
			new TaskRoutine(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default, new List<TaskDescriptor>(), false);
		}
	}

	protected void SetAcceleration(float val)
	{
		if (myActor.navAgent != null)
		{
			if (!myActor.OnScreen || myActor.awareness.ChDefCharacterType != CharacterType.Human || mCarried != null)
			{
				val = 8f;
			}
			myActor.navAgent.acceleration = val;
		}
	}

	private void ProcessBreathing()
	{
		if (mBreatheCategory == -1 || !myActor.OnScreen)
		{
			return;
		}
		if (mCarried != null)
		{
			myActor.animDirector.SetCategoryWeight(mBreatheCategory, 0f);
			return;
		}
		myActor.animDirector.SetCategoryWeight(mBreatheCategory, 1f);
		float to = 3f;
		float frac = 0.05f;
		float num = 1.5f;
		if (myActor.behaviour.PlayerControlled)
		{
			to = 2f;
			num = 1.5f;
			frac = 0.003f;
		}
		if (myActor.awareness.EngagedInCombat())
		{
			SetBreathingRate(num);
		}
		if (IsRunning())
		{
			SetBreathingRate(WorldHelper.ExpBlend(targetBreathingRate, to, frac));
		}
		SetBreathingRate(1f);
		WorldHelper.ExpBlend(ref breathingRate, targetBreathingRate, (!(targetBreathingRate > breathingRate)) ? 0.01f : 0.1f);
		myActor.animDirector.SetCategorySpeed(mBreatheCategory, breathingRate);
	}

	private void ApplyAnimOverrides()
	{
		if (mPistolOverride >= 0 && !(myActor.weapon == null) && myActor.weapon.ActiveWeapon != null)
		{
			myActor.animDirector.EnableOverride(mPistolOverride, myActor.weapon.ActiveWeapon.GetClass() == WeaponDescriptor.WeaponClass.Pistol);
		}
	}

	private void DetermineHandGestureEligibility()
	{
		if (handGestureMod == null)
		{
			return;
		}
		if (myActor.behaviour.alertState < BehaviourController.AlertState.Suspicious)
		{
			handGestureMod.SetValidGestureRequests(HandGestureModule.ForbidPriority.Context, HandGestureModule.GestureEnum.kNone);
			handGestureMod.SetValidGestureParticipation(HandGestureModule.ForbidPriority.Context, HandGestureModule.GestureEnum.kNone);
			return;
		}
		handGestureMod.SetValidGestureRequests(HandGestureModule.ForbidPriority.Context, HandGestureModule.GestureEnum.kAll);
		if (mCarried == null)
		{
			handGestureMod.SetValidGestureParticipation(HandGestureModule.ForbidPriority.Context, HandGestureModule.GestureEnum.kAll);
		}
		else
		{
			handGestureMod.SetValidGestureParticipation(HandGestureModule.ForbidPriority.Context, HandGestureModule.GestureEnum.kNone);
		}
	}

	private void UpdateNavMeshAgent()
	{
		NavMeshAgent navAgent = myActor.navAgent;
		if ((bool)navAgent)
		{
			navAgent.enabled = mNextFrameNavMesh;
			if (CharacterTypeHelper.CanMantle(myActor.awareness.ChDefCharacterType) && navAgent.isOnOffMeshLink)
			{
				navAgent.CompleteOffMeshLink();
			}
		}
		if (mNextFrameNavMesh && IsMortallyWounded())
		{
			EnableNavMesh(false);
		}
	}

	private void OneTimeRegistration()
	{
		GameplayController gameplayController = GameplayController.Instance();
		gameplayController.RegisterActor(myActor);
		mIsRegisteredWithWorld = true;
	}

	protected virtual void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		AliveTime += Time.deltaTime;
		AliveTimeReal += TimeManager.DeltaTime;
		ProcessBreathing();
		ApplyAnimOverrides();
		UpdateNavMeshAgent();
		DetermineHandGestureEligibility();
		if (!mIsRegisteredWithWorld)
		{
			OneTimeRegistration();
		}
		Vector3 vector = BaseCharacter.WorldSpaceToFrame(base.transform.position, mReferenceFrame);
		Vector3 newVel = Vector3.zero;
		if (myActor.navAgent != null && myActor.navAgent.enabled)
		{
			newVel = myActor.navAgent.velocity;
			if (myActor.navAgent.isOnOffMeshLink)
			{
				newVel.Set(0f, 0f, 0f);
			}
		}
		else if (IsBeingMovedManually())
		{
			newVel = (vector - lastPosition) / Time.deltaTime;
		}
		newVel.y = 0f;
		Vector3 vector2 = ((!(myActor.navAgent != null) || !myActor.navAgent.enabled) ? base.transform.position : myActor.navAgent.destination);
		float num = Vector3.SqrMagnitude(base.transform.position - vector2);
		float sqrMagnitude = newVel.sqrMagnitude;
		string newStateStr = "Walk";
		if (forcedCrouch || GetStance() == Stance.Crouched)
		{
			newStateStr = "Crawl";
		}
		float acceleration = 4f;
		switch (mMovementStyle)
		{
		case MovementStyle.Run:
		case MovementStyle.AsFastAsSafelyPossible:
			if (sqrMagnitude > 9f && num > 6.25f)
			{
				newStateStr = "Run";
				acceleration = ((!(sqrMagnitude > 16f)) ? 5f : 1f);
			}
			break;
		case MovementStyle.Walk:
			if (myActor.behaviour.alertState < BehaviourController.AlertState.Suspicious)
			{
				newStateStr = "Walk";
				if (myActor.navAgent != null)
				{
					myActor.navAgent.speed = GetGaitSpeed(MovementStyle.Walk) * 1f + Mathf.Sin(mWander) * 0.1f;
				}
			}
			break;
		}
		SetAcceleration(acceleration);
		if (IsFirstPerson)
		{
			mIdleTimer = 0f;
		}
		if (sqrMagnitude <= 0.040000003f)
		{
			if (mIdleTimer > 3f && (!IsCrouching() || !myActor.behaviour.PlayerControlled))
			{
				newStateStr = "Idle";
			}
			else
			{
				mIdleTimer += Time.deltaTime;
				newStateStr = ((!forcedCrouch && GetStance() != Stance.Crouched) ? "Stand" : "Crouch");
			}
			if (mStealth)
			{
				newStateStr = "StealthStand";
			}
		}
		else
		{
			mIdleTimer = 0f;
			if (mStealth)
			{
				newStateStr = "Stealth";
			}
		}
		if (!IsUsingNavMesh() && !IsBeingMovedManually())
		{
			newStateStr = "Puppet";
		}
		bool expensiveTick = OptimisationManager.Update(OptType.PoseModule, myActor);
		Vector3 aimDir = myActor.awareness.LookDirection;
		if (CharacterTypeHelper.IsAimDirectionLockedToFacing(myActor.awareness.ChDefCharacterType))
		{
			aimDir = base.transform.forward;
		}
		if (mWantsToThrowGrenade)
		{
			newStateStr = "ThrowGrenade";
			aimDir = mGrenadeDirection;
		}
		lastVelocity = Vector3.zero;
		if (!base.IsUsingFixedGun || myActor.behaviour.PlayerControlled)
		{
			myActor.Pose.UpdatePose(vector2, base.transform.position, newVel, aimDir, ref newStateStr, expensiveTick);
			if (CharacterTypeHelper.DoPosePostModuleUpdate(myActor.awareness.ChDefCharacterType) && mCarriedBy == null)
			{
				myActor.Pose.PostModuleUpdate();
			}
		}
		if (IsFirstPerson)
		{
			GameplayController gameplayController = GameplayController.Instance();
			bool flag = true;
			if (gameplayController != null)
			{
				if (CommonHudController.Instance.DropPressed && mCarried != null)
				{
					OrdersHelper.OrderDropImmediately(gameplayController);
					flag = false;
				}
				else if (CommonHudController.Instance.StealthKillPressed && CommonHudController.Instance.ActorToStealthKill != null)
				{
					OrdersHelper.OrderMeleeAttack(gameplayController, CommonHudController.Instance.ActorToStealthKill);
					flag = false;
				}
			}
			if (flag)
			{
				if (CommonHudController.Instance.MeleePressed)
				{
					if (!(myActor.weapon.ActiveWeapon is Weapon_Knife) && !(myActor.weapon.DesiredWeapon is Weapon_Knife))
					{
						myActor.weapon.SetTrigger(false);
						myActor.weapon.SwapTo(new Weapon_Knife(), 4f);
					}
				}
				else
				{
					myActor.weapon.SetTrigger(CommonHudController.Instance.TriggerPressed);
				}
			}
		}
		else if (!base.IsUsingFixedGun && !IsDead() && !IsMortallyWounded() && myActor.weapon != null && CharacterTypeHelper.ReleaseTriggerEveryFrame(myActor.awareness.ChDefCharacterType))
		{
			myActor.weapon.SetTrigger(false);
		}
		lastPosition = vector;
		forcedCrouch = false;
		float num2 = 4f;
		float b = 4f * Time.deltaTime;
		float num3 = 16f * Time.deltaTime;
		mCrouchTransition = Mathf.Clamp01(mCrouchTransition + num2 * ((!IsCrouching()) ? Time.deltaTime : (0f - Time.deltaTime)));
		if (mStandardCamera != null)
		{
			mStandardCamera.HeadHeight = Mathf.Lerp(InputSettings.FirstPersonHeightHeightCrouched, InputSettings.FirstPersonHeightHeightStanding, mCrouchTransition);
		}
		if (base.FirstPersonCamera != null)
		{
			float num4 = num3 * (0.2f * Mathf.Max(Mathf.Abs(base.FirstPersonCamera.RecoilAngles.x), Mathf.Abs(base.FirstPersonCamera.RecoilAngles.y)));
			mStandardCamera.RecoilAngles += new Vector3(Mathf.Clamp(0f - base.FirstPersonCamera.RecoilAngles.x, 0f - num4, num4), Mathf.Clamp(0f - base.FirstPersonCamera.RecoilAngles.y, 0f - num4, num4), 0f);
		}
		mFlinch -= Mathf.Min(mFlinch, b);
		mRecoil -= Mathf.Min(mRecoil, num3);
		UpdateWeapons();
		UpdateNoiseLevel();
	}

	private void LateUpdate()
	{
		if (myActor.weapon != null)
		{
			myActor.weapon.LateUpdate();
		}
		if (IsFirstPerson)
		{
			int num = 0;
			num = ((CommonHudController.Instance.CurrentFPPThrowingWeapon != 0) ? PlayerSquadManager.Instance.ClaymoreCount : PlayerSquadManager.Instance.GrenadeCount);
			if (UpdateFPPHudText)
			{
				if (CommonHudController.Instance.CurrentFPPThrowingWeapon == FPPThrowWeaponIcon.Grenade)
				{
					CommonHudController.Instance.GrenadeAmmoReadout.Text = PlayerSquadManager.Instance.GetGrenadeAmmoString();
				}
				else
				{
					CommonHudController.Instance.GrenadeAmmoReadout.Text = PlayerSquadManager.Instance.GetClaymoreAmmoString();
				}
				CommonHudController.Instance.AmmoReadout.Text = myActor.weapon.GetAmmoString();
				CommonHudController.Instance.AmmoProgressBar.Value = myActor.weapon.GetPercentageAmmoInClip();
				UpdateFPPHudText = false;
			}
			Color color = ((!myActor.weapon.IsReloading()) ? ColourChart.HudYellow : Color.red);
			if (CommonHudController.Instance.AmmoReadout.color != color)
			{
				CommonHudController.Instance.AmmoReadout.SetColor(color);
			}
			CommonHudController.Instance.AmmoProgressBar.renderer.material.color = ((!myActor.weapon.IsReloading()) ? Color.white : Color.red);
			Color color2 = ((num > 0) ? ColourChart.HudYellow : Color.red);
			if (CommonHudController.Instance.GrenadeAmmoReadout.color != color2)
			{
				CommonHudController.Instance.GrenadeAmmoReadout.SetColor(color2);
			}
		}
		CachePositionGproj();
		if (ForceOnScreen)
		{
			myActor.OnScreen = true;
		}
		else
		{
			float num2 = 1.4f;
			Vector3 vector = base.transform.position + base.transform.up;
			Vector3 vector2 = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(vector);
			float num3 = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(vector + num2 * CameraManager.Instance.CurrentCamera.transform.right).x - vector2.x;
			if ((vector2.z >= 0f && vector2.x >= 0f - num3 && vector2.x <= 1f + num3 && vector2.y >= 0f - num3 && vector2.y <= 1f + num3) || IsSniper)
			{
				myActor.OnScreen = !mForceOffscreen;
				if (GameController.Instance.IsFirstPerson)
				{
					if (GameplayController.Instance() != null)
					{
						Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
						if (mFirstPersonActor != null && mFirstPersonActor.realCharacter.CanUseFirstPersonHideOpt() && myActor.awareness.IsCoverDataValid && mFirstPersonActor.awareness.EstimateVisibility(myActor.awareness) == CoverTable.VisibilityEstimate.Impossible)
						{
							myActor.OnScreen = false;
						}
					}
				}
				else if (InteractionsManager.IsBusy() && Vector3.SqrMagnitude(base.transform.position - CameraManager.Instance.CurrentCamera.transform.position) > 4000f)
				{
					myActor.OnScreen = false;
				}
			}
			else if (GameController.Instance.IsFirstPerson)
			{
				if (base.IsBeingCarried || Vector3.SqrMagnitude(base.transform.position - CameraManager.Instance.CurrentCamera.transform.position) < ((!(vector2.z >= 0f)) ? 4f : 50f))
				{
					myActor.OnScreen = !mForceOffscreen;
				}
				else
				{
					myActor.OnScreen = false;
				}
			}
			else
			{
				myActor.OnScreen = false;
			}
			if (mCharacterLightingRef != null && mCharacterLightingRef.enabled != myActor.OnScreen)
			{
				mCharacterLightingRef.enabled = myActor.OnScreen;
			}
		}
		if (SimpleHitBounds != null)
		{
			if (!Ragdoll.Kinematic || IsMortallyWounded())
			{
				SimpleHitBounds.gameObject.SetActive(false);
			}
			else
			{
				SimpleHitBounds.gameObject.SetActive(true);
				if (!OptimisationManager.opt_ParentSimpleCollider)
				{
					SimpleHitBounds.transform.position = base.transform.position;
					SimpleHitBounds.transform.rotation = base.transform.rotation;
				}
				float num4 = Mathf.Lerp(0.9f, 1.8f, mCrouchTransition);
				if (num4 != currentColliderHeight)
				{
					currentColliderHeight = num4;
					CapsuleCollider capsuleCollider = SimpleHitBounds.collider as CapsuleCollider;
					capsuleCollider.height = num4;
					capsuleCollider.center = 0.5f * num4 * Vector3.up;
				}
			}
		}
		if (mCarried != null)
		{
			mCarried.awareness.LookDirection = myActor.model.transform.forward;
			if (IsFirstPerson)
			{
				mCarried.model.transform.position = base.transform.position;
				mCarried.model.transform.forward = base.FirstPersonCamera.transform.forward;
				base.FirstPersonCamera.LockPitch = true;
			}
			else
			{
				if (!IsInASetPiece)
				{
					mCarried.SetPosition(myActor.GetPosition());
					mCarried.Pose.onAxisTrans.position = myActor.GetPosition();
					mCarried.Pose.offAxisTrans.position = myActor.GetPosition();
				}
				mCarried.transform.forward = myActor.model.transform.forward;
				mCarried.Pose.PostModuleUpdate();
			}
			if (mCarried.realCharacter.Ragdoll != null && mCarried.realCharacter.Ragdoll.Pelvis != null && mCarried.realCharacter.Ragdoll.Pelvis.rigidbody != null)
			{
				mCarried.realCharacter.Ragdoll.Pelvis.transform.position = myActor.GetPosition() + new Vector3(0f, 1.5f, 0f);
				mCarried.realCharacter.Ragdoll.Pelvis.rigidbody.isKinematic = true;
			}
		}
		else if (!mIsBeingCarried && Ragdoll != null && Ragdoll.Pelvis != null && !Ragdoll.Kinematic)
		{
			myActor.SetPosition(Ragdoll.Pelvis.transform.position);
		}
	}

	public void ToggleWeapon()
	{
		myActor.weapon.Toggle();
	}

	protected void UpdateNoiseLevel()
	{
		myActor.awareness.currentNoiseRadius = 0f;
		myActor.awareness.dominantSound = DominantSoundType.Silence;
		if (myActor.weapon.IsFiring() && !myActor.weapon.IsShootingSilenced() && !IsDead() && !IsMortallyWounded())
		{
			myActor.awareness.currentNoiseRadius = AudioResponseRanges.Gunshot;
			myActor.awareness.dominantSound = DominantSoundType.Gunfire;
			if (myActor.weapon.GetTarget() != null && myActor.weapon.GetTarget().behaviour != null)
			{
				myActor.weapon.GetTarget().behaviour.ShotAt();
			}
		}
		if (IsMakingNoise() && myActor.awareness.currentNoiseRadius < AudioResponseRanges.Footsteps)
		{
			myActor.awareness.currentNoiseRadius = AudioResponseRanges.Footsteps;
			myActor.awareness.dominantSound = DominantSoundType.Footsteps;
		}
		if (myActor.behaviour.PlayerControlled && GameSettings.Instance.PerksEnabled)
		{
			myActor.awareness.currentNoiseRadius *= NoiseRadiusModifier;
		}
		myActor.awareness.currentNoiseRadiusSqr = myActor.awareness.currentNoiseRadius * myActor.awareness.currentNoiseRadius;
	}

	protected bool IsMakingNoise()
	{
		return IsRunning() && (!IsCrouching() || !IsFirstPerson);
	}

	protected override void UpdateWeapons()
	{
		if (IsDead() || IsMortallyWounded())
		{
			return;
		}
		if (myActor.weapon != null)
		{
			if (myActor.behaviour.alertState < BehaviourController.AlertState.Alerted && !myActor.behaviour.PlayerControlled && !myActor.OnScreen)
			{
				return;
			}
			myActor.weapon.CharacterPermitsFiring(!IsKnockedDown() && !base.IsBeingCarried && !IsFirstPerson && !IsMortallyWounded() && !IsDead());
			myActor.weapon.Pump();
			if ((roundRobinIndex & 0x1F) == (Time.frameCount & 0x1F))
			{
				if (myActor.weapon.IsFiring() && !myActor.behaviour.PlayerControlled && myActor.speech != null)
				{
					myActor.speech.PlayBattleChatter(myActor);
				}
				if (myActor.weapon.CanReload() && !myActor.weapon.IsFiring() && myActor.awareness.EnemiesIKnowAboutRecent == 0 && !IsFirstPerson)
				{
					myActor.weapon.Reload();
				}
			}
			if (mCharacterLightingRef != null && Shadow != null)
			{
				Shadow.ShadowColour = mCharacterLightingRef.ShadowColour;
			}
		}
		if (IsFirstPerson)
		{
			if (mPreviousClaymoreCount != PlayerSquadManager.Instance.ClaymoreCount || mPreviousGrenadeCount != PlayerSquadManager.Instance.GrenadeCount || mPreviousReloadState != myActor.weapon.IsReloading() || mPreviousWeaponId != myActor.weapon.Id || mPreviousAmmoCount != (int)(myActor.weapon.GetPercentageAmmoInClip() * 100f))
			{
				UpdateFPPHudText = true;
			}
			mPreviousClaymoreCount = PlayerSquadManager.Instance.ClaymoreCount;
			mPreviousGrenadeCount = PlayerSquadManager.Instance.GrenadeCount;
			mPreviousAmmoCount = (int)(myActor.weapon.GetPercentageAmmoInClip() * 100f);
			mPreviousReloadState = myActor.weapon.IsReloading();
			mPreviousWeaponId = myActor.weapon.Id;
			float accuracy = Mathf.Lerp(myActor.weapon.ActiveWeapon.GetFirstPersonAccuracy(), 0f, GetCurrentAccuracyForCrosshair());
			CommonHudController.Instance.Crosshair.Accuracy = accuracy;
		}
	}

	public override Vector3 GetCameraTransitionTarget()
	{
		if (IsFirstPerson)
		{
			return base.FirstPersonCamera.transform.position;
		}
		if (myActor.OnScreen && hitBone != null)
		{
			return hitBone.position;
		}
		return myActor.GetPosition() + 1.2f * Vector3.up;
	}

	public override Vector3 GetSoftLockPosition()
	{
		return GetSnapPosition();
	}

	public override Vector3 GetSnapPosition()
	{
		if (IsFirstPerson)
		{
			return base.FirstPersonCamera.transform.position;
		}
		if (myActor.OnScreen && mLockLocatorA != null && mLockLocatorB != null)
		{
			return Vector3.Lerp(mLockLocatorA.position, mLockLocatorB.position, GameController.Instance.TapToTargetBlend);
		}
		float num = Mathf.Lerp(0.9f, 1.3f, mCrouchTransition);
		return myActor.GetPosition() + num * Vector3.up;
	}

	public override Vector3 GetBulletOrigin()
	{
		float num = Mathf.Lerp(0.7f, 1.4f, mCrouchTransition);
		return myActor.GetPosition() + num * Vector3.up;
	}

	public override Vector3 GetHeadshotSpot()
	{
		if (SimpleHitBounds != null)
		{
			CapsuleCollider capsuleCollider = SimpleHitBounds.collider as CapsuleCollider;
			if (capsuleCollider != null)
			{
				return SimpleHitBounds.transform.position + Vector3.up * (capsuleCollider.height - 0.1f);
			}
		}
		return GetBulletOrigin();
	}

	public override Vector3 GetStandingBulletOrigin()
	{
		return myActor.GetPosition() + 1.4f * Vector3.up;
	}

	public float GetCurrentAccuracyForCrosshair()
	{
		return GetCurrentAccuracy(true);
	}

	public float GetCurrentAccuracy()
	{
		return GetCurrentAccuracy(false);
	}

	private float GetCurrentAccuracy(bool applyRecoilInFirstPerson)
	{
		if (myActor.behaviour.aimedShotTarget != null && myActor.behaviour.aimedShotTarget == myActor.weapon.GetTarget())
		{
			return 1f;
		}
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(myActor.weapon.ActiveWeapon);
		float t = ((weaponADS == null) ? 0f : weaponADS.GetHipsToSightsBlendAmount());
		float num = Mathf.Clamp01(Mathf.InverseLerp(0f, 1f, mFlinch));
		float num2 = ((applyRecoilInFirstPerson || !IsFirstPerson) ? Mathf.Clamp01(Mathf.InverseLerp(0f, 2f, mRecoil)) : 0f);
		float num3 = ((!IsFirstPerson) ? myActor.navAgent.velocity.magnitude : GameController.Instance.LastVelocity.magnitude);
		float num4 = Mathf.Clamp01(0.2f * num3);
		float t2 = Mathf.Clamp01(num + num2 + num4);
		float to = ((!IsFirstPerson) ? Mathf.Lerp(NPC_CROUCH_ACCURACY_LOW, NPC_NOCROUCH_ACCURACY_LOW, mCrouchTransition) : Mathf.Lerp(0.2f, 0f, mCrouchTransition));
		float from = ((!IsFirstPerson) ? Mathf.Lerp(NPC_CROUCH_ACCURACY_HIGH, NPC_NOCROUCH_ACCURACY_HIGH, mCrouchTransition) : Mathf.Lerp(0.5f, 0.4f, mCrouchTransition));
		float from2 = Mathf.Lerp(from, to, t2);
		float num5 = Mathf.Lerp(from2, 1f, t);
		float num6 = num5;
		if (myActor.weapon.GetTarget() != null && myActor.weapon.GetTarget().baseCharacter.IsFirstPerson)
		{
			num6 = FirstPersonPenaliser.ApplyAccuracyBonus(num5);
		}
		if (weaponADS != null && weaponADS.GetADSState() == ADSState.Hips && myActor.behaviour.PlayerControlled && myActor.baseCharacter.IsFirstPerson && GameSettings.Instance.PerksEnabled)
		{
			num6 *= mHipFireAccuracyModifier;
		}
		if (myActor.behaviour != null && !myActor.behaviour.PlayerControlled)
		{
			num6 = base.GMGTweaks.GMGModifier_EnemyAccuracy(num6);
		}
		return num6;
	}

	public float GetBestFeasibleAccuracy()
	{
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(myActor.weapon.ActiveWeapon);
		float num = ((!IsCrouching()) ? NPC_NOCROUCH_ACCURACY_HIGH : NPC_CROUCH_ACCURACY_HIGH);
		float from = num;
		return Mathf.Lerp(from, 1f, (weaponADS == null) ? 0f : weaponADS.GetHipsToSightsBlendAmount());
	}

	public float GetWorstFeasibleAccuracy()
	{
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(myActor.weapon.ActiveWeapon);
		float num = ((!IsCrouching()) ? NPC_NOCROUCH_ACCURACY_LOW : NPC_CROUCH_ACCURACY_LOW);
		float from = num;
		return Mathf.Lerp(from, 1f, (weaponADS == null) ? 0f : weaponADS.GetHipsToSightsBlendAmount());
	}

	public float GetCurrentAccuracyDelta()
	{
		float worstFeasibleAccuracy = GetWorstFeasibleAccuracy();
		float bestFeasibleAccuracy = GetBestFeasibleAccuracy();
		float num = bestFeasibleAccuracy - worstFeasibleAccuracy;
		float currentAccuracy = GetCurrentAccuracy();
		float num2 = currentAccuracy - worstFeasibleAccuracy;
		return num2 / num;
	}

	public override void PlayReload()
	{
		if ((myActor.OnScreen || myActor.baseCharacter.IsFirstPerson) && reloadActionHandle != null)
		{
			RawAnimation rawAnimation = myActor.animDirector.PlayAction(reloadActionHandle, true);
			if (rawAnimation != null)
			{
				float reloadDuration = myActor.weapon.ActiveWeapon.GetReloadDuration();
				myActor.animDirector.SetCategorySpeed(reloadActionHandle.CategoryID, rawAnimation.AnimClip.length / reloadDuration);
			}
		}
	}

	public override void ApplyRecoil(float amount)
	{
		mRecoil += amount;
		if (myActor.OnScreen && shootActionHandle != null)
		{
			myActor.animDirector.PlayAction(shootActionHandle, true);
		}
	}

	public float GetRecoil()
	{
		return mRecoil;
	}

	public override void Flinch(Vector3 projectileOrigin)
	{
		if (GameController.Instance.ShouldFlinchInFirstPerson)
		{
			mFlinch = Mathf.Max(mFlinch, UnityEngine.Random.value);
			if (base.FirstPersonCamera != null)
			{
				base.FirstPersonCamera.Flinch(projectileOrigin, mFlinch * 2f);
			}
		}
	}

	public Vector2 GetDesiredFirstPersonVelocity(Vector2 moveAmount, float sprintZoneInterpolator)
	{
		float b = Mathf.Lerp(1f, InputSettings.FirstPersonSprintZoneEndMultiplier, sprintZoneInterpolator);
		float num = 0.4f;
		bool playerControlled = myActor.behaviour != null && myActor.behaviour.PlayerControlled;
		float num2 = 5f * (myActor.weapon.ActiveWeapon.GetRunSpeed(playerControlled, IsFirstPerson) * 0.01f);
		return moveAmount * Mathf.Lerp(num, Mathf.Max(num, b), mCrouchTransition) * num2;
	}

	public float GetSensitivityModifier()
	{
		return GameController.Instance.FirstPersonFieldOfViewSensitivityModifer.Evaluate(base.FirstPersonCamera.Fov);
	}

	public override void ResetState()
	{
		mIsMoving = false;
		mStance = Stance.Standing;
		mMovementStyle = MovementStyle.Walk;
		mCarried = null;
		mPOITimer = 0f;
	}

	public bool CanBeControlledInFirstPerson()
	{
		return !IsMoving() && !IsInASetPiece;
	}

	public bool CanUseFirstPersonHideOpt()
	{
		if (mReferenceFrame != null)
		{
			return false;
		}
		return true;
	}

	public override void SetReferenceFrame(Transform frame)
	{
		Vector3 worldPosition = BaseCharacter.FrameToWorldSpace(lastPosition, mReferenceFrame);
		lastPosition = BaseCharacter.WorldSpaceToFrame(worldPosition, frame);
		base.SetReferenceFrame(frame);
	}

	public override void ShootAtTarget(Actor target)
	{
		mIdleTimer = 0f;
		mTarget = target;
		if (mTarget.realCharacter == null)
		{
			base.ShootAtTarget(target);
		}
		else
		{
			myActor.weapon.SetTarget(target);
		}
	}

	public void SetTarget(Actor target)
	{
		mTarget = target;
	}

	public bool IsFlanking(Actor target)
	{
		Vector3 rhs = target.GetPosition() - myActor.GetPosition();
		rhs.Normalize();
		Vector3 lookDirection = myActor.awareness.LookDirection;
		if (Vector3.Dot(lookDirection, rhs) < 0f)
		{
			return false;
		}
		Vector3 lookDirection2 = target.awareness.LookDirection;
		if (Vector3.Dot(lookDirection2, rhs) < 0f)
		{
			return false;
		}
		return true;
	}

	public void ThrowGrenadeFirstPerson(float cookedTime)
	{
		GameObject gameObject = SceneNanny.Instantiate(ExplosionManager.Instance.Grenade) as GameObject;
		gameObject.name = base.name + "_firstPersonGrenade";
		Vector3 direction = base.FirstPersonCamera.transform.forward + 0.2f * base.FirstPersonCamera.transform.up;
		Grenade component = gameObject.GetComponent<Grenade>();
		component.LaunchFromFirstPerson(myActor, direction, cookedTime);
		if (myActor.behaviour.PlayerControlled)
		{
			PlayerSquadManager instance = PlayerSquadManager.Instance;
			instance.ReduceGrenadeCount();
		}
	}

	public void ThrowGrenade(Vector3 target)
	{
		TBFAssert.DoAssert(myActor.grenadeThrower != null, string.Format("ThrowGrenade called without a GrenadeThrowerComponent existing on {0}", base.name));
		EventHub.Instance.Report(new Events.GrenadeThrown());
		Vector3 vector = target - myActor.GetPosition();
		if (vector.sqrMagnitude > 0f)
		{
			TurnToFaceDirection(vector.normalized);
		}
		myActor.grenadeThrower.Throw();
		if (myActor.behaviour.PlayerControlled)
		{
			PlayerSquadManager instance = PlayerSquadManager.Instance;
			instance.ReduceGrenadeCount();
		}
		mWantsToThrowGrenade = false;
	}

	public void CancelThrowGrenade()
	{
		TBFAssert.DoAssert(myActor.grenadeThrower != null, string.Format("ThrowGrenade called without a GrenadeThrowerComponent existing on {0}", base.name));
		mWantsToThrowGrenade = false;
	}

	public override bool IsSelectable()
	{
		if (!Selectable)
		{
			return false;
		}
		if (IsMortallyWounded())
		{
			return false;
		}
		if (IsDead())
		{
			return false;
		}
		return true;
	}

	public override bool IsDead()
	{
		if (myActor.awareness.ChDefCharacterType == CharacterType.AutonomousGroundRobot)
		{
			return myActor.health.Health <= 0f;
		}
		return base.IsDead();
	}

	public override bool IsMortallyWounded()
	{
		return myActor.health.IsMortallyWounded();
	}

	public override void Kill(string damageType)
	{
		myActor.health.ModifyHealth(base.gameObject, 0f - myActor.health.Health, damageType, Vector3.zero, false);
	}

	public bool CanBeSuppressed()
	{
		if (myActor.awareness.closestCoverPoint == null)
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < GameplayController.instance.Selected.Count; i++)
		{
			Actor actor = GameplayController.instance.Selected[i];
			if (actor != null && actor.awareness.EstimateVisibility(myActor.awareness) != 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		if (myActor.awareness.IsInCover())
		{
			return true;
		}
		if (myActor.realCharacter != null && myActor.realCharacter.IsUsingFixedGun)
		{
			return true;
		}
		if (myActor.awareness.ChDefCharacterType == CharacterType.SentryGun)
		{
			return true;
		}
		return false;
	}

	private void OnHealthOverTimeComplete(object sender, EventArgs args)
	{
		if (GameController.Instance.GMGReviveModeActive)
		{
			GameController.Instance.EndGMGReviveMode();
		}
	}

	private void OnHealthEmpty(object sender, EventArgs args)
	{
		CharacterType chDefCharacterType = myActor.awareness.ChDefCharacterType;
		if (chDefCharacterType == CharacterType.RiotShieldNPC || chDefCharacterType == CharacterType.RPG || chDefCharacterType == CharacterType.Human)
		{
			myActor.awareness.crouchingEyeLevel = NewCoverPointManager.deadHeightOffset;
			myActor.awareness.standingEyeLevel = NewCoverPointManager.deadHeightOffset;
		}
		if (mBreatheCategory != -1)
		{
			myActor.animDirector.StopAction(mBreatheAction);
			myActor.animDirector.EnableCategory(mBreatheCategory, false, 0f);
		}
		GKM.HideFromFriends(myActor);
		GlobalKnowledgeManager.Instance().aliveMask &= ~myActor.ident;
		GameController.Instance.NotifyCombatantKilled();
		if (myActor.awareness.closestCoverPoint != null)
		{
			myActor.awareness.closestCoverPoint.DeathNearby(myActor);
		}
		if (SimpleHitBounds != null && SimpleHitBounds.collider != null)
		{
			SimpleHitBounds.collider.enabled = false;
		}
		HealthComponent.HeathChangeEventArgs heathChangeEventArgs = (HealthComponent.HeathChangeEventArgs)args;
		if (myActor.awareness.ChDefCharacterType == CharacterType.AutonomousGroundRobot)
		{
			myActor.Command("MeleeDeath");
			new TaskDisabled(myActor.tasks, TaskManager.Priority.REACTIVE, Task.Config.Default);
			return;
		}
		if (myActor.awareness.ChDefCharacterType == CharacterType.SentryGun)
		{
			myActor.Command("MeleeDeath");
			TriggerKillEvent(heathChangeEventArgs);
			new TaskDead(myActor.tasks, TaskManager.Priority.REACTIVE, Task.Config.Default, null);
			return;
		}
		VocalSFXHelper.DeathCry(myActor, heathChangeEventArgs);
		EnableNavMesh(false);
		if (!myActor.tasks.IsRunningTask(typeof(TaskDead)))
		{
			TriggerKillEvent(heathChangeEventArgs);
			new TaskDead(myActor.tasks, TaskManager.Priority.REACTIVE, Task.Config.Default, heathChangeEventArgs.From);
			myActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.Corpse);
			myActor.Command(heathChangeEventArgs.DamageType);
			myActor.animDirector.AnimationPlayer.animatePhysics = false;
			GameplayController gameplayController = GameplayController.Instance();
			gameplayController.BroadcastEventDeath(myActor, heathChangeEventArgs);
			bool flag = OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.Ragdoll);
			if (Ragdoll != null && CharacterTypeHelper.CanRagdoll(myActor.awareness.ChDefCharacterType) && KillTypeHelper.IsNotAStealthKill(heathChangeEventArgs) && KillTypeHelper.IsNotASuicide(heathChangeEventArgs) && myActor.realCharacter.myActor.OnScreen && flag)
			{
				myActor.animDirector.enabled = false;
				myActor.animDirector.AnimationPlayer.enabled = false;
				Ragdoll.SwitchToRagdoll();
				if (heathChangeEventArgs.HitLocation != null && heathChangeEventArgs.HitLocation.rigidbody != null)
				{
					if (!myActor.behaviour.PlayerControlled && ForceOnDeathTriggers != null && ForceOnDeathTriggers.Count > 0 && ForceOnDeathTriggers[0].ApplyForce(heathChangeEventArgs.HitLocation, heathChangeEventArgs.Impact))
					{
						ForceOnDeathTriggers.RemoveAt(0);
					}
					else if (!heathChangeEventArgs.HitLocation.rigidbody.isKinematic)
					{
						if (myActor.realCharacter != null && myActor.realCharacter.IsSniper)
						{
							float num = Vector3.Dot(myActor.awareness.LookDirection, heathChangeEventArgs.Impact.direction);
							if (num > 0.2f)
							{
								float num2 = 300f;
								heathChangeEventArgs.HitLocation.rigidbody.AddForceAtPosition((0f - num2) * -myActor.realCharacter.GetStanceDirection(), heathChangeEventArgs.Impact.position, ForceMode.Impulse);
							}
							else
							{
								heathChangeEventArgs.HitLocation.rigidbody.AddForceAtPosition(heathChangeEventArgs.Force, heathChangeEventArgs.Impact.position, ForceMode.Impulse);
							}
						}
						else
						{
							float[] siblingForceFraction = heathChangeEventArgs.HitLocation.SiblingForceFraction;
							if (siblingForceFraction != null && siblingForceFraction.Length == Ragdoll.Bones.Length)
							{
								for (int i = 0; i < siblingForceFraction.Length; i++)
								{
									Ragdoll.Bones[i].rigidbody.AddForceAtPosition(heathChangeEventArgs.Force * siblingForceFraction[i], heathChangeEventArgs.Impact.position, ForceMode.Impulse);
								}
							}
							else
							{
								heathChangeEventArgs.HitLocation.rigidbody.AddForceAtPosition(heathChangeEventArgs.Force, heathChangeEventArgs.Impact.position, ForceMode.Impulse);
							}
						}
					}
				}
			}
			else if (!myActor.behaviour.PlayerControlled && Ragdoll != null && KillTypeHelper.IsAStealthKill(heathChangeEventArgs) && ForceOnDeathTriggers != null && ForceOnDeathTriggers.Count > 0)
			{
				ForceOnDeathTriggers[0].ApplySilentKillForce(Ragdoll.Bones[0].rigidbody, -myActor.model.transform.forward);
			}
			if (myActor.awareness.ChDefCharacterType == CharacterType.Human && heathChangeEventArgs.DamageType != "SilentNeckSnap")
			{
				StartCoroutine(SpillBlood());
			}
			if (!IsSniper && myActor.awareness.ChDefCharacterType != CharacterType.RPG && !DontDropAmmo && AmmoDropManager.Instance != null)
			{
				AmmoDropManager.Instance.DropAmmo(myActor.navAgent.transform.position, KillTypeHelper.IsAStealthKill(heathChangeEventArgs));
			}
		}
		if (myActor.weapon != null && KillTypeHelper.IsNotAStealthKill(heathChangeEventArgs) && KillTypeHelper.IsNotASuicide(heathChangeEventArgs))
		{
			myActor.weapon.Drop();
		}
		WaypointMarkerManager.Instance.RemoveMarker(base.gameObject);
		WaypointMarkerManager.Instance.RemoveMarker(myActor.model);
		DestroyHudMarker();
	}

	private void TriggerKillEvent(HealthComponent.HeathChangeEventArgs hce)
	{
		if (hce.From != null && !myActor.behaviour.PlayerControlled && !RespottedAfterDeath)
		{
			Actor component = hce.From.GetComponent<Actor>();
			if (EventHub.Instance != null && component != null)
			{
				Events.EventActor attacker = ((!(component == null)) ? component.EventActor() : null);
				EventHub.Instance.Report(new Events.Kill(attacker, myActor.EventActor(), hce.DamageType, hce.HeadShot, hce.OneShotKill, hce.LongShotKill));
			}
		}
	}

	public void DestroyHudMarker()
	{
		if (HudMarker != null)
		{
			HudMarker.SwitchOff();
			UnityEngine.Object.Destroy(HudMarker.gameObject);
		}
	}

	private IEnumerator SpillBlood()
	{
		GameObject bloodPool2 = null;
		while (this != null && Ragdoll != null)
		{
			if (bloodPool2 == null && Ragdoll.Bones[0].rigidbody != null && Ragdoll.Bones[0].rigidbody.IsSleeping() && !Ragdoll.Kinematic)
			{
				RaycastHit hit;
				NavMeshHit navHit;
				if (Physics.Raycast(Ragdoll.Bones[0].transform.position, Vector3.down, out hit, 1f, 1 << LayerMask.NameToLayer("Default")) && NavMesh.SamplePosition(hit.point, out navHit, 0.2f, myActor.navAgent.walkableMask))
				{
					bloodPool2 = EffectsController.Instance.GetBloodPool(navHit.position - 0.02f * navHit.normal);
				}
				break;
			}
			yield return null;
		}
	}

	private void ProcessFlinch(HealthComponent.HeathChangeEventArgs healthArgs)
	{
		if (reactionCategoryHandle == -1 || !myActor.OnScreen || IsDead() || myActor.animDirector.GetCategoryLength(reactionCategoryHandle) != 0f || !(healthArgs.HitLocation != null))
		{
			return;
		}
		bool flag = Vector3.Dot(healthArgs.Direction, myActor.model.transform.forward) < 0f;
		AnimDirector.ActionHandle actionHandle = null;
		switch (ParseCommand(healthArgs.HitLocation.name))
		{
		case CommandEnum.kLeftLeg:
		case CommandEnum.kRightLeg:
		case CommandEnum.kHead:
		case CommandEnum.kPelvis:
		case CommandEnum.kTorso:
			switch (UnityEngine.Random.Range(0, 1))
			{
			case 0:
				actionHandle = ((!flag) ? flinch_LeftArmBackHandle : flinch_LeftArmFrontHandle);
				break;
			case 1:
				actionHandle = ((!flag) ? flinch_RightArmBackHandle : flinch_RightArmFrontHandle);
				break;
			}
			break;
		case CommandEnum.kLeftArm:
			actionHandle = ((!flag) ? flinch_LeftArmBackHandle : flinch_LeftArmFrontHandle);
			break;
		case CommandEnum.kRightArm:
			actionHandle = ((!flag) ? flinch_RightArmBackHandle : flinch_RightArmFrontHandle);
			break;
		}
		if (actionHandle != null)
		{
			myActor.animDirector.PlayAction(actionHandle, 0f, true);
			myActor.animDirector.SetCategoryWeight(actionHandle.CategoryID, 0.5f);
		}
	}

	private void OnHealthChange(object sender, EventArgs args)
	{
		HealthComponent.HeathChangeEventArgs heathChangeEventArgs = args as HealthComponent.HeathChangeEventArgs;
		ProcessFlinch(heathChangeEventArgs);
		if (!(myActor.health.Health > 0f))
		{
			return;
		}
		if (IsMortallyWounded() && !myActor.tasks.IsRunningTask(typeof(TaskMortallyWounded)))
		{
			if (heathChangeEventArgs.From != null && myActor.behaviour.PlayerControlled)
			{
				Actor component = heathChangeEventArgs.From.GetComponent<Actor>();
				if (EventHub.Instance != null)
				{
					Events.EventActor attacker = ((!(component == null)) ? component.EventActor() : null);
					EventHub.Instance.Report(new Events.Kill(attacker, myActor.EventActor(), heathChangeEventArgs.DamageType, heathChangeEventArgs.HeadShot, heathChangeEventArgs.OneShotKill, heathChangeEventArgs.LongShotKill));
				}
			}
			GameplayController gameplayController = GameplayController.Instance();
			gameplayController.BroadcastEventAboutToBeMortallyWounded(myActor);
			if (myActor.InitiatingGameFail)
			{
				myActor.health.ModifyHealth(myActor.gameObject, myActor.health.HealthMax, "FixedGunHeal", Vector3.down, false);
				return;
			}
			WasFirstPersonWhenMortallyWounded = IsFirstPerson;
			myActor.tasks.CancelTasksExcluding(typeof(TaskRoutine));
			new TaskMortallyWounded(myActor.tasks, TaskManager.Priority.REACTIVE, Task.Config.Default);
			WaypointMarkerManager.Instance.RemoveMarker(base.gameObject);
			WaypointMarkerManager.Instance.RemoveMarker(myActor.model);
			if (myActor.awareness.ChDefCharacterType != CharacterType.AutonomousGroundRobot)
			{
				if (SectionTypeHelper.IsAGMG())
				{
					IsAimingDownSights = false;
					AnimationClip specialCaseAnim = GameplayController.Instance().GetSpecialCaseAnim("Die");
					if (specialCaseAnim != null)
					{
						myActor.firstThirdPersonWidget.Reset(myActor, specialCaseAnim, 1f, myActor.transform.position, myActor.transform.rotation, string.Empty, false);
						myActor.firstThirdPersonWidget.HoldAnimFinishPose();
						myActor.speech.GMGDeath();
					}
				}
				else
				{
					int categoryHandle = myActor.animDirector.GetCategoryHandle("Wounded");
					AnimDirector.ActionHandle actionHandle = myActor.animDirector.GetActionHandle(categoryHandle, "FromBackHit");
					AnimDirector.ActionHandle actionHandle2 = myActor.animDirector.GetActionHandle(actionHandle.CategoryID, "FromBackIdle");
					RawAnimation rawAnimation = myActor.animDirector.PlayAction(actionHandle);
					myActor.animDirector.ChainAction(actionHandle.CategoryID, actionHandle2, (rawAnimation == null) ? 0f : rawAnimation.AnimClip.length);
				}
			}
			if (myActor.behaviour.PlayerControlled)
			{
				bool flag = true;
				int num = 1;
				int num2 = 4;
				ObjectiveManager objectiveManager = null;
				objectiveManager = ((!(GlobalObjectiveManager.Instance != null)) ? (UnityEngine.Object.FindObjectOfType(typeof(ObjectiveManager)) as ObjectiveManager) : GlobalObjectiveManager.Instance.CurrentObjectiveManager);
				if (objectiveManager != null)
				{
					num2 = objectiveManager.MaxNumberOfCasualties();
				}
				ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask & ~myActor.ident);
				Actor a;
				while (actorIdentIterator.NextActor(out a))
				{
					if (!a.realCharacter.IsMortallyWounded())
					{
						flag = false;
					}
					num++;
				}
				if (flag || (num >= num2 && PlayerSquadManager.Instance.MedkitCount < num2))
				{
					MusicManager.Instance.PlayMortallyWoundedThemeMusic();
					if (flag || PlayerSquadManager.Instance.MedkitCount < num || (ActStructure.Instance != null && ActStructure.Instance.CurrentMissionIsSpecOps()) || (num >= num2 && PlayerSquadManager.Instance.MedkitCount < num))
					{
						PlayerSquadManager instance = PlayerSquadManager.Instance;
						if (myActor != null && instance != null)
						{
							GameController.Instance.SuppressHud(true);
							StartCoroutine(DisplayContinueScreen(myActor.firstThirdPersonWidget.GetCurrentAnimLength()));
						}
					}
				}
				if (!SecureStorage.Instance.HasViewedReviveTutorial)
				{
					SecureStorage.Instance.HasViewedReviveTutorial = true;
					MessageBoxController instance2 = MessageBoxController.Instance;
					if (instance2 != null)
					{
						instance2.DoHintDialogue("S_MTX_HEAL_REVIVE", "S_MTX_HEAL_TUTORIAL", HintMessageBox.ImageLayout.Left, "Tutorial_Images/tutorial_MortallyWounded", null, string.Empty);
					}
				}
			}
			if (!SectionTypeHelper.IsAGMG())
			{
				gameplayController.RemoveFromSelected_NoAutoSelect(myActor);
			}
		}
		if (IsMortallyWounded())
		{
			return;
		}
		WasFirstPersonWhenMortallyWounded = false;
		if (!myActor.behaviour.PlayerControlled || !(heathChangeEventArgs.From != null))
		{
			return;
		}
		Actor component2 = heathChangeEventArgs.From.GetComponent<Actor>();
		if (component2 != null && !WorldHelper.IsPlayerControlledActor(component2))
		{
			EnemyBlip enemyBlip = component2.realCharacter.HudMarker as EnemyBlip;
			if (enemyBlip != null)
			{
				enemyBlip.AttackPulse();
			}
		}
	}

	private IEnumerator DisplayContinueScreen(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		FrontEndController.Instance.TransitionTo(ScreenID.ContinueScreen);
	}

	public GameObject PickSomethingToAimAt(Actor nearestVisibleEnemy)
	{
		float num = mIdleTimer;
		Vector3 dir = Vector3.zero;
		mIdleTimer = 0f;
		if (forceTarget != null)
		{
			TurnToFacePosition(forceTarget.transform.position);
			return forceTarget;
		}
		if (ImposedLookDirectionValid(ref dir))
		{
			TurnToFaceDirection(dir);
			return null;
		}
		TaskShoot taskShoot = (TaskShoot)myActor.tasks.GetRunningTask(typeof(TaskShoot));
		if (taskShoot != null)
		{
			return taskShoot.ManageLookAt();
		}
		if (nearestVisibleEnemy != null && (nearestVisibleEnemy.behaviour.alertState >= BehaviourController.AlertState.Suspicious || nearestVisibleEnemy == myActor.behaviour.aimedShotTarget))
		{
			TurnToFacePosition(nearestVisibleEnemy.baseCharacter.GetBulletOrigin());
			watchEnemyUntil = Time.time + 1f;
			return nearestVisibleEnemy.gameObject;
		}
		if (watchEnemyUntil > Time.time)
		{
			return null;
		}
		Vector3 pos = Vector3.zero;
		if (handGestureMod != null && handGestureMod.GetAimAt(ref pos))
		{
			TurnToFacePosition(pos);
			return null;
		}
		Actor nearestKnownThreat = myActor.awareness.GetNearestKnownThreat();
		if (nearestKnownThreat != null)
		{
			TurnToFacePosition(nearestKnownThreat.baseCharacter.GetBulletOrigin());
			watchThreatUntil = Time.time + 1f;
			return nearestKnownThreat.gameObject;
		}
		if (watchThreatUntil > Time.time)
		{
			return null;
		}
		Vector3 lastKnownPosition;
		Actor nearestKnownEnemy = myActor.awareness.GetNearestKnownEnemy(out lastKnownPosition, true);
		if (myActor.OnScreen && (myActor.behaviour.PlayerControlled || nearestKnownEnemy == null || (GKM.InCrowdOf(nearestKnownEnemy).obstructed & myActor.ident) != 0) && CheckForPOI())
		{
			if (nearestKnownEnemy == null)
			{
				mIdleTimer = num;
			}
			TurnToFaceDirection(mPOIDir);
			return null;
		}
		if (nearestVisibleEnemy != null)
		{
			TurnToFacePosition(nearestVisibleEnemy.baseCharacter.GetBulletOrigin());
			watchUnawareEnemyUntil = Time.time + 1f;
			return nearestVisibleEnemy.gameObject;
		}
		if (watchUnawareEnemyUntil > Time.time)
		{
			return null;
		}
		if (nearestKnownEnemy != null)
		{
			TurnToFacePosition(lastKnownPosition);
			watchOutOfSightEnemyUntil = Time.time + 1f;
			return null;
		}
		if (watchOutOfSightEnemyUntil > Time.time)
		{
			return null;
		}
		mIdleTimer = num;
		if (DefaultLookDirectionValid(ref dir))
		{
			watchDefaultDirectionUntil = Time.time + 1f;
			TurnToFaceDirection(dir);
			return null;
		}
		if (watchDefaultDirectionUntil > Time.time)
		{
			return null;
		}
		if (myActor.navAgent != null && myActor.navAgent.enabled && myActor.navAgent.velocity.xz().sqrMagnitude > 0.01f)
		{
			TurnToFaceDirection(myActor.navAgent.velocity);
			watchMovementDirectionUntil = Time.time + 2f;
			return null;
		}
		if (watchMovementDirectionUntil > Time.time)
		{
			return null;
		}
		TurnToFaceDirection(myActor.model.transform.forward);
		return null;
	}

	public bool CheckForPOI()
	{
		if (myActor.awareness.closestCoverPoint == null || (!myActor.behaviour.PlayerControlled && myActor.behaviour.alertState < BehaviourController.AlertState.Suspicious))
		{
			return false;
		}
		int[] neighbours = myActor.awareness.closestCoverPoint.neighbours;
		if (neighbours == null || neighbours.GetLength(0) == 0)
		{
			return false;
		}
		int length = neighbours.GetLength(0);
		Vector3 position = myActor.GetPosition();
		Vector3 lookDirection = myActor.awareness.LookDirection;
		uint num = ~(myActor.ident | GKM.InCrowdOf(myActor).friendlyMembers);
		if (mPOITimer > 0f)
		{
			mPOITimer -= Time.deltaTime;
			for (int i = 0; i < length; i++)
			{
				CoverPointCore coverPointCore = CoverNeighbour.CoverPoint(neighbours[i]);
				if ((coverPointCore.interestingTo & myActor.ident) == 0)
				{
					continue;
				}
				Vector3 vector = coverPointCore.gamePos - position;
				float num2 = Vector3.Dot(coverPointCore.gamePos - position, lookDirection);
				if (num2 > 0f)
				{
					num2 = num2 * num2 / vector.sqrMagnitude;
					if (num2 > 0.6f)
					{
						coverPointCore.interestingTo &= num;
					}
				}
			}
			return true;
		}
		for (int i = 0; i < length; i++)
		{
			CoverPointCore coverPointCore2 = CoverNeighbour.CoverPoint(neighbours[i]);
			if ((coverPointCore2.interestingTo & myActor.ident) != 0)
			{
				Vector3 vector = coverPointCore2.gamePos - position;
				float num2 = Vector3.Dot(coverPointCore2.gamePos - position, myActor.awareness.LookDirection);
				if (!(num2 < 0f))
				{
					coverPointCore2.interestingTo &= num;
					mPOIDir = coverPointCore2.gamePos - myActor.GetPosition();
					mPOITimer = 1f;
					return true;
				}
			}
		}
		return false;
	}

	private void PlacePOIProp(Vector3 pos)
	{
		if (POIProp == null)
		{
			POIProp = GameObject.CreatePrimitive(PrimitiveType.Cube);
			POIProp.name = "POIPROP";
			POIProp.transform.localScale = new Vector3(0.3f, 3.3f, 0.3f);
			UnityEngine.Object.DestroyImmediate(POIProp.GetComponent<BoxCollider>());
		}
		POIProp.transform.position = pos;
	}

	public override void Command(string com)
	{
		switch (ParseCommand(com))
		{
		case CommandEnum.kPopUp:
		case CommandEnum.kStand:
			Stand();
			break;
		case CommandEnum.kPopDown:
		case CommandEnum.kCrouch:
			Crouch();
			break;
		case CommandEnum.kKill:
			Kill(null);
			break;
		case CommandEnum.kSilentKill:
			Kill("Silent");
			break;
		case CommandEnum.kSilentKillNeckSnap:
			Kill("SilentNeckSnap");
			break;
		case CommandEnum.kStealth:
			mStealth = true;
			break;
		case CommandEnum.kUnstealth:
			mStealth = false;
			break;
		case CommandEnum.kDisableShadow:
			if (Shadow != null)
			{
				Shadow.FadeOutAndHide();
			}
			break;
		case CommandEnum.kEnableShadow:
			if (Shadow != null)
			{
				Shadow.UnHideAndFadeIn();
			}
			break;
		case CommandEnum.kAnimatePhysics:
			myActor.model.animation.animatePhysics = true;
			break;
		case CommandEnum.kDeRagdoll:
			if (Ragdoll != null)
			{
				if (!Ragdoll.Kinematic)
				{
					Ragdoll.SwitchToKinematic();
				}
				Ragdoll.ReawakenSkinnedMesh();
			}
			myActor.animDirector.enabled = true;
			myActor.animDirector.AnimationPlayer.enabled = true;
			break;
		case CommandEnum.kAnimatePhysicsOff:
			myActor.model.animation.animatePhysics = false;
			break;
		case CommandEnum.kThrowGrenade:
			mWantsToThrowGrenade = true;
			mGrenadeDirection = mImposedLookDirection;
			break;
		case CommandEnum.kBashed:
			if (GameController.Instance.mFirstPersonActor == myActor && base.FirstPersonCamera != null)
			{
				base.FirstPersonCamera.Flinch(myActor.GetPosition(), 20f);
			}
			break;
		case CommandEnum.kCoverInvalid:
		case CommandEnum.kLeftArm:
		case CommandEnum.kLeftLeg:
		case CommandEnum.kRightArm:
		case CommandEnum.kRightLeg:
		case CommandEnum.kHead:
		case CommandEnum.kPelvis:
		case CommandEnum.kTorso:
			break;
		}
	}

	public bool StartRunning()
	{
		if (mCarried == null)
		{
			myActor.tasks.Command("Run");
			GKM.OrderedToStand(myActor);
			return true;
		}
		return false;
	}

	public bool IsAHumanCharacter()
	{
		return myActor.awareness.ChDefCharacterType == CharacterType.Human;
	}
}
