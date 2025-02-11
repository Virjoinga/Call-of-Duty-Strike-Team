using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DelayedMessageData
{
	public GameObject Target;

	public string Message;

	public string StringParam;

	public GameObject ObjectParam;

	public List<GameObject> GroupObjectToCall;

	public List<string> GroupFunctionToCall;

	public List<string> GroupStringParam;

	public List<GameObject> GroupObjectParam;

	public float TimeDelay;

	public bool LoopTimer;

	public bool DestroyOnTrigger = true;
}
