using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlacedC4Data
{
	public GameObject ObjectToCallOnSuccess;

	public string FunctionToCallOnSuccess;

	public List<GameObject> ObjectsToMessageOnCollection;

	public List<string> FunctionsToCallOnCollection;

	public List<GameObject> ParamToPass;

	public bool m_NotifyFlashpointManager = true;
}
