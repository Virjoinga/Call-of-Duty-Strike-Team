using System;
using UnityEngine;

[Serializable]
public class SpawnerDoorData : SpawnerDoorDataBase
{
	public BuildingDoor.eDoorType DoorType = BuildingDoor.eDoorType.Arcti_1;

	public void CopyContainerData(SpawnerDoor sd)
	{
		if ((bool)StaticTether)
		{
			sd.StaticTether = StaticTether.GetComponentInChildren<AITetherPoint>();
		}
		if (PreferredTarget != null)
		{
			sd.PreferredTarget = PreferredTarget.GetComponentInChildren<ActorWrapper>();
			if (sd.PreferredTarget == null)
			{
				Debug.Log("null");
			}
			else
			{
				Debug.Log("!null");
			}
		}
		sd.DoorType = DoorType;
	}
}
