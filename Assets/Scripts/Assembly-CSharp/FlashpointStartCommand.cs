using System.Collections;
using UnityEngine;

public class FlashpointStartCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		DoWarningDialogue();
		switch (MissionListings.Instance.CurrentFlashPointData.CurrentObjective)
		{
		case MissionListings.FlashpointData.Objective.Clear:
			HUDMessenger.Instance.PushMessage(Language.Get("S_FL_CLEAR_MSG_START_01"), string.Empty, string.Empty, false);
			break;
		case MissionListings.FlashpointData.Objective.Survive:
			HUDMessenger.Instance.PushMessage(Language.Get("S_FL_SURVIVE_MSG_START_01"), string.Empty, Language.Get("S_FL_SURVIVE_MSG_START_02"), false);
			break;
		case MissionListings.FlashpointData.Objective.Collect:
			HUDMessenger.Instance.PushMessage(Language.Get("S_FL_COLLECT_MSG_START_01"), string.Empty, Language.Get("S_FL_COLLECT_MSG_START_02"), false);
			break;
		case MissionListings.FlashpointData.Objective.Destroy:
			HUDMessenger.Instance.PushMessage(Language.Get("S_FL_DESTROY_MSG_START_01"), string.Empty, Language.Get("S_FL_DESTROY_MSG_START_02"), false);
			break;
		}
		GMGSFX.Instance.NextWave.Play2D();
		GameObject mh2 = null;
		mh2 = GameObject.Find("Music_Danger");
		if ((bool)mh2)
		{
			Container.SendMessage(mh2, "Activate");
		}
		yield break;
	}

	private void DoWarningDialogue()
	{
	}
}
