using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriggerVolumeData
{
	public bool AnyPlayer;

	public string EntityType;

	public bool Locked;

	public List<GameObject> NotifyOnEnter;

	public List<string> OptionalFunctionToCallOnEnter = new List<string>();

	public List<string> OptionalStringParamToPassOnEnter = new List<string>();

	public List<GameObject> NotifyOnLeave;

	public List<string> OptionalFunctionToCallOnLeave = new List<string>();

	public List<string> OptionalStringParamToPassOnLeave = new List<string>();

	public bool OneShot = true;
}
