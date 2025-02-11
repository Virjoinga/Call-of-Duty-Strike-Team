using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskRoutine : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		NoneCombatAI = 1,
		OneShotRoutineTasks = 2,
		PingPongRoutineOrdering = 4
	}

	private bool oneShotCompleted;

	private TaskDescriptor[] mTasks;

	private int mCurrentTaskIndex;

	private bool mCountingDown;

	public GameObject Magnet;

	public bool dynamicMagnet;

	private Flags mFlags;

	public bool OneShotRoutineTasks
	{
		set
		{
			if (value)
			{
				mFlags |= Flags.OneShotRoutineTasks;
			}
			else
			{
				mFlags &= ~Flags.OneShotRoutineTasks;
			}
		}
	}

	public bool PingPongRoutineOrdering
	{
		set
		{
			if (value)
			{
				mFlags |= Flags.PingPongRoutineOrdering;
			}
			else
			{
				mFlags &= ~Flags.PingPongRoutineOrdering;
			}
		}
	}

	public bool NoneCombatAI
	{
		get
		{
			return (mFlags & Flags.NoneCombatAI) != 0;
		}
	}

	public TaskRoutine(TaskManager owner, TaskManager.Priority priority, Config flags, ICollection<TaskDescriptor> tasks, bool noneCombatAI)
		: base(owner, priority, flags)
	{
		mTasks = new TaskDescriptor[tasks.Count];
		tasks.CopyTo(mTasks, 0);
		mCurrentTaskIndex = -1;
		mFlags = (noneCombatAI ? Flags.NoneCombatAI : Flags.Default);
		if (!(mActor.fireAtWill == null))
		{
			if (noneCombatAI)
			{
				mActor.fireAtWill.Enabled = false;
			}
			else
			{
				mActor.fireAtWill.Enabled = true;
			}
		}
	}

	private void LogRoutineDebug(Task t)
	{
	}

	public override void Update()
	{
		if ((mFlags & Flags.NoneCombatAI) == 0 && (mFlags & Flags.OneShotRoutineTasks) == 0 && UpdateCombatAI())
		{
			return;
		}
		if (mTasks.Length == 0)
		{
			mFlags &= ~Flags.OneShotRoutineTasks;
			if ((mConfigFlags & Config.DenyPlayerInput) != 0)
			{
				mConfigFlags &= ~Config.DenyPlayerInput;
			}
			return;
		}
		if (mCountingDown)
		{
			mCurrentTaskIndex--;
			if (mCurrentTaskIndex < 0)
			{
				mCurrentTaskIndex = 1;
				mCountingDown = false;
			}
		}
		else
		{
			mCurrentTaskIndex++;
			if (mCurrentTaskIndex >= mTasks.Length)
			{
				if ((mFlags & Flags.PingPongRoutineOrdering) != 0)
				{
					mCurrentTaskIndex = Mathf.Max(0, mTasks.Length - 2);
					mCountingDown = true;
				}
				else
				{
					mCurrentTaskIndex = 0;
				}
				if ((mFlags & Flags.OneShotRoutineTasks) != 0)
				{
					mTasks = new TaskDescriptor[0];
					mFlags &= ~Flags.OneShotRoutineTasks;
					oneShotCompleted = true;
					return;
				}
			}
		}
		Config flags = Config.ClearAllCurrentType | Config.AbortOnAlert;
		if ((mFlags & Flags.OneShotRoutineTasks) != 0)
		{
			flags = Config.Default;
		}
		Task task = mTasks[mCurrentTaskIndex].CreateTask(base.Owner, base.Priority, flags);
		LogRoutineDebug(task);
		if (task == null || !(task is TaskPlayAnimation))
		{
			return;
		}
		int num = mCurrentTaskIndex;
		if (mCountingDown)
		{
			num--;
			if (num < 0)
			{
				num = 1;
			}
		}
		else
		{
			num++;
			if (num >= mTasks.Length)
			{
				num = (((mFlags & Flags.OneShotRoutineTasks) != 0) ? (-1) : (((mFlags & Flags.PingPongRoutineOrdering) != 0) ? Mathf.Max(0, mTasks.Length - 2) : 0));
			}
		}
		if (num >= 0 && mTasks[num] is PlayAnimationDescriptor)
		{
			TaskPlayAnimation taskPlayAnimation = task as TaskPlayAnimation;
			taskPlayAnimation.PartOfSequence = true;
		}
	}

	public override void Finish()
	{
		if (Magnet != null && dynamicMagnet)
		{
			UnityEngine.Object.Destroy(Magnet);
			Magnet = null;
			dynamicMagnet = false;
		}
	}

	public override bool HasFinished()
	{
		if ((base.ConfigFlags & Config.AbortOnAlert) != 0 && mActor.behaviour.InActiveAlertState())
		{
			return true;
		}
		if (oneShotCompleted && NoneCombatAI)
		{
			return true;
		}
		return false;
	}

	private bool UpdateCombatAI()
	{
		if (NoneCombatAI)
		{
			return false;
		}
		if (mActor == null || (base.ConfigFlags & Config.AbortOnAlert) != 0)
		{
			return false;
		}
		if (mActor.behaviour.PlayerControlled && !GameController.Instance.FirstPersonFollowMe)
		{
			return false;
		}
		if (mActor.awareness.ChDefCharacterType == CharacterType.AutonomousGroundRobot)
		{
			return false;
		}
		if (mActor.behaviour.InPassiveAlertState())
		{
			return false;
		}
		if (Magnet != null)
		{
			AssaultParams assaultParams = new AssaultParams();
			assaultParams.CopyFrom(mActor.realCharacter.mPersistentAssaultParams);
			assaultParams.Target.theObject = Magnet;
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Walk);
			if (WorldHelper.PercentageChance(30))
			{
				inheritableMovementParams.mMovementStyle = BaseCharacter.MovementStyle.Run;
			}
			new TaskAssault(base.Owner, base.Priority, Config.Default, inheritableMovementParams, assaultParams);
			return true;
		}
		if (!mActor.behaviour.PlayerControlled)
		{
			AssaultParams assaultParams2 = new AssaultParams();
			assaultParams2.CopyFrom(mActor.realCharacter.mPersistentAssaultParams);
			dynamicMagnet = true;
			Magnet = new GameObject("DynamicMagnet");
			assaultParams2.Target.theObject = Magnet;
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(mActor.awareness.EnemiesIKnowAbout);
			float num = float.MaxValue;
			Actor actor = mActor;
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				float sqrMagnitude = (a.GetPosition() - mActor.GetPosition()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					actor = a;
					num = sqrMagnitude;
				}
			}
			Magnet.transform.position = actor.GetPosition();
			InheritableMovementParams inheritableMovementParams2 = new InheritableMovementParams(BaseCharacter.MovementStyle.Walk);
			if (WorldHelper.PercentageChance(30))
			{
				inheritableMovementParams2.mMovementStyle = BaseCharacter.MovementStyle.Run;
			}
			new TaskAssault(base.Owner, base.Priority, Config.ConsultParent, inheritableMovementParams2, assaultParams2);
			return true;
		}
		if (base.Priority != TaskManager.Priority.LONG_TERM)
		{
			return false;
		}
		bool flag = mActor.tether.IsWithinTether();
		float distanceSquared;
		Actor nearestVisibleEnemy = mActor.awareness.GetNearestVisibleEnemy(out distanceSquared);
		if (flag && nearestVisibleEnemy != null)
		{
			FixedGun nearAvailableFixedGun = FixedGunManager.instance.GetNearAvailableFixedGun(mActor.GetPosition());
			if (FactionHelper.AreEnemies(FactionHelper.Category.Player, mActor.awareness.faction) && nearAvailableFixedGun != null && nearAvailableFixedGun.IsPositionTargetable(nearestVisibleEnemy.GetPosition()))
			{
				new TaskUseFixedGun(mOwner, TaskManager.Priority.IMMEDIATE, Config.Default, nearAvailableFixedGun, false, false, false, false);
				return true;
			}
			bool canMoveIntoCover;
			if (ShouldMoveIntoCover(nearestVisibleEnemy, distanceSquared, out canMoveIntoCover))
			{
				OrderMoveToCover(nearestVisibleEnemy);
				return true;
			}
			if (IsAutomatedBehaviourAllowed(nearestVisibleEnemy))
			{
				Actor bestTarget = mActor.behaviour.GetBestTarget(nearestVisibleEnemy);
				if (mActor.behaviour.IsGoodTarget(bestTarget))
				{
					TaskShoot taskShoot = new TaskShoot(mOwner, TaskManager.Priority.IMMEDIATE, bestTarget, Config.Default);
					if (!canMoveIntoCover)
					{
						taskShoot.ContinueIfLosing = true;
					}
					return true;
				}
			}
		}
		else
		{
			if (IsAllowedToFlushOutEnemies() && mActor.behaviour.InActiveAlertState())
			{
				new TaskFlushOutEnemies(mOwner, TaskManager.Priority.LONG_TERM, Config.Default);
				return true;
			}
			if (!flag && (mActor.behaviour.PlayerControlled || mActor.awareness.EnemiesICanSee == 0))
			{
				CoverPointCore validCoverNearestSpecifiedPosition = mActor.awareness.GetValidCoverNearestSpecifiedPosition(mActor.tether.Position, mActor.tether.TetherLimit, 0f, false, 0f);
				InheritableMovementParams inheritableMovementParams3 = new InheritableMovementParams(BaseCharacter.MovementStyle.Run, mActor.tether.Position);
				if (mActor.behaviour.PlayerControlled)
				{
					inheritableMovementParams3.holdCoverWhenBored = true;
					inheritableMovementParams3.mMovementStyle = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
				}
				if (validCoverNearestSpecifiedPosition != null)
				{
					new TaskMoveToCover(mOwner, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType, validCoverNearestSpecifiedPosition, inheritableMovementParams3);
				}
				else
				{
					inheritableMovementParams3.mDestinationThresholdSqr = mActor.tether.TetherLimitSq;
					new TaskRouteTo(mOwner, TaskManager.Priority.LONG_TERM, Config.ClearAllCurrentType, inheritableMovementParams3);
				}
			}
		}
		return false;
	}

	private bool IsAutomatedBehaviourAllowed(Actor target)
	{
		if (mActor.realCharacter == null)
		{
			return false;
		}
		if (!mActor.behaviour.PlayerControlled)
		{
			return true;
		}
		if (target.behaviour.InActiveAlertState() && target.awareness.ChDefCharacterType != CharacterType.SentryGun)
		{
			return true;
		}
		return false;
	}

	private bool IsAllowedToFlushOutEnemies()
	{
		if (mActor.realCharacter == null)
		{
			return false;
		}
		if (mOwner.IsRunningTask(typeof(TaskTrackingRobot)))
		{
			return false;
		}
		Vector3 trackingTarget;
		if (!TaskFlushOutEnemies.GetValidTrackingPosition(mActor, out trackingTarget))
		{
			return false;
		}
		if (!mActor.behaviour.PlayerControlled)
		{
			return true;
		}
		return false;
	}

	private bool ShouldMoveIntoCover(Actor nearestEnemyInView, float nearestEnemyInViewDistSq, out bool canMoveIntoCover)
	{
		if (mActor.awareness.IsInCover())
		{
			canMoveIntoCover = false;
			return false;
		}
		canMoveIntoCover = mActor.awareness.IsCoverAvailable();
		if (canMoveIntoCover)
		{
			if (!mActor.behaviour.IsWinning(nearestEnemyInView) || mActor.aiGunHandler.OnCooldown() || mActor.weapon.IsReloading())
			{
				return true;
			}
			if (mActor.realCharacter.FiringRange * mActor.realCharacter.FiringRange < nearestEnemyInViewDistSq)
			{
				return true;
			}
		}
		return false;
	}

	private void OrderMoveToCover(Actor nearestEnemyInView)
	{
		InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Walk);
		if (WorldHelper.PercentageChance(30))
		{
			inheritableMovementParams.mMovementStyle = BaseCharacter.MovementStyle.Run;
		}
		inheritableMovementParams.mDestination = mActor.GetPosition();
		if (mActor.behaviour.PlayerControlled)
		{
			inheritableMovementParams.holdCoverWhenBored = true;
		}
		new TaskMoveToCover(mOwner, TaskManager.Priority.LONG_TERM, Config.ClearAllCurrentType, inheritableMovementParams);
		mActor.behaviour.ResetEngagementAssessor(nearestEnemyInView);
	}

	public override bool Consult(Task child)
	{
		if (dynamicMagnet)
		{
			Vector3 lastKnownPosition;
			if (mActor.tether.Active)
			{
				Magnet.transform.position = mActor.tether.Position;
			}
			else if (mActor.awareness.GetNearestKnownEnemy(out lastKnownPosition, true) != null)
			{
				Magnet.transform.position = lastKnownPosition;
			}
		}
		return false;
	}
}
