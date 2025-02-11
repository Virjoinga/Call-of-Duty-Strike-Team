using System;
using UnityEngine;

public class AnimationEventData : ScriptableObject
{
	[Serializable]
	public class Event
	{
		public string Name;

		public float Time;
	}

	public Event[] Events;
}
