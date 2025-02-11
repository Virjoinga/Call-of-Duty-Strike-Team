using System;
using UnityEngine;

public class EventOnMortallyWounded : EventDescriptor
{
	private HealthComponent mHealthComponentRef;

	private Actor mActorRef;

	private void OnHealthChange(object sender, EventArgs args)
	{
		if (mActorRef.realCharacter.IsMortallyWounded())
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
			mActorRef = component;
		}
		else
		{
			Debug.LogWarning("On Death Event cant find a health component");
		}
	}

	public void OnDestroy()
	{
		if (mHealthComponentRef != null)
		{
			mHealthComponentRef.OnHealthChange -= OnHealthChange;
		}
	}
}
