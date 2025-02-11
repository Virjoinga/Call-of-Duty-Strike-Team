using System.Collections.Generic;
using UnityEngine;

public class PoseModuleSharedData : BaseActorComponent
{
	public enum Modules
	{
		Invalid = 0,
		MoveAim = 1,
		CrouchCover = 2,
		HighCornerCover = 3,
		Carrying = 4,
		Carried = 5,
		Corpse = 6,
		AGR = 7,
		FixedGun = 8,
		SecurityCamera = 9,
		SentryGun = 10,
		RiotShieldNPC = 11,
		RPG = 12,
		Count = 13
	}

	public enum BlendDirection
	{
		None = 0,
		OntoAxis = 1,
		OffAxis = 2
	}

	public enum CommandCode
	{
		Undefined = -1,
		CancelCarried = 0,
		CancelCarrying = 1,
		Idle = 2,
		Stand = 3,
		Crouch = 4,
		AtEase = 5,
		Crawl = 6,
		Walk = 7,
		Saunter = 8,
		Run = 9,
		Shoot = 10,
		StartPointing = 11,
		StopPointing = 12,
		CancelIdle = 13,
		Puppet = 14,
		ResetStand = 15,
		Stealth = 16,
		StealthStand = 17,
		PrimAway = 18,
		PrimOut = 19,
		SecAway = 20,
		SecOut = 21,
		KnifeAway = 22,
		KnifeOut = 23,
		NoAim = 24,
		PopUp = 25,
		Hunch = 26,
		PeekOver = 27,
		PopDown = 28,
		HitchLeft = 29,
		HitchRight = 30,
		StepLeft = 31,
		StepRight = 32,
		ShotFront = 33,
		Melee = 34,
		Ragdoll = 35,
		Bashed = 36,
		MoveToBegin = 37,
		Edge1 = 38,
		Edge2 = 39,
		Edge3 = 40,
		Dive = 41,
		ThrowGrenade = 42,
		CoverInvalid = 43,
		Cancel = 44,
		Count = 45
	}

	private BasePoseModule[] modules = new BasePoseModule[13];

	public bool faceLookDirection;

	public bool restrictAiming;

	public static Dictionary<string, CommandCode> mTranslationTable;

	protected int[] mCategoryHandle;

	protected AnimDirector.ActionHandle[] mActionHandle;

	public float blend;

	public float prevBlend;

	public BlendDirection direction;

	public float blendStartTime;

	public float blendDuration;

	public bool mWasOnScreen = true;

	public bool mAlwaysUpdateOrientation;

	public Vector3 mAimDir;

	public float mAimWeight;

	public float mTargetAimWeight;

	public SegueData mSegueData;

	public float mSegueEndTime;

	public AnimDirector.ActionHandle mSegueToAction;

	private RawAnimation mAnimToStop;

	public float mSegueToStartTime;

	public bool mSegueToDestination;

	public Transform modelTrans;

	public Transform onAxisTrans;

	public Transform offAxisTrans;

	public Transform offAxisTransParent;

	public Transform idealOnAxisTrans;

	private Vector3 internalModelPosition;

	private Modules activeModule;

	public MoveAimDescriptor moveAimDesc;

	private AnimDirector.BlendEasing easeBegin;

	private AnimDirector.BlendEasing easeEnd;

	private bool navAgentOffAxis;

	public Vector3 offAxisPos
	{
		get
		{
			return offAxisTrans.position;
		}
		set
		{
			offAxisTrans.position = value;
		}
	}

	public Quaternion offAxisRot
	{
		get
		{
			return offAxisTrans.rotation;
		}
		set
		{
			offAxisTrans.rotation = value;
		}
	}

	public Vector3 offAxisForward
	{
		get
		{
			return offAxisTrans.forward;
		}
		set
		{
			offAxisTrans.forward = value;
		}
	}

	public Vector3 idealOnAxisPos
	{
		get
		{
			return idealOnAxisTrans.position;
		}
		set
		{
			idealOnAxisTrans.position = value;
		}
	}

	public Quaternion idealOnAxisRot
	{
		get
		{
			return idealOnAxisTrans.rotation;
		}
		set
		{
			idealOnAxisTrans.rotation = value;
		}
	}

	public Vector3 idealOnAxisForward
	{
		get
		{
			return idealOnAxisTrans.forward;
		}
		set
		{
			idealOnAxisTrans.forward = value;
		}
	}

	public Modules ActiveModule
	{
		get
		{
			return activeModule;
		}
	}

	public BasePoseModule GetModule(Modules m)
	{
		return modules[(int)m];
	}

