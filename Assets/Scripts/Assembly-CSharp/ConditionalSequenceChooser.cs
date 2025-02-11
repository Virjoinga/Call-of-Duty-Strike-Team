using System.Collections.Generic;
using UnityEngine;

public class ConditionalSequenceChooser : MonoBehaviour
{
	public List<GameObject> conditionalSequenceObjects;

	private List<ConditionalSequence> conditionalSequences = new List<ConditionalSequence>();

	public bool oneShot;

	private bool done;

	private int conditionsMet;

	private void Start()
	{
		if (conditionalSequenceObjects == null)
		{
			return;
		}
		foreach (GameObject conditionalSequenceObject in conditionalSequenceObjects)
		{
			if (conditionalSequenceObject != null)
			{
				ConditionalSequence componentInChildren = conditionalSequenceObject.GetComponentInChildren<ConditionalSequence>();
				if (componentInChildren != null)
				{
					conditionalSequences.Add(componentInChildren);
				}
			}
		}
	}

	private void Update()
	{
		if (done)
		{
			return;
		}
		conditionsMet = 0;
		foreach (ConditionalSequence conditionalSequence in conditionalSequences)
		{
			if (conditionalSequence.HasActivated())
			{
				conditionsMet++;
			}
			else if (conditionalSequence.ConditionsMet())
			{
				conditionalSequence.ActivateSequence();
				conditionsMet++;
				if (oneShot)
				{
					done = true;
				}
			}
		}
		if (conditionsMet >= conditionalSequences.Count)
		{
			done = true;
		}
	}
}
