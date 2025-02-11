using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BreachActorData
{
	public ActorDescriptor Actor;

	public bool VisibleBeforeBreach;

	public List<GameObject> ObjectToCallOnDeath;

	public List<string> FunctionToCallOnDeath;

	public void CopyContainerData(BreachActor ba)
	{
		if (Actor != null)
		{
			ba.Actor = Actor;
		}
	}
}
