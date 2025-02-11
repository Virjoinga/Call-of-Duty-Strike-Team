using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HackableObjectData
{
	public float TimeToHack = 5f;

	public bool IsMultiManHack;

	public GameObject ObjectToCallOnStart;

	public string FunctionToCallOnStart;

	public GameObject ObjectToCallOnSuccess;

	public string FunctionToCallOnSuccess;

	public List<GameObject> GroupObjectToCallOnSuccess = new List<GameObject>();

	public List<string> GroupFunctionToCallOnSuccess = new List<string>();

	public GameObject ObjectToCallOnFail;

	public string FunctionToCallOnFail;

	public List<GameObject> GroupObjectToCallOnFail = new List<GameObject>();

	public List<string> GroupFunctionToCallOnFail = new List<string>();

	public TimerSwitchData TimerData = new TimerSwitchData();
}
