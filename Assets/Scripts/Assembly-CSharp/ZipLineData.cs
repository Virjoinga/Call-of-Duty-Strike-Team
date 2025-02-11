using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ZipLineData
{
	public bool ForceFirstPerson;

	public List<GameObject> GroupObjectToCallOnStart = new List<GameObject>();

	public List<string> GroupFunctionToCallOnStart = new List<string>();
}
