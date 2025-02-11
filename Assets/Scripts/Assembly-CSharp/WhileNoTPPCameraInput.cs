using System.Collections;
using UnityEngine;

public class WhileNoTPPCameraInput : Command
{
	public enum CameraInput
	{
		Move = 0,
		Rotate = 1,
		Zoom = 2,
		ZoomAndRotate = 3
	}

	public CameraInput InputToWaitFor;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		while (GameController.Instance.IsFirstPerson)
		{
			yield return null;
		}
		CameraManager cameras = CameraManager.Instance;
		NavMeshCamera nmc = null;
		if (cameras != null)
		{
			nmc = cameras.PlayCameraController.CurrentCameraBase as NavMeshCamera;
		}
		if (!(nmc != null))
		{
			yield break;
		}
		switch (InputToWaitFor)
		{
		case CameraInput.Move:
		{
			Vector3 panVelocity = nmc.PanVelocity;
			while (panVelocity == nmc.PanVelocity)
			{
				yield return null;
			}
			break;
		}
		case CameraInput.Rotate:
		{
			float orbitVelocity = nmc.IdealYaw;
			float rotAmount = 0f;
			while (rotAmount < 30f)
			{
				rotAmount += Mathf.Abs(orbitVelocity - nmc.IdealYaw);
				orbitVelocity = nmc.IdealYaw;
				yield return null;
			}
			break;
		}
		case CameraInput.Zoom:
		{
			float fov = nmc.Fov;
			float zoomAmount = 0f;
			while (zoomAmount < 5f)
			{
				zoomAmount += Mathf.Abs(fov - nmc.Fov);
				fov = nmc.Fov;
				yield return null;
			}
			break;
		}
		case CameraInput.ZoomAndRotate:
		{
			float fov2 = nmc.Fov;
			float zoomAmount2 = 0f;
			float orbitVelocity2 = nmc.IdealYaw;
			float rotAmount2 = 0f;
			while (zoomAmount2 < 5f && rotAmount2 < 30f)
			{
				zoomAmount2 += Mathf.Abs(fov2 - nmc.Fov);
				fov2 = nmc.Fov;
				rotAmount2 += Mathf.Abs(orbitVelocity2 - nmc.IdealYaw);
				orbitVelocity2 = nmc.IdealYaw;
				yield return null;
			}
			break;
		}
		}
	}
}
