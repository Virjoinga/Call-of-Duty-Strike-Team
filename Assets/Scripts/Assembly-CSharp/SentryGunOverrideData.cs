using System;
using UnityEngine;

[Serializable]
public class SentryGunOverrideData
{
	public float Speed = 0.15f;

	public float Health = 100f;

	public float VisionDistance = 25f;

	public float VisionFov = 45f;

	[HideInInspector]
	public float WarmTime = 0.5f;

	[HideInInspector]
	public float CoolTime = 2f;

	[HideInInspector]
	public float ZoneInOnTargetTime = 0.3f;

	public float TimeToHack = 5f;
}
