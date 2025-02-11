using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HostageSequenceData
{
	public ActorDescriptor HostageTaker;

	public ActorDescriptor Hostage;

	public bool VisibleBeforeFinalSequence;

	public List<GameObject> ObjectToCallOnHostageDeath;

	public List<string> FunctionToCallOnHostageDeath;

	public List<GameObject> ObjectToCallOnTakerDeath;

	public List<string> FunctionToCallOnTakerDeath;

	public List<GameObject> ObjectToCallOnHostageSaved;

	public List<string> FunctionToCallOnHostageSaved;

	public void CopyContainerData(HostageSequence hs)
	{
		hs.Hostage = Hostage ?? hs.Hostage;
		hs.HostageTaker = HostageTaker ?? hs.HostageTaker;
	}
}
