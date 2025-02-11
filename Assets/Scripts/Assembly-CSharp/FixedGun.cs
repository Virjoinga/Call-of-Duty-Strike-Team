using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CMFixedGun))]
public class FixedGun : MonoBehaviour
{
	public enum HeatLevel
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public class FixedGunWeaponModel : IWeaponModel
	{
		private FixedGun mFixedGun;

		public FixedGunWeaponModel(FixedGun fixedGun)
		{
			mFixedGun = fixedGun;
		}

		public Transform GetMuzzleLocator()
		{
			return mFixedGun.BarrelExit;
		}

		public Transform GetCasingsLocator()
		{
			return mFixedGun.CasingsLocator;
		}
	}

	private enum HeatingState
	{
		Inactive = 0,
		WarmUp = 1,
		Warming = 2,
		Looping = 3,
		CoolDown = 4,
		Finished = 5
	}

	private const float mLengthOfSpinDown = 0.5f;

	private const float mLengthOfSpinUp = 0.5f;

	private const float mLengthOfSpinUpShort = 0.5f;

	private const float mStartPointForSpinUp = 0.2f;

	private const float mStartPointForSpinUpShort = 0.8f;

	public MinigunDescriptor Descriptor;

	public GameObject Model;

	public Transform BarrelExit;

	public GameObject[] BarrelLODs;

	public Transform CasingsLocator;

	public float BarrelRotationSpeed;

	public FirstPersonCamera CameraAim;

	public Transform Pivot;

	public Transform GunnerLocator;

	public float GunPitchLimitUp;

	public float GunPitchLimitDown;

	public float GunYawLimit;

	public SingleAnimation AimLow;

	public SingleAnimation AimHigh;

	public SingleAnimation LookLow;

	public SingleAnimation LookHigh;

	public SingleAnimation Mount;

	public SingleAnimation Dismount;

	public SingleAnimation Fire;

	public float AnimationPitchLimit;

	public float AnimationYawLimit;

	public Transform TargetOverride;

	public bool UseAirborneCoverWhenDocked;

	public int ModifiedDifficultySettingsWhenDocked;

	public float TakeDamageMultiplier = 1f;

	public List<GameObject> ObjectsToCallOnUse;

	public List<string> FunctionsToCallOnUse;

	public List<GameObject> ObjectsToCallOnFire;

	public List<string> FunctionsToCallOnFire;

	public List<GameObject> ObjectsToCallOnLeave;

	public List<string> FunctionsToCallOnLeave;

	public ObjectMessage MessageToSendOnOverheat;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public bool OneShotMessages = true;

	private float mEarliestReUseTime;

	private bool mOnUseMessagesFired;

	private bool mOnFireMessagesFired;

	private bool mOnLeaveMessagesFired;

	private bool mOverheatMessageFired;

	private Actor mGunner;

	private Transform mGunnerLookLocator;

	private Actor mBookingAgent;

	private bool mActivated = true;

	private CharacterLighting mLighting;

	private Weapon_Minigun mWeaponState;

	private AudioSource mAudioSourceSpin;

	private AudioSource mAudioSourceFire;

	private float mPreviousSpinUpAmount;

	private float mTimeSpinUpStarted;

	private float mTimeSpinUpShortStarted;

	private float mTimeSpinDownStarted;

	private bool mCanPlayStart = true;

	private bool mHasGoneDownOrOff = true;

	private bool mPlayingSpinLoop;

	private bool mPlayingFireLoop;

	public Vector2 AimAngles { get; set; }

	public float PitchLimitUp
	{
		get
		{
			return Mathf.Min(GunPitchLimitUp, AnimationPitchLimit);
		}
	}

	public float PitchLimitDown
	{
		get
		{
			return Mathf.Min(GunPitchLimitDown, AnimationPitchLimit);
		}
	}

	public float YawLimit
	{
		get
		{
			return Mathf.Min(GunYawLimit, AnimationYawLimit);
		}
	}

	public HeatLevel GetHeatLevel()
	{
		if ((double)mWeaponState.GetHeatLevel() < 0.35)
		{
			return HeatLevel.Low;
		}
		if ((double)mWeaponState.GetHeatLevel() < 0.5)
		{
			return HeatLevel.Medium;
		}
		return HeatLevel.High;
	}

	public float CalculateAnimationPitchBlend(float pitch)
	{
		return Mathf.InverseLerp(0f - AnimationPitchLimit, AnimationPitchLimit, pitch);
	}

