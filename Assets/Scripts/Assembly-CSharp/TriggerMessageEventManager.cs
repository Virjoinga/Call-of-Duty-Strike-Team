using System.Collections.Generic;
using UnityEngine;

public class TriggerMessageEventManager : MonoBehaviour
{
	public List<TriggerMessageEvent> Events;

	public static TriggerMessageEventManager instance;

	private List<TriggerMessageEvent.Type> mEventBuffer;

	public static TriggerMessageEventManager Instance()
	{
		return instance;
	}

	private void Awake()
	{
		TBFAssert.DoAssert(instance == null, "Should only be one instance of TriggerMessageEventManager running");
		instance = this;
		mEventBuffer = new List<TriggerMessageEvent.Type>();
	}

	private void Start()
	{
		if (Events == null)
		{
			return;
		}
		foreach (TriggerMessageEvent @event in Events)
		{
			@event.HasTriggered = false;
		}
	}

	private void Update()
	{
		if (Events == null)
		{
			return;
		}
		foreach (TriggerMessageEvent @event in Events)
		{
			@event.Update(this);
		}
		foreach (TriggerMessageEvent.Type item in mEventBuffer)
		{
			foreach (TriggerMessageEvent event2 in Events)
			{
				if (!event2.HasTriggered && !event2.Execute && event2.Id == item)
				{
					event2.Execute = true;
				}
			}
		}
		mEventBuffer.Clear();
	}

	public void NotifyEvent(TriggerMessageEvent.Type id)
	{
		mEventBuffer.Add(id);
	}
}
