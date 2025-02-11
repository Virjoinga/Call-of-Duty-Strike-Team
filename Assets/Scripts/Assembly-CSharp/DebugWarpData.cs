using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DebugWarpData
{
	public List<GameObject> ActorsToWarp = new List<GameObject>();

	public GameObject WarpPosition;

	public List<GameObject> WarpPositions = new List<GameObject>();

	public DebugWarp.WarpRequirements WarpRule = DebugWarp.WarpRequirements.OnlyWarpFirstToTrigger;

	public int RequiredWarpNumber;

	public void CopyContainerData(DebugWarp dw)
	{
		dw.Actors.Clear();
		foreach (GameObject item2 in ActorsToWarp)
		{
			ActorWrapper[] componentsInChildren = item2.GetComponentsInChildren<ActorWrapper>();
			foreach (ActorWrapper item in componentsInChildren)
			{
				dw.Actors.Add(item);
			}
		}
	}
}
