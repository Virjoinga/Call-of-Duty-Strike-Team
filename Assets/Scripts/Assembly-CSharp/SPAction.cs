using System;
using UnityEngine;

[Serializable]
public class SPAction
{
	public enum SPActionType
	{
		Invalid = 0,
		PlayAnim = 1,
		SetFog = 2,
		Teleport = 3,
		MoveTo = 4,
		ActorComplete = 5,
		PlaySound = 6,
		SetLineOfSight = 7,
		PoseCommand = 8,
		Selectability = 9,
		CameraLook = 10,
		CameraFinished = 11,
		ShowHide = 12,
		ForceBranch = 13,
		Response = 14,
		CameraToActor = 15,
		BeginCinematic = 16,
		EndCinematic = 17,
		Attach = 18,
		Detach = 19,
		StartPFX = 20,
		ScriptedSequence = 21,
		CameraToFirstPerson = 22,
		CameraToThirdPerson = 23,
		PlayCameraDependentAnim = 24,
		StopSound = 25,
		EnableDisable = 26,
		AlterSlowMo = 27,
		FadeSound = 28,
		FadeInSound = 29,
		FadeOutSound = 30,
		EnableDisableBlobShadow = 31,
		ConfigureHUD = 32,
		HealUnit = 33,
		EnableDisableAwareness = 34,
		FadeSoundGroup = 35,
		ClearArea = 36,
		PlayBodyDropSfx = 37,
		ScreenTransition = 38,
		WeatherSwitch = 39,
		ScreenStaticEffect = 40,
		SetActorStance = 41,
		DropActorAttachment = 42,
		FirstPersonObjectsSwitch = 43,
		FirstPersonTransform = 44,
		SetFppAnimState = 45,
		ResumeNormalTime = 46,
		NumActions = 47
	}

	public enum ReturnCode
	{
		Done = 0,
		Wait = 1,
		Branch = 2
	}

	public enum SPConditionType
	{
		Invalid = 0,
		Default = 1,
		Walking = 2,
		Running = 3,
		NumConditions = 4
	}

	public enum SPMovementType
	{
		Blend = 0,
		Teleport = 1,
		Walk = 2,
		Run = 3,
		NumTypes = 4
	}

	public enum SPTransitionType
	{
		TransitionToSolid = 0,
		TransitionFromSolid = 1
	}

	public enum SPStanceType
	{
		Crouch = 0,
		Stand = 1
	}

	public enum SPFppAnimStateType
	{
		HoldFinalPose = 0,
		ClearHold = 1
	}

	private delegate void ActionDelegate();

	private SetPieceModule mModuleRef;

	public SPActionType Type;

	public AnimationClip Clip;

	public AnimationClip FPSClip;

	public SPObjectReference Actor;

	public SPObjectReference Prop;

	public SPConditionType ConditionType;

	public GameObject ParticleFX;

	public SPObjectReference Target;

	public float DelayTime;

	public float DelayTimeFP;

	public float ActionTime;

	public bool OnOff;

	public bool GOOnOff;

	public bool BlobShadowOnOff;

	public bool HUDOnOff;

	public bool LaserOnOff;

	public string CommandText;

	public string MessageText;

	public bool ActionDone;

	public float Value;

	public string AttachPoint;

	public bool DestroyOnDetach;

	public bool IsSelected;

	public float AnimSpeed;

	public bool AnimLooped;

	public float BlendTime;

	public RawAnimation.AnimBlendType Easing;

	public bool ContinuePlaying;

	public bool SetTether;

	public string WeaponID;

	public bool DontClearDepthBuffer;

	public bool AlwaysAnim;

	public SPMovementType movementType;

	private bool endOnAxis;

	private bool triggerBranch;

	public CameraBase CameraBase;

	public SPObjectReference LookAt;

	public HealthComponent.HealAmount HealAmount;

	public float HealDelayTime;

	public bool IsLockedToCurrentCharacter;

	public bool IsZoomInAvailable;

	public bool IsZoomOutAvailable;

	public SFXBank SoundFPBank;

	public SFXBank SoundBank;

	public string PlayFPFunction;

	public string PlayFunction;

	public bool PlayFP3D;

	public bool Play3D;

	public float PlayVolume = 1f;

	public float PlayVolumeFP = 1f;

	public float FadeDuration;

	public float FadeDurationFP;

	public float DesiredVolume;

	public float DesiredVolumeFP;

	public string FadeFunction;

	public string FadeFPFunction;

	public SoundFXData.VolumeGroup FadeGroup;

	public float FadeGroupDuration;

	public float DesiredFadeGroupVolume;

	public bool AwarenessCanLook;

	public bool AwarenessCanBeLookedAt;

	private bool syncCharacter;

	private bool DoLerpOverTime;

	private bool WaitOnAction;

	private bool AnimationTriggered;

	private float TeleportStartTime;

	private Vector3 TeleportStartPos;

	private Quaternion TeleportStartRot;

	private AnimationCullingType mPreviousCullType;

	public static GameObject debugsphere;

	public static GameObject debugcube;

	private ActionDelegate ActionFunc;

	private static bool Skipping;

