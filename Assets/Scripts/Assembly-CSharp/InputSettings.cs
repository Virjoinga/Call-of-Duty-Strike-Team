public static class InputSettings
{
	public static float DeviceWidthScale = 1f;

	public static float DeviceHeightScale = 1f;

	public static bool GyroAntiDrift = true;

	public static float GyroDriftThreshold = 0.034f;

	public static float GyroSmoothing = 1f;

	public static float GyroDriftSmoothing = 0.8f;

	public static float FirstPersonGyroSensitivityHorizontalLow;

	public static float FirstPersonGyroSensitivityHorizontalHigh = 32f;

	public static float FirstPersonGyroSensitivityVerticalLow;

	public static float FirstPersonGyroSensitivityVerticalHigh = 16f;

	public static float FirstPersonTrackpadLookSensitivityHorizontalLow = 0.1f;

	public static float FirstPersonTrackpadLookSensitivityHorizontalHigh = 15f;

	public static float FirstPersonTrackpadLookSensitivityVerticalLow = 0.1f;

	public static float FirstPersonTrackpadLookSensitivityVerticalHigh = 8f;

	public static float FirstPersonGamepadLookSensitivityHorizontalLow = 10f;

	public static float FirstPersonGamepadLookSensitivityHorizontalHigh = 720f;

	public static float FirstPersonGamepadLookSensitivityVerticalLow = 10f;

	public static float FirstPersonGamepadLookSensitivityVerticalHigh = 720f;

	public static float OverwatchGamepadLookSensitivityHorizontalLow = 10f;

	public static float OverwatchGamepadLookSensitivityHorizontalHigh = 80f;

	public static float OverwatchGamepadLookSensitivityVerticalLow = 10f;

	public static float OverwatchGamepadLookSensitivityVerticalHigh = 80f;

	public static float FirstPersonTrackpadHorizontalAccelerationThreshold = 0.05f;

	public static float FirstPersonTrackpadHorizontalAccelerationAmount = 8f;

	public static float FirstPersonTrackpadFilteringCutoff = 1000f;

	public static float FirstPersonTrackpadFilteringFalloff = 0.75f;

	public static float FirstPersonTrackpadVerticalAccelerationThreshold = 0.05f;

	public static float FirstPersonTrackpadVerticalAccelerationAmount = 8f;

	public static float FirstPersonAimAssistSensitivityModifierX = 0.4f;

	public static float FirstPersonAimAssistSensitivityModifierY = 0.1f;

	public static float FirstPersonDoubleTapThreshold = 0.3f;

	public static bool ApplySprintFieldOfView = true;

	public static float FirstPersonFieldOfViewStandard = 65f;

	public static float FirstPersonFieldOfViewSprint = 70f;

	public static float FirstPersonFieldOfViewSprintThreshold = 4f;

	public static float FirstPersonViewOffsetStandard;

	public static float FirstPersonViewOffsetSprint = 0.01f;

	public static float FirstPersonSprintZoneStartCosine = 0.7f;

	public static float FirstPersonSprintZoneEndCosine = 0.5f;

	public static float FirstPersonSprintZoneEndMultiplier = 0.85f;

	public static float FirstPersonViewBobAmount = 0.042f;

	public static float FirstPersonCrosshairScaling = 0.26f;

	public static float FirstPersonHeightHeightCrouched = 0.8f;

	public static float FirstPersonHeightHeightStanding = 1.6f;

	public static bool DirectFireToSoftLockPosition;

	public static bool ShowSoftLockIndicator;

	public static float OverwatchGyroSensitivityHorizontalLow;

	public static float OverwatchGyroSensitivityHorizontalHigh = 5f;

	public static float OverwatchGyroSensitivityVerticalLow;

	public static float OverwatchGyroSensitivityVerticalHigh = 4f;

	public static float OverwatchTrackpadLookSensitivityHorizontalLow = 0.5f;

	public static float OverwatchTrackpadLookSensitivityHorizontalHigh = 1f;

	public static float OverwatchTrackpadLookSensitivityVerticalLow = 0.5f;

	public static float OverwatchTrackpadLookSensitivityVerticalHigh = 1f;

	public static float FirstPersonFieldOfView { get; set; }

	public static float FirstPersonViewOffset { get; set; }
}
