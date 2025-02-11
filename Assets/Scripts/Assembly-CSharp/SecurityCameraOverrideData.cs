using System;

[Serializable]
public class SecurityCameraOverrideData
{
	public float TravelRadius = 180f;

	public float Speed = 1f;

	public float ConeRange = 50f;

	public float ConeAngle = 45f;

	public float PauseTimeLeft;

	public float PauseTimeRight;

	public bool UseAirborneCover;
}
