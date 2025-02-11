using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingWithInteriorData
{
	public GameObject Exterior;

	public GameObject Interior;

	public Transform TransitionFromPoint;

	public float MaxPeekAmount = 30f;

	public float TransisionTime = 0.5f;

	public GameObject[] ObjectsToHide;

	public CMWindow[] Windows;

	public List<ActorWrapper> SecurityCameras;
}
