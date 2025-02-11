using UnityEngine;

public class DominationBeaconRadar : MonoBehaviour
{
	public GameObject radarMesh;

	public GameObject radarCollision;

	public float RotationSpeed = 50f;

	private void Start()
	{
	}

	private void Update()
	{
		if (radarMesh != null && radarCollision != null)
		{
			radarMesh.transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime, Space.World);
			radarCollision.transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime, Space.World);
		}
	}
}
