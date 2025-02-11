using System;
using UnityEngine;

[Serializable]
public class RespotActorDescriptor
{
	public GameObject ActorToRespot;

	public GameObject OptionalRespawnRoutine;

	public GameObject OptionalRespotLocation;

	[HideInInspector]
	public ActorWrapper actorWrapper;

	[HideInInspector]
	public Spawner spawner;
}
