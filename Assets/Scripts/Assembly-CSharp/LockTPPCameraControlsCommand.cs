using System.Collections;

public class LockTPPCameraControlsCommand : Command
{
	public bool CameraMove;

	public bool CameraMoveX;

	public bool CameraMoveZ;

	public bool CameraRotate;

	public bool CameraZoom;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CameraController cc = CameraManager.Instance.PlayCameraController;
		if (!(cc != null))
		{
			yield break;
		}
		PlayCameraInterface cfd = cc.StartCamera as PlayCameraInterface;
		if (cfd != null)
		{
			NavMeshCamera nmc = cfd as NavMeshCamera;
			if (nmc != null)
			{
				nmc.allowPanning = !CameraMove;
				nmc.allowPinchZoom = !CameraZoom;
				nmc.allowOrbit = !CameraRotate;
				TutorialToggles.LockCameraMoveX = CameraMoveX;
				TutorialToggles.LockCameraMoveZ = CameraMoveZ;
			}
		}
	}
}
