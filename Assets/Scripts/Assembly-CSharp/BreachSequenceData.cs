using System;
using UnityEngine;

[Serializable]
public class BreachSequenceData
{
	public float MaximumSlowDownDuration = 10f;

	public GameObject BreachComponents;

	public BreachMessages BreachMessages = new BreachMessages();

	public int RequiredActors = 1;
}
