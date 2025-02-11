using System.Collections;
using UnityEngine;

public class SetLeaderCommand : Command
{
	public enum SelectionMethods
	{
		SpecifiedActor = 0,
		ClosestToObject = 1
	}

	public SelectionMethods SelectionMethod = SelectionMethods.ClosestToObject;

	public GuidRef TheObject;

	public override bool Blocking()
	{
		return false;
	}

	public override void ResolveGuidLinks()
	{
		TheObject.ResolveLink();
	}

	public override IEnumerator Execute()
	{
		if (SelectionMethod == SelectionMethods.ClosestToObject)
		{
			Actor bestActor = null;
			float bestDistance = 0f;
			ActorIdentIterator aii = new ActorIdentIterator(GKM.PlayerControlledMask & GKM.AliveMask);
			Actor a2;
			while (aii.NextActor(out a2))
			{
				if (bestActor == null)
				{
					bestActor = a2;
					bestDistance = Mathf.Abs((TheObject.theObject.transform.position - a2.transform.position).sqrMagnitude);
					continue;
				}
				float distance = Mathf.Abs((TheObject.theObject.transform.position - a2.transform.position).sqrMagnitude);
				if (distance < bestDistance)
				{
					bestDistance = distance;
					bestActor = a2;
				}
			}
			GameplayController.instance.SelectedLeader = bestActor;
			bestActor.behaviour.SelectedMarkerObj.MarkAsLeader(true);
			yield break;
		}
		ActorWrapper aw = TheObject.theObject.GetComponentInChildren<ActorWrapper>();
		if (aw != null)
		{
			Actor a = aw.GetActor();
			if (a != null)
			{
				GameplayController.instance.SelectedLeader = a;
				a.behaviour.SelectedMarkerObj.MarkAsLeader(true);
			}
		}
	}
}
