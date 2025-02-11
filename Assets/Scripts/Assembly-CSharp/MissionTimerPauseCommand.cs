using System.Collections;

public class MissionTimerPauseCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CommonHudController.Instance.MissionTimer.PauseTimer();
		yield break;
	}
}