	public SPAction()
	{
		AnimSpeed = 1f;
		AnimLooped = false;
		AlwaysAnim = false;
		WeaponID = string.Empty;
		movementType = SPMovementType.Blend;
		ContinuePlaying = false;
		SetTether = false;
		BlendTime = 0.25f;
		DelayTime = 0f;
		SoundFPBank = null;
		SoundBank = null;
		PlayFPFunction = null;
		PlayFunction = null;
		PlayFP3D = true;
		Play3D = true;
		OnOff = true;
		GOOnOff = true;
		LaserOnOff = true;
		BlobShadowOnOff = true;
		CommandText = string.Empty;
		ActionTime = 1f;
		Value = 0f;
		triggerBranch = false;
		IsSelected = true;
		PlayVolume = 1f;
		PlayVolumeFP = 1f;
		FadeDuration = 1f;
		FadeDurationFP = 1f;
		DesiredVolume = 1f;
		DesiredVolumeFP = 1f;
		FadeFunction = string.Empty;
		FadeFPFunction = string.Empty;
		FadeGroupDuration = 1f;
		DesiredFadeGroupVolume = 1f;
		Reset();
	}

	private ActionDelegate GetActionFunc(SPActionType type)
	{
		ActionDelegate result = null;
		switch (type)
		{
		case SPActionType.PlayAnim:
		case SPActionType.ActorComplete:
		case SPActionType.PlayCameraDependentAnim:
			result = Action_PlayCameraDependentAnim;
			break;
		case SPActionType.SetFog:
			result = Action_SetFog;
			break;
		case SPActionType.Teleport:
		case SPActionType.MoveTo:
			result = Action_MoveTo;
			break;
		case SPActionType.PlaySound:
			result = Action_PlaySound;
			break;
		case SPActionType.StopSound:
			result = Action_StopSound;
			break;
		case SPActionType.FadeInSound:
			result = Action_FadeInSound;
			break;
		case SPActionType.FadeOutSound:
			result = Action_FadeOutSound;
			break;
		case SPActionType.FadeSound:
			result = Action_FadeSound;
			break;
		case SPActionType.SetLineOfSight:
			result = Action_LOS;
			break;
		case SPActionType.PoseCommand:
			result = Action_PoseCommand;
			break;
		case SPActionType.Selectability:
			result = Action_Selectability;
			break;
		case SPActionType.CameraLook:
			result = Action_CameraLook;
			break;
		case SPActionType.CameraToActor:
			result = Action_CameraToActor;
			break;
		case SPActionType.CameraFinished:
			result = Action_CameraFinished;
			break;
		case SPActionType.ShowHide:
			result = Action_ShowHideActor;
			break;
		case SPActionType.ForceBranch:
			result = Action_ForceBranch;
			break;
		case SPActionType.Response:
			result = Action_Response;
			break;
		case SPActionType.BeginCinematic:
			result = Action_BeginCinematic;
			break;
		case SPActionType.EndCinematic:
			result = Action_EndCinematic;
			break;
		case SPActionType.Attach:
			result = Action_Attach;
			break;
		case SPActionType.Detach:
			result = Action_Detach;
			break;
		case SPActionType.StartPFX:
			result = Action_StartPFX;
			break;
		case SPActionType.ScriptedSequence:
			result = Action_ScriptedSequence;
			break;
		case SPActionType.CameraToFirstPerson:
			result = Action_CameraToFirstPerson;
			break;
		case SPActionType.CameraToThirdPerson:
			result = Action_CameraToThirdPerson;
			break;
		case SPActionType.EnableDisable:
			result = Action_EnableDisable;
			break;
		case SPActionType.EnableDisableBlobShadow:
			result = Action_EnableDisableBlobShadow;
			break;
		case SPActionType.ConfigureHUD:
			result = Action_ConfigureHUD;
			break;
		case SPActionType.HealUnit:
			result = Action_HealUnit;
			break;
		case SPActionType.EnableDisableAwareness:
			result = Action_EnableDisableAwareness;
			break;
		case SPActionType.FadeSoundGroup:
			result = Action_FadeSoundGroup;
			break;
		case SPActionType.ClearArea:
			result = Action_ClearArea;
			break;
		case SPActionType.PlayBodyDropSfx:
			result = Action_PlayBodyDropSfx;
			break;
		case SPActionType.ScreenTransition:
			result = Action_ScreenTransition;
			break;
		case SPActionType.WeatherSwitch:
			result = Action_WeatherSwitch;
			break;
		case SPActionType.ScreenStaticEffect:
			result = Action_PlayScreenStaticEffect;
			break;
		case SPActionType.SetActorStance:
			result = Action_SetActorStance;
			break;
		case SPActionType.DropActorAttachment:
			result = Action_DropActorAttachment;
			break;
		case SPActionType.FirstPersonObjectsSwitch:
			result = Action_FirstPersonObjectsSwitch;
			break;
		case SPActionType.FirstPersonTransform:
			result = Action_FirstPersonTransform;
			break;
		case SPActionType.SetFppAnimState:
			result = Action_SetFppAnimState;
			break;
		case SPActionType.ResumeNormalTime:
			result = Action_ResumeNormalTime;
			break;
		default:
			Debug.Log("Couldn't find action function for the setpiece");
			break;
		}
		return result;
	}

	public void SetUp(SPActionType type, SetPieceModule moduleRef)
	{
		Type = type;
		ActionFunc = GetActionFunc(Type);
		mModuleRef = moduleRef;
	}

	public void SetUp(SetPieceModule moduleRef)
	{
		mModuleRef = moduleRef;
	}

	public void Reset()
	{
		TeleportStartTime = -1f;
		ActionDone = false;
		AnimationTriggered = false;
	}

