using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterPropertyData
{
	public List<GameObject> ActorsToModify;

	public string OptionalEntityTag;

	public ActorAccessor.ActorTypes ActorTypesToModify;

	public Collider ModifyActorsInVolume;

	public GameObject ModifyActorsInVolumeObject;

	public bool OnlyActorsOutsideArea;

	public void CopyContainerData(CharacterPropertyModifier cpm)
	{
		cpm.actors.Clear();
		cpm.actorWrappers.Clear();
		if (ActorsToModify != null)
		{
			foreach (GameObject item2 in ActorsToModify)
			{
				if (item2 != null)
				{
					ActorWrapper[] componentsInChildren = item2.GetComponentsInChildren<ActorWrapper>(true);
					foreach (ActorWrapper item in componentsInChildren)
					{
						cpm.actorWrappers.Add(item);
					}
				}
			}
		}
		if (cpm != null)
		{
			if (ModifyActorsInVolume != null)
			{
				cpm.ModifyActorsInVolume = ModifyActorsInVolume;
			}
			if (ModifyActorsInVolumeObject != null)
			{
				cpm.ModifyActorsInVolume = ModifyActorsInVolumeObject.GetComponentInChildren<Collider>();
			}
		}
	}
}
