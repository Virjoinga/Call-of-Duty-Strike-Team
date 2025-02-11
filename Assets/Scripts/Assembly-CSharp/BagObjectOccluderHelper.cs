using UnityEngine;

public class BagObjectOccluderHelper : MonoBehaviour
{
	public bool RemoveOccluder = true;

	public bool RemoveOccludee;

	private void Awake()
	{
		Object.Destroy(this);
	}
}