	public ReturnCode DoAction(float StatementTime)
	{
		WaitOnAction = false;
		DoLerpOverTime = false;
		syncCharacter = false;
		if (!ActionDone)
		{
			if (StatementTime < DelayTime)
			{
				WaitOnAction = true;
			}
			else
			{
				if (ActionFunc == null)
				{
					ActionFunc = GetActionFunc(Type);
				}
				if (ActionFunc != null)
				{
					ActionFunc();
				}
			}
			if (DoLerpOverTime)
			{
				Update_LerpOverTime();
			}
		}
		SyncActor();
		if (WaitOnAction)
		{
			return ReturnCode.Wait;
		}
		if (triggerBranch)
		{
			return ReturnCode.Branch;
		}
		return ReturnCode.Done;
	}

	public void Skip()
	{
		Skipping = true;
		if (!ActionDone)
		{
			if (ActionFunc == null)
			{
				ActionFunc = GetActionFunc(Type);
			}
			if (ActionFunc != null)
			{
				ActionFunc();
			}
		}
		Skipping = false;
	}

	public void PostSkip()
	{
		if (IsSoundAction(Type))
		{
			Action_StopSound();
		}
		SyncActor();
	}

	private bool IsSoundAction(SPActionType type)
	{
		if (Type == SPActionType.PlaySound || Type == SPActionType.FadeInSound || Type == SPActionType.FadeSound)
		{
			return true;
		}
		return false;
	}

	private void SyncActor()
	{
		if (syncCharacter && Actor.ActorRef != null)
		{
			Actor.ActorRef.transform.rotation = Actor.ObjectTransform.rotation;
			Actor.ActorRef.SetPosition(Actor.ObjectTransform.position);
		}
	}

	private void Update_LerpOverTime()
	{
		WaitOnAction = true;
		if (Actor == null || Actor.ActorRef == null)
		{
			WaitOnAction = false;
			return;
		}
		if (TeleportStartTime == -1f)
		{
			TeleportStartTime = WorldHelper.ThisFrameTime;
			TeleportStartPos = Actor.ObjectTransform.position;
			TeleportStartRot = Actor.ObjectTransform.rotation;
		}
		float num = (WorldHelper.ThisFrameTime - TeleportStartTime) * (1f / ActionTime);
		if (Target != null && Target.ObjectTransform != null)
		{
			if (num >= 1f)
			{
				num = 1f;
				Actor.ObjectTransform.rotation = FixUpNodeRotation(Target.ObjectTransform.rotation);
				Actor.ObjectTransform.position = Target.ObjectTransform.position;
			}
			else
			{
				float num2 = AnimDirector.Blender.Evaluate(0f, 1f, num, RawAnimation.EaseBegin(Easing), RawAnimation.EaseEnd(Easing));
				Quaternion quaternion = FixUpNodeRotation(Target.ObjectTransform.rotation);
				Quaternion quaternion2 = Quaternion.Slerp(TeleportStartRot, quaternion, num2);
				if (endOnAxis)
				{
					Vector3 vector = Quaternion.Inverse(TeleportStartRot) * (Target.ObjectTransform.position - TeleportStartPos);
					Vector3 vector2 = Vector3.Lerp(TeleportStartPos, Target.ObjectTransform.position, num2);
					Vector3 vector3 = vector2 + quaternion2 * vector * (1f - num2);
					Actor.ObjectTransform.position = vector2 - (vector3 - Target.ObjectTransform.position);
					Actor.ObjectTransform.rotation = quaternion2;
				}
				else
				{
					Vector3 vector4 = Quaternion.Inverse(quaternion) * (TeleportStartPos - Target.ObjectTransform.position);
					Vector3 vector5 = Vector3.Lerp(TeleportStartPos, Target.ObjectTransform.position, num2);
					Vector3 vector6 = vector5 + quaternion2 * vector4 * num2;
					Actor.ObjectTransform.position = vector5 - (vector6 - TeleportStartPos);
					Actor.ObjectTransform.rotation = quaternion2;
				}
			}
			syncCharacter = true;
		}
		if (num >= 1f)
		{
			WaitOnAction = false;
			ActionDone = true;
			TeleportStartTime = -1f;
			if (Type == SPActionType.ActorComplete)
			{
				Actor.SetFinished(OnOff, ContinuePlaying, SetTether);
			}
		}
	}

