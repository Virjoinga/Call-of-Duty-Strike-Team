using System.Collections;

public class HUDMessageCommand : Command
{
	public string Message;

	public bool Localised;

	public bool HoldMessageUntilCleared;

	public bool PriorityMessage;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (HUDMessenger.Instance != null)
		{
			string strToDisplay = ((!Localised) ? Message : Language.Get(Message));
			if (PriorityMessage)
			{
				HUDMessenger.Instance.PushPriorityMessage(strToDisplay, HoldMessageUntilCleared);
			}
			else
			{
				HUDMessenger.Instance.PushMessage(strToDisplay, HoldMessageUntilCleared);
			}
		}
		yield break;
	}
}
