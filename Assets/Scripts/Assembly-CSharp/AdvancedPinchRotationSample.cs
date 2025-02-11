using UnityEngine;

public class AdvancedPinchRotationSample : SampleBase
{
	public PinchGestureRecognizer pinchGesture;

	public RotationGestureRecognizer rotationGesture;

	public Transform target;

	public Material rotationMaterial;

	public Material pinchMaterial;

	public Material pinchAndRotationMaterial;

	public float pinchScaleFactor = 0.02f;

	private Material originalMaterial;

	protected override void Start()
	{
		base.Start();
		base.UI.StatusText = "Use two fingers anywhere on the screen to rotate and scale the green object.";
		originalMaterial = target.renderer.sharedMaterial;
		pinchGesture.OnStateChanged += Gesture_OnStateChanged;
		pinchGesture.OnPinchMove += OnPinchMove;
		pinchGesture.SetCanBeginDelegate(CanBeginPinch);
		rotationGesture.OnStateChanged += Gesture_OnStateChanged;
		rotationGesture.OnRotationMove += OnRotationMove;
		rotationGesture.SetCanBeginDelegate(CanBeginRotation);
	}

	private bool CanBeginRotation(GestureRecognizer gr, FingerGestures.IFingerList touches)
	{
		return !pinchGesture.IsActive;
	}

	private bool CanBeginPinch(GestureRecognizer gr, FingerGestures.IFingerList touches)
	{
		return !rotationGesture.IsActive;
	}

	private void Gesture_OnStateChanged(GestureRecognizer source)
	{
		if (source.PreviousState == GestureRecognizer.GestureState.Ready && source.State == GestureRecognizer.GestureState.InProgress)
		{
			base.UI.StatusText = string.Concat(source, " gesture started");
		}
		else if (source.PreviousState == GestureRecognizer.GestureState.InProgress)
		{
			if (source.State == GestureRecognizer.GestureState.Failed)
			{
				base.UI.StatusText = string.Concat(source, " gesture failed");
			}
			else if (source.State == GestureRecognizer.GestureState.Recognized)
			{
				base.UI.StatusText = string.Concat(source, " gesture ended");
			}
		}
		UpdateTargetMaterial();
	}

	private void OnPinchMove(PinchGestureRecognizer source)
	{
		base.UI.StatusText = "Pinch updated by " + source.Delta + " degrees";
		target.transform.localScale += source.Delta * pinchScaleFactor * Vector3.one;
	}

	private void OnRotationMove(RotationGestureRecognizer source)
	{
		base.UI.StatusText = "Rotation updated by " + source.RotationDelta + " degrees";
		target.Rotate(0f, 0f, source.RotationDelta);
	}

	protected override string GetHelpText()
	{
		return "This sample demonstrates advanced use of the GestureRecognizer classes for Pinch and Rotation";
	}

	private void UpdateTargetMaterial()
	{
		Material sharedMaterial = ((pinchGesture.IsActive && rotationGesture.IsActive) ? pinchAndRotationMaterial : (pinchGesture.IsActive ? pinchMaterial : ((!rotationGesture.IsActive) ? originalMaterial : rotationMaterial)));
		target.renderer.sharedMaterial = sharedMaterial;
	}
}
