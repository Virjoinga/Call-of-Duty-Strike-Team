using System;
using UnityEngine;

[Serializable]
public class ActorOverride
{
	public GameObject Actor;

	[HideInInspector]
	public string contGUID = string.Empty;
}
