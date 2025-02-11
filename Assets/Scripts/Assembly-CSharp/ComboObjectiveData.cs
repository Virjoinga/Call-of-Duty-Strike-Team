using System;
using System.Collections.Generic;

[Serializable]
public class ComboObjectiveData
{
	public List<Container> SubObjectives = new List<Container>();

	public void CopyContainerData(ComboObjective r)
	{
		foreach (Container subObjective in SubObjectives)
		{
			MissionObjective[] componentsInChildren = subObjective.GetComponentsInChildren<MissionObjective>();
			foreach (MissionObjective item in componentsInChildren)
			{
				r.SubObjectives.Add(item);
			}
		}
	}
}
