using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhileNotMovingInTPP : Command
{
	public List<GameObject> Actors;

	public bool TestSelected;

	public bool TestAll;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		bool quitLoop = false;
		List<Actor> actors = new List<Actor>();
		if (!TestSelected && !TestAll)
		{
			foreach (GameObject go in Actors)
			{
				ActorWrapper aw = go.GetComponentInChildren<ActorWrapper>();
				if (aw != null)
				{
					Actor a4 = aw.GetActor();
					if (a4 != null)
					{
						actors.Add(a4);
					}
				}
			}
		}
		while (!quitLoop)
		{
			if (TestSelected)
			{
				foreach (Actor a3 in GameplayController.instance.Selected)
				{
					if (a3.tasks.IsRunningTask(typeof(TaskMoveTo)))
					{
						quitLoop = true;
					}
				}
			}
			else if (TestAll)
			{
				foreach (Actor a2 in GameplayController.instance.GetValidFirstPersonActors())
				{
					if (a2.tasks.IsRunningTask(typeof(TaskMoveTo)))
					{
						quitLoop = true;
					}
				}
			}
			else
			{
				foreach (Actor a in actors)
				{
					if (a.tasks.IsRunningTask(typeof(TaskMoveTo)))
					{
						quitLoop = true;
					}
				}
			}
			yield return null;
		}
	}
}
