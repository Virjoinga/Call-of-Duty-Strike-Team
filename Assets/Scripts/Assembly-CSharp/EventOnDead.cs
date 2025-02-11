using System;
using UnityEngine;

public class EventOnDead : EventDescriptor
{
	private HealthComponent mHealthComponentRef;

	private void OnHealthEmpty(object sender, EventArgs args)
	{
		FireEvent();
	}

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null && component.health != null)
		{
			if (component.health.HealthEmpty)
			{
				OnHealthEmpty(null, null);
				return;
			}
			mHealthComponentRef = component.health;
			mHealthComponentRef.OnHealthEmpty += OnHealthEmpty;
		}
		else
		{
			Debug.LogWarning("On Death Event cant find a health component");
		}
	}

	public override void InitialiseForActor(Actor actor)
	{
		if (actor != null && actor.health != null)
		{
			if (actor.health.HealthEmpty)
			{
				OnHealthEmpty(null, null);
				return;
			}
			mHealthComponentRef = actor.health;
			mHealthComponentRef.OnHealthEmpty += OnHealthEmpty;
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
			mHealthComponentRef.OnHealthEmpty -= OnHealthEmpty;
			mHealthComponentRef = null;
		}
	}

	public void OnDestroy()
	{
		DeInitialise();
	}
}
