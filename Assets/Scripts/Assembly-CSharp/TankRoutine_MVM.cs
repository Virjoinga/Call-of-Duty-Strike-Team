using UnityEngine;

public class TankRoutine_MVM : MonoBehaviour
{
	public bool MovingToTargetOnStartup;

	public GameObject MagnetToMessage;

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
			if (ForcedStop)
			{
				return false;
			}
			if (!HasValidWaypoints)
			{
				return false;
			}
			return !HasReachedWaypointTarget;
		}
	}

	private bool ForcedStop { get; set; }

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

	private void Awake()
	{
	}

	private void Start()
	{
		if (MovingToTargetOnStartup)
		{
			ForcedStop = false;
		}
		else
		{
			ForcedStop = true;
		}
	}

	private void Update()
	{
		if (HasValidWaypoints && HasReachedWaypointTarget)
		{
			if (MagnetToMessage != null)
			{
				Container.SendMessage(MagnetToMessage, "Activate");
			}
			mWaypointTargetIndex++;
			if (mWaypointTargetIndex >= Waypoints.Length)
			{
				ForceStopMoving();
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (HasValidWaypoints && MovingToTarget)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, WaypointTargetPosition);
		}
	}

	public void ForceStopMoving()
	{
		ForcedStop = true;
	}

	public void ForceStartMoving()
	{
		ForcedStop = false;
	}
}
