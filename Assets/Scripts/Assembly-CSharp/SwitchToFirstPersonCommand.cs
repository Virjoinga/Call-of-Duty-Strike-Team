using System.Collections;
using UnityEngine;

public class SwitchToFirstPersonCommand : Command
{
	public GameObject Actor;

	public bool LockToFirstPerson;

	public bool UseLeader;

	public bool UseLastFPPActor;

	public bool Transitions = true;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		ActorWrapper aw = null;
		if (!UseLeader && !UseLastFPPActor)
		{
			aw = Actor.GetComponentInChildren<ActorWrapper>();
		}
		Actor actor = null;
		if (aw != null || UseLeader || UseLastFPPActor)
		{
			do
			{
				actor = (UseLeader ? GameplayController.instance.SelectedLeader : ((!UseLastFPPActor) ? aw.GetActor() : GameController.Instance.mPreviousFirstPersonActor));
				if (actor == null)
				{
					yield return null;
				}
			}
			while (actor == null);
		}
		if (actor != null)
		{
			GameController.Instance.SwitchToFirstPerson(actor, Transitions);
			if (LockToFirstPerson)
			{
				GameController.Instance.IsLockedToFirstPerson = true;
				GameController.Instance.IsLockedToCurrentCharacter = true;
			}
		}
	}
}
