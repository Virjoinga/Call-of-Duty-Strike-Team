using UnityEngine;

public class EventOnHacked : EventDescriptor
{
	private HackableObject mHackComponent;

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null)
		{
			TrackingRobotRealCharacter trackingRobotRealCharacter = component.realCharacter as TrackingRobotRealCharacter;
			if (trackingRobotRealCharacter != null)
			{
				mHackComponent = trackingRobotRealCharacter.GetComponent<HackableObject>();
				if (mHackComponent == null)
				{
					Debug.LogWarning("Can't find the HackableObject component on the TrackingRobotReal for the hack event");
				}
			}
			else
			{
				Debug.LogWarning("cant find Tracking robot real char for hack event");
			}
		}
		else
		{
			Debug.LogWarning("cant find actor");
		}
	}

	public void Update()
	{
		if (mHackComponent != null && mHackComponent.FullyHacked)
		{
			FireEvent();
		}
	}
}
