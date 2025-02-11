using System.Collections;

public class BlackBarsCommand : Command
{
	public bool Enable;

	public bool Skippable = true;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		BlackBarsController.Instance.SetBlackBars(Enable, Skippable);
		yield break;
	}
}
