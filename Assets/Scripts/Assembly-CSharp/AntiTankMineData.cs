using System;
using UnityEngine;

[Serializable]
public class AntiTankMineData
{
	public float TimeToDefuse = 5f;

	public GameObject ObjectToCallOnStart;

	public string FunctionToCallOnStart;

	public GameObject ObjectToCallOnSuccess;

	public string FunctionToCallOnSuccess;
}
