using System.Collections;

public class MissionTimerStopCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CommonHudController.Instance.MissionTimer.StopTimer();
		yield break;
	}
}
