using System;
using System.Collections.Generic;
using UnityEngine;

public class CommonHudController : MonoBehaviour
{
	private enum UnityAbilityTextState
	{
		Appearing = 0,
		Normal = 1,
		Warning = 2
	}

	private struct TapZone
	{
		public Vector2 position;

		public float time;
	}

	private enum GestureType
	{
		None = 0,
		ZoomIn = 1,
		ZoomOut = 2,
		Stand = 3,
		Crouch = 4,
		SwitchLeft = 5,
		SwitchRight = 6,
		ThrowGrenade = 7
	}

	public delegate void OnZoomInTriggeredEventHandler(object sender, EventArgs args);

	public delegate void OnZoomOutTriggeredEventHandler(object sender, EventArgs args);

	public delegate void FirstPersonClickEventHandler(object sender, EventArgs args);

	private const float doubleTapMovementThreshold = 32f;

	private const float kMeleeTypeAttackMaxViewDifference = 22.5f;

	private const float mButtonHoldThreshold = 0.5f;

	private const int MAX_TOUCHES = 32;

	private int lastScreenWidth;

	private int lastScreenHeight;

	public GameObject ContextMenuPrefab;

	public GameObject GrenadeThrowMarkerPrefab;

	private GrenadeThrowMarker mActiveGrenadeThrowMarker;

	public GameObject FingerHoldDragMarkerPrefab;

	private GameObject mActiveFingerHoldDragMarker;

	public SimpleLog MessageLog;

	public MissionTimer MissionTimer;

	public XpBlipFeedback XpFeedback;

	public UIButton ZoomInButton;

	public PackedSprite ZoomInButtonSuggest;

	public PackedSprite ZoomButtonIcon;

	public UIButton FollowMeButton;

	public UIButton ADSButton;

	public UIButton CrouchButton;

	public PackedSprite CrouchButtonIcon;

	public UIButton ContextInteractionButton;

	public PackedSprite ContextInteractionIcon;

	public UIButton DropBodyButton;

	public UIButton DropGrenadeButton;

	public UIButton DropClaymoreButton;

	public GameObject ScreenEdging;

	public SpriteText ScoreOutput;

	public SpriteText WaveOutput;

	public SpriteText EnemiesRemainingOutput;

	public SpriteText TokenOutput;

	public GMGResultsPane GMGResults;

	public GMGScorePanes GMGScore;

	public TeleTypeLabel MissionNameLabel;

	public DialogueQueue MissionDialogueQueue;

	public float MinTimeBetweenXPMessages = 2f;

	public UIButton TriggerButton;

	public PackedSprite TriggerButtonIcon;

	public UIButton Look;

	public UIButton Move;

	public SpriteText AmmoReadout;

	public UIProgressBar AmmoProgressBar;

	public SpriteText GrenadeAmmoReadout;

	public SpriteText UnitAbility;

	public SpriteText UnitName;

	public UIButton ChangeUnitLeftButton;

	public UIButton ChangeUnitRightButton;

	public DynamicCrosshair Crosshair;

	public DamageDirectionIndicator DamageIndicator;

	public GrenadeWarnIndicator GrenadeIndicator;

	public HudHeartBeatHealth HeartBeat;

	public UIWeaponFlickButton WeaponIcon;

	public UIButton HoldBreathButton;

	public UIProgressBar HoldBreathTimer;

	public PackedSprite SnapToTargetLeft;

	public PackedSprite SnapToTargetRight;

	public UIButton FirstPersonGrenadeButton;

	public VirtualStickFeedback MoveStick;

	public UIButton PauseButton;

	public UIButton MoreInfoButton;

	private MoreInfoTrigger mMoreInfoTrigger;

	private bool mSnapToTargetLeftRed;

	private bool mSnapToTargetRightRed;

	private Vector3 mSnapToTargetLeftScale;

	private Vector3 mSnapToTargetRightScale;

	public UIProgressBar HeatBarLeft;

	public UIProgressBar HeatBarRight;

	public PackedSprite HeatWarningLeft;

	public PackedSprite HeatWarningRight;

	public GameObject ChainGun;

	public GameObject WeaponSelect;

	public GameObject GreneadeSelect;

	public SpriteText FriendlyUnitName;

	public UnitSelecter TPPUnitSelecter;

	public Shader TransitionEffectShader;

	public bool IsContextActionInProgress;

	public Actor ActorToStealthKill;

	public List<GameObject> HUDInvulnerabilityFlashList;

	private List<PackedSprite> mHUDInvulnerabilityFlashSprites;

	private static CommonHudController smInstance;

	public static bool DebugDistancesDisplay;

	private GameObject mContextMenuObj;

	private InterfaceableObject mInterfaceableObj;

	private float mFirstPersonTriggerTimer = -1f;

	private Queue<XPMessageData> m_XPMessages = new Queue<XPMessageData>();

	private float m_TimeOfLastXPMessage;

	private bool mContextMenuHidden;

	private int mDamageIndicatorIndex;

	private DamageDirectionIndicator[] mDamageIndicators;

	private GrenadeWarnIndicator mGrenadeWarnIndicator;

	private bool mTriggerPressedBuffer;

	private bool mCrouchPressedBuffer;

	private bool mFollowMeBuffer;

	private bool mTapTriggerActive;

	private float mMovementSinceFirstTap;

	private bool mSnapLeftBuffered;

	private bool mSnapRightBuffered;

	private int mSnapLetTouchBeganWithFinger = -1;

	private int mSnapRightTouchBeganWithFinger = -1;

	private float mUnitAbilityDuration;

	private float mUnitAbilityAlertTime;

	private float mUnitAbilityTimer;

	private UnityAbilityTextState mUnitAbilityTextState;

	private int mNumBodiesCached = -1;

	private int mNumGrenadesCached = -1;

	private int mNumClaymoresCached = -1;

	private int mCrouchingnAnimFrame = -1;

	private bool mAnyTriggerPressed;

	private Vector3[] mMoveStickNotchesTouch;

	private Vector3[] mMoveStickNotchesPad;

	private Vector3[] mLookStickNotchesPad;

	private Vector2 mMoveAmountInternalTouch;

	private Vector2 mMoveAmountInternalPad;

	private Vector2 mMoveAmountExternal;

	private bool mLookAmountStall;

	private Vector2[] mLookAmountFrames = new Vector2[16];

	private Vector2 mLookAmountInternalTouch;

	private Vector2 mLookAmountExternalTouch;

	private Vector2 mLookAmountInternalPad;

	private Vector2 mLookAmountExternalPad;

	private bool[] mValidLookIds = new bool[32];

	private TapZone[] mTapZones = new TapZone[16];

	private int mActiveTapZones;

	private int mLatestTapZone;

	private int mNavZoneLayerMask;

	private NavigationZone mCurrNavZone;

	private bool mChainGunUIEnabled;

	private float mFlashZoomInButtonTimer = -1f;

	private float mFlashZoomInButtonDuration = 1f;

	private Color mFlashZoomInFlashColour;

	public GameObject TutorialHighlighter;

	public GameObject TutorialHighlighterSmall;

	public GameObject TutorialHighlighterArrows;

	public GameObject TutorialHighlighterPinchIn;

	public GameObject TutorialHighlighterPinchOut;

	public GameObject TutorialHighlighterRotate;

	public GameObject TutorialHighlighterSwipe;

	private float mContextIconTimout;

	private InterfaceableObject mContextInteractionObject;

	private FPPThrowWeaponIcon mFPPThrowWeaponIcon;

	private float mWeaponSwapButtonDown = float.MaxValue;

	private float mClaymoreDropButtonDown = float.MaxValue;

	private Vector2[] mTouchStartPositions = new Vector2[32];

	private GestureType mBufferedGestureType;

	private bool mCanStartGesture;

	private bool mWaitingForLeftTouch;

	private bool mWaitingForRightTouch;

	private bool mGestureStarted;

	private float mGestureStartedTime;

	private float mGestureEndedTime;

	private int mLeftTouchIndex;

	private int mRightTouchIndex;

	private Vector2 mLeftTouchStartPosition;

	private Vector2 mRightTouchStartPosition;

	private SampleWindow mLeftSamples = new SampleWindow(4);

	private SampleWindow mRightSamples = new SampleWindow(4);

	private int mSnapToTargetFingerMask;

	public bool mMovingTrigger;

	private bool mHUDInvulnerabilityEffectActive;

	private int lockoutBecauseTheseButtonsGoHugeIfTheyAreFlashedOnAndOff;

	public static CommonHudController Instance
	{
		get
		{
			return smInstance;
		}
	}

	public bool AnyTriggerInput
	{
		get
		{
			return mAnyTriggerPressed;
		}
	}

	private int FollowMeOrderFrame { get; set; }

	public Vector3[] MoveStickNotchesTouch
	{
		get
		{
			return mMoveStickNotchesTouch;
		}
		set
		{
			if (value != null && value.Length > 0)
			{
				mMoveStickNotchesTouch = value;
				return;
			}
			mMoveStickNotchesTouch = new Vector3[1]
			{
				new Vector3(1f, 0f, 1f)
			};
		}
	}

	public Vector3[] MoveStickNotchesPad
	{
		get
		{
			return mMoveStickNotchesPad;
		}
		set
		{
			if (value != null && value.Length > 0)
			{
				mMoveStickNotchesPad = value;
				return;
			}
			mMoveStickNotchesPad = new Vector3[1]
			{
				new Vector3(1f, 0f, 1f)
			};
		}
	}

	public Vector3[] LookStickNotchesPad
	{
		get
		{
			return mLookStickNotchesPad;
		}
		set
		{
			if (value != null && value.Length > 0)
			{
				mLookStickNotchesPad = value;
				return;
			}
			mLookStickNotchesPad = new Vector3[1]
			{
				new Vector3(1f, 0f, 1f)
			};
		}
	}

	public bool PreventWeaponSwap { get; set; }

	public bool TPPDropGrenadeLocked { get; set; }

	public bool TPPDropBodyLocked { get; set; }

	public bool TPPDropClaymoreLocked { get; set; }

	public bool TPPUnitSelectorLocked { get; set; }

	public bool FPPFollowMeLocked { get; set; }

	public bool FPPCrouchLocked { get; set; }

	public bool FPPADSLocked { get; set; }

	public bool TriggerLocked { get; set; }

	public bool FPPStealthKillLocked { get; set; }

	public bool ContextInteractionLocked { get; set; }

	public bool ExitMinigunButtonLocked { get; set; }

	public bool HoldBreathLocked { get; set; }

	public bool FPPUntChangeCached { get; set; }

	public FPPThrowWeaponIcon CurrentFPPThrowingWeapon
	{
		get
		{
			return mFPPThrowWeaponIcon;
		}
	}

	public bool TriggerPressed { get; private set; }

	public bool DropPressed { get; private set; }

	public bool MeleePressed { get; private set; }

	public bool StealthKillPressed { get; set; }

	public bool HoldingView { get; private set; }

	public Vector2 LookAmountTouch
	{
		get
		{
			return (!base.gameObject.activeInHierarchy) ? Vector2.zero : mLookAmountExternalTouch;
		}
	}

	public Vector2 LookAmountGamepad
	{
		get
		{
			return (!base.gameObject.activeInHierarchy) ? Vector2.zero : mLookAmountExternalPad;
		}
	}

	public Vector2 MoveAmount
	{
		get
		{
			return (!base.gameObject.activeInHierarchy) ? Vector2.zero : mMoveAmountExternal;
		}
	}

	public Vector3 DamageIndicatorPosition { get; private set; }

	public Vector3 GrenadeIndicatorPosition { get; private set; }

	public bool HasSnappedToTarget { get; private set; }

	public bool HasSnappedToTargetLeft { get; private set; }

	public bool HasSnappedToTargetRight { get; private set; }

	public float TouchSensitivityModifier
	{
		get
		{
			if (mGestureStarted && Input.touchCount == 2)
			{
				return 0f;
			}
			if (mWaitingForLeftTouch || mWaitingForRightTouch)
			{
				return 1f;
			}
			return Mathf.Clamp01(4f * (Time.realtimeSinceStartup - (mGestureStartedTime + 0.25f)));
		}
	}

	public bool HUDInvulnerabilityEffect
	{
		private get
		{
			return mHUDInvulnerabilityEffectActive;
		}
		set
		{
			mHUDInvulnerabilityEffectActive = value;
			if (mHUDInvulnerabilityFlashSprites == null || mHUDInvulnerabilityEffectActive)
			{
				return;
			}
			foreach (PackedSprite mHUDInvulnerabilityFlashSprite in mHUDInvulnerabilityFlashSprites)
			{
				mHUDInvulnerabilityFlashSprite.SetColor(Color.white);
			}
		}
	}

	public bool ContextMenuActive
	{
		get
		{
			return mContextMenuObj != null;
		}
	}

	public ContextMenuBase ContextMenu
	{
		get
		{
			if (ContextMenuActive)
			{
				return mContextMenuObj.GetComponent<ContextMenuBase>();
			}
			return null;
		}
	}

	public GrenadeThrowMarker ActiveGrenadeThrowMarker
	{
		get
		{
			return mActiveGrenadeThrowMarker;
		}
	}

	public event OnZoomInTriggeredEventHandler OnZoomInTriggered;

	public event OnZoomOutTriggeredEventHandler OnZoomOutTriggered;

	public event FirstPersonClickEventHandler OnSnapEnemyLeftClick;

	public event FirstPersonClickEventHandler OnSnapEnemyRightClick;

	public event FirstPersonClickEventHandler OnSnapUnitLeftClick;

	public event FirstPersonClickEventHandler OnSnapUnitRightClick;

	public event FirstPersonClickEventHandler OnInClick;

	public event FirstPersonClickEventHandler OnOutClick;

	public event FirstPersonClickEventHandler OnDuckClick;

	public event FirstPersonClickEventHandler OnReloadClick;

	public event FirstPersonClickEventHandler OnGrenadeIndicatorClick;

	public event FirstPersonClickEventHandler OnThrowGrenadeClick;

