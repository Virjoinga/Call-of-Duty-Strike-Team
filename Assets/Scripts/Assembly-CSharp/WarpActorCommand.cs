using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpActorCommand : Command
{
	public List<GameObject> Actors;

	public List<GameObject> WarpLocations;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		int i = 0;
		foreach (GameObject go in Actors)
		{
			ActorWrapper aw = go.GetComponentInChildren<ActorWrapper>();
			if ((bool)aw)
			{
				Actor a = aw.GetActor();
				if ((bool)a && i < WarpLocations.Count)
				{
					DebugWarp.WarpActor(a, WarpLocations[i].transform.position, WarpLocations[i].transform.eulerAngles);
				}
			}
			i++;
		}
		yield break;
	}
}
