using UnityEngine;

public class DebugCameraSwitcher : MonoBehaviour
{
	public CameraController CameraController;

	public CameraTransitionData[] CameraTransition;

	public int UseTransition = -1;

	private void Start()
	{
	}

	private void Update()
	{
		if (UseTransition >= 0)
		{
			CameraController.BlendTo(CameraTransition[UseTransition]);
			UseTransition = -1;
		}
	}
}
