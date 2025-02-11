using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskDesriptOverrideHubData
{
	public List<ActorOverride> Actors;

	public int EntityAssignmentLimit;

	public List<GameObject> TasksToAssign;

	public bool DestroyOnTrigger;

	public bool ClearCurrentTasks;

	public bool StartOnTrigger = true;

	public bool StartOnPlayerBoxCollider;

	public bool DontEnableControlOnFinished;

	public List<GameObjectBroadcaster> BroadcastOnCompletion;

	public List<GuidRef> ActorsToSelectAfter;

	public void CopyContainerData(TaskDescriptorOverrideHub tdoh)
	{
		tdoh.ActorsToSelect = new List<ActorWrapper>();
		foreach (GuidRef item in ActorsToSelectAfter)
		{
			if (item.theObject != null)
			{
				ActorWrapper componentInChildren = item.theObject.GetComponentInChildren<ActorWrapper>();
				if (componentInChildren != null)
				{
					tdoh.ActorsToSelect.Add(componentInChildren);
				}
			}
		}
	}

	public void ResolveGuidLinks()
	{
		foreach (GuidRef item in ActorsToSelectAfter)
		{
			item.ResolveLink();
		}
	}
}
