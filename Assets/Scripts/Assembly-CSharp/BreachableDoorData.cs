using System;
using UnityEngine;

[Serializable]
public class BreachableDoorData
{
	public bool CanOpenAndClose = true;

	public bool Breachable = true;

	public bool IsInterior;

	public BuildingDoor.DoorSate State;

	public NavGateData.CharacterType WalkableBy = NavGateData.CharacterType.Enemy;

	public float TimeToOpen = 0.5f;

	public float OpenAngle = 135f;

	public GameObject DoorMesh;

	public GameObject AdditionalDoorMesh;

	public bool StartLocked;

	public Transform ExplosionOrigin;

	public BuildingDoor.DoorDirection OpeningDirection;

	public bool StartContextDisabled;

	public void CopyContainerData(BuildingDoor bd)
	{
		if (DoorMesh != null)
		{
			bd.DoorMesh = DoorMesh.GetComponentInChildren<MeshFilter>();
		}
		if (AdditionalDoorMesh != null)
		{
			bd.AdditionalDoorMesh = AdditionalDoorMesh.GetComponentInChildren<MeshFilter>();
		}
	}
}
