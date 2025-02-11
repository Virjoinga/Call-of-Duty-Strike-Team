using System.Collections;
using UnityEngine;

public class Pakistan1_DialogueManager : MonoBehaviour
{
	public void IntroDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_INTRO");
	}

	public void KeepMovingDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_ MOVING");
	}

	public void TankDialogue()
	{
		StartCoroutine("tankDelay");
	}

	private IEnumerator tankDelay()
	{
		yield return new WaitForSeconds(2f);
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_TANK1");
	}

	public void Tank2Dialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_TANK2");
	}

	public void BreachDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_BREACH");
	}

	public void StairsDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_STAIRS");
	}

	public void Market1Dialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_MARKET1");
	}

	public void Market2Dialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_MARKET2");
	}

	public void Market3Dialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_MARKET3");
	}

	public void TurretDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_TURRET");
	}

	public void SnipersDialogue()
	{
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue("S_P1_DIALOGUE_SNIPERS");
	}
}
