using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DiscoverableObjectData
{
	public List<GameObject> ObjectsToFind = new List<GameObject>();

	public void CopyContainerData(DiscoverableObjectObjective objective)
	{
		foreach (GameObject item2 in ObjectsToFind)
		{
			DiscoverableObject[] componentsInChildren = item2.GetComponentsInChildren<DiscoverableObject>();
			foreach (DiscoverableObject item in componentsInChildren)
			{
				objective.ObjectsToFind.Add(item);
			}
		}
	}
}
