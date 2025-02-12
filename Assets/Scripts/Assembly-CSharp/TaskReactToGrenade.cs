using System;
using UnityEngine;

public class TaskReactToGrenade : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		Invalidated = 1
	}

	private enum State
	{
		Start = 0,
		ReactionDelay = 1,
		RunToSafety = 2,
		CatchingGrenade = 3,
		ThrowGrenadeBack = 4,
		ThrowingGrenadeBack = 5,
		Finished = 6
	}

	private const float kHoldTime = 0.2f;

	private Vector3 mTarget;

	private Vector3 mTargetDir;

	private float mHoldTime;

	private bool mWasInCrouchCover;

	private Flags mFlags;

	private State mState;

	private Grenade mGrenade;

	private float mReactTime;

	public TaskReactToGrenade(TaskManager owner, TaskManager.Priority priority, Config flags, Grenade grenade)
		: base(owner, priority, flags)
	{
		mFlags = Flags.Default;
		mState = State.Start;
		mWasInCrouchCover = false;
		mGrenade = grenade;
	}

	public override void Update()
	{
		if (mGrenade == null || mGrenade.Owner == null)
		{
			mFlags |= Flags.Invalidated;
			return;
		}
		switch (mState)
		{
		case State.Start:
			base.Owner.CancelTasks(typeof(TaskRouteTo));
			base.Owner.CancelTasks(typeof(TaskMoveTo));
			base.Owner.CancelTasks(typeof(TaskMoveToCover));
			if (CanThrowGrenadeBack())
			{
				CatchGrenade();
				mState = State.CatchingGrenade;
			}
			else
			{
				mReactTime = Time.time + 1f;
				mState = State.ReactionDelay;
			}
			break;
		case State.ReactionDelay:
			if (Time.time > mReactTime)
			{
				RunToSafety();
				mState = State.RunToSafety;
			}
			break;
		case State.RunToSafety:
			if (!base.Owner.IsRunningTask(typeof(TaskRouteTo)))
			{
				mState = State.Finished;
			}
			break;
		case State.CatchingGrenade:
			mHoldTime -= Time.deltaTime;
			if (mGrenade.GetComponent<Rigidbody>().IsSleeping() && mHoldTime <= 0f)
			{
				ThrowGrenadeBack();
				mState = State.ThrowGrenadeBack;
			}
			break;
		case State.ThrowingGrenadeBack:
			LetGoNow();
			mState = State.Finished;
			break;
		case State.ThrowGrenadeBack:
			break;
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.Invalidated) != 0)
		{
			return true;
		}
		return mState == State.Finished;
	}

	public override void Command(string com)
	{
		base.Command(com);
		if (com == "ThrowGrenade")
		{
			mState = State.ThrowingGrenadeBack;
		}
	}

	private void RunToSafety()
	{
		CoverPointCore offensiveCoverIgnoringEnemies = mActor.awareness.GetOffensiveCoverIgnoringEnemies(mGrenade.transform.position, mGrenade.DamageRadius * 2f, mGrenade.DamageRadius, false);
		if (offensiveCoverIgnoringEnemies != null)
		{
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.Run);
			new TaskMoveToCover(base.Owner, base.Priority, Config.ClearAllCurrentType, offensiveCoverIgnoringEnemies, moveParams);
			return;
		}
		Vector3 normalized = (mActor.GetPosition() - mGrenade.transform.position).normalized;
		Vector3 position = mGrenade.transform.position + normalized * (mGrenade.DamageRadius * 1.5f);
		int navMeshLayerFromName = UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Default");
		UnityEngine.AI.NavMeshHit navMeshHit = NavMeshUtils.SampleNavMesh(position, 1 << navMeshLayerFromName);
		if (navMeshHit.hit)
		{
			Vector3 position2 = navMeshHit.position;
			InheritableMovementParams moveParams2 = new InheritableMovementParams(BaseCharacter.MovementStyle.Run, position2);
			new TaskRouteTo(base.Owner, base.Priority, Config.ClearAllCurrentType, moveParams2);
		}
		else
		{
			mFlags |= Flags.Invalidated;
		}
	}

	private bool CanThrowGrenadeBack()
	{
		if (!(ActStructure.Instance != null) || ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran)
		{
			return false;
		}
		if (mGrenade.ThrownBack)
		{
			return false;
		}
		if (mGrenade.Owner == null)
		{
			return false;
		}
		if (mActor.baseCharacter.VocalAccent == BaseCharacter.Nationality.Friendly)
		{
			return false;
		}
		float sqrMagnitude = (mGrenade.transform.position - mActor.GetPosition()).sqrMagnitude;
		if (sqrMagnitude > 1f)
		{
			return false;
		}
		Vector3 facing = mGrenade.GetFacing();
		Vector3 lookDirection = mActor.awareness.LookDirection;
		if (Vector3.Dot(facing, lookDirection) > 0f)
		{
			return false;
		}
		return true;
	}

	private void CatchGrenade()
	{
		mGrenade.GetComponent<Rigidbody>().Sleep();
		mGrenade.GetComponent<Renderer>().enabled = false;
		mHoldTime = 0.2f;
	}

	private void ThrowGrenadeBack()
	{
		mTarget = mGrenade.Owner.GetPosition() - mActor.GetPosition();
		mTargetDir = mTarget.normalized;
		if (mActor.Pose.ActiveModule == PoseModuleSharedData.Modules.CrouchCover)
		{
			mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.MoveAim);
			mWasInCrouchCover = true;
		}
		if (mActor.realCharacter != null)
		{
			mActor.realCharacter.ImposeLookDirection(mTargetDir);
			mActor.realCharacter.Command("ThrowGrenade");
		}
	}

	private void LetGoNow()
	{
		if (mActor.GetComponent<Collider>() != null)
		{
			Physics.IgnoreCollision(mGrenade.GetComponent<Collider>(), mActor.GetComponent<Collider>());
		}
		if (mActor.realCharacter.SimpleHitBounds != null)
		{
			Physics.IgnoreCollision(mGrenade.GetComponent<Collider>(), mActor.realCharacter.SimpleHitBounds.GetComponent<Collider>());
		}
		mGrenade.SetThrowingPosition(mActor);
		mGrenade.Throw(mGrenade.Owner.GetPosition());
		mGrenade.ThrownBack = true;
		mActor.realCharacter.CancelThrowGrenade();
		if (mWasInCrouchCover)
		{
			mActor.Pose.SetActiveModule(PoseModuleSharedData.Modules.CrouchCover);
		}
	}
}
