using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(ContextMenuTriggerLogic))]
public class GameController : MonoBehaviour
{
	private class AutoSnapTarget
	{
		public SnapTarget Target;

		public Vector3 LookAt;

		public AutoSnapTarget(SnapTarget target)
		{
			Target = target;
		}

		public AutoSnapTarget(Vector3 lookAt)
		{
			LookAt = lookAt;
		}

		public Vector3 GetSnapPosition()
		{
			return (!(Target != null)) ? LookAt : Target.GetSnapPosition();
		}
	}

	public enum SnapType
	{
		SnapLeft = 0,
		SnapRight = 1,
		SnapAny = 2,
		SnapShort = 3
	}

	public delegate void OnExitFirstPersonEventHandler();

	public delegate void OnSwitchToPersonEventHandler();

	public delegate void OnFirstPersonActorChangeEventHandler();

	private const float kStartTransitionTime = 0.75f;

	private bool mAimimgDownScopeThisFrame;

	private bool m_bDoBriefing;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private static GameController smInstance;

	private GameObject mCommonHud;

	private GameObject mStrategyHud;

	private GameObject mFirstPersonHud;

	private TechMenuController mTechMenuCtrl;

	private LoadoutMenuNavigator mLoadoutMenuCtrl;

	private OverwatchController mOverwatchController;

	private ContextMenuTriggerLogic mCMTriggerLogic;

	private Actor mFPActor;

	public Actor mPreviousFirstPersonActor;

	private Transform mFirstPersonPointOfInterest;

	private Vector3 mFirstPersonLockOnAngles;

	private Vector3 mFirstPersonSoftLockAngles;

	private Vector3 mSoftLockIntensity;

	private float mTimeOnFirstPersonTarget;

	private Vector3 mLastLockOnUpdate;

	private float mLastAimAssistValue;

	private float mSprintBlend;

	private float mTargetDist;

	private Actor mFirstPersonTarget;

	private CoverPointCore mFirstPersonCoverPoint;

	public bool PlayIntroSequence = true;

	public ScriptedSequence mIntroSequence;

	private bool mZoomInAvailable;

	private bool mZoomOutAvailable;

	public bool mIsLeavingFirstPersonForSetPiece;

	private bool mSuppressHud;

	private float mTimePlayed;

	private float mMinuteCount;

	private bool mGrenadeThrowingMode;

	private bool mGMGReviveMode;

	private bool mClaymoreDroppingMode;

	private bool mSentryHackingMode;

	private bool mPlacementMode;

	private bool mGameplayStarted;

	private float mTimeSinceFirstPersonTransition;

	private float mTimeSinceFirstPersonTransitionHUD;

	private float mTimeSinceLock;

	private SnapTarget mLastLockOnTarget;

	private Actor mSoftLockTarget;

	private float mSwayTimer;

	public float FirstPersonGyroThreshold = 0.5f;

	public AnimationCurve FirstPersonFieldOfViewSensitivityModifer;

	private bool mMissionEnding;

	private bool mForceMissionEnd;

	private float mDelayTime;

	private GyroController mGyroController;

	private AutoSnapTarget mAutoSnapTarget;

	private bool mDisableZoomButtonsDuringCoroutineTransition;

	private bool mPaused;

	private bool mScreenRotationLocked;

	private bool mAutorotateToLandscapeLeftRestoreState;

	private bool mAutorotateToLandscapeRightRestoreState;

	private bool mAutorotateToPortraitRestoreState;

	private bool mAutorotateToPortraitUpsideDownRestoreState;

	private static float MINIMUM_FPS_DISTANCE_HIGH_COVER_IS_VALID = 1f;

	private static float MINIMUM_FPS_DISTANCE_LOW_COVER_IS_VALID = 2f;

	public bool AimimgDownScopeThisFrame
	{
		get
		{
			return AimingDownScope;
		}
	}

