using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
	public Camera FaceCamera;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if (FaceCamera != null)
		{
			Vector3 vector = FaceCamera.transform.position - base.transform.position;
			base.transform.rotation = Quaternion.LookRotation(vector.normalized, FaceCamera.transform.up);
		}
	}
}
