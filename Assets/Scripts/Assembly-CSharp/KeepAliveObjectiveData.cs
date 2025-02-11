using System;
using UnityEngine;

[Serializable]
public class KeepAliveObjectiveData
{
	public GameObject Actor;

	public void CopyContainerData(KeepObjectAliveObjective objective)
	{
		objective.Actor = null;
		if (Actor != null)
		{
			objective.Actor = Actor.GetComponentInChildren<ActorWrapper>();
		}
	}
}
