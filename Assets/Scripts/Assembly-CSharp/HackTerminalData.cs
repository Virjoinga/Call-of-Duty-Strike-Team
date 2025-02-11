using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HackTerminalData
{
	public float TimeToHack = 5f;

	public GameObject ObjectToCallOnSuccess;

	public string FunctionToCallOnSuccess;

	public List<GameObject> GroupObjectToCallOnSuccess = new List<GameObject>();

	public List<string> GroupFunctionToCallOnSuccess = new List<string>();

	public GameObject ObjectToCallOnFail;

	public string FunctionToCallOnFail;

	public List<GameObject> GroupObjectToCallOnFail = new List<GameObject>();

	public List<string> GroupFunctionToCallOnFail = new List<string>();

	public TimerSwitchData TimerData;
}
