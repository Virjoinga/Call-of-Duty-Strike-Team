using System.Collections;
using UnityEngine;

public class WeaponsHoldCommand : Command
{
	public GameObject[] Actors;

	public float Seconds;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		int numActors = Actors.Length;
		Actor[] actors = new Actor[numActors];
		for (int j = 0; j < numActors; j++)
		{
			GameObject actorWrapperRoot = Actors[j];
			ActorWrapper actorWrapper = ((!(actorWrapperRoot != null)) ? null : actorWrapperRoot.GetComponentInChildren<ActorWrapper>());
			Actor actor = ((!(actorWrapper != null)) ? null : actorWrapper.GetActor());
			if (actor != null && actor.aiGunHandler != null)
			{
				actor.aiGunHandler.WeaponsHold = true;
			}
			actors[j] = actor;
		}
		yield return new WaitForSeconds(Seconds);
		for (int i = 0; i < numActors; i++)
		{
			Actor actor2 = actors[i];
			if (actor2 != null && actor2.aiGunHandler != null)
			{
				actor2.aiGunHandler.WeaponsHold = false;
			}
		}
	}
}
