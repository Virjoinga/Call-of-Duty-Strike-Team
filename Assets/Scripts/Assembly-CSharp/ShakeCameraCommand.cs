using System.Collections;

public class ShakeCameraCommand : Command
{
	public bool Wait;

	public float ShakeDuration;

	public float FirstPersonShakeAmount;

	public float ThirdPersonShakeAmount;

	public override bool Blocking()
	{
		return Wait;
	}

	public override IEnumerator Execute()
	{
		CameraBase cb = CameraManager.Instance.PlayCameraController.CurrentCameraBase;
		FirstPersonCamera fpc = cb as FirstPersonCamera;
		if (fpc != null)
		{
			cb.AddShake(FirstPersonShakeAmount, ShakeDuration);
		}
		else
		{
			cb.AddShake(ThirdPersonShakeAmount, ShakeDuration);
		}
		yield return null;
	}
}
