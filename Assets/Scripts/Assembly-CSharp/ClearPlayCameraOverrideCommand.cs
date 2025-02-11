using System.Collections;
using UnityEngine;

public class ClearPlayCameraOverrideCommand : Command
{
	public float TransitionSeconds;

	public bool Wait;

	public override bool Blocking()
	{
		return Wait;
	}

	public override IEnumerator Execute()
	{
		CameraController cc = CameraManager.Instance.PlayCameraController;
		cc.RestoreCameraToDefault(TransitionSeconds);
		GameController.Instance.ExitFirstPerson();
		if (Wait)
		{
			yield return new WaitForSeconds(TransitionSeconds);
		}
	}
}
