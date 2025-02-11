using System.Collections;
using UnityEngine;

public class WhileNotDoorStateCommand : Command
{
	public GameObject DoorOrInterior;

	public int InteriorDoorIndexToReference;

	public BuildingDoor.DoorSate State;

	private BuildingDoor door;

	private void Start()
	{
		if (DoorOrInterior == null)
		{
			TBFAssert.DoAssert(false, "No Door or Interior Specified");
		}
		InteriorOverride componentInChildren = DoorOrInterior.GetComponentInChildren<InteriorOverride>();
		if ((bool)componentInChildren)
		{
			door = componentInChildren.m_ActiveDoors[InteriorDoorIndexToReference].m_Object.GetComponentInChildren<BuildingDoor>();
		}
		else
		{
			door = DoorOrInterior.gameObject.GetComponentInChildren<BuildingDoor>();
		}
		if (door == null)
		{
			TBFAssert.DoAssert(false, "No Door found with specified parameters");
		}
	}

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		while (door.m_Interface.State != State)
		{
			yield return null;
		}
	}
}
