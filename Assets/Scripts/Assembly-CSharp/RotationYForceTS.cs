using UnityEngine;

public class RotationYForceTS : MonoBehaviour
{
	public GameObject controlledObject;

	private bool RotationOverrider;

	private void Start()
	{
	}

	private void Update()
	{
		if (RotationOverrider)
		{
			float x = controlledObject.transform.rotation.x;
			float z = controlledObject.transform.rotation.z;
			controlledObject.transform.rotation = Quaternion.Slerp(controlledObject.transform.rotation, Quaternion.Euler(x, 182f, z), Time.deltaTime * 3.5f);
		}
	}

	public void ForceYRotationStart()
	{
		RotationOverrider = true;
	}

	public void ForceYRotationStop()
	{
		RotationOverrider = false;
	}
}
