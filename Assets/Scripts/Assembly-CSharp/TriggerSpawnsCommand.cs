using System.Collections;
using UnityEngine;

public class TriggerSpawnsCommand : Command
{
	public GameObject sceneReference;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		sceneReference.BroadcastMessage("ProcessSpawn");
		yield break;
	}
}
