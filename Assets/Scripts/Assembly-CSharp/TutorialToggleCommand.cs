using System.Collections;

public class TutorialToggleCommand : Command
{
	public enum TutorialToggleEnum
	{
		AimedShotDoubleTap = 0,
		EnemyContextMenus = 1,
		GhostDrag = 2,
		SoldierMarker = 3,
		CameraFocus = 4
	}

	public TutorialToggleEnum toggleType;

	public bool enable;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		switch (toggleType)
		{
		case TutorialToggleEnum.AimedShotDoubleTap:
			TutorialToggles.enableDoubleTapAimedShot = enable;
			break;
		case TutorialToggleEnum.EnemyContextMenus:
			TutorialToggles.enableEnemyContextMenu = enable;
			break;
		case TutorialToggleEnum.GhostDrag:
			TutorialToggles.enableGhostDrag = enable;
			break;
		case TutorialToggleEnum.SoldierMarker:
			TutorialToggles.enableSoldierMarkerButton = enable;
			break;
		case TutorialToggleEnum.CameraFocus:
			TutorialToggles.LockCameraFocus = !enable;
			break;
		}
		yield break;
	}
}