	public event FirstPersonClickEventHandler OnSwitchWeapon;

	public bool FollowMeRecentlyPressed()
	{
		int num = Time.frameCount - FollowMeOrderFrame;
		return num >= 0 && num <= 3;
	}

	private void NewTapZone(Vector2 position)
	{
		int num = mTapZones.Length;
		mLatestTapZone = (mLatestTapZone + 1) % num;
		mActiveTapZones = Mathf.Min(mActiveTapZones + 1, num);
		mTapZones[mLatestTapZone].time = Time.realtimeSinceStartup;
		mTapZones[mLatestTapZone].position = position;
	}

	private void RemoveExpiredTapZones()
	{
		float firstPersonDoubleTapThreshold = InputSettings.FirstPersonDoubleTapThreshold;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		int b = mTapZones.Length;
		for (int i = 0; i < mActiveTapZones; i++)
		{
			int num = Maths.Modulus(mLatestTapZone - i, b);
			if (realtimeSinceStartup - mTapZones[num].time > firstPersonDoubleTapThreshold)
			{
				mActiveTapZones = i;
			}
		}
	}

	private void CheckTapZone(Vector2 position)
	{
		int b = mTapZones.Length;
		for (int i = 0; i < mActiveTapZones; i++)
		{
			int num = Maths.Modulus(mLatestTapZone - i, b);
			float num2 = Vector2.Distance(position, mTapZones[num].position);
			float dpi = Screen.dpi;
			dpi = ((dpi != 0f) ? dpi : 72f);
			float num3 = num2 / dpi;
			if (num3 < 0.5f)
			{
				ToggleADS();
				mActiveTapZones = 0;
				return;
			}
		}
		if (!InLeftSnapZone(position) && !InRightSnapZone(position))
		{
			NewTapZone(position);
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple CommonHudController");
		}
		smInstance = this;
		if (GameController.Instance != null)
		{
			GameController.Instance.LinkCommonHudController(this);
			HideFollowMe(true);
			SetMoreInfoButton(null);
		}
		mContextMenuHidden = false;
		AmmoProgressBar.Value = 1f;
		TriggerButton.AddInputDelegate(OnTriggerInput);
		Look.AddInputDelegate(OnLookInput);
		Move.AddInputDelegate(OnMoveInput);
		int num = 4;
		DamageIndicator.Indicator.Color = DamageIndicator.Indicator.Color.Alpha(0f);
		mDamageIndicators = new DamageDirectionIndicator[num];
		for (int i = 0; i < mDamageIndicators.Length; i++)
		{
			DamageDirectionIndicator component = (UnityEngine.Object.Instantiate(DamageIndicator.gameObject) as GameObject).GetComponent<DamageDirectionIndicator>();
			component.transform.parent = DamageIndicator.transform.parent;
			mDamageIndicators[i] = component;
		}
		UnityEngine.Object.Destroy(DamageIndicator.gameObject);
		DamageIndicator = null;
		GrenadeIndicator.Indicator.Color = GrenadeIndicator.Indicator.Color.Alpha(0f);
		ShowWave(false);
		ShowTokens(false);
		ShowGMGResults(false);
		ShowScore(false);
		ShowEnemiesRemaining(false);
		ShowGMGScore(false);
		SetFPPThrowWeaponButton(FPPThrowWeaponIcon.Grenade);
		mNavZoneLayerMask = 1 << LayerMask.NameToLayer("NavZones");
		DropPressed = false;
		StealthKillPressed = false;
		if (DropBodyButton != null)
		{
			DropBodyButton.gameObject.SetActive(false);
		}
		mMoveStickNotchesTouch = new Vector3[3]
		{
			new Vector3(0.05f, 0f, 0.4f),
			new Vector3(0.09f, 0.4f, 0.95f),
			new Vector3(0.15f, 0.95f, 1.2f)
		};
		mMoveStickNotchesPad = new Vector3[3]
		{
			new Vector3(0.4f, 0f, 0.4f),
			new Vector3(0.8f, 0.4f, 0.95f),
			new Vector3(1f, 0.95f, 1.2f)
		};
		mLookStickNotchesPad = new Vector3[2]
		{
			new Vector3(0.19f, 0f, 0f),
			new Vector3(1f, 0f, 1f)
		};
		UnitAbility.Text = string.Empty;
		Moga_Input moga_Input = UnityEngine.Object.FindObjectOfType<Moga_Input>();
		if (moga_Input == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<Moga_Input>();
		}
	}

	public void Start()
	{
		ResizeLookAndMoveRegions();
		CacheHUDInvulnerabilityFlashSprites();
		if (!GameSettings.Instance.HasPerk(PerkType.IronLungs))
		{
			HoldBreathLocked = true;
			HoldBreathButton.gameObject.SetActive(false);
		}
		if (GameController.Instance != null)
		{
			SetZoomButtonFrame(GameController.Instance.IsFirstPerson);
		}
		MoveRightSnapToTargetForKindleFire();
		FollowMeOrderFrame = 0;
	}

