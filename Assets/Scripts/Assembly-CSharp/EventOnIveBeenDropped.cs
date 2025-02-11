using UnityEngine;

public class EventOnIveBeenDropped : EventDescriptor
{
	public void OnDropped()
	{
		FireEvent();
	}

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
	}
}
