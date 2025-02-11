using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InflictDamageData
{
	public List<GameObject> ActorsToDamage;

	public List<float> DamageToApply;

	public float RandomDamageMin;

	public float RandomDamageMax;

	public bool ForceCriticalInjured;

	public bool OneShot = true;

	public ReactionModifier ReactionVars;

	public void CopyContainerData(InflictDamage es)
	{
		es.actorWrappers.Clear();
		if (ActorsToDamage == null)
		{
			return;
		}
		foreach (GameObject item2 in ActorsToDamage)
		{
			if (item2 != null)
			{
				ActorWrapper[] componentsInChildren = item2.GetComponentsInChildren<ActorWrapper>();
				foreach (ActorWrapper item in componentsInChildren)
				{
					es.actorWrappers.Add(item);
				}
			}
		}
	}
}
