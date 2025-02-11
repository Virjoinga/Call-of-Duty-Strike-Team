using UnityEngine;

public class EventOnIveBeenStealthKilled : EventDescriptor
{
	public void OnDeathsColdEmbrace()
	{
		FireEvent();
	}

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
	}
}
