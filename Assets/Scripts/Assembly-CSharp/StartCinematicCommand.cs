using System.Collections;

public class StartCinematicCommand : Command
{
	public bool Skippable = true;

	public bool UpdateHUD = true;

	public bool AllowFPPAnims;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (HUDMessenger.Instance != null)
		{
			HUDMessenger.Instance.HideActiveMessages(true);
		}
		CinematicHelper.Begin(Skippable, UpdateHUD, AllowFPPAnims);
		yield break;
	}
}
