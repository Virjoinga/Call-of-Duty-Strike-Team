using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SquadMortallyWoundedData
{
	public List<GameObject> Actors = new List<GameObject>();

	public void CopyContainerData(SquadMortallyWoundedObjective objective)
	{
		objective.Actors.Clear();
		foreach (GameObject actor in Actors)
		{
			if (actor != null)
			{
				ActorWrapper[] componentsInChildren = actor.GetComponentsInChildren<ActorWrapper>();
				foreach (ActorWrapper item in componentsInChildren)
				{
					objective.Actors.Add(item);
				}
			}
		}
	}
}
