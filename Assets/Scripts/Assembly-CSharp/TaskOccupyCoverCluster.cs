using UnityEngine;

public class TaskOccupyCoverCluster : Task
{
	private enum State
	{
		Start = 0,
		MoveToCover = 1,
		MoveToOpenSpace = 2,
		Illegal = 3
	}

	public float holdCoverTimeMin = 14f;

	public float holdCoverTimeMax = 21f;

	private float holdCoverUntil;

	private InheritableMovementParams mMoveParams;

	private int[] coverPoints;

	private CoverPointCore chosenCover;

	private State state;

	public TaskOccupyCoverCluster(TaskManager owner, TaskManager.Priority priority, Config flags, InheritableMovementParams moveParams)
		: base(owner, priority, flags)
	{
		mMoveParams = moveParams;
		if (mMoveParams.coverCluster == null)
		{
			state = State.Illegal;
			Debug.LogError("OccupyCoverCluster task spawned with no cover cluster!");
		}
		else
		{
			coverPoints = mMoveParams.coverCluster.AsArray();
		}
	}

	private void ChooseRandomCover()
	{
		int num = Random.Range(0, coverPoints.Length - 1);
		CoverPointCore coverPointCore = chosenCover;
		for (int i = 0; i < coverPoints.Length; i++)
		{
			chosenCover = NewCoverPointManager.Instance().coverPoints[coverPoints[num]];
			num = (num + 1) % coverPoints.Length;
			if (chosenCover.type != 0 && (!(chosenCover.Occupant != null) || !(chosenCover.Occupant != mActor)) && !(chosenCover == coverPointCore))
			{
				return;
			}
		}
		chosenCover = null;
	}

	public override void Update()
	{
		switch (state)
		{
		case State.Start:
			if (coverPoints.Length == 0)
			{
				InheritableMovementParams inheritableMovementParams = mMoveParams.Clone();
				mMoveParams.mDestination = mMoveParams.coverCluster.transform.position;
				mMoveParams.FinalLookDirection = mMoveParams.coverCluster.transform.forward;
				new TaskRouteTo(base.Owner, TaskManager.Priority.IMMEDIATE, Config.ConsultParent, inheritableMovementParams);
				state = State.MoveToOpenSpace;
				break;
			}
			if (chosenCover == null || !chosenCover.Available(mActor))
			{
				ChooseRandomCover();
			}
			if (chosenCover != null)
			{
				state = State.MoveToCover;
				InheritableMovementParams inheritableMovementParams = mMoveParams.Clone();
				mMoveParams.mDestination = chosenCover.gamePos;
				inheritableMovementParams.holdCoverWhenBored = true;
				new TaskMoveToCover(base.Owner, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType | Config.ConsultParent, chosenCover, inheritableMovementParams);
			}
			break;
		case State.MoveToCover:
			if (chosenCover == null || !chosenCover.Available(mActor) || mActor.awareness.closestCoverPoint != chosenCover)
			{
				ChooseRandomCover();
				state = State.Start;
			}
			else if ((chosenCover.noCoverAgainst & mActor.awareness.EnemiesIKnowAboutRecent) == 0)
			{
				InheritableMovementParams inheritableMovementParams = mMoveParams.Clone();
				mMoveParams.mDestination = chosenCover.gamePos;
				inheritableMovementParams.holdCoverWhenBored = true;
				new TaskMoveToCover(base.Owner, TaskManager.Priority.IMMEDIATE, Config.ClearAllCurrentType | Config.ConsultParent, chosenCover, inheritableMovementParams);
			}
			break;
		case State.MoveToOpenSpace:
			break;
		}
	}

	public override bool Consult(Task child)
	{
		if (state == State.Start)
		{
			return true;
		}
		if (state == State.MoveToCover)
		{
			if (mActor.awareness.isInCover && (chosenCover == null || mActor.awareness.closestCoverPoint == null || mActor.awareness.closestCoverPoint.index == chosenCover.index))
			{
				if (Time.time > holdCoverUntil && coverPoints.Length > 1)
				{
					ChooseRandomCover();
					holdCoverUntil = Time.time + Random.Range(holdCoverTimeMin, holdCoverTimeMax);
					if (chosenCover != null && chosenCover != mActor.awareness.coverBooked)
					{
						mMoveParams.mMovementStyle = BaseCharacter.MovementStyle.AsFastAsSafelyPossible;
						state = State.Start;
						return true;
					}
				}
			}
			else
			{
				holdCoverUntil = Time.time + Random.Range(holdCoverTimeMin, holdCoverTimeMax);
			}
		}
		return false;
	}

	public override bool HasFinished()
	{
		return state == State.Illegal;
	}

	public override void OnSleep()
	{
		if (mActor.behaviour.PlayerControlled)
		{
			state = State.Illegal;
		}
	}
}
