using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskDropClaymore : Task
{
	private enum State
	{
		Start = 0,
		ExecutingSetPiece = 1,
		Complete = 2
	}

	private SetPieceLogic setPiece;

	private GameObject mClaymoreObj;

	private HackableObjectClaymore mClaymoreHackComponent;

	private Vector3 mTarget;

	private Vector3 mFacing;

	private State mState;

	private bool mCachedInvulnerableFlag;

	private bool mInvalidate;

	public TaskDropClaymore(TaskManager owner, TaskManager.Priority priority, Config flags, Vector3 target, Vector3 facing)
		: base(owner, priority, flags)
	{
		mTarget = target;
		if (GameController.Instance.IsFirstPerson && mActor == GameController.Instance.mFirstPersonActor)
		{
			UnityEngine.AI.NavMeshHit hit;
			if (UnityEngine.AI.NavMesh.Raycast(mActor.transform.position, mTarget, out hit, mActor.navAgent.walkableMask))
			{
				mTarget = hit.position;
			}
			else
			{
				if (!UnityEngine.AI.NavMesh.SamplePosition(mTarget, out hit, 1f, mActor.navAgent.walkableMask))
				{
					Debug.LogWarning(string.Format("TaskDropClaymore - FPP Placement validation has failed to find a suitable Navmesh point"));
					mActor.tasks.CancelTasks<TaskDropClaymore>();
					return;
				}
				mTarget = hit.position;
			}
			Vector3 crosshairCentre = ViewModelRig.Instance().GetCrosshairCentre();
			Vector3 collision;
			if (!WorldHelper.IsClearTrace(crosshairCentre, crosshairCentre + facing, out collision))
			{
				Debug.LogWarning(string.Format("TaskDropClaymore - FPP Standing position may clip through geometry when animating: ({0})", collision));
				mActor.tasks.CancelTasks<TaskDropClaymore>();
				return;
			}
		}
		else
		{
			UnityEngine.AI.NavMeshHit hit2;
			if (!UnityEngine.AI.NavMesh.SamplePosition(mTarget, out hit2, 3f, mActor.navAgent.walkableMask))
			{
				Debug.LogWarning(string.Format("TaskDropClaymore - TPP Target position {0} unable to sample navmesh position", mTarget));
				mActor.tasks.CancelTasks<TaskDropClaymore>();
				return;
			}
			mTarget = hit2.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(mTarget, Vector3.down, out hitInfo, 2f))
			{
				mTarget = hitInfo.point;
			}
			UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
			if (UnityEngine.AI.NavMesh.CalculatePath(mActor.GetPosition(), mTarget, mActor.navAgent.walkableMask, navMeshPath) && navMeshPath.status != 0)
			{
				Debug.LogWarning(string.Format("TaskDropClaymore - TPP Target position cannot be pathed-to by Agent: ({0})", mTarget));
				mActor.tasks.CancelTasks<TaskDropClaymore>();
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				Vector3 vector = mTarget + Vector3.up * (1f - (float)i * 0.25f);
				Vector3 collision2;
				if (!WorldHelper.IsClearTrace(vector, vector - facing, out collision2))
				{
					Debug.LogWarning(string.Format("TaskDropClaymore - TPP Standing position is deemed to be hitting collision: ({0})", collision2));
					mActor.tasks.CancelTasks<TaskDropClaymore>();
					return;
				}
			}
		}
		mFacing = facing;
		mState = State.Start;
		if (mActor != null)
		{
			mCachedInvulnerableFlag = mActor.health.Invulnerable;
			mActor.health.Invulnerable = true;
		}
	}

	public void SwitchWeapon()
	{
		if (mActor != null && mActor.weapon != null)
		{
			mActor.weapon.SwapTo(new Weapon_Claymore(this), float.MaxValue);
		}
	}

	public override void Update()
	{
		switch (mState)
		{
		case State.Start:
			StartSetPiece();
			mState = State.ExecutingSetPiece;
			break;
		case State.ExecutingSetPiece:
			if (!mOwner.IsRunningTask(typeof(TaskSetPiece)))
			{
				if (mActor.behaviour.PlayerControlled)
				{
					List<Actor> list = new List<Actor>();
					list.Add(mActor);
					OrdersHelper.UpdatePlayerSquadTetherPoint(mActor.GetPosition(), list);
				}
				mState = State.Complete;
			}
			break;
		}
	}

	public override bool HasFinished()
	{
		if (mInvalidate)
		{
			return true;
		}
		if (CheckConfigFlagsFinished())
		{
			return true;
		}
		return mState == State.Complete;
	}

	public override void Finish()
	{
		if (mActor.behaviour.PlayerControlled && mState == State.Complete)
		{
			PlayerSquadManager.Instance.ClaymoreConfirmed();
		}
		WaypointMarkerManager.Instance.RemoveMarker(mActor.gameObject);
		if (mActor != null)
		{
			mActor.health.Invulnerable = mCachedInvulnerableFlag;
		}
	}

	public override void Command(string com)
	{
		if (com == "SpawnClaymore")
		{
			PlayerSquadManager instance = PlayerSquadManager.Instance;
			if (instance != null && instance.ClaymoreCount > 0)
			{
				SpawnClaymore();
				instance.ReduceClaymoreCount();
			}
		}
	}

	public void SpawnClaymore()
	{
		if (mClaymoreObj != null)
		{
			mClaymoreObj.transform.position = mTarget;
			mClaymoreObj.transform.forward = mFacing;
			Vector3 eulerAngles = mClaymoreObj.transform.localRotation.eulerAngles;
			mClaymoreObj.transform.localRotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
			mClaymoreHackComponent.SetFacing(mFacing);
			mClaymoreHackComponent.PlaceClaymore(mClaymoreObj, mActor, mTarget, mFacing, null);
			mClaymoreHackComponent.DisableInteraction();
			mClaymoreObj.SetActive(true);
		}
	}

	private void StartSetPiece()
	{
		mClaymoreObj = UnityEngine.Object.Instantiate(ExplosionManager.Instance.Claymore) as GameObject;
		if (mClaymoreObj != null)
		{
			mClaymoreHackComponent = (HackableObjectClaymore)mClaymoreObj.GetComponentInChildren(typeof(HackableObjectClaymore));
			if (!(mClaymoreHackComponent != null))
			{
				return;
			}
			Vector3 forward = mClaymoreHackComponent.transform.forward;
			mClaymoreHackComponent.transform.position = mTarget;
			mClaymoreHackComponent.transform.forward = mFacing;
			mClaymoreHackComponent.SetFacing(mFacing);
			setPiece = mActor.realCharacter.CreateSetPieceLogic();
			setPiece.SetModule((!GameController.Instance.IsFirstPerson) ? mClaymoreHackComponent.SetPiece : mClaymoreHackComponent.SetPiece_FPP);
			setPiece.PlaceSetPiece(mClaymoreHackComponent.transform);
			if (GameController.Instance.IsFirstPerson)
			{
				Transform characterStartNode = setPiece.GetCharacterStartNode(0);
				UnityEngine.AI.NavMeshHit hit;
				if (characterStartNode != null && !UnityEngine.AI.NavMesh.SamplePosition(characterStartNode.position, out hit, 0.5f, mActor.navAgent.walkableMask))
				{
					Debug.LogWarning(string.Format("TaskDropClaymore - FPP Set-piece failed because it wants to place us off the Navmesh: {0}", characterStartNode.position));
					mInvalidate = true;
				}
			}
			else
			{
				Transform characterStartNode2 = setPiece.GetCharacterStartNode(0);
				if (characterStartNode2 != null)
				{
					UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
					if (UnityEngine.AI.NavMesh.CalculatePath(mActor.GetPosition(), characterStartNode2.position, mActor.navAgent.walkableMask, navMeshPath) && navMeshPath.status != 0)
					{
						Debug.LogWarning(string.Format("TaskDropClaymore - TPP Target position cannot be pathed-to by Agent: ({0})", mTarget));
						mInvalidate = true;
					}
				}
			}
			if (!mInvalidate)
			{
				mClaymoreHackComponent.transform.forward = forward;
				mClaymoreHackComponent.DisableInteraction();
				mClaymoreObj.SetActive(false);
			}
			bool flag = true;
			if (GameController.Instance.IsFirstPerson)
			{
				flag = false;
			}
			InheritableMovementParams moveParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible);
			TaskSetPiece taskSetPiece = new TaskSetPiece(mOwner, base.Priority, base.ConfigFlags, setPiece, false, flag, flag, moveParams, 0f);
			taskSetPiece.AboutToStartSetPieceCallback = (TaskSetPiece.AboutToStartSetPiece)Delegate.Combine(taskSetPiece.AboutToStartSetPieceCallback, new TaskSetPiece.AboutToStartSetPiece(SwitchWeapon));
			if (mInvalidate)
			{
				taskSetPiece.CleanUpSetPiece();
				UnityEngine.Object.Destroy(mClaymoreObj);
			}
		}
		else
		{
			mState = State.Complete;
		}
	}
}
