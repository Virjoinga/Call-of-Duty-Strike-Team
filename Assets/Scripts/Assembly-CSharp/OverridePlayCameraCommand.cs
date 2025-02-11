using System.Collections;
using UnityEngine;

public class OverridePlayCameraCommand : Command
{
	public CameraBase Override;

	public float TransitionSeconds;

	public bool Wait;

	public override bool Blocking()
	{
		return Wait;
	}

	public override IEnumerator Execute()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			GameController.Instance.ExitFirstPerson();
		}
		CameraController cc = CameraManager.Instance.PlayCameraController;
		cc.BlendTo(new CameraTransitionData(Override, TweenFunctions.TweenType.easeInOutCubic, TransitionSeconds));
		if (Wait)
		{
			yield return new WaitForSeconds(TransitionSeconds);
		}
	}
}