	private void Action_PlayAnim()
	{
		if (Skipping)
		{
			if (Type == SPActionType.ActorComplete && Actor.AnimDirector != null && !ContinuePlaying)
			{
				Actor.AnimDirector.EnableCategory(Actor.AnimDirector.GetCategoryHandle("SetPiece"), false, 0f, Easing);
			}
			if (Target.Index <= 0)
			{
				return;
			}
			Actor.ObjectTransform.rotation = FixUpNodeRotation(Target.ObjectTransform.rotation);
			Actor.ObjectTransform.position = Target.ObjectTransform.position;
			syncCharacter = true;
			ActionDone = true;
			if (Type == SPActionType.ActorComplete)
			{
				Actor.SetFinished(OnOff, ContinuePlaying, SetTether);
				if (AlwaysAnim)
				{
					RestorePreviousCullType();
				}
			}
			return;
		}
		endOnAxis = Type == SPActionType.ActorComplete && !ContinuePlaying;
		if (!AnimationTriggered)
		{
			if (Type == SPActionType.ActorComplete)
			{
				if (Actor.AnimDirector != null)
				{
					if (!ContinuePlaying)
					{
						Actor.AnimDirector.EnableCategory(Actor.AnimDirector.GetCategoryHandle("SetPiece"), false, BlendTime, Easing);
					}
				}
				else
				{
					Actor.PlayAnimation(Clip, 1f, true, BlendTime, Easing);
				}
			}
			else
			{
				Actor.PlayAnimation(Clip, AnimSpeed, AnimLooped, BlendTime, Easing);
				if (AlwaysAnim)
				{
					SetToAlwaysAnimate();
				}
			}
			if (Target.Index > 0)
			{
				if (Actor.ActorRef != null)
				{
					PoseModuleSharedData poseModuleSharedData = Actor.ActorRef.poseModuleSharedData;
					if (endOnAxis)
					{
						poseModuleSharedData.blend = 1f;
						poseModuleSharedData.onAxisTrans.position = Target.ObjectTransform.position;
						poseModuleSharedData.onAxisTrans.rotation = FixUpNodeRotation(Target.ObjectTransform.rotation);
						poseModuleSharedData.idealOnAxisPos = poseModuleSharedData.onAxisTrans.position;
						poseModuleSharedData.idealOnAxisRot = poseModuleSharedData.onAxisTrans.rotation;
						Actor.ActorRef.SetPosition(poseModuleSharedData.onAxisTrans.position);
						Actor.ActorRef.transform.rotation = poseModuleSharedData.onAxisTrans.rotation;
						poseModuleSharedData.BlendOntoAxis(BlendTime, RawAnimation.EaseBegin(Easing), RawAnimation.EaseEnd(Easing));
						ActionDone = true;
						Actor.SetFinished(OnOff, ContinuePlaying, SetTether);
						return;
					}
					if (poseModuleSharedData.blend >= 0f)
					{
						if (Actor.linkTransform != null)
						{
							poseModuleSharedData.idealOnAxisPos = Actor.linkTransform.position;
							poseModuleSharedData.idealOnAxisRot = FixUpNodeRotation(Actor.linkTransform.rotation);
						}
						else
						{
							poseModuleSharedData.idealOnAxisPos = poseModuleSharedData.onAxisTrans.position;
							poseModuleSharedData.idealOnAxisRot = poseModuleSharedData.onAxisTrans.rotation;
						}
						poseModuleSharedData.offAxisPos = Target.ObjectTransform.position;
						poseModuleSharedData.offAxisRot = FixUpNodeRotation(Target.ObjectTransform.rotation);
						poseModuleSharedData.BlendOffAxis(BlendTime, RawAnimation.EaseBegin(Easing), RawAnimation.EaseEnd(Easing), false);
						ActionDone = true;
						return;
					}
					Actor.ActorRef.baseCharacter.EnableNavMesh(false);
				}
				if (BlendTime > 0f)
				{
					TeleportStartTime = -1f;
					ActionTime = BlendTime;
					DoLerpOverTime = true;
				}
				else
				{
					Actor.ObjectTransform.rotation = FixUpNodeRotation(Target.ObjectTransform.rotation);
					Actor.ObjectTransform.position = Target.ObjectTransform.position;
					syncCharacter = true;
					ActionDone = true;
					if (Type == SPActionType.ActorComplete)
					{
						Actor.SetFinished(OnOff, ContinuePlaying, SetTether);
						if (AlwaysAnim)
						{
							RestorePreviousCullType();
						}
					}
				}
			}
			else
			{
				ActionDone = true;
				if (Type == SPActionType.ActorComplete)
				{
					Actor.SetFinished(OnOff, ContinuePlaying, SetTether);
					if (AlwaysAnim)
					{
						RestorePreviousCullType();
					}
				}
			}
			AnimationTriggered = true;
		}
		else
		{
			DoLerpOverTime = true;
		}
	}

	private void Action_PlayCameraDependentAnim()
	{
		Action_PlayAnim();
		if (Type != SPActionType.ActorComplete && GameController.Instance != null && Actor.ActorRef != null && Actor.ActorRef.firstThirdPersonWidget != null)
		{
			bool flag = false;
			if (Target.ObjectTransform != null)
			{
				Quaternion rotation = Target.ObjectTransform.rotation;
				Target.ObjectTransform.rotation = FixUpNodeRotation(Target.ObjectTransform.rotation);
				flag = Actor.ActorRef.firstThirdPersonWidget.Reset(Actor.ActorRef, FPSClip, AnimSpeed, Target.ObjectTransform.position, Target.ObjectTransform.rotation, WeaponID, DontClearDepthBuffer);
				Target.ObjectTransform.rotation = rotation;
			}
			else if (GameController.Instance.IsFirstPerson && Actor.ActorRef.realCharacter.FirstPersonCamera != null)
			{
				flag = Actor.ActorRef.firstThirdPersonWidget.Reset(Actor.ActorRef, FPSClip, AnimSpeed, Actor.ActorRef.transform.position, Actor.ActorRef.realCharacter.FirstPersonCamera.RotationOnlyY, WeaponID, DontClearDepthBuffer);
			}
			if (!flag && GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.ident == Actor.ActorRef.ident && GameController.Instance.IsFirstPerson && !Actor.ActorRef.tasks.IsRunningTask(typeof(TaskCarry)))
			{
				GameController.Instance.ExitFirstPerson(true, OnOff);
			}
		}
	}

