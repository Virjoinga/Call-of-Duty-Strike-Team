using System.Collections;
using UnityEngine;

public class ClearFPPCacheCommand : Command
{
	public GameObject Actor;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		Actor a = null;
		ActorWrapper aw = Actor.GetComponentInChildren<ActorWrapper>();
		if (aw != null)
		{
			a = aw.GetActor();
		}
		if (a != null)
		{
			TaskCacheStanceState task = (TaskCacheStanceState)a.tasks.GetRunningTask(typeof(TaskCacheStanceState));
			if (task != null)
			{
				task.ClearFppCache();
			}
		}
		yield break;
	}
}
