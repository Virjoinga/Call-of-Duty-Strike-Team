using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DominationCapturePointData
{
	public enum CapturePointIdentityEnum
	{
		DominationObjectiveA = 0,
		DominationObjectiveB = 1,
		DominationObjectiveC = 2
	}

	public int MaxNumberHoldingPoint = 1;

	public float TimeForPlayerToCapture = 5f;

	public float TimeForEnemyToCapture = 10f;

	public List<CaptureOverrideWrapper> AlternateDestinations = new List<CaptureOverrideWrapper>();

	public CapturePointIdentityEnum NameOfCapturePoint;

	public GameObject ObjectiveBlipPrefab;

	public void CopyContainerData(DominationCapturePoint dcp)
	{
		dcp.AlternateDestinationRoutines.Clear();
		if (AlternateDestinations == null)
		{
			return;
		}
		dcp.AlternateDestinationRoutines.Clear();
		foreach (CaptureOverrideWrapper alternateDestination in AlternateDestinations)
		{
			if (alternateDestination != null)
			{
				DestinationRoutineWrapper destinationRoutineWrapper = new DestinationRoutineWrapper();
				destinationRoutineWrapper.Point = alternateDestination.CapturePoint.GetComponentInChildren<DominationCapturePoint>();
				destinationRoutineWrapper.DestinationRoutine = alternateDestination.Routine;
				if (destinationRoutineWrapper.DestinationRoutine != null && destinationRoutineWrapper.Point != null)
				{
					dcp.AlternateDestinationRoutines.Add(destinationRoutineWrapper);
				}
			}
		}
	}
}
