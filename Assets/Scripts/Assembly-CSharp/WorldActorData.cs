using System;
using UnityEngine;

[Serializable]
public class WorldActorData
{
	public enum ActionTypes
	{
		AI = 0,
		Kill = 1,
		Enabled = 2,
		KillAndHide = 3
	}

	public ActorAccessor.ActorTypes TypesOfActor;

	public string OptionalEntityTag = string.Empty;

	public ActionTypes TypeOfAction;

	public GameObject OptionalAreaToUse;

	public bool OnlyActorsOutsideArea;

	public bool IncludeFakeActors = true;

	public void CopyContainerData(WorldActorsCommand wa)
	{
		if (OptionalAreaToUse != null && wa != null)
		{
			wa.OptionalAreaToUse = OptionalAreaToUse.GetComponentInChildren<Collider>();
		}
	}
}
