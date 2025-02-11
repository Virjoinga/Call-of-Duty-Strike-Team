using System.Collections.Generic;
using UnityEngine;

public static class TutorialToggles
{
	public enum CMButtonLockState
	{
		Active = 0,
		Hidden = 1,
		GreyedOut = 2
	}

	public static bool enableDoubleTapAimedShot = true;

	public static bool enableEnemyContextMenu = true;

	public static bool enableGhostDrag = true;

	public static bool enableTapToMove = true;

	public static bool enableSoldierMarkerButton = true;

	public static bool LockToWalkOnly = false;

	public static bool LockToRunOnly = false;

	public static GameObject TapRestrictionObject = null;

	public static float tapRestrictionRadius = 0f;

	public static float snapModifier = 0f;

	public static bool DisableAllInteractions = false;

	public static List<GameObject> ActiveHighlights = new List<GameObject>();

	public static bool[] ActiveHighlightStates;

	public static bool LockFPPMovement = false;

	public static bool LockFPPLook = false;

	public static bool LockZoom = false;

	public static bool PlayerSelectionLocked = false;

	public static bool PlayerSelectAllLocked = false;

	public static CMButtonLockState CMMeeleLockState = CMButtonLockState.Active;

	public static CMButtonLockState CMAimedShotLockState = CMButtonLockState.Active;

	public static CMButtonLockState CMSupressLockState = CMButtonLockState.Active;

	public static CMButtonLockState CMCarryBodyLockState = CMButtonLockState.Active;

	public static int RespotCount = 0;

	public static bool IsRespotting = false;

	public static bool ShouldClearHighlights = false;

	public static bool LockCameraMoveX = false;

	public static bool LockCameraMoveZ = false;

	public static bool LockCameraFocus = false;

	public static bool HighlightingCM = false;

	public static bool WithinTapRestrictionRadius(Vector3 worldPos, ref Vector3 ajustedPosition)
	{
		ajustedPosition = worldPos;
		if (TapRestrictionObject == null || tapRestrictionRadius == 0f)
		{
			return true;
		}
		ajustedPosition = TapRestrictionObject.transform.position;
		return (TapRestrictionObject.transform.position - worldPos).sqrMagnitude < tapRestrictionRadius * tapRestrictionRadius;
	}

	public static void Reset()
	{
		enableDoubleTapAimedShot = true;
		enableEnemyContextMenu = true;
		enableGhostDrag = true;
		enableTapToMove = true;
		enableSoldierMarkerButton = true;
		TapRestrictionObject = null;
		tapRestrictionRadius = 0f;
		DisableAllInteractions = false;
		ActiveHighlightStates = null;
		ActiveHighlights = new List<GameObject>();
		LockFPPMovement = false;
		LockFPPLook = false;
		LockToWalkOnly = false;
		LockToRunOnly = false;
		PlayerSelectionLocked = false;
		CMMeeleLockState = CMButtonLockState.Active;
		CMAimedShotLockState = CMButtonLockState.Active;
		CMSupressLockState = CMButtonLockState.Active;
		CMCarryBodyLockState = CMButtonLockState.Active;
		LockZoom = false;
		LockCameraMoveX = false;
		LockCameraMoveZ = false;
		LockCameraFocus = false;
		RespotCount = 0;
		snapModifier = 0f;
		HighlightingCM = false;
	}
}
