using System.Collections;

public class HUDClearHeldMessageCommand : Command
{
	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		if (HUDMessenger.Instance != null)
		{
			HUDMessenger.Instance.ClearHeldMessage();
		}
		yield break;
	}
}
