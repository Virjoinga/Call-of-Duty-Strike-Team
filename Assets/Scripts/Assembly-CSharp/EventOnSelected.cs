using UnityEngine;

public class EventOnSelected : EventDescriptor
{
	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
	}

	public void Selected()
	{
		FireEvent();
	}
}
