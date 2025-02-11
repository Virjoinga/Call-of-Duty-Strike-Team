using System;
using UnityEngine;

[Serializable]
public class MoreInfoTriggerData
{
	public bool RequireSelectedSoldiers = true;

	public GameObject ScriptedSequence;

	public void CopyContainerData(MoreInfoTrigger mt)
	{
		if ((bool)ScriptedSequence)
		{
			mt.ScriptSeq = ScriptedSequence.GetComponentInChildren<ScriptedSequence>();
		}
	}
}
