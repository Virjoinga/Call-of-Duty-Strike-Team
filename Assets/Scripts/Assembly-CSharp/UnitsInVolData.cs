using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitsInVolData
{
	public Collider TriggerVolume;

	public UnitsInVolumeObjective.TriggerCheck Check;

	public int SpecificNumber = 1;

	public List<GameObject> Actors = new List<GameObject>();

	public string PromptIfNotRequiredNumber = string.Empty;

	public bool ClearDialogueQueueOnSuccess;

	public void CopyContainerData(UnitsInVolumeObjective objective)
	{
		objective.Actors.Clear();
		foreach (GameObject actor in Actors)
		{
			if (actor != null)
			{
				ActorWrapper[] componentsInChildren = actor.GetComponentsInChildren<ActorWrapper>();
				foreach (ActorWrapper item in componentsInChildren)
				{
					objective.Actors.Add(item);
				}
			}
		}
	}
}
