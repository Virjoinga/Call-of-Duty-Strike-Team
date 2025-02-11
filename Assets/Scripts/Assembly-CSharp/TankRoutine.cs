using UnityEngine;

public class TankRoutine : MonoBehaviour
{
	public Transform[] Waypoints;

	private static float REACHED_TARGET_THRESHOLD = 2f;

	private static float REACHED_TARGET_THRESHOLD_SQ = REACHED_TARGET_THRESHOLD * REACHED_TARGET_THRESHOLD;

	private int mWaypointTargetIndex;

	public Vector3 WaypointTargetPosition
	{
		get
		{
			TBFAssert.DoAssert(HasValidWaypoints, "Bad data in TankRoutine");
			return Waypoints[mWaypointTargetIndex].position;
		}
	}

	public bool MovingToTarget
	{
		get
		{
			if (!HasValidWaypoints)
			{
				return false;
			}
			return !HasReachedWaypointTarget;
		}
	}

	private bool HasValidWaypoints
	{
		get
		{
			return Waypoints != null && mWaypointTargetIndex < Waypoints.Length;
		}
	}

	private bool HasReachedWaypointTarget
	{
		get
		{
			return (WaypointTargetPosition - base.transform.position).sqrMagnitude <= REACHED_TARGET_THRESHOLD_SQ;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (HasValidWaypoints && HasReachedWaypointTarget)
		{
			mWaypointTargetIndex++;
			if (mWaypointTargetIndex >= Waypoints.Length)
			{
				mWaypointTargetIndex = 0;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (HasValidWaypoints)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, WaypointTargetPosition);
		}
	}
}
