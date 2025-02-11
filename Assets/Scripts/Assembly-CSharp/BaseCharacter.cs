using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BaseCharacter : BaseActorComponent
{
	public enum Nationality
	{
		Friendly = 0,
		Russian = 1,
		Arabic = 2,
		Chinese = 3
	}

	protected enum CommandEnum
	{
		kUndefined = -1,
		kKill = 0,
		kSilentKill = 1,
		kStealth = 2,
		kUnstealth = 3,
		kDisableShadow = 4,
		kEnableShadow = 5,
		kCoverInvalid = 6,
		kAnimatePhysics = 7,
		kAnimatePhysicsOff = 8,
		kPopUp = 9,
		kPopDown = 10,
		kThrowGrenade = 11,
		kCrouch = 12,
		kStand = 13,
		kBashed = 14,
		kDeRagdoll = 15,
		kLeftArm = 16,
		kLeftLeg = 17,
		kRightArm = 18,
		kRightLeg = 19,
		kHead = 20,
		kPelvis = 21,
		kTorso = 22,
		kSilentKillNeckSnap = 23
	}

	protected enum LookType
	{
		None = 0,
		Direction = 1,
		Position = 2,
		Target = 3
	}

	public enum Stance
	{
		Prone = 0,
		Crouched = 1,
		Standing = 2
	}

	public enum MovementStyle
	{
		Walk = 0,
		Run = 1,
		AsFastAsSafelyPossible = 2
	}

	public const float kFirstPersonWalkSpeed = 2f;

	public const float kFirstPersonWalkSpeedSqrSafe = 4.1f;

	public const float kPlayerSprintSpeedModifier = 1.25f;

	public const float kNonPlayerSprintSpeedModifier = 1.1f;

	public const float kWalkToStanceMinVelocity = 0.2f;

	public const float kWalkToStanceMinVelocitySqr = 0.040000003f;

	public const float kRunToStanceMinVelocity = 3f;

	public const float kRunToStanceMinVelocitySqr = 9f;

	public const float kNoRunNearDestinationSqr = 6.25f;

	public const int kMaxPoseModules = 4;

	public const float kStandToIdleTime = 3f;

	protected Dictionary<string, int> mTranslationTable;

	public static float m_sDebugGaitOverride;

	public CharacterSettings Settings;

	public GameObject forceTarget;

	private int mRoutingCount;

	public string UnitName;

	public Material ModelOverWatchMaterial;

	protected Material ModelBaseMaterial;

	public SnapTarget SnapTarget;

	public HitLocation SimpleHitBounds;

	public Ragdoll Ragdoll;

	public HudBlipIcon HudMarker;

	public NavigationSetPieceLogic mNavigationSetPiece;

	public GameObject LaserSight;

	public int roundRobinIndex;

	public static int roundRobinCounter;

	protected FirstPersonCamera mStandardCamera;

	protected ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public MoveAimDescriptor moveAimDescription;

	protected int carryingOverride;

	protected int carriedOverride;

	public bool carryBooked;

	public bool healBooked;

	public bool IsUncarriable;

	public bool CanTriggerAlarms = true;

	public int shootCategoryHandle;

	public AnimDirector.ActionHandle shootActionHandle;

	public AnimDirector.ActionHandle reloadActionHandle;

	public int reactionCategoryHandle;

	public AnimDirector.ActionHandle flinch_LeftArmFrontHandle;

	public AnimDirector.ActionHandle flinch_LeftArmBackHandle;

	public AnimDirector.ActionHandle flinch_RightArmFrontHandle;

	public AnimDirector.ActionHandle flinch_RightArmBackHandle;

	public AnimDirector.ActionHandle flinch_LeftLegFrontHandle;

	public AnimDirector.ActionHandle flinch_LeftLegBackHandle;

	public AnimDirector.ActionHandle flinch_RightLegFrontHandle;

	public AnimDirector.ActionHandle flinch_RightLegBackHandle;

	public BlobShadow Shadow;

	public Nationality VocalAccent;

	public bool IsInASetPiece;

	protected Transform mAimBone;

	protected Vector3 mPOIDir;

	protected Vector3 lastPosition;

	protected Vector3 targetPosition;

	protected float mPOITimer;

	public float mIdleTimer;

	protected bool forcedCrouch;

	protected float mWander;

	protected float mWanderRate;

	protected Transform mRootBone;

	protected bool mNextFrameNavMesh;

	private static BaseCharacter sLastActorDropped;

	public bool mForceOffscreen;

	protected bool mStealth;

	protected int mMovingManually;

	private int mNumberOfFollowers;

	private int mNumberOfDefenders;

	protected Transform mReferenceFrame;

	protected bool mReferenceFrameNotNull;

	protected Transform mTransform;

	protected Vector2 mGProj = new Vector2(0f, 0f);

	private GMGBalanceTweaks mGMGBalanceTweaks;

	protected bool mIsDeadForReal;

	public bool refuseToSlowDown;

	public MovementStyle safeMovementStyle = MovementStyle.Run;

	private float tryRunningTime;

	protected bool mIsFirstPerson;

	protected bool mIsMoving;

	protected Stance mStance;

	protected MovementStyle mMovementStyle;

	protected BuildingWithInterior mLocation;

	protected bool mDocked;

	protected HidingPlace mDockedContainer;

	protected Actor mCarried;

	protected bool mHasBeenCarried;

	protected bool mIsBeingCarried;

	protected Actor mCarriedBy;

	protected EventOnMove mEventOnMove;

	protected LookType mImposedLookDirectionValid;

	protected LookType mDefaultLookDirectionValid;

	protected Vector3 mImposedLookDirection = Vector3.zero;

	protected Vector3 mDefaultLookDirection = Vector3.zero;

	protected Vector3 mImposedLookPosition = Vector3.zero;

	protected Vector3 mDefaultLookPosition = Vector3.zero;

	protected GameObject mImposedLookTarget;

	protected GameObject mDefaultLookTarget;

	protected Vector3 mCachedLookDirection = Vector3.zero;

	protected Vector3 mCachedLookPosition = Vector3.zero;

	protected GameObject mCachedLookTarget;

	public FirstPersonCamera FirstPersonCamera { get; private set; }

	public bool IsPerformingUninterruptableSetPiece
	{
		get
		{
			foreach (Task item in myActor.tasks.LongTerm.Stack)
			{
				TaskSetPiece taskSetPiece = item as TaskSetPiece;
				if (taskSetPiece != null && taskSetPiece.DoNotInterrupt)
				{
					return true;
				}
			}
			foreach (Task item2 in myActor.tasks.Immediate.Stack)
			{
				TaskSetPiece taskSetPiece2 = item2 as TaskSetPiece;
				if (taskSetPiece2 != null && taskSetPiece2.DoNotInterrupt)
				{
					return true;
				}
			}
			foreach (Task item3 in myActor.tasks.Reactive.Stack)
			{
				TaskSetPiece taskSetPiece3 = item3 as TaskSetPiece;
				if (taskSetPiece3 != null && taskSetPiece3.DoNotInterrupt)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool NextFrameNavMesh
	{
		get
		{
			return mNextFrameNavMesh;
		}
	}

	public bool ForceCrouch
	{
		set
		{
			forcedCrouch = value;
		}
	}

	public Vector3 TargetPosition
	{
		get
		{
			return targetPosition;
		}
		set
		{
			targetPosition = value;
		}
	}

	public int NumberOfFollowers
	{
		get
		{
			return mNumberOfFollowers;
		}
		set
		{
			mNumberOfFollowers = value;
		}
	}

	public int NumberOfDefenders
	{
		get
		{
			return mNumberOfDefenders;
		}
		set
		{
			mNumberOfDefenders = value;
		}
	}

	protected GMGBalanceTweaks GMGTweaks
	{
		get
		{
			if (mGMGBalanceTweaks == null)
			{
				mGMGBalanceTweaks = GMGBalanceTweaks.Instance;
			}
			return mGMGBalanceTweaks;
		}
	}

	public bool IsDeadForReal
	{
		get
		{
			return mIsDeadForReal;
		}
		set
		{
			mIsDeadForReal = value;
		}
	}

	public bool CanChangeWeapon
	{
		get
		{
			return !IsDead() && !IsMortallyWounded();
		}
	}

	public bool IsUsingFixedGun { get; set; }

	public bool Docked
	{
		get
		{
			return mDocked;
		}
		set
		{
			mDocked = value;
			if (mDocked)
			{
				EventOnHidden componentInChildren = myActor.GetComponentInChildren<EventOnHidden>();
				if (componentInChildren != null)
				{
					componentInChildren.OnHidden();
				}
			}
		}
	}

	public HidingPlace DockedContainer
	{
		get
		{
			return mDockedContainer;
		}
		set
		{
			mDockedContainer = value;
		}
	}

	public EventOnMove EventMoved
	{
		set
		{
			mEventOnMove = value;
		}
	}

	public Actor Carried
	{
		get
		{
			return mCarried;
		}
		set
		{
			if (mCarried != null)
			{
				mCarried.baseCharacter.CarriedBy = null;
			}
			mCarried = value;
			if (value != null)
			{
				mCarried.realCharacter.HasBeenCarried = true;
				mCarried.realCharacter.IsBeingCarried = true;
				mCarried.realCharacter.CarriedBy = myActor;
				myActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.Carrying);
				if (mCarried.realCharacter.Ragdoll != null)
				{
					mCarried.animDirector.enabled = true;
					mCarried.animDirector.AnimationPlayer.enabled = true;
					mCarried.realCharacter.Ragdoll.SwitchToKinematic();
					mCarried.animDirector.ForceUpdateFor(1f);
				}
				mCarried.Pose.SetActiveModule(PoseModuleSharedData.Modules.Carried);
				mCarried.realCharacter.EnableNavMesh(false);
				EventOnCarried componentInChildren = mCarried.GetComponentInChildren<EventOnCarried>();
				if (componentInChildren != null)
				{
					componentInChildren.OnCarried(myActor);
				}
			}
		}
	}

	public bool HasBeenCarried
	{
		get
		{
			return mHasBeenCarried;
		}
		set
		{
			mHasBeenCarried = value;
		}
	}

	public bool IsBeingCarried
	{
		get
		{
			return mIsBeingCarried;
		}
		set
		{
			mIsBeingCarried = value;
		}
	}

	public Actor CarriedBy
	{
		get
		{
			return mCarriedBy;
		}
		set
		{
			mCarriedBy = value;
		}
	}

	public bool WasLastActorDropped
	{
		get
		{
			return this == sLastActorDropped;
		}
	}

	public MovementStyle MovementStyleActive
	{
		get
		{
			if (mStance == Stance.Crouched)
			{
				return MovementStyle.Walk;
			}
			if (myActor.behaviour != null && myActor.behaviour.NeedToWalk())
			{
				return MovementStyle.Walk;
			}
			if (mMovementStyle == MovementStyle.AsFastAsSafelyPossible)
			{
				if (tryRunningTime < Time.time)
				{
					safeMovementStyle = MovementStyle.Run;
				}
				return safeMovementStyle;
			}
			return mMovementStyle;
		}
		set
		{
			mMovementStyle = value;
			if (mMovementStyle == MovementStyle.Run)
			{
				GlobalKnowledgeManager.Instance().crouchBecauseOf[myActor.quickIndex] = 0u;
				refuseToSlowDown = true;
			}
			else if (refuseToSlowDown)
			{
				refuseToSlowDown = false;
			}
			if (mEventOnMove != null)
			{
				mEventOnMove.OnMove(myActor);
			}
		}
	}

	public MovementStyle MovementStyleRequested
	{
		get
		{
			return mMovementStyle;
		}
	}

	public BuildingWithInterior Location
	{
		get
		{
			return mLocation;
		}
		set
		{
			mLocation = value;
		}
	}

	public virtual bool IsFirstPerson
	{
		get
		{
			return mIsFirstPerson;
		}
		set
		{
			mIsFirstPerson = value;
		}
	}

	public virtual bool IsAimingDownSights { get; set; }

	public virtual bool IsADSSuppressed
	{
		get
		{
			object obj;
			if (myActor != null && myActor.weapon != null)
			{
				IWeapon activeWeapon = myActor.weapon.ActiveWeapon;
				obj = activeWeapon;
			}
			else
			{
				obj = null;
			}
			IWeapon weapon = (IWeapon)obj;
			object obj2;
			if (weapon != null)
			{
				IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(weapon);
				obj2 = weaponEquip;
			}
			else
			{
				obj2 = null;
			}
			IWeaponEquip weaponEquip2 = (IWeaponEquip)obj2;
			bool flag = weaponEquip2 != null && (weaponEquip2.IsPuttingAway() || weaponEquip2.IsTakingOut());
			bool flag2 = weapon != null && weapon.IsReloading();
			return (IsFirstPerson && (GameController.Instance.ADSSuppressed() || ViewModelRig.Instance().IsOverrideActive)) || flag || flag2;
		}
	}

	public void SetCameraOverride(FirstPersonCamera camera)
	{
		FirstPersonCamera = camera;
	}

	public void ClearCameraOverride()
	{
		FirstPersonCamera = mStandardCamera;
	}

	public virtual void SetReferenceFrame(Transform frame)
	{
		mReferenceFrame = frame;
		mReferenceFrameNotNull = frame != null;
	}

	public virtual Quaternion GetReferenceQuaternion()
	{
		return (!mReferenceFrameNotNull) ? Quaternion.identity : mReferenceFrame.rotation;
	}

	public virtual Vector3 GetReferenceAngles()
	{
		return (!mReferenceFrameNotNull) ? Vector3.zero : mReferenceFrame.eulerAngles;
	}

	public virtual Vector3 GetPointInReferenceFrame(Vector3 position)
	{
		return WorldSpaceToFrame(position, mReferenceFrame);
	}

	protected static Vector3 FrameToWorldSpace(Vector3 localPosition, Transform frame)
	{
		return (!(frame != null)) ? localPosition : frame.TransformPoint(localPosition);
	}

	protected static Vector3 WorldSpaceToFrame(Vector3 worldPosition, Transform frame)
	{
		return (!(frame != null)) ? worldPosition : frame.InverseTransformPoint(worldPosition);
	}

	[Conditional("LOG_CHARACTER")]
	public static void Log(string message)
	{
		UnityEngine.Debug.Log(message);
	}

	private void Awake()
	{
	}

	private void OnDestroy()
	{
	}

	public virtual Vector3 GetCameraTargetPosition()
	{
		Vector3 position = mTransform.position;
		if (mRootBone == null)
		{
			if (myActor.model != null)
			{
				mRootBone = myActor.model.transform.FindInHierarchy("Bip002");
			}
		}
		else if (myActor.OnScreen && !mForceOffscreen)
		{
			position = mRootBone.position;
		}
		return position;
	}

	public virtual void EnableNavMesh(bool OnOff)
	{
		mNextFrameNavMesh = OnOff;
		if (!OnOff)
		{
			myActor.navAgent.enabled = OnOff;
		}
	}

	public virtual bool IsUsingNavMesh()
	{
		return myActor.navAgent != null && myActor.navAgent.enabled;
	}

	public void StartMovingManually()
	{
		if (mMovingManually == 0)
		{
			EnableNavMesh(false);
		}
		mMovingManually++;
		if (myActor.awareness.ChDefCharacterType == CharacterType.RiotShieldNPC)
		{
			myActor.Pose.Command("Advance");
		}
	}

	public void StopMovingManually()
	{
		TBFAssert.DoAssert(mMovingManually != 0);
		mMovingManually--;
		if (mMovingManually == 0)
		{
			EnableNavMesh(true);
		}
		if (myActor.awareness.ChDefCharacterType == CharacterType.RiotShieldNPC)
		{
			myActor.Pose.Command("Stand");
		}
	}

	public virtual bool IsBeingMovedManually()
	{
		return mMovingManually > 0;
	}

	public virtual void SetSelectable(bool OnOff, bool HUDOnOff)
	{
	}

	public virtual SetPieceLogic CreateSetPieceLogic()
	{
		return mNavigationSetPiece.CreateSetPiece();
	}

	public virtual void CreateStandardCamera(Transform headBone, GameObject owner, GameObject model)
	{
		if (mStandardCamera == null)
		{
			FirstPersonCamera firstPersonCamera = SceneNanny.NewGameObject("First Person Camera").AddComponent<FirstPersonCamera>();
			firstPersonCamera.HeadBone = headBone;
			firstPersonCamera.Fov = InputSettings.FirstPersonFieldOfView;
			firstPersonCamera.owner = owner;
			firstPersonCamera.model = model;
			firstPersonCamera.HeadHeight = InputSettings.FirstPersonHeightHeightStanding;
			firstPersonCamera.UpdatePosition();
			mStandardCamera = firstPersonCamera;
			FirstPersonCamera = mStandardCamera;
		}
	}

	private void Start()
	{
	}

	protected void SetUpTransForm()
	{
		mTransform = base.transform;
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
	}

	protected virtual void UpdateWeapons()
	{
	}

	public virtual void ResetState()
	{
	}

	public virtual Vector3 GetCameraTransitionTarget()
	{
		return myActor.GetPosition();
	}

	public virtual Vector3 GetSoftLockPosition()
	{
		return myActor.GetPosition();
	}

	public virtual Vector3 GetSnapPosition()
	{
		return myActor.GetPosition();
	}

	public virtual Vector3 GetBulletOrigin()
	{
		return myActor.GetPosition();
	}

	public virtual Vector3 GetHeadshotSpot()
	{
		return myActor.GetPosition();
	}

	public virtual Vector3 GetStandingBulletOrigin()
	{
		return myActor.GetPosition();
	}

	public virtual Vector3 GetStanceDirection()
	{
		return myActor.model.transform.forward;
	}

	public virtual Vector2 GetPositionGproj()
	{
		return mGProj;
	}

	protected void CachePositionGproj()
	{
		mGProj.x = mTransform.position.x;
		mGProj.y = mTransform.position.z;
	}

	public virtual void TurnToFaceDirection(Vector3 direction)
	{
		if (myActor.awareness.ChDefCharacterType == CharacterType.RPG)
		{
			Vector3 forward = new Vector3(direction.x, mTransform.forward.y, direction.z);
			mTransform.forward = forward;
		}
		myActor.awareness.LookDirection = direction;
	}

	public virtual void TurnToFacePosition(Vector3 position)
	{
		Vector3 lookDirection = position - GetBulletOrigin();
		myActor.awareness.LookDirection = lookDirection;
	}

	public virtual bool IsMoving()
	{
		return mIsMoving;
	}

	public bool IsRunning()
	{
		if (IsFirstPerson)
		{
			return GameController.Instance.LastVelocity.sqrMagnitude > 4.1f;
		}
		return IsMoving() && MovementStyleActive == MovementStyle.Run;
	}

	public virtual void SetIsMoving(bool active, Vector3 targetPoint)
	{
		mIsMoving = active;
	}

	public virtual void HoldPosition()
	{
		myActor.tasks.CancelTasks(typeof(TaskRouteTo));
		myActor.tasks.CancelTasks(typeof(TaskMoveTo));
		myActor.tasks.CancelTasks(typeof(TaskShoot));
		myActor.tasks.CancelTasks(typeof(TaskDefend));
		myActor.tasks.CancelTasks(typeof(TaskFollow));
		myActor.tasks.CancelTasks(typeof(TaskFollowMovingTarget));
		myActor.behaviour.Suppress(null, false);
	}

	public virtual void Crouch()
	{
		mStance = Stance.Crouched;
	}

	public bool LowProfile()
	{
		return IsCrouching() || IsDead();
	}

	public bool IsCrouching()
	{
		return mStance == Stance.Crouched;
	}

	public virtual void Stand()
	{
		mStance = Stance.Standing;
	}

	public virtual Stance GetStance()
	{
		return mStance;
	}

	public virtual void ShootAtTarget(Actor target)
	{
	}

	public virtual bool IsDead()
	{
		return mIsDeadForReal;
	}

	public virtual bool IsMortallyWounded()
	{
		return false;
	}

	public virtual bool CanSelect()
	{
		if (myActor.behaviour == null)
		{
			return false;
		}
		return myActor.behaviour.PlayerControlled && !IsDead() && !IsMortallyWounded();
	}

	public virtual bool CanGoFirstPerson()
	{
		if (myActor.tasks.IsRunningTask<TaskEnter>())
		{
			return false;
		}
		if (IsInASetPiece)
		{
			if (myActor.tasks.IsRunningTask<TaskHack>())
			{
				return false;
			}
			if (myActor.tasks.IsRunningTask<TaskHeal>())
			{
				return false;
			}
			if (myActor.tasks.IsRunningTask<TaskCarry>())
			{
				return false;
			}
			return myActor.behaviour != null && myActor.behaviour.PlayerControlled && !CinematicHelper.IsInCinematic;
		}
		return myActor.behaviour != null && myActor.behaviour.PlayerControlled && !IsDead() && !IsMortallyWounded() && !IsKnockedDown() && !CinematicHelper.IsInCinematic && IsSelectable();
	}

	public virtual bool CanMove()
	{
		return !IsDead() && !IsMortallyWounded() && !IsKnockedDown() && !IsUsingFixedGun && !IsInASetPiece && !IsMoving() && myActor.navAgent.enabled && !TutorialToggles.LockFPPMovement;
	}

	public void StartRouting()
	{
		mRoutingCount++;
	}

	public void EndRouting()
	{
		mRoutingCount--;
		if (mRoutingCount == 0)
		{
			refuseToSlowDown = false;
		}
	}

	public bool IsRouting()
	{
		return mRoutingCount > 0;
	}

	public bool IsKnockedDown()
	{
		return myActor.tasks.IsRunningTask(typeof(TaskKnockedDown));
	}

	public virtual bool IsSelectable()
	{
		return false;
	}

	public virtual bool CanBeCarried()
	{
		return !carryBooked && !IsUncarriable && myActor.poseModuleSharedData.blend == 0f && (IsMortallyWounded() || IsDead());
	}

	public virtual bool CanBeHealed()
	{
		return !carryBooked && !healBooked && IsMortallyWounded() && !IsDead();
	}

	public virtual void Drop()
	{
		if (mCarried == null)
		{
			return;
		}
		mCarried.Command("CancelCarried");
		myActor.Command("CancelCarrying");
		if (mCarried != null)
		{
			SelectableObject component = mCarried.GetComponent<SelectableObject>();
			if (component != null)
			{
				component.enabled = true;
			}
			mCarried.realCharacter.IsBeingCarried = false;
			EventOnIveBeenDropped componentInChildren = mCarried.GetComponentInChildren<EventOnIveBeenDropped>();
			if (componentInChildren != null)
			{
				componentInChildren.OnDropped();
			}
		}
		sLastActorDropped = mCarried.baseCharacter;
		Carried = null;
	}

	public virtual void Kill(string damageType)
	{
	}

	public bool SlowDown()
	{
		if (mMovementStyle == MovementStyle.Walk)
		{
			return true;
		}
		if (mMovementStyle == MovementStyle.AsFastAsSafelyPossible)
		{
			safeMovementStyle = MovementStyle.Walk;
			tryRunningTime = Time.time + 1f;
			return true;
		}
		return false;
	}

	public bool ImposedLookDirectionValid(ref Vector3 dir)
	{
		switch (mImposedLookDirectionValid)
		{
		case LookType.None:
			return false;
		case LookType.Direction:
			dir = mImposedLookDirection;
			break;
		case LookType.Position:
			dir = mImposedLookPosition - myActor.GetPosition();
			break;
		case LookType.Target:
			if (mImposedLookTarget == null)
			{
				mImposedLookDirectionValid = LookType.None;
				return false;
			}
			dir = mImposedLookTarget.transform.position - myActor.GetPosition();
			break;
		}
		mImposedLookDirectionValid = LookType.None;
		mDefaultLookDirectionValid = LookType.None;
		return true;
	}

	public virtual void ImposeLookDirection(Vector3 dir)
	{
		float num = 0.01f;
		if (dir.sqrMagnitude > num)
		{
			mImposedLookDirectionValid = LookType.Direction;
			mImposedLookDirection = dir;
		}
		else
		{
			mImposedLookDirectionValid = LookType.None;
			mImposedLookDirection = Vector3.zero;
		}
	}

	public virtual void ImposeLookPosition(Vector3 pos)
	{
		mImposedLookPosition = pos;
		mImposedLookDirectionValid = LookType.Position;
	}

	public virtual void ImposeLookTarget(GameObject go)
	{
		mImposedLookTarget = go;
		mImposedLookDirectionValid = LookType.Target;
	}

	public bool DefaultLookDirectionValid(ref Vector3 dir)
	{
		switch (mDefaultLookDirectionValid)
		{
		case LookType.None:
			return false;
		case LookType.Direction:
			dir = mDefaultLookDirection;
			break;
		case LookType.Position:
			dir = mDefaultLookPosition - myActor.GetPosition();
			break;
		case LookType.Target:
			if (mDefaultLookTarget == null)
			{
				mDefaultLookDirectionValid = LookType.None;
				return false;
			}
			dir = mDefaultLookTarget.transform.position - myActor.GetPosition();
			break;
		}
		mDefaultLookDirectionValid = LookType.None;
		return true;
	}

	public virtual void SetDefaultLookDirection(Vector3 dir)
	{
		float num = 0.01f;
		if (dir.sqrMagnitude > num)
		{
			mDefaultLookDirectionValid = LookType.Direction;
			mDefaultLookDirection = dir;
		}
		else
		{
			mDefaultLookDirectionValid = LookType.None;
			mDefaultLookDirection = Vector3.zero;
		}
	}

	public virtual void SetDefaultLookPosition(Vector3 pos)
	{
		mDefaultLookPosition = pos;
		mDefaultLookDirectionValid = LookType.Position;
	}

	public virtual void SetDefaultLookTarget(GameObject go)
	{
		mDefaultLookTarget = go;
		mDefaultLookDirectionValid = LookType.Target;
	}

	protected virtual void SetupAnimationHandles()
	{
		if (myActor.animDirector == null)
		{
			RulesSystem.Log("Cannot set up Animation Handles - The Character doesn't have an anim director setup!");
		}
		else if (moveAimDescription != null)
		{
			if (CharacterTypeHelper.RequiresShootReloadActionHandles(moveAimDescription.CharacterType))
			{
				shootCategoryHandle = myActor.animDirector.GetCategoryHandle("Shooting");
				shootActionHandle = myActor.animDirector.GetActionHandle(shootCategoryHandle, "Fire");
				reloadActionHandle = myActor.animDirector.GetActionHandle(shootCategoryHandle, "Reload");
			}
			if (CharacterTypeHelper.CanFlinch(moveAimDescription.CharacterType))
			{
				reactionCategoryHandle = myActor.animDirector.GetCategoryHandle("Reactions");
				flinch_LeftArmFrontHandle = myActor.animDirector.GetActionHandle(reactionCategoryHandle, "FPS_L_ArmShotFront");
				flinch_LeftArmBackHandle = myActor.animDirector.GetActionHandle(reactionCategoryHandle, "FPS_L_ArmShotBack");
				flinch_RightArmFrontHandle = myActor.animDirector.GetActionHandle(reactionCategoryHandle, "FPS_R_ArmShotFront");
				flinch_RightArmBackHandle = myActor.animDirector.GetActionHandle(reactionCategoryHandle, "FPS_R_ArmShotBack");
				flinch_LeftLegFrontHandle = myActor.animDirector.GetActionHandle(reactionCategoryHandle, "FPS_L_LegShotFront");
				flinch_LeftLegBackHandle = myActor.animDirector.GetActionHandle(reactionCategoryHandle, "FPS_L_LegShotBack");
				flinch_RightLegFrontHandle = myActor.animDirector.GetActionHandle(reactionCategoryHandle, "FPS_R_LegShotFront");
				flinch_RightLegBackHandle = myActor.animDirector.GetActionHandle(reactionCategoryHandle, "FPS_R_LegShotBack");
			}
		}
	}

	public virtual void Command(string com)
	{
		switch (ParseCommand(com))
		{
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
		}
	}

	public float GetGaitSpeed(MovementStyle gait)
	{
		if (m_sDebugGaitOverride != 0f)
		{
			return m_sDebugGaitOverride;
		}
		float num = 0f;
		bool flag = myActor.behaviour.alertState < BehaviourController.AlertState.Reacting;
		bool flag2 = mStealth;
		if (myActor.awareness.ChDefCharacterType == CharacterType.RiotShieldNPC)
		{
			gait = MovementStyle.Walk;
			flag = (flag2 = false);
		}
		switch (gait)
		{
		case MovementStyle.Run:
		case MovementStyle.AsFastAsSafelyPossible:
			num = ((mMovementStyle != MovementStyle.Run) ? Settings.RunSpeed : ((!(myActor.behaviour != null) || !myActor.behaviour.PlayerControlled) ? (Settings.RunSpeed * 1.1f) : (Settings.RunSpeed * 1.25f)));
			break;
		case MovementStyle.Walk:
			num = ((!flag) ? ((!flag2) ? Settings.WalkSpeed : Settings.StealthSpeed) : Settings.SaunterSpeed);
			break;
		}
		if (myActor.weapon != null && (!myActor.behaviour.PlayerControlled || !GameSettings.Instance.HasPerk(PerkType.LightWeight)))
		{
			num *= myActor.weapon.RunSpeed * 0.01f;
		}
		OverwatchController instance = OverwatchController.Instance;
		if (instance != null && instance.Active)
		{
			num *= instance.CharacterMovementSpeedMultiplier;
		}
		if (!myActor.behaviour.PlayerControlled)
		{
			num = GMGTweaks.GMGModifier_EnemyMovement(num);
		}
		return num;
	}

	protected CommandEnum ParseCommand(string s)
	{
		int value;
		if (s != null && mTranslationTable != null && mTranslationTable.TryGetValue(s, out value))
		{
			return (CommandEnum)value;
		}
		return CommandEnum.kUndefined;
	}

	public bool IsLaserSiteOn()
	{
		if (LaserSight != null)
		{
			return LaserSight.activeSelf;
		}
		return false;
	}

	public void UseLaserSite(bool on)
	{
		if (LaserSight != null)
		{
			LaserSight.SetActive(on);
		}
	}

	public virtual void SetOverWatchModel()
	{
		if (!(ModelOverWatchMaterial == null) && !(myActor.model == null))
		{
			Component[] componentsInChildren = myActor.model.GetComponentsInChildren(typeof(SkinnedMeshRenderer), true);
			Component[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[i];
				ModelBaseMaterial = skinnedMeshRenderer.material;
				skinnedMeshRenderer.material = ModelOverWatchMaterial;
			}
		}
	}

	public virtual void SetBaseModel()
	{
		if (ModelOverWatchMaterial == null || myActor.model == null || ModelBaseMaterial == null)
		{
			return;
		}
		Component[] componentsInChildren = myActor.model.GetComponentsInChildren(typeof(SkinnedMeshRenderer), true);
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[i];
			if (skinnedMeshRenderer != null)
			{
				skinnedMeshRenderer.material = ModelBaseMaterial;
			}
		}
	}

	public virtual void ApplyRecoil(float amount)
	{
	}

	public virtual void PlayReload()
	{
	}

	public virtual void Flinch(Vector3 projectileOrigin)
	{
	}
}
