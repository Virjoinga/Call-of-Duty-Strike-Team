using System.Collections;
using UnityEngine;

public class SpawnActorCommand : Command
{
	public ActorDescriptor Descriptor;

	public Transform SpawnPoint;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GameObject spawn = SceneNanny.CreateActor(Descriptor, SpawnPoint.position, SpawnPoint.rotation);
		Actor actor = spawn.GetComponent<Actor>();
		if (actor != null)
		{
			actor.realCharacter.EnableNavMesh(true);
		}
		yield break;
	}
}
