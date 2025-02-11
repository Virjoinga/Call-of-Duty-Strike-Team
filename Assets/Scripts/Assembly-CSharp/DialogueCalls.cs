using UnityEngine;

public class DialogueCalls : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void TruckDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_TI_DIALOGUE_TRUCK");
	}

	public void TerminalDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_TI_DIALOGUE_TERMINAL");
	}

	public void GateDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_TI_DIALOGUE_GATEG");
	}

	public void PartsDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_TI_DIALOGUE_PARTS");
	}

	public void ComsDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_TI_DIALOGUE_COMS");
	}

	public void HackDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_TI_DIALOGUE_HACK");
	}

	public void SupportDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_TI_DIALOGUE_REIN");
	}
}
