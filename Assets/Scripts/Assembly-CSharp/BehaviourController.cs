using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : BaseActorComponent
{
	public enum AlertState
	{
		Casual = 0,
		Focused = 1,
		Reacting = 2,
		Suspicious = 3,
		Alerted = 4,
		Combat = 5,
		Unused = 123
	}

	public enum AlertEvent
	{
		HeardGunfire = 0,
		HeardFootsteps = 1,
		SeenEnemy = 2,
		SeenBody = 3,
		ToldAboutEnemy = 4
	}

	private const float kMinSuppressionDuration = 1f;

	private const float kCombatToAlertCooldownTime = 10f;

	private const float kSuppressionRadiusSqr = 9f;

	private const float kCasualToSuspiciousDelay = 2f;

	private const float kCasualToAlertedDelay = 2f;

	private const float kCasualToCombatDelay = 2.5f;

	private const float kFocusedToSuspiciousDelay = 1f;

	private const float kFocusedToAlertedDelay = 1f;

	private const float kFocusedToCombatDelay = 2f;

	private const float kSuspiciousToAlertedDelay = 1f;

	private const float kSuspiciousToCombatDelay = 1.5f;

	private const float kAlertedToCombatDelay = 0.5f;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public AlertState alertState;

	public bool mExposed;

	private bool playerControlled;

	public SoldierMarker SelectedMarkerObj;

	public float BroadcastPerceptionRange;

	private float runAwayTime;

	private float neglectedSince = 1000000f;

	public float IdealMaxBurstFireTimeMin;

	public float IdealMaxBurstFireTimeMax;

	public float TimeToPopUp;

	private bool suppressionScorePending;

	public Actor suppressionTarget;

	private bool suppressionSync;

	public Actor aimedShotTarget;

	private bool aimedShotSync;

	private float aimedShotExpiration;

	private float mAimedFor;

	private bool mCanExtendAimedShot = true;

	public Actor engagementTarget;

	public float engagementExpiration;

	public float engagementCommencement;

	public float noShootingUntil;

	public float crouchToAvoidBeingSeenUntil;

	public bool aggressive = true;

	private float suppressedUntil;

	public ActorMask bodiesIveSeen = new ActorMask(0u, "Bodies I've Seen");

	private ActorMask enemyKnowledgeHistory = new ActorMask(0u, "Enemy Knowledge History");

	private bool mResetAlerted;

	private ActorWrapper mPreferredTarget;

	private bool mTestStanding;

	private bool mShouldStandToAimedShot;

	private List<EngagementAssessor> mEngagementAssessors;

	public AlertState nextAlertState;

	private float nextAlertStateTime;

	public bool PlayerControlled
	{
		get
		{
			return playerControlled;
		}
		set
		{
			if (value)
			{
				aggressive = false;
			}
			playerControlled = value;
		}
	}

	public bool Suppressed
	{
		get
		{
			return Time.time < suppressedUntil;
		}
		set
		{
			if (value)
			{
				suppressedUntil = Time.time + 1f;
			}
			else
			{
				suppressedUntil = 0f;
			}
		}
	}

	public bool ResetAlerted
	{
		set
		{
			mResetAlerted = value;
		}
	}

	public ActorWrapper PreferredTarget
	{
		set
		{
			mPreferredTarget = value;
		}
	}

	private void ResetNeglect()
	{
		neglectedSince = Time.time;
		runAwayTime = 0f;
	}

	public bool SafeToStand()
	{
		return crouchToAvoidBeingSeenUntil < Time.time;
	}

	private void Awake()
	{
		mEngagementAssessors = new List<EngagementAssessor>();
		bodiesIveSeen.Set(0u);
		enemyKnowledgeHistory.Set(0u);
	}

	private void Start()
	{
		myActor.health.OnHealthChange += OnHealthChange;
		AuditoryAwarenessManager.Instance.OnHeard += OnHeard;
		SetupUnitNameDisplay();
		ResetState();
	}

	private void OnDestroy()
	{
		AuditoryAwarenessManager.Instance.OnHeard -= OnHeard;
		bodiesIveSeen.Release();
		enemyKnowledgeHistory.Release();
	}

	private void DealWithNeglect()
	{
		if (myActor.baseCharacter.IsMoving() || myActor.baseCharacter.IsRouting() || myActor.baseCharacter.IsFirstPerson || myActor.baseCharacter.IsDead())
		{
			ResetNeglect();
		}
	}

	private void Update()
	{
		if (GameplayController.Instance() == null)
		{
			return;
		}
		if (!myActor.baseCharacter.IsDead())
		{
			DealWithNeglect();
			ProcessFiring();
			UpdateAlertTimer();
			Actor actor = suppressionTarget ?? aimedShotTarget ?? engagementTarget;
			if (actor != null)
			{
				alertState = AlertState.Combat;
			}
			uint num = CheckForHearing();
			uint num2 = CheckForSpotting();
			enemyKnowledgeHistory.Include(num | num2);
		}
		if (PlayerControlled)
		{
			bool flag = GameplayController.Instance().IsInMultiSelectMode();
			if (SelectedMarkerObj != null)
			{
				if (GameplayController.Instance().IsSelected(myActor))
				{
					SelectedMarkerObj.MarkSelected(true);
					SelectedMarkerObj.MarkCanSelect(false);
				}
				else
				{
					SelectedMarkerObj.MarkSelected(false);
					if (flag)
					{
						SelectedMarkerObj.MarkCanSelect(true);
					}
					else
					{
						SelectedMarkerObj.MarkCanSelect(false);
					}
				}
			}
		}
		for (int num3 = mEngagementAssessors.Count - 1; num3 >= 0; num3--)
		{
			EngagementAssessor engagementAssessor = mEngagementAssessors[num3];
			if (!myActor.awareness.CanSee(engagementAssessor.Target))
			{
				mEngagementAssessors.RemoveAt(num3);
			}
		}
	}

	private void ResetState()
	{
		mEngagementAssessors.Clear();
		if (!mResetAlerted)
		{
			return;
		}
		GlobalKnowledgeManager.Instance().RefreshFactionEnemyMask(myActor.awareness.faction);
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.FriendsMask(myActor));
		uint num = 0u;
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			num |= a.awareness.EnemiesIKnowAbout;
		}
		num &= GKM.AliveMask;
		actorIdentIterator = ((num == 0) ? myActorIdentIterator.ResetWithMask(GKM.EnemiesMask(myActor)) : myActorIdentIterator.ResetWithMask(num));
		float num2 = float.MaxValue;
		Actor actor = null;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a != null)
			{
				float sqrMagnitude = (a.GetPosition() - myActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					actor = a;
				}
			}
		}
		if (actor != null)
		{
			myActor.awareness.BecomeAware(actor, actor.GetPosition());
		}
	}

	public void ShotAt()
	{
		if (!PlayerControlled || ActStructure.Instance.CurrentMissionSectionIsTutorial() || myActor.awareness.isInCover)
		{
			return;
		}
		TaskRoutine runningTask = myActor.tasks.GetRunningTask<TaskRoutine>();
		if (runningTask == null)
		{
			ResetNeglect();
		}
		else if (runningTask.Priority != TaskManager.Priority.LONG_TERM || runningTask.NoneCombatAI)
		{
			ResetNeglect();
		}
		else
		{
			if (!(Time.time - neglectedSince > 2f))
			{
				return;
			}
			if (runAwayTime == 0f)
			{
				runAwayTime = Time.time + 4f;
				if (!myActor.baseCharacter.IsCrouching())
				{
					myActor.Command("Crouch");
				}
			}
			if (Time.time > runAwayTime)
			{
				InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(myActor.GetPosition());
				inheritableMovementParams.holdCoverWhenBored = true;
				inheritableMovementParams.holdCoverWhenFlanked = true;
				new TaskMoveToCover(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default, inheritableMovementParams);
				ResetNeglect();
			}
		}
	}

	public bool IsWinning(Actor target)
	{
		foreach (EngagementAssessor mEngagementAssessor in mEngagementAssessors)
		{
			if (mEngagementAssessor.Target == target)
			{
				return mEngagementAssessor.IsWinning();
			}
		}
		EngagementAssessor engagementAssessor = new EngagementAssessor(myActor, target);
		mEngagementAssessors.Add(engagementAssessor);
		return engagementAssessor.IsWinning();
	}

	public void ResetEngagementAssessor(Actor target)
	{
		foreach (EngagementAssessor mEngagementAssessor in mEngagementAssessors)
		{
			if (mEngagementAssessor.Target == target)
			{
				mEngagementAssessor.Reset();
				break;
			}
		}
	}

	public void AimedShot(Actor target, bool sync)
	{
		ResetNeglect();
		if (!(myActor.realCharacter.Carried != null))
		{
			if (aimedShotTarget != target)
			{
				ClearAimedShotTarget();
				aimedShotTarget = target;
				mShouldStandToAimedShot = false;
				mTestStanding = false;
			}
			aimedShotExpiration = WorldHelper.ThisFrameTime + GlobalBalanceTweaks.kAimedShotDuration;
			mCanExtendAimedShot = true;
			mAimedFor = 0f;
			aimedShotSync = sync;
			TaskMoveTo runningTask = myActor.tasks.GetRunningTask<TaskMoveTo>();
			if (runningTask != null)
			{
				runningTask.CancelFinalLookAt();
			}
			ClearSuppressionTarget();
			ClearEngagementTarget();
		}
	}

	public void EngageTarget(Actor target)
	{
		engagementTarget = target;
		engagementExpiration = WorldHelper.ThisFrameTime + 5f;
		engagementCommencement = WorldHelper.ThisFrameTime + 0.25f;
		myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.None);
		ClearAimedShotTarget();
		ClearSuppressionTarget();
	}

	public void Suppress(Actor target, bool sync)
	{
		ResetNeglect();
		suppressionTarget = target;
		suppressionSync = sync;
		if (suppressionTarget != null)
		{
			suppressionScorePending = true;
		}
		ClearAimedShotTarget();
		ClearEngagementTarget();
	}

	private bool TargetOutOfHeadShotRange(Actor target)
	{
		IWeaponStats weaponStats = WeaponUtils.GetWeaponStats(myActor.weapon.ActiveWeapon);
		if (weaponStats == null || !weaponStats.IsHeadShotAllowed((target.GetPosition() - myActor.GetPosition()).sqrMagnitude))
		{
			return true;
		}
		return false;
	}

	public bool ClearAimedShotTarget()
	{
		if (aimedShotTarget != null)
		{
			UpdateAimShotHud(-1f, -1f);
			aimedShotTarget = null;
			aimedShotSync = false;
			return true;
		}
		return false;
	}

	public void ClearSuppressionTarget()
	{
		suppressionTarget = null;
		suppressionSync = false;
	}

	public void ClearEngagementTarget()
	{
		engagementTarget = null;
	}

	public bool NeedToWalk()
	{
		return aimedShotTarget != null || suppressionTarget != null || engagementTarget != null;
	}

	public Actor GetBestTarget(Actor nearestEnemyInView)
	{
		Actor actor = nearestEnemyInView;
		if (aimedShotTarget != null)
		{
			return aimedShotTarget;
		}
		if (engagementTarget != null)
		{
			return engagementTarget;
		}
		Actor preferredTarget = GetPreferredTarget();
		if (preferredTarget != null)
		{
			if (myActor.awareness.CanSee(preferredTarget))
			{
				actor = preferredTarget;
			}
		}
		else
		{
			actor = FirstPersonPenaliser.PickTarget(myActor, actor);
		}
		if (suppressionTarget != null)
		{
			actor = suppressionTarget;
		}
		return actor;
	}

	public bool IsGoodTarget(Actor target)
	{
		TBFAssert.DoAssert(target != null, "Makes no sense to call this func with null ref");
		if (target.realCharacter.IsSniper)
		{
			return false;
		}
		return true;
	}

	public Actor GetPreferredTarget()
	{
		return (!(mPreferredTarget != null)) ? null : mPreferredTarget.GetActor();
	}

	public void BlameEnemyForEvent(Vector3 eventOrigin)
	{
		Actor actor = null;
		float num = float.MaxValue;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.EnemiesMask(myActor) & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!a.baseCharacter.Docked)
			{
				float sqrMagnitude = (eventOrigin - a.GetPosition()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					actor = a;
					num = sqrMagnitude;
				}
			}
		}
		if (actor == null)
		{
			if (myActor.awareness.ChDefCharacterType != CharacterType.SentryGun && myActor.awareness.ChDefCharacterType != CharacterType.SecurityCamera)
			{
				new TaskFlushOutEnemies(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default);
			}
		}
		else
		{
			myActor.awareness.BecomeAware(actor, eventOrigin);
		}
		HandleAlertEvent(AlertEvent.HeardGunfire);
	}

	private void ProcessFiring()
	{
		if (GameController.Instance.mFirstPersonActor == myActor || myActor.realCharacter.Carried != null)
		{
			ClearAimedShotTarget();
			ClearSuppressionTarget();
			ClearEngagementTarget();
		}
		if (WorldHelper.ThisFrameTime > aimedShotExpiration)
		{
			if ((bool)aimedShotTarget)
			{
				myActor.speech.LostAimedShot();
			}
			ClearAimedShotTarget();
		}
		if (WorldHelper.ThisFrameTime > engagementExpiration)
		{
			ClearEngagementTarget();
		}
		if (myActor.weapon == null)
		{
			mExposed = false;
			return;
		}
		if (myActor.baseCharacter.IsMortallyWounded())
		{
			ClearAimedShotTarget();
		}
		if (aimedShotTarget != null && aimedShotTarget.baseCharacter.IsDead())
		{
			ImpactSFX.Instance.HeadshotKill.Play(aimedShotTarget.gameObject);
			ClearAimedShotTarget();
		}
		if (suppressionTarget != null && suppressionTarget.baseCharacter.IsDead())
		{
			ClearSuppressionTarget();
		}
		if (engagementTarget != null && engagementTarget.baseCharacter.IsDead())
		{
			ClearEngagementTarget();
		}
		if (noShootingUntil > Time.time || myActor.baseCharacter.IsInASetPiece)
		{
			mExposed = false;
			myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.None);
		}
		else if (suppressionTarget != null)
		{
			mExposed = false;
			ProcessFiring_Suppression();
		}
		else if (aimedShotTarget != null)
		{
			mExposed = false;
			ProcessFiring_AimedShot();
		}
		else if (engagementTarget != null)
		{
			mExposed = false;
			ProcessFiring_Engagement();
		}
		else
		{
			if (!OptimisationManager.Update(OptType.ProcessFiring, myActor))
			{
				return;
			}
			myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.None);
			if (Suppressed && myActor.awareness.isInCover)
			{
				mExposed = false;
				return;
			}
			TaskShoot runningTask = myActor.tasks.GetRunningTask<TaskShoot>();
			mExposed = Exposed();
			if ((!aggressive && runningTask == null && !mExposed) || myActor.weapon.IsReloading())
			{
				return;
			}
			if (alertState == AlertState.Combat)
			{
				if (myActor.awareness.ChDefCharacterType != CharacterType.SentryGun)
				{
					myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.Burst);
				}
				else
				{
					myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.FullAuto);
				}
				Actor target = myActor.weapon.GetTarget();
				if (target != null)
				{
					if (myActor.awareness.Obstructed(target) || target.baseCharacter.IsMortallyWounded())
					{
						myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.Hold);
					}
					else if ((myActor.realCharacter == null || !myActor.realCharacter.IsFirstPerson) && !myActor.awareness.isInCover && myActor.awareness.MustStandToShoot(target, true))
					{
						myActor.Command("Stand");
					}
				}
			}
			else if (alertState == AlertState.Alerted)
			{
				myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.Hold);
			}
		}
	}

	private bool Exposed()
	{
		if (!myActor.awareness.isInCover)
		{
			return true;
		}
		uint num = myActor.awareness.closestCoverPoint.noCoverAgainst & myActor.awareness.EnemiesIKnowAboutRecent & GKM.UpAndAboutMask;
		if (num == 0)
		{
			return false;
		}
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(num);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.baseCharacter.IsFirstPerson)
			{
				return true;
			}
			if (myActor.weapon != null && myActor.weapon.GetTarget() == a)
			{
				if (myActor.awareness.coverBooked.type == CoverPointCore.Type.ShootOver)
				{
					bool canShootFromCoverStance;
					bool canShootFromCrouch;
					myActor.awareness.coverBooked.DetermineStanceNecessaryToShoot((myActor.weapon.GetTarget().GetPosition() - myActor.GetPosition()).xz(), out canShootFromCoverStance, out canShootFromCrouch);
					return canShootFromCrouch;
				}
				return true;
			}
		}
		return false;
	}

	private void UpdateAimShotHud(float aimedFor, float takeShotAfterTime)
	{
		if (aimedShotTarget.realCharacter.HudMarker != null)
		{
			aimedShotTarget.realCharacter.HudMarker.Targetted(Mathf.Min(aimedFor, takeShotAfterTime), takeShotAfterTime);
		}
	}

	private void ProcessFiring_AimedShot()
	{
		ResetNeglect();
		float kAimedShotDelay = GlobalBalanceTweaks.kAimedShotDelay;
		myActor.aiGunHandler.DeadEye();
		myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.Hold);
		if (aimedShotSync && !CheckForSynchShooting())
		{
			UpdateAimShotHud(mAimedFor, kAimedShotDelay);
			return;
		}
		aimedShotSync = false;
		if (!myActor.weapon.ShootingIsAllowed())
		{
			UpdateAimShotHud(mAimedFor, kAimedShotDelay);
			return;
		}
		if (mShouldStandToAimedShot && myActor.baseCharacter.IsCrouching() && myActor.Pose.ActiveModule == PoseModuleSharedData.Modules.MoveAim)
		{
			myActor.Command("Stand");
		}
		Vector3 traceStart = ((!mTestStanding) ? myActor.baseCharacter.GetBulletOrigin() : myActor.baseCharacter.GetStandingBulletOrigin());
		Vector3 collision;
		if (WorldHelper.IsClearTrace(traceStart, aimedShotTarget.baseCharacter.GetHeadshotSpot(), out collision))
		{
			if (TargetOutOfHeadShotRange(aimedShotTarget))
			{
				UpdateAimShotHud(mAimedFor, kAimedShotDelay);
				return;
			}
			if (mAimedFor == 0f)
			{
				float num = Time.time + GlobalBalanceTweaks.kAimedShotDelay + 1f;
				if (num > aimedShotExpiration && mCanExtendAimedShot)
				{
					mCanExtendAimedShot = false;
					aimedShotExpiration = num;
				}
				if (mTestStanding)
				{
					mShouldStandToAimedShot = true;
				}
			}
			mAimedFor += Time.deltaTime * GlobalBalanceTweaks.AimedShotRate(myActor, aimedShotTarget);
		}
		else
		{
			if (mAimedFor != 0f)
			{
				UpdateAimShotHud(mAimedFor, kAimedShotDelay);
				return;
			}
			mTestStanding = !mTestStanding;
		}
		UpdateAimShotHud(mAimedFor, kAimedShotDelay);
		if (!(mAimedFor < GlobalBalanceTweaks.kAimedShotDelay))
		{
			myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.Burst);
		}
	}

	private void ProcessFiring_Suppression()
	{
		myActor.weapon.SetTarget(suppressionTarget);
		myActor.aiGunHandler.DeadEye();
		if (suppressionSync)
		{
			myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.Hold);
			if (!CheckForSynchShooting())
			{
				return;
			}
		}
		suppressionSync = false;
		if (!myActor.weapon.IsReloading())
		{
			myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.FullAuto);
			if (suppressionScorePending)
			{
				FirstPersonPenaliser.EventOccurred(FirstPersonPenaliser.EventEnum.ThirdPersonSuppression);
				suppressionScorePending = false;
			}
		}
		else
		{
			myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.None);
		}
	}

	private void ProcessFiring_Engagement()
	{
		if (Time.time < engagementCommencement)
		{
			myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.None);
			return;
		}
		if (!myActor.awareness.isInCover && myActor.awareness.MustStandToShoot(aimedShotTarget, true))
		{
			myActor.Command("Stand");
		}
		myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.Burst);
	}

	private bool CheckForSynchShooting()
	{
		bool result = true;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			Task runningTask = a.tasks.GetRunningTask();
			if (runningTask != null && !runningTask.mTaskSync.CanContinue(runningTask, TaskSynchroniser.SyncState.WeaponsFree))
			{
				myActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.None);
				return false;
			}
			if (a.behaviour.aimedShotTarget != null && a.behaviour.aimedShotSync && !a.weapon.ShootingIsAllowed())
			{
				result = false;
			}
			if (a.behaviour.suppressionTarget != null && a.behaviour.aimedShotSync && !a.weapon.ShootingIsAllowed())
			{
				result = false;
			}
		}
		return result;
	}

	private void OnHealthChange(object sender, EventArgs args)
	{
		HealthComponent.HeathChangeEventArgs heathChangeEventArgs = (HealthComponent.HeathChangeEventArgs)args;
		if (heathChangeEventArgs.From != null)
		{
			Actor component = heathChangeEventArgs.From.GetComponent<Actor>();
			if (component != null && !component.realCharacter.IsDead() && myActor.awareness.IsEnemy(component))
			{
				myActor.awareness.BecomeAware(component);
			}
		}
	}

	private uint CheckForHearing()
	{
		uint num = myActor.awareness.ICanHear & ~(uint)enemyKnowledgeHistory;
		if (num != 0)
		{
			ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(num);
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				if (a.awareness.dominantSound == DominantSoundType.Gunfire)
				{
					HandleAlertEvent(AlertEvent.HeardGunfire);
				}
				else
				{
					HandleAlertEvent(AlertEvent.HeardFootsteps);
				}
			}
		}
		return num;
	}

	private uint CheckForSpotting()
	{
		uint num = myActor.awareness.ICanSee & ~GKM.AliveMask & ~(uint)bodiesIveSeen;
		uint num2 = myActor.awareness.EnemiesIKnowAbout & ~(uint)enemyKnowledgeHistory;
		if (!myActor.behaviour.PlayerControlled && alertState < AlertState.Alerted && num != 0)
		{
			HandleAlertEvent(AlertEvent.SeenBody);
			ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(num);
			if (AllowedToReactToDeadBodies())
			{
				Actor actor = actorIdentIterator.NextActor();
				if (myActor.awareness.ChDefCharacterType != CharacterType.SecurityCamera)
				{
					TaskReaction taskReaction = (TaskReaction)myActor.tasks.GetRunningTask(typeof(TaskReaction));
					if (taskReaction == null)
					{
						new TaskReaction(myActor.tasks, actor, TaskManager.Priority.LONG_TERM, Task.Config.Default);
					}
					else
					{
						taskReaction.OverrideSearch(actor);
					}
				}
				else
				{
					TaskSecurityCamera taskSecurityCamera = (TaskSecurityCamera)myActor.tasks.GetRunningTask(typeof(TaskSecurityCamera));
					if (taskSecurityCamera != null)
					{
						taskSecurityCamera.SoundAlarm(actor, actor.GetPosition());
					}
				}
			}
			GameplayController.Instance().BroadcastEventShootOrder(myActor);
		}
		if (myActor.awareness.EnemiesICanSee != 0)
		{
			HandleAlertEvent(AlertEvent.SeenEnemy);
		}
		if (num2 != 0)
		{
			HandleAlertEvent(AlertEvent.ToldAboutEnemy);
			ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(num2);
			if (myActor.awareness.ChDefCharacterType != CharacterType.SecurityCamera)
			{
				Actor target = actorIdentIterator.NextActor();
				GameController.Instance.NotifyEnemySpotted(myActor, target);
			}
		}
		bodiesIveSeen.Include(num);
		return num2;
	}

	private bool AllowedToReactToDeadBodies()
	{
		switch (myActor.awareness.ChDefCharacterType)
		{
		case CharacterType.AutonomousGroundRobot:
		case CharacterType.SentryGun:
		case CharacterType.RiotShieldNPC:
		case CharacterType.RPG:
			return false;
		default:
			if (myActor.realCharacter.IsWindowLookout)
			{
				return false;
			}
			if (myActor.realCharacter.IsSniper)
			{
				return false;
			}
			return true;
		}
	}

	private void OnHeard(object sender, EventArgs args)
	{
		AuditoryAwarenessManager.HeardEventArgs heardEventArgs = (AuditoryAwarenessManager.HeardEventArgs)args;
		Actor component = heardEventArgs.Listener.GetComponent<Actor>();
		if (heardEventArgs.Target != null)
		{
			Actor component2 = heardEventArgs.Target.GetComponent<Actor>();
			OnHeardActor(component, component2);
		}
		else if (heardEventArgs.TargetEvent != null)
		{
			AuditoryAwarenessEvent targetEvent = heardEventArgs.TargetEvent;
			OnHeardAuditoryAwarenessEvent(component, targetEvent);
		}
		else
		{
			AuditoryAwarenessManager.AAWorldEvent targetWorldEvent = heardEventArgs.TargetWorldEvent;
			TBFAssert.DoAssert(targetWorldEvent != null, "HeardEventArgs - Target, TargetEvent and TargetWorldEvent should not all be null!");
			OnHeardAuditoryAwarenessWorldEvent(component, targetWorldEvent);
		}
	}

	private void OnHeardActor(Actor listenerActor, Actor targetActor)
	{
		if (!(listenerActor == null) && !(targetActor == null) && !(targetActor == myActor) && !listenerActor.realCharacter.IsDead() && !targetActor.realCharacter.IsDead() && !myActor.realCharacter.IsDead() && listenerActor.awareness.IsEnemy(targetActor) && listenerActor.awareness.AwarenessZonesInSync(targetActor) && (listenerActor == myActor || ((listenerActor.awareness.BroadcastsTo & myActor.ident) != 0 && myActor.awareness.AwarenessZonesInSync(listenerActor))) && IsInBroadcastRange(listenerActor) && (listenerActor.ears.GetNoiseRadius(targetActor.ears) > AudioResponseRanges.Footsteps || !targetActor.behaviour.SlowDownToAvoidBeingHeard(listenerActor)))
		{
			bool flag = myActor.awareness.BecomeAware(targetActor);
			if (!PlayerControlled && flag)
			{
				GameplayController.Instance().BroadcastEventShootOrder(myActor);
			}
		}
	}

	public bool SlowDownToAvoidBeingHeard(Actor listener)
	{
		if ((listener.awareness.EnemiesIKnowAbout & myActor.ident) != 0 && GKM.InCrowdOf(myActor).upToDate)
		{
			return false;
		}
		if (GameController.Instance.mFirstPersonActor == myActor)
		{
			return false;
		}
		if (!FactionHelper.WillSlowToAvoidBeingHeard(myActor.awareness.faction))
		{
			return false;
		}
		return myActor.baseCharacter.SlowDown();
	}

	private void OnHeardAuditoryAwarenessEvent(Actor listenerActor, AuditoryAwarenessEvent targetEvent)
	{
		if (!(listenerActor == null) && !(targetEvent == null) && !listenerActor.realCharacter.IsDead() && !myActor.realCharacter.IsDead() && (listenerActor == myActor || ((listenerActor.awareness.BroadcastsTo & myActor.ident) != 0 && myActor.awareness.AwarenessZonesInSync(listenerActor))) && IsInBroadcastRange(listenerActor) && !targetEvent.NoEnemyAwareness)
		{
			BlameEnemyForEvent(targetEvent.Origin);
		}
	}

	private void OnHeardAuditoryAwarenessWorldEvent(Actor listenerActor, AuditoryAwarenessManager.AAWorldEvent targetWorldEvent)
	{
		if (listenerActor == null || targetWorldEvent == null || listenerActor.realCharacter.IsDead() || myActor.realCharacter.IsDead() || !IsInBroadcastRange(listenerActor))
		{
			return;
		}
		if (listenerActor == myActor)
		{
			HandleAlertEvent(AlertEvent.HeardGunfire);
			if (!PlayerControlled)
			{
				BlameEnemyForEvent(targetWorldEvent.Origin);
			}
		}
		else if ((listenerActor.awareness.BroadcastsTo & myActor.ident) != 0 && myActor.awareness.AwarenessZonesInSync(listenerActor))
		{
			HandleAlertEvent(AlertEvent.ToldAboutEnemy);
			if (!PlayerControlled)
			{
				BlameEnemyForEvent(targetWorldEvent.Origin);
			}
		}
	}

	private bool IsInBroadcastRange(Actor broadcaster)
	{
		if (broadcaster == null)
		{
			return false;
		}
		if (myActor.awareness.ChDefCharacterType != CharacterType.Human)
		{
			return false;
		}
		if (broadcaster == myActor)
		{
			return true;
		}
		float sqrMagnitude = (broadcaster.GetPosition() - myActor.GetPosition()).sqrMagnitude;
		return sqrMagnitude < BroadcastPerceptionRange * BroadcastPerceptionRange;
	}

	private void SetupUnitNameDisplay()
	{
		if (!(SelectedMarkerObj == null))
		{
			SelectedMarkerObj.SetDisplayName(myActor.realCharacter.UnitName);
		}
	}

	private void UpdateAlertTimer()
	{
		if (Time.time > nextAlertStateTime && this.alertState != nextAlertState)
		{
			AlertState alertState = nextAlertState;
			if ((alertState == AlertState.Alerted || alertState == AlertState.Combat) && InPassiveAlertState())
			{
				SetAlertPerceptionState();
				GameplayController.Instance().BroadcastEventShootOrder(myActor);
			}
			this.alertState = nextAlertState;
			if (this.alertState == AlertState.Combat)
			{
				nextAlertStateTime = Time.time + 10f;
				nextAlertState = AlertState.Alerted;
			}
		}
		if (this.alertState == AlertState.Combat && (myActor.awareness.EnemiesICanSee & GKM.AliveMask) != 0)
		{
			nextAlertStateTime = Time.time + 2f;
		}
	}

	public void SetAlertPerceptionState()
	{
		TryExtendAwarenessRanges();
		if (myActor.awareness.faction == FactionHelper.Category.Enemy || myActor.awareness.faction == FactionHelper.Category.SoloEnemy)
		{
			GameplayController.Instance().FireAlarmEvents(myActor);
		}
	}

	private void TryExtendAwarenessRanges()
	{
		if (!myActor.tasks.IsRunningTask(typeof(TaskSentryGun)) && !myActor.tasks.IsRunningTask(typeof(TaskSecurityCamera)) && !myActor.realCharacter.IsSniper && !myActor.realCharacter.IsWindowLookout && (SectionTypeHelper.IsAGMG() || !myActor.realCharacter.IsUsingFixedGun))
		{
			myActor.awareness.VisionRange = 100f;
			if (myActor.ears != null)
			{
				myActor.ears.Range = 100f;
				myActor.ears.RangeSqr = myActor.ears.Range * myActor.ears.Range;
			}
		}
	}

	private void HandleAlertEvent(AlertEvent ae)
	{
		switch (alertState)
		{
		case AlertState.Casual:
			HandleAlertEvent_Casual(ae);
			break;
		case AlertState.Focused:
			HandleAlertEvent_Focused(ae);
			break;
		case AlertState.Reacting:
			HandleAlertEvent_Reacting(ae);
			break;
		case AlertState.Suspicious:
			HandleAlertEvent_Suspicious(ae);
			break;
		case AlertState.Alerted:
			HandleAlertEvent_Alerted(ae);
			break;
		case AlertState.Combat:
			HandleAlertEvent_Combat(ae);
			break;
		}
	}

	private void HandleAlertEvent_Casual(AlertEvent ae)
	{
		switch (ae)
		{
		case AlertEvent.HeardFootsteps:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 2f);
			break;
		case AlertEvent.HeardGunfire:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 2f);
			break;
		case AlertEvent.SeenEnemy:
			SetReacting();
			ElevateAlertState(AlertState.Combat, 2.5f);
			break;
		case AlertEvent.SeenBody:
		case AlertEvent.ToldAboutEnemy:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 2f);
			break;
		}
	}

	private void HandleAlertEvent_Focused(AlertEvent ae)
	{
		switch (ae)
		{
		case AlertEvent.HeardFootsteps:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		case AlertEvent.HeardGunfire:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		case AlertEvent.SeenEnemy:
			SetReacting();
			ElevateAlertState(AlertState.Combat, 2f);
			break;
		case AlertEvent.SeenBody:
		case AlertEvent.ToldAboutEnemy:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		}
	}

	private void HandleAlertEvent_Suspicious(AlertEvent ae)
	{
		switch (ae)
		{
		case AlertEvent.HeardFootsteps:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		case AlertEvent.HeardGunfire:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		case AlertEvent.SeenEnemy:
			SetReacting();
			ElevateAlertState(AlertState.Combat, 1.5f);
			break;
		case AlertEvent.SeenBody:
		case AlertEvent.ToldAboutEnemy:
			SetReacting();
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		}
	}

	private void HandleAlertEvent_Reacting(AlertEvent ae)
	{
		switch (ae)
		{
		case AlertEvent.HeardFootsteps:
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		case AlertEvent.HeardGunfire:
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		case AlertEvent.SeenEnemy:
			ElevateAlertState(AlertState.Combat, 2f);
			break;
		case AlertEvent.SeenBody:
		case AlertEvent.ToldAboutEnemy:
			ElevateAlertState(AlertState.Alerted, 1f);
			break;
		}
	}

	private void HandleAlertEvent_Alerted(AlertEvent ae)
	{
		switch (ae)
		{
		case AlertEvent.HeardFootsteps:
			break;
		case AlertEvent.HeardGunfire:
			break;
		case AlertEvent.SeenEnemy:
			ElevateAlertState(AlertState.Combat, 0.5f);
			break;
		case AlertEvent.SeenBody:
		case AlertEvent.ToldAboutEnemy:
			break;
		}
	}

	private void HandleAlertEvent_Combat(AlertEvent ae)
	{
		switch (ae)
		{
		case AlertEvent.HeardFootsteps:
			break;
		case AlertEvent.HeardGunfire:
			break;
		case AlertEvent.SeenEnemy:
			break;
		case AlertEvent.ToldAboutEnemy:
			break;
		case AlertEvent.SeenBody:
			break;
		}
	}

	private void SetReacting()
	{
		if (alertState != AlertState.Reacting)
		{
			GameplayController.Instance().FireOnReactEvents(myActor);
			alertState = AlertState.Reacting;
		}
	}

	private void ElevateAlertState(AlertState to, float delay)
	{
		if (to > nextAlertState)
		{
			delay += Time.time;
			nextAlertState = to;
			if (delay > nextAlertStateTime)
			{
				nextAlertStateTime = delay;
			}
		}
		if (to >= AlertState.Alerted && alertState < AlertState.Alerted && myActor.realCharacter != null && myActor.realCharacter.CanTriggerAlarms && myActor.navAgent != null && myActor.navAgent.enabled && AlarmManager.Instance != null && !AlarmManager.Instance.AlarmSounding && !myActor.tasks.IsRunningTask<TaskUseAlarmPanel>())
		{
			AlarmPanel nearestAlarmPanel = AlarmManager.Instance.GetNearestAlarmPanel(myActor);
			if (nearestAlarmPanel != null && nearestAlarmPanel.CanBeTriggered())
			{
				myActor.tasks.CancelTasks<TaskPlayAnimation>();
				myActor.tasks.CancelTasks<TaskPlayRandomAnimations>();
				new TaskUseAlarmPanel(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default, nearestAlarmPanel);
			}
		}
	}

	private void DecreaseAlertState(AlertState to, float delay)
	{
		if (to < nextAlertState)
		{
			delay += Time.time;
			nextAlertState = to;
			if (delay > nextAlertStateTime)
			{
				nextAlertStateTime = delay;
			}
		}
	}

	public bool InPassiveAlertState()
	{
		return IsPassiveAlertState(alertState);
	}

	public bool InActiveAlertState()
	{
		return IsActiveAlertState(alertState);
	}

	public static bool IsActiveAlertState(AlertState s)
	{
		return s > AlertState.Suspicious;
	}

	public static bool IsPassiveAlertState(AlertState s)
	{
		return s <= AlertState.Suspicious;
	}
}
