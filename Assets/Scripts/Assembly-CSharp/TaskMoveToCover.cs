using System;
using UnityEngine;

public class TaskMoveToCover : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		CoverPointInvalidated = 1
	}

	private enum State
	{
		Start = 0,
		MovingToCoverPoint = 1,
		HoldingCornerCover = 2,
		HoldingLowCover = 3,
		HoldingHighWallCover = 4,
		Hitching = 5
	}

	private enum CoverState
	{
		Suppressed = 0,
		EdgeOut1 = 1,
		EdgeOut2 = 2,
		EdgeOut3 = 3,
		PullingBack = 4
	}

	private enum PopUpBehaviour
	{
		Nothing = 0,
		StoodUp = 1,
		Hunched = 2,
		PeekedOver = 3,
		SteppedOut = 4,
		Count = 5
	}

	private const float kPreventFaffingMoveAim = 2f;

	private const float kPreventFaffingCrouchCover = 1f;

	private const float kInitialPopUpDelay = 0.75f;

	private const float kEdgeOutRate = 0.2f;

	private const float kEdgeOutReactionDelay = 0.75f;

	public const float kShootFromCoverConeConst = -0.25f;

	private bool ignoreDodgeBecauseFPCanSeeUsAnyway;

	private UnityEngine.AI.NavMeshHit myNavMeshHit;

	private bool orderPopDown = true;

	private bool orderPopUp = true;

	private float dontFaffAboutTime;

	private static int[,] cumulativeOdds = new int[5, 2];

	private static int oddsCount = 0;

	private static int oddsTotal = 0;

	private float timeOutOfCover;

	private float maxTimeOutOfCover;

	private bool forceEdgeOut;

	private PopUpBehaviour whatIDidLastTime;

	private PopUpBehaviour whatImDoingNow;

	private static int[,] behaviourChances = new int[5, 5]
	{
		{ 0, 10, 10, 10, 10 },
		{ 0, 20, 20, 10, 100 },
		{ 0, 20, 20, 10, 100 },
		{ 0, 50, 50, 10, 100 },
		{ 0, 50, 50, 10, 10 }
	};

	private static int[,] behaviourChangeHitchChance = new int[5, 5]
	{
		{ 0, 10, 10, 10, 10 },
		{ 0, 100, 50, 50, 0 },
		{ 0, 50, 100, 50, 0 },
		{ 0, 50, 50, 100, 0 },
		{ 0, 10, 10, 10, 50 }
	};

	private static Vector2[] popUpPositions = new Vector2[5]
	{
		new Vector2(0f, 0f),
		new Vector2(0f, 0f),
		new Vector2(0f, 0f),
		new Vector2(0f, 0f),
		new Vector2(1f, 0f)
	};

	private CoverState coverState;

	private Flags mFlags;

	private State mState;

	private InheritableMovementParams mMoveParams;

	private CoverPointCore mCoverPoint;

	private float mTimeInCoverPassive;

	private float mPassiveTimeLimit;

	private RealCharacter realCharacter;

	private float edgeOutTimer;

	private float inCoverStartTime;

	private float mPopUpDelay;

	private string popUpCommand = string.Empty;

	private int hitchLeftMax;

	private int hitchRightMax;

	private int hitchPos;

	private Vector3 hitchOffset;

	private int targetHitchPos;

	private Vector3 hitchDir;

	private float hitchingUntil;

	private int hitchChance;

	private float delayLeavingUntil;

	private float mStepDirection;

	private Vector3 poppedUpOffset;

	public float PassiveTimeLimit
	{
		set
		{
			mPassiveTimeLimit = value;
		}
	}

	public TaskMoveToCover(TaskManager owner, TaskManager.Priority priority, Config flags, InheritableMovementParams moveParams)
		: base(owner, priority, flags)
	{
		Initialise(moveParams);
		mCoverPoint = mActor.awareness.GetValidCoverNearestSpecifiedPosition(moveParams.mDestination, 25f, 0f, true, 0f);
		if (mCoverPoint != null)
		{
			mMoveParams.mDestination = mCoverPoint.gamePos;
			mActor.awareness.BookCoverPoint(mCoverPoint, 0f);
		}
	}

	public TaskMoveToCover(TaskManager owner, TaskManager.Priority priority, Config flags, CoverPointCore specifiedCoverPoint, InheritableMovementParams moveParams)
		: base(owner, priority, flags)
	{
		Initialise(moveParams);
		TBFAssert.DoAssert(specifiedCoverPoint != null, "TakeMoveToCover called with a null specified cover point. Use the appropriate constructor if this is desired!");
		mCoverPoint = specifiedCoverPoint;
		mActor.awareness.BookCoverPoint(mCoverPoint, 0f);
		mMoveParams.mDestination = mCoverPoint.gamePos;
	}

	public override void Update()
	{
		if ((Time.frameCount & 0x1F) == (mActor.quickIndex & 0x1F))
		{
			orderPopUp = true;
			orderPopDown = true;
		}
		switch (mState)
		{
		case State.Start:
			inCoverStartTime = 0f;
			if (mCoverPoint != null)
			{
				if ((mCoverPoint.noCoverAgainst & mActor.awareness.EnemiesIKnowAboutRecent) == 0)
				{
					mMoveParams.FinalLookDirection = mCoverPoint.StandFacing;
					mMoveParams.LookTowardsDist = 2f;
				}
				else
				{
					mMoveParams.mFinalLookAtValid = false;
				}
				if (!mCoverPoint.IsHighCornerCover())
				{
					mMoveParams.stanceAtEnd = InheritableMovementParams.StanceOrder.CrouchInCover;
					mMoveParams.DestinationThreshold = 0.25f;
				}
				else
				{
					mMoveParams.stanceAtEnd = InheritableMovementParams.StanceOrder.StandInCover;
					mMoveParams.DestinationThreshold = 0.25f;
				}
				mMoveParams.stopDead = true;
				new TaskRouteTo(mOwner, base.Priority, Config.ClearAllCurrentType, mMoveParams);
			}
			else
			{
				mFlags |= Flags.CoverPointInvalidated;
			}
			mState = State.MovingToCoverPoint;
			break;
		case State.MovingToCoverPoint:
			if (mActor.awareness.isInCover)
			{
				inCoverStartTime = Time.time;
				mActor.awareness.PoppedOutOfCover = false;
				whatImDoingNow = PopUpBehaviour.Nothing;
				whatIDidLastTime = PopUpBehaviour.Nothing;
				poppedUpOffset = Vector3.zero;
				if (mCoverPoint.IsHighCornerCover())
				{
					mPopUpDelay = 0.75f;
					mState = State.HoldingCornerCover;
					coverState = CoverState.Suppressed;
					edgeOutTimer = 0f;
				}
				else if (mCoverPoint.type == CoverPointCore.Type.HighWall)
				{
					mState = State.HoldingHighWallCover;
					mActor.Command("Crouch");
				}
				else
				{
					mPopUpDelay = 0.75f;
					mState = State.HoldingLowCover;
					orderPopUp = true;
					orderPopDown = true;
					mActor.realCharacter.ImposeLookDirection(-mCoverPoint.snappedNormal);
					hitchPos = 0;
					targetHitchPos = 0;
					hitchDir = mCoverPoint.snappedTangent * 0.5f;
					if (mCoverPoint.stepOutLeft)
					{
						hitchLeftMax = 0;
					}
					else
					{
						hitchLeftMax = Mathf.Max(-4, (int)(mCoverPoint.minProprietaryExtent * 2f));
					}
					if (mCoverPoint.stepOutRight)
					{
						hitchRightMax = 0;
					}
					else
					{
						hitchRightMax = Mathf.Min(4, (int)(mCoverPoint.maxProprietaryExtent * 2f));
					}
				}
				if ((mCoverPoint.noCoverAgainst & mActor.awareness.EnemiesIKnowAboutRecent) != 0)
				{
					mPopUpDelay = 0f;
				}
			}
			else
			{
				mFlags |= Flags.CoverPointInvalidated;
			}
			break;
		case State.Hitching:
			ProcessHitching();
			mActor.Pose.onAxisTrans.position = mCoverPoint.gamePos + hitchOffset;
			mActor.SetPosition(mActor.Pose.onAxisTrans.position);
			mActor.awareness.keepCoverBooked = true;
			if (Time.time > hitchingUntil)
			{
				if (hitchPos == targetHitchPos)
				{
					mState = State.HoldingLowCover;
				}
				else
				{
					HitchIntoPosition();
				}
			}
			break;
		case State.HoldingHighWallCover:
			mTimeInCoverPassive += Time.deltaTime;
			if (mActor.weapon.IsFiring())
			{
				mTimeInCoverPassive = 0f;
			}
			mActor.realCharacter.SetDefaultLookDirection(mCoverPoint.snappedNormal);
			break;
		case State.HoldingLowCover:
		{
			ProcessHitching();
			mTimeInCoverPassive += Time.deltaTime;
			if (mActor.weapon.IsFiring())
			{
				mTimeInCoverPassive = 0f;
			}
			mActor.awareness.keepCoverBooked = true;
			if (FirstPersonPenaliser.ShouldDodge(mActor))
			{
				mPopUpDelay = 1f;
				ConsiderHitching();
			}
			Vector3 to = mCoverPoint.snappedTangent * (popUpPositions[(int)whatImDoingNow].x * mStepDirection) + mCoverPoint.snappedNormal * popUpPositions[(int)whatImDoingNow].y;
			WorldHelper.ExpBlend(ref poppedUpOffset, to, 0.05f, 0.02f);
			Vector3 position = mCoverPoint.gamePos + poppedUpOffset + hitchOffset;
			mActor.Pose.onAxisTrans.position = position;
			mActor.SetPosition(position);
			bool canShootFromCoverStance;
			bool canShootFromCrouch;
			mCoverPoint.DetermineStanceNecessaryToShoot(mActor.awareness.LookDirectionXZ, out canShootFromCoverStance, out canShootFromCrouch);
			bool flag2 = true;
			if (mActor.weapon != null && canShootFromCrouch)
			{
				Actor target = mActor.weapon.GetTarget();
				if (target != null)
				{
					Vector3 rhs = target.GetPosition() - mActor.GetPosition();
					float num = Vector3.Dot(mCoverPoint.snappedTangent, rhs);
					if (num < 0f)
					{
						flag2 = false;
					}
					else
					{
						float num2 = Vector3.Dot(mCoverPoint.snappedNormal, rhs);
						if (!(num2 > 0f))
						{
						}
					}
				}
			}
			if (flag2 && !forceEdgeOut && (mActor.weapon.DesiredFiringBehaviour() == SoldierFiringState.FireType.None || mPopUpDelay > 0f || mActor.aiGunHandler.OnCooldown()))
			{
				mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.CrouchCover);
				if (orderPopDown)
				{
					mActor.Command("PopDown");
					orderPopUp = true;
					orderPopDown = false;
				}
				if (mActor.awareness.PoppedOutOfCover)
				{
					mActor.awareness.PoppedOutOfCover = false;
					hitchingUntil = Time.time + 0.5f;
				}
				whatImDoingNow = PopUpBehaviour.Nothing;
				mActor.realCharacter.SetDefaultLookDirection(-mCoverPoint.snappedNormal);
				ConsiderHitching();
				dontFaffAboutTime = 0f;
			}
			else
			{
				if (!canShootFromCoverStance && dontFaffAboutTime < Time.time)
				{
					if (mActor.Pose.ActiveModule == PoseModuleSharedData.Modules.CrouchCover)
					{
						mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.MoveAim);
						dontFaffAboutTime = Time.time + 2f;
					}
					if (canShootFromCrouch)
					{
						mActor.realCharacter.Crouch();
						mActor.awareness.PoppedOutOfCover = false;
					}
					else
					{
						mActor.realCharacter.Stand();
						mActor.awareness.PoppedOutOfCover = true;
					}
					mActor.behaviour.crouchToAvoidBeingSeenUntil = Time.time + 0.5f;
					orderPopUp = true;
					orderPopDown = true;
				}
				else if (canShootFromCoverStance && dontFaffAboutTime < Time.time)
				{
					if (!mActor.awareness.PoppedOutOfCover)
					{
						ChoosePopDirection();
						mActor.awareness.PoppedOutOfCover = true;
						orderPopUp = true;
					}
					if (mActor.Pose.ActiveModule == PoseModuleSharedData.Modules.MoveAim)
					{
						dontFaffAboutTime = 1f;
						mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.CrouchCover);
					}
					if (orderPopUp)
					{
						mActor.Command(popUpCommand);
						orderPopUp = false;
						orderPopDown = true;
					}
				}
				mActor.realCharacter.SetDefaultLookDirection(-mCoverPoint.snappedNormal);
			}
			mPopUpDelay -= Time.deltaTime;
			break;
		}
		case State.HoldingCornerCover:
			mActor.awareness.keepCoverBooked = true;
			mTimeInCoverPassive += Time.deltaTime;
			if (mActor.weapon.IsFiring())
			{
				mTimeInCoverPassive = 0f;
			}
			switch (coverState)
			{
			case CoverState.Suppressed:
				mActor.awareness.PoppedOutOfCover = false;
				if (FlankedInHighCover())
				{
					if (mActor.Pose.ActiveModule == PoseModuleSharedData.Modules.HighCornerCover)
					{
						mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.MoveAim);
					}
					break;
				}
				if (mActor.Pose.ActiveModule == PoseModuleSharedData.Modules.MoveAim)
				{
					mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.HighCornerCover);
				}
				if ((mActor.weapon.DesiredFiringBehaviour() != 0 || mActor.behaviour.aimedShotTarget != null || forceEdgeOut) && mTimeInCoverPassive > 1f)
				{
					bool flag = false;
					Actor actor = mActor.behaviour.suppressionTarget ?? mActor.behaviour.aimedShotTarget ?? mActor.behaviour.engagementTarget;
					if (actor != null && mActor.awareness.MustStandToShoot(actor, true))
					{
						flag = true;
					}
					else if (UnityEngine.Random.Range(0, 256) > 128)
					{
						flag = true;
					}
					if (flag)
					{
						mActor.realCharacter.Stand();
					}
					else
					{
						mActor.realCharacter.Crouch();
					}
					coverState = CoverState.EdgeOut1;
					edgeOutTimer = 0f;
					mActor.Command("Edge1");
					ignoreDodgeBecauseFPCanSeeUsAnyway = FirstPersonPenaliser.ShouldDodge(mActor);
				}
				mActor.realCharacter.SetDefaultLookDirection(mCoverPoint.snappedTangent + mCoverPoint.snappedNormal);
				break;
			case CoverState.EdgeOut1:
				mActor.awareness.PoppedOutOfCover = true;
				if (ShouldDiveBack())
				{
					mActor.Command("Dive");
					coverState = CoverState.Suppressed;
					edgeOutTimer = 0f;
					mTimeInCoverPassive = 0f;
					break;
				}
				CheckForHoldFire(1);
				mActor.realCharacter.SetDefaultLookDirection(mCoverPoint.snappedTangent - mCoverPoint.snappedNormal * 0.3f);
				if (EdgeOut(1, "Dive", "Edge2", true))
				{
					coverState = CoverState.EdgeOut2;
				}
				break;
			case CoverState.EdgeOut2:
				mActor.awareness.PoppedOutOfCover = true;
				if (ShouldDiveBack())
				{
					mActor.Command("Dive");
					coverState = CoverState.Suppressed;
					edgeOutTimer = 0f;
					break;
				}
				CheckForHoldFire(2);
				mActor.realCharacter.SetDefaultLookDirection(mCoverPoint.snappedTangent * 0.5f - mCoverPoint.snappedNormal * 0.5f);
				if (EdgeOut(2, "Dive", "Edge3", true))
				{
					coverState = CoverState.EdgeOut3;
				}
				break;
			case CoverState.EdgeOut3:
				mActor.awareness.PoppedOutOfCover = true;
				if (ShouldDiveBack())
				{
					mActor.Command("Dive");
					coverState = CoverState.Suppressed;
					edgeOutTimer = 0f;
				}
				else
				{
					CheckForHoldFire(3);
					mActor.realCharacter.SetDefaultLookDirection(mCoverPoint.snappedTangent * 0.3f - mCoverPoint.snappedNormal);
					EdgeOut(3, "Dive", string.Empty, false);
				}
				break;
			}
			break;
		}
	}

	private void CheckForHoldFire(int step)
	{
		if (!(mActor.weapon == null))
		{
			Actor target = mActor.weapon.GetTarget();
			if (!(target == null) && mCoverPoint.CoverProvidedAgainstSteppingOut(target, step) == CoverTable.CoverProvided.Full)
			{
				mActor.weapon.SetDesiredFiringBehaviour(SoldierFiringState.FireType.Hold);
				mActor.Pose.RestrictAiming();
			}
		}
	}

	private void ClearOdds()
	{
		oddsCount = 0;
		oddsTotal = 0;
	}

	private void AppendOdds(PopUpBehaviour val)
	{
		int num = BehaviourChance(whatIDidLastTime, val);
		cumulativeOdds[oddsCount, 0] = (int)val;
		cumulativeOdds[oddsCount++, 1] = oddsTotal + num;
		oddsTotal += num;
	}

	private PopUpBehaviour PickBehaviourUsingOdds()
	{
		int num = UnityEngine.Random.Range(0, oddsTotal);
		for (int i = 0; i < oddsCount; i++)
		{
			if (cumulativeOdds[i, 1] >= num)
			{
				return (PopUpBehaviour)cumulativeOdds[i, 0];
			}
		}
		if (oddsCount > 0)
		{
			return (PopUpBehaviour)cumulativeOdds[oddsCount - 1, 0];
		}
		return PopUpBehaviour.StoodUp;
	}

	private void ChoosePopDirection()
	{
		popUpCommand = "PopUp";
		Actor target = mActor.weapon.GetTarget();
		hitchChance += 10;
		if (mActor.behaviour.PlayerControlled || target == null)
		{
			return;
		}
		ClearOdds();
		AppendOdds(PopUpBehaviour.StoodUp);
		Vector3 vector = target.GetPosition() - mActor.GetPosition();
		if ((mCoverPoint.stepOutLeft || mCoverPoint.stepOutRight) && hitchPos == 0)
		{
			float num = Vector3.Dot(vector, mCoverPoint.snappedNormal);
			float num2 = Vector3.Dot(vector, mCoverPoint.snappedTangent);
			if (num * num > num2 * num2 && num2 / num < 0.2f)
			{
				AppendOdds(PopUpBehaviour.SteppedOut);
			}
		}
		float num3 = vector.y * Mathf.Abs(vector.y) / (vector.xz().sqrMagnitude + 0.01f);
		if (num3 < 0.15f && num3 > -0.1f)
		{
			AppendOdds(PopUpBehaviour.PeekedOver);
		}
		AppendOdds(PopUpBehaviour.Hunched);
		whatImDoingNow = PickBehaviourUsingOdds();
		hitchChance += behaviourChangeHitchChance[(int)whatIDidLastTime, (int)whatImDoingNow];
		mStepDirection = 1f;
		whatIDidLastTime = whatImDoingNow;
		switch (whatImDoingNow)
		{
		case PopUpBehaviour.StoodUp:
			break;
		case PopUpBehaviour.Hunched:
			popUpCommand = "Hunch";
			break;
		case PopUpBehaviour.PeekedOver:
			popUpCommand = "PeekOver";
			break;
		case PopUpBehaviour.SteppedOut:
			if (mCoverPoint.stepOutLeft)
			{
				mStepDirection = -0.5f;
				popUpCommand = "StepLeft";
			}
			else
			{
				popUpCommand = "StepRight";
			}
			break;
		}
	}

	private bool ShouldDiveBack()
	{
		if (forceEdgeOut || mActor.behaviour.aimedShotTarget != null)
		{
			return false;
		}
		if (mActor.weapon.DesiredFiringBehaviour() == SoldierFiringState.FireType.None)
		{
			return true;
		}
		if (!ignoreDodgeBecauseFPCanSeeUsAnyway && FirstPersonPenaliser.ShouldDodge(mActor))
		{
			return true;
		}
		if (timeOutOfCover > maxTimeOutOfCover)
		{
			maxTimeOutOfCover = UnityEngine.Random.Range(3f, 6f);
			timeOutOfCover = 0f;
			return true;
		}
		timeOutOfCover += Time.deltaTime;
		return false;
	}

	private bool FlankedInHighCover()
	{
		if (mActor.behaviour.mExposed)
		{
			return true;
		}
		Actor actor = mActor.behaviour.aimedShotTarget ?? mActor.behaviour.suppressionTarget ?? mActor.behaviour.engagementTarget;
		if (actor == null)
		{
			return false;
		}
		if (mCoverPoint.CoverProvidedAgainstSteppingOut(actor, 0) == CoverTable.CoverProvided.None)
		{
			return true;
		}
		return false;
	}

	private bool EdgeOut(int stage, string diveCommand, string edgeCommand, bool canGoFurther)
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(mActor.awareness.EnemiesICanSee & GKM.AliveMask);
		bool flag = false;
		edgeOutTimer += Time.deltaTime;
		if (edgeOutTimer >= 0.75f)
		{
			flag = canGoFurther;
			if (mActor.awareness.EnemiesICanSee != 0)
			{
				Actor a;
				while (actorIdentIterator.NextActor(out a))
				{
					if ((!(mActor.behaviour.aimedShotTarget == null) && !(a == mActor.behaviour.aimedShotTarget)) || (!(mActor.behaviour.suppressionTarget == null) && !(a == mActor.behaviour.suppressionTarget)) || (!(mActor.behaviour.engagementTarget == null) && !(a == mActor.behaviour.engagementTarget)))
					{
						continue;
					}
					switch (mCoverPoint.CoverProvidedAgainstSteppingOut(a, stage))
					{
					case CoverTable.CoverProvided.None:
						mActor.Command(diveCommand);
						coverState = CoverState.Suppressed;
						mTimeInCoverPassive = 0f;
						return false;
					case CoverTable.CoverProvided.High:
						if (canGoFurther)
						{
							flag = false;
							mTimeInCoverPassive = 0f;
						}
						break;
					}
				}
			}
		}
		if (flag)
		{
			mActor.Command(edgeCommand);
			edgeOutTimer = 0f;
			timeOutOfCover = 0f;
		}
		return flag;
	}

	public override bool HasFinished()
	{
		if (mActor.behaviour.Suppressed)
		{
			delayLeavingUntil = Time.time + 4f;
		}
		if (delayLeavingUntil > Time.time)
		{
			return false;
		}
		if (mConsultant != null && mConsultant.Consult(this))
		{
			return true;
		}
		if (mState == State.Start)
		{
			return false;
		}
		if ((base.ConfigFlags & Config.AbortOnAlert) != 0 && mActor.awareness.IsInCover() && mActor.behaviour.InActiveAlertState())
		{
			return true;
		}
		if (!mActor.tether.IsWithinTether())
		{
			return true;
		}
		if ((mFlags & Flags.CoverPointInvalidated) != 0)
		{
			return true;
		}
		if (Time.time - inCoverStartTime < mMoveParams.minimumCoverTime)
		{
			return false;
		}
		if (!mActor.awareness.CoverValid())
		{
			mActor.awareness.CancelCover();
			return true;
		}
		if (!mMoveParams.holdCoverWhenFlanked && !mActor.awareness.CoverSafe())
		{
			return true;
		}
		if (mState == State.HoldingCornerCover || mState == State.HoldingLowCover || mState == State.HoldingHighWallCover)
		{
			if (!mMoveParams.holdCoverWhenBored && mTimeInCoverPassive >= mPassiveTimeLimit)
			{
				mActor.awareness.CancelCover();
				return true;
			}
			return false;
		}
		if (mState == State.MovingToCoverPoint || mState == State.Hitching)
		{
			return false;
		}
		TBFAssert.DoAssert(false, string.Format("TaskMoveToCover - logic failure, shouldn't drop through to here. State={0} Flags={1}", mState, mFlags));
		mActor.awareness.CancelCover();
		return true;
	}

	public override void Finish()
	{
		mActor.awareness.CancelCover();
		mActor.Command("CoverInvalid");
	}

	private void Initialise(InheritableMovementParams moveParams)
	{
		mMoveParams = moveParams;
		mFlags = Flags.Default;
		mState = State.Start;
		mTimeInCoverPassive = 0f;
		mPassiveTimeLimit = 10f;
		if (moveParams.coverCluster != null)
		{
			mActor.awareness.coverCluster = mMoveParams.coverCluster;
		}
	}

	private void HitchIntoPosition()
	{
		if (hitchPos > targetHitchPos)
		{
			hitchPos--;
			mState = State.Hitching;
			hitchingUntil = Time.time + 0.6f;
			mActor.Command("HitchLeft");
		}
		else if (hitchPos < targetHitchPos)
		{
			hitchPos++;
			mState = State.Hitching;
			hitchingUntil = Time.time + 0.6f;
			mActor.Command("HitchRight");
		}
	}

	private void ConsiderHitching()
	{
		if (hitchingUntil > Time.time || hitchLeftMax == hitchRightMax || mActor.behaviour.PlayerControlled)
		{
			return;
		}
		int num = UnityEngine.Random.Range(0, 100);
		if (num < hitchChance)
		{
			if (hitchPos != 0 && (mCoverPoint.stepOutLeft || mCoverPoint.stepOutRight) && (num & 3) == 0)
			{
				targetHitchPos = 0;
			}
			else
			{
				targetHitchPos = hitchPos + UnityEngine.Random.Range(1, hitchRightMax - hitchLeftMax);
				if (targetHitchPos > hitchRightMax)
				{
					targetHitchPos -= hitchRightMax - hitchLeftMax;
				}
			}
			HitchIntoPosition();
			hitchChance = Mathf.Max(0, hitchChance - 60);
		}
		else
		{
			hitchingUntil = Time.time + 1000f;
		}
	}

	private void ProcessHitching()
	{
		Vector3 to = hitchDir * hitchPos;
		WorldHelper.ExpBlend(ref hitchOffset, to, 0.1f, 0.02f);
		Vector3 offAxisPos = mCoverPoint.gamePos + hitchOffset;
		if (mActor.navAgent.enabled)
		{
			offAxisPos.y = mActor.GetPosition().y;
		}
		mActor.Pose.offAxisPos = offAxisPos;
	}

	private int BehaviourChance(PopUpBehaviour a, PopUpBehaviour b)
	{
		if (b == PopUpBehaviour.PeekedOver && mCoverPoint.isLowLowCover)
		{
			return 0;
		}
		return behaviourChances[(int)a, (int)b];
	}
}
