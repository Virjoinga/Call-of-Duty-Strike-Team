using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CollectableData
{
	public enum PositioningType
	{
		OnTable = 0,
		OnFloor = 1
	}

	public List<GameObject> ObjectsToMessageOnCollection;

	public List<string> FunctionsToCallOnCollection;

	public List<GameObject> ParamToPass;

	public PositioningType Positioning;

	public bool TutorialIntel;
}
