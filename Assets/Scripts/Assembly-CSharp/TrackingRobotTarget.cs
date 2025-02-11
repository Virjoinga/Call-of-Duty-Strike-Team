using UnityEngine;

public class TrackingRobotTarget
{
	private Vector3 mPosition;

	private Actor mActor;

	public Actor ActorTargeted
	{
		get
		{
			return mActor;
		}
	}

	public Vector3 Position
	{
		get
		{
			if (mPosition != Vector3.zero)
			{
				return mPosition;
			}
			return mActor.GetPosition();
		}
	}

	public TrackingRobotTarget(Actor actor)
	{
		mActor = actor;
		mPosition = Vector3.zero;
	}

	public TrackingRobotTarget(Vector3 position)
	{
		mActor = null;
		mPosition = position;
	}

	public TrackingRobotTarget(Actor actor, Vector3 lastKnownPosition)
	{
		mActor = actor;
		mPosition = lastKnownPosition;
	}

	public bool SameAs(TrackingRobotTarget target)
	{
		if (this == null || target == null)
		{
			return false;
		}
		if (mActor != null && target.mActor != null)
		{
			return mActor == target.mActor;
		}
		return Position == target.Position;
	}
}
