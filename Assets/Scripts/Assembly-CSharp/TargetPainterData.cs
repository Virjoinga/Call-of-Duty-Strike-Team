using System;
using UnityEngine;

[Serializable]
public class TargetPainterData
{
	public float TimeToPaint = 5f;

	public GameObject ObjectToCallOnStart;

	public string FunctionToCallOnStart;

	public GameObject ObjectToCallOnSuccess;

	public string FunctionToCallOnSuccess;
}
