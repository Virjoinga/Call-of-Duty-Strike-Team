using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskMultiCharacterSetPiece : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		SetPieceInvalidated = 1
	}

	private enum State
	{
		Start = 0,
		MovingToTarget = 1,
		Animating = 2,
		Complete = 3
	}

	public delegate void AboutToStartSetPiece();

	private const int kMaxActors = 4;

	private const float kSetPieceTouchDist = 1.5f;

	private const float kSetPieceFaceDist = 2f;

	private const float kOnNavMeshDistance = 0.2f;

	private const float kSetPieceTouchDist_Sq = 2.25f;

	private const float kSetPieceFaceDist_Sq = 4f;

	private CMSetPiece mContextMenu;

	public AboutToStartSetPiece AboutToStartSetPieceCallback;

	private int fixedActorOrderFrom;

	private Flags mFlags;

	private SetPieceLogic mSetPiece;

	private State mState;

	private List<SquadFormation.FormationSlot> mEntryPoints;

	private float[] mLookTowardsDist;

	private Vector3[] mFinalLookAt;

	private bool[] mLookAtOverride;

	private TaskCarry mCarryTask;

	public bool ForceIntoPlace;

	private List<Actor> mActors;

	public TaskMultiCharacterSetPiece(TaskManager owner, TaskManager.Priority priority, Config flags, SetPieceLogic setPiece, List<Actor> actors)
		: this(owner, priority, flags, setPiece, actors, null)
	{
	}

	public TaskMultiCharacterSetPiece(TaskManager owner, TaskManager.Priority priority, Config flags, SetPieceLogic setPiece, List<Actor> actors, CMSetPiece contextMenu)
		: base(owner, priority, flags)
	{
		mSetPiece = setPiece;
		mState = State.Start;
		mFlags = Flags.Default;
		mActors = actors;
		fixedActorOrderFrom = mActors.Count;
		mLookTowardsDist = new float[4];
		mFinalLookAt = new Vector3[4];
		mLookAtOverride = new bool[4];
		mContextMenu = contextMenu;
		for (int i = 0; i < 4; i++)
		{
			mLookTowardsDist[i] = 0f;
			mLookAtOverride[i] = false;
		}
		if (mSetPiece.ObjectActors != null)
		{
			for (int j = 0; j < mSetPiece.ObjectActors.GetLength(0); j++)
			{
				Actor actor = null;
				if (mSetPiece.ObjectActors[j] != null)
				{
					actor = mSetPiece.ObjectActors[j].GetComponent<Actor>();
					if (actor == null)
					{
						Spawner component = mSetPiece.ObjectActors[j].GetComponent<Spawner>();
						if (component != null && component.spawned != null)
						{
							actor = component.spawned.GetComponent<Actor>();
						}
					}
				}
				if ((bool)actor)
				{
					if (j < mActors.Count)
					{
						mActors[j] = actor;
					}
					else
					{
						mActors.Add(actor);
					}
				}
			}
		}
		mEntryPoints = new List<SquadFormation.FormationSlot>();
	}

	public void SetDestinationLookTowardsThreshold(int actor, float range)
	{
		mLookTowardsDist[actor] = range;
	}

	public void SetFinalLookAt(int actor, Vector3 lookAt)
	{
		mFinalLookAt[actor] = lookAt;
		mLookAtOverride[actor] = true;
	}

	public override void Update()
	{
		if (mSetPiece == null)
		{
			mState = State.Complete;
			return;
		}
		switch (mState)
		{
		case State.Start:
		{
			bool flag = true;
			for (int j = 0; j < mActors.Count; j++)
			{
				TaskCarry taskCarry = (TaskCarry)mActors[j].tasks.GetRunningTask(typeof(TaskCarry));
				if (taskCarry != null)
				{
					if (mCarryTask == null)
					{
						mCarryTask = taskCarry;
					}
					if (!taskCarry.ForceDropDone(this))
					{
						mFlags |= Flags.SetPieceInvalidated;
						return;
					}
				}
			}
			if (!flag)
			{
				break;
			}
			mEntryPoints.Clear();
			for (int k = 0; k < mActors.Count; k++)
			{
				Transform characterStartNode = mSetPiece.GetCharacterStartNode(k);
				SquadFormation.FormationSlot formationSlot = null;
				formationSlot = ((!(characterStartNode != null)) ? new SquadFormation.FormationSlot(mActors[k].GetPosition(), mActors[k].realCharacter.GetStanceDirection()) : new SquadFormation.FormationSlot(characterStartNode.position, WorldHelper.UfM_Forward(mSetPiece.GetCharacterStartNode(k))));
				mEntryPoints.Add(formationSlot);
			}
			for (int l = fixedActorOrderFrom; l < mActors.Count; l++)
			{
				mEntryPoints[l].Assigned = mActors[l];
			}
			if (fixedActorOrderFrom > 0)
			{
				SquadFormation.AssignSlotsByPenalisingFurthest(mActors, mEntryPoints, fixedActorOrderFrom, fixedActorOrderFrom);
			}
			mState = State.Animating;
			if (!ForceIntoPlace)
			{
				bool flag2 = true;
				for (int m = 0; m < mActors.Count; m++)
				{
					mActors[m] = mEntryPoints[m].Assigned;
					Actor actor = mActors[m];
					SquadFormation.FormationSlot formationSlot2 = mEntryPoints[m];
					if (!((mEntryPoints[m].Position - actor.GetPosition()).sqrMagnitude > 0.45000002f) || !actor.gameObject.activeSelf || actor.navAgent == null || !actor.navAgent.enabled)
					{
						continue;
					}
					UnityEngine.AI.NavMeshPath navMeshPath = new UnityEngine.AI.NavMeshPath();
					if (actor.navAgent.CalculatePath(formationSlot2.Position, navMeshPath))
					{
						if (navMeshPath.status != 0)
						{
							flag2 = false;
						}
					}
					else
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					mFlags |= Flags.SetPieceInvalidated;
					break;
				}
				for (int n = 0; n < mActors.Count; n++)
				{
					mActors[n] = mEntryPoints[n].Assigned;
					Actor actor2 = mActors[n];
					SquadFormation.FormationSlot formationSlot3 = mEntryPoints[n];
					if ((mEntryPoints[n].Position - actor2.GetPosition()).sqrMagnitude > 0.45000002f)
					{
						InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(BaseCharacter.MovementStyle.AsFastAsSafelyPossible, formationSlot3.Position);
						if (mLookAtOverride[n])
						{
							inheritableMovementParams.FinalLookAt = mFinalLookAt[n];
						}
						else
						{
							inheritableMovementParams.FinalLookDirection = formationSlot3.Facing;
						}
						inheritableMovementParams.DestinationThreshold = 0.3f;
						if (mLookTowardsDist[n] > 0f)
						{
							inheritableMovementParams.LookTowardsDist = mLookTowardsDist[n];
						}
						inheritableMovementParams.forceFaceForwards = true;
						inheritableMovementParams.mGunAwayAtEnd = mSetPiece.GunAway(n);
						new TaskRouteTo(actor2.tasks, base.Priority, Config.ClearAllCurrentType, inheritableMovementParams);
						mState = State.MovingToTarget;
					}
					else if (mActors[n].navAgent.enabled)
					{
						mActors[n].navAgent.Stop();
						mActors[n].navAgent.velocity = Vector3.zero;
					}
				}
			}
			if (mState == State.Animating)
			{
				StartSetPiece();
			}
			break;
		}
		case State.MovingToTarget:
		{
			bool flag3 = true;
			for (int num = 0; num < mEntryPoints.Count; num++)
			{
				Actor assigned = mEntryPoints[num].Assigned;
				if (!assigned.tasks.IsRunningTask(typeof(TaskRouteTo)))
				{
					float sqrMagnitude = (mEntryPoints[num].Position.xz() - assigned.GetPosition().xz()).sqrMagnitude;
					if (sqrMagnitude > 2.25f)
					{
						mFlags |= Flags.SetPieceInvalidated;
					}
				}
				else if (!assigned.realCharacter.IsDead() && !assigned.realCharacter.IsMortallyWounded() && !assigned.realCharacter.IsBeingCarried)
				{
					flag3 = false;
					break;
				}
			}
			if ((mFlags & Flags.SetPieceInvalidated) == 0 && flag3)
			{
				if (mSetPiece.SPModule != null && mSetPiece.SPModule.IsPlaying())
				{
					mFlags |= Flags.SetPieceInvalidated;
				}
				else
				{
					StartSetPiece();
				}
			}
			break;
		}
		case State.Animating:
		{
			for (int i = 0; i < mActors.Count; i++)
			{
				if (!mSetPiece.HasActorFinished(i))
				{
					mActors[i].realCharacter.mIdleTimer = 0f;
				}
			}
			if (mSetPiece.HasFinished())
			{
				EndSetPiece();
			}
			break;
		}
		}
	}

	public override void Finish()
	{
		if (mSetPiece != null && mSetPiece.Mortal)
		{
			UnityEngine.Object.DestroyImmediate(mSetPiece.gameObject);
		}
	}

	public override bool HasFinished()
	{
		if ((mFlags & Flags.SetPieceInvalidated) != 0)
		{
			return true;
		}
		if (mCarryTask != null && mCarryTask.HasTargetBecomeUncarriable())
		{
			return true;
		}
		if (mState == State.Start)
		{
			return false;
		}
		return mState == State.Complete;
	}

	private void StartSetPiece()
	{
		if (AboutToStartSetPieceCallback != null)
		{
			AboutToStartSetPieceCallback();
		}
		for (int i = 0; i < mEntryPoints.Count; i++)
		{
			Actor assigned = mEntryPoints[i].Assigned;
			mSetPiece.SetActor_IndexOnlyCharacters(i, assigned);
		}
		mSetPiece.PlaySetPiece();
		mSetPiece.ShowHUD(false);
		mState = State.Animating;
	}

	private void TurnOffSoliderMarkersIfCarried()
	{
		if (mEntryPoints.Count == 2 && (bool)mEntryPoints[1].Assigned && mEntryPoints[1].Assigned.realCharacter.carryBooked && mEntryPoints[1].Assigned.behaviour.SelectedMarkerObj != null)
		{
			mEntryPoints[1].Assigned.behaviour.SelectedMarkerObj.SwitchOff();
		}
	}

	private void EndSetPiece()
	{
		mState = State.Complete;
		mSetPiece.ShowHUD(true);
		mSetPiece.WasSuccessful = true;
		if (mContextMenu != null)
		{
			mContextMenu.IsComplete = true;
		}
		TurnOffSoliderMarkersIfCarried();
	}

	public void FixedActorOrder()
	{
		fixedActorOrderFrom = 0;
	}
}
