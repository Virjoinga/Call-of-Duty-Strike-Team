using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultiManSwitchData
{
	public List<GameObject> RequiredTerminals = new List<GameObject>();

	public List<GameObject> ObjectsToMessageOnSuccess = new List<GameObject>();

	public List<string> FunctionsToCallOnSuccess;

	public List<GameObject> ObjectsToMessageOnFailure = new List<GameObject>();

	public List<string> FunctionsToCallOnFailure;

	public float MaxTimeBetweenActivations = 1f;
}
