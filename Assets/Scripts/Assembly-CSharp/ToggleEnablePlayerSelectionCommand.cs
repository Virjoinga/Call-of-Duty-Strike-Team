using System.Collections;

public class ToggleEnablePlayerSelectionCommand : Command
{
	public bool LockSelectIndividual;

	public bool LockSelectAll;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		TutorialToggles.PlayerSelectionLocked = LockSelectIndividual;
		TutorialToggles.PlayerSelectAllLocked = LockSelectAll;
		if (LockSelectIndividual)
		{
			CommonHudController.Instance.HideFPPUntChangeButtons(true);
		}
		else if (GKM.UnitCount(GKM.PlayerControlledMask & GKM.UpAndAboutMask) > 1)
		{
			CommonHudController.Instance.HideFPPUntChangeButtons(false);
		}
		yield break;
	}
}