	private void Action_MoveTo()
	{
		switch (movementType)
		{
		case SPMovementType.Teleport:
			if ((bool)Actor.ActorRef)
			{
				Actor.ActorRef.baseCharacter.EnableNavMesh(false);
			}
			Actor.ObjectTransform.rotation = FixUpNodeRotation(Target.ObjectTransform.rotation);
			Actor.ObjectTransform.position = Target.ObjectTransform.position;
			syncCharacter = true;
			ActionDone = true;
			break;
		case SPMovementType.Blend:
			if ((bool)Actor.ActorRef)
			{
				Actor.ActorRef.poseModuleSharedData.offAxisPos = Actor.ActorRef.poseModuleSharedData.onAxisTrans.position;
				Actor.ActorRef.poseModuleSharedData.offAxisRot = Actor.ActorRef.poseModuleSharedData.onAxisTrans.rotation;
				Actor.ActorRef.baseCharacter.EnableNavMesh(false);
			}
			DoLerpOverTime = true;
			break;
		case SPMovementType.Walk:
		case SPMovementType.Run:
			if ((bool)Actor.ActorRef)
			{
				if (Actor.ActorRef.baseCharacter.IsUsingNavMesh())
				{
					InheritableMovementParams inheritableMovementParams = new InheritableMovementParams((movementType != SPMovementType.Walk) ? BaseCharacter.MovementStyle.Run : BaseCharacter.MovementStyle.Walk, Target.ObjectTransform.position);
					inheritableMovementParams.FinalLookDirection = Target.ObjectTransform.forward;
					new TaskRouteTo(Actor.ActorRef.tasks, TaskManager.Priority.REACTIVE, Task.Config.Default, inheritableMovementParams);
					ActionDone = true;
				}
				else
				{
					Actor.ActorRef.baseCharacter.EnableNavMesh(true);
				}
			}
			else
			{
				DoLerpOverTime = true;
			}
			break;
		}
	}

	private void Action_SetFog()
	{
		if (OnOff)
		{
			if (OnOff)
			{
				CameraManager instance = CameraManager.Instance;
				if ((bool)instance)
				{
					instance.FogOverride = Value;
				}
				else
				{
					RenderSettings.fogDensity = Value;
				}
			}
			else
			{
				CameraManager instance2 = CameraManager.Instance;
				if ((bool)instance2)
				{
					instance2.ResetFog();
				}
				else
				{
					RenderSettings.fogDensity = 0f;
				}
			}
		}
		ActionDone = true;
	}

	private void Action_PlaySound()
	{
		if (Skipping)
		{
			return;
		}
		GameObject go = null;
		if (Target != null && Target.ObjectTransform != null)
		{
			go = Target.ObjectTransform.gameObject;
		}
		bool flag = true;
		SoundFXData sfxData = null;
		if (IsInACutscene() && IsFirstPersonActorPlayerControlled() && SoundFPBank != null)
		{
			flag = false;
			PlaySoundFx.PlaySfxHelper(go, SoundFPBank, PlayFPFunction, PlayFP3D, PlayVolumeFP, out sfxData);
		}
		if (flag && SoundBank != null)
		{
			PlaySoundFx.PlaySfxHelper(go, SoundBank, PlayFunction, Play3D, PlayVolume, out sfxData);
		}
		if (GameController.Instance != null && !HUDOnOff && sfxData.m_audioSourceData != null && sfxData.m_audioSourceData.Count > 0 && sfxData.m_audioSourceData[0] != null)
		{
			string name = sfxData.m_audioSourceData[0].name;
			if (name.StartsWith("A_"))
			{
				name = "S_" + name.Substring(2, name.Length - 2);
				BlackBarsController.Instance.DisplaySubtitle(name, sfxData.m_audioSourceData[0].length);
			}
		}
		ActionDone = true;
	}

	private void Action_StopSound()
	{
		GameObject go = null;
		if (Target != null && Target.ObjectTransform != null)
		{
			go = Target.ObjectTransform.gameObject;
		}
		if (GameController.Instance != null)
		{
			bool flag = true;
			if (IsInACutscene() && IsFirstPersonActorPlayerControlled() && SoundFPBank != null)
			{
				flag = false;
				if (PlayFPFunction != string.Empty)
				{
					PlaySoundFx.StopSfxHelper(go, SoundFPBank, PlayFPFunction, PlayFP3D);
				}
				else if (FadeFPFunction != string.Empty)
				{
					PlaySoundFx.StopSfxHelper(go, SoundFPBank, FadeFPFunction, PlayFP3D);
				}
			}
			if (flag && SoundBank != null)
			{
				if (PlayFunction != string.Empty)
				{
					PlaySoundFx.StopSfxHelper(go, SoundBank, PlayFunction, Play3D);
				}
				else if (FadeFunction != string.Empty)
				{
					PlaySoundFx.StopSfxHelper(go, SoundBank, FadeFunction, Play3D);
				}
			}
		}
		ActionDone = true;
	}

	private void Action_FadeSound()
	{
		if (Skipping)
		{
			return;
		}
		GameObject go = null;
		if (Target != null && Target.ObjectTransform != null)
		{
			go = Target.ObjectTransform.gameObject;
		}
		if (GameController.Instance != null)
		{
			bool flag = true;
			if (IsInACutscene() && IsFirstPersonActorPlayerControlled() && SoundFPBank != null)
			{
				flag = false;
				FadeSoundFx.FadeSfxHelper(go, SoundFPBank, FadeFPFunction, PlayFP3D, false, FadeDurationFP, DesiredVolumeFP, false);
			}
			if (flag && SoundBank != null)
			{
				FadeSoundFx.FadeSfxHelper(go, SoundBank, FadeFunction, Play3D, false, FadeDuration, DesiredVolume, false);
			}
		}
		ActionDone = true;
	}

	private void Action_FadeInSound()
	{
		if (Skipping)
		{
			return;
		}
		GameObject go = null;
		if (Target != null && Target.ObjectTransform != null)
		{
			go = Target.ObjectTransform.gameObject;
		}
		if (GameController.Instance != null)
		{
			bool flag = true;
			if (IsInACutscene() && IsFirstPersonActorPlayerControlled() && SoundFPBank != null)
			{
				flag = false;
				FadeSoundFx.FadeSfxHelper(go, SoundFPBank, FadeFPFunction, PlayFP3D, true, FadeDurationFP, DesiredVolumeFP, false);
			}
			if (flag && SoundBank != null)
			{
				FadeSoundFx.FadeSfxHelper(go, SoundBank, FadeFunction, Play3D, true, FadeDuration, DesiredVolume, false);
			}
		}
		ActionDone = true;
	}

