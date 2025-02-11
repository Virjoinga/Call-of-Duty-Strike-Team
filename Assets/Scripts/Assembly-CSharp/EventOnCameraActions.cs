using UnityEngine;

public class EventOnCameraActions : EventDescriptor
{
	public enum CameraEvents
	{
		Drag = 0,
		Rotate = 1,
		Zoom = 2
	}

	public CameraEvents CameraEvent;

	public override void Start()
	{
		switch (CameraEvent)
		{
		case CameraEvents.Drag:
			InputManager.Instance.AddOnFingerDragEndEventHandler(OnDrag, 0);
			break;
		case CameraEvents.Rotate:
			InputManager.Instance.AddOnRotateEndEventHandler(OnRotate, 0);
			break;
		case CameraEvents.Zoom:
			FingerGestures.OnPinchEnd += OnZoom;
			break;
		}
	}

	private void OnDrag(int fingerIndex, Vector2 fingerPos)
	{
		if (fingerIndex == 0)
		{
			FireEvent();
		}
	}

	private void OnRotateEditor(int fingerIndex, Vector2 fingerPos)
	{
		if (fingerIndex == 1)
		{
			FireEvent();
		}
	}

	private void OnRotate(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
	{
		FireEvent();
	}

	private void OnZoom(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		FireEvent();
	}

	public override void DeInitialise()
	{
		switch (CameraEvent)
		{
		case CameraEvents.Drag:
			InputManager.Instance.RemoveOnFingerDragEndEventHandler(OnDrag);
			break;
		case CameraEvents.Rotate:
			InputManager.Instance.RemoveOnRotateEndEventHandler(OnRotate);
			break;
		case CameraEvents.Zoom:
			FingerGestures.OnPinchEnd -= OnZoom;
			break;
		}
	}

	public void OnDestroy()
	{
		DeInitialise();
	}
}
