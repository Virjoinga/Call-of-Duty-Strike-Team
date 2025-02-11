using UnityEngine;

public interface PlayCameraInterface
{
	Vector3 PanOffset { get; }

	Vector3 PanVelocity { get; }

	bool MovingToFocusPoint { get; }

	float WorldRotation { get; }

	void AllowInput(bool allow);

	void EnablePlacementInput(bool enable);

	bool FocusOnTarget(Transform trans, bool forceEvenIfOnScreen);

	void SnapToTarget(Transform trans);

	void FocusAndSelectTarget(Transform trans);
}