	private void Action_FadeOutSound()
	{
		if (Skipping)
		{
			return;
		}
		GameObject go = null;
		if (Target != null && Target.ObjectTransform != null)
		{
			go = Target.ObjectTransform.gameObject;
		}
		if (GameController.Instance != null)
		{
			bool flag = true;
			if (IsInACutscene() && IsFirstPersonActorPlayerControlled() && SoundFPBank != null)
			{
				flag = false;
				FadeSoundFx.FadeSfxHelper(go, SoundFPBank, FadeFPFunction, PlayFP3D, false, FadeDurationFP, 0f, true);
			}
			if (flag && SoundBank != null)
			{
				FadeSoundFx.FadeSfxHelper(go, SoundBank, FadeFunction, Play3D, false, FadeDuration, 0f, true);
			}
		}
		ActionDone = true;
	}

	private void Action_FadeSoundGroup()
	{
		if (!Skipping)
		{
			VolumeGroupFader volumeGroupFader = SoundManager.Instance.gameObject.AddComponent<VolumeGroupFader>();
			volumeGroupFader.TimeToFade = FadeGroupDuration;
			volumeGroupFader.VolumeGroupToFade = FadeGroup;
			volumeGroupFader.DesiredVolume = DesiredFadeGroupVolume;
			ActionDone = true;
		}
	}

	private void Action_PlayBodyDropSfx()
	{
		if (!Skipping)
		{
			GameObject gameObject = null;
			if (Target != null && Target.ObjectTransform != null)
			{
				gameObject = Target.ObjectTransform.gameObject;
			}
			if (gameObject != null)
			{
				SoundManager.Instance.PlayBodyFallSfx(gameObject.transform.position, gameObject);
			}
			ActionDone = true;
		}
	}

	private void Action_ScreenTransition()
	{
		if (Skipping)
		{
			return;
		}
		InteractionsManager instance = InteractionsManager.Instance;
		if (instance != null)
		{
			if ((int)Value == 0)
			{
				instance.TransitionToSolid(ActionTime);
			}
			else
			{
				instance.TransitionFromSolid(ActionTime);
			}
		}
		ActionDone = true;
	}

	private void Action_PlayScreenStaticEffect()
	{
		if (!Skipping)
		{
			AnimatedScreenBackground.Instance.Deactivate();
			ActionDone = true;
		}
	}

	private void Action_WeatherSwitch()
	{
		CloseSnow.CutSceneWeatherSwitch(OnOff);
		ActionDone = true;
	}

	private void Action_ClearArea()
	{
		if (!Skipping)
		{
			GameObject gameObject = new GameObject("AreaClearer");
			gameObject.AddComponent<AreaClearer>().Initialise(Value, ActionTime);
			gameObject.transform.position = Target.ObjectTransform.position;
			ActionDone = true;
		}
	}

	private void Action_PoseCommand()
	{
		if (Actor.ActorRef != null)
		{
			Actor.ActorRef.Command(CommandText);
		}
		ActionDone = true;
	}

	private void Action_LOS()
	{
		if (Actor.ActorRef != null)
		{
			Actor.ActorRef.awareness.CanBeLookedAt = OnOff;
		}
		ActionDone = true;
	}

	private void Action_Selectability()
	{
		if (Actor != null && Actor.ActorRef != null && Actor.ActorRef.behaviour.PlayerControlled)
		{
			Actor.ActorRef.realCharacter.SetSelectable(OnOff, HUDOnOff, true);
			Actor.ActorRef.realCharacter.UseLaserSite(LaserOnOff);
			if (GameplayController.instance.IsSelected(Actor.ActorRef))
			{
				if (!IsSelected)
				{
					GameplayController.instance.RemoveFromSelected(Actor.ActorRef);
				}
			}
			else if (IsSelected)
			{
				GameplayController.instance.AddToSelected(Actor.ActorRef);
			}
		}
		ActionDone = true;
	}

	private void Action_CameraLook()
	{
		if ((bool)CameraManager.Instance)
		{
			CameraController playCameraController = CameraManager.Instance.PlayCameraController;
			if (CameraBase != null && playCameraController != null)
			{
				if (Target != null)
				{
				}
				if (CanControlCamera())
				{
					Camera component = CameraBase.gameObject.GetComponent<Camera>();
					if (component != null)
					{
						CameraBase.Fov = component.fieldOfView;
					}
					playCameraController.BlendTo(new CameraTransitionData(CameraBase, TweenFunctions.TweenType.easeInOutCubic, ActionTime));
					InputManager instance = InputManager.Instance;
					if ((bool)instance)
					{
						instance.SetForCutscene();
					}
				}
			}
		}
		ActionDone = true;
	}

	private void Action_CameraToActor()
	{
		if ((bool)CameraManager.Instance)
		{
			CameraController playCameraController = CameraManager.Instance.PlayCameraController;
			CameraBase = Actor.ObjectTransform.GetComponentInChildren<CameraBase>();
			if (CameraBase != null && playCameraController != null && CanControlCamera())
			{
				playCameraController.BlendTo(new CameraTransitionData(CameraBase, TweenFunctions.TweenType.easeInOutCubic, ActionTime));
				InputManager instance = InputManager.Instance;
				if ((bool)instance)
				{
					instance.SetForCutscene();
				}
			}
		}
		ActionDone = true;
	}