	public void FaceLookDirection()
	{
		faceLookDirection = true;
	}

	public void RestrictAiming()
	{
		restrictAiming = true;
	}

	public void OnDestroy()
	{
		if (onAxisTrans != null)
		{
			Object.Destroy(onAxisTrans.gameObject);
		}
		if (offAxisTrans != null)
		{
			Object.Destroy(offAxisTrans.gameObject);
		}
		if (idealOnAxisTrans != null)
		{
			Object.Destroy(idealOnAxisTrans.gameObject);
		}
		if (offAxisTransParent != null)
		{
			Object.Destroy(offAxisTransParent.gameObject);
		}
	}

	public void Initialise(GameObject model, MoveAimDescriptor mad)
	{
		moveAimDesc = mad;
		modelTrans = model.transform;
		mAnimToStop = null;
		mAimDir = base.transform.forward;
		internalModelPosition = modelTrans.position;
		mAimWeight = 0f;
		mTargetAimWeight = 0f;
		onAxisTrans = SceneNanny.NewGameObject("onAxisTrans").transform;
		offAxisTrans = SceneNanny.NewGameObject("offAxisTrans").transform;
		offAxisTransParent = SceneNanny.NewGameObject("offAxisTransParent").transform;
		idealOnAxisTrans = SceneNanny.NewGameObject("idealOnAxisTrans").transform;
		offAxisTrans.parent = offAxisTransParent;
		onAxisTrans.position = modelTrans.position;
		onAxisTrans.rotation = modelTrans.rotation;
		if (mTranslationTable == null)
		{
			mTranslationTable = new Dictionary<string, CommandCode>();
			mTranslationTable.Add("CancelCarried", CommandCode.CancelCarried);
			mTranslationTable.Add("CancelCarrying", CommandCode.CancelCarrying);
			mTranslationTable.Add("Idle", CommandCode.Idle);
			mTranslationTable.Add("Stand", CommandCode.Stand);
			mTranslationTable.Add("Crouch", CommandCode.Crouch);
			mTranslationTable.Add("AtEase", CommandCode.AtEase);
			mTranslationTable.Add("Crawl", CommandCode.Crawl);
			mTranslationTable.Add("Walk", CommandCode.Walk);
			mTranslationTable.Add("Saunter", CommandCode.Saunter);
			mTranslationTable.Add("Run", CommandCode.Run);
			mTranslationTable.Add("Shoot", CommandCode.Shoot);
			mTranslationTable.Add("Point", CommandCode.StartPointing);
			mTranslationTable.Add("Aim", CommandCode.StopPointing);
			mTranslationTable.Add("CancelIdle", CommandCode.CancelIdle);
			mTranslationTable.Add("Puppet", CommandCode.Puppet);
			mTranslationTable.Add("ResetStand", CommandCode.ResetStand);
			mTranslationTable.Add("Stealth", CommandCode.Stealth);
			mTranslationTable.Add("StealthStand", CommandCode.StealthStand);
			mTranslationTable.Add("PrimAway", CommandCode.PrimAway);
			mTranslationTable.Add("PrimOut", CommandCode.PrimOut);
			mTranslationTable.Add("SecAway", CommandCode.PrimAway);
			mTranslationTable.Add("SecOut", CommandCode.PrimOut);
			mTranslationTable.Add("KnifeAway", CommandCode.PrimAway);
			mTranslationTable.Add("KnifeOut", CommandCode.PrimOut);
			mTranslationTable.Add("PopUp", CommandCode.PopUp);
			mTranslationTable.Add("Hunch", CommandCode.Hunch);
			mTranslationTable.Add("PeekOver", CommandCode.PeekOver);
			mTranslationTable.Add("PopDown", CommandCode.PopDown);
			mTranslationTable.Add("StepLeft", CommandCode.StepLeft);
			mTranslationTable.Add("StepRight", CommandCode.StepRight);
			mTranslationTable.Add("HitchLeft", CommandCode.HitchLeft);
			mTranslationTable.Add("HitchRight", CommandCode.HitchRight);
			mTranslationTable.Add("Explosion", CommandCode.ShotFront);
			mTranslationTable.Add("Claymore", CommandCode.ShotFront);
			mTranslationTable.Add("Grenade", CommandCode.ShotFront);
			mTranslationTable.Add("SmartBomb", CommandCode.ShotFront);
			mTranslationTable.Add("Shot", CommandCode.ShotFront);
			mTranslationTable.Add("ActOfGod", CommandCode.ShotFront);
			mTranslationTable.Add("MeleeDeath", CommandCode.Melee);
			mTranslationTable.Add("Ragdoll", CommandCode.Ragdoll);
			mTranslationTable.Add("NoAim", CommandCode.NoAim);
			mTranslationTable.Add("Bashed", CommandCode.Bashed);
			mTranslationTable.Add("MoveToBegin", CommandCode.MoveToBegin);
			mTranslationTable.Add("Edge1", CommandCode.Edge1);
			mTranslationTable.Add("Edge2", CommandCode.Edge2);
			mTranslationTable.Add("Edge3", CommandCode.Edge3);
			mTranslationTable.Add("Dive", CommandCode.Dive);
			mTranslationTable.Add("ThrowGrenade", CommandCode.ThrowGrenade);
			mTranslationTable.Add("CoverInvalid", CommandCode.CoverInvalid);
		}
		if (mad.CharacterType == CharacterType.AutonomousGroundRobot)
		{
			modules[7] = base.gameObject.AddComponent<AGRModule>().ConnectModule(myActor);
			activeModule = Modules.Invalid;
			SetActiveModule(Modules.AGR);
		}
		else if (mad.CharacterType == CharacterType.SecurityCamera)
		{
			modules[9] = base.gameObject.AddComponent<SecurityCameraModule>().ConnectModule(myActor);
			activeModule = Modules.Invalid;
			SetActiveModule(Modules.SecurityCamera);
		}
		else if (mad.CharacterType == CharacterType.SentryGun)
		{
			modules[10] = base.gameObject.AddComponent<SentryGunModule>().ConnectModule(myActor);
			activeModule = Modules.Invalid;
			SetActiveModule(Modules.SentryGun);
		}
		else if (mad.CharacterType == CharacterType.RiotShieldNPC)
		{
			modules[5] = base.gameObject.AddComponent<CarriedModule>().ConnectModule(myActor);
			modules[6] = base.gameObject.AddComponent<CorpseModule>().ConnectModule(myActor);
			modules[11] = base.gameObject.AddComponent<RiotShieldNPCModule>().ConnectModule(myActor);
			activeModule = Modules.Invalid;
			SetActiveModule(Modules.RiotShieldNPC);
		}
		else if (mad.CharacterType == CharacterType.RPG)
		{
			modules[5] = base.gameObject.AddComponent<CarriedModule>().ConnectModule(myActor);
			modules[6] = base.gameObject.AddComponent<CorpseModule>().ConnectModule(myActor);
			modules[12] = base.gameObject.AddComponent<RPGModule>().ConnectModule(myActor);
			activeModule = Modules.Invalid;
			SetActiveModule(Modules.RPG);
		}
		else
		{
			modules[1] = base.gameObject.AddComponent<MoveAimModule>().ConnectModule(myActor);
			modules[2] = base.gameObject.AddComponent<CrouchCoverModule>().ConnectModule(myActor);
			modules[3] = base.gameObject.AddComponent<HighCornerCoverModule>().ConnectModule(myActor);
			modules[5] = base.gameObject.AddComponent<CarriedModule>().ConnectModule(myActor);
			modules[4] = base.gameObject.AddComponent<CarryingModule>().ConnectModule(myActor);
			modules[6] = base.gameObject.AddComponent<CorpseModule>().ConnectModule(myActor);
			modules[8] = base.gameObject.AddComponent<FixedGunModule>().ConnectModule(myActor);
			activeModule = Modules.Invalid;
			SetActiveModule(Modules.MoveAim);
		}
		mSegueEndTime = -1f;
	}