	public float CalculateAnimationYawBlend(float yaw)
	{
		return Mathf.InverseLerp(0f - AnimationYawLimit, AnimationYawLimit, yaw);
	}

	public Vector2 CalculateAimAnglesForTarget(Vector3 target)
	{
		Vector3 forward = base.transform.forward;
		Vector3 right = base.transform.right;
		Vector3 normalized = (target - BarrelExit.position).normalized;
		Vector2 normalized2 = normalized.xz().normalized;
		float num = Mathf.Acos(Mathf.Clamp(Vector2.Dot(normalized2, forward.xz().normalized), -1f, 1f));
		num *= ((!(Vector2.Dot(normalized2, right.xz()) > 0f)) ? (-1f) : 1f);
		return new Vector2(57.29578f * Mathf.Asin(0f - normalized.y), 57.29578f * num);
	}

	public Vector2 CalculateLookAnglesForTarget(Vector3 target)
	{
		if (mGunnerLookLocator == null)
		{
			return Vector2.zero;
		}
		Vector3 forward = base.transform.forward;
		Vector3 right = base.transform.right;
		Vector3 normalized = (target - mGunnerLookLocator.position).normalized;
		Vector2 normalized2 = normalized.xz().normalized;
		float num = Mathf.Acos(Mathf.Clamp(Vector2.Dot(normalized2, forward.xz().normalized), -1f, 1f));
		num *= ((!(Vector2.Dot(normalized2, right.xz()) > 0f)) ? (-1f) : 1f);
		return new Vector2(57.29578f * Mathf.Asin(0f - normalized.y), 57.29578f * num);
	}

