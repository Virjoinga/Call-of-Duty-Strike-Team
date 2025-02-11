using UnityEngine;

public class ListenerLocator : MonoBehaviour
{
	public float MaxHearingRange = 25f;

	public float BiasTowards = 0.8f;

	private void Start()
	{
	}

	private void Update()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			Transform transform = CameraManager.Instance.PlayCamera.transform;
			base.transform.position = transform.position;
			base.transform.rotation = transform.rotation;
		}
		else
		{
			Camera currentCamera = CameraManager.Instance.CurrentCamera;
			Ray ray = currentCamera.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
			RaycastHit hitInfo;
			Vector3 to = ((!Physics.Raycast(ray, out hitInfo, MaxHearingRange, 1)) ? (currentCamera.transform.position + ray.direction * (MaxHearingRange * BiasTowards)) : (currentCamera.transform.position + ray.direction * (hitInfo.distance * BiasTowards)));
			base.transform.position = Vector3.Lerp(base.transform.position, to, Time.deltaTime * 8f);
			base.transform.rotation = currentCamera.transform.rotation;
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = base.transform.position + new Vector3(0f, 0.5f, 0f);
		Gizmos.color = Color.white;
		Gizmos.DrawIcon(vector, "ears");
		Gizmos.color = Color.red;
		Gizmos.DrawLine(vector, vector - base.transform.right);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(vector, vector + base.transform.right);
	}
}
