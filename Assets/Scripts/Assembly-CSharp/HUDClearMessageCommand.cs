using System.Collections;

public class HUDClearMessageCommand : Command
{
	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		if (HUDMessenger.Instance != null)
		{
			HUDMessenger.Instance.ClearAllMessages();
		}
		yield break;
	}
}
