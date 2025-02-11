using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissionObjectiveData
{
	public string ObjectiveLabel = "S_OBJECTIVE_LABEL";

	public bool IsPrimaryObjective = true;

	public bool IsVisible = true;

	public bool StartDormant;

	public Container[] ObjectivesToEnableOnPass;

	public Container[] ObjectivesToDisableOnPass;

	public GameObject[] GameObjectsToActivate;

	public List<GameObject> ObjectToCallOnComplete;

	public List<string> FunctionToCallOnComplete;

	public ObjectiveBlip.BlipType BlipType = ObjectiveBlip.BlipType.None;

	public bool InfiniteBlipFlashing;

	public bool ShowBlipFlashing = true;

	public void CopyContainerObjectives(MissionObjective objective)
	{
		if (ObjectivesToEnableOnPass != null)
		{
			List<MissionObjective> list = new List<MissionObjective>();
			Container[] objectivesToEnableOnPass = ObjectivesToEnableOnPass;
			foreach (Container container in objectivesToEnableOnPass)
			{
				if (container != null)
				{
					MissionObjective componentInChildren = container.GetComponentInChildren<MissionObjective>();
					if (componentInChildren != null)
					{
						list.Add(componentInChildren);
					}
				}
			}
			objective.EnableObjectivesOnPass = list.ToArray();
		}
		if (ObjectivesToDisableOnPass == null)
		{
			return;
		}
		List<MissionObjective> list2 = new List<MissionObjective>();
		Container[] objectivesToDisableOnPass = ObjectivesToDisableOnPass;
		foreach (Container container2 in objectivesToDisableOnPass)
		{
			if (container2 != null)
			{
				MissionObjective componentInChildren2 = container2.GetComponentInChildren<MissionObjective>();
				if (componentInChildren2 != null)
				{
					list2.Add(componentInChildren2);
				}
			}
		}
		objective.DisableObjectivesOnPass = list2.ToArray();
	}
}
