using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AlarmPanelData
{
	public List<GameObject> InterestZones = new List<GameObject>();

	public List<GameObject> spawnerObjects;

	public float range = 50f;

	public void CopyContainerData(AlarmPanel ap)
	{
		if (InterestZones != null)
		{
			foreach (GameObject interestZone in InterestZones)
			{
				if (interestZone != null)
				{
					InterestZone componentInChildren = interestZone.GetComponentInChildren<InterestZone>();
					if (componentInChildren != null)
					{
						ap.InterestZones.Add(componentInChildren);
					}
				}
			}
		}
		if (spawnerObjects == null)
		{
			return;
		}
		foreach (GameObject spawnerObject in spawnerObjects)
		{
			if (spawnerObject != null)
			{
				SpawnerBase componentInChildren2 = spawnerObject.GetComponentInChildren<SpawnerBase>();
				if (componentInChildren2 != null)
				{
					ap.spawners.Add(componentInChildren2);
				}
				SpawnerCoordinator componentInChildren3 = spawnerObject.GetComponentInChildren<SpawnerCoordinator>();
				if (componentInChildren3 != null)
				{
					ap.spawnCoordinators.Add(componentInChildren3);
				}
			}
		}
	}
}
