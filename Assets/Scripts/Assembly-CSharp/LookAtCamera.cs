using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : CameraBase
{
	public Transform LookAt;

	private void Start()
	{
	}

	private void Update()
	{
		if (LookAt != null)
		{
			Vector3 vector = LookAt.transform.position - base.transform.position;
			base.transform.rotation = Quaternion.LookRotation(vector.normalized, base.transform.up);
		}
	}
}
