using UnityEngine;

public class BuildingInteriorVolume : MonoBehaviour
{
	public BuildingWithInterior ParentBuilding;

	private void OnTriggerEnter(Collider other)
	{
		ParentBuilding.OnTriggerEnter(other);
	}

	private void OnTriggerExit(Collider other)
	{
		ParentBuilding.OnTriggerExit(other);
	}
}
