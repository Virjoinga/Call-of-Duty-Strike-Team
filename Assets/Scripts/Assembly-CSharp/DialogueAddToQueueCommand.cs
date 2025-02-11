using System.Collections;

public class DialogueAddToQueueCommand : Command
{
	public string Dialogue;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(Dialogue);
		yield break;
	}
}
