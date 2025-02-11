using System.Collections;

public class WhileCameraNotFocusingCommand : Command
{
	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
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
		if (camera != null)
		{
			while (!camera.IsFocusingOnPoint())
			{
				yield return null;
			}
		}
	}
}
