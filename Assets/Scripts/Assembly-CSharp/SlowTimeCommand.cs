using System.Collections;

public class SlowTimeCommand : Command
{
	public TimeManager.SlowTimeType Type;

	public float Duration;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		TimeManager.instance.SlowDownTime(Duration, Type);
		yield break;
	}
}