	public void SetActiveModule(Modules m)
	{
		if (modules[(int)m] == null)
		{
			Debug.LogError(string.Concat("Something is trying to enable module ", m, " on unit ", base.name, " which doesn't have that kind of module!"));
		}
		else if (activeModule != m)
		{
			if (activeModule > Modules.Invalid)
			{
				modules[(int)activeModule].OnInactive(m);
			}
			modules[(int)m].OnActive(activeModule);
			activeModule = m;
		}
	}

	public void BlendOntoAxis(float duration, AnimDirector.BlendEasing eBegin, AnimDirector.BlendEasing eEnd)
	{
		if (blend != 0f || direction == BlendDirection.OffAxis)
		{
			if (duration == 0f)
			{
				blend = 0f;
				return;
			}
			direction = BlendDirection.OntoAxis;
			blendStartTime = WorldHelper.ThisFrameTime;
			blendDuration = duration;
			Vector3 position = offAxisTrans.position;
			Quaternion rotation = offAxisTrans.rotation;
			offAxisTransParent.position = idealOnAxisPos;
			offAxisTransParent.rotation = idealOnAxisRot;
			offAxisTrans.rotation = rotation;
			offAxisTrans.position = position;
			easeBegin = eEnd;
			easeEnd = eBegin;
		}
	}

