using System.Collections.Generic;
using UnityEngine;

public class DiscoverableObjectObjective : MissionObjective
{
	public DiscoverableObjectData m_DiscoverableInterface;

	[HideInInspector]
	public List<DiscoverableObject> ObjectsToFind = new List<DiscoverableObject>();

	public override void Start()
	{
		base.Start();
		foreach (DiscoverableObject item in ObjectsToFind)
		{
			if (item != null)
			{
				item.AddEventHandler(OnDiscovered);
			}
			else
			{
				Debug.LogWarning("DiscoverableObject object null ? - " + base.gameObject.name);
			}
			base.gameObject.transform.position = item.gameObject.transform.position;
			MoveToBoundsY(item.gameObject);
			UpdateBlipTarget();
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (DiscoverableObject item in ObjectsToFind)
		{
			item.RemoveEventHandler(OnDiscovered);
		}
	}

	private void OnDiscovered(object sender)
	{
		ObjectsToFind.Remove(sender as DiscoverableObject);
		if (ObjectsToFind.Count == 0)
		{
			Pass();
		}
	}
}
