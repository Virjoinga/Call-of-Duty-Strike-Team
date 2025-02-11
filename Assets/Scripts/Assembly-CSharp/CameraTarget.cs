using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	public NavMeshCamera GameCamera;

	private void Start()
	{
		if (GameCamera != null)
		{
			GameCamera.FocusOnPoint(base.transform.position, base.transform.rotation.eulerAngles.y);
		}
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = base.transform.forward * 2f;
		Vector3 right = base.transform.right;
		Gizmos.color = Color.blue;
		Vector3 position = base.transform.position;
		Vector3 vector2 = base.transform.position + vector;
		Vector3 to = vector2 - vector / 3f - right;
		Vector3 to2 = vector2 - vector / 3f + right;
		Gizmos.DrawLine(position, vector2);
		Gizmos.DrawLine(vector2, to);
		Gizmos.DrawLine(vector2, to2);
	}
}