	private void Action_CameraFinished()
	{
		if ((bool)CameraManager.Instance)
		{
			CameraController playCameraController = CameraManager.Instance.PlayCameraController;
			if (playCameraController != null)
			{
				if (CanControlCamera())
				{
					playCameraController.RestoreCameraToDefault(ActionTime);
					GameController.Instance.SwitchToPlayCamera();
					GameController.Instance.ExitFirstPerson(OnOff);
				}
				PlayCameraInterface playCameraInterface = playCameraController.StartCamera as PlayCameraInterface;
				if (playCameraInterface != null)
				{
					playCameraInterface.AllowInput(true);
					if (Value > 0f)
					{
						playCameraInterface.SnapToTarget(Actor.ObjectTransform);
					}
				}
			}
			InputManager instance = InputManager.Instance;
			if ((bool)instance)
			{
				instance.SetForGamplay();
			}
		}
		ActionDone = true;
	}

	private void Action_CameraToFirstPerson()
	{
		ActionDone = true;
		if (!(GameController.Instance == null) && !GameController.Instance.IsFirstPerson)
		{
			if (OnOff)
			{
				GameController.Instance.SwitchToLastFirstPerson(OnOff);
			}
			else if (Actor != null)
			{
				GameController.Instance.SwitchToFirstPerson(Actor.ActorRef, OnOff);
			}
		}
	}

	private void Action_CameraToThirdPerson()
	{
		ActionDone = true;
		if (!(GameController.Instance == null) && GameController.Instance.IsFirstPerson)
		{
			GameController.Instance.ExitFirstPerson(OnOff);
		}
	}

	private void Action_ConfigureHUD()
	{
		ActionDone = true;
		if (GameController.Instance == null)
		{
			return;
		}
		HudStateController instance = HudStateController.Instance;
		if (instance != null)
		{
			if (IsLockedToCurrentCharacter)
			{
				instance.StoreStateAndHide();
				HudBlipIcon.SwitchOffForCutscene();
			}
			else
			{
				HudBlipIcon.SwitchOnAfterCutscene();
				instance.RestoreState();
			}
		}
		if (GameController.Instance.IsFirstPerson)
		{
			GameController.Instance.ZoomInAvailable = false;
			if (GameController.Instance.IsLockedToFirstPerson)
			{
				GameController.Instance.ZoomOutAvailable = false;
			}
			else
			{
				GameController.Instance.ZoomOutAvailable = IsZoomOutAvailable;
			}
		}
		else
		{
			GameController.Instance.ZoomOutAvailable = false;
			if (GameController.Instance.AllowFirstPersonAtAnyPoint)
			{
				GameController.Instance.ZoomInAvailable = IsZoomInAvailable;
			}
			else
			{
				GameController.Instance.ZoomInAvailable = false;
			}
		}
	}