	public bool IsGoodForActor(Actor actor)
	{
		if (Time.time < mEarliestReUseTime)
		{
			return false;
		}
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.EnemiesMask(actor) & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			Vector3 lhs = a.GetPosition() - base.transform.position;
			float num = Vector3.Dot(lhs, base.transform.forward);
			if (num < 0f && lhs.sqrMagnitude < 144f)
			{
				return false;
			}
		}
		return true;
	}

	private void Awake()
	{
		CharacterLighting characterLighting = Model.AddComponent<CharacterLighting>();
		characterLighting.Renderers = Model.GetComponentsInChildren<Renderer>();
		characterLighting.ProbeAnchor = Model.transform;
		characterLighting.FlashAnchor = BarrelExit;
		mLighting = characterLighting;
	}

	private void Start()
	{
		mWeaponState = new Weapon_Minigun(new FixedGunWeaponModel(this), Descriptor);
		mWeaponState.UseFiringSounds = false;
		mPreviousSpinUpAmount = 0f;
	}

	public void OnDestroy()
	{
		StopAllSound();
	}

	private void Update()
	{
		Actor actor = mGunner;
		if (actor != null)
		{
			actor.SetPosition(GunnerLocator.transform.position);
			actor.transform.rotation = GunnerLocator.transform.rotation;
			if (actor.baseCharacter != null)
			{
				actor.baseCharacter.Stand();
			}
			if (actor.health != null)
			{
				actor.health.TakeDamageModifier = TakeDamageMultiplier;
			}
			if (mGunner != null && mGunner.realCharacter != null)
			{
				Pivot.rotation = Quaternion.Euler(mGunner.baseCharacter.GetReferenceAngles()) * Quaternion.Euler(new Vector3(0f, AimAngles.y, 0f)) * Quaternion.Euler(new Vector3(AimAngles.x, 0f, 0f));
			}
			if (actor.realCharacter != null && actor.realCharacter.IsFirstPerson)
			{
				float heatLevel = mWeaponState.GetHeatLevel();
				if (heatLevel > 0.8f && !mOverheatMessageFired)
				{
					if (MessageToSendOnOverheat.Object != null)
					{
						Container.SendMessage(MessageToSendOnOverheat.Object, MessageToSendOnOverheat.Message, base.gameObject);
					}
					mOverheatMessageFired = true;
				}
				CommonHudController.Instance.SetChainGunHeat(heatLevel, mWeaponState.GetJammed());
			}
			if (mWeaponState.IsFiring() && actor.behaviour != null && actor.behaviour.PlayerControlled)
			{
				TriggerFireMessages();
			}
			if (mGunner.baseCharacter != null && (mGunner.baseCharacter.IsMortallyWounded() || mGunner.baseCharacter.IsDead()))
			{
				ClearGunner();
			}
		}
		else
		{
			if (mWeaponState != null)
			{
				mWeaponState.ReleaseTrigger();
				mWeaponState.Update(Time.deltaTime, null, null);
			}
			AimAngles = Vector2.zero;
		}
		if (mBookingAgent != null && (mBookingAgent.realCharacter.IsDead() || !mBookingAgent.tasks.IsRunningTask(typeof(TaskUseFixedGun))))
		{
			mBookingAgent = null;
		}
		DoSpinLoop();
		if (mWeaponState != null)
		{
			GameObject[] barrelLODs = BarrelLODs;
			foreach (GameObject gameObject in barrelLODs)
			{
				gameObject.transform.Rotate(0f, mWeaponState.GetSpinUpAmount() * BarrelRotationSpeed * Time.deltaTime, 0f);
			}
			if (mWeaponState.IsFiring())
			{
				CameraAim.AddShake(Descriptor.ShakeIntensity, 0.1f);
				StartFireLoop();
				if (actor != null && actor.realCharacter != null && actor.realCharacter.IsFirstPerson)
				{
					mLighting.Flash(BarrelExit.transform, 10f);
				}
			}
			else
			{
				StopFireLoop();
			}
		}
		if (mGunner != null && mGunner.realCharacter != null && mGunner.realCharacter.IsFirstPerson)
		{
			Pivot.localEulerAngles = CameraAim.Angles;
		}
		else
		{
			CameraAim.Angles = Pivot.eulerAngles;
		}
	}

	private void LateUpdate()
	{
		mLighting.UpdateMaterials(true);
	}

	private void TriggerUseMessages()
	{
		if ((OneShotMessages && mOnUseMessagesFired) || ObjectsToCallOnUse == null || ObjectsToCallOnUse.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in ObjectsToCallOnUse)
		{
			string message = string.Empty;
			if (FunctionsToCallOnUse != null && num < FunctionsToCallOnUse.Count)
			{
				message = FunctionsToCallOnUse[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
		mOnUseMessagesFired = true;
	}

	private void TriggerFireMessages()
	{
		if (mOnFireMessagesFired || ObjectsToCallOnFire == null || ObjectsToCallOnFire.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in ObjectsToCallOnFire)
		{
			string message = string.Empty;
			if (FunctionsToCallOnFire != null && num < FunctionsToCallOnFire.Count)
			{
				message = FunctionsToCallOnFire[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
		mOnFireMessagesFired = true;
	}

	private void TriggerLeaveMessages()
	{
		if ((OneShotMessages && mOnLeaveMessagesFired) || (!mOnUseMessagesFired && !mOnFireMessagesFired) || ObjectsToCallOnLeave == null || ObjectsToCallOnLeave.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in ObjectsToCallOnLeave)
		{
			string message = string.Empty;
			if (FunctionsToCallOnLeave != null && num < FunctionsToCallOnLeave.Count)
			{
				message = FunctionsToCallOnLeave[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
		mOnLeaveMessagesFired = true;
	}

	public Actor GetGunner()
	{
		return mGunner;
	}

	public void SetGunner(Actor actor)
	{
		ClearGunner();
		mGunner = actor;
		if (actor.model != null)
		{
			mGunnerLookLocator = actor.model.transform.FindInHierarchy("Bip002 Head");
		}
		if (actor.weapon != null)
		{
			mGunner.weapon.SwapTo(mWeaponState, 1f);
		}
		if (actor.animDirector != null)
		{
			actor.animDirector.enabled = false;
			Animation animationPlayer = actor.animDirector.AnimationPlayer;
			animationPlayer.Stop();
			AimLow.Initialise("AimLow", animationPlayer);
			AimHigh.Initialise("AimHigh", animationPlayer);
			LookLow.Initialise("LookLow", animationPlayer);
			LookHigh.Initialise("LookHigh", animationPlayer);
			Fire.Initialise("Fire", animationPlayer);
			LookLow.State.AddMixingTransform(mGunnerLookLocator);
			LookHigh.State.AddMixingTransform(mGunnerLookLocator);
		}
		if (actor.baseCharacter != null)
		{
			actor.baseCharacter.IsUsingFixedGun = true;
			EnemyBlip enemyBlip = actor.baseCharacter.HudMarker as EnemyBlip;
			if (enemyBlip != null)
			{
				enemyBlip.UseHeavyWeightEnemyIcon(true);
			}
		}
		CMFixedGun componentInChildren = GetComponentInChildren<CMFixedGun>();
		if (componentInChildren != null)
		{
			componentInChildren.enabled = false;
		}
		if (actor.behaviour != null && actor.behaviour.PlayerControlled)
		{
			TriggerUseMessages();
			StartCoroutine(HideCrouch(true));
		}
	}

	public void ClearGunner()
	{
		if (mGunner != null)
		{
			AimLow.Reset();
			AimHigh.Reset();
			LookLow.Reset();
			LookHigh.Reset();
			Mount.Reset();
			Fire.Reset();
			if (mGunner.behaviour != null && mGunner.behaviour.PlayerControlled)
			{
				TriggerLeaveMessages();
				StartCoroutine(HideCrouch(false));
			}
			if (mGunner.animDirector != null)
			{
				mGunner.animDirector.enabled = true;
			}
			if (mGunner.baseCharacter != null)
			{
				mGunner.baseCharacter.IsUsingFixedGun = false;
				EnemyBlip enemyBlip = mGunner.baseCharacter.HudMarker as EnemyBlip;
				if (enemyBlip != null)
				{
					enemyBlip.UseHeavyWeightEnemyIcon(false);
				}
				mGunner.baseCharacter.IsAimingDownSights = false;
			}
			if (mGunner.weapon != null)
			{
				mGunner.weapon.SwitchToPrevious();
			}
			mGunner = null;
			mEarliestReUseTime = Time.time + Random.Range(2f, 4f);
			ContextMenuDistanceManager componentInChildren = GetComponentInChildren<ContextMenuDistanceManager>();
			if (componentInChildren != null)
			{
				componentInChildren.ForceUpdate();
			}
		}
		mBookingAgent = null;
		StopAllSound();
	}

	private IEnumerator HideCrouch(bool hide)
	{
		if (hide)
		{
			while (!GameController.Instance.IsFirstPerson)
			{
				yield return null;
			}
		}
		CommonHudController.Instance.HideCrouchButton(hide);
	}

	public bool HasGunner()
	{
		return mGunner != null;
	}

	public bool IsBooked()
	{
		return mBookingAgent != null;
	}

	public void BookGun(Actor booker)
	{
		if (booker != null)
		{
			mBookingAgent = booker;
		}
	}

	public bool IsAvailableForUse()
	{
		return !HasGunner() && !IsBooked() && mActivated;
	}

	public bool IsPositionTargetable(Vector3 positionToTarget)
	{
		return false;
	}

	public void Activate()
	{
		mActivated = true;
		CMFixedGun componentInChildren = IncludeDisabled.GetComponentInChildren<CMFixedGun>(base.gameObject);
		if (componentInChildren != null)
		{
			componentInChildren.Activate();
			componentInChildren.TurnOn();
		}
	}

	public void Deactivate()
	{
		mActivated = false;
		if (HasGunner())
		{
			ClearGunner();
		}
		else
		{
			CMFixedGun componentInChildren = IncludeDisabled.GetComponentInChildren<CMFixedGun>(base.gameObject);
			if (componentInChildren != null)
			{
				componentInChildren.Deactivate();
				componentInChildren.TurnOn();
			}
		}
		StopAllSound();
	}

	public Vector3 GetMuzzlePosition()
	{
		return BarrelExit.position;
	}

	public Vector3 GetTraceOrigin()
	{
		return CameraAim.transform.position;
	}

	public Vector3 GetTraceDirection()
	{
		return CameraAim.transform.forward;
	}

	public WeaponAmmo GetWeaponAmmo()
	{
		return null;
	}

	public void OnDrawGizmos()
	{
		if (!(mGunner != null))
		{
			return;
		}
		Transform transform = mGunnerLookLocator;
		if (transform != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, transform.position + 100f * transform.up);
			if (TargetOverride != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, TargetOverride.position);
			}
		}
	}

	private void DoSpinLoop()
	{
		if (mWeaponState == null || !HasGunner())
		{
			return;
		}
		float spinUpAmount = mWeaponState.GetSpinUpAmount();
		if (spinUpAmount < mPreviousSpinUpAmount || spinUpAmount == 0f)
		{
			mHasGoneDownOrOff = true;
			mCanPlayStart = false;
		}
		if (spinUpAmount > mPreviousSpinUpAmount && mHasGoneDownOrOff)
		{
			mCanPlayStart = true;
			mHasGoneDownOrOff = false;
		}
		if (spinUpAmount > 0.5f && spinUpAmount >= mPreviousSpinUpAmount)
		{
			StartSpinLoop();
		}
		else if (spinUpAmount < mPreviousSpinUpAmount)
		{
			if (SpinLoopPlaying())
			{
				StopSpinUp();
				StopSpinDown();
				StartSpinDown();
			}
			StopSpinLoop();
		}
		if (mCanPlayStart && !SpinUpPlaying() && !SpinUpShortPlaying())
		{
			if (mPreviousSpinUpAmount < 0.2f)
			{
				StopSpinDown();
				StopSpinUp();
				StopSpinUpShort();
				StartSpinUp();
			}
			else if (mPreviousSpinUpAmount < 0.8f)
			{
				StopSpinDown();
				StopSpinUp();
				StopSpinUpShort();
				StartSpinUpShort();
			}
		}
		mPreviousSpinUpAmount = spinUpAmount;
	}

	private bool SpinLoopPlaying()
	{
		return mPlayingSpinLoop;
	}

	private void StartSpinDown()
	{
		mTimeSpinDownStarted = Time.time;
		WeaponSFX.Instance.MiniGunSpinDown.Play(base.gameObject);
	}

	private void StopSpinDown()
	{
		mTimeSpinDownStarted = 0f;
		WeaponSFX.Instance.MiniGunSpinDown.Stop(base.gameObject);
	}

	private bool SpinDownPlaying()
	{
		bool result = true;
		if (Time.time > mTimeSpinDownStarted + 0.5f)
		{
			result = false;
		}
		return result;
	}

	private void StartSpinUp()
	{
		mTimeSpinUpStarted = Time.time;
		WeaponSFX.Instance.MiniGunSpinUp.Play(base.gameObject);
	}

	private void StopSpinUp()
	{
		mTimeSpinUpStarted = 0f;
		if (WeaponSFX.HasInstance)
		{
			WeaponSFX.Instance.MiniGunSpinUp.Stop(base.gameObject);
		}
	}

	private bool SpinUpPlaying()
	{
		bool result = true;
		if (Time.time > mTimeSpinUpStarted + 0.5f)
		{
			result = false;
		}
		return result;
	}

	private void StartSpinUpShort()
	{
		mTimeSpinUpShortStarted = Time.time;
		WeaponSFX.Instance.MiniGunSpinUpShort.Play(base.gameObject);
	}

	private void StopSpinUpShort()
	{
		mTimeSpinUpShortStarted = 0f;
		if (WeaponSFX.HasInstance)
		{
			WeaponSFX.Instance.MiniGunSpinUpShort.Stop(base.gameObject);
		}
	}

	private bool SpinUpShortPlaying()
	{
		bool result = true;
		if (Time.time > mTimeSpinUpShortStarted + 0.5f)
		{
			result = false;
		}
		return result;
	}

	private void StartSpinLoop()
	{
		if (!mPlayingSpinLoop)
		{
			mPlayingSpinLoop = true;
			WeaponSFX.Instance.MiniGunSpinLoop.Play(base.gameObject);
		}
	}

	private void StopSpinLoop()
	{
		if (mPlayingSpinLoop)
		{
			mPlayingSpinLoop = false;
			if (WeaponSFX.HasInstance)
			{
				WeaponSFX.Instance.MiniGunSpinLoop.Stop(base.gameObject);
			}
		}
	}

	private void StartFireLoop()
	{
		if (!mPlayingFireLoop && HasGunner())
		{
			mPlayingFireLoop = true;
			WeaponSFX.Instance.MiniGunFireLoop.Play(base.gameObject);
		}
	}

	private void StopFireLoop()
	{
		if (mPlayingFireLoop)
		{
			mPlayingFireLoop = false;
			if (WeaponSFX.HasInstance)
			{
				WeaponSFX.Instance.MiniGunFireLoop.Stop(base.gameObject);
			}
		}
	}

	private void StopAllSound()
	{
		StopSpinLoop();
		StopFireLoop();
		StopSpinUpShort();
		StopSpinUp();
		if (mWeaponState != null)
		{
			mWeaponState.StopOverHeatWarning();
		}
	}
}
