using System.Collections;

public class DialogueClearQueueCommand : Command
{
	public string Dialogue;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CommonHudController.Instance.MissionDialogueQueue.ClearDialogueQueue(false);
		yield break;
	}
}
