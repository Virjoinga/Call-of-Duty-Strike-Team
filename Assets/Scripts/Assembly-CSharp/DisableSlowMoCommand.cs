using System.Collections;

public class DisableSlowMoCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		TimeManager.instance.DisableSlomo();
		yield break;
	}
}
