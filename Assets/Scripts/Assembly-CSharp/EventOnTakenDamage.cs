using System;
using UnityEngine;

public class EventOnTakenDamage : EventDescriptor
{
	private HealthComponent mHealthComponentRef;

	public float MinimumDamageToFire = 1f;

	private void OnHealthChange(object sender, EventArgs args)
	{
		HealthComponent.HeathChangeEventArgs heathChangeEventArgs = args as HealthComponent.HeathChangeEventArgs;
		if (Mathf.Abs(heathChangeEventArgs.Amount) >= MinimumDamageToFire)
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
			Debug.LogWarning("On Death Event cant find a health component");
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
