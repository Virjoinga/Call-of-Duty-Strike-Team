using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSelectedPlayersCommand : Command
{
	public List<GameObject> actorObjects;

	public bool moveCameraToActor;

	private List<Actor> actors;

	private bool actorsGenerated;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		while (!actorsGenerated)
		{
			GetActors();
			yield return 0;
		}
		if (actors == null)
		{
			yield break;
		}
		GameplayController gameplayController = GameplayController.instance;
		if (gameplayController == null)
		{
			yield break;
		}
		for (int i = 0; i < actors.Count; i++)
		{
			if (actors[i] != null)
			{
				if (i == 0)
				{
					gameplayController.SelectOnlyThis(actors[i]);
				}
				else
				{
					gameplayController.AddToSelected(actors[i]);
				}
			}
		}
		if (moveCameraToActor)
		{
			CameraController cc = CameraManager.Instance.PlayCameraController;
			PlayCameraInterface cfd = cc.StartCamera as PlayCameraInterface;
			if (cfd != null && actors[0] != null)
			{
				cfd.FocusOnTarget(actors[0].transform, true);
			}
		}
	}

	private void GetActors()
	{
		if (actorObjects == null)
		{
			actorsGenerated = true;
		}
		else
		{
			if (!GameController.Instance.GameplayStarted)
			{
				return;
			}
			bool flag = false;
			actors = new List<Actor>();
			foreach (GameObject actorObject in actorObjects)
			{
				ActorWrapper componentInChildren = actorObject.GetComponentInChildren<ActorWrapper>();
				Actor actor = null;
				if (!(componentInChildren != null))
				{
					continue;
				}
				flag = true;
				actor = componentInChildren.GetActor();
				if (actor != null)
				{
					actorsGenerated = true;
					if (!actor.behaviour.PlayerControlled)
					{
						TBFAssert.DoAssert(false, "Trying to select NPC for control");
					}
					if (!actor.realCharacter.IsSelectable())
					{
						break;
					}
					actors.Add(actor);
				}
			}
			if (!flag)
			{
				actorsGenerated = true;
			}
		}
	}
}
