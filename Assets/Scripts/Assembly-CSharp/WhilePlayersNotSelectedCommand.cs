using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhilePlayersNotSelectedCommand : Command
{
	public List<GameObject> Players = new List<GameObject>();

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		List<Actor> actors = new List<Actor>();
		foreach (GameObject go in Players)
		{
			ActorWrapper aw = go.GetComponentInChildren<ActorWrapper>();
			if (aw != null)
			{
				Actor a2 = aw.GetActor();
				if (a2 != null)
				{
					actors.Add(a2);
				}
			}
		}
		bool quitLoop = false;
		while (!quitLoop)
		{
			bool allActorsSelected = true;
			foreach (Actor a in actors)
			{
				if (!GameplayController.instance.IsSelected(a))
				{
					allActorsSelected = false;
				}
			}
			if (allActorsSelected)
			{
				quitLoop = true;
			}
			yield return null;
		}
	}
}