	public void BlendOffAxis(float duration, AnimDirector.BlendEasing eBegin, AnimDirector.BlendEasing eEnd, bool navAgentEnabled)
	{
		if (blend != 1f || direction == BlendDirection.OntoAxis)
		{
			if (duration == 0f)
			{
				blend = 1f;
				modelTrans.position = offAxisPos;
				modelTrans.rotation = offAxisRot;
				return;
			}
			direction = BlendDirection.OffAxis;
			blendStartTime = WorldHelper.ThisFrameTime;
			blendDuration = duration;
			Vector3 position = offAxisTrans.position;
			Quaternion rotation = offAxisTrans.rotation;
			offAxisTransParent.position = idealOnAxisPos;
			offAxisTransParent.rotation = idealOnAxisRot;
			offAxisTrans.rotation = rotation;
			offAxisTrans.position = position;
			easeBegin = eBegin;
			easeEnd = eEnd;
			navAgentOffAxis = navAgentEnabled;
		}
	}

	public void Puppet()
	{
		ApplyLocation(offAxisTrans.position, offAxisTrans.rotation);
	}

	public void DirtyInternalModelPosition()
	{
		internalModelPosition = new Vector3(0f, -5000f, 0f);
	}

	public void ApplyLocation(Vector3 lerpPos, Quaternion lerpRot)
	{
		bool flag = !mWasOnScreen;
		if ((lerpPos - internalModelPosition).sqrMagnitude > 0f)
		{
			flag = true;
			internalModelPosition = lerpPos;
		}
		bool flag2 = myActor.animDirector.ShouldAnimate();
		flag2 &= !myActor.baseCharacter.IsFirstPerson;
		if (flag2 & (activeModule != Modules.FixedGun))
		{
			if (modelTrans.rotation != lerpRot)
			{
				modelTrans.rotation = lerpRot;
			}
			internalModelPosition = lerpPos;
			if (flag)
			{
				modelTrans.position = internalModelPosition;
			}
			mWasOnScreen = true;
		}
		else
		{
			if (mWasOnScreen)
			{
				modelTrans.position = new Vector3(lerpPos.x, 5000f, lerpPos.z);
			}
			if (mAlwaysUpdateOrientation && modelTrans.rotation != lerpRot)
			{
				modelTrans.rotation = lerpRot;
			}
			mWasOnScreen = false;
		}
	}

