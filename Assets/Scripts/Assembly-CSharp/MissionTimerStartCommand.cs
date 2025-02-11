using System.Collections;

public class MissionTimerStartCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CommonHudController.Instance.MissionTimer.StartTimer();
		yield break;
	}
}
