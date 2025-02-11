using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventsSwitcherData
{
	public bool ClearExistingEventsList = true;

	public List<GameObject> ActorsToSwitch;

	public GameObject EventsToSwitch;

	public void CopyContainerData(EventsSwitcher es)
	{
		es.actorWrappers.Clear();
		if (ActorsToSwitch != null)
		{
			foreach (GameObject item2 in ActorsToSwitch)
			{
				if (item2 != null)
				{
					ActorWrapper[] componentsInChildren = item2.GetComponentsInChildren<ActorWrapper>();
					foreach (ActorWrapper item in componentsInChildren)
					{
						es.actorWrappers.Add(item);
					}
				}
			}
		}
		es.eventsList = EventsToSwitch;
	}
}
