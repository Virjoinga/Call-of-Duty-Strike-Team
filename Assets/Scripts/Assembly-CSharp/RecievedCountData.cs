using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecievedCountData
{
	public int TargetMessageRecievedCount;

	public GameObject ObjectToMessage;

	public string OptionalMessage;

	public string OptionalStringParam;

	public GameObject OptionalObjectParam;

	public List<GameObject> GroupObjectToCall;

	public List<string> GroupFunctionToCall;

	public List<string> GroupStringParam;

	public List<GameObject> GroupObjectParam;

	public bool ResetOnFinish;

	public bool DestroyOnFinish = true;
}
