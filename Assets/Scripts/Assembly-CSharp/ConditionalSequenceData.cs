using System;
using UnityEngine;

[Serializable]
public class ConditionalSequenceData
{
	public GameObject sequence;

	public bool independant;

	public bool allConditionsRequired = true;

	public void CopyContainerData(ConditionalSequence cs)
	{
		if (sequence != null)
		{
			ScriptedSequence componentInChildren = sequence.GetComponentInChildren<ScriptedSequence>();
			if (componentInChildren != null)
			{
				cs.sequence = componentInChildren;
			}
		}
	}
}
