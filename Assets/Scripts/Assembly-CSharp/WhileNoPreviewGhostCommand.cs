using System.Collections;
using UnityEngine;

public class WhileNoPreviewGhostCommand : Command
{
	public GameObject ActorToTest;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		if (!(ActorToTest != null))
		{
			yield break;
		}
		ActorWrapper aw = ActorToTest.GetComponentInChildren<ActorWrapper>();
		if (!(aw != null))
		{
			yield break;
		}
		Actor a = aw.GetActor();
		if (a != null)
		{
			while (!(WaypointMarkerManager.Instance.GetMarker(a.gameObject) != null))
			{
				yield return null;
			}
		}
	}
}
