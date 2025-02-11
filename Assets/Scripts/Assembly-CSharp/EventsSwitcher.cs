using System.Collections.Generic;
using UnityEngine;

public class EventsSwitcher : MonoBehaviour
{
	public EventsSwitcherData m_Interface;

	public List<ActorWrapper> actorWrappers;

	public GameObject eventsList;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void CopyNewEvents()
	{
		foreach (ActorWrapper actorWrapper in actorWrappers)
		{
			Actor actor = actorWrapper.GetActor();
			EventsCreator componentInChildren = eventsList.GetComponentInChildren<EventsCreator>();
			if (componentInChildren != null)
			{
				foreach (EventDescriptor @event in componentInChildren.GetEvents())
				{
					@event.Initialise(actor.gameObject);
				}
			}
			eventsList.name = actor.name + " Events List";
			eventsList.transform.parent = actor.transform;
		}
	}

	public void ClearExistingEvents()
	{
		foreach (ActorWrapper actorWrapper in actorWrappers)
		{
			Actor actor = actorWrapper.GetActor();
			EventsCreator componentInChildren = actor.GetComponentInChildren<EventsCreator>();
			if (!(componentInChildren != null))
			{
				continue;
			}
			foreach (EventDescriptor @event in componentInChildren.GetEvents())
			{
				Object.Destroy(@event);
			}
		}
	}

	public void Activate()
	{
		if (m_Interface.ClearExistingEventsList)
		{
			ClearExistingEvents();
		}
		if (eventsList != null)
		{
			CopyNewEvents();
		}
	}

	public void Deactivate()
	{
	}
}
