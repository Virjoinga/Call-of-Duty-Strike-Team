using UnityEngine;

public static class ColourChart
{
	public static Color HealthBarBack = MakeFrom255Colour(255, 102, 0, 164);

	public static Color HealthBarTopFull = MakeFrom255Colour(0, 255, 0, 255);

	public static Color HealthBarTopMed = MakeFrom255Colour(255, 255, 0, 255);

	public static Color HealthBarTopLow = MakeFrom255Colour(255, 102, 0, 255);

	public static Color HealthBarTopEmpty = MakeFrom255Colour(255, 0, 0, 255);

	public static Color HealthBarBackGrey = MakeFrom255Colour(166, 166, 166, 164);

	public static Color HealthBarTopGrey = MakeFrom255Colour(166, 166, 166, 128);

	public static float HEALTH_MIN_PULSE_01 = 0.3f;

	public static Color WaypointMarkerWalk = MakeFrom255Colour(250, 235, 57, 128);

	public static Color WaypointMarkerRun = MakeFrom255Colour(188, 18, 22, 50);

	public static Color WaypointMarkerCover = MakeFrom255Colour(64, 250, 0, 128);

	public static Color WaypointMarkerPreview = MakeFrom255Colour(92, 198, 204, 50);

	public static Color WaypointMarkerHighlightedPreviewHigh = MakeFrom255Colour(92, 198, 204, 50);

	public static Color WaypointMarkerHighlightedPreviewLow = MakeFrom255Colour(92, 198, 204, 50);

	public static Color WaypointMarkerInTransit = MakeFrom255Colour(92, 198, 204, 50);

	public static Color SelectedPlayerSoldierArrow = MakeFrom255Colour(256, 256, 256, 255);

	public static Color SelectedPlayerSoldier = MakeFrom255Colour(256, 256, 256, 255);

	public static Color UnSelectedPlayerSoldier = MakeFrom255Colour(192, 192, 192, 164);

	public static Color UnSelectedPlayerSoldierDim = MakeFrom255Colour(64, 64, 64, 164);

	public static Color UnSelectedPlayerSoldierNameDim = MakeFrom255Colour(256, 256, 256, 164);

	public static Color AimShotIdle = MakeFrom255Colour(255, 255, 255, 255);

	public static Color AimShotActive = MakeFrom255Colour(203, 189, 52, 255);

	public static Color HudYellow = MakeFrom255Colour(203, 189, 52, 255);

	public static Color HudWhite = MakeFrom255Colour(255, 255, 255, 255);

	public static Color HudRed = MakeFrom255Colour(255, 0, 0, 255);

	public static Color HudDarkRed = MakeFrom255Colour(192, 0, 0, 255);

	public static Color HudDarkerRed = MakeFrom255Colour(128, 0, 0, 255);

	public static Color HudGreen = MakeFrom255Colour(0, 255, 0, 255);

	public static Color HudBlue = MakeFrom255Colour(92, 198, 204, 255);

	public static Color SoldierCameoSelectedBg = MakeFrom255Colour(92, 198, 204, 64);

	public static Color SoldierCameoUnSelectedBg = MakeFrom255Colour(46, 99, 102, 64);

	public static Color HudButtonPress = MakeFrom255Colour(255, 255, 0, 255);

	public static Color EnemyBlip = MakeFrom255Colour(215, 6, 6, 190);

	public static Color EnemyBlipTargetted = MakeFrom255Colour(6, 215, 6, 190);

	public static Color FriendlyBlip = MakeFrom255Colour(159, 250, 65, 190);

	public static Color GrenadeThrow = MakeFrom255Colour(215, 215, 215, 200);

	public static Color GrenadeCancel = MakeFrom255Colour(215, 6, 6, 200);

	public static Color ContextMenuItemNormal = MakeFrom255Colour(256, 256, 256, 255);

	public static Color ContextMenuItemOver = MakeFrom255Colour(0, 256, 256, 128);

	public static Color ContextMenuItemInactive = MakeFrom255Colour(256, 0, 0, 128);

	public static Color ContextMenuMarker = MakeFrom255Colour(256, 256, 256, 255);

	public static Color MissionSurvival = MakeFrom255Colour(200, 80, 80, 255);

	public static Color MissionFlashpoint = MakeFrom255Colour(27, 141, 70, 255);

	public static Color MissionStory = MakeFrom255Colour(203, 189, 52, 255);

	public static Color MissionLocked = MakeFrom255Colour(128, 128, 128, 255);

	public static Color MissionEmpty = MakeFrom255Colour(128, 128, 128, 200);

	public static Color MissionDemo = MakeFrom255Colour(255, 128, 0, 200);

	public static Color MissionTest = MakeFrom255Colour(255, 0, 0, 200);

	public static Color GreyedOut = MakeFrom255Colour(70, 70, 70, 128);

	public static Color GreyedOutIcon = MakeFrom255Colour(128, 128, 128, 200);

	public static Color UnselectedSection = MakeFrom255Colour(42, 96, 105, 255);

	public static Color ObjectivePassed = MakeFrom255Colour(27, 141, 70, 255);

	public static Color ObjectiveFailed = MakeFrom255Colour(154, 27, 30, 255);

	public static Color ObjectiveSuccess = MakeFrom255Colour(92, 198, 204, 255);

	public static Color ObjectiveNeutral = MakeFrom255Colour(142, 142, 142, 142);

	public static Color ViewConeEnemy = MakeFrom255Colour(219, 29, 29, 204);

	public static Color ViewConeCamera = MakeFrom255Colour(228, 210, 31, 204);

	public static Color ViewConeSentryGun = MakeFrom255Colour(218, 116, 17, 204);

	public static Color ViewConeFriendly = MakeFrom255Colour(49, 210, 17, 204);

	public static Color LeaderboardEntry = MakeFrom255Colour(255, 255, 255, 255);

	public static Color LeaderboardPlayerEntry = MakeFrom255Colour(92, 198, 204, 255);

	public static Color FrontEndButtonNormal = MakeFrom255Colour(42, 96, 105, 255);

	public static Color FrontEndButtonActive = MakeFrom255Colour(203, 189, 52, 255);

	public static Color FrontEndButtonSelected = MakeFrom255Colour(92, 198, 204, 255);

	public static Color FrontEndButtonDisabled = MakeFrom255Colour(142, 142, 142, 142);

	public static Color FrontEndButtonHighlight = MakeFrom255Colour(203, 189, 52, 255);

	public static Color GetHealthBarColour(float health01)
	{
		if (health01 > 0.95f)
		{
			return HealthBarTopFull;
		}
		if (health01 > 0.8f)
		{
			return HealthBarTopMed;
		}
		if (health01 > 0.5f)
		{
			return HealthBarTopLow;
		}
		return HealthBarTopEmpty;
	}

	public static int GetAnimFrameForHealth(float health01)
	{
		if (health01 > 0.95f)
		{
			return 0;
		}
		if (health01 > 0.8f)
		{
			return 1;
		}
		if (health01 > 0.5f)
		{
			return 2;
		}
		if (health01 >= HEALTH_MIN_PULSE_01)
		{
			return 3;
		}
		return 4;
	}

	private static Color MakeFrom255Colour(int r255, int g255, int b255, int a255)
	{
		return new Color((float)r255 / 255f, (float)g255 / 255f, (float)b255 / 255f, (float)a255 / 255f);
	}
}
