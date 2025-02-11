using System.Collections.Generic;
using UnityEngine;

public class ConditionalSequence : MonoBehaviour
{
	public List<Condition> conditions = new List<Condition>();

	[HideInInspector]
	public ScriptedSequence sequence;

	[HideInInspector]
	public ConditionalSequenceData m_Interface;

	private bool activated;

	public void Update()
	{
		if (m_Interface.independant && !activated && ConditionsMet())
		{
			ActivateSequence();
		}
	}

	public bool ConditionsMet()
	{
		if (activated)
		{
			return false;
		}
		if (conditions.Count < 1)
		{
			return false;
		}
		bool result = true;
		foreach (Condition condition in conditions)
		{
			if (!condition.Value())
			{
				result = false;
			}
			else if (!m_Interface.allConditionsRequired)
			{
				return true;
			}
		}
		return result;
	}

	public void ActivateSequence()
	{
		sequence.ResetSequence();
		sequence.Activate();
		activated = true;
	}

	public bool HasActivated()
	{
		return activated;
	}
}
