using System.Collections;
using UnityEngine;

public class OpenDoorCommand : Command
{
	public BuildingDoor Door;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		Debug.LogWarning("AC: Scripted opening of doors not yet supported!");
		yield break;
	}
}
