using System;
using UnityEngine;

[Serializable]
public class DebugActorWarpData
{
	public bool Active = true;

	public bool FocusCameraHere;

	public GameObject ActorToWarp;

	public void CopyContainerData(DebugActorWarp wa)
	{
		if (ActorToWarp != null)
		{
			wa.actorWrapper = ActorToWarp.GetComponentInChildren<ActorWrapper>();
		}
	}
}
