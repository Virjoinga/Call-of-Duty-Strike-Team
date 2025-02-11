using UnityEngine;

public class Kowloon2_DialogueManager : MonoBehaviour
{
	public void ZipLineDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_ZIPLINE");
	}

	public void SetChargesDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_SETTHECHARGES");
	}

	public void GetClearDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_GETCLEAR");
	}

	public void FirstSniperDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_SNIPER");
	}

	public void Unit2InPositionDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_INPOSITION1");
	}

	public void PinnedInHereDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_PINNED");
	}

	public void Unit3and4InPositionDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_COVERED");
	}

	public void DisableCameraDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_CAMERA");
	}

	public void BreachDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("K_II_DIALOGUE_BREACH");
	}
}
