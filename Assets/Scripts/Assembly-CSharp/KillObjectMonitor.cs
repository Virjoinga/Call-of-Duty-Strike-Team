using System;
using UnityEngine;

[Serializable]
public class KillObjectMonitor
{
	public GameObject ActorToKill;

	public HealthComponent Target;

	[HideInInspector]
	public ActorWrapper Actor;

	public void CopyContainerData(KillObjectObjective objective)
	{
		ActorWrapper[] componentsInChildren = ActorToKill.GetComponentsInChildren<ActorWrapper>();
		foreach (ActorWrapper actor in componentsInChildren)
		{
			Actor = actor;
		}
	}
}
