using System.Collections;

public class DialogueAddToTopOfQueueCommand : Command
{
	public string Dialogue;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToTopOfQueue(Dialogue);
		yield break;
	}
}
