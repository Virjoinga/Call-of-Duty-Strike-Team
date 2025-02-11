using UnityEngine;

public class EventOnCarried : EventDescriptor
{
	[HideInInspector]
	public Actor CarrierRef;

	public void OnCarried(Actor Carrier)
	{
		CarrierRef = Carrier;
		FireEvent();
	}

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
	}
}
