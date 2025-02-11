using System;
using UnityEngine;

public class EventOnLowHealth : EventDescriptor
{
	private HealthComponent mHealthComponentRef;

	public float LowHealthPercentage = 0.5f;

	private void OnHealthChange(object sender, EventArgs args)
	{
		if (mHealthComponentRef.Health01 < LowHealthPercentage)
		{
			FireEvent();
		}
	}

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null && component.health != null)
		{
			mHealthComponentRef = component.health;
			mHealthComponentRef.OnHealthChange += OnHealthChange;
		}
		else
		{
			Debug.LogWarning("On Low Health cant find a health component");
		}
	}

	public override void DeInitialise()
	{
		if (mHealthComponentRef != null)
		{
			mHealthComponentRef.OnHealthChange -= OnHealthChange;
			mHealthComponentRef = null;
		}
	}

	public void OnDestroy()
	{
		DeInitialise();
	}
}