	public bool PlayerEngagedInCombat
	{
		get
		{
			ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				if (a.awareness.EngagedInCombat())
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsPlayerBreaching { get; set; }

	public float MovementSpeedMultiplier { get; set; }

	public bool FirstPersonTransitionInActive { get; private set; }

	public float TransitionEffectAmount
	{
		get
		{
			float time = ((InteractionsManager.Instance.TransitionState == InteractionsManager.TransState.None) ? mTimeSinceFirstPersonTransition : InteractionsManager.Instance.TimeSinceTransition);
			return EffectsController.Instance.Effects.CameraTransitionRamp.Evaluate(time);
		}
		set
		{
			mTimeSinceFirstPersonTransition = value;
		}
	}

	public float TransitionEffectAmountHUD
	{
		get
		{
			float time = ((InteractionsManager.Instance.TransitionState == InteractionsManager.TransState.None) ? mTimeSinceFirstPersonTransitionHUD : InteractionsManager.Instance.TimeSinceTransition);
			return EffectsController.Instance.Effects.CameraTransitionRamp.Evaluate(time);
		}
		set
		{
			mTimeSinceFirstPersonTransitionHUD = value;
		}
	}

	public bool ShouldFlinchInFirstPerson
	{
		get
		{
			if (mFirstPersonActor != null && mFirstPersonActor.baseCharacter.IsUsingFixedGun)
			{
				return false;
			}
			return mTimeSinceLock > 2f;
		}
	}

	public bool DirectFireToSoftLockPosition
	{
		get
		{
			return InputSettings.DirectFireToSoftLockPosition;
		}
	}

	public bool FirstPersonAccuracyCheat { get; set; }

	public bool SoftLockDisabled { get; set; }

	public bool IsLockedToFirstPerson { get; set; }

	public bool IsLockedToCurrentCharacter { get; set; }

	public bool AllowMovementInFirstPerson { get; set; }

	public bool CanUseExplosivesInFirstPerson
	{
		get
		{
			return !IsPlayerBreaching && mFirstPersonActor != null && mFirstPersonActor.realCharacter != null && mFirstPersonActor.realCharacter.CanBeControlledInFirstPerson();
		}
	}

	public float TapToTargetBlend { get; set; }

	public bool AllowFirstPersonAtAnyPoint { get; set; }

	public bool AllowSeparateViewModelBands
	{
		get
		{
			return BlackBarsController.Instance == null || !BlackBarsController.Instance.BlackBarsEnabled;
		}
	}

	public bool AllowExplosionShake
	{
		get
		{
			return !IsPlayerBreaching;
		}
	}

	public bool LockGyro { get; set; }

	public static GameController Instance
	{
		get
		{
			return smInstance;
		}
	}

	public static ContextMenuTriggerLogic ContextMenuLogic
	{
		get
		{
			return smInstance.mCMTriggerLogic;
		}
	}

	public uint PeakHeapUsage { get; private set; }

	public bool IsReady { get; private set; }

	public bool IsFirstPerson
	{
		get
		{
			return mFirstPersonActor != null;
		}
	}

	public FirstPersonCamera FirstPersonCamera
	{
		get
		{
			FirstPersonCamera firstPersonCamera = CameraManager.Instance.PlayCameraController.CurrentCameraBase as FirstPersonCamera;
			return firstPersonCamera ?? ((!(mFirstPersonActor != null) || !(mFirstPersonActor.realCharacter != null)) ? null : mFirstPersonActor.realCharacter.FirstPersonCamera);
		}
	}

	public bool AimingDownScope
	{
		get
		{
			ViewModelRig viewModelRig = ViewModelRig.Instance();
			if (viewModelRig == null || viewModelRig.IsOverrideActive || viewModelRig.GetScopeTexture() == null)
			{
				return false;
			}
			if (mFirstPersonActor == null || mFirstPersonActor.weapon == null)
			{
				return false;
			}
			if (mFirstPersonActor.realCharacter == null || !mFirstPersonActor.realCharacter.IsAimingDownSights)
			{
				return false;
			}
			if (mFirstPersonActor.realCharacter.IsMortallyWounded() || mFirstPersonActor.realCharacter.IsDead())
			{
				return false;
			}
			IWeapon activeWeapon = mFirstPersonActor.weapon.ActiveWeapon;
			if (activeWeapon == null || !activeWeapon.HasScope())
			{
				return false;
			}
			IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(activeWeapon);
			return weaponADS != null && weaponADS.GetHipsToSightsBlendAmount() >= 1f;
		}
	}

	public bool FirstPersonFollowMe { get; set; }

	public bool AllowContextActionsInFirstPerson { get; set; }

	public Actor mFirstPersonActor
	{
		get
		{
			return mFPActor;
		}
		set
		{
			mFPActor = value;
			if (mFPActor != null)
			{
				GameplayController.instance.SelectOnlyThis(mFPActor);
			}
			if (this.OnFirstPersonActorChange != null)
			{
				this.OnFirstPersonActorChange();
			}
		}
	}

	public float TargetDist
	{
		get
		{
			return mTargetDist;
		}
		private set
		{
			mTargetDist = value;
		}
	}

	public Actor FirstPersonTarget
	{
		get
		{
			return mFirstPersonTarget;
		}
		private set
		{
			mTimeOnFirstPersonTarget = ((!(mFirstPersonTarget != null) || !(mFirstPersonTarget == value)) ? 0f : mTimeOnFirstPersonTarget);
			mFirstPersonTarget = value;
			if (mFirstPersonActor != null)
			{
				mFirstPersonActor.realCharacter.SetTarget(mFirstPersonTarget);
			}
		}
	}

	public bool FirstPersonTargetIsLivingEnemy
	{
		get
		{
			Actor actor = mFirstPersonActor;
			Actor firstPersonTarget = FirstPersonTarget;
			return actor != null && firstPersonTarget != null && !firstPersonTarget.baseCharacter.IsDead() && FactionHelper.AreEnemies(actor.awareness.faction, firstPersonTarget.awareness.faction);
		}
	}

	public CoverPointCore FirstPersonCoverPoint
	{
		get
		{
			return mFirstPersonCoverPoint;
		}
	}

	public Actor GetSquadLeader
	{
		get
		{
			if (mFirstPersonActor != null)
			{
				return mFirstPersonActor;
			}
			if (mPreviousFirstPersonActor != null)
			{
				return mPreviousFirstPersonActor;
			}
			return GameplayController.Instance().GetBestFirstPersonActor();
		}
	}

	public bool ZoomInAvailable
	{
		get
		{
			return mZoomInAvailable;
		}
		set
		{
			mZoomInAvailable = value;
		}
	}

	public bool ZoomOutAvailable
	{
		get
		{
			return mZoomOutAvailable;
		}
		set
		{
			mZoomOutAvailable = value;
		}
	}

	public bool MissionEnding
	{
		get
		{
			return mMissionEnding;
		}
	}

	public LoadoutMenuNavigator Loadout
	{
		get
		{
			return mLoadoutMenuCtrl;
		}
	}

	public bool PlacementModeActive
	{
		get
		{
			return mPlacementMode;
		}
	}

	public Vector2 LastLookInput { get; set; }

	public Vector2 LastVelocity { get; set; }

	public Vector2 LastViewRotation { get; set; }

	public bool IsPaused
	{
		get
		{
			return mPaused;
		}
	}

	public Actor PlayerUnitTakingDamage { get; set; }

	public bool GrenadeThrowingModeActive
	{
		get
		{
			return mGrenadeThrowingMode;
		}
	}

	public bool GMGReviveModeActive
	{
		get
		{
			return mGMGReviveMode;
		}
	}

	public bool ClaymoreDroppingModeActive
	{
		get
		{
			return mClaymoreDroppingMode;
		}
	}

	public bool SentryHackingModeActive
	{
		get
		{
			return mSentryHackingMode;
		}
	}

	public bool GameplayStarted
	{
		get
		{
			return mGameplayStarted;
		}
	}

	public event OnExitFirstPersonEventHandler OnExitFirstPerson;

	public event OnSwitchToPersonEventHandler OnSwitchToFirstPerson;

	public event OnFirstPersonActorChangeEventHandler OnFirstPersonActorChange;

	public void UpdateTransitionEffect()
	{
		float manualAlpha = Mathf.Clamp01(TransitionEffectAmount);
		AnimatedScreenBackground.Instance.SetManualAlpha(manualAlpha);
	}

	public void OnActivateSection()
	{
	}

	[Conditional("LOG_GAME")]
	public static void Log(string message)
	{
		UnityEngine.Debug.Log(message);
	}

	public OverwatchController GetOverwatchController()
	{
		return mOverwatchController;
	}

	public void OverrideTimePlayed(float timeInSeconds)
	{
		mTimePlayed = timeInSeconds;
	}

	private void Awake()
	{
		MovementSpeedMultiplier = 1f;
		Input.compensateSensors = true;
		mGyroController = base.gameObject.AddComponent<GyroController>();
		LockGyro = false;
		base.gameObject.AddComponent<InputDeviceController>();
		if (Screen.dpi > 0f)
		{
			if (Screen.width > Screen.height)
			{
				InputSettings.DeviceWidthScale = (float)Screen.width / Screen.dpi / 7.757576f;
				InputSettings.DeviceHeightScale = (float)Screen.height / Screen.dpi / 5.818182f;
			}
			else if (Screen.width < Screen.height)
			{
				InputSettings.DeviceWidthScale = (float)Screen.height / Screen.dpi / 7.757576f;
				InputSettings.DeviceHeightScale = (float)Screen.width / Screen.dpi / 5.818182f;
			}
		}
		AllowFirstPersonAtAnyPoint = true;
		AllowMovementInFirstPerson = true;
		InputSettings.FirstPersonFieldOfView = InputSettings.FirstPersonFieldOfViewStandard;
		FirstPersonFollowMe = false;
		FirstPersonPenaliser.EnablePenalty(true);
		if (false)
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = -1;
		}
		else
		{
			QualitySettings.vSyncCount = 1;
			if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.HighEnd))
			{
				Application.targetFrameRate = 60;
			}
			else
			{
				Application.targetFrameRate = 30;
			}
		}
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple GameControllers");
		}
		smInstance = this;
		mGameplayStarted = false;
		List<string> list = new List<string>();
		list.Add("hud_strategy");
		list.Add("LoadoutMenu");
		list.Add("MissionComplete");
		list.Add("hud_common");
		list.Add("hud_pausemenu");
		mCMTriggerLogic = base.gameObject.GetComponentInChildren<ContextMenuTriggerLogic>();
		StartCoroutine(SetReadyAfterLoad(list.ToArray()));
		mMissionEnding = false;
		mOverwatchController = OverwatchController.Instance;
		MissionListings.eMissionID mission = ActStructure.Instance.CurrentMissionID;
		int section = ActStructure.Instance.CurrentSection;
		if ((bool)MissionSetup.Instance)
		{
			mission = MissionSetup.Instance.GetMissionIDFromSetup();
			section = MissionSetup.Instance.GetMissionSectionFromSetup();
		}
		if (ActStructure.Instance != null && !ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			MissionSetup.LoadAndCopySectionSFX(mission, section);
		}
		TapToTargetBlend = 0.5f;
		mTimePlayed = 0f;
		mMinuteCount = 0f;
	}

	private IEnumerator SetReadyAfterLoad(IEnumerable<string> loads)
	{
		FirstPersonOnly.ClearList();
		BuildingWithInterior.ClearList();
		foreach (string load in loads)
		{
			AsyncOperation ao = Application.LoadLevelAdditiveAsync(load);
			while (!ao.isDone)
			{
				yield return ao;
			}
		}
		while (SceneLoader.IsTeleporting)
		{
			yield return null;
		}
		if (mCommonHud == null || mFirstPersonHud == null || mStrategyHud == null)
		{
			yield return null;
		}
		HudStateController.Instance.SetCommonHudCtrl(mCommonHud);
		HudStateController.Instance.SetFirstPeronHudCtrl(mFirstPersonHud);
		HudStateController.Instance.SetStrategyHudCtrl(mStrategyHud);
		HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
		IsReady = true;
		if ((bool)LoadingProgressMarker.Instance)
		{
			LoadingProgressMarker.Instance.ClearOff();
		}
		if ((bool)LoadingSection.Instance)
		{
			PlayerSquadManager.Instance.SetupInventory();
			CheckpointManager.Instance.SaveCheckPointData();
			LoadingSection.Instance.ClearOff();
		}
		else if (CheckpointManager.Instance.LoadFromSavePoint)
		{
			GameSettings.Instance.Load();
			PlayerSquadManager.Instance.SetupInventory();
		}
		if (MissionSetup.Instance != null)
		{
			CommonHudController.Instance.DisplayMissionName(MissionSetup.Instance.MissionTitle);
		}
		if (CheckpointManager.Instance == null || !CheckpointManager.Instance.LoadFromSavePoint)
		{
			bool bTutorial = ActStructure.Instance.CurrentMissionSectionIsTutorial();
			bool bDoBriefing = false;
			bool bDoLoadout = false;
			MissionSetup missionSetup = MissionSetup.Instance;
			if (!GameSettings.DisableLoadoutAndBriefing)
			{
				if (((missionSetup != null && missionSetup.PlayMissionBriefing) || GameSettings.LaunchedFromGlobe) && missionSetup != null && missionSetup.MissionBriefSequence != null)
				{
					bDoBriefing = true;
				}
				if (!bTutorial)
				{
					bDoLoadout = true;
				}
			}
			m_bDoBriefing = bDoBriefing;
			if (bTutorial)
			{
				GameSettings.Instance.SetupForTutorial();
			}
			SoundManager.Instance.SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 0f);
			if (bDoBriefing)
			{
				ScriptedSequence scriptedSequence = missionSetup.MissionBriefSequence.GetComponentInChildren<ScriptedSequence>();
				if (scriptedSequence != null)
				{
					CameraManager.Instance.EnableOverwatchShader();
					scriptedSequence.StartSequence();
				}
				GameSettings.Instance.UpdateGameSettingsWithSoldierPresence();
			}
			else if (bDoLoadout)
			{
				mLoadoutMenuCtrl.StartLoadout();
			}
			else
			{
				ZoomInAvailable = true;
				GameSettings.Instance.UpdateGameSettingsWithSoldierPresence();
				PlayerSquadManager.Instance.SetupInventory();
				PostLoadoutBeginGameplay();
			}
		}
		else
		{
			GameSettings.Instance.UpdateGameSettingsWithSoldierPresence();
			PostLoadoutBeginGameplay();
		}
		CheckpointManager.Instance.LoadFromSavePoint = false;
		if (ActStructure.Instance.CurrentMissionIsSpecOps() && GameSettings.LaunchedFromGlobe)
		{
			GameModeManager.Instance.ActivateRelevantGameMode();
		}
		Resources.UnloadUnusedAssets();
	}

	private void Start()
	{
		AllowContextActionsInFirstPerson = true;
		mZoomInAvailable = AllowFirstPersonAtAnyPoint;
		if (IsFirstPerson)
		{
			mZoomOutAvailable = true;
		}
		else
		{
			mZoomOutAvailable = false;
		}
		InputManager.Instance.SetForGamplay();
		FingerGestures.GlobalTouchFilter = delegate(int fingerIndex, Vector2 position)
		{
			if (DebugMenu.IsInDetectionZone(position))
			{
				return true;
			}
			bool flag = false;
			bool flag2 = ContextMenuBase.NumContextMenusActive > 0;
			bool flag3 = false;
			bool flag4 = UIManager.instance.DidAnyPointerHitUI();
			return (flag || flag2 || (!flag3 && !flag4)) ? true : false;
		};
	}

	private void OnDestroy()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FlushMusicStack();
		}
		if (SoundManager.Instance != null)
		{
			SoundManager.Instance.CleanUpOnLevelEnd();
		}
		FingerGestures.GlobalTouchFilter = null;
		RestoreScreenRotation();
		ThemedModelDescriptor.ClearThemeModelDescGameList();
		CleanUpData();
		smInstance = null;
	}

	private void CleanUpData()
	{
		AwarenessComponent.ClearCoverArrays();
		WeaponUtils.MuzzleList = null;
		HudBlipIcon.mCamManRef = null;
		ActorGenerator.NullInstance();
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		GameSettings instance = GameSettings.Instance;
		bool flag = instance != null;
		mAimimgDownScopeThisFrame = AimingDownScope;
		PeakHeapUsage = Math.Max(PeakHeapUsage, UnityEngine.Profiling.Profiler.usedHeapSize);
		if (mPaused || !(deltaTime > 0f))
		{
			return;
		}
		InteractionsManager instance2 = InteractionsManager.Instance;
		if (instance2 != null && !instance2.IsPlayingCutscene())
		{
			mMinuteCount += deltaTime;
			mTimePlayed += deltaTime;
			if (mMinuteCount > 60f)
			{
				mMinuteCount -= 60f;
				EventHub.Instance.Report(new Events.GameplayMinutePassed(mTimePlayed));
			}
		}
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.StartCamera as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				NavMeshCamera navMeshCamera = playCameraInterface as NavMeshCamera;
				if (navMeshCamera != null)
				{
					navMeshCamera.SuppressPanMove = false;
				}
			}
		}
		FirstPersonCamera firstPersonCamera = FirstPersonCamera;
		CommonHudController instance3 = CommonHudController.Instance;
		if (instance3 != null)
		{
			instance3.Look.gameObject.SetActive(firstPersonCamera != null);
			if (mAimimgDownScopeThisFrame)
			{
				instance3.SetHoldBreathUI(mFirstPersonActor.realCharacter.CanHoldBreath, mFirstPersonActor.realCharacter.HoldingBreathFraction);
			}
			else
			{
				if ((bool)mFirstPersonActor && (bool)mFirstPersonActor.realCharacter)
				{
					mFirstPersonActor.realCharacter.HandleForcedExhale();
				}
				instance3.SetHoldBreathUI(false, 0f);
			}
		}
		if (firstPersonCamera != null)
		{
			Player player = ((!flag) ? null : instance.PlayerGameSettings());
			float num = ((!instance3.HoldingView && !LockGyro) ? 1f : 0f);
			num *= ((!flag || !player.FirstPersonGyroscopeEnabled) ? 0f : 1f);
			float t = ((!flag) ? 0.5f : player.FirstPersonGyroscope);
			float num2 = Mathf.Lerp(InputSettings.FirstPersonGyroSensitivityHorizontalLow, InputSettings.FirstPersonGyroSensitivityHorizontalHigh, t);
			float num3 = 0f - Mathf.Lerp(InputSettings.FirstPersonGyroSensitivityVerticalLow, InputSettings.FirstPersonGyroSensitivityVerticalHigh, t);
			float num4 = Mathf.Lerp(InputSettings.FirstPersonTrackpadLookSensitivityHorizontalLow, InputSettings.FirstPersonTrackpadLookSensitivityHorizontalHigh, (!flag) ? 0.5f : player.FirstPersonXSensitivity);
			float num5 = Mathf.Lerp(InputSettings.FirstPersonTrackpadLookSensitivityVerticalLow, InputSettings.FirstPersonTrackpadLookSensitivityVerticalHigh, (!flag) ? 0.5f : player.FirstPersonYSensitivity);
			float num6 = Mathf.Lerp(InputSettings.FirstPersonGamepadLookSensitivityHorizontalLow, InputSettings.FirstPersonGamepadLookSensitivityHorizontalHigh, (!flag) ? 0.5f : player.FirstPersonGamepadSensitivity);
			float num7 = Mathf.Lerp(InputSettings.FirstPersonGamepadLookSensitivityVerticalLow, InputSettings.FirstPersonGamepadLookSensitivityVerticalHigh, (!flag) ? 0.5f : player.FirstPersonGamepadSensitivity);
			num2 *= ((!flag || !player.FirstPersonInvertGyroscopeX) ? 1f : (-1f));
			num3 *= ((!flag || !player.FirstPersonInvertGyroscopeY) ? 1f : (-1f));
			num4 *= ((!flag || !player.FirstPersonInvertTouchX) ? 1f : (-1f));
			num5 *= ((!flag || !player.FirstPersonInvertTouchY) ? 1f : (-1f));
			num6 *= ((!flag || !player.FirstPersonInvertGamepadX) ? 1f : (-1f));
			num7 *= ((!flag || !player.FirstPersonInvertGamepadY) ? 1f : (-1f));
			num5 *= InputSettings.DeviceHeightScale;
			num5 *= instance3.TouchSensitivityModifier;
			num4 *= InputSettings.DeviceWidthScale;
			num4 *= instance3.TouchSensitivityModifier;
			float num8 = 1f / FirstPersonGyroThreshold;
			float x = mGyroController.GetRotationRateUnbiased().x;
			float num9 = ((!(Mathf.Abs(x) > FirstPersonGyroThreshold)) ? (Mathf.Sign(x) * FirstPersonGyroThreshold * Mathf.Pow(x * num8, 2f)) : x);
			float num10 = num3 * num * num9;
			num10 -= CommonHudController.Instance.LookAmountTouch.y * num5;
			num10 -= CommonHudController.Instance.LookAmountGamepad.y * num7;
			float num11 = 0f - mGyroController.GetRotationRateUnbiased().y;
			float num12 = ((!(Mathf.Abs(num11) > FirstPersonGyroThreshold)) ? (Mathf.Sign(num11) * FirstPersonGyroThreshold * Mathf.Pow(num11 * num8, 2f)) : num11);
			float num13 = num2 * num * num12;
			num13 += CommonHudController.Instance.LookAmountTouch.x * num4;
			num13 += CommonHudController.Instance.LookAmountGamepad.x * num6;
			LastLookInput = new Vector2(num10, num13);
			float num14 = 4f;
			float a = ((!FirstPersonTargetIsLivingEnemy) ? 0f : Mathf.Clamp01(player.FirstPersonAimAssist));
			a = Mathf.Clamp(mLastAimAssistValue - TimeManager.DeltaTime * num14, Mathf.Max(a, 0f), 1f);
			float num15 = Mathf.Lerp(1f, InputSettings.FirstPersonAimAssistSensitivityModifierX, a);
			float num16 = Mathf.Lerp(1f, InputSettings.FirstPersonAimAssistSensitivityModifierY, a);
			float num17 = ((!(mFirstPersonActor != null)) ? 1f : mFirstPersonActor.realCharacter.GetSensitivityModifier());
			num17 *= ((!(mFirstPersonLockOnAngles.sqrMagnitude > 1f) && !(mFirstPersonPointOfInterest != null)) ? 1f : 0f);
			num17 *= ((!TutorialToggles.LockFPPLook && !instance2.IsPlayingCutscene() && TimeManager.instance.GlobalTimeState != TimeManager.State.IngamePaused) ? 1f : 0f);
			num10 *= num17 * num15;
			num13 *= num17 * num16;
			TurnFirstPersonCamera(new Vector3(num10, num13, 0f));
			LastViewRotation = new Vector2(num10, num13) + mLastLockOnUpdate.xy();
			mLastAimAssistValue = a;
			if (mFirstPersonActor != null && mFirstPersonActor.realCharacter != null)
			{
				bool flag2 = false;
				if (mFirstPersonActor.realCharacter.CanMove())
				{
					Vector2 moveAmount = Vector2.zero;
					if (AllowMovementInFirstPerson && !TutorialToggles.IsRespotting && !ViewModelRig.Instance().IsOverrideActive)
					{
						moveAmount = instance3.TouchSensitivityModifier * (instance3.MoveAmount * MovementSpeedMultiplier);
					}
					float num18 = Mathf.InverseLerp(InputSettings.FirstPersonSprintZoneStartCosine, InputSettings.FirstPersonSprintZoneEndCosine, Vector2.Dot(moveAmount.normalized, Vector2.up));
					flag2 = num18 < 1f;
					float num19 = 30f;
					Vector2 desiredFirstPersonVelocity = mFirstPersonActor.realCharacter.GetDesiredFirstPersonVelocity(moveAmount, num18);
					LastVelocity += Vector2.ClampMagnitude(desiredFirstPersonVelocity - LastVelocity, num19 * deltaTime);
					moveAmount = deltaTime * LastVelocity;
					Vector3 right = firstPersonCamera.transform.right;
					right = new Vector3(right.x, 0f, right.z).normalized;
					Vector3 forward = firstPersonCamera.transform.forward;
					forward = new Vector3(forward.x, 0f, forward.z).normalized;
					Vector3 offset = moveAmount.x * right + moveAmount.y * forward;
					Vector3 position = mFirstPersonActor.transform.position;
					mFirstPersonActor.navAgent.Move(offset);
					Vector3 position2 = mFirstPersonActor.transform.position;
					ViewModelRig.Instance().Velocity = (position2 - position) / deltaTime;
					mFirstPersonActor.navAgent.updatePosition = true;
					CalculateFirstPersonCover();
				}
				else
				{
					LastVelocity = Vector2.zero;
				}
				float num20 = ((!flag2 || !(LastVelocity.magnitude > InputSettings.FirstPersonFieldOfViewSprintThreshold)) ? (-5f) : 5f);
				mSprintBlend = Mathf.Clamp01(mSprintBlend + TimeManager.DeltaTime * num20);
				float to = Mathf.Lerp(InputSettings.FirstPersonFieldOfViewStandard, InputSettings.FirstPersonFieldOfViewSprint, mSprintBlend);
				InputSettings.FirstPersonFieldOfView = Mathf.Lerp(InputSettings.FirstPersonFieldOfView, to, 10f * deltaTime);
				float to2 = Mathf.Lerp(InputSettings.FirstPersonViewOffsetStandard, InputSettings.FirstPersonViewOffsetSprint, mSprintBlend);
				InputSettings.FirstPersonViewOffset = Mathf.Lerp(InputSettings.FirstPersonViewOffset, to2, 10f * deltaTime);
				if (Input.touchCount == 0 && Input.GetMouseButtonDown(1) && instance3.ADSButton.gameObject.activeInHierarchy)
				{
					mFirstPersonActor.realCharacter.IsAimingDownSights = !mFirstPersonActor.realCharacter.IsAimingDownSights;
					if (mFirstPersonActor.realCharacter.IsAimingDownSights)
					{
						Instance.ADSAutoLockOn();
					}
				}
				if (mAutoSnapTarget != null)
				{
					FirstPersonSnapToActor(mAutoSnapTarget);
					mAutoSnapTarget = null;
				}
			}
			PlayCameraInterface playCameraInterface2 = playCameraController.StartCamera as PlayCameraInterface;
			if (playCameraInterface2 != null)
			{
				NavMeshCamera navMeshCamera2 = playCameraInterface2 as NavMeshCamera;
				if (mFirstPersonActor != null)
				{
					playCameraInterface2.FocusOnTarget(mFirstPersonActor.transform, true);
					navMeshCamera2.IdealYaw = mFirstPersonActor.realCharacter.FirstPersonCamera.Angles.y;
				}
				navMeshCamera2.SuppressPanMove = true;
			}
		}
		if (!(OverwatchController.Instance != null))
		{
			return;
		}
		OverWatchCamera overwatchLC = OverwatchController.Instance.OverwatchLC;
		if (!(overwatchLC != null))
		{
			return;
		}
		StrategyHudController instance4 = StrategyHudController.Instance;
		if (instance4 != null)
		{
			float num21 = ((!(instance != null) || !instance.PlayerGameSettings().FirstPersonGyroscopeEnabled) ? 0f : 1f);
			if (instance4.HoldingView)
			{
				num21 = 0f;
			}
			float num22 = Mathf.Lerp(InputSettings.OverwatchGyroSensitivityHorizontalLow, InputSettings.OverwatchGyroSensitivityHorizontalHigh, (!(instance == null)) ? instance.PlayerGameSettings().FirstPersonGyroscope : 0.5f);
			float num23 = 0f - Mathf.Lerp(InputSettings.OverwatchGyroSensitivityVerticalLow, InputSettings.OverwatchGyroSensitivityVerticalHigh, (!(instance == null)) ? instance.PlayerGameSettings().FirstPersonGyroscope : 0.5f);
			float num24 = Mathf.Lerp(InputSettings.OverwatchTrackpadLookSensitivityHorizontalLow, InputSettings.OverwatchTrackpadLookSensitivityHorizontalHigh, (!(instance == null)) ? instance.PlayerGameSettings().FirstPersonXSensitivity : 0.5f);
			float num25 = Mathf.Lerp(InputSettings.OverwatchTrackpadLookSensitivityVerticalLow, InputSettings.OverwatchTrackpadLookSensitivityVerticalHigh, (!(instance == null)) ? instance.PlayerGameSettings().FirstPersonYSensitivity : 0.5f);
			Player player2 = ((!flag) ? null : instance.PlayerGameSettings());
			float num26 = Mathf.Lerp(InputSettings.OverwatchGamepadLookSensitivityHorizontalLow, InputSettings.OverwatchGamepadLookSensitivityHorizontalHigh, (!flag) ? 0.5f : player2.FirstPersonGamepadSensitivity);
			float num27 = Mathf.Lerp(InputSettings.OverwatchGamepadLookSensitivityVerticalLow, InputSettings.OverwatchGamepadLookSensitivityVerticalHigh, (!flag) ? 0.5f : player2.FirstPersonGamepadSensitivity);
			float num28 = 1f / FirstPersonGyroThreshold;
			float num29 = ((!instance.PlayerGameSettings().FirstPersonInvertGyroscopeY) ? mGyroController.GetRotationRateUnbiased().x : (0f - mGyroController.GetRotationRateUnbiased().x));
			float num30 = ((!(Mathf.Abs(num29) > FirstPersonGyroThreshold)) ? (Mathf.Sign(num29) * FirstPersonGyroThreshold * Mathf.Pow(num29 * num28, 2f)) : num29);
			float num31 = num23 * num30 * num21;
			num31 -= instance4.LookAmount.y * num25;
			num31 -= instance4.LookAmountPad.y * num27;
			float num32 = ((!instance.PlayerGameSettings().FirstPersonInvertGyroscopeX) ? (0f - mGyroController.GetRotationRateUnbiased().y) : mGyroController.GetRotationRateUnbiased().y);
			float num33 = ((!(Mathf.Abs(num32) > FirstPersonGyroThreshold)) ? (Mathf.Sign(num32) * FirstPersonGyroThreshold * Mathf.Pow(num32 * num28, 2f)) : num32);
			float num34 = num22 * num33 * num21;
			num34 += instance4.LookAmount.x * num24;
			num34 += instance4.LookAmountPad.x * num26;
			Vector2 scale = new Vector2(num24 * ((!instance.PlayerGameSettings().FirstPersonInvertTouchX) ? (-1f) : 1f), num25 * ((!instance.PlayerGameSettings().FirstPersonInvertTouchY) ? (-1f) : 1f));
			if (instance2.IsPlayingCutscene())
			{
				scale *= 0f;
			}
			Vector2 delta = new Vector2(num34, 0f - num31);
			delta.Scale(scale);
			overwatchLC.DoPan(delta);
		}
	}

	private void LateUpdate()
	{
		mTimeSinceFirstPersonTransition += TimeManager.DeltaTime;
		mTimeSinceFirstPersonTransitionHUD += TimeManager.DeltaTime;
		mTimeSinceLock += TimeManager.DeltaTime;
		mTimeOnFirstPersonTarget += TimeManager.DeltaTime;
		if (!IsPaused)
		{
			FirstPersonTargetUpdate();
			SoftLockUpdate();
			FirstPersonLockOnUpdate();
		}
		bool zoomIn = false;
		bool zoomOut = false;
		if (!mDisableZoomButtonsDuringCoroutineTransition)
		{
			BloodEffect bloodEffect = CameraManager.Instance.BloodEffect;
			if (bloodEffect != null)
			{
				if (mFirstPersonActor != null)
				{
					float num2;
					if (IsInvulnerabilityEffectActive())
					{
						bloodEffect.SetAsInvulnerabilityEffect();
						float num = Mathf.Sin(Time.time * 3f) * 0.33f;
						num2 = EffectsController.Instance.Effects.BloodyScreenRamp.Evaluate(0.5f + num);
					}
					else
					{
						bloodEffect.SetAsBloodEffect();
						num2 = EffectsController.Instance.Effects.BloodyScreenRamp.Evaluate(1f - mFirstPersonActor.health.Health01);
					}
					bloodEffect.enabled = true;
					bloodEffect.Alpha = num2;
					bloodEffect.Severity = num2;
				}
				else
				{
					bloodEffect.enabled = false;
				}
			}
			if (mFirstPersonActor != null)
			{
				mFirstPersonActor.awareness.LookDirection = mFirstPersonActor.realCharacter.FirstPersonCamera.transform.forward;
				if (mAimimgDownScopeThisFrame)
				{
					float swayAmount = mFirstPersonActor.realCharacter.SwayAmount;
					float swayFrequency = mFirstPersonActor.realCharacter.SwayFrequency;
					mSwayTimer += swayFrequency * Time.deltaTime;
					Vector2 vector = swayAmount * Noise.Smooth(mSwayTimer).xy();
					mFirstPersonActor.realCharacter.FirstPersonCamera.SwayAngles = new Vector3(vector.x, vector.y, 0f);
				}
				zoomIn = false;
				zoomOut = !IsLockedToFirstPerson;
			}
			else
			{
				zoomIn = mZoomInAvailable;
				zoomOut = mZoomOutAvailable;
			}
		}
		CommonHudController instance = CommonHudController.Instance;
		if (instance != null)
		{
			instance.SetZoomButtons(zoomIn, zoomOut);
		}
		UpdateTransitionEffect();
	}

	private bool IsInvulnerabilityEffectActive()
	{
		return mFirstPersonActor != null && mFirstPersonActor.health.Invulnerable && ActStructure.Instance.CurrentMissionIsSpecOps() && mFirstPersonActor.MysteryBoxInvulnerability;
	}

	public void TogglePause()
	{
		mPaused = !mPaused;
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.StartCamera as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				NavMeshCamera navMeshCamera = playCameraInterface as NavMeshCamera;
				if (navMeshCamera != null)
				{
					navMeshCamera.AllowInput(!mPaused);
				}
			}
		}
		if (mPaused)
		{
			SuppressHud(true);
			GameplayController.Instance().DisableInput();
			FrontEndController.Instance.TransitionTo(ScreenID.Pause);
			RestoreScreenRotation();
			return;
		}
		SuppressHud(false);
		GameplayController.Instance().EnableInput();
		FrontEndController.Instance.TransitionTo(ScreenID.None);
		if (IsFirstPerson)
		{
			LockScreenRotationIfGyroEnabled();
		}
	}

	private void SoftLockUpdate()
	{
		float distanceThreshold = 0.05f;
		if (FirstPersonTargetIsLivingEnemy)
		{
			mSoftLockTarget = mFirstPersonTarget;
			distanceThreshold = float.MaxValue;
		}
		if (mSoftLockTarget != null)
		{
			if (mSoftLockTarget.realCharacter != null && !mSoftLockTarget.realCharacter.IsDead() && !mSoftLockTarget.realCharacter.IsMortallyWounded())
			{
				SoftLockUpdate(mSoftLockTarget.ident, distanceThreshold);
			}
			else
			{
				mSoftLockTarget = null;
			}
		}
		if (mSoftLockTarget == null && mFirstPersonActor != null)
		{
			SoftLockUpdate(mFirstPersonActor.awareness.EnemiesICanSee & GKM.AliveMask & ~GKM.CharacterTypeMask(CharacterType.SentryGun), 0.05f);
		}
	}

	private void SoftLockUpdate(uint candidates, float distanceThreshold)
	{
		IWeaponEquip weaponEquip = null;
		if (mFirstPersonActor != null)
		{
			weaponEquip = WeaponUtils.GetWeaponEquip(mFirstPersonActor.weapon.ActiveWeapon);
		}
		if (mFirstPersonActor == null || mFirstPersonActor.realCharacter.IsAimingDownSights || mAimimgDownScopeThisFrame || SoftLockDisabled || (weaponEquip != null && weaponEquip.HasNoWeapon()))
		{
			mSoftLockTarget = null;
			mFirstPersonSoftLockAngles = Vector3.zero;
			CommonHudController.Instance.ClearSoftLock();
			ViewModelRig.Instance().ClearSoftLock();
			return;
		}
		Vector3 position = Vector3.zero;
		Vector3 vector = Vector3.zero;
		float num = float.MaxValue;
		float num2 = distanceThreshold * (float)Screen.height;
		float num3 = num2 * num2;
		float num4 = 0f * (float)Screen.height;
		float num5 = num4 * num4;
		Camera playCamera = CameraManager.Instance.PlayCamera;
		Vector3 vector2 = GUISystem.Instance.m_guiCamera.WorldToScreenPoint(Vector3.zero).xy();
		Actor actor = null;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(candidates);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			Vector3 softLockPosition = a.realCharacter.GetSoftLockPosition();
			Vector3 v = playCamera.WorldToScreenPoint(softLockPosition);
			if (!(v.z > 0f))
			{
				continue;
			}
			v = v.xy();
			float sqrMagnitude = (v - vector2).sqrMagnitude;
			if (!(sqrMagnitude < num))
			{
				continue;
			}
			SurfaceImpact surfaceImpact = ProjectileManager.Trace(playCamera.transform.position, softLockPosition, ProjectileManager.ProjectileMask);
			if (surfaceImpact != null && surfaceImpact.gameobject != null)
			{
				HitLocation component = surfaceImpact.gameobject.GetComponent<HitLocation>();
				if (component != null && component.Actor == a)
				{
					actor = a;
					num = sqrMagnitude;
					position = v;
					vector = softLockPosition;
				}
			}
		}
		if (num5 < num && num < num3)
		{
			Vector3 softLockPosition2 = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(position);
			CommonHudController.Instance.SetSoftLockPosition(softLockPosition2);
			ViewModelRig.Instance().SetSoftLockPosition(vector);
			mFirstPersonSoftLockAngles = GetAnglesForFirstPersonLookAt(vector);
		}
		else
		{
			mFirstPersonSoftLockAngles = Vector3.zero;
			CommonHudController.Instance.ClearSoftLock();
			ViewModelRig.Instance().ClearSoftLock();
			actor = null;
		}
		mSoftLockTarget = actor;
	}

	public void PostLoadoutBeginGameplay()
	{
		if (mGameplayStarted)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("BriefingSequence");
		if (gameObject != null)
		{
			UnityEngine.Object.DestroyImmediate(gameObject);
		}
		if (MissionSetup.Instance != null && MissionSetup.Instance.BGMusicTrack != null && MissionSetup.Instance.BGMusicTrack != string.Empty)
		{
			if (!m_bDoBriefing)
			{
				MusicManager.Instance.PlayAmbientMusic(MissionSetup.Instance.BGMusicTrack);
			}
		}
		else
		{
			MusicManager.Instance.PlayAmbientRainMusic();
		}
		LoadWeaponData();
		mLoadoutMenuCtrl.DeleteFromGameController();
		if (UnityEngine.Object.FindObjectsOfType(typeof(CameraTarget)) == null)
		{
			CameraController playCameraController = CameraManager.Instance.PlayCameraController;
			if (playCameraController != null)
			{
				PlayCameraInterface playCameraInterface = playCameraController.StartCamera as PlayCameraInterface;
				if (playCameraInterface != null)
				{
					NavMeshCamera navMeshCamera = playCameraInterface as NavMeshCamera;
					if (navMeshCamera != null)
					{
						ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
						Actor actor = actorIdentIterator.NextActor();
						if (actor != null)
						{
							navMeshCamera.FocusOnPoint(actor.transform.position, actor.transform.rotation.eulerAngles.y);
						}
					}
				}
			}
		}
		StartCoroutine(TransitionToStart());
		SoundManager.Instance.SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 1f);
	}

	private IEnumerator TransitionToStart()
	{
		yield return null;
		yield return Resources.UnloadUnusedAssets();
		if (!mGameplayStarted)
		{
			SwitchToPlayCamera();
			if (BeginGameplay())
			{
				InteractionsManager.Instance.TransitionFromSolid(0.75f);
			}
		}
	}

	public void LoadWeaponData()
	{
		ViewModelData viewModelData = ScriptableObject.CreateInstance<ViewModelData>();
		SoldierSettings[] soldiers = GameSettings.Instance.Soldiers;
		foreach (SoldierSettings soldierSettings in soldiers)
		{
			if (soldierSettings.Present)
			{
				PatchInViewModelData(viewModelData, soldierSettings.Weapon.Descriptor.Name);
			}
		}
		MissionSetup instance = MissionSetup.Instance;
		if (instance != null)
		{
			WeaponDescriptor[] scriptedWeapons = instance.ScriptedWeapons;
			foreach (WeaponDescriptor weaponDescriptor in scriptedWeapons)
			{
				PatchInViewModelData(viewModelData, weaponDescriptor.Name);
			}
		}
		PatchInViewModelData(viewModelData, "Ammo");
		PatchInViewModelData(viewModelData, "Beretta 23R");
		PatchInViewModelData(viewModelData, "C4");
		PatchInViewModelData(viewModelData, "Claymore");
		PatchInViewModelData(viewModelData, "Frag Grenade");
		PatchInViewModelData(viewModelData, "Knife");
		ViewModelData viewModelData2 = Resources.Load("ViewModelData/ViewModelData_Berreta") as ViewModelData;
		viewModelData.Arms = viewModelData2.Arms;
		viewModelData.ThemedMaterials = viewModelData2.ThemedMaterials;
		ViewModelRig.Instance().CreateRig(viewModelData);
	}

	private void PatchInViewModelData(ViewModelData vmd, string dataId)
	{
		if (dataId == "Ammo" && vmd.Ammo == null)
		{
			ViewModelData viewModelData = Resources.Load("ViewModelData/ViewModelData_Ammo") as ViewModelData;
			vmd.Ammo = viewModelData.Ammo;
			vmd.AmmoCoreAnims = viewModelData.AmmoCoreAnims;
		}
		if (dataId == "AP-96" && vmd.AN94 == null)
		{
			ViewModelData viewModelData2 = Resources.Load("ViewModelData/ViewModelData_AN94") as ViewModelData;
			vmd.AN94 = viewModelData2.AN94;
			vmd.AN94CoreAnims = viewModelData2.AN94CoreAnims;
			vmd.AN94SpecificAnims = viewModelData2.AN94SpecificAnims;
		}
		else if (dataId == "M8A1" && vmd.M8A1 == null)
		{
			ViewModelData viewModelData3 = Resources.Load("ViewModelData/ViewModelData_M8A1") as ViewModelData;
			vmd.M8A1 = viewModelData3.M8A1;
			vmd.M8A1CoreAnims = viewModelData3.M8A1CoreAnims;
			vmd.M8A1SpecificAnims = viewModelData3.M8A1SpecificAnims;
		}
		else if (dataId == "Type25" && vmd.Type25 == null)
		{
			ViewModelData viewModelData4 = Resources.Load("ViewModelData/ViewModelData_Type25") as ViewModelData;
			vmd.Type25 = viewModelData4.Type25;
			vmd.Type25CoreAnims = viewModelData4.Type25CoreAnims;
			vmd.Type25SpecificAnims = viewModelData4.Type25SpecificAnims;
		}
		else if (dataId == "HAMR" && vmd.HAMR == null)
		{
			ViewModelData viewModelData5 = Resources.Load("ViewModelData/ViewModelData_HAMR") as ViewModelData;
			vmd.HAMR = viewModelData5.HAMR;
			vmd.HAMRCoreAnims = viewModelData5.HAMRCoreAnims;
			vmd.HAMRSpecificAnims = viewModelData5.HAMRSpecificAnims;
		}
		else if (dataId == "LSAT" && vmd.LSAT == null)
		{
			ViewModelData viewModelData6 = Resources.Load("ViewModelData/ViewModelData_LSAT") as ViewModelData;
			vmd.LSAT = viewModelData6.LSAT;
			vmd.LSATCoreAnims = viewModelData6.LSATCoreAnims;
			vmd.LSATSpecificAnims = viewModelData6.LSATSpecificAnims;
		}
		else if (dataId == "QBBLSW" && vmd.QBBLSW == null)
		{
			ViewModelData viewModelData7 = Resources.Load("ViewModelData/ViewModelData_QBBLSW") as ViewModelData;
			vmd.QBBLSW = viewModelData7.QBBLSW;
			vmd.QBBLSWCoreAnims = viewModelData7.QBBLSWCoreAnims;
			vmd.QBBLSWSpecificAnims = viewModelData7.QBBLSWSpecificAnims;
		}
		else if (dataId == "SS-23K" && vmd.KS23 == null)
		{
			ViewModelData viewModelData8 = Resources.Load("ViewModelData/ViewModelData_KS23") as ViewModelData;
			vmd.KS23 = viewModelData8.KS23;
			vmd.KS23CoreAnims = viewModelData8.KS23CoreAnims;
			vmd.KS23SpecificAnims = viewModelData8.KS23SpecificAnims;
		}
		else if (dataId == "KSG" && vmd.KSG == null)
		{
			ViewModelData viewModelData9 = Resources.Load("ViewModelData/ViewModelData_KSG") as ViewModelData;
			vmd.KSG = viewModelData9.KSG;
			vmd.KSGCoreAnims = viewModelData9.KSGCoreAnims;
			vmd.KSGSpecificAnims = viewModelData9.KSGSpecificAnims;
		}
		else if (dataId == "M1216" && vmd.M1216 == null)
		{
			ViewModelData viewModelData10 = Resources.Load("ViewModelData/ViewModelData_M1216") as ViewModelData;
			vmd.M1216 = viewModelData10.M1216;
			vmd.M1216CoreAnims = viewModelData10.M1216CoreAnims;
			vmd.M1216SpecificAnims = viewModelData10.M1216SpecificAnims;
		}
		else if (dataId == "XPR-50" && vmd.XPR50 == null)
		{
			ViewModelData viewModelData11 = Resources.Load("ViewModelData/ViewModelData_XPR50") as ViewModelData;
			vmd.XPR50 = viewModelData11.XPR50;
			vmd.XPR50CoreAnims = viewModelData11.XPR50CoreAnims;
			vmd.XPR50SpecificAnims = viewModelData11.XPR50SpecificAnims;
			vmd.XPR50Scope = viewModelData11.XPR50Scope;
		}
		else if (dataId == "Ballista" && vmd.Ballista == null)
		{
			ViewModelData viewModelData12 = Resources.Load("ViewModelData/ViewModelData_Ballista") as ViewModelData;
			vmd.Ballista = viewModelData12.Ballista;
			vmd.BallistaCoreAnims = viewModelData12.BallistaCoreAnims;
			vmd.BallistaSpecificAnims = viewModelData12.BallistaSpecificAnims;
			vmd.BallistaScope = viewModelData12.BallistaScope;
		}
		else if (dataId == "SVU-AS" && vmd.SVUAS == null)
		{
			ViewModelData viewModelData13 = Resources.Load("ViewModelData/ViewModelData_SVU-AS") as ViewModelData;
			vmd.SVUAS = viewModelData13.SVUAS;
			vmd.SVUASCoreAnims = viewModelData13.SVUASCoreAnims;
			vmd.SVUASSpecificAnims = viewModelData13.SVUASSpecificAnims;
			vmd.SVUASScope = viewModelData13.SVUASScope;
		}
		else if (dataId == "Vektor K10" && vmd.Vektor == null)
		{
			ViewModelData viewModelData14 = Resources.Load("ViewModelData/ViewModelData_Vektor") as ViewModelData;
			vmd.Vektor = viewModelData14.Vektor;
			vmd.VektorCoreAnims = viewModelData14.VektorCoreAnims;
			vmd.VektorSpecificAnims = viewModelData14.VektorSpecificAnims;
		}
		else if (dataId == "Skorpion EVO" && vmd.Skorpion == null)
		{
			ViewModelData viewModelData15 = Resources.Load("ViewModelData/ViewModelData_Skorpion") as ViewModelData;
			vmd.Skorpion = viewModelData15.Skorpion;
			vmd.SkorpionCoreAnims = viewModelData15.SkorpionCoreAnims;
			vmd.SkorpionSpecificAnims = viewModelData15.SkorpionSpecificAnims;
		}
		else if (dataId == "PDW-57" && vmd.PDW == null)
		{
			ViewModelData viewModelData16 = Resources.Load("ViewModelData/ViewModelData_PDW") as ViewModelData;
			vmd.PDW = viewModelData16.PDW;
			vmd.PDWCoreAnims = viewModelData16.PDWCoreAnims;
			vmd.PDWSpecificAnims = viewModelData16.PDWSpecificAnims;
		}
		else if (dataId == "Beretta 23R" && vmd.Beretta == null)
		{
			ViewModelData viewModelData17 = Resources.Load("ViewModelData/ViewModelData_Berreta") as ViewModelData;
			vmd.Beretta = viewModelData17.Beretta;
			vmd.BerettaCoreAnims = viewModelData17.BerettaCoreAnims;
			vmd.BerettaSpecificAnims = viewModelData17.BerettaSpecificAnims;
		}
		else if (dataId == "Frag Grenade" && vmd.FragGrenade == null)
		{
			ViewModelData viewModelData18 = Resources.Load("ViewModelData/ViewModelData_FragGrenade") as ViewModelData;
			vmd.FragGrenade = viewModelData18.FragGrenade;
			vmd.FragGrenadeCoreAnims = viewModelData18.FragGrenadeCoreAnims;
			vmd.FragGrenadeSpecificAnims = viewModelData18.FragGrenadeSpecificAnims;
		}
		else if (dataId == "Knife" && vmd.Knife == null)
		{
			ViewModelData viewModelData19 = Resources.Load("ViewModelData/ViewModelData_Knife") as ViewModelData;
			vmd.Knife = viewModelData19.Knife;
			vmd.KnifeCoreAnims = viewModelData19.KnifeCoreAnims;
			vmd.KnifeSpecificAnims = viewModelData19.KnifeSpecificAnims;
		}
		else if (dataId == "C4" && vmd.C4 == null)
		{
			ViewModelData viewModelData20 = Resources.Load("ViewModelData/ViewModelData_C4") as ViewModelData;
			vmd.C4 = viewModelData20.C4;
			vmd.C4Remote = viewModelData20.C4Remote;
			vmd.C4CoreAnims = viewModelData20.C4CoreAnims;
		}
		else if (dataId == "Claymore" && vmd.Claymore == null)
		{
			ViewModelData viewModelData21 = Resources.Load("ViewModelData/ViewModelData_Claymore") as ViewModelData;
			vmd.Claymore = viewModelData21.Claymore;
			vmd.ClaymoreCoreAnims = viewModelData21.ClaymoreCoreAnims;
		}
		else
		{
			UnityEngine.Debug.Log(string.Format("Could not patch in view model data for {0}. Potential duplication between Script and Loadout?", dataId));
		}
	}

	public void SwitchToStrategyCamera(CameraBase logicalCameraTo)
	{
		if (GrenadeThrowingModeActive)
		{
			EndGrenadeThrowingMode();
		}
		if (mPlacementMode)
		{
			EndPlacementMode();
		}
		GameplayController.Instance().CancelAnyPlacement();
		if (!mSuppressHud && (!(OverwatchController.Instance != null) || !OverwatchController.Instance.Active))
		{
			HudStateController.Instance.SetState(HudStateController.HudState.Strategy);
		}
		SoundManager.Instance.ActivateOverwatchSFX();
		MusicManager.Instance.PlayHighDramaThemeMusic();
		CameraManager instance = CameraManager.Instance;
		if (instance != null)
		{
			instance.SwitchToCameraType(CameraManager.ActiveCameraType.StrategyCamera, logicalCameraTo);
			NavMeshCamera navMeshCamera = instance.PlayCameraController.StartCamera as NavMeshCamera;
			if (navMeshCamera != null)
			{
				StrategyViewCamera strategyViewCamera = instance.StrategyCameraController.StartCamera as StrategyViewCamera;
				if (strategyViewCamera != null)
				{
					Vector3 collision;
					Vector3 position = (WorldHelper.IsClearTrace(navMeshCamera.transform.position, navMeshCamera.transform.position + navMeshCamera.transform.forward * 1000f, out collision) ? (navMeshCamera.transform.position + navMeshCamera.transform.forward * 40f) : collision);
					strategyViewCamera.PanOffset = Vector3.zero;
					strategyViewCamera.target.position = position;
				}
			}
		}
		DropZoneController.Instance().ActivateDropZoneAreas(true);
		ZoomOutAvailable = false;
	}

	public void SwitchToPlayCamera()
	{
		SwitchToPlayCamera(false);
	}

	public void SwitchToPlayCamera(bool restoreDefault)
	{
		FirstPersonOnly.ShowHideFPPObjects(false);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null && restoreDefault)
		{
			playCameraController.RestoreCameraToDefault(1f);
			Instance.ExitFirstPerson();
			NavMeshCamera navMeshCamera = playCameraController.CurrentCameraBase as NavMeshCamera;
			if (navMeshCamera != null)
			{
				navMeshCamera.ClearFocusingOnPoint();
			}
		}
		if (!mSuppressHud)
		{
			if (mFirstPersonActor != null)
			{
				HudStateController.Instance.SetState(HudStateController.HudState.FPP);
			}
			else
			{
				HudStateController.Instance.SetState(HudStateController.HudState.TPP);
			}
		}
		playCameraController.StartCamera.AllowInput(true);
		CameraManager instance = CameraManager.Instance;
		if (instance != null)
		{
			if (!instance.IsSwitching)
			{
				instance.SwitchToCameraType(CameraManager.ActiveCameraType.PlayCamera, null);
				CommonHudController.Instance.ZoomButtonIcon.SetFrame(0, 0);
			}
			NavMeshCamera navMeshCamera2 = instance.PlayCameraController.StartCamera as NavMeshCamera;
			if (navMeshCamera2 != null)
			{
				navMeshCamera2.ClearFocusingOnPoint();
			}
		}
		if (!mSuppressHud)
		{
			mCMTriggerLogic.AllowInput(true);
		}
		DropZoneController.Instance().ActivateDropZoneAreas(false);
	}

	private void SetFPPHud()
	{
		if (IsLockedToFirstPerson && mFirstPersonActor.realCharacter.IsUsingFixedGun)
		{
			HudStateController.Instance.SetState(HudStateController.HudState.FPPLockedMountedWeapon);
		}
		else
		{
			HudStateController.Instance.SetState(HudStateController.HudState.FPP);
		}
	}

	public void SwitchToLastFirstPerson(bool DoTransition)
	{
		if (mPreviousFirstPersonActor != null)
		{
			SwitchToFirstPerson(mPreviousFirstPersonActor, DoTransition);
		}
	}

	public IEnumerator SwitchToFirstPerson(Actor actor, float delay)
	{
		yield return new WaitForSeconds(delay);
		SwitchToFirstPerson(actor, true);
	}

	public void SwitchToFirstPerson(Actor actor, bool DoTransition)
	{
		if (actor.behaviour == null || !actor.behaviour.PlayerControlled || actor.tasks.IsRunningTask<TaskEnter>())
		{
			return;
		}
		GameplayController.Instance().CancelDragging();
		bool isFirstPerson = IsFirstPerson;
		bool flag = mFirstPersonActor != actor;
		FirstPersonOnly.ShowHideFPPObjects(true);
		LockScreenRotationIfGyroEnabled();
		TimeManager.instance.ResumeNormalTime();
		CommonHudController.Instance.ClearContextMenu();
		WaypointMarkerManager.Instance.RemoveMarker(actor.gameObject);
		QueuedOrder.DestroyOrders(actor);
		ClearCharacterFirstPerson();
		mFirstPersonActor = actor;
		mFirstPersonActor.realCharacter.SetupForFirstPerson();
		if (flag)
		{
			Vector3 angles = mFirstPersonActor.poseModuleSharedData.GetModule(mFirstPersonActor.poseModuleSharedData.ActiveModule).IdealFirstPersonAngles();
			mFirstPersonActor.realCharacter.FirstPersonCamera.Angles = angles;
		}
		if (!mFirstPersonActor.realCharacter.IsUsingFixedGun)
		{
			mFirstPersonActor.Command("MoveToBegin");
			Task runningTask = mFirstPersonActor.tasks.GetRunningTask();
			if (runningTask != null)
			{
				Type type = runningTask.GetType();
				if (type == typeof(TaskMultiCharacterSetPiece))
				{
					Type[] taskTypes = new Type[4]
					{
						typeof(TaskRoutine),
						typeof(TaskSetPiece),
						typeof(TaskMultiCharacterSetPiece),
						typeof(TaskCacheStanceState)
					};
					mFirstPersonActor.tasks.CancelTasksExcluding(taskTypes);
				}
				else
				{
					Type[] taskTypes2 = new Type[4]
					{
						typeof(TaskRoutine),
						typeof(TaskSetPiece),
						typeof(TaskBreach),
						typeof(TaskCacheStanceState)
					};
					mFirstPersonActor.tasks.CancelTasksExcluding(taskTypes2);
				}
			}
		}
		StartCoroutine(AddTaskFirstPersonWhenFinished());
		mFirstPersonActor.realCharacter.IsFirstPerson = true;
		CameraBase firstPersonCamera = actor.realCharacter.FirstPersonCamera;
		if (firstPersonCamera != null)
		{
			CameraController playCameraController = CameraManager.Instance.PlayCameraController;
			playCameraController.ForcedCutTo(firstPersonCamera);
			if (DoTransition)
			{
				if (!OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.Bloom))
				{
					mTimeSinceFirstPersonTransition = 0f;
				}
				mTimeSinceFirstPersonTransitionHUD = 0f;
			}
		}
		SetFPPHud();
		CommonHudController.Instance.SwitchToFirstPerson(actor);
		if (AllowContextActionsInFirstPerson)
		{
			mCMTriggerLogic.AllowInput(true);
			InputManager.Instance.SetForGamplay();
		}
		else
		{
			mCMTriggerLogic.AllowInput(false);
			InputManager.Instance.SetForCutscene();
		}
		if (!isFirstPerson)
		{
			RealCharacter.SetUpdateFPPHudText();
			if (this.OnSwitchToFirstPerson != null)
			{
				this.OnSwitchToFirstPerson();
			}
		}
		WaypointMarkerManager.Instance.DisableRendering();
		PlayerUnitTakingDamage = null;
		TaskCacheStanceState taskCacheStanceState = mFirstPersonActor.tasks.GetRunningTask(typeof(TaskCacheStanceState)) as TaskCacheStanceState;
		if (taskCacheStanceState != null)
		{
			taskCacheStanceState.IgnoreFPPCacheCheck();
		}
	}

	private IEnumerator AddTaskFirstPersonWhenFinished()
	{
		Task runningTask = mFirstPersonActor.tasks.GetRunningTask();
		while ((mFirstPersonActor != null && !mFirstPersonActor.baseCharacter.IsDead() && runningTask != null && (runningTask.GetType() == typeof(TaskSetPiece) || runningTask.GetType() == typeof(TaskMultiCharacterSetPiece) || runningTask.GetType() == typeof(TaskBreach))) || InteractionsManager.Instance.IsPlayingCutscene())
		{
			runningTask = mFirstPersonActor.tasks.GetRunningTask();
			yield return null;
		}
		if (mFirstPersonActor != null)
		{
			new TaskFirstPerson(mFirstPersonActor.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType);
			mFirstPersonActor.firstThirdPersonWidget.FinishEarly();
		}
	}

	public void SwitchToOverwatch()
	{
		if (IsFirstPerson)
		{
			ExitFirstPerson();
		}
		if (!CameraManager.Instance.IsSwitching)
		{
			SwitchToStrategyCamera(mOverwatchController.OverwatchLC);
		}
		ZoomInAvailable = false;
		ZoomOutAvailable = false;
		HudStateController.Instance.SetState(HudStateController.HudState.Overwatch);
		CameraManager instance = CameraManager.Instance;
		if (instance != null)
		{
			instance.AllowInput(false);
			CameraTransitionData ctd = new CameraTransitionData(mOverwatchController.OverwatchLC, TweenFunctions.TweenType.easeInOutCubic, 1f);
			instance.StrategyCameraController.BlendTo(ctd);
		}
	}

	public void HideHUDElements()
	{
		HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
		HudBlipIcon.SwitchOffForCutscene();
		InputManager.Instance.SetForCutscene();
		WaypointMarkerManager.Instance.DisableRendering();
	}

	public void ReleaseFirstPersonGrenade(float cookedTime, Actor owner)
	{
		owner.realCharacter.ThrowGrenadeFirstPerson(cookedTime);
	}

	public void SetFirstPersonPointOfInterest(Transform pointOfInterest)
	{
		mFirstPersonPointOfInterest = pointOfInterest;
	}

	public void ClearFirstPersonPointOfInterest()
	{
		mFirstPersonPointOfInterest = null;
	}

	public void FirstPersonLockOn(SnapType snapType)
	{
		if (mFirstPersonActor == null || mFirstPersonActor.realCharacter == null)
		{
			return;
		}
		float num = 1000f;
		Vector3 lineOrigin = mFirstPersonActor.realCharacter.FirstPersonCamera.transform.position;
		Vector3 lineEnd = lineOrigin + num * mFirstPersonActor.realCharacter.FirstPersonCamera.transform.forward;
		Plane sidePlane = new Plane(mFirstPersonActor.realCharacter.FirstPersonCamera.transform.right, lineOrigin);
		Comparison<SnapTarget> comparison = delegate(SnapTarget a, SnapTarget b)
		{
			if (a == b)
			{
				return 0;
			}
			if (snapType != SnapType.SnapShort)
			{
				if (mSoftLockTarget != null)
				{
					if (a.gameObject == mSoftLockTarget.gameObject)
					{
						return 1;
					}
					if (b.gameObject == mSoftLockTarget.gameObject)
					{
						return -1;
					}
				}
				if (a == mLastLockOnTarget && Time.realtimeSinceStartup - a.LastUsedTime < 1f)
				{
					return 1;
				}
				if (b == mLastLockOnTarget && Time.realtimeSinceStartup - b.LastUsedTime < 1f)
				{
					return -1;
				}
				if (a.LastDamageTime > b.LastDamageTime)
				{
					if (Time.realtimeSinceStartup - a.LastDamageTime < 1f)
					{
						return -1;
					}
				}
				else if (Time.realtimeSinceStartup - b.LastDamageTime < 1f)
				{
					return 1;
				}
			}
			Vector3 snapPosition2 = a.GetSnapPosition();
			Vector3 snapPosition3 = b.GetSnapPosition();
			bool side = sidePlane.GetSide(snapPosition2);
			bool side2 = sidePlane.GetSide(snapPosition3);
			bool flag = (snapType == SnapType.SnapLeft) ^ side;
			bool flag2 = (snapType == SnapType.SnapLeft) ^ side2;
			bool flag3 = snapType != SnapType.SnapAny && snapType != SnapType.SnapShort;
			if (flag3)
			{
				if (flag && !flag2)
				{
					return -1;
				}
				if (flag2 && !flag)
				{
					return 1;
				}
			}
			float num2 = Maths.DistanceToLineSegment(snapPosition2, lineOrigin, lineEnd);
			float num3 = Maths.DistanceToLineSegment(snapPosition3, lineOrigin, lineEnd);
			if (flag3 && (!flag || !flag2))
			{
				if (num2 > num3)
				{
					return -1;
				}
				if (num3 > num2)
				{
					return 1;
				}
			}
			else
			{
				if (num2 < num3)
				{
					return -1;
				}
				if (num3 < num2)
				{
					return 1;
				}
			}
			return 0;
		};
		SnapTarget snapTarget = null;
		Actor actor = null;
		List<SnapTarget> list = new List<SnapTarget>(SnapTarget.Instances);
		list.Sort(comparison);
		foreach (SnapTarget item in list)
		{
			if (snapTarget != null)
			{
				break;
			}
			item.LockOnDetector.SetActive(true);
			Vector3 snapPosition = item.GetSnapPosition();
			SnapTarget.LockOnType lockOnType = item.GetLockOnType();
			if (Time.realtimeSinceStartup - item.LastDamageTime < 1f || lockOnType == SnapTarget.LockOnType.LockOn || (lockOnType == SnapTarget.LockOnType.DoTrace && WorldHelper.IsClearLockOnTrace(mFirstPersonActor.realCharacter.FirstPersonCamera.transform.position, snapPosition)))
			{
				snapTarget = item;
				actor = item.GetComponent<Actor>();
			}
			item.LockOnDetector.SetActive(false);
		}
		if (snapTarget != null)
		{
			InterfaceSFX.Instance.TargetSnap.Play2D();
			FirstPersonSnapToActor(new AutoSnapTarget(snapTarget), snapType);
			snapTarget.LastUsedTime = Time.realtimeSinceStartup;
		}
		else if (snapType != SnapType.SnapShort)
		{
			InterfaceSFX.Instance.TargetSnapUnavailable.Play2D();
		}
		mLastLockOnTarget = snapTarget;
		mSoftLockTarget = actor;
	}

	public void ADSAutoLockOn()
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().ADSAutoLockOn)
		{
			Instance.FirstPersonLockOn(SnapType.SnapShort);
		}
	}

	private void FirstPersonSnapToActor(AutoSnapTarget autoSnapTarget)
	{
		FirstPersonSnapToActor(autoSnapTarget, SnapType.SnapAny);
	}

	private void FirstPersonSnapToActor(AutoSnapTarget autoSnapTarget, SnapType snapType)
	{
		if (!(mFirstPersonActor == null) && !(mFirstPersonPointOfInterest != null))
		{
			mTimeSinceLock = 0f;
			bool flag = mFirstPersonActor.weapon.ActiveWeapon.HasScope();
			if (!mFirstPersonActor.realCharacter.IsAimingDownSights)
			{
				mFirstPersonActor.realCharacter.IsAimingDownSights = !flag;
			}
			float num = ((!flag) ? 40f : 5f);
			mFirstPersonLockOnAngles = GetAnglesForFirstPersonLookAt(autoSnapTarget.GetSnapPosition());
			if (snapType == SnapType.SnapShort && (Mathf.Abs(mFirstPersonLockOnAngles.x) > num || Mathf.Abs(mFirstPersonLockOnAngles.y) > num))
			{
				mFirstPersonLockOnAngles = Vector3.zero;
			}
		}
	}

	public void FirstPersonTargetUpdate()
	{
		if (!(FirstPersonCamera != null))
		{
			return;
		}
		int hitLayerMask = (1 << LayerMask.NameToLayer("PelvicRegion")) | ProjectileManager.SimpleUnitProjectileMask;
		Vector3 position = FirstPersonCamera.Position;
		Vector3 forward = FirstPersonCamera.transform.forward;
		position += forward;
		Actor[] array = null;
		array = ProjectileManager.TraceAllUnits(position, position + forward, 500f, hitLayerMask);
		if (array != null && array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].realCharacter.Ragdoll.ParentBones();
			}
		}
		GameObject outGo;
		Vector3 outPosition;
		ProjectileManager.TraceFast(position, position + forward, ProjectileManager.ProjectileMask, out outGo, out outPosition);
		if (outGo != null)
		{
			HitLocation component = outGo.GetComponent<HitLocation>();
			FirstPersonTarget = ((!(component != null) || !(component.Actor != null) || component.Actor.awareness.ChDefCharacterType == CharacterType.SentryGun) ? null : component.Actor);
			TargetDist = Vector3.Distance(FirstPersonCamera.Position, outPosition);
		}
		else
		{
			TargetDist = float.MaxValue;
		}
	}

	public void FirstPersonLockOnUpdate()
	{
		mLastLockOnUpdate = Vector3.zero;
		if (mFirstPersonActor != null)
		{
			if (mFirstPersonPointOfInterest != null)
			{
				mFirstPersonLockOnAngles = GetAnglesForFirstPersonLookAt(mFirstPersonPointOfInterest.position);
			}
			bool flag = mFirstPersonLockOnAngles.sqrMagnitude < 300f;
			float num = TimeManager.DeltaTime * ((!flag) ? 360f : 90f);
			Vector3 vector = new Vector3(Mathf.Clamp(mFirstPersonLockOnAngles.x, 0f - num, num), Mathf.Clamp(mFirstPersonLockOnAngles.y, 0f - num, num), 0f);
			mFirstPersonLockOnAngles -= vector;
			TurnFirstPersonCamera(vector);
			mLastLockOnUpdate += vector;
			if (mFirstPersonLockOnAngles.sqrMagnitude < 1f && mFirstPersonPointOfInterest == null)
			{
				float num2 = ((!(mSoftLockTarget != null) || !(mSoftLockTarget.realCharacter != null)) ? 0f : Mathf.Clamp01(mSoftLockTarget.realCharacter.AliveTimeReal));
				float x = ((!(Mathf.Abs(LastLookInput.x) < 0.25f) && Mathf.Sign(LastLookInput.x) != Mathf.Sign(mFirstPersonSoftLockAngles.x)) ? 0f : 1f);
				float y = ((!(Mathf.Abs(LastLookInput.y) < 0.25f) && Mathf.Sign(LastLookInput.y) != Mathf.Sign(mFirstPersonSoftLockAngles.y)) ? 0f : 1f);
				Vector3 vector2 = 0.5f * TimeManager.DeltaTime * new Vector3(1f, 1f, 0f);
				mSoftLockIntensity = Vector3.Min(mSoftLockIntensity + vector2, num2 * new Vector3(x, y, 0f));
				num = TimeManager.DeltaTime * 360f;
				vector = new Vector3(Mathf.Clamp(mFirstPersonSoftLockAngles.x, mSoftLockIntensity.x * (0f - num), mSoftLockIntensity.x * num), Mathf.Clamp(mFirstPersonSoftLockAngles.y, mSoftLockIntensity.y * (0f - num), mSoftLockIntensity.y * num), 0f);
				mFirstPersonSoftLockAngles -= vector;
				TurnFirstPersonCamera(vector);
				mLastLockOnUpdate += vector;
			}
			else if (mTimeSinceLock < 1f && mLastLockOnTarget != null)
			{
				mFirstPersonLockOnAngles = GetAnglesForFirstPersonLookAt(mLastLockOnTarget.GetSnapPosition());
			}
		}
	}

	private void TurnFirstPersonCamera(Vector3 amount)
	{
		if ((amount.x != 0f || amount.y != 0f) && !(mFirstPersonActor == null) && !(mFirstPersonActor.realCharacter == null))
		{
			FirstPersonCamera firstPersonCamera = mFirstPersonActor.realCharacter.FirstPersonCamera;
			Vector3 angles = firstPersonCamera.Angles + amount;
			angles.x = Mathf.Clamp(angles.x, -80f, 80f);
			angles = firstPersonCamera.GetConstrainedAngles(angles);
			firstPersonCamera.Angles = angles;
		}
	}

	private Actor GetPreviousFirstPersonActor()
	{
		ReadOnlyCollection<Actor> validFirstPersonActors = GameplayController.Instance().GetValidFirstPersonActors();
		if (validFirstPersonActors.Count > 1)
		{
			for (int i = 0; i < validFirstPersonActors.Count; i++)
			{
				if (validFirstPersonActors[i] == mFirstPersonActor)
				{
					return validFirstPersonActors[Maths.Modulus(i - 1, validFirstPersonActors.Count)];
				}
			}
		}
		return null;
	}

	private Actor GetNextFirstPersonActor()
	{
		ReadOnlyCollection<Actor> validFirstPersonActors = GameplayController.Instance().GetValidFirstPersonActors();
		if (validFirstPersonActors.Count > 1)
		{
			for (int i = 0; i < validFirstPersonActors.Count; i++)
			{
				if (validFirstPersonActors[i] == mFirstPersonActor)
				{
					return validFirstPersonActors[Maths.Modulus(i + 1, validFirstPersonActors.Count)];
				}
			}
		}
		return null;
	}

	public void SuppressHud(bool suppress)
	{
		mSuppressHud = suppress;
		if (suppress)
		{
			HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
			mCMTriggerLogic.AllowInput(false);
			return;
		}
		if (OverwatchController.Instance != null && OverwatchController.Instance.Active)
		{
			HudStateController.Instance.SetState(HudStateController.HudState.Overwatch);
			return;
		}
		if (mFirstPersonActor != null)
		{
			SetFPPHud();
			mCMTriggerLogic.AllowInput(AllowContextActionsInFirstPerson);
			return;
		}
		switch (CameraManager.Instance.ActiveCamera)
		{
		case CameraManager.ActiveCameraType.PlayCamera:
			HudStateController.Instance.SetState(HudStateController.HudState.TPP);
			mCMTriggerLogic.AllowInput(true);
			break;
		case CameraManager.ActiveCameraType.StrategyCamera:
			HudStateController.Instance.SetState(HudStateController.HudState.Strategy);
			mCMTriggerLogic.AllowInput(false);
			break;
		default:
			throw new Exception("Enabling HUD but have no idea what camera type this is");
		}
	}

	public void LinkCommonHudController(CommonHudController hudCtrl)
	{
		hudCtrl.OnZoomInTriggered += OnZoomInTriggered;
		hudCtrl.OnZoomOutTriggered += OnZoomOutTriggered;
		mCommonHud = GameObject.Find("CommonHud");
		TBFAssert.DoAssert(mCommonHud, "no CommonHud found");
		hudCtrl.SetZoomButtons(AllowFirstPersonAtAnyPoint, true);
		hudCtrl.OnSnapEnemyLeftClick += OnFirstPersonSnapEnemyLeftClick;
		hudCtrl.OnSnapEnemyRightClick += OnFirstPersonSnapEnemyRightClick;
		hudCtrl.OnGrenadeIndicatorClick += OnFirstPersonGrenadeIndicatorClick;
		hudCtrl.OnSnapUnitLeftClick += OnFirstPersonSnapUnitLeftClick;
		hudCtrl.OnSnapUnitRightClick += OnFirstPersonSnapUnitRightClick;
		hudCtrl.OnDuckClick += OnFirstPersonDuckClick;
		hudCtrl.OnReloadClick += OnFirstPersonReloadClick;
		mFirstPersonHud = GameObject.Find("FirstPersonHud");
		TBFAssert.DoAssert(mFirstPersonHud, "no FirstPersonHud found");
		mFirstPersonHud.SetActive(false);
		mFirstPersonHud.transform.parent = base.transform;
		mCommonHud.transform.parent = base.transform;
		mCommonHud.SetActive(false);
	}

	public void LinkHudController(StrategyHudController hudCtrl)
	{
		hudCtrl.OnTechPurchaseClick += OnTechPurchaseClick;
		mStrategyHud = GameObject.Find("StrategyHud");
		TBFAssert.DoAssert(mStrategyHud, "no StrategyHud found");
		mStrategyHud.transform.parent = base.transform;
		mStrategyHud.SetActive(false);
	}

	public void LinkTechMenuController(TechMenuController techMenu)
	{
		mTechMenuCtrl = techMenu;
	}

	public void LinkLoadoutMenuController(LoadoutMenuNavigator loadoutMenu)
	{
		mLoadoutMenuCtrl = loadoutMenu;
	}

	public void OnMissionFailed(object sender)
	{
		if (!mMissionEnding)
		{
			mMissionEnding = true;
			StartCoroutine(DelayMissionFail());
		}
	}

	private IEnumerator DelayMissionFail()
	{
		while (mPaused)
		{
			yield return null;
		}
		if (!Instance.IsLockedToFirstPerson)
		{
			float time = Time.realtimeSinceStartup + 5f;
			while (Time.realtimeSinceStartup < time)
			{
				yield return null;
			}
			SwitchToPlayCamera(true);
		}
		SuppressHud(true);
		InputManager.Instance.SetForMessageBox();
		ActStructure.Instance.MissionFinished(false, mTimePlayed / 60f);
		FrontEndController.Instance.TransitionTo(ScreenID.MissionComplete);
	}

	public void OnMissionPassed(object sender, float delayTime)
	{
		if (delayTime == 0f)
		{
			mDelayTime = 0f;
			mForceMissionEnd = true;
		}
		if (!mMissionEnding)
		{
			mMissionEnding = true;
			StartCoroutine(DelayMissionPass(delayTime));
		}
	}

	private IEnumerator DelayMissionPass(float delayTime)
	{
		mDelayTime = Time.realtimeSinceStartup + delayTime;
		while (Time.realtimeSinceStartup < mDelayTime || (InteractionsManager.IsBusy() && !mForceMissionEnd))
		{
			yield return null;
		}
		while (mPaused)
		{
			yield return null;
		}
		bool isGMGMission = ActStructure.Instance != null && ActStructure.Instance.CurrentMissionIsSpecOps();
		if (!mForceMissionEnd && !isGMGMission)
		{
			SwitchToPlayCamera(true);
		}
		BlackBarsController.Instance.ClearSubtitle();
		SuppressHud(true);
		InputManager.Instance.SetForMessageBox();
		ActStructure.Instance.MissionFinished(true, mTimePlayed / 60f);
		FrontEndController.Instance.TransitionTo(ScreenID.MissionComplete);
	}

	public void OnStrategyViewClick(object sender, EventArgs args)
	{
		if (!CameraManager.Instance.IsSwitching)
		{
			SwitchToStrategyCamera(null);
			MusicManager.Instance.PlayStrategyMapMusic();
		}
	}

	public void OnGameplayViewClick(object sender, EventArgs args)
	{
		if (!mGameplayStarted)
		{
			BeginGameplay();
		}
		if (!CameraManager.Instance.IsSwitching)
		{
			SwitchToPlayCamera();
			if (MissionSetup.Instance != null && MissionSetup.Instance.BGMusicTrack != null && MissionSetup.Instance.BGMusicTrack != string.Empty)
			{
				MusicManager.Instance.PlayAmbientMusic(MissionSetup.Instance.BGMusicTrack);
			}
			else
			{
				MusicManager.Instance.PlayAmbientRainMusic();
			}
		}
	}

	public void StartCountdownToAdreneline(PlayCameraInterface pci, Actor actor, float delay, Actor damageDealer)
	{
		StartCoroutine(SwitchToFirstPersonWhenCameraFocusFinished(pci, actor, delay, damageDealer));
	}

	private IEnumerator SwitchToFirstPersonWhenCameraFocusFinished(PlayCameraInterface pci, Actor actor, float delay, Actor damageDealer)
	{
		FirstPersonTransitionInActive = true;
		if (actor.baseCharacter.IsMortallyWounded() || actor.baseCharacter.IsDead() || actor.tasks.IsRunningTask<TaskHeal>())
		{
			CommonHudController.Instance.SetZoomButtonFrame(false);
			FirstPersonTransitionInActive = false;
			yield break;
		}
		if ((actor.tasks.IsRunningTask<TaskPlantC4>() || actor.tasks.IsRunningTask<TaskDropClaymore>() || actor.tasks.IsRunningTask<TaskPickUp>()) && !actor.baseCharacter.IsInASetPiece)
		{
			actor.tasks.CancelTasks<TaskPlantC4>();
			actor.tasks.CancelTasks<TaskDropClaymore>();
			actor.tasks.CancelTasks<TaskPickUp>();
		}
		if (actor.tasks.IsRunningTask<TaskCarry>())
		{
			actor.tasks.CancelTasks<TaskCarry>();
			actor.tasks.CancelTasks<TaskMultiCharacterSetPiece>();
		}
		float timeout = 2f;
		while (pci.MovingToFocusPoint && timeout > 0f)
		{
			timeout -= TimeManager.DeltaTime;
			if (InteractionsManager.Instance.IsPlayingCutscene())
			{
				FirstPersonTransitionInActive = false;
				yield break;
			}
			yield return null;
		}
		if (!GameplayController.instance.IsSelectedLeader(actor))
		{
			FirstPersonTransitionInActive = false;
			yield break;
		}
		if (delay <= 0f && !CameraManager.Instance.IsSwitching && CameraManager.Instance.ActiveCamera != CameraManager.ActiveCameraType.StrategyCamera)
		{
			HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
			if (PostProcessGFXInterface.Instance != null)
			{
				PostProcessGFXInterface.Instance.DoTrans();
			}
			float transitionTime = 0.5f;
			TransitionCamera cam = new GameObject("TransitionCamera").AddComponent<TransitionCamera>();
			float fovPerSecond = 0f;
			mDisableZoomButtonsDuringCoroutineTransition = true;
			if (cam != null)
			{
				CameraController cc = CameraManager.Instance.PlayCameraController;
				cam.transform.position = cc.CurrentCameraBase.Position;
				cam.transform.rotation = cc.CurrentCameraBase.Rotation;
				cam.Fov = cc.CurrentCameraBase.Fov;
				cc.BlendTo(new CameraTransitionData(cam, TweenFunctions.TweenType.easeInCubic, transitionTime));
				fovPerSecond = (cam.Fov - 5f) / transitionTime;
			}
			while (transitionTime > 0f)
			{
				if (InteractionsManager.Instance.IsPlayingCutscene())
				{
					mDisableZoomButtonsDuringCoroutineTransition = false;
					FirstPersonTransitionInActive = false;
					yield break;
				}
				float timeDelta = TimeManager.DeltaTime;
				if (IsPaused)
				{
					timeDelta = 0f;
				}
				transitionTime -= timeDelta;
				if (!CameraManager.Instance.IsSwitching)
				{
					yield return null;
				}
				cam.Fov -= timeDelta * fovPerSecond;
				cam.Rotation = Quaternion.RotateTowards(to: Quaternion.LookRotation((actor.realCharacter.GetCameraTransitionTarget() - cam.Position).normalized, Vector3.up), from: cam.Rotation, maxDegreesDelta: timeDelta * 30f);
			}
			if (!CameraManager.Instance.IsSwitching && CameraManager.Instance.ActiveCamera != CameraManager.ActiveCameraType.StrategyCamera)
			{
				InterfaceSFX.Instance.ViewChangeStatic.Play2D();
				Instance.SwitchToFirstPerson(actor, true);
			}
			mDisableZoomButtonsDuringCoroutineTransition = false;
			UnityEngine.Object.Destroy(cam.gameObject);
		}
		if (mAutoSnapTarget == null && !(damageDealer == null))
		{
			mAutoSnapTarget = new AutoSnapTarget(damageDealer.baseCharacter.SnapTarget);
		}
		if (GrenadeThrowingModeActive)
		{
			UnityEngine.Debug.LogWarning("Grenade Throwing Mode is active on entry to FPP. This could cause a soft-lock on return to TPP.");
		}
		FirstPersonTransitionInActive = false;
	}

	public void OnZoomInTriggered(object sender, EventArgs args)
	{
		if (CameraManager.Instance.IsSwitching)
		{
			return;
		}
		if (IsFirstPerson)
		{
			mFirstPersonActor.realCharacter.IsAimingDownSights = true;
			Instance.ADSAutoLockOn();
			ZoomInAvailable = false;
			ZoomOutAvailable = true;
		}
		else
		{
			ZoomInAvailable = AllowFirstPersonAtAnyPoint;
			ZoomOutAvailable = false;
		}
		if (!IsFirstPerson && CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.PlayCamera)
		{
			Actor bestFirstPersonActor = GameplayController.Instance().GetBestFirstPersonActor();
			if (bestFirstPersonActor != null && !IsLockedToCurrentSetPiece(bestFirstPersonActor))
			{
				CameraController playCameraController = CameraManager.Instance.PlayCameraController;
				PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
				if (playCameraInterface != null)
				{
					playCameraInterface.FocusOnTarget(bestFirstPersonActor.transform, true);
					StartCoroutine(SwitchToFirstPersonWhenCameraFocusFinished(playCameraInterface, bestFirstPersonActor, -1f, null));
				}
			}
			else
			{
				CommonHudController.Instance.SetZoomButtonFrame(false);
				ZoomInAvailable = true;
				ZoomOutAvailable = false;
			}
		}
		else if (CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.StrategyCamera)
		{
			OnGameplayViewClick(sender, args);
		}
	}

	public void OnZoomOutTriggered(object sender, EventArgs args)
	{
		if (CameraManager.Instance.IsSwitching)
		{
			return;
		}
		if (CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.PlayCamera)
		{
			if (!HandleExitingFirstPerson())
			{
				OnStrategyViewClick(sender, args);
			}
			return;
		}
		MessageBoxController instance = MessageBoxController.Instance;
		if (instance != null)
		{
			SuppressHud(true);
			instance.DoConfirmQuitDialogue(this, "MessageBoxResultConfirmQuit", "MessageBoxCancelled");
		}
	}

	private bool HandleExitingFirstPerson()
	{
		if (mFirstPersonActor != null)
		{
			if (mFirstPersonActor.realCharacter.IsAimingDownSights)
			{
				mFirstPersonActor.realCharacter.IsAimingDownSights = false;
			}
			TaskCacheStanceState runningTask = mFirstPersonActor.tasks.GetRunningTask<TaskCacheStanceState>();
			if (runningTask != null)
			{
				runningTask.IgnoreFPPCacheCheck();
				if (mFirstPersonActor.tasks.IsRunningTask<TaskUseFixedGun>())
				{
					runningTask.ClearFppCache();
					mFirstPersonActor.tasks.CancelTasks<TaskSetPiece>();
				}
			}
			if (IsLockedToCurrentSetPiece(mFirstPersonActor))
			{
				CommonHudController.Instance.SetZoomButtonFrame(true);
				return true;
			}
			ExitFirstPerson();
			ZoomInAvailable = AllowFirstPersonAtAnyPoint;
			ZoomOutAvailable = false;
			StartCoroutine(HideTPPHUDDuringTransition());
			return true;
		}
		return false;
	}

	private bool IsLockedToCurrentSetPiece(Actor actor)
	{
		if (actor == null)
		{
			return false;
		}
		TaskManager tasks = actor.tasks;
		if (tasks != null && (tasks.IsRunningTask<TaskDropClaymore>() || tasks.IsRunningTask<TaskPlantC4>() || tasks.IsRunningTask<TaskPickUp>()))
		{
			return actor.baseCharacter.IsInASetPiece;
		}
		return false;
	}

	private void MessageBoxResultConfirmQuit()
	{
		ActStructure.Instance.MissionQuit();
		Application.LoadLevel("GlobeSelectCore");
		TBFUtils.UberGarbageCollect();
	}

	private void MessageBoxCancelled()
	{
		SuppressHud(false);
	}

	public void ExitFirstPerson(bool isLeavingFirstPersonForSetPiece, bool DoTransition)
	{
		if (Instance.IsFirstPerson)
		{
			mIsLeavingFirstPersonForSetPiece = isLeavingFirstPersonForSetPiece;
			ExitFirstPerson(DoTransition);
		}
	}

	public void ExitFirstPerson()
	{
		ExitFirstPerson(true);
	}

	private IEnumerator ExitFirstPersonTransition(CameraController cc, Actor actor)
	{
		float transitionTime = 0.3f;
		TransitionCamera cam = new GameObject("TransitionCamera").AddComponent<TransitionCamera>();
		CameraBase targetCam = cc.StartCamera;
		AnimationCurve fovCurve = new AnimationCurve(new Keyframe(transitionTime, 15f, 0f, 0f), new Keyframe(0f, targetCam.Fov, 0f, 0f));
		if (cam != null)
		{
			Vector3 toTarget = (actor.realCharacter.GetCameraTransitionTarget() - targetCam.Position).normalized;
			cam.transform.position = targetCam.Position;
			cam.transform.rotation = Quaternion.LookRotation(toTarget, Vector3.up);
			cam.Fov = fovCurve.Evaluate(transitionTime);
			cc.ForcedCutTo(cam);
		}
		mDisableZoomButtonsDuringCoroutineTransition = true;
		CameraController cc2 = default(CameraController);
		Func<bool> cameraCheck = () => (!cc2.IsTransitioning() && cc2.CurrentCameraBase == cam) || cc2.IsTransitioningTo(cam);
		while (transitionTime > 0f && cameraCheck())
		{
			if (InteractionsManager.Instance.IsPlayingCutscene())
			{
				mDisableZoomButtonsDuringCoroutineTransition = false;
				yield break;
			}
			float timeDelta = TimeManager.DeltaTime;
			if (IsPaused)
			{
				timeDelta = 0f;
			}
			transitionTime -= timeDelta;
			if (!CameraManager.Instance.IsSwitching)
			{
				yield return null;
			}
			cam.Fov = fovCurve.Evaluate(transitionTime);
			cam.Rotation = Quaternion.RotateTowards(cam.Rotation, targetCam.Rotation, timeDelta * 10f);
			yield return null;
		}
		mDisableZoomButtonsDuringCoroutineTransition = false;
		if (cameraCheck())
		{
			cc.ForcedCutTo(targetCam);
			cc.RestoreCameraToDefault();
		}
		UnityEngine.Object.Destroy(cam.gameObject);
	}

	public void ExitFirstPerson(bool DoTransition)
	{
		if (!IsFirstPerson)
		{
			return;
		}
		if (!IsLockedToFirstPerson || !IsLockedToCurrentCharacter)
		{
			IsLockedToFirstPerson = false;
			IsLockedToCurrentCharacter = false;
		}
		FirstPersonOnly.ShowHideFPPObjects(false);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		StartCoroutine(ExitFirstPersonTransition(playCameraController, mFirstPersonActor));
		PlayCameraInterface playCameraInterface = playCameraController.StartCamera as PlayCameraInterface;
		if (playCameraInterface != null && mFirstPersonActor != null)
		{
			playCameraInterface.FocusOnTarget(mFirstPersonActor.transform, true);
			if (mIsLeavingFirstPersonForSetPiece)
			{
				mIsLeavingFirstPersonForSetPiece = false;
			}
			else
			{
				GameplayController.instance.MoveIntoNearbyCover(mFirstPersonActor);
			}
		}
		ClearFirstPerson();
		SwitchToPlayCamera();
		ZoomInAvailable = AllowFirstPersonAtAnyPoint;
		ZoomOutAvailable = false;
		FirstPersonFollowMe = false;
		TimeManager.instance.SlowDownTime(0f, TimeManager.instance.StopTimeModeSpeed);
		if (DoTransition)
		{
			if (!OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.Bloom))
			{
				mTimeSinceFirstPersonTransition = 0f;
			}
			mTimeSinceFirstPersonTransitionHUD = 0f;
		}
		if (this.OnExitFirstPerson != null)
		{
			this.OnExitFirstPerson();
		}
	}

	public bool StartGrenadeThrowingMode()
	{
		TBFAssert.DoAssert(!mGrenadeThrowingMode, "Grenade Throwing mode already active when trying to start");
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				SuppressHud(true);
				playCameraInterface.EnablePlacementInput(true);
				mGrenadeThrowingMode = true;
				return true;
			}
		}
		return false;
	}

	public void EndGrenadeThrowingMode()
	{
		TBFAssert.DoAssert(mGrenadeThrowingMode, "Grenade Throwing mode not active when trying to end");
		SuppressHud(false);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.EnablePlacementInput(false);
				mGrenadeThrowingMode = false;
			}
		}
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController != null)
		{
			gameplayController.CancelAnyPrimedGrenade();
		}
	}

	public bool StartGMGReviveMode()
	{
		TBFAssert.DoAssert(!mGMGReviveMode, "Grenade Throwing mode already active when trying to start");
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				SuppressHud(true);
				playCameraInterface.EnablePlacementInput(true);
				mGMGReviveMode = true;
				return true;
			}
		}
		return false;
	}

	public void EndGMGReviveMode()
	{
		TBFAssert.DoAssert(mGMGReviveMode, "Grenade Throwing mode not active when trying to end");
		SuppressHud(false);
		CameraManager.Instance.AllowInput(true);
		LockGyro = false;
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.EnablePlacementInput(false);
				mGMGReviveMode = false;
			}
		}
	}

	public void StartPlacementMode()
	{
		TBFAssert.DoAssert(!mPlacementMode, "Placement mode already active when trying to start");
		SuppressHud(true);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.EnablePlacementInput(true);
				mPlacementMode = true;
			}
		}
	}

	public void EndPlacementMode()
	{
		if (!mPlacementMode)
		{
			UnityEngine.Debug.LogWarning("Placement mode not active when trying to end");
			return;
		}
		SuppressHud(false);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.EnablePlacementInput(false);
				mPlacementMode = false;
			}
		}
	}

	public void StartClaymoreDroppingMode()
	{
		TBFAssert.DoAssert(!mClaymoreDroppingMode, "Claymore dropping mode already active when trying to start");
		SuppressHud(true);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.AllowInput(false);
				mClaymoreDroppingMode = true;
			}
		}
	}

	public void EndClaymoreDroppingMode()
	{
		TBFAssert.DoAssert(mClaymoreDroppingMode, "Claymore dropping mode not active when trying to end");
		SuppressHud(false);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.AllowInput(true);
				mClaymoreDroppingMode = false;
			}
		}
	}

	public bool ClaymoreDroppingInProgress()
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.tasks.IsRunningTask(typeof(TaskDropClaymore)))
			{
				return true;
			}
		}
		return false;
	}

	public void StartSentryHackingMode()
	{
		TBFAssert.DoAssert(!mSentryHackingMode, "Sentry hacking mode already active when trying to start");
		SuppressHud(true);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.AllowInput(false);
				mSentryHackingMode = true;
			}
		}
	}

	public void EndSentryHackingMode()
	{
		TBFAssert.DoAssert(mSentryHackingMode, "Sentry hacking mode not active when trying to end");
		SuppressHud(false);
		CameraController playCameraController = CameraManager.Instance.PlayCameraController;
		if (playCameraController != null)
		{
			PlayCameraInterface playCameraInterface = playCameraController.CurrentCameraBase as PlayCameraInterface;
			if (playCameraInterface != null)
			{
				playCameraInterface.AllowInput(true);
				mSentryHackingMode = false;
			}
		}
	}

	public void NotifyCombatantKilled()
	{
		if (!IsFirstPerson)
		{
			TimeManager.instance.SlowDownTime(0f, TimeManager.instance.StopTimeModeSpeed);
		}
	}

	public void NotifyEnemySpotted(Actor observer, Actor target)
	{
		if ((bool)observer.speech)
		{
			observer.speech.EnemySpotted(observer, target);
		}
	}

	public void IssueFollowMeCommand()
	{
		if (mFirstPersonActor == null)
		{
			return;
		}
		List<Actor> list = new List<Actor>();
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a != mFirstPersonActor)
			{
				a.speech.FollowMe();
				list.Add(a);
				TaskEnter taskEnter = a.tasks.GetRunningTask(typeof(TaskEnter)) as TaskEnter;
				if (taskEnter != null && taskEnter.Container != null && taskEnter.HasFinishedEntering)
				{
					OrdersHelper.OrderExit(GameplayController.instance, taskEnter.Container);
					return;
				}
			}
		}
		OrdersHelper.UpdatePlayerSquadTetherPoint(mFirstPersonActor.GetPosition(), list);
	}

	private void OnTechPurchaseClick(object sender, EventArgs args)
	{
		if (mTechMenuCtrl != null)
		{
			mTechMenuCtrl.Args = (TechPurchaseEventArgs)args;
			mTechMenuCtrl.Activate();
		}
	}

	private void OnFirstPersonSnapEnemyLeftClick(object sender, EventArgs args)
	{
		FirstPersonLockOn(SnapType.SnapLeft);
	}

	private void OnFirstPersonSnapEnemyRightClick(object sender, EventArgs args)
	{
		FirstPersonLockOn(SnapType.SnapRight);
	}

	private void OnFirstPersonGrenadeIndicatorClick(object sender, EventArgs args)
	{
		mFirstPersonLockOnAngles = GetAnglesForFirstPersonLookAt(CommonHudController.Instance.GrenadeIndicatorPosition);
	}

	public bool ADSSuppressed()
	{
		return mFirstPersonLockOnAngles.sqrMagnitude > 300f;
	}

	private Vector3 GetAnglesForFirstPersonLookAt(Vector3 lookAtPosition)
	{
		return GetAnglesForFirstPersonLookAt(mFirstPersonActor, lookAtPosition);
	}

	private Vector3 GetAnglesForFirstPersonLookAt(Actor actor, Vector3 lookAtPosition)
	{
		Vector3 result = Vector3.zero;
		if (actor != null)
		{
			Vector3 pointInReferenceFrame = mFirstPersonActor.realCharacter.GetPointInReferenceFrame(lookAtPosition);
			Vector3 pointInReferenceFrame2 = mFirstPersonActor.realCharacter.GetPointInReferenceFrame(actor.realCharacter.FirstPersonCamera.transform.position);
			result = Quaternion.LookRotation(pointInReferenceFrame - pointInReferenceFrame2).eulerAngles;
			Vector3 angles = mFirstPersonActor.realCharacter.FirstPersonCamera.Angles;
			result = new Vector3(Mathf.DeltaAngle(angles.x, result.x), Mathf.DeltaAngle(angles.y, result.y), 0f);
		}
		return result;
	}

	private void SnapUnitLeft()
	{
		if (!IsLockedToCurrentCharacter && !IsLockedToCurrentSetPiece(mFirstPersonActor))
		{
			Actor previousFirstPersonActor = GetPreviousFirstPersonActor();
			if (previousFirstPersonActor != null)
			{
				SwitchToFirstPerson(previousFirstPersonActor, true);
			}
		}
	}

	private void SnapUnitRight()
	{
		if (!IsLockedToCurrentCharacter && !IsLockedToCurrentSetPiece(mFirstPersonActor))
		{
			Actor nextFirstPersonActor = GetNextFirstPersonActor();
			if (nextFirstPersonActor != null)
			{
				SwitchToFirstPerson(nextFirstPersonActor, true);
			}
		}
	}

	private void OnFirstPersonSnapUnitLeftClick(object sender, EventArgs args)
	{
		SnapUnitLeft();
	}

	private void OnFirstPersonSnapUnitRightClick(object sender, EventArgs args)
	{
		SnapUnitRight();
	}

	private void OnFirstPersonDuckClick(object sender, EventArgs args)
	{
	}

	private void OnFirstPersonReloadClick(object sender, EventArgs args)
	{
		if (mFirstPersonActor != null && mFirstPersonActor.weapon != null && mFirstPersonActor.weapon.CanReload())
		{
			mFirstPersonActor.weapon.Reload();
		}
	}

	private IEnumerator HideTPPHUDDuringTransition()
	{
		HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
		while (TransitionEffectAmountHUD > 0f)
		{
			yield return null;
		}
		HudStateController.Instance.SetState(HudStateController.HudState.TPP);
	}

	public IEnumerator DisplayInGameMessageBoxHelper(MessageBox msgBoxPrefab, string title, string body)
	{
		yield return DisplayInGameMessageBoxHelper(msgBoxPrefab, title, body, new Vector2(-1f, -1f), false);
	}

	public IEnumerator DisplayInGameMessageBoxHelper(MessageBox msgBoxPrefab, string title, string body, Vector2 minSize, bool autoSize)
	{
		CommonHudController.Instance.ClearContextMenu();
		InputManager.Instance.SetForMessageBox();
		TimeManager.instance.StopTime();
		MessageBox messageBox = (MessageBox)UnityEngine.Object.Instantiate(msgBoxPrefab);
		yield return StartCoroutine(messageBox.Display(title, body, false));
		InputManager.Instance.SetForGamplay();
		TimeManager.instance.ResumeNormalTime();
	}

	private void ClearCharacterFirstPerson()
	{
		if (mFirstPersonActor != null)
		{
			mFirstPersonActor.tasks.CancelTasks<TaskFirstPerson>();
			mFirstPersonActor.realCharacter.IsFirstPerson = false;
			mFirstPersonActor.realCharacter.SetupForThirdPerson();
			mFirstPersonActor.realCharacter.HandleForcedExhale();
			mPreviousFirstPersonActor = mFirstPersonActor;
			mFirstPersonActor = null;
		}
		mSprintBlend = 0f;
	}

	private void ClearFirstPerson()
	{
		RestoreScreenRotation();
		ClearCharacterFirstPerson();
		InputManager.Instance.SetForGamplay();
		WaypointMarkerManager.Instance.EnableRendering();
	}

	private void LockScreenRotationIfGyroEnabled()
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PlayerGameSettings().FirstPersonGyroscopeEnabled)
		{
			LockScreenRotation();
		}
	}

	private void LockScreenRotation()
	{
		if (!mScreenRotationLocked)
		{
			mAutorotateToLandscapeLeftRestoreState = Screen.autorotateToLandscapeLeft;
			mAutorotateToLandscapeRightRestoreState = Screen.autorotateToLandscapeRight;
			mAutorotateToPortraitRestoreState = Screen.autorotateToPortrait;
			mAutorotateToPortraitUpsideDownRestoreState = Screen.autorotateToPortraitUpsideDown;
			Screen.autorotateToLandscapeLeft = false;
			Screen.autorotateToLandscapeRight = false;
			Screen.autorotateToPortrait = false;
			Screen.autorotateToPortraitUpsideDown = false;
			mScreenRotationLocked = true;
		}
	}

	private void RestoreScreenRotation()
	{
		if (mScreenRotationLocked)
		{
			Screen.autorotateToLandscapeLeft = mAutorotateToLandscapeLeftRestoreState;
			Screen.autorotateToLandscapeRight = mAutorotateToLandscapeRightRestoreState;
			Screen.autorotateToPortrait = mAutorotateToPortraitRestoreState;
			Screen.autorotateToPortraitUpsideDown = mAutorotateToPortraitUpsideDownRestoreState;
			mScreenRotationLocked = false;
		}
	}

	private bool BeginGameplay()
	{
		bool result = false;
		if (!mGameplayStarted)
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController != null)
			{
				gameplayController.EnableInput();
			}
			MissionDescriptor instance = MissionDescriptor.Instance;
			if (instance != null && instance.m_PlayIntroSequence)
			{
				result = true;
			}
			else
			{
				SuppressHud(false);
				if ((bool)gameplayController)
				{
					gameplayController.LevelStarted(this);
				}
			}
			mGameplayStarted = true;
			TimeManager.instance.ResumeNormalTime();
			if (instance != null && !instance.m_PlayIntroSequence)
			{
				TimeManager.instance.StopTime();
			}
			SoundManager.Instance.GamePlayStarted();
		}
		return result;
	}

	private void CalculateFirstPersonCover()
	{
		TBFAssert.DoAssert(mFirstPersonActor != null, "Invalid use of CalculateFirstPersonCover");
		if (mFirstPersonActor.awareness.closestCoverPoint == null)
		{
			return;
		}
		mFirstPersonCoverPoint = null;
		int[] neighbours = mFirstPersonActor.awareness.closestCoverPoint.neighbours;
		if (neighbours == null)
		{
			return;
		}
		int length = neighbours.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			CoverPointCore coverPointCore = CoverNeighbour.CoverPoint(neighbours[i]);
			switch (coverPointCore.type)
			{
			case CoverPointCore.Type.HighCornerLeft:
			case CoverPointCore.Type.HighCornerRight:
			{
				float sqrMagnitude = (coverPointCore.gamePos - mFirstPersonActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude < MINIMUM_FPS_DISTANCE_HIGH_COVER_IS_VALID)
				{
					mFirstPersonCoverPoint = coverPointCore;
					return;
				}
				break;
			}
			case CoverPointCore.Type.ShootOver:
			{
				Vector3 lhs = mFirstPersonActor.GetPosition() - coverPointCore.snappedPos;
				float num = Vector3.Dot(lhs, coverPointCore.snappedNormal);
				if (num > 0f && num < MINIMUM_FPS_DISTANCE_LOW_COVER_IS_VALID)
				{
					float num2 = Vector3.Dot(lhs, coverPointCore.snappedTangent);
					if (num2 > coverPointCore.minExtent && num2 < coverPointCore.maxExtent)
					{
						mFirstPersonCoverPoint = coverPointCore;
						return;
					}
				}
				break;
			}
			}
		}
	}

	public IEnumerator WaitForEnterCover(Actor actor)
	{
		yield return new WaitForEndOfFrame();
		while (!(actor == null))
		{
			Task task = actor.tasks.GetRunningTask(typeof(TaskOverrideRoutine));
			if (task != null && task.BroadcastOnCompletion != null)
			{
				if (!actor.awareness.IsInCover())
				{
					yield return null;
					continue;
				}
				task.BroadcastOnCompletion.SendMessages();
				task.BroadcastOnCompletion.ActivateTurnonables();
				task.BroadcastOnCompletion.DeactivateTurnoffables();
				task.BroadcastOnCompletion = null;
				break;
			}
			break;
		}
	}
}
