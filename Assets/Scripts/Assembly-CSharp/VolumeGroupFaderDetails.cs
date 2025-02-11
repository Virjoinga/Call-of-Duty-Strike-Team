using System;
using UnityEngine;

[Serializable]
public class VolumeGroupFaderDetails
{
	public SoundFXData.VolumeGroup VolumeGroupToFade;

	public float TimeToFade;

	[HideInInspector]
	public float CurrentVolume;

	[HideInInspector]
	public float StartVolume = -1f;

	public float TargetVolume = -1f;
}
