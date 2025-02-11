using System.Collections.Generic;
using UnityEngine;

public class DeadBodyWrapper
{
	private Actor mActor;

	private List<Actor> mSeenBy;

	private Vector3 mPositionSpotted;

	public List<Actor> SeenBy
	{
		get
		{
			return mSeenBy;
		}
	}

	public Vector3 SpottedPosition
	{
		get
		{
			return mPositionSpotted;
		}
	}

	public Actor ActorWithin
	{
		get
		{
			return mActor;
		}
	}

	public DeadBodyWrapper(Actor deadActor, Actor spottingActor)
	{
		mActor = deadActor;
		mSeenBy = new List<Actor>();
		AlsoSeenBy(spottingActor);
		mPositionSpotted = deadActor.GetPosition();
	}

	public void AlsoSeenBy(Actor spottingCharacter)
	{
		if (!IsAlreadySeenBy(spottingCharacter))
		{
			mSeenBy.Add(spottingCharacter);
		}
	}

	public bool IsAlreadySeenBy(Actor potentialSpottingCharacter)
	{
		foreach (Actor item in mSeenBy)
		{
			if (item == potentialSpottingCharacter)
			{
				return true;
			}
		}
		return false;
	}
}
