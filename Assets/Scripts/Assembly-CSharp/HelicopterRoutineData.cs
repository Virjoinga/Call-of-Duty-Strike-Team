using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HelicopterRoutineData
{
	public float Radius = 10f;

	public float ArrivalSpeed = 1f;

	public float Speed = 1f;

	public float SpeedCrashMultiplier = 3f;

	public float SpeedFleeMultiplier = 3f;

	public float SpeedFleeReturnMultiplier = 3f;

	public HelicopterRoutine.BehaviourType Style;

	[HideInInspector]
	public bool ChasePlayer;

	public bool DisableTargeting;

	public bool FinishOnFlee;

	public bool UseTimedFlee;

	public GameObject PriorityTarget;

	public float ArrivalOffset = 50f;

	public float PauseTime = 3f;

	public float DirectionChangePause = 6f;

	public float FleeAfterTime = 90f;

	public float FleeIdleTime = 5f;

	public GameObject ActivateOnDestruction;

	public List<GameObject> ActivateOnFlee = new List<GameObject>();

	public List<GameObject> PathPoints = new List<GameObject>();

	public List<GameObject> FleePoints = new List<GameObject>();

	public Transform CrashPoint;

	public void CopyContainerData(HelicopterRoutine hr)
	{
		hr.Radius = Radius;
		hr.ArrivalSpeed = ArrivalSpeed;
		hr.Speed = Speed;
		hr.SpeedCrashMultiplier = SpeedCrashMultiplier;
		hr.SpeedFleeMultiplier = SpeedFleeMultiplier;
		hr.SpeedFleeReturnMultiplier = SpeedFleeReturnMultiplier;
		hr.Style = Style;
		hr.ChasePlayer = ChasePlayer;
		hr.DisableTargeting = DisableTargeting;
		hr.ArrivalOffset = ArrivalOffset;
		hr.PauseTime = PauseTime;
		hr.DirectionChangePause = DirectionChangePause;
		hr.FleeIdleTime = FleeIdleTime;
		hr.ActivateOnDestruction = ActivateOnDestruction;
		hr.FleeAfterTime = FleeAfterTime;
		hr.PathPoints.Clear();
		foreach (GameObject pathPoint in PathPoints)
		{
			if (pathPoint != null)
			{
				Transform[] componentsInChildren = pathPoint.GetComponentsInChildren<Transform>();
				foreach (Transform item in componentsInChildren)
				{
					hr.PathPoints.Add(item);
				}
			}
		}
		hr.FleePoints.Clear();
		foreach (GameObject fleePoint in FleePoints)
		{
			if (fleePoint != null)
			{
				Transform[] componentsInChildren2 = fleePoint.GetComponentsInChildren<Transform>();
				foreach (Transform item2 in componentsInChildren2)
				{
					hr.FleePoints.Add(item2);
				}
			}
		}
		hr.ActivateOnFlee.Clear();
		foreach (GameObject item3 in ActivateOnFlee)
		{
			if (item3 != null)
			{
				hr.ActivateOnFlee.Add(item3);
			}
		}
		hr.CrashPoint = CrashPoint;
		if (PriorityTarget != null)
		{
			hr.PriorityTarget = PriorityTarget.GetComponentInChildren<ActorWrapper>();
		}
	}
}
