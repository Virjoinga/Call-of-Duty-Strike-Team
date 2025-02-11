using System.Collections;

public class SwitchToPlayCameraCommand : Command
{
	public bool Wait;

	public override bool Blocking()
	{
		return Wait;
	}

	public override IEnumerator Execute()
	{
		GameController.Instance.SwitchToPlayCamera();
		while (CameraManager.Instance.IsSwitching)
		{
			yield return null;
		}
	}
}
