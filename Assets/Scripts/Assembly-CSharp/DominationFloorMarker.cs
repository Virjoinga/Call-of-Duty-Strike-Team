using UnityEngine;

public class DominationFloorMarker : MonoBehaviour
{
	public float RotationSpeed = 1f;

	public GameObject Beacon;

	private void Start()
	{
		SetColourBeacon(Color.blue);
	}

	private void Update()
	{
		if (Beacon != null)
		{
			base.transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime, Space.World);
		}
	}

	public void SetColourBeacon(Color Colour)
	{
		if (Beacon != null)
		{
			Beacon.GetComponent<Renderer>().material.color = Colour;
		}
	}
}
