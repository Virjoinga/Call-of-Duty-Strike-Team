using UnityEngine;

public class EventOnEnterCover : EventDescriptor
{
	private Actor mActorRef;

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null)
		{
			mActorRef = component;
		}
		else
		{
			Debug.LogWarning("Cant Find actor for event");
		}
		if (!FireOnlyOnce)
		{
			Debug.LogWarning("Fire Only Once flag set to false, this will fire every frame while in cover, are you sure?");
		}
	}

	public void Update()
	{
		if (mActorRef != null && mActorRef.awareness.isInCover)
		{
			FireEvent();
		}
	}
}
