using System.Collections;
using UnityEngine;

public class CloseDoorCommand : Command
{
	public BuildingDoor Door;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		Debug.LogWarning("AC: Scripted closing of doors not yet supported!");
		yield break;
	}
}
