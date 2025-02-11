using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SwitchRoutineData
{
	public List<GameObject> Actors = new List<GameObject>();

	public List<GuidRef> FirstRoutines = new List<GuidRef>();

	public float FirstRoutineTimeOut;

	public List<GuidRef> SecondRoutines = new List<GuidRef>();

	public float SecondRoutineTimeOut;

	public List<GameObject> TriggerActors = new List<GameObject>();

	public bool SwitchOnTriggerVolume;

	public bool AllActorsRequiredInVolume;

	public bool IgnoreActorOfInterestCheck;

	public bool OneShot;

	public void CopyContainerData(SwitchRoutine r)
	{
		r.Actors.Clear();
		foreach (GameObject actor in Actors)
		{
			ActorWrapper[] componentsInChildren = actor.GetComponentsInChildren<ActorWrapper>();
			foreach (ActorWrapper item in componentsInChildren)
			{
				r.Actors.Add(item);
			}
		}
		r.TriggerActors.Clear();
		foreach (GameObject triggerActor in TriggerActors)
		{
			ActorWrapper[] componentsInChildren2 = triggerActor.GetComponentsInChildren<ActorWrapper>();
			foreach (ActorWrapper item2 in componentsInChildren2)
			{
				r.TriggerActors.Add(item2);
			}
		}
		r.FirstRoutines.Clear();
		foreach (GuidRef firstRoutine in FirstRoutines)
		{
			firstRoutine.ResolveLink();
			TaskDescriptor componentInChildren = firstRoutine.theObject.GetComponentInChildren<TaskDescriptor>();
			if (componentInChildren != null)
			{
				r.FirstRoutines.Add(componentInChildren);
			}
		}
		r.SecondRoutines.Clear();
		foreach (GuidRef secondRoutine in SecondRoutines)
		{
			secondRoutine.ResolveLink();
			TaskDescriptor componentInChildren2 = secondRoutine.theObject.GetComponentInChildren<TaskDescriptor>();
			if (componentInChildren2 != null)
			{
				r.SecondRoutines.Add(componentInChildren2);
			}
		}
	}
}