	private void MoveRightSnapToTargetForKindleFire()
	{
		if (TBFUtils.IsKindleFire())
		{
			EZScreenPlacement component = SnapToTargetRight.GetComponent<EZScreenPlacement>();
			if ((bool)component)
			{
				Vector3 screenPos = component.screenPos;
				screenPos.x -= 32f;
				component.screenPos = screenPos;
				component.PositionOnScreen();
			}
		}
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	public void SwitchToFirstPerson(Actor actor)
	{
		SetCurrentUnitData();
		FriendlyUnitName.SetColor(ColourChart.HudGreen);
		GameController.Instance.UpdateTransitionEffect();
		HideChainGunUi(!mChainGunUIEnabled, true);
	}

	public void OnEnable()
	{
		ResetLookAmount();
		ResetMoveAmount();
	}

	public void OnDisable()
	{
		ResetLookAmount();
		ResetMoveAmount();
	}

	public void TriggerFirstPerson(float delay)
	{
		mFirstPersonTriggerTimer = delay;
	}

	public void FlashZoomButton(Color flashColour, float animationTime)
	{
		if (ZoomInButton.gameObject.activeInHierarchy)
		{
			mFlashZoomInButtonTimer = animationTime;
			mFlashZoomInButtonDuration = animationTime;
			mFlashZoomInFlashColour = flashColour;
			ZoomInButtonSuggest.Hide(false);
		}
	}

	private void Update()
	{
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
		Actor actor = ((!GameController.Instance) ? null : GameController.Instance.mFirstPersonActor);
		UpdateZoomInTimer();
		UpdateOnScreenContextMenu();
		UpdateKeyboardInput();
		UpdatePadInput();
		UpdateGestures();
		UpdateXPFeedback();
		UpdateSnapToTargetUI();
		UpdateWeaponBar();
		UpdateTrigger(actor);
		UpdateCrouch();
		UpdateFollowMe();
		ClearButtonBuffer();
		UpdateSnapToTarget();
		UpdateLookAmount();
		UpdateMoveAmount();
		UpdateUnitAbility();
		SetWeaponIcon();
		SetContextInteractionButton(null, actor);
		if (actor != null)
		{
			SetCrouchButtonFrame(actor.realCharacter.IsCrouching());
		}
		if (ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			UpdateGMGModesHUD();
		}
		UpdateHUDInvulnerabilityEffect();
	}

	private void LateUpdate()
	{
		mAnyTriggerPressed = false;
		Actor actor = ((!GameController.Instance) ? null : GameController.Instance.mFirstPersonActor);
		if (actor != null && actor.realCharacter != null)
		{
			bool hide = !actor.realCharacter.IsUsingFixedGun;
			HideChainGunUi(hide);
			UpdateTargetDetails(actor);
			UpdateMantleButton(actor);
			SetTriggerButtonIcon(actor);
			if (GKM.UnitCount(GKM.PlayerControlledMask & GKM.UpAndAboutMask & GKM.SelectableMask) <= 1)
			{
				HideFollowMe(true);
				HideFPPUntChangeButtons(true);
			}
			else
			{
				HideFollowMe(false);
				HideFPPUntChangeButtons(false);
			}
		}
		RemoveExpiredTapZones();
	}

	private void UpdateKeyboardInput()
	{
	}

	private void UpdatePadInput()
	{
		Controller.State state = Controller.GetState();
		if (state.connected && SwrveServerVariables.Instance.AllowGCController)
		{
			if (state.pause)
			{
				OnPauseButtonPress();
			}
			if (!TutorialToggles.IsRespotting && !(GameController.Instance == null) && !GameController.Instance.IsPaused && !MessageBoxController.Instance.IsAnyMessageActive)
			{
				UpdateAndroidPadInput(state);
			}
			return;
		}
		PadControl instance = PadControl.Instance;
		if (!(instance != null) || TutorialToggles.IsRespotting || !instance.IsPadConnected() || GameController.Instance == null || !GameController.Instance.IsFirstPerson)
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (!FPPADSLocked && mFirstPersonActor != null && mFirstPersonActor.realCharacter != null)
		{
			if (instance.AdsDown())
			{
				if (!mFirstPersonActor.realCharacter.IsAimingDownSights)
				{
					mFirstPersonActor.realCharacter.IsAimingDownSights = true;
					GameController.Instance.ADSAutoLockOn();
				}
			}
			else if (instance.AdsReleased())
			{
				mFirstPersonActor.realCharacter.IsAimingDownSights = false;
			}
		}
		if (instance.FireDown())
		{
			mTriggerPressedBuffer = true;
		}
		if (!FPPCrouchLocked && instance.CrouchPressed())
		{
			ToggleCrouch();
		}
		if (AllowSnapToTarget())
		{
			if (instance.TargetLeftPressed())
			{
				SnapEnemyLeftButtonClick();
			}
			if (instance.TargetRightPressed())
			{
				SnapEnemyRightButtonClick();
			}
		}
		if (instance.ReloadChangeWeaponHeld())
		{
			ReloadButtonClick();
		}
		if (!TPPDropGrenadeLocked && instance.GrenadePressed())
		{
			StartThrowGrenadeFirst();
			EndThrowGrenadeFirst();
		}
		if (!TPPDropClaymoreLocked && instance.ClaymorePressed() && StartPlantClaymoreFirst())
		{
			EndPlantClaymoreFirst();
		}
		if (instance.ReloadChangeWeaponHeld())
		{
			SwitchWeaponRight();
		}
		if (instance.ChangeUnitPressed())
		{
			SnapUnitRightButtonClick();
		}
		mMoveAmountInternalPad = instance.MoveAxis();
		if (AllowLook())
		{
			mLookAmountInternalPad = instance.LookAxis();
		}
	}

	private void UpdateIphonePadInput(Controller.State controllerState)
	{
		if (controllerState.leftShoulder.down)
		{
			ZoomToggled();
		}
		if (!GameController.Instance.IsFirstPerson)
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (!FPPADSLocked && mFirstPersonActor != null && mFirstPersonActor.realCharacter != null)
		{
			if (controllerState.leftTrigger.down && !mFirstPersonActor.realCharacter.IsAimingDownSights)
			{
				GameController.Instance.ADSAutoLockOn();
			}
			mFirstPersonActor.realCharacter.IsAimingDownSights = controllerState.leftTrigger.pressed;
		}
		if (controllerState.rightTrigger.pressed && TriggerButton != null && TriggerButton.gameObject.activeInHierarchy && AllowTrigger(mFirstPersonActor))
		{
			mTriggerPressedBuffer = true;
		}
		if (!FPPCrouchLocked && controllerState.b.down)
		{
			ToggleCrouch();
		}
		if (AllowSnapToTarget())
		{
			if (controllerState.dpad.left.down)
			{
				SnapEnemyLeftButtonClick();
			}
			if (controllerState.dpad.right.down)
			{
				SnapEnemyRightButtonClick();
			}
		}
		if (controllerState.x.down)
		{
			mWeaponSwapButtonDown = Time.realtimeSinceStartup;
		}
		if (controllerState.x.pressed && Time.realtimeSinceStartup - mWeaponSwapButtonDown > 0.5f)
		{
			SwitchWeaponRight();
			mWeaponSwapButtonDown = float.MaxValue;
		}
		if (controllerState.x.up && mWeaponSwapButtonDown != float.MaxValue)
		{
			ReloadButtonClick();
		}
		if (!TPPDropGrenadeLocked && controllerState.rightShoulder.down)
		{
			StartThrowGrenadeFirst();
			EndThrowGrenadeFirst();
		}
		if (!TPPDropClaymoreLocked && controllerState.y.down && StartPlantClaymoreFirst())
		{
			EndPlantClaymoreFirst();
		}
		if (controllerState.dpad.up.down)
		{
			SnapUnitRightButtonClick();
		}
		if (controllerState.dpad.down.down)
		{
			ToggleFollowMe();
		}
		if (controllerState.a.down)
		{
			if (!HoldBreathLocked && HoldBreathButton != null && HoldBreathButton.gameObject.activeInHierarchy && !HoldBreathButton.IsHidden())
			{
				OnHoldBreathPress();
			}
			else
			{
				CallContextInteraction();
			}
		}
		Vector2 input = new Vector2(controllerState.leftThumbstick.x, controllerState.leftThumbstick.y);
		mMoveAmountInternalPad = InputUtils.GetThumbstickInput(input, mMoveStickNotchesPad);
		if (AllowLook())
		{
			Vector2 input2 = new Vector2(controllerState.rightThumbstick.x, controllerState.rightThumbstick.y);
			mLookAmountInternalPad = TimeManager.DeltaTime * InputUtils.GetThumbstickInput(input2, mLookStickNotchesPad);
		}
	}

	private void UpdateAndroidPadInput(Controller.State controllerState)
	{
		bool isMogaBasic = Controller.GetIsMogaBasic();
		bool isMogaPro = Controller.GetIsMogaPro();
		if (isMogaPro && controllerState.leftShoulder.down)
		{
			ZoomToggled();
		}
		if (!GameController.Instance.IsFirstPerson)
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (!FPPADSLocked && mFirstPersonActor != null && mFirstPersonActor.realCharacter != null)
		{
			if ((controllerState.leftTrigger.down || (!isMogaPro && controllerState.leftShoulder.down)) && !mFirstPersonActor.realCharacter.IsAimingDownSights)
			{
				GameController.Instance.ADSAutoLockOn();
			}
			mFirstPersonActor.realCharacter.IsAimingDownSights = controllerState.leftTrigger.pressed || (!isMogaPro && controllerState.leftShoulder.pressed);
		}
		if ((controllerState.rightTrigger.pressed || (!isMogaPro && controllerState.rightShoulder.pressed)) && TriggerButton != null && TriggerButton.gameObject.activeInHierarchy && AllowTrigger(mFirstPersonActor))
		{
			mTriggerPressedBuffer = true;
		}
		if (!FPPCrouchLocked && controllerState.b.down)
		{
			ToggleCrouch();
		}
		if (!isMogaBasic && AllowSnapToTarget())
		{
			if (controllerState.dpad.left.down)
			{
				SnapEnemyLeftButtonClick();
			}
			if (controllerState.dpad.right.down)
			{
				SnapEnemyRightButtonClick();
			}
		}
		if (controllerState.x.down)
		{
			mWeaponSwapButtonDown = Time.realtimeSinceStartup;
		}
		if (controllerState.x.pressed && Time.realtimeSinceStartup - mWeaponSwapButtonDown > 0.5f)
		{
			SwitchWeaponRight();
			mWeaponSwapButtonDown = float.MaxValue;
		}
		if (controllerState.x.up && mWeaponSwapButtonDown != float.MaxValue)
		{
			ReloadButtonClick();
		}
		if (!TPPDropGrenadeLocked && controllerState.rightShoulder.down && isMogaPro)
		{
			StartThrowGrenadeFirst();
			EndThrowGrenadeFirst();
		}
		if (!TPPDropClaymoreLocked && controllerState.y.down && isMogaPro && StartPlantClaymoreFirst())
		{
			EndPlantClaymoreFirst();
		}
		if (!TPPDropGrenadeLocked && controllerState.y.down && !isMogaPro)
		{
			mClaymoreDropButtonDown = Time.realtimeSinceStartup;
		}
		if (!TPPDropClaymoreLocked && controllerState.y.pressed && !isMogaPro && Time.realtimeSinceStartup - mClaymoreDropButtonDown > 0.5f && StartPlantClaymoreFirst())
		{
			EndPlantClaymoreFirst();
			mClaymoreDropButtonDown = float.MaxValue;
		}
		if (!TPPDropGrenadeLocked && controllerState.y.up && !isMogaPro && mClaymoreDropButtonDown != float.MaxValue)
		{
			StartThrowGrenadeFirst();
			EndThrowGrenadeFirst();
		}
		if (!isMogaBasic && controllerState.dpad.up.down)
		{
			SnapUnitRightButtonClick();
		}
		if ((!isMogaBasic && controllerState.dpad.down.down) || controllerState.select)
		{
			ToggleFollowMe();
		}
		if (controllerState.a.down)
		{
			if (!HoldBreathLocked && HoldBreathButton != null && HoldBreathButton.gameObject.activeInHierarchy && !HoldBreathButton.IsHidden())
			{
				OnHoldBreathPress();
			}
			else
			{
				CallContextInteraction();
			}
		}
		Vector2 input = new Vector2(controllerState.leftThumbstick.x, controllerState.leftThumbstick.y);
		mMoveAmountInternalPad = InputUtils.GetThumbstickInput(input, mMoveStickNotchesPad);
		if (AllowLook())
		{
			Vector2 input2 = new Vector2(controllerState.rightThumbstick.x, controllerState.rightThumbstick.y);
			Vector2 thumbstickInput = InputUtils.GetThumbstickInput(input2, mLookStickNotchesPad);
			mLookAmountInternalPad = TimeManager.DeltaTime * (thumbstickInput * thumbstickInput.magnitude);
		}
	}

	private void UpdateZoomInTimer()
	{
		if (mFirstPersonTriggerTimer > 0f)
		{
			mFirstPersonTriggerTimer -= TimeManager.DeltaTime;
			if (mFirstPersonTriggerTimer <= 0f)
			{
				mFirstPersonTriggerTimer = -1f;
				if (ZoomInButton.scriptWithMethodToInvoke != null)
				{
					ZoomInButton.scriptWithMethodToInvoke.Invoke(ZoomInButton.methodToInvoke, 0f);
				}
			}
		}
		if (mFlashZoomInButtonTimer > 0f)
		{
			float alpha = mFlashZoomInButtonTimer / mFlashZoomInButtonDuration;
			ZoomInButtonSuggest.SetColor(mFlashZoomInFlashColour.Alpha(alpha));
			mFlashZoomInButtonTimer -= Time.deltaTime;
			if (mFlashZoomInButtonTimer <= 0f || !ZoomInButton.gameObject.activeInHierarchy)
			{
				ZoomInButtonSuggest.SetColor(new Color(0f, 0f, 0f, 0f));
				ZoomInButtonSuggest.Hide(true);
				mFlashZoomInButtonTimer = -1f;
			}
		}
	}

	private void UpdateGestures()
	{
		GameSettings instance = GameSettings.Instance;
		bool flag = instance != null && instance.PlayerGameSettings().FirstPersonGesturesEnabled;
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				mTouchStartPositions[i] = touch.position;
			}
		}
		float num = 0.25f;
		bool flag2 = Time.realtimeSinceStartup < mGestureStartedTime + num;
		if (!flag2)
		{
			mGestureStarted = false;
		}
		if (Input.touchCount == 0)
		{
			float num2 = 0.2f;
			if (flag && Time.realtimeSinceStartup < mGestureEndedTime + num2)
			{
				switch (mBufferedGestureType)
				{
				case GestureType.Crouch:
					Crouch();
					break;
				case GestureType.Stand:
					Stand();
					break;
				case GestureType.ZoomIn:
					ZoomToggled();
					break;
				case GestureType.ZoomOut:
					ZoomToggled();
					break;
				case GestureType.SwitchLeft:
					SnapUnitLeftButtonClick();
					break;
				case GestureType.SwitchRight:
					SnapUnitRightButtonClick();
					break;
				case GestureType.ThrowGrenade:
					StartThrowGrenadeFirst();
					EndThrowGrenadeFirst();
					break;
				}
				mBufferedGestureType = GestureType.None;
			}
			mCanStartGesture = flag;
			mGestureStarted = false;
			mWaitingForLeftTouch = false;
			mWaitingForRightTouch = false;
		}
		else if (Input.touchCount == 1)
		{
			if (mCanStartGesture && !mWaitingForLeftTouch && !mWaitingForRightTouch)
			{
				mGestureStartedTime = Time.realtimeSinceStartup;
				Touch touch2 = Input.GetTouch(0);
				if (InLeftGestureZone(touch2.position))
				{
					mWaitingForRightTouch = true;
					mLeftTouchStartPosition = touch2.position;
				}
				else if (InRightGestureZone(touch2.position))
				{
					mWaitingForLeftTouch = true;
					mRightTouchStartPosition = touch2.position;
				}
			}
			mCanStartGesture = false;
		}
		else if (Input.touchCount == 2)
		{
			Touch touch3 = Input.GetTouch(0);
			Touch touch4 = Input.GetTouch(1);
			if (mCanStartGesture)
			{
				Touch touch5 = ((!(touch3.position.x < touch4.position.x)) ? touch4 : touch3);
				Touch touch6 = ((!(touch3.position.x < touch4.position.x)) ? touch3 : touch4);
				if (InLeftGestureZone(touch5.position) && InRightGestureZone(touch6.position))
				{
					mGestureStarted = true;
					mGestureStartedTime = Time.realtimeSinceStartup;
					mLeftTouchIndex = touch5.fingerId;
					mRightTouchIndex = touch6.fingerId;
					mLeftTouchStartPosition = touch5.position;
					mRightTouchStartPosition = touch6.position;
					mLeftSamples.Reset();
					mRightSamples.Reset();
				}
			}
			else if (mWaitingForLeftTouch && flag2)
			{
				if (InLeftGestureZone(touch4.position))
				{
					mGestureStarted = true;
					mLeftTouchIndex = touch4.fingerId;
					mRightTouchIndex = touch3.fingerId;
					mLeftTouchStartPosition = touch4.position;
					mLeftSamples.Reset();
					mRightSamples.Reset();
				}
			}
			else if (mWaitingForRightTouch && flag2)
			{
				if (InRightGestureZone(touch4.position))
				{
					mGestureStarted = true;
					mLeftTouchIndex = touch3.fingerId;
					mRightTouchIndex = touch4.fingerId;
					mRightTouchStartPosition = touch4.position;
					mLeftSamples.Reset();
					mRightSamples.Reset();
				}
			}
			else if (mGestureStarted && mLeftTouchIndex < Input.touchCount && mRightTouchIndex < Input.touchCount)
			{
				Touch touch7 = Input.GetTouch(mLeftTouchIndex);
				Touch touch8 = Input.GetTouch(mRightTouchIndex);
				mLeftSamples.AddSample(touch7.deltaPosition);
				mRightSamples.AddSample(touch8.deltaPosition);
				Vector2 rhs = touch7.position - mLeftTouchStartPosition;
				Vector2 averageValue = mLeftSamples.GetAverageValue();
				Vector2 normalized = averageValue.normalized;
				Vector2 rhs2 = touch8.position - mRightTouchStartPosition;
				Vector2 averageValue2 = mRightSamples.GetAverageValue();
				Vector2 normalized2 = averageValue2.normalized;
				float num3 = 26f;
				float num4 = 10f;
				float num5 = 0.8f;
				bool flag3 = Vector2.Dot(new Vector2(-1f, 0f), normalized) > num5;
				bool flag4 = Vector2.Dot(new Vector2(-1f, 0f), rhs) > num3;
				bool flag5 = Vector2.Dot(new Vector2(1f, 0f), normalized) > num5;
				bool flag6 = Vector2.Dot(new Vector2(1f, 0f), rhs) > num3;
				bool flag7 = Vector2.Dot(new Vector2(0f, 1f), normalized) > num5;
				bool flag8 = Vector2.Dot(new Vector2(0f, 1f), rhs) > num3;
				bool flag9 = Vector2.Dot(new Vector2(0f, -1f), normalized) > num5;
				bool flag10 = Vector2.Dot(new Vector2(0f, -1f), rhs) > num3;
				bool flag11 = averageValue.magnitude > num4;
				bool flag12 = Vector2.Dot(new Vector2(1f, 0f), normalized2) > num5;
				bool flag13 = Vector2.Dot(new Vector2(1f, 0f), rhs2) > num3;
				bool flag14 = Vector2.Dot(new Vector2(-1f, 0f), normalized2) > num5;
				bool flag15 = Vector2.Dot(new Vector2(-1f, 0f), rhs2) > num3;
				bool flag16 = Vector2.Dot(new Vector2(0f, 1f), normalized2) > num5;
				bool flag17 = Vector2.Dot(new Vector2(0f, 1f), rhs2) > num3;
				bool flag18 = Vector2.Dot(new Vector2(0f, -1f), normalized2) > num5;
				bool flag19 = Vector2.Dot(new Vector2(0f, -1f), rhs2) > num3;
				bool flag20 = averageValue2.magnitude > num4;
				if (flag11 && flag3 && flag4 && flag20 && flag12 && flag13)
				{
					mBufferedGestureType = GestureType.ZoomIn;
					mGestureStarted = false;
					mGestureEndedTime = Time.realtimeSinceStartup;
				}
				else if (flag11 && flag5 && flag6 && flag20 && flag14 && flag15)
				{
					mBufferedGestureType = GestureType.ZoomOut;
					mGestureStarted = false;
					mGestureEndedTime = Time.realtimeSinceStartup;
				}
				else if (flag11 && flag9 && flag10 && flag20 && flag18 && flag19)
				{
					mBufferedGestureType = GestureType.Crouch;
					mGestureStarted = false;
					mGestureEndedTime = Time.realtimeSinceStartup;
				}
				else if (flag11 && flag7 && flag8 && flag20 && flag16 && flag17)
				{
					mBufferedGestureType = GestureType.Stand;
					mGestureStarted = false;
					mGestureEndedTime = Time.realtimeSinceStartup;
				}
				else if (flag11 && flag3 && flag4 && flag20 && flag14 && flag15)
				{
					mBufferedGestureType = GestureType.SwitchLeft;
					mGestureStarted = false;
					mGestureEndedTime = Time.realtimeSinceStartup;
				}
				else if (flag11 && flag5 && flag6 && flag20 && flag12 && flag13)
				{
					mBufferedGestureType = GestureType.SwitchRight;
					mGestureStarted = false;
					mGestureEndedTime = Time.realtimeSinceStartup;
				}
				else if (InLeftGrenadeZone(touch7.position) && InRightGrenadeZone(touch8.position))
				{
					mBufferedGestureType = GestureType.ThrowGrenade;
					mGestureStarted = false;
					mGestureEndedTime = Time.realtimeSinceStartup;
				}
			}
			mCanStartGesture = false;
			mWaitingForLeftTouch = false;
			mWaitingForRightTouch = false;
		}
		else if (Input.touchCount > 2)
		{
			mCanStartGesture = false;
			mGestureStarted = false;
		}
		float num6 = 0.2f;
		if (!(mGestureEndedTime + num6 < Time.realtimeSinceStartup))
		{
			return;
		}
		int num7 = 0;
		for (int j = 0; j < Input.touchCount; j++)
		{
			Touch touch9 = Input.GetTouch(j);
			if (GameController.Instance.IsFirstPerson && (mSnapToTargetFingerMask & (1 << touch9.fingerId)) == 0)
			{
				if (InLeftSnapZone(touch9.position))
				{
					if (touch9.phase == TouchPhase.Began)
					{
						mSnapLetTouchBeganWithFinger = touch9.fingerId;
					}
					else if (touch9.phase == TouchPhase.Ended && mSnapLetTouchBeganWithFinger == touch9.fingerId)
					{
						SnapEnemyLeftButtonClick();
					}
				}
				else if (InRightSnapZone(touch9.position))
				{
					if (touch9.phase == TouchPhase.Began)
					{
						mSnapRightTouchBeganWithFinger = touch9.fingerId;
					}
					else if (touch9.phase == TouchPhase.Ended && mSnapRightTouchBeganWithFinger == touch9.fingerId)
					{
						SnapEnemyRightButtonClick();
					}
				}
			}
			if (touch9.phase == TouchPhase.Ended && mSnapLetTouchBeganWithFinger == touch9.fingerId)
			{
				mSnapLetTouchBeganWithFinger = -1;
			}
			if (touch9.phase == TouchPhase.Ended && mSnapRightTouchBeganWithFinger == touch9.fingerId)
			{
				mSnapRightTouchBeganWithFinger = -1;
			}
			if (touch9.phase != TouchPhase.Ended || touch9.phase != TouchPhase.Canceled)
			{
				num7 |= touch9.fingerId;
			}
		}
		mSnapToTargetFingerMask &= num7;
	}

	private void ResetLookAmountFrames()
	{
		for (int i = 0; i < mLookAmountFrames.Length; i++)
		{
			mLookAmountFrames[i] = Vector2.zero;
		}
	}

	private void ResetLookAmount()
	{
		ResetLookAmountFrames();
		mLookAmountInternalTouch = Vector2.zero;
		mLookAmountExternalTouch = Vector2.zero;
		mLookAmountInternalPad = Vector2.zero;
		mLookAmountExternalPad = Vector2.zero;
	}

	private void ResetMoveAmount()
	{
		mMoveAmountInternalTouch = Vector2.zero;
		mMoveAmountExternal = Vector2.zero;
	}

	private void CacheHUDInvulnerabilityFlashSprites()
	{
		if (HUDInvulnerabilityFlashList == null)
		{
			return;
		}
		foreach (GameObject hUDInvulnerabilityFlash in HUDInvulnerabilityFlashList)
		{
			if (mHUDInvulnerabilityFlashSprites == null)
			{
				mHUDInvulnerabilityFlashSprites = new List<PackedSprite>(hUDInvulnerabilityFlash.GetComponentsInChildren<PackedSprite>(true));
			}
			else if (hUDInvulnerabilityFlash != null)
			{
				mHUDInvulnerabilityFlashSprites.AddRange(hUDInvulnerabilityFlash.GetComponentsInChildren<PackedSprite>(true));
			}
		}
	}

	private void UpdateHUDInvulnerabilityEffect()
	{
		if (!mHUDInvulnerabilityEffectActive || mHUDInvulnerabilityFlashSprites == null)
		{
			return;
		}
		foreach (PackedSprite mHUDInvulnerabilityFlashSprite in mHUDInvulnerabilityFlashSprites)
		{
			mHUDInvulnerabilityFlashSprite.SetColor(Color.Lerp(Color.green, Color.white, Mathf.Sin(Time.time * 10f)));
		}
	}

	private void ResizeLookAndMoveRegions()
	{
		BoxCollider boxCollider = (Look.collider as BoxCollider) ?? Look.gameObject.AddComponent<BoxCollider>();
		BoxCollider boxCollider2 = (Move.collider as BoxCollider) ?? Move.gameObject.AddComponent<BoxCollider>();
		if (boxCollider != null && boxCollider2 != null && GUISystem.Instance != null && GUISystem.Instance.m_guiCamera != null)
		{
			Vector3 vector = GUISystem.Instance.m_guiCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
			Vector3 vector2 = GUISystem.Instance.m_guiCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f));
			Vector3 vector3 = GUISystem.Instance.m_guiCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
			Vector3 size = vector3 - vector;
			Vector3 size2 = vector2 - vector;
			boxCollider.center = new Vector3(vector.x + 0.5f * size.x, vector.y + 0.5f * size.y, 0f);
			boxCollider.size = size;
			boxCollider2.center = new Vector3(vector.x + 0.5f * size2.x, vector.y + 0.5f * size2.y, 0f);
			boxCollider2.size = size2;
			Look.transform.localScale = Vector3.one;
			Look.transform.position = new Vector3(0f, 0f, Look.transform.position.z);
			Move.transform.localScale = Vector3.one;
			Move.transform.position = new Vector3(0f, 0f, Move.transform.position.z);
		}
	}

	private bool AllowLook()
	{
		return !mMovingTrigger;
	}

	private bool AllowTrigger(Actor firstPersonActor)
	{
		return firstPersonActor != null && firstPersonActor.realCharacter != null && firstPersonActor.realCharacter.CanBeControlledInFirstPerson() && !ViewModelRig.Instance().IsOverrideActive && !TutorialToggles.IsRespotting && !mMovingTrigger && !StealthKillPressed && !MeleePressed;
	}

	private bool AllowSnapToTarget()
	{
		return !mMovingTrigger;
	}

	private bool InGestureZone(Vector2 position)
	{
		return InLeftGestureZone(position) || InRightGestureZone(position);
	}

	private bool InLeftGestureZone(Vector2 position)
	{
		return new Rect(0f, 0f, 0.5f * (float)lastScreenWidth, lastScreenHeight).Contains(position);
	}

	private bool InRightGestureZone(Vector2 position)
	{
		return new Rect(0.5f * (float)lastScreenWidth, 0f, lastScreenWidth, lastScreenHeight).Contains(position);
	}

	private bool InLeftSnapZone(Vector2 position)
	{
		return new Rect(0f, 0.375f * (float)lastScreenHeight, 0.075f * (float)lastScreenWidth, 0.25f * (float)lastScreenHeight).Contains(position);
	}

	private bool InRightSnapZone(Vector2 position)
	{
		return new Rect(0.925f * (float)lastScreenWidth, 0.375f * (float)lastScreenHeight, lastScreenWidth, 0.25f * (float)lastScreenHeight).Contains(position);
	}

	private bool InLeftGrenadeZone(Vector2 position)
	{
		return false;
	}

	private bool InRightGrenadeZone(Vector2 position)
	{
		return false;
	}

	private bool InTriggerZone(Vector2 position)
	{
		bool flag = new Rect(0.5f * (float)lastScreenWidth, 0f, lastScreenWidth, lastScreenHeight).Contains(position);
		bool flag2 = InLeftSnapZone(position) || InRightSnapZone(position) || InLeftGrenadeZone(position) || InRightGrenadeZone(position);
		return flag && !flag2;
	}

	public bool InTriggerButtonZone(Vector2 position)
	{
		Rect rect = new Rect((float)lastScreenWidth - 0.4f * (float)lastScreenWidth, 0.15f * (float)lastScreenHeight, 0.4f * (float)lastScreenWidth, 0.7f * (float)lastScreenHeight);
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(TriggerButton.transform);
		Vector3 vector = TriggerButton.collider.bounds.size * 0.5f / num;
		Vector3 vector2 = position + new Vector2(0f - vector.x, 0f - vector.y);
		Vector3 vector3 = position + new Vector2(vector.x, 0f - vector.y);
		Vector3 vector4 = position + new Vector2(0f - vector.x, vector.y);
		Vector3 vector5 = position + new Vector2(vector.x, vector.y);
		bool flag = rect.Contains(vector2) && rect.Contains(vector3) && rect.Contains(vector4) && rect.Contains(vector5);
		bool flag2 = InLeftSnapZone(vector2) || InRightSnapZone(vector2) || InLeftGrenadeZone(vector2) || InRightGrenadeZone(vector2);
		bool flag3 = InLeftSnapZone(vector3) || InRightSnapZone(vector3) || InLeftGrenadeZone(vector3) || InRightGrenadeZone(vector3);
		bool flag4 = InLeftSnapZone(vector4) || InRightSnapZone(vector4) || InLeftGrenadeZone(vector4) || InRightGrenadeZone(vector4);
		bool flag5 = InLeftSnapZone(vector5) || InRightSnapZone(vector5) || InLeftGrenadeZone(vector5) || InRightGrenadeZone(vector5);
		return flag && !flag2 && !flag3 && !flag4 && !flag5;
	}

	public void ZoomToggled()
	{
		if (!ZoomInButton.gameObject.activeInHierarchy)
		{
			return;
		}
		if (GameController.Instance != null && GameController.Instance.GrenadeThrowingModeActive)
		{
			Debug.LogWarning("TPP/FPP Camera transition is being requested whilst in Grenade Throwing Mode. This will cause shenanigans.");
		}
		else if (GameController.Instance.mFirstPersonActor == null)
		{
			SetZoomButtonFrame(true);
			if (this.OnZoomInTriggered != null)
			{
				this.OnZoomInTriggered(this, new EventArgs());
			}
			TriggerFirstPerson(-1f);
			if (HUDMessenger.Instance != null)
			{
				HUDMessenger.Instance.HideActiveMessages(true);
			}
		}
		else
		{
			SetZoomButtonFrame(false);
			if (this.OnZoomOutTriggered != null)
			{
				this.OnZoomOutTriggered(this, new EventArgs());
			}
			if (HUDMessenger.Instance != null)
			{
				HUDMessenger.Instance.HideActiveMessages(true);
			}
		}
	}

	public void SetZoomButtons(bool zoomIn, bool zoomOut)
	{
		if ((zoomIn || zoomOut) && !TutorialToggles.LockZoom)
		{
			ZoomInButton.gameObject.SetActive(true);
		}
		else
		{
			ZoomInButton.gameObject.SetActive(false);
		}
	}

	public void AddToMessageLog(string text)
	{
		if (MessageLog != null)
		{
			MessageLog.AddLine(text);
		}
	}

	public void AddXpFeedback(int score, string msg, GameObject target)
	{
		if (m_XPMessages.Count == 0 && Time.realtimeSinceStartup - m_TimeOfLastXPMessage > MinTimeBetweenXPMessages)
		{
			DisplayXpFeedback(score, msg, target);
		}
		else
		{
			m_XPMessages.Enqueue(new XPMessageData(score, msg, target));
		}
	}

	private void DisplayXpFeedback(int score, string msg, GameObject target)
	{
		m_TimeOfLastXPMessage = Time.realtimeSinceStartup;
		Vector3 position = Vector3.zero;
		if (target != null)
		{
			position = CameraManager.Instance.CurrentCamera.WorldToScreenPoint(target.transform.position);
			if (position.z < 0f)
			{
				position.y = 0f - position.y;
				position.x = 0f - position.x;
			}
		}
		XpBlipFeedback xpBlipFeedback = UnityEngine.Object.Instantiate(XpFeedback, position, Quaternion.identity) as XpBlipFeedback;
		xpBlipFeedback.Display(score, msg, target, MinTimeBetweenXPMessages);
	}

	private void UpdateXPFeedback()
	{
		if (m_XPMessages.Count > 0 && Time.realtimeSinceStartup - m_TimeOfLastXPMessage >= MinTimeBetweenXPMessages)
		{
			XPMessageData xPMessageData = m_XPMessages.Dequeue();
			DisplayXpFeedback(xPMessageData.Score, xPMessageData.Message, xPMessageData.Target);
		}
	}

	public void AddContextMenu(InterfaceableObject iobj, Vector2 selectedScreenPos)
	{
		if (!(mContextMenuObj != null) || !(mInterfaceableObj == iobj))
		{
			if (mContextMenuObj != null)
			{
				ClearContextMenu();
			}
			InputManager.Instance.SetForContextMenu();
			mInterfaceableObj = iobj;
			mContextMenuObj = SceneNanny.Instantiate(ContextMenuPrefab) as GameObject;
			ContextMenuBase component = mContextMenuObj.GetComponent<ContextMenuBase>();
			component.ConstructForInteractableObject(mInterfaceableObj, selectedScreenPos, false);
		}
	}

	public void ClearContextMenu()
	{
		if (mContextMenuObj != null)
		{
			InputManager.Instance.SetForGamplay();
			mContextMenuObj.GetComponent<ContextMenuBase>().TransitionOff();
		}
		mContextMenuObj = null;
		mInterfaceableObj = null;
	}

	public void ClearContextMenu(InterfaceableObject iobj)
	{
		if (mInterfaceableObj == iobj)
		{
			if (mContextMenuObj != null)
			{
				InputManager.Instance.SetForGamplay();
				mContextMenuObj.GetComponent<ContextMenuBase>().TransitionOff();
			}
			mContextMenuObj = null;
			mInterfaceableObj = null;
		}
	}

	private void OnPauseButtonPress()
	{
		if (PauseButton.gameObject.activeInHierarchy)
		{
			GameController.Instance.TogglePause();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused && !MessageBoxController.Instance.IsAnyMessageActive && FrontEndController.Instance.ActiveScreen != ScreenID.HUDEditScreen)
		{
			GameController.Instance.TogglePause();
		}
	}

	private void OnTriggerMoreInfo()
	{
		if (mMoreInfoTrigger != null)
		{
			iTween component = MoreInfoButton.gameObject.GetComponent<iTween>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
				MoreInfoButton.gameObject.ColorUpdate(Color.white, 0f);
			}
			mMoreInfoTrigger.TriggerSequence();
			if (mMoreInfoTrigger.ScriptSeq != null)
			{
				SwrveEventsUI.MoreInfoViewed(mMoreInfoTrigger.ScriptSeq.name);
			}
		}
	}

	public void DisplayMissionName(string text)
	{
		if (MissionNameLabel != null)
		{
			MissionNameLabel.StartTyping(text);
		}
	}

	public void ToggleFollowMe()
	{
		if (FollowMeButton != null && FollowMeButton.gameObject.activeInHierarchy)
		{
			mFollowMeBuffer = true;
			FollowMeOrderFrame = Time.frameCount;
		}
	}

	public void ToggleFollowMe_Old()
	{
		GameController instance = GameController.Instance;
		TBFAssert.DoAssert(instance != null);
		instance.FirstPersonFollowMe = !instance.FirstPersonFollowMe;
		if (instance.FirstPersonFollowMe)
		{
			instance.IssueFollowMeCommand();
		}
	}

	public void UpdateFollowMe()
	{
		if (mFollowMeBuffer)
		{
			GameController instance = GameController.Instance;
			TBFAssert.DoAssert(instance != null);
			instance.FirstPersonFollowMe = true;
			instance.IssueFollowMeCommand();
		}
	}

	public void SetMoreInfoButton(MoreInfoTrigger mit)
	{
		MoreInfoButton.gameObject.SetActive(mit != null);
		iTween component = MoreInfoButton.gameObject.GetComponent<iTween>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
			MoreInfoButton.gameObject.ColorUpdate(Color.white, 0f);
		}
		MoreInfoButton.gameObject.ColorFrom(ColourChart.HudYellow, 0.5f, 0f, LoopType.pingPong);
		mMoreInfoTrigger = mit;
	}

	public void ForceCrouchButtonIcon(bool hide)
	{
		if (!FPPCrouchLocked && CrouchButtonIcon != null)
		{
			SetCrouchButtonFrame(hide);
		}
	}

	public void SetCrouchButtonFrame(bool crouching)
	{
		if (CrouchButtonIcon != null)
		{
			int num = (crouching ? 1 : 0);
			if (mCrouchingnAnimFrame != num)
			{
				mCrouchingnAnimFrame = num;
				CrouchButtonIcon.SetFrame(0, num);
				CrouchButtonIcon.SetColor((!crouching) ? ColourChart.HudWhite : ColourChart.HudYellow);
			}
		}
	}

	public void SetZoomButtonFrame(bool isFPP)
	{
		if (ZoomButtonIcon != null)
		{
			ZoomButtonIcon.SetFrame(0, isFPP ? 1 : 0);
		}
	}

	public void HideFollowMe(bool hide)
	{
		if (!FPPFollowMeLocked)
		{
			FollowMeButton.gameObject.SetActive(!hide);
		}
	}

	public void HideCrouchButton(bool hide)
	{
		if (FPPCrouchLocked)
		{
			return;
		}
		CrouchButton.gameObject.SetActive(!hide);
		if (!hide && (bool)GameController.Instance)
		{
			Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
			if (mFirstPersonActor != null)
			{
				SetCrouchButtonFrame(mFirstPersonActor.realCharacter.IsCrouching());
			}
		}
	}

	public void HideADSButton(bool hide)
	{
		if (!FPPADSLocked)
		{
			ADSButton.gameObject.SetActive(!hide);
		}
	}

	public void HideFPPUntChangeButtons(bool hide)
	{
		if (FPPUntChangeCached != hide && lockoutBecauseTheseButtonsGoHugeIfTheyAreFlashedOnAndOff <= Time.frameCount)
		{
			FPPUntChangeCached = hide;
			lockoutBecauseTheseButtonsGoHugeIfTheyAreFlashedOnAndOff = Time.frameCount + 10;
			if (ChangeUnitLeftButton != null)
			{
				ChangeUnitLeftButton.Hide(hide);
				BoxCollider boxCollider = ChangeUnitLeftButton.collider as BoxCollider;
				boxCollider.size = Vector3.zero;
			}
			if (ChangeUnitRightButton != null)
			{
				ChangeUnitRightButton.Hide(hide);
				BoxCollider boxCollider2 = ChangeUnitRightButton.collider as BoxCollider;
				boxCollider2.size = Vector3.zero;
			}
		}
	}

	public void HideContextInteractionButton(bool hide)
	{
		if (hide)
		{
			mContextIconTimout = Time.frameCount - 1;
		}
		if (ContextInteractionButton.gameObject.activeSelf != !hide && !ContextInteractionLocked)
		{
			ContextInteractionButton.gameObject.SetActive(!hide);
		}
	}

	public void SetContextInteractionButton(InterfaceableObject iobj)
	{
		SetContextInteractionButton(iobj, null);
	}

	public void SetContextInteractionButton(InterfaceableObject iobj, Actor firstPersonActor)
	{
		if (GameController.Instance != null && GameController.Instance.AimimgDownScopeThisFrame)
		{
			HideContextInteractionButton(true);
			return;
		}
		if (mContextIconTimout < (float)Time.frameCount)
		{
			HideContextInteractionButton(true);
		}
		else
		{
			HideContextInteractionButton(false);
		}
		bool flag = false;
		if (iobj == null)
		{
			if (firstPersonActor != null)
			{
				TaskUseFixedGun runningTask = firstPersonActor.tasks.GetRunningTask<TaskUseFixedGun>();
				if (runningTask != null && !runningTask.IsMountedInAVTOL && !GameController.Instance.IsLockedToFirstPerson && !ExitMinigunButtonLocked)
				{
					ContextInteractionIcon.SetFrame(0, 1);
					flag = true;
				}
			}
			else
			{
				ContextInteractionIcon.SetFrame(0, 19);
				flag = true;
			}
		}
		else
		{
			ContextInteractionIcon.SetFrame(0, (int)iobj.DefaultContextIcon);
			flag = true;
		}
		if (flag)
		{
			mContextIconTimout = Time.frameCount + 1;
			mContextInteractionObject = iobj;
		}
	}

	public void CallContextInteraction()
	{
		if (IsContextActionInProgress || !(ContextInteractionButton != null) || !ContextInteractionButton.gameObject.activeInHierarchy)
		{
			return;
		}
		if (mContextInteractionObject != null)
		{
			mContextInteractionObject.CallDefaultMethod();
			return;
		}
		if (mCurrNavZone != null)
		{
			OnMantlePress();
			return;
		}
		GameController instance = GameController.Instance;
		if (instance != null && instance.mFirstPersonActor != null)
		{
			Actor mFirstPersonActor = instance.mFirstPersonActor;
			if (mFirstPersonActor.tasks.IsRunningTask<TaskUseFixedGun>())
			{
				mFirstPersonActor.tasks.CancelTasks<TaskUseFixedGun>();
				instance.SwitchToFirstPerson(mFirstPersonActor, false);
			}
		}
	}

	public void AddGrenadeThrowMarker()
	{
		if (mActiveGrenadeThrowMarker == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(GrenadeThrowMarkerPrefab) as GameObject;
			mActiveGrenadeThrowMarker = gameObject.GetComponent<GrenadeThrowMarker>();
		}
	}

	public void RemoveGrenadeThrowMarker()
	{
		if (mActiveGrenadeThrowMarker != null)
		{
			UnityEngine.Object.Destroy(mActiveGrenadeThrowMarker.gameObject);
			mActiveGrenadeThrowMarker = null;
		}
	}

	public void AddPlaceFingerHereMarker(Vector3 worldpos, bool rotate)
	{
		Camera currentCamera = CameraManager.Instance.CurrentCamera;
		Vector3 position = currentCamera.WorldToScreenPoint(worldpos);
		if (position.z < 0f)
		{
			position.y = 0f - position.y;
			position.x = 0f - position.x;
			position.z = 0f - position.z;
		}
		position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(position);
		position.z = 0f;
		if (!(mActiveFingerHoldDragMarker == null))
		{
			return;
		}
		mActiveFingerHoldDragMarker = UnityEngine.Object.Instantiate(FingerHoldDragMarkerPrefab) as GameObject;
		if (mActiveFingerHoldDragMarker != null)
		{
			FingerHoldDragMarker component = mActiveFingerHoldDragMarker.GetComponent<FingerHoldDragMarker>();
			if (component != null)
			{
				component.SetFingerType(rotate ? FingerHoldDragMarker.FingerType.Rotate : FingerHoldDragMarker.FingerType.Point);
			}
			mActiveFingerHoldDragMarker.transform.position = position;
		}
	}

	public void AddPlaceFingerHereMarkerAtScreenCentre()
	{
		if (mActiveFingerHoldDragMarker == null)
		{
			mActiveFingerHoldDragMarker = UnityEngine.Object.Instantiate(FingerHoldDragMarkerPrefab) as GameObject;
		}
		mActiveFingerHoldDragMarker.transform.position = Vector3.zero;
	}

	public void RemovePlaceFingerHereMarker()
	{
		if (mActiveFingerHoldDragMarker != null)
		{
			UnityEngine.Object.Destroy(mActiveFingerHoldDragMarker);
			mActiveFingerHoldDragMarker = null;
		}
	}

	public void HideScreenEdging(bool hide)
	{
		ScreenEdging.BroadcastMessage("Hide", hide, SendMessageOptions.DontRequireReceiver);
	}

	private void UpdateOnScreenContextMenu()
	{
		if (!(GameController.Instance != null) || mContextMenuHidden)
		{
			return;
		}
		PlayerSquadManager instance = PlayerSquadManager.Instance;
		GameplayController gameplayController = GameplayController.Instance();
		if (!(instance != null) || !(gameplayController != null) || !(DropBodyButton != null) || !(DropGrenadeButton != null) || !(DropClaymoreButton != null))
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < gameplayController.Selected.Count; i++)
		{
			if (gameplayController.Selected[i].realCharacter.Carried != null)
			{
				num++;
			}
		}
		if (mNumBodiesCached != num)
		{
			DropBodyButton.spriteText.Text = num.ToString("D2");
			mNumBodiesCached = num;
		}
		DropBodyButton.gameObject.SetActive(num > 0);
		if (num > 0 && !SecureStorage.Instance.HasSeenDropBodyTutorial)
		{
			SecureStorage.Instance.HasSeenDropBodyTutorial = true;
			MessageBoxController instance2 = MessageBoxController.Instance;
			if (instance2 != null)
			{
				instance2.DoHintDialogue("S_DEADBODY_HINT_TITLE", "S_DEADBODY_HINT_BODY", HintMessageBox.ImageLayout.Left, "Tutorial_Images/tutorial_DeadBodies", null, string.Empty);
			}
			return;
		}
		if (mNumGrenadesCached != instance.GrenadeCount)
		{
			DropGrenadeButton.spriteText.Text = instance.GrenadeCount.ToString("D2");
			mNumGrenadesCached = instance.GrenadeCount;
		}
		if (mNumClaymoresCached != instance.ClaymoreCount)
		{
			DropClaymoreButton.spriteText.Text = instance.ClaymoreCount.ToString("D2");
			mNumClaymoresCached = instance.ClaymoreCount;
		}
	}

	public void HideOnScreenContextMenu(bool hide)
	{
		if (mContextMenuHidden != hide && DropBodyButton != null && DropGrenadeButton != null && DropClaymoreButton != null)
		{
			if (hide || !TPPDropBodyLocked)
			{
				DropBodyButton.gameObject.SetActive(!hide);
			}
			if (hide || !TPPDropGrenadeLocked)
			{
				DropGrenadeButton.gameObject.SetActive(!hide);
			}
			if (hide || !TPPDropClaymoreLocked)
			{
				DropClaymoreButton.gameObject.SetActive(!hide);
			}
			mContextMenuHidden = hide;
		}
	}

	public void HideTPPUnitSelecter(bool hide)
	{
		if (TPPUnitSelecter != null && !TPPUnitSelectorLocked)
		{
			TPPUnitSelecter.gameObject.SetActive(!hide);
			if (!hide)
			{
				TPPUnitSelecter.HideUnitSelecters(hide);
			}
		}
	}

	public void ThrowGrenadeThird()
	{
		if (!SecureStorage.Instance.HasSeenGrenadeTutorial)
		{
			SecureStorage.Instance.HasSeenGrenadeTutorial = true;
			MessageBoxController instance = MessageBoxController.Instance;
			if (instance != null)
			{
				instance.DoHintDialogue("S_GRENADE_HINT_TITLE", "S_GRENADE_HINT_BODY", HintMessageBox.ImageLayout.Left, "Tutorial_Images/tutorial_Grenade", null, string.Empty);
			}
		}
		else
		{
			PlayerSquadManager instance2 = PlayerSquadManager.Instance;
			if (instance2 != null)
			{
				instance2.ThrowGrenade(true);
			}
		}
	}

	public void PlaceMine()
	{
		if (!SecureStorage.Instance.HasSeenClaymoreTutorial)
		{
			SecureStorage.Instance.HasSeenClaymoreTutorial = true;
			MessageBoxController instance = MessageBoxController.Instance;
			if (instance != null)
			{
				instance.DoHintDialogue("S_CLAYMORE_HINT_TITLE", "S_CLAYMORE_HINT_BODY", HintMessageBox.ImageLayout.Left, "Tutorial_Images/tutorial_GrenadeRotate", null, string.Empty);
			}
		}
		else
		{
			PlayerSquadManager instance2 = PlayerSquadManager.Instance;
			if (instance2 != null)
			{
				instance2.DropClaymore(true);
			}
		}
	}

	public void DropBody()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (CanDropBody() && gameplayController != null)
		{
			GameplayController.Instance().BeginPlacingBody();
		}
	}

	public void ShowScore(bool show)
	{
		if (ScoreOutput != null)
		{
			ScoreOutput.gameObject.SetActive(show);
		}
	}

	public void UpdateScore(int score)
	{
		if (ScoreOutput != null)
		{
			ScoreOutput.Text = score.ToString("D5");
		}
	}

	public void ShowWave(bool show)
	{
		if (WaveOutput != null)
		{
			WaveOutput.gameObject.SetActive(show);
		}
	}

	public void UpdateWave(int wave)
	{
		if (WaveOutput != null)
		{
			string text = Language.Get("S_GMG_WAVE");
			WaveOutput.Text = text + " " + wave.ToString("D2");
		}
	}

	public void ShowTokens(bool show)
	{
		if (TokenOutput != null)
		{
			TokenOutput.gameObject.SetActive(show);
		}
	}

	public void UpdateTokens(int tokens)
	{
		if (TokenOutput != null)
		{
			TokenOutput.Text = string.Format("{0} {1}", CommonHelper.HardCurrencySymbol(), tokens.ToString("D6"));
		}
	}

	public void ShowGMGScore(bool show)
	{
		if (!(GMGScore != null))
		{
			return;
		}
		if (show)
		{
			GMGData.GameType gameType = GMGData.GameType.Total;
			ActStructure instance = ActStructure.Instance;
			MissionListings instance2 = MissionListings.Instance;
			if (instance != null && instance2 != null)
			{
				MissionData missionData = instance2.Mission(instance.CurrentMissionID);
				if (missionData != null)
				{
					gameType = missionData.Sections[instance.CurrentSection].GMGGameType;
				}
			}
			GMGScore.Show(gameType);
		}
		else
		{
			GMGScore.Hide();
		}
	}

	public void UpdateGMGModesHUD()
	{
		StatsManager instance = StatsManager.Instance;
		if (instance != null && GMGScore != null)
		{
			GMGScore.UpdateScore(instance.PlayerStats().GetCurrentMissionStat().Score);
			LeaderboardResult nearestToPlayer = instance.LeaderboardManagerInstance.GetNearestToPlayer();
			GMGScore.UpdateFriend(nearestToPlayer);
		}
	}

	public void UpdateMultipiler(int multiplier)
	{
		if (GMGScore != null)
		{
			GMGScore.UpdateMultiplier(multiplier);
		}
	}

	public void ShowGMGResults(bool show)
	{
		if (GMGResults != null)
		{
			GMGResults.gameObject.SetActive(show);
			if (show)
			{
				GMGResults.UpdateStats();
			}
		}
	}

	public void ShowEnemiesRemaining(bool show)
	{
		if (EnemiesRemainingOutput != null)
		{
			EnemiesRemainingOutput.gameObject.SetActive(show);
		}
	}

	public void UpdateEnemiesRemaining(int remaining)
	{
		if (EnemiesRemainingOutput != null)
		{
			EnemiesRemainingOutput.Text = remaining.ToString("D2");
		}
	}

	public void UpdateEnemiesRemaining(int remaining, int outOf)
	{
		if (EnemiesRemainingOutput != null)
		{
			EnemiesRemainingOutput.Text = string.Format("{0}/{1}", remaining.ToString("D2"), outOf.ToString("D2"));
		}
	}

	private bool CanDropBody()
	{
		GameplayController gameplayController = GameplayController.Instance();
		GameController instance = GameController.Instance;
		if (!instance.ClaymoreDroppingModeActive && !instance.PlacementModeActive && !gameplayController.SettingClaymore)
		{
			foreach (Actor item in gameplayController.Selected)
			{
				if (item.realCharacter.Carried != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void UpdateLookAmount()
	{
		if (AllowLook())
		{
			float num = 1f;
			float num2 = num;
			Vector2 vector = mLookAmountInternalTouch;
			if (vector.sqrMagnitude <= float.Epsilon)
			{
				if (!mLookAmountStall)
				{
					mLookAmountStall = true;
					vector = mLookAmountFrames[0];
				}
			}
			else
			{
				mLookAmountStall = false;
			}
			mLookAmountExternalTouch = vector;
			for (int i = 0; i < mLookAmountFrames.Length; i++)
			{
				Vector2 vector2 = mLookAmountFrames[i];
				mLookAmountFrames[i] = vector;
				float sqrMagnitude = (vector - vector2).sqrMagnitude;
				num *= InputSettings.FirstPersonTrackpadFilteringFalloff * Mathf.InverseLerp(InputSettings.FirstPersonTrackpadFilteringCutoff, 0f, sqrMagnitude);
				num2 += num;
				mLookAmountExternalTouch += num * vector2;
				vector = vector2;
			}
			mLookAmountExternalTouch *= 1f / num2;
			mLookAmountExternalTouch = ApplyLookAcceleration(mLookAmountExternalTouch);
			mLookAmountExternalPad = mLookAmountInternalPad;
		}
		else
		{
			mLookAmountExternalTouch = Vector2.zero;
			mLookAmountExternalPad = Vector2.zero;
		}
		mLookAmountInternalTouch = Vector2.zero;
		mLookAmountInternalPad = Vector2.zero;
	}

	private Vector2 ApplyLookAcceleration(Vector2 lookAmount)
	{
		Vector2 vector = Vector2.Scale(lookAmount, new Vector2(1f / (TimeManager.DeltaTime * (float)Screen.width), 1f / (TimeManager.DeltaTime * (float)Screen.height)));
		float num = Mathf.Max(0f, Mathf.Abs(vector.x) - InputSettings.FirstPersonTrackpadHorizontalAccelerationThreshold);
		float x = 1f + InputSettings.FirstPersonTrackpadHorizontalAccelerationAmount * num;
		float num2 = Mathf.Max(0f, Mathf.Abs(vector.y) - InputSettings.FirstPersonTrackpadVerticalAccelerationThreshold);
		float y = 1f + InputSettings.FirstPersonTrackpadVerticalAccelerationAmount * num2;
		return Vector2.Scale(lookAmount, new Vector2(x, y));
	}

	private void UpdateMoveAmount()
	{
		mMoveAmountExternal = ((!(mMoveAmountInternalTouch.sqrMagnitude > mMoveAmountInternalPad.sqrMagnitude)) ? mMoveAmountInternalPad : mMoveAmountInternalTouch);
		mMoveAmountInternalTouch = Vector2.zero;
		mMoveAmountInternalPad = Vector2.zero;
	}

	public void AnimateAddedAmmo()
	{
		iTween component = WeaponSelect.gameObject.GetComponent<iTween>();
		if (component == null)
		{
			Vector3[] path = new Vector3[2]
			{
				WeaponSelect.gameObject.transform.position + new Vector3(0f, 0.5f, 0f),
				WeaponSelect.gameObject.transform.position
			};
			WeaponSelect.gameObject.MoveTo(path, 0.2f, 0f, EaseType.linear);
		}
	}

	private void UpdateWeaponBar()
	{
		if (WeaponIcon.IsDragging || WeaponIcon.IsCancelingDrag)
		{
			Color color = AmmoProgressBar.Color;
			if (color.a != 0f)
			{
				color.a = 0f;
				AmmoProgressBar.Color = color;
			}
			return;
		}
		Color color2 = AmmoProgressBar.Color;
		float num = 1f - WeaponIcon.Color.a;
		if (color2.a != num)
		{
			color2.a = num;
			AmmoProgressBar.Color = color2;
		}
	}

	private void UpdateCrouch()
	{
		if (!mCrouchPressedBuffer || !GameController.Instance || mTriggerPressedBuffer)
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor != null)
		{
			if (mFirstPersonActor.realCharacter.GetStance() == BaseCharacter.Stance.Standing)
			{
				Crouch();
			}
			else
			{
				Stand();
			}
		}
	}

	private void UpdateTrigger(Actor firstPersonActor)
	{
		bool flag = AllowTrigger(firstPersonActor);
		MeleePressed = false;
		if (flag && firstPersonActor != null && firstPersonActor.realCharacter != null)
		{
			if ((bool)firstPersonActor.realCharacter.Carried)
			{
				DropPressed = mTriggerPressedBuffer;
				SetTriggerPressed(false);
				return;
			}
			if (CloseEnoughToStealthKill())
			{
				StealthKillPressed = mTriggerPressedBuffer;
				SetTriggerPressed(false);
				if (StealthKillPressed)
				{
					ActorToStealthKill = ((!(GameController.Instance.mFirstPersonActor != null)) ? null : GameController.Instance.mFirstPersonActor.awareness.closestVisibleEnemy);
				}
				return;
			}
			if (CloseEnoughToMelee())
			{
				MeleePressed = mTriggerPressedBuffer;
				SetTriggerPressed(false);
				return;
			}
		}
		bool flag2 = false;
		float num = 0.05f * Screen.dpi;
		if (!TriggerLocked)
		{
			bool singleTapActive = false;
			if (TapTriggerActive(out singleTapActive))
			{
				int num2 = ((!singleTapActive) ? 1 : 0);
				int touchCount = Input.touchCount;
				for (int i = 0; i < touchCount; i++)
				{
					Touch touch = Input.GetTouch(i);
					if (touch.tapCount > num2 && touch.phase == TouchPhase.Ended && InTriggerZone(touch.position) && Vector2.Distance(touch.position, mTouchStartPositions[touch.fingerId]) < num)
					{
						flag2 = true;
					}
				}
			}
		}
		SetTriggerPressed(flag && (flag2 || mTriggerPressedBuffer));
	}

	private void ClearButtonBuffer()
	{
		mCrouchPressedBuffer = false;
		mTriggerPressedBuffer = false;
		mFollowMeBuffer = false;
	}

	private void UpdateSnapToTarget()
	{
		HasSnappedToTarget = false;
		HasSnappedToTargetLeft = false;
		HasSnappedToTargetRight = false;
		if (AllowSnapToTarget())
		{
			if (mSnapLeftBuffered && this.OnSnapEnemyLeftClick != null && SnapToTargetLeft.gameObject.activeInHierarchy)
			{
				this.OnSnapEnemyLeftClick(this, new EventArgs());
				HasSnappedToTarget = true;
				HasSnappedToTargetLeft = true;
			}
			else if (mSnapRightBuffered && this.OnSnapEnemyRightClick != null && SnapToTargetRight.gameObject.activeInHierarchy)
			{
				this.OnSnapEnemyRightClick(this, new EventArgs());
				HasSnappedToTarget = true;
				HasSnappedToTargetRight = true;
			}
		}
		mSnapLeftBuffered = false;
		mSnapRightBuffered = false;
	}

	private void UpdateSnapToTargetUI()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < mDamageIndicators.Length; i++)
		{
			if (mDamageIndicators[i].Indicator.gameObject.activeSelf)
			{
				flag = true;
				if (mDamageIndicators[i].transform.rotation.eulerAngles.z < 180f)
				{
					flag2 = true;
				}
				else
				{
					flag3 = true;
				}
			}
		}
		if (mSnapToTargetLeftRed != flag2)
		{
			SnapToTargetLeft.Color = ((!flag2) ? ColourChart.HudWhite : ColourChart.HudRed);
		}
		if (mSnapToTargetRightRed != flag3)
		{
			SnapToTargetRight.Color = ((!flag3) ? ColourChart.HudWhite : ColourChart.HudRed);
		}
		mSnapToTargetLeftRed = flag2;
		mSnapToTargetRightRed = flag3;
		Vector3 vector = Vector3.one;
		Vector3 vector2 = Vector3.one;
		if (flag)
		{
			Vector3 vector3 = Vector3.one * 1f + Vector3.one * 0.5f * (1f + Mathf.Sin(Time.time * 30f));
			if (flag2)
			{
				vector = vector3;
			}
			if (flag3)
			{
				vector2 = vector3;
			}
		}
		if (vector != mSnapToTargetLeftScale)
		{
			SnapToTargetLeft.transform.localScale = vector;
			mSnapToTargetLeftScale = vector;
		}
		if (vector2 != mSnapToTargetRightScale)
		{
			SnapToTargetRight.transform.localScale = vector2;
			mSnapToTargetRightScale = vector2;
		}
	}

	private void UpdateTargetDetails(Actor firstPersonActor)
	{
		Actor firstPersonTarget = GameController.Instance.FirstPersonTarget;
		bool flag = firstPersonTarget != null && !firstPersonTarget.baseCharacter.IsDead();
		bool flag2 = false;
		if (firstPersonTarget != null && (firstPersonTarget.awareness.ChDefCharacterType == CharacterType.SentryGun || firstPersonTarget.awareness.ChDefCharacterType == CharacterType.SecurityCamera))
		{
			flag2 = true;
		}
		bool flag3 = firstPersonTarget != null && FactionHelper.AreEnemies(firstPersonActor.awareness.faction, firstPersonTarget.awareness.faction);
		float alpha = 0.5f * firstPersonActor.weapon.ActiveWeapon.GetCrosshairOpacity();
		Crosshair.Color = ((!flag) ? ColourChart.HudWhite.Alpha(alpha) : ((!flag3) ? ColourChart.HudGreen.Alpha(alpha) : ColourChart.HudRed.Alpha(alpha)));
		if (flag && !flag3 && firstPersonTarget.realCharacter.UnitName != null)
		{
			if (!flag2 && FriendlyUnitName.IsHidden())
			{
				FriendlyUnitName.Hide(false);
			}
			FriendlyUnitName.Text = firstPersonTarget.realCharacter.UnitName;
		}
		else if (!FriendlyUnitName.IsHidden())
		{
			FriendlyUnitName.Hide(true);
		}
		if (DebugDistancesDisplay && firstPersonTarget != null)
		{
			float magnitude = (firstPersonTarget.GetPosition() - firstPersonActor.GetPosition()).magnitude;
			FriendlyUnitName.Text = magnitude.ToString();
			if (FriendlyUnitName.IsHidden())
			{
				FriendlyUnitName.Hide(false);
			}
		}
	}

	private bool IsMantleVisible(Actor firstPersonActor, NavigationZone navZone, ref Vector3 navZonePos, ref Quaternion navZoneRot)
	{
		Vector3 position = firstPersonActor.GetPosition();
		Vector3 pos;
		Quaternion rot;
		navZone.GetReferencePositionRotation(position, out pos, out rot);
		Transform transform = firstPersonActor.realCharacter.FirstPersonCamera.transform;
		Vector2 vector = pos.xz() - transform.position.xz();
		vector.Normalize();
		float num = Vector3.Dot(vector, transform.forward.xz());
		if (num > 0f)
		{
			navZonePos = pos;
			navZoneRot = rot;
			NavigationZone.NavigationType type = navZone.Type;
			if (type == NavigationZone.NavigationType.VaultLowWallOneWay || type == NavigationZone.NavigationType.VaultWindowOneWay)
			{
				return Vector3.Angle(firstPersonActor.baseCharacter.FirstPersonCamera.transform.forward, navZone.transform.forward) <= 35f;
			}
			return true;
		}
		return false;
	}

	private void UpdateMantleButton(Actor firstPersonActor)
	{
		if (firstPersonActor == null || firstPersonActor.realCharacter.IsUsingFixedGun || GameController.Instance.AimimgDownScopeThisFrame)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(firstPersonActor.transform.position, 0.1f, mNavZoneLayerMask);
		if (array == null || array.Length == 0)
		{
			mCurrNavZone = null;
			return;
		}
		mCurrNavZone = null;
		Vector3 navZonePos = Vector3.zero;
		Quaternion navZoneRot = Quaternion.identity;
		float num = 1024f;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			float num2 = Vector3.Distance(firstPersonActor.transform.position, collider.transform.position);
			if (num2 < num)
			{
				NavZoneRef component = collider.GetComponent<NavZoneRef>();
				if (component != null && IsMantleVisible(firstPersonActor, component.NavZone, ref navZonePos, ref navZoneRot))
				{
					num = num2;
					mCurrNavZone = component.NavZone;
				}
			}
		}
		if (mCurrNavZone == null)
		{
			return;
		}
		if (mCurrNavZone.Type == NavigationZone.NavigationType.MantelLowDownOnly || mCurrNavZone.Type == NavigationZone.NavigationType.MantelHighDownOnly || mCurrNavZone.Type == NavigationZone.NavigationType.Mantel3M || mCurrNavZone.Type == NavigationZone.NavigationType.Mantel3MDownOnly)
		{
			if (Mathf.Abs(firstPersonActor.GetPosition().y - navZonePos.y) > 1f)
			{
				SetContextInteractionButton(null);
			}
			return;
		}
		if (mCurrNavZone.Type == NavigationZone.NavigationType.MantelHighUpOnly || mCurrNavZone.Type == NavigationZone.NavigationType.MantelLowUpOnly)
		{
			if (Mathf.Abs(firstPersonActor.GetPosition().y - navZonePos.y) < 1f)
			{
				SetContextInteractionButton(null);
			}
			return;
		}
		switch (mCurrNavZone.Type)
		{
		case NavigationZone.NavigationType.Vent:
		case NavigationZone.NavigationType.Mantel3MUpOnly:
		case NavigationZone.NavigationType.TwoManVault:
			return;
		}
		SetContextInteractionButton(null);
	}

	public void ClearSoftLock()
	{
		Crosshair.StaticCrosshair.renderer.enabled = false;
	}

	public void SetSoftLockPosition(Vector3 position)
	{
		Crosshair.StaticCrosshair.renderer.enabled = InputSettings.ShowSoftLockIndicator;
		Crosshair.StaticCrosshair.transform.position = position;
	}

	public void TakeDamage(Vector3 fromWorldPosition)
	{
		mDamageIndicators[mDamageIndicatorIndex].TakeDamage(fromWorldPosition);
		mDamageIndicatorIndex = (mDamageIndicatorIndex + 1) % mDamageIndicators.Length;
	}

	public void GrenadeLanded(Grenade grenade)
	{
		GrenadeIndicator.GrenadeLanded(grenade);
		GrenadeIndicator.Indicator.Color = GrenadeIndicator.Indicator.Color.Alpha(0.75f);
		GrenadeIndicator.Icon.Color = GrenadeIndicator.Icon.Color.Alpha(0.75f);
	}

	private void OnTriggerInput(ref POINTER_INFO ptr)
	{
		Actor firstPersonActor = ((!(GameController.Instance != null)) ? null : GameController.Instance.mFirstPersonActor);
		if (AllowTrigger(firstPersonActor))
		{
			if (ptr.active)
			{
				mTriggerPressedBuffer = true;
			}
			mAnyTriggerPressed = true;
			float num = 0.5f;
			mLookAmountInternalTouch += ((!ptr.active || ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS) ? Vector2.zero : (num * InputUtils.GetTrackpadInput(ptr.ToPointerInfo())));
			mSnapToTargetFingerMask |= 1 << ptr.id;
		}
	}

	public void SetTriggerPressed(bool pressed)
	{
		if (pressed)
		{
			TriggerPressed = true;
			TriggerButton.SetColor(ColourChart.HudButtonPress);
		}
		else
		{
			TriggerPressed = false;
			TriggerButton.SetColor(new Color(1f, 1f, 1f, 1f));
		}
	}

	public void OnLookInput(ref POINTER_INFO ptr)
	{
		if (ptr.id >= mValidLookIds.Length)
		{
			return;
		}
		bool flag = ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE || ptr.evt == POINTER_INFO.INPUT_EVENT.DRAG || (ptr.evt == POINTER_INFO.INPUT_EVENT.NO_CHANGE && mValidLookIds[ptr.id]);
		if (!flag)
		{
			mValidLookIds[ptr.id] = false;
		}
		if (mValidLookIds[ptr.id] && !TutorialToggles.LockFPPLook && !TutorialToggles.IsRespotting)
		{
			mLookAmountInternalTouch += ((!ptr.active || ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS) ? Vector2.zero : InputUtils.GetTrackpadInput(ptr.ToPointerInfo()));
		}
		if (flag)
		{
			mValidLookIds[ptr.id] = true;
		}
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
		{
			int id = ptr.id;
			if (Input.touchCount > id)
			{
				Touch touch = Input.GetTouch(id);
				int tapCount = touch.tapCount;
				if (tapCount > 1 && tapCount % 2 == 0)
				{
					mTapTriggerActive = InTriggerZone(touch.position) && mMovementSinceFirstTap < 32f;
				}
				else
				{
					mMovementSinceFirstTap = 0f;
				}
			}
			if (mTriggerPressedBuffer)
			{
				ptr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			}
		}
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			if (!SingleTapTriggerActive() && !DoubleTapTriggerActive())
			{
				CheckTapZone(ptr.devicePos.xy());
			}
			mTapTriggerActive = false;
		}
		mMovementSinceFirstTap += ptr.inputDelta.magnitude;
		HoldingView = ptr.active && !mTapTriggerActive;
	}

	private bool TapTriggerActive(out bool singleTapActive)
	{
		GameSettings instance = GameSettings.Instance;
		singleTapActive = false;
		bool flag = false;
		if (instance != null)
		{
			Player player = instance.PlayerGameSettings();
			singleTapActive = player.SingleTapShoot;
			flag = player.DoubleTapShoot;
		}
		return singleTapActive || flag;
	}

	private bool SingleTapTriggerActive()
	{
		GameSettings instance = GameSettings.Instance;
		return instance != null && instance.PlayerGameSettings().SingleTapShoot;
	}

	private bool DoubleTapTriggerActive()
	{
		GameSettings instance = GameSettings.Instance;
		return instance != null && instance.PlayerGameSettings().DoubleTapShoot;
	}

	public void OnMoveInput(ref POINTER_INFO ptr)
	{
		if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
		{
			CheckTapZone(ptr.devicePos.xy());
		}
		if (GameController.Instance != null && GameController.Instance.AllowMovementInFirstPerson && GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.realCharacter != null && GameController.Instance.mFirstPersonActor.realCharacter.CanMove() && !ViewModelRig.Instance().IsOverrideActive && !TutorialToggles.LockFPPMovement && !TutorialToggles.IsRespotting)
		{
			mMoveAmountInternalTouch = ((!ptr.active || ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS) ? Vector2.zero : InputUtils.GetThumbstickInput(ptr.ToPointerInfo(), mMoveStickNotchesTouch));
			if (mMoveAmountInternalTouch != Vector2.zero)
			{
				MoveStick.UpdateFromInput(ptr.origPos, mMoveAmountInternalTouch);
			}
		}
		else if (!TutorialToggles.LockFPPMovement && !TutorialToggles.LockFPPLook && !TutorialToggles.IsRespotting)
		{
			mLookAmountInternalTouch += ((!ptr.active || ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS) ? Vector2.zero : InputUtils.GetTrackpadInput(ptr.ToPointerInfo()));
		}
	}

	public void UseExplosiveWeaponNow()
	{
		if (CurrentFPPThrowingWeapon == FPPThrowWeaponIcon.Grenade)
		{
			ThrowGrenadeNow();
		}
		else
		{
			PlantClaymoreNow();
		}
	}

	private void ThrowGrenadeNow()
	{
		StartThrowGrenadeFirst();
		EndThrowGrenadeFirst();
	}

	private void PlantClaymoreNow()
	{
		if (StartPlantClaymoreFirst())
		{
			EndPlantClaymoreFirst();
		}
	}

	public void ToggleCrouch()
	{
		mCrouchPressedBuffer = true;
	}

	public void Stand()
	{
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor != null && CrouchButton.gameObject.activeInHierarchy && !mFirstPersonActor.baseCharacter.IsInASetPiece)
		{
			FoleySFX.Instance.PlayerStand.Play2D();
			mFirstPersonActor.realCharacter.Stand();
			mFirstPersonActor.realCharacter.ForceCrouch = false;
			if (CrouchButtonIcon != null)
			{
				SetCrouchButtonFrame(false);
			}
		}
	}

	public void Crouch()
	{
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor != null && CrouchButton.gameObject.activeInHierarchy && !mFirstPersonActor.baseCharacter.IsInASetPiece)
		{
			FoleySFX.Instance.PlayerCrouch.Play2D();
			mFirstPersonActor.realCharacter.Crouch();
			mFirstPersonActor.realCharacter.ForceCrouch = true;
			if (CrouchButtonIcon != null)
			{
				SetCrouchButtonFrame(true);
			}
		}
	}

	public void ToggleADS()
	{
		if (!GameController.Instance || !ADSButton.gameObject.activeInHierarchy)
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor != null && mFirstPersonActor.realCharacter != null)
		{
			mFirstPersonActor.realCharacter.IsAimingDownSights = !mFirstPersonActor.realCharacter.IsAimingDownSights;
			if (mFirstPersonActor.realCharacter.IsAimingDownSights)
			{
				GameController.Instance.ADSAutoLockOn();
			}
		}
	}

	public void ToggleWeapon()
	{
		if ((bool)GameController.Instance)
		{
			Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
			if (mFirstPersonActor != null)
			{
				mFirstPersonActor.realCharacter.IsAimingDownSights = false;
				InterfaceSFX.Instance.WeaponSwitch.Play2D();
				mFirstPersonActor.realCharacter.ToggleWeapon();
			}
		}
	}

	public void SnapEnemyLeftButtonClick()
	{
		mSnapLeftBuffered = true;
	}

	public void SnapEnemyRightButtonClick()
	{
		mSnapRightBuffered = true;
	}

	public void SnapUnitLeftButtonClick()
	{
		if (GameController.Instance != null && GameController.Instance.IsFirstPerson && UnitName.gameObject.activeInHierarchy && !TutorialToggles.PlayerSelectionLocked)
		{
			RealCharacter.SetUpdateFPPHudText();
			if (this.OnSnapUnitLeftClick != null)
			{
				this.OnSnapUnitLeftClick(this, new EventArgs());
			}
			SetCurrentUnitData();
		}
		if (ChangeUnitLeftButton.gameObject.activeInHierarchy)
		{
			AnimateButtonOnInvoke component = ChangeUnitLeftButton.gameObject.GetComponent<AnimateButtonOnInvoke>();
			if (component != null)
			{
				component.ActivateButtonPress();
			}
		}
	}

	public void SnapUnitRightButtonClick()
	{
		if (GameController.Instance != null && GameController.Instance.IsFirstPerson && UnitName.gameObject.activeInHierarchy && !TutorialToggles.PlayerSelectionLocked)
		{
			RealCharacter.SetUpdateFPPHudText();
			if (this.OnSnapUnitRightClick != null)
			{
				this.OnSnapUnitRightClick(this, new EventArgs());
			}
			SetCurrentUnitData();
		}
		if (ChangeUnitRightButton.gameObject.activeInHierarchy)
		{
			AnimateButtonOnInvoke component = ChangeUnitRightButton.gameObject.GetComponent<AnimateButtonOnInvoke>();
			if (component != null)
			{
				component.ActivateButtonPress();
			}
		}
	}

	public void SwitchWeaponLeft()
	{
		if (!PreventWeaponSwap)
		{
			ToggleWeapon();
			if (this.OnSwitchWeapon != null)
			{
				this.OnSwitchWeapon(this, new EventArgs());
			}
		}
	}

	public void SwitchWeaponRight()
	{
		if (!PreventWeaponSwap)
		{
			ToggleWeapon();
			if (this.OnSwitchWeapon != null)
			{
				this.OnSwitchWeapon(this, new EventArgs());
			}
		}
	}

	private void SetWeaponIcon()
	{
		if (!(GameController.Instance == null) && GameController.Instance.IsFirstPerson && (bool)GameController.Instance.mFirstPersonActor)
		{
			int num = GameController.Instance.mFirstPersonActor.weapon.DesiredWeapon.GetWeaponType();
			int num2 = 1;
			if (WeaponIcon.GetCurAnim() != null)
			{
				num2 = WeaponIcon.GetCurAnim().GetFrameCount();
			}
			if (num >= num2)
			{
				num = num2 - 1;
			}
			else if (num < 0)
			{
				num = 0;
			}
			if (WeaponIcon.GetCurAnim() == null || WeaponIcon.GetCurAnim().GetCurPosition() != num)
			{
				WeaponIcon.SetFrame(0, num);
			}
		}
	}

	public void SwitchGrenadeLeft()
	{
		SwitchGrenadeRight();
	}

	public void SwitchGrenadeRight()
	{
		if (mFPPThrowWeaponIcon == FPPThrowWeaponIcon.Grenade)
		{
			mFPPThrowWeaponIcon = FPPThrowWeaponIcon.Claymore;
		}
		else
		{
			mFPPThrowWeaponIcon = FPPThrowWeaponIcon.Grenade;
		}
		SetFPPThrowWeaponButton(mFPPThrowWeaponIcon);
	}

	public void SetFPPThrowWeaponButton(FPPThrowWeaponIcon icon)
	{
		mFPPThrowWeaponIcon = icon;
		FirstPersonGrenadeButton.SetFrame(0, (int)mFPPThrowWeaponIcon);
		RealCharacter.SetUpdateFPPHudText();
	}

	private bool CanPerformExplosiveWeaponTask()
	{
		return GameController.Instance == null || GameController.Instance.CanUseExplosivesInFirstPerson;
	}

	public void StartThrowGrenadeFirst()
	{
		if (GreneadeSelect.activeInHierarchy && CanPerformExplosiveWeaponTask() && PlayerSquadManager.Instance != null)
		{
			PlayerSquadManager.Instance.ThrowGrenade(false);
		}
	}

	public void EndThrowGrenadeFirst()
	{
		if (!(GameController.Instance != null))
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (!(mFirstPersonActor != null))
		{
			return;
		}
		Weapon_Grenade weapon_Grenade = (mFirstPersonActor.weapon.ActiveWeapon as Weapon_Grenade) ?? (mFirstPersonActor.weapon.DesiredWeapon as Weapon_Grenade);
		if (weapon_Grenade != null)
		{
			weapon_Grenade.Release();
			if (this.OnThrowGrenadeClick != null)
			{
				this.OnThrowGrenadeClick(this, new EventArgs());
			}
		}
	}

	public bool StartPlantClaymoreFirst()
	{
		if (!GreneadeSelect.activeInHierarchy)
		{
			return false;
		}
		if (!CanPerformExplosiveWeaponTask())
		{
			return false;
		}
		if (GameController.Instance != null)
		{
			Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
			if (mFirstPersonActor != null && mFirstPersonActor.health.HealthEmpty)
			{
				return false;
			}
		}
		if (PlayerSquadManager.Instance != null)
		{
			PlayerSquadManager.Instance.DropClaymore(false);
		}
		return true;
	}

	public void EndPlantClaymoreFirst()
	{
		if ((PlayerSquadManager.Instance != null && PlayerSquadManager.Instance.ClaymoreCount == 0) || !(GameController.Instance != null))
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor != null)
		{
			if (mFirstPersonActor.baseCharacter.IsInASetPiece)
			{
				Debug.LogWarning("EndPlantClaymoreFirst - failed due to FPP Actor being in a Set-Piece");
				return;
			}
			if (mFirstPersonActor.tasks.IsRunningTask<TaskOpen>())
			{
				Debug.LogWarning("EndPlantClaymoreFirst - failed due to FPP Actor running TaskOpen");
				return;
			}
			if (mFirstPersonActor.tasks.RunningTaskDeniesPlayerInput)
			{
				Debug.LogWarning("EndPlantClaymoreFirst - failed due to running Task suppressing player input");
				return;
			}
			if (InteractionsManager.Instance != null && InteractionsManager.Instance.IsPlayingCutscene())
			{
				Debug.LogWarning("EndPlantClaymoreFirst - failed due to Cutscene playing");
				return;
			}
			if (BlackBarsController.Instance != null && BlackBarsController.Instance.BlackBarsEnabled)
			{
				Debug.LogWarning("EndPlantClaymoreFirst - failed due to Black Bars being active");
				return;
			}
			Vector2 vector = mFirstPersonActor.realCharacter.FirstPersonCamera.transform.forward.xz();
			Vector3 vector2 = new Vector3(vector.x, 0f, vector.y);
			Vector3 position = mFirstPersonActor.transform.position;
			Vector3 dropPosition = position + vector2;
			OrdersHelper.OrderDropClaymore(GameplayController.instance, dropPosition, vector2);
		}
	}

	private void SetTriggerButtonIcon(Actor firstPersonActor)
	{
		bool flag = true;
		if ((TriggerLocked && FPPStealthKillLocked) || (TriggerLocked && !FPPStealthKillLocked && !CloseEnoughToStealthKill()) || (firstPersonActor.realCharacter != null && !firstPersonActor.realCharacter.CanBeControlledInFirstPerson()) || ViewModelRig.Instance().IsOverrideActive)
		{
			TriggerButtonIcon.SetFrame(0, 0);
			flag = false;
		}
		else if (firstPersonActor.realCharacter.Carried != null)
		{
			TriggerButtonIcon.SetFrame(0, 2);
		}
		else if (CloseEnoughToStealthKill() || CloseEnoughToMelee())
		{
			TriggerButtonIcon.SetFrame(0, 1);
		}
		else
		{
			TriggerButtonIcon.SetFrame(0, 0);
			flag = true;
		}
		TriggerButton.gameObject.SetActive(flag);
	}

	public int GetTriggerFrame()
	{
		return TriggerButtonIcon.GetCurAnim().GetCurPosition();
	}

	private void SetCurrentUnitData()
	{
		TriggerButtonIcon.SetFrame(0, 0);
		Actor actor = null;
		if ((bool)GameController.Instance)
		{
			actor = GameController.Instance.mFirstPersonActor;
		}
		if (actor != null)
		{
			UnitName.Text = actor.realCharacter.UnitName.ToUpper();
			SetTriggerButtonIcon(actor);
			if (CrouchButtonIcon != null)
			{
				SetCrouchButtonFrame(actor.realCharacter.IsCrouching());
			}
			SetZoomButtonFrame(true);
		}
		else
		{
			UnitName.Text = "Unknown";
		}
		HeartBeat.SetupForNewUnit();
		SetFPPThrowWeaponButton(mFPPThrowWeaponIcon);
	}

	public void SetUnitAbility(string ability, float duration, float alertDuration)
	{
		UnitAbility.Text = ability;
		mUnitAbilityTimer = 0f;
		mUnitAbilityDuration = duration;
		mUnitAbilityAlertTime = alertDuration;
		mUnitAbilityTextState = UnityAbilityTextState.Appearing;
		iTween component = UnitAbility.GetComponent<iTween>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
		UnitAbility.SetColor(Color.white);
		UnitAbility.gameObject.ScaleFrom(Vector3.zero, 0.5f, 0f);
	}

	public void ClearUnitAbility()
	{
		UnitAbility.Text = string.Empty;
		mUnitAbilityDuration = 0f;
		mUnitAbilityAlertTime = 0f;
		mUnitAbilityTimer = 0f;
		iTween component = UnitAbility.GetComponent<iTween>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	private void UpdateUnitAbility()
	{
		if (!(mUnitAbilityDuration > 0f))
		{
			return;
		}
		mUnitAbilityTimer += Time.deltaTime;
		mUnitAbilityDuration -= Time.deltaTime;
		if (mUnitAbilityDuration <= 0f)
		{
			ClearUnitAbility();
		}
		else if (mUnitAbilityDuration <= mUnitAbilityAlertTime)
		{
			if (mUnitAbilityTextState != UnityAbilityTextState.Warning)
			{
				GMGSFX.Instance.TimedRewardExpiring.Play2D();
				mUnitAbilityTextState = UnityAbilityTextState.Warning;
				iTween component = UnitAbility.GetComponent<iTween>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				UnitAbility.gameObject.ScaleUpdate(Vector3.one, 0f);
				UnitAbility.gameObject.ColorUpdate(Color.white, 0f);
				UnitAbility.gameObject.ColorTo(Color.red, 0.3f, 0f, LoopType.pingPong);
			}
		}
		else if (mUnitAbilityTextState == UnityAbilityTextState.Appearing)
		{
			iTween component2 = UnitAbility.GetComponent<iTween>();
			if (component2 == null)
			{
				mUnitAbilityTextState = UnityAbilityTextState.Normal;
				UnitAbility.gameObject.ColorTo(ColourChart.HudBlue, 1f, 0f, LoopType.pingPong);
			}
		}
	}

	private bool CloseEnoughToMelee()
	{
		if (MeleePressed)
		{
			return false;
		}
		GameController instance = GameController.Instance;
		if (instance == null || !instance.IsFirstPerson)
		{
			return false;
		}
		Actor mFirstPersonActor = instance.mFirstPersonActor;
		if (mFirstPersonActor == null)
		{
			return false;
		}
		if (GameController.Instance.AimimgDownScopeThisFrame)
		{
			return false;
		}
		Actor closestVisibleEnemy = mFirstPersonActor.awareness.closestVisibleEnemy;
		if (closestVisibleEnemy == null || !IsValidMeleeTarget(closestVisibleEnemy.awareness.ChDefCharacterType) || closestVisibleEnemy.baseCharacter.IsDead())
		{
			return false;
		}
		Vector3 v = closestVisibleEnemy.transform.position - mFirstPersonActor.transform.position;
		if (v.sqrMagnitude > Weapon_Knife.KnifeRange)
		{
			return false;
		}
		return Mathf.Abs(Vector2.Angle(mFirstPersonActor.awareness.LookDirectionXZ, v.xz())) < 22.5f;
	}

	private bool IsValidMeleeTarget(CharacterType characterType)
	{
		return characterType == CharacterType.Human || characterType == CharacterType.RiotShieldNPC || characterType == CharacterType.RPG;
	}

	private bool CloseEnoughToStealthKill()
	{
		GameController instance = GameController.Instance;
		if (StealthKillPressed)
		{
			return false;
		}
		if (instance == null || !instance.IsFirstPerson)
		{
			return false;
		}
		Actor mFirstPersonActor = instance.mFirstPersonActor;
		if (mFirstPersonActor == null)
		{
			return false;
		}
		if (GameController.Instance.AimimgDownScopeThisFrame)
		{
			return false;
		}
		AwarenessComponent awareness = mFirstPersonActor.awareness;
		Actor closestVisibleEnemy = awareness.closestVisibleEnemy;
		if (closestVisibleEnemy == null || closestVisibleEnemy.awareness.ChDefCharacterType != CharacterType.Human || closestVisibleEnemy.baseCharacter.IsDead() || (closestVisibleEnemy.behaviour != null && closestVisibleEnemy.behaviour.InActiveAlertState()))
		{
			return false;
		}
		float sqrMagnitude = (closestVisibleEnemy.transform.position - mFirstPersonActor.transform.position).sqrMagnitude;
		if (sqrMagnitude > 3f)
		{
			return false;
		}
		float num = Vector2.Dot(awareness.LookDirectionXZ, closestVisibleEnemy.awareness.LookDirectionXZ);
		if (num <= 0f)
		{
			return false;
		}
		return Mathf.Abs(Vector2.Angle(awareness.LookDirectionXZ, closestVisibleEnemy.transform.position.xz() - mFirstPersonActor.transform.position.xz())) < 22.5f;
	}

	public void InButtonClick()
	{
		if (this.OnInClick != null)
		{
			this.OnInClick(this, new EventArgs());
		}
	}

	public void OutButtonClick()
	{
		if (this.OnOutClick != null)
		{
			this.OnOutClick(this, new EventArgs());
		}
	}

	public void DuckButtonClick()
	{
		if (this.OnDuckClick != null)
		{
			this.OnDuckClick(this, new EventArgs());
		}
	}

	public void ReloadButtonClick()
	{
		if (this.OnReloadClick != null)
		{
			this.OnReloadClick(this, new EventArgs());
		}
	}

	public void SetHoldBreathUI(bool enabled, float fraction)
	{
		if (!HoldBreathLocked)
		{
			if (HoldBreathButton.gameObject.activeInHierarchy && HoldBreathButton.IsHidden() != !enabled)
			{
				HoldBreathButton.BroadcastMessage("Hide", !enabled, SendMessageOptions.DontRequireReceiver);
			}
			if (HoldBreathTimer.gameObject.activeInHierarchy && !HoldBreathTimer.IsHidden())
			{
				HoldBreathTimer.Value = fraction;
			}
		}
	}

	public void OnHoldBreathPress()
	{
		if (GameController.Instance != null)
		{
			Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
			if (mFirstPersonActor != null && !mFirstPersonActor.realCharacter.IsHoldingBreath)
			{
				mFirstPersonActor.realCharacter.StartHoldingBreath();
			}
		}
	}

	public void OnMantlePress()
	{
		if (!(GameController.Instance != null))
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (!(mFirstPersonActor != null))
		{
			return;
		}
		SetPieceLogic setPieceLogic = null;
		float delay = 0f;
		float time = Time.time;
		if (mFirstPersonActor.realCharacter.IsAimingDownSights)
		{
			ToggleADS();
		}
		setPieceLogic = mFirstPersonActor.realCharacter.mNavigationSetPiece.CreateFirstPersonSetPiece(mFirstPersonActor, mCurrNavZone, mFirstPersonActor.baseCharacter.MovementStyleActive, time, out delay);
		if (setPieceLogic != null)
		{
			if (setPieceLogic.GetNumActorsInvolved() > 1)
			{
				OrdersHelper.OrderMultiCharacterSetPiece(GameplayController.Instance(), setPieceLogic, mFirstPersonActor.GetPosition(), null);
			}
			else
			{
				OrdersHelper.OrderSetPiece(GameplayController.Instance(), setPieceLogic, null);
			}
		}
	}

	public void OnStealthKillPress()
	{
		if (GameController.Instance == null)
		{
			return;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (!(mFirstPersonActor == null))
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (!(gameplayController == null))
			{
				OrdersHelper.OrderMeleeAttack(gameplayController, mFirstPersonActor);
			}
		}
	}

	public void HideChainGunUi(bool hide)
	{
		HideChainGunUi(hide, false);
	}

	public void HideChainGunUi(bool hide, bool force)
	{
		if (force || mChainGunUIEnabled == hide)
		{
			GreneadeSelect.BroadcastMessage("Hide", !hide, SendMessageOptions.DontRequireReceiver);
			WeaponSelect.BroadcastMessage("Hide", !hide, SendMessageOptions.DontRequireReceiver);
			ChainGun.BroadcastMessage("Hide", hide, SendMessageOptions.DontRequireReceiver);
			HeatWarningLeft.Hide(hide);
			HeatWarningRight.Hide(hide);
			HeatBarLeft.Hide(hide);
			HeatBarRight.Hide(hide);
			mChainGunUIEnabled = !hide;
		}
	}

	public void SetChainGunHeat(float heatFraction, bool jammed)
	{
		HideChainGunUi(false);
		HeatBarLeft.Value = heatFraction;
		HeatBarRight.Value = heatFraction;
		float num = 0.05f;
		Color color = ((!jammed && !(Time.time % (num * 2f) > num)) ? ColourChart.HudWhite : ColourChart.HudRed);
		if (jammed || heatFraction > 0.8f)
		{
			HeatWarningLeft.SetColor(color);
			HeatWarningRight.SetColor(color);
		}
		else
		{
			HeatWarningLeft.SetColor(ColourChart.HudWhite);
			HeatWarningRight.SetColor(ColourChart.HudWhite);
		}
	}
}
