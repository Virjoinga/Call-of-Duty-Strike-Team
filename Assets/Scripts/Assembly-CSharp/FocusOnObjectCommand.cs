using System.Collections;
using UnityEngine;

public class FocusOnObjectCommand : Command
{
	public GameObject spawner;

	public bool snap;

	public bool OnlyFocusIfOffScreen;

	public float Speed = 0.3f;

	public float Rotation = -1f;

	public float RotationSpeed = 8f;

	public float Zoom = -1f;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (!(spawner != null))
		{
			yield break;
		}
		ActorWrapper aw = spawner.GetComponentInChildren<ActorWrapper>();
		Actor a = null;
		Transform target = null;
		if (aw != null)
		{
			a = aw.GetActor();
			if (a != null)
			{
				target = a.transform;
			}
		}
		if (target == null)
		{
			target = spawner.transform;
		}
		bool shouldFocus = true;
		if (OnlyFocusIfOffScreen)
		{
			if (a != null)
			{
				shouldFocus = !a.OnScreen;
			}
			else
			{
				float radiusFactor = 1.4f;
				Vector3 characterCentre = target.position + base.transform.up;
				Vector3 viewPortPos = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(characterCentre);
				float radius = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(characterCentre + radiusFactor * CameraManager.Instance.CurrentCamera.transform.right).x - viewPortPos.x;
				if (viewPortPos.z >= 0f && viewPortPos.x >= 0f - radius && viewPortPos.x <= 1f + radius && viewPortPos.y >= 0f - radius && viewPortPos.y <= 1f + radius)
				{
					shouldFocus = false;
				}
			}
		}
		if (!shouldFocus)
		{
			yield break;
		}
		CameraManager cameras = CameraManager.Instance;
		if (!(cameras != null))
		{
			yield break;
		}
		NavMeshCamera camera = cameras.PlayCameraController.CurrentCameraBase as NavMeshCamera;
		while (camera == null)
		{
			camera = cameras.PlayCameraController.CurrentCameraBase as NavMeshCamera;
			yield return null;
		}
		if (!(camera != null))
		{
			yield break;
		}
		Rotation = ((Rotation != -1f) ? Rotation : camera.Yaw);
		Zoom = ((Zoom != -1f) ? Zoom : camera.Fov);
		if (snap)
		{
			camera.SnapToTarget(target, Rotation, Zoom);
			yield break;
		}
		camera.SpeedToFocusPoint = Speed;
		camera.FocusOnPoint(target.position, Rotation, Zoom);
		if (camera.smoothOrbitSpeed != RotationSpeed)
		{
			camera.smoothOrbitSpeed = RotationSpeed;
			camera.OrbitSpeedAltered = true;
		}
	}
}
