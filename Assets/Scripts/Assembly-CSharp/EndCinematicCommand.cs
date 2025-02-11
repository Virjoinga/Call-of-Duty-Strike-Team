using System.Collections;

public class EndCinematicCommand : Command
{
	public bool UpdateHUD = true;

	public bool AutoSwitchToFPP = true;

	public bool AllowTransitions = true;

	public bool MaintainSelection = true;

	public bool ForceHUDOff;

	public bool KeepBarsOn;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (KeepBarsOn)
		{
			BlackBarsController bbc = BlackBarsController.Instance;
			if (bbc != null)
			{
				bbc.HideSkipAndSubtitles();
			}
		}
		else
		{
			CinematicHelper.End(UpdateHUD, AutoSwitchToFPP, AllowTransitions, MaintainSelection, ForceHUDOff);
		}
		yield break;
	}
}
