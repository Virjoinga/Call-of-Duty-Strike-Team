using UnityEngine;

public class EventOnHidden : EventDescriptor
{
	public void OnHidden()
	{
		FireEvent();
	}

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
	}
}
