using UnityEngine;

public class Tether : BaseActorComponent
{
	public Vector3 Position;

	public AITetherPoint AIMarker;

	public bool Active;

	private float mTetherLimitSq;

	private float mTetherLimit;

	public float TetherLimitSq
	{
		get
		{
			return mTetherLimitSq;
		}
	}

	public float TetherLimit
	{
		get
		{
			return mTetherLimit;
		}
	}

	public Vector2 PositionGproj
	{
		get
		{
			return new Vector2(Position.x, Position.z);
		}
	}

	public Tether()
	{
		Active = false;
	}

	public bool IsWithinTether()
	{
		return IsDestinationWithinTether(myActor.GetPosition());
	}

	public bool IsDestinationWithinTether(Vector3 pos)
	{
		if (!Active)
		{
			return true;
		}
		if (!myActor.behaviour.PlayerControlled && myActor.behaviour.alertState < BehaviourController.AlertState.Reacting)
		{
			return true;
		}
		float sqrMagnitude = (Position - myActor.GetPosition()).sqrMagnitude;
		return sqrMagnitude <= mTetherLimitSq;
	}

	public void TetherToSelf()
	{
		Active = true;
		Position = myActor.GetPosition();
		mTetherLimit = TetheringManager.TETHER_LIMIT;
		mTetherLimitSq = TetheringManager.TETHER_LIMIT_SQ;
	}

	public void TetherToAITetherPoint(AITetherPoint point)
	{
		Active = true;
		AIMarker = point;
		Position = point.transform.position;
		mTetherLimit = point.m_Interface.TetherDistance;
		mTetherLimitSq = mTetherLimit * mTetherLimit;
	}

	public void SetPositionAndActivate(Vector3 pos)
	{
		Position = pos;
		Active = true;
		mTetherLimit = TetheringManager.TETHER_LIMIT;
		mTetherLimitSq = TetheringManager.TETHER_LIMIT_SQ;
	}

	public void SetPositionAndRangeAndActivate(Vector3 pos, float range)
	{
		Position = pos;
		Active = true;
		mTetherLimit = range;
		mTetherLimitSq = range * range;
	}
}
