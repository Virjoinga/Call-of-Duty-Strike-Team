using System.Collections;

public class AddToMessageLogCommand : Command
{
	public string Label;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CommonHudController.Instance.AddToMessageLog(AutoLocalize.Get(Label));
		yield return null;
	}
}
