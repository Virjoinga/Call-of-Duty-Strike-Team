using System.Collections;

public class SwitchToStrategyCameraCommand : Command
{
	public bool Wait;

	public override bool Blocking()
	{
		return Wait;
	}

	public override IEnumerator Execute()
	{
		GameController.Instance.SwitchToStrategyCamera(null);
		while (CameraManager.Instance.IsSwitching)
		{
			yield return null;
		}
	}
}
