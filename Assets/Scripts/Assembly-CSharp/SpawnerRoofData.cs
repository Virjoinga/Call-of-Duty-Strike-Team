using System;
using UnityEngine;

[Serializable]
public class SpawnerRoofData : SpawnerDoorDataBase
{
	public void CopyContainerData(SpawnerRoof sr)
	{
		if ((bool)StaticTether)
		{
			sr.StaticTether = StaticTether.GetComponentInChildren<AITetherPoint>();
		}
		if (PreferredTarget != null)
		{
			sr.PreferredTarget = PreferredTarget.GetComponentInChildren<ActorWrapper>();
			if (sr.PreferredTarget == null)
			{
				Debug.Log("null");
			}
			else
			{
				Debug.Log("!null");
			}
		}
	}
}