	private void Action_ShowHideActor()
	{
		Transform objectTransform = Actor.ObjectTransform;
		if ((bool)objectTransform)
		{
			Renderer[] componentsInChildren = objectTransform.GetComponentsInChildren<Renderer>(true);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = OnOff;
			}
		}
		ActionDone = true;
	}

	private void Action_ForceBranch()
	{
		triggerBranch = true;
		ActionDone = true;
	}

	private void Action_Response()
	{
		if (Actor != null && Actor.ObjectTransform != null && Actor.ObjectTransform.gameObject != null)
		{
			Container.SendMessage(Actor.ObjectTransform.gameObject, CommandText);
		}
		ActionDone = true;
	}

	private void Action_ScriptedSequence()
	{
		if (Actor != null && Actor.ObjectTransform != null && Actor.ObjectTransform.gameObject != null)
		{
			ScriptedSequence component = Actor.ObjectTransform.gameObject.GetComponent<ScriptedSequence>();
			if (component != null)
			{
				component.StartSequence();
			}
		}
		ActionDone = true;
	}

	private void Action_BeginCinematic()
	{
		CinematicHelper.Begin(true, true, false);
		ActionDone = true;
	}

	private void Action_EndCinematic()
	{
		CinematicHelper.End(true, true, true, true, false);
		ActionDone = true;
	}

	private void Action_ResumeNormalTime()
	{
		if (TimeManager.instance != null)
		{
			TimeManager.instance.ResumeNormalTimeImmediate();
		}
		ActionDone = true;
	}

	private void Action_Attach()
	{
		if (Actor != null && Actor.ActorRef != null && Prop != null && Prop.ObjectTransform != null && Actor.ActorRef.model != null)
		{
			Transform parent = Actor.ActorRef.model.transform.Find(AttachPoint);
			Prop.ObjectTransform.parent = parent;
			Prop.ObjectTransform.localPosition = Vector3.zero;
			Prop.ObjectTransform.localRotation = Quaternion.identity;
		}
		ActionDone = true;
	}

	private void Action_Detach()
	{
		if (Prop != null && Prop.ObjectTransform != null)
		{
			Prop.ObjectTransform.parent = null;
			if (DestroyOnDetach)
			{
				UnityEngine.Object.Destroy(Prop.ObjectTransform.gameObject);
			}
		}
		ActionDone = true;
	}

	private void Action_StartPFX()
	{
		if (ParticleFX != null)
		{
			ParticleSystem component = ParticleFX.GetComponent<ParticleSystem>();
			ParticleEmitter component2 = ParticleFX.GetComponent<ParticleEmitter>();
			Animation component3 = ParticleFX.GetComponent<Animation>();
			if (component != null)
			{
				component.Play();
			}
			else if (component2 != null)
			{
				component2.Emit();
			}
			else if (component3 != null)
			{
				component3.Play();
			}
		}
		ActionDone = true;
	}

	private void Action_EnableDisable()
	{
		Transform objectTransform = Actor.ObjectTransform;
		if ((bool)objectTransform)
		{
			Transform[] componentsInChildren = objectTransform.GetComponentsInChildren<Transform>(true);
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				transform.gameObject.SetActive(GOOnOff);
			}
		}
		ActionDone = true;
	}

	private void Action_EnableDisableBlobShadow()
	{
		if (Actor != null && Actor.ActorRef != null)
		{
			Actor.ActorRef.baseCharacter.Command((!BlobShadowOnOff) ? "DisableShadow" : "EnableShadow");
		}
		ActionDone = true;
	}

	private void Action_HealUnit()
	{
		if (Actor != null)
		{
			if (HealAmount == HealthComponent.HealAmount.Completely)
			{
				Actor.ActorRef.health.ModifyHealthOverTime(Actor.ActorRef.gameObject, Actor.ActorRef.health.HealthMax - Actor.ActorRef.health.Health, "Heal", Actor.ActorRef.transform.forward, false, 4f, true, 6f, HealDelayTime);
				EventHub.Instance.Report(new Events.CharacterHealed(Actor.ActorRef.EventActor()));
			}
			else if (HealAmount == HealthComponent.HealAmount.ToMortallyWounded)
			{
				Actor.ActorRef.health.ModifyHealth(Actor.ActorRef.gameObject, Actor.ActorRef.health.MortallyWoundedThreshold + 1f - Actor.ActorRef.health.Health, "Heal", Actor.ActorRef.transform.forward, false);
			}
			if (CommonHudController.Instance != null)
			{
				CommonHudController.Instance.HUDInvulnerabilityEffect = true;
			}
		}
		ActionDone = true;
	}

	private void Action_EnableDisableAwareness()
	{
		if (Actor != null && Actor.ActorRef != null)
		{
			Actor.ActorRef.awareness.canLook = AwarenessCanLook;
			Actor.ActorRef.awareness.CanBeLookedAt = AwarenessCanBeLookedAt;
		}
		ActionDone = true;
	}

	private void Action_SetActorStance()
	{
		if (Actor != null && Actor.ActorRef != null)
		{
			switch ((SPStanceType)(int)Value)
			{
			case SPStanceType.Crouch:
				Actor.ActorRef.baseCharacter.Crouch();
				break;
			case SPStanceType.Stand:
				Actor.ActorRef.baseCharacter.Stand();
				break;
			}
		}
		ActionDone = true;
	}

	private void Action_FirstPersonObjectsSwitch()
	{
		if (GOOnOff)
		{
			FirstPersonOnly.ShowHideFPPObjects(OnOff);
		}
		else
		{
			BuildingWithInterior.ForceShowHideRoofs(OnOff);
		}
		ActionDone = true;
	}

	private void Action_DropActorAttachment()
	{
		if (Actor.ActorRef != null && Actor.ActorRef.weapon != null)
		{
			Actor.ActorRef.weapon.Drop();
		}
		ActionDone = true;
	}

	private void Action_FirstPersonTransform()
	{
		if (Actor != null)
		{
			mModuleRef.FirstPersonTransform = Actor.ObjectTransform;
		}
	}

	private void Action_SetFppAnimState()
	{
		if (Actor != null && Actor.ActorRef != null)
		{
			switch ((SPFppAnimStateType)(int)Value)
			{
			case SPFppAnimStateType.HoldFinalPose:
				Actor.ActorRef.firstThirdPersonWidget.HoldAnimFinishPose();
				break;
			case SPFppAnimStateType.ClearHold:
				Actor.ActorRef.firstThirdPersonWidget.ClearAnimFinishPoseHold();
				break;
			}
		}
		ActionDone = true;
	}

	private Quaternion FixUpNodeRotation(Quaternion quat)
	{
		return WorldHelper.UfM_Rotation(quat);
	}

	private void SetToAlwaysAnimate()
	{
		if (Actor.AnimDirector != null && Actor.AnimDirector.AnimationPlayer != null)
		{
			mPreviousCullType = Actor.AnimDirector.AnimationPlayer.cullingType;
			Actor.AnimDirector.AnimationPlayer.cullingType = AnimationCullingType.AlwaysAnimate;
		}
		else if (Actor.ActorAnimation != null)
		{
			mPreviousCullType = Actor.ActorAnimation.cullingType;
			Actor.ActorAnimation.cullingType = AnimationCullingType.AlwaysAnimate;
		}
	}

	private void RestorePreviousCullType()
	{
		if (Actor.AnimDirector != null && Actor.AnimDirector.AnimationPlayer != null)
		{
			Actor.AnimDirector.AnimationPlayer.cullingType = mPreviousCullType;
		}
		else if (Actor.ActorAnimation != null)
		{
			Actor.ActorAnimation.cullingType = mPreviousCullType;
		}
	}

	private bool IsInACutscene()
	{
		return mModuleRef != null && !mModuleRef.UsePlaygroundMeshes && mModuleRef.TheCamera == null;
	}

	private bool IsFirstPersonActorPlayerControlled()
	{
		return GameController.Instance != null && GameController.Instance.mFirstPersonActor != null && Actor.ActorRef != null && GameController.Instance.mFirstPersonActor.ident == Actor.ActorRef.ident;
	}

	private bool CanControlCamera()
	{
		if (mModuleRef != null)
		{
			if (InteractionsManager.Instance.CanIControlTheCamera(mModuleRef.gameObject, null, Actor.ActorRef))
			{
				return true;
			}
			return false;
		}
		return true;
	}
}
