using UnityEngine;

public class TaskUseFixedGun : Task
{
	private enum State
	{
		Start = 0,
		MovingToGun = 1,
		MountingGun = 2,
		Gunning = 3,
		ReadingToDismount = 4,
		DismountingGun = 5,
		Done = 6,
		Invalidated = 7
	}

	private ActorIdentIterator myAii = new ActorIdentIterator(0u);

	private FixedGun mFixedGun;

	private IWeapon mWeapon;

	private State mState;

	private bool mOnGun;

	private bool mWarp;

	private bool mAirBorne;

	private bool mDontDismount;

	private bool mSuppressTransition;

	private bool mShouldFire;

	private bool mShouldDismount;

	private Vector2 mDesiredAimAngles;

	private Vector2 mDesiredLookAngles;

	private Vector2 mLookAngles;

	private bool mWaitForCooldown;

	private bool mIsNotUsingMountAnims;

	private float mTimeBeforeNextRandomTarget;

	private float mTimeBeforeRandomGunTrack;

	private bool mFireAtWillWasEnabled;

	public bool IsMountedInAVTOL
	{
		get
		{
			return mAirBorne;
		}
	}

	public TaskUseFixedGun(TaskManager owner, TaskManager.Priority priority, Config flags, FixedGun gun, bool warp, bool airborne, bool dontDismount, bool suppressTransition)
		: base(owner, priority, flags)
	{
		mFixedGun = gun;
		mFixedGun.BookGun(mActor);
		mState = State.Start;
		mWarp = warp;
		mAirBorne = airborne;
		mDontDismount = dontDismount;
		mSuppressTransition = suppressTransition;
		mFireAtWillWasEnabled = false;
		FireAtWillComponent component = base.Owner.GetComponent<FireAtWillComponent>();
		if (component != null)
		{
			mFireAtWillWasEnabled = component.enabled;
			component.enabled = false;
		}
		if (mWarp)
		{
			mActor.SetPosition(mFixedGun.GunnerLocator.position);
			if (mActor.weapon != null && mActor.behaviour.PlayerControlled)
			{
				mActor.weapon.SwapToImmediately(new Weapon_Null());
			}
			mActor.awareness.airborne = mFixedGun.UseAirborneCoverWhenDocked;
			GlobalBalanceTweaks.DifficultySettingsModifier = mFixedGun.ModifiedDifficultySettingsWhenDocked;
			GotOnGun();
			mState = ((!(mFixedGun.GetGunner() == mActor)) ? State.Done : State.Gunning);
		}
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
		{
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams();
			if (mActor.behaviour.PlayerControlled)
			{
				inheritableMovementParams.mMovementStyle = BaseCharacter.MovementStyle.Run;
			}
			else
			{
				inheritableMovementParams.mMovementStyle = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
			}
			inheritableMovementParams.mDestination = mFixedGun.GunnerLocator.position;
			new TaskRouteTo(mOwner, base.Priority, Config.ClearAllCurrentType, inheritableMovementParams);
			mState = State.MovingToGun;
			break;
		}
		case State.MovingToGun:
			if (!((mActor.GetPosition() - mFixedGun.GunnerLocator.position).sqrMagnitude < 0.1f))
			{
				break;
			}
			if (!mFixedGun.HasGunner())
			{
				GameController instance = GameController.Instance;
				if (instance != null && instance.IsFirstPerson)
				{
					mIsNotUsingMountAnims = mActor == instance.mFirstPersonActor;
				}
				mFixedGun.Mount.Initialise("Mount", mActor.animDirector.AnimationPlayer);
				mFixedGun.Mount.State.enabled = true;
				mFixedGun.Mount.State.wrapMode = WrapMode.ClampForever;
				mFixedGun.Mount.State.time = 0f;
				mFixedGun.Mount.State.speed = 1f;
				mFixedGun.Mount.State.weight = 0f;
				mFixedGun.Mount.State.layer = 17;
				mState = State.MountingGun;
				mConfigFlags |= Config.DenyPlayerInput;
			}
			else
			{
				mState = State.Invalidated;
			}
			break;
		case State.MountingGun:
			if (mIsNotUsingMountAnims || mFixedGun.Mount.RemainingTime <= 0f)
			{
				GotOnGun();
				mState = ((!(mFixedGun.GetGunner() == mActor)) ? State.Done : State.Gunning);
			}
			else
			{
				UpdatePosition();
				mFixedGun.Mount.State.weight += 8f * Time.deltaTime;
			}
			break;
		case State.Gunning:
			mActor.awareness.airborne = mFixedGun.UseAirborneCoverWhenDocked;
			GlobalBalanceTweaks.DifficultySettingsModifier = mFixedGun.ModifiedDifficultySettingsWhenDocked;
			if (mFixedGun.Mount.State != null)
			{
				mFixedGun.Mount.State.weight -= 8f * Time.deltaTime;
			}
			if (mActor.realCharacter != null)
			{
				UpdatePosition();
				UpdateTargeting();
				Vector2 vector = mDesiredAimAngles - mFixedGun.AimAngles;
				float num = Mathf.Lerp(0.1f, 5f, Mathf.Clamp01(vector.magnitude / 50f));
				float num2 = 40f;
				float num3 = ((!(mActor.behaviour != null) || !mActor.behaviour.InActiveAlertState()) ? num : num2);
				mFixedGun.AimAngles += Vector2.ClampMagnitude(vector, Mathf.Min(num3 * Time.deltaTime, 10f * vector.magnitude));
				Vector2 vector2 = mDesiredLookAngles - mLookAngles;
				float t = Mathf.Clamp01(vector2.magnitude / 50f);
				float num4 = Mathf.Lerp(1f, 360f, t);
				float num5 = Mathf.Lerp(1f, 30f, t);
				float num6 = ((!(mActor.behaviour != null) || !mActor.behaviour.InActiveAlertState()) ? num5 : num4);
				mLookAngles += Vector2.ClampMagnitude(vector2, Mathf.Min(num6 * Time.deltaTime, 10f * vector2.magnitude));
				UpdateGunningAnimations();
				if (mActor.behaviour.PlayerControlled)
				{
					if (!mActor.realCharacter.IsFirstPerson)
					{
						mState = State.ReadingToDismount;
					}
					break;
				}
				mActor.weapon.SetTrigger(mShouldFire);
				if (ShouldDismount())
				{
					mState = State.ReadingToDismount;
				}
			}
			else
			{
				mState = State.Done;
			}
			break;
		case State.ReadingToDismount:
		{
			Vector2 vector3 = -mFixedGun.AimAngles;
			float magnitude = vector3.magnitude;
			mFixedGun.AimAngles += Vector2.ClampMagnitude(vector3, 40f * Time.deltaTime);
			if (magnitude < 0.01f || mActor.behaviour.PlayerControlled)
			{
				mState = State.DismountingGun;
				base.ConfigFlags = Config.DenyPlayerInput;
				mFixedGun.Dismount.Initialise("Dismount", mActor.animDirector.AnimationPlayer);
				mFixedGun.Dismount.State.enabled = true;
				mFixedGun.Dismount.State.wrapMode = WrapMode.ClampForever;
				mFixedGun.Dismount.State.time = 0f;
				mFixedGun.Dismount.State.speed = 1f;
				mFixedGun.Dismount.State.weight = 0f;
				mFixedGun.Dismount.State.layer = 17;
			}
			else
			{
				UpdateGunningAnimations();
			}
			break;
		}
		case State.DismountingGun:
			if (mFixedGun.Dismount.RemainingTime <= 0f)
			{
				GotOffGun();
				mFixedGun.Dismount.State.weight = 1f;
				mState = State.Done;
			}
			else
			{
				mFixedGun.Dismount.State.weight += 8f * Time.deltaTime;
			}
			break;
		case State.Done:
			mFixedGun.Dismount.State.weight -= 8f * Time.deltaTime;
			break;
		case State.Invalidated:
			break;
		}
	}

	public override bool HasFinished()
	{
		if (mState == State.Start)
		{
			return false;
		}
		return (mState == State.Done && mFixedGun.Dismount != null && mFixedGun.Dismount.State != null && mFixedGun.Dismount.State.weight <= 0f) || mState == State.Invalidated || (mOnGun && mFixedGun.GetGunner() != mActor);
	}

	public override void Finish()
	{
		mActor.awareness.airborne = false;
		GlobalBalanceTweaks.DifficultySettingsModifier = 0;
		mFixedGun.Mount.Reset();
		mFixedGun.Dismount.Reset();
		GotOffGun();
		FireAtWillComponent component = base.Owner.GetComponent<FireAtWillComponent>();
		if (component != null)
		{
			component.enabled = mFireAtWillWasEnabled;
		}
	}

	public override void OnSleep()
	{
	}

	private void GotOnGun()
	{
		if (mActor != null)
		{
			mFixedGun.SetGunner(mActor);
			mActor.realCharacter.StartMovingManually();
			mActor.realCharacter.SetReferenceFrame(mFixedGun.GunnerLocator);
			mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.FixedGun);
			FixedGunModule fixedGunModule = mActor.Pose.GetModule(PoseModuleSharedData.Modules.FixedGun) as FixedGunModule;
			fixedGunModule.SetFixedGun(mFixedGun);
			mActor.animDirector.AnimationPlayer.Stop();
			mActor.awareness.airborne = mAirBorne;
			GlobalBalanceTweaks.DifficultySettingsModifier = mFixedGun.ModifiedDifficultySettingsWhenDocked;
			if (mActor.behaviour.PlayerControlled)
			{
				FirstPersonCamera cameraAim = mFixedGun.CameraAim;
				mActor.realCharacter.SetCameraOverride(cameraAim);
				GameController.Instance.SwitchToFirstPerson(mActor, !mSuppressTransition && !mActor.realCharacter.IsFirstPerson);
				cameraAim.Angles = Vector3.zero;
				cameraAim.SetConstraints(new Vector3(0f, 0f, 0f), mFixedGun.GunYawLimit, mFixedGun.GunPitchLimitUp, mFixedGun.GunPitchLimitDown);
			}
			GameController instance = GameController.Instance;
			if (instance != null && instance.IsFirstPerson && mActor == instance.mFirstPersonActor)
			{
				AnimatedScreenBackground.Instance.SetManualAlpha(0f);
				HudStateController.Instance.SetState(HudStateController.HudState.FPP);
			}
			mOnGun = true;
		}
	}

	private void UpdateTargeting()
	{
		myAii.ResetWithMask(GKM.EnemiesMask(mActor) & GKM.AliveMask);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		uint num4 = 0u;
		if (mFixedGun.TargetOverride != null)
		{
			Vector3 position = mFixedGun.TargetOverride.position;
			mDesiredAimAngles = mFixedGun.CalculateAimAnglesForTarget(position);
			mDesiredLookAngles = mFixedGun.CalculateLookAnglesForTarget(position);
			num4 = uint.MaxValue;
		}
		else
		{
			num4 = 0u;
			Actor a;
			while (myAii.NextActor(out a))
			{
				if (!(a == null) && !a.realCharacter.IsMortallyWounded() && a.awareness.ChDefCharacterType != CharacterType.SentryGun && mActor.awareness.CanSee(a))
				{
					num3++;
					Vector3 target = ((!(a.realCharacter != null)) ? a.GetPosition() : a.realCharacter.GetBulletOrigin());
					Vector2 vector = mFixedGun.CalculateAimAnglesForTarget(target);
					if (vector.x > 0f - mFixedGun.PitchLimitDown && vector.x < mFixedGun.PitchLimitUp && Mathf.Abs(vector.y) < mFixedGun.YawLimit)
					{
						num++;
						num4 |= a.ident;
					}
					else
					{
						num2++;
					}
				}
			}
			if (num4 != 0)
			{
				a = TargetScorer.GetBestTargetFromMask(mActor, num4);
				if (a != null)
				{
					Vector3 target2 = ((!(a.realCharacter != null)) ? a.GetPosition() : a.realCharacter.GetBulletOrigin());
					mDesiredAimAngles = mFixedGun.CalculateAimAnglesForTarget(target2);
					mDesiredLookAngles = mFixedGun.CalculateLookAnglesForTarget(target2);
				}
			}
		}
		if (num4 == 0)
		{
			PickRandomTarget();
		}
		if (mFixedGun.GetHeatLevel() == FixedGun.HeatLevel.High)
		{
			mWaitForCooldown = true;
		}
		else if (mFixedGun.GetHeatLevel() == FixedGun.HeatLevel.Low)
		{
			mWaitForCooldown = false;
		}
		bool flag = mActor.behaviour != null && mActor.behaviour.Suppressed;
		mShouldFire = num3 > 0 && !mWaitForCooldown && !flag;
		mShouldDismount = !mDontDismount && num == 0 && num2 > 0;
	}

	private void PickRandomTarget()
	{
		mTimeBeforeNextRandomTarget -= Time.deltaTime;
		mTimeBeforeRandomGunTrack -= Time.deltaTime;
		if (mTimeBeforeNextRandomTarget <= 0f)
		{
			float num = 0.1f;
			float num2 = 0.5f;
			float num3 = 1f;
			float min = 0.3f;
			float max = 3f;
			float min2 = 1f;
			float max2 = 5f;
			if (mActor.behaviour.InActiveAlertState())
			{
				min = 0.2f;
				max = 1f;
				min2 = 0.2f;
				max2 = 1.5f;
			}
			mDesiredLookAngles = new Vector2(Random.Range(num2 * (0f - mFixedGun.PitchLimitUp), num * mFixedGun.PitchLimitDown), num3 * Random.Range(0f - mFixedGun.YawLimit, mFixedGun.YawLimit));
			mTimeBeforeNextRandomTarget = Random.Range(min, max);
			mTimeBeforeRandomGunTrack = Random.Range(min2, max2);
		}
		if (mTimeBeforeRandomGunTrack <= 0f)
		{
			mDesiredAimAngles = mDesiredLookAngles;
		}
	}

	private bool ShouldDismount()
	{
		return mShouldDismount;
	}

	private void GotOffGun()
	{
		if (mOnGun && mActor.realCharacter != null)
		{
			mFixedGun.ClearGunner();
			if (!mActor.realCharacter.IsDead())
			{
				mActor.realCharacter.StopMovingManually();
				mActor.realCharacter.SetReferenceFrame(null);
				mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.MoveAim);
				mActor.awareness.airborne = false;
				GlobalBalanceTweaks.DifficultySettingsModifier = 0;
			}
			if (mActor.behaviour != null && mActor.behaviour.PlayerControlled)
			{
				GameplayController gameplayController = GameplayController.Instance();
				gameplayController.AddToSelected(mActor);
				mFixedGun.CameraAim.ClearConstraints();
				mActor.realCharacter.ClearCameraOverride();
			}
			GameController instance = GameController.Instance;
			if (instance != null && instance.IsFirstPerson && mActor == instance.mFirstPersonActor && !InteractionsManager.Instance.IsPlayingCutscene())
			{
				AnimatedScreenBackground.Instance.SetManualAlpha(0f);
				HudStateController.Instance.SetState(HudStateController.HudState.FPP);
			}
		}
		mOnGun = false;
	}

	private void UpdatePosition()
	{
		if (mActor.realCharacter != null && mActor.realCharacter.IsFirstPerson)
		{
			mActor.model.transform.position = new Vector3(0f, 5000f, 0f);
			mActor.model.transform.rotation = mFixedGun.GunnerLocator.transform.rotation;
		}
		else
		{
			mActor.model.transform.position = mFixedGun.GunnerLocator.transform.position;
			mActor.model.transform.rotation = mFixedGun.GunnerLocator.transform.rotation;
		}
		mActor.SetPosition(mFixedGun.GunnerLocator.position);
		mActor.transform.rotation = mFixedGun.GunnerLocator.rotation;
		mActor.awareness.LookDirection = mFixedGun.GunnerLocator.forward;
	}

	private void UpdateGunningAnimations()
	{
		float num = mFixedGun.CalculateAnimationPitchBlend(mFixedGun.AimAngles.x);
		float normalizedTime = mFixedGun.CalculateAnimationYawBlend(mFixedGun.AimAngles.y);
		float num2 = mFixedGun.CalculateAnimationPitchBlend(mLookAngles.x);
		float normalizedTime2 = 1f - mFixedGun.CalculateAnimationYawBlend(mLookAngles.y - mFixedGun.AimAngles.y);
		mFixedGun.AimLow.State.enabled = true;
		mFixedGun.AimLow.State.weight = num;
		mFixedGun.AimLow.State.speed = 0f;
		mFixedGun.AimLow.State.normalizedTime = normalizedTime;
		mFixedGun.AimLow.State.layer = 16;
		mFixedGun.AimHigh.State.enabled = true;
		mFixedGun.AimHigh.State.weight = 1f - num;
		mFixedGun.AimHigh.State.speed = 0f;
		mFixedGun.AimHigh.State.normalizedTime = normalizedTime;
		mFixedGun.AimHigh.State.layer = 16;
		mFixedGun.LookLow.State.enabled = true;
		mFixedGun.LookLow.State.weight = num2;
		mFixedGun.LookLow.State.speed = 0f;
		mFixedGun.LookLow.State.normalizedTime = normalizedTime2;
		mFixedGun.LookLow.State.layer = 17;
		mFixedGun.LookHigh.State.enabled = true;
		mFixedGun.LookHigh.State.weight = 1f - num2;
		mFixedGun.LookHigh.State.speed = 0f;
		mFixedGun.LookHigh.State.normalizedTime = normalizedTime2;
		mFixedGun.LookHigh.State.layer = 17;
		mFixedGun.Fire.State.enabled = true;
		mFixedGun.Fire.State.blendMode = AnimationBlendMode.Additive;
		mFixedGun.Fire.State.speed = 2f;
		mFixedGun.Fire.State.weight = ((!mActor.weapon.IsFiring()) ? 0f : 0.1f);
		mFixedGun.Fire.State.layer = 16;
	}
}
