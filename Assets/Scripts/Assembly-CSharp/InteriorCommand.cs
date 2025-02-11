using System.Collections;
using UnityEngine;

public class InteriorCommand : Command
{
	public enum CommandType
	{
		KeepCurrentCeilingState = 0,
		ForceCeilingOn = 1,
		ForceCeilingOff = 2
	}

	public GuidRef Building;

	public CommandType Command;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (Building == null)
		{
			yield break;
		}
		GameObject buildingGO = Building.theObject;
		if (!(buildingGO != null))
		{
			yield break;
		}
		BuildingWithInterior bwi = buildingGO.GetComponentInChildren<BuildingWithInterior>();
		if (bwi != null)
		{
			switch (Command)
			{
			case CommandType.KeepCurrentCeilingState:
				bwi.KeepCurrentCeilingState = true;
				break;
			case CommandType.ForceCeilingOn:
				bwi.Deactivate();
				break;
			case CommandType.ForceCeilingOff:
				bwi.Activate();
				break;
			}
		}
	}
}
