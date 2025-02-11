using System.Collections;
using UnityEngine;

public class ProgressionEventCommand : Command
{
	public enum TutorialSections
	{
		FPPCamera = 0,
		FPPStealthKill1 = 1,
		FPPStealthKill2 = 2,
		FPPSniperKill = 3,
		FPPSnapToTarget = 4,
		FPPSwapPlayer = 5,
		FPPKillGuards = 6,
		FPPHackTerminal = 7,
		FPPMinigun = 8,
		TPPCamera = 9,
		TPPPlayerMovement = 10,
		TPPPlayerMovementGhost = 11,
		TPPAimedShot = 12,
		TPPTwoManMantle = 13,
		TPPWindowPeek_INVALID = 14,
		TPPOpenDoor = 15,
		TPPStealthKill = 16,
		TPPGatherIntel = 17
	}

	public enum SectionStages
	{
		SectionStart = 0,
		SectionEnd = 1
	}

	public TutorialSections TutorialSection;

	public SectionStages SectionStage;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		if (SectionStage == SectionStages.SectionStart)
		{
			SwrveEventsProgression.StartTutorialSection(TutorialSection.ToString());
			Debug.Log("Sending Swrve Events Progresstion for the beginning of section " + TutorialSection);
			yield break;
		}
		SwrveEventsProgression.EndTutorialSection(TutorialSection.ToString(), TutorialToggles.RespotCount);
		Debug.Log("Sending Swrve Events Progresstion for the end of section " + TutorialSection.ToString() + ". We failed " + TutorialToggles.RespotCount + " times.");
		TutorialToggles.RespotCount = 0;
	}
}
