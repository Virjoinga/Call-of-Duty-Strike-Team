using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(Collider))]
public class UnitsNotInVolData
{
	public Collider TriggerVolume;

	public UnitsInVolumeObjective.TriggerCheck Check;

	public int SpecificNumber = 1;

	public List<GameObject> Actors;

	public void CopyContainerData(UnitsNotInVolumeObjective objective)
	{
		foreach (GameObject actor in Actors)
		{
			ActorWrapper[] componentsInChildren = actor.GetComponentsInChildren<ActorWrapper>();
			foreach (ActorWrapper item in componentsInChildren)
			{
				objective.Actors.Add(item);
			}
		}
	}
}
