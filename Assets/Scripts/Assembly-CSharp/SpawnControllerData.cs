using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnControllerData
{
	public List<GameObject> SpawnersToControl;

	public bool StopProcessOnActive = true;

	public bool SpawnImmediate = true;

	public List<GameObject> ObjectsToCallOnSpawn;

	public List<string> FunctionsToCallOnSpawn;

	public void CopyContainerData(SpawnController sc)
	{
		if (sc == null || SpawnersToControl == null)
		{
			return;
		}
		sc.Spawners.Clear();
		foreach (GameObject item2 in SpawnersToControl)
		{
			if (item2 != null)
			{
				Spawner[] componentsInChildren = item2.GetComponentsInChildren<Spawner>();
				foreach (Spawner item in componentsInChildren)
				{
					sc.Spawners.Add(item);
				}
			}
		}
	}
}