	public void PostModuleUpdate()
	{
		if (mSegueEndTime >= 0f && mSegueEndTime <= WorldHelper.ThisFrameTime)
		{
			float num = WorldHelper.ThisFrameTime - mSegueEndTime;
			mSegueEndTime = -1f;
			if (mSegueToAction != null)
			{
				myActor.animDirector.PlayAction(mSegueToAction, 0f);
				myActor.animDirector.SetCategoryTime(mSegueToAction.CategoryID, mSegueToStartTime + num);
			}
			onAxisTrans.position = offAxisPos + offAxisRot * mSegueData.segueEndOffsetPos;
			onAxisTrans.rotation = offAxisRot * mSegueData.segueEndOffsetRot;
			idealOnAxisPos = onAxisTrans.position;
			idealOnAxisRot = onAxisTrans.rotation;
			if (mSegueToDestination)
			{
				myActor.navAgent.destination = onAxisTrans.position;
			}
			else
			{
				Vector3 vector = offAxisRot * mSegueData.segueEndVelocity;
				onAxisTrans.position += vector * num;
				myActor.navAgent.velocity = vector;
			}
			myActor.SetPosition(onAxisTrans.position);
			if (mAnimToStop != null && mAnimToStop.AnimClip != null)
			{
				myActor.animDirector.StopAnim(mAnimToStop);
			}
			mAnimToStop = null;
			blend = 0f;
		}
		Vector3 zero = Vector3.zero;
		Quaternion identity = Quaternion.identity;
		if (direction == BlendDirection.OffAxis)
		{
			blend = (WorldHelper.ThisFrameTime - blendStartTime) / blendDuration;
			if (blend >= 1f)
			{
				blend = 1f;
				zero = offAxisPos;
				identity = offAxisRot;
				offAxisTransParent.position = idealOnAxisTrans.position;
				offAxisTransParent.rotation = idealOnAxisTrans.rotation;
				direction = BlendDirection.None;
				if (!navAgentOffAxis)
				{
					myActor.realCharacter.EnableNavMesh(false);
				}
			}
		}
		else if (direction == BlendDirection.OntoAxis)
		{
			blend = 1f - (WorldHelper.ThisFrameTime - blendStartTime) / blendDuration;
			if (blend <= 0f)
			{
				blend = 0f;
				zero = onAxisTrans.position;
				identity = onAxisTrans.rotation;
				direction = BlendDirection.None;
			}
		}
		prevBlend = blend;
		bool flag = myActor.Picker != null;
		if (myActor.realCharacter != null && myActor.realCharacter.Ragdoll != null && !myActor.realCharacter.Ragdoll.Kinematic)
		{
			flag = false;
		}
		if (prevBlend <= 0f)
		{
			zero = onAxisTrans.position;
			identity = onAxisTrans.rotation;
			if (flag)
			{
				myActor.Picker.transform.localPosition = Vector3.up;
			}
		}
		else if (prevBlend >= 1f)
		{
			zero = offAxisPos;
			identity = offAxisRot;
			if (flag)
			{
				myActor.Picker.transform.position = myActor.realCharacter.HudMarker.OffsetTarget.position - Vector3.up * 0.4f;
			}
		}
		else
		{
			float num2 = AnimDirector.Blender.Evaluate(0f, 1f, prevBlend, easeBegin, easeEnd);
			if (flag)
			{
				myActor.Picker.transform.position = Vector3.Lerp(myActor.GetPosition() + Vector3.up, myActor.realCharacter.HudMarker.OffsetTarget.position - Vector3.up * 0.4f, num2);
			}
			offAxisTransParent.position = Vector3.Lerp(onAxisTrans.position, idealOnAxisTrans.position, num2);
			offAxisTransParent.rotation = Quaternion.Slerp(onAxisTrans.rotation, idealOnAxisTrans.rotation, num2);
			identity = Quaternion.Slerp(onAxisTrans.rotation, offAxisRot, num2);
			Vector3 vector2 = Quaternion.Inverse(offAxisRot) * (idealOnAxisPos - offAxisPos);
			zero = Vector3.Lerp(onAxisTrans.position, offAxisPos, num2);
			Vector3 vector3 = zero + identity * vector2 * num2;
			zero -= vector3 - offAxisTransParent.position;
		}
		ApplyLocation(zero, identity);
		prevBlend = blend;
	}

	public void Command(string com)
	{
		SetActiveModule(modules[(int)activeModule].Command(com));
	}

	public void UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick)
	{
		SetActiveModule(modules[(int)activeModule].UpdatePose(destination, newPos, newVel, aimDir, ref newStateStr, expensiveTick));
		faceLookDirection = false;
		restrictAiming = false;
	}

	public void Segue(AnimDirector.ActionHandle fromAction, AnimDirector.ActionHandle toAction, float toStartTime, bool setDestination)
	{
		SegueData segueData = myActor.animDirector.GetSegueData(fromAction);
		RawAnimation rawAnimation = myActor.animDirector.PlayAction(fromAction);
		myActor.animDirector.SetCategoryTime(fromAction.CategoryID, 0f);
		if (segueData != null && rawAnimation != null && segueData.IsValid() && rawAnimation.AnimClip != null)
		{
			if (mAnimToStop != null && mAnimToStop.AnimClip != null)
			{
				myActor.animDirector.StopAnim(mAnimToStop);
			}
			mAnimToStop = rawAnimation;
			mSegueData = segueData;
			offAxisPos = onAxisTrans.position + onAxisTrans.rotation * segueData.segueStartOffsetPos;
			offAxisRot = onAxisTrans.rotation * segueData.segueStartOffsetRot;
			blend = 1f;
			mSegueEndTime = WorldHelper.ThisFrameTime + rawAnimation.AnimClip.length;
			mSegueToAction = toAction;
			mSegueToStartTime = toStartTime;
			mSegueToDestination = setDestination;
		}
		else
		{
			myActor.animDirector.ChainAction(fromAction.CategoryID, toAction, toStartTime);
		}
	}

	public void CancelSegue()
	{
		if (mSegueToAction != null)
		{
			mSegueToAction = null;
			mSegueEndTime = -1f;
			BlendOntoAxis(0f, AnimDirector.BlendEasing.Linear, AnimDirector.BlendEasing.Linear);
		}
	}

	public bool Segueing()
	{
		return mSegueEndTime >= 0f;
	}
}
