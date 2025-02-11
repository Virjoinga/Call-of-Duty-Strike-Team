using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GenericTriggerData
{
	public List<GameObject> Actors;

	public string OptionalEntityTag = string.Empty;

	public GameObject ObjectToCall;

	public string FunctionToCall;

	public bool RandomMessage;

	public List<GameObject> GroupObjectToCall;

	public List<string> GroupFunctionToCall;

	public string OptionalParm;

	public GameObject OptionalObjectParam;

	public bool OneShot = true;

	public bool AllActorsRequired;

	public bool DeathCountsAsExit;

	public float RepeatTriggerDelay;

	public void CopyContainerData(GenericTrigger gt)
	{
		gt.Actors.Clear();
		if (Actors == null)
		{
			return;
		}
		gt.Actors.Clear();
		foreach (GameObject actor in Actors)
		{
			if (actor != null)
			{
				ActorWrapper[] componentsInChildren = actor.GetComponentsInChildren<ActorWrapper>();
				foreach (ActorWrapper item in componentsInChildren)
				{
					gt.Actors.Add(item);
				}
			}
		}
	}
}
