using System;

[Serializable]
public class HackObjectiveData
{
	public Container ContainerTerminal;

	public void CopyContainerData(HackObjective objective)
	{
		if (ContainerTerminal != null)
		{
			HackableObject componentInChildren = ContainerTerminal.GetComponentInChildren<HackableObject>();
			if (componentInChildren != null)
			{
				objective.AssociatedHackableTerminal = componentInChildren.gameObject;
			}
		}
	}
}
