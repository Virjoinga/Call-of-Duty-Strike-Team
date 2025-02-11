using System.Collections;

public class SetMissionTimerCommand : Command
{
	public float StartTime;

	public float EndTime;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CommonHudController.Instance.MissionTimer.Set(StartTime, EndTime);
		yield break;
	}
}
