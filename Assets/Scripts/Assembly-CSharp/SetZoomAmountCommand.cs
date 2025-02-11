using System.Collections;

public class SetZoomAmountCommand : Command
{
	public float ZoomLevel;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (!GameController.Instance.IsFirstPerson)
		{
			CameraManager cameras = CameraManager.Instance;
			NavMeshCamera nmc = null;
			if (cameras != null)
			{
				nmc = cameras.PlayCameraController.CurrentCameraBase as NavMeshCamera;
			}
			if (nmc != null)
			{
				nmc.SetZoom(ZoomLevel);
			}
		}
		yield break;
	}
}
