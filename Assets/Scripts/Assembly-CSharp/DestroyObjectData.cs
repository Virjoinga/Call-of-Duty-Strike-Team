using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DestroyObjectData
{
	public List<GameObject> ObjectsToDestroy = new List<GameObject>();

	public bool ArmOnly;

	public bool DestroyOnMessage;

	public void CopyContainerData(DestroyObjectObjective objective)
	{
		foreach (GameObject item2 in ObjectsToDestroy)
		{
			ExplodableObject[] componentsInChildren = item2.GetComponentsInChildren<ExplodableObject>();
			foreach (ExplodableObject item in componentsInChildren)
			{
				objective.ObjectsToDestroy.Add(item);
			}
		}
	}
}
