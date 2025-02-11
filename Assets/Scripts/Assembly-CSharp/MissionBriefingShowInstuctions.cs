using System.Collections;
using UnityEngine;

public class MissionBriefingShowInstuctions : Command
{
	public string StringKey;

	public float BlockInSeconds;

	public int NarrationId;

	public int WaitId;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		MissionBriefingController missionBriefing = MissionBriefingController.Instance;
		float blockTime = 0f;
		int id = ((WaitId != 0 && WaitId != -1) ? WaitId : (missionBriefing.CurrentID + 1));
		while (id > missionBriefing.WaitID)
		{
			yield return new WaitForEndOfFrame();
		}
		if (missionBriefing != null && missionBriefing.UpdateInstructions(StringKey, NarrationId, id, false))
		{
			blockTime = BlockInSeconds;
		}
		yield return new WaitForSeconds(blockTime);
		if (id == missionBriefing.WaitID)
		{
			yield return new WaitForSeconds(1f);
			missionBriefing.NextInstructionInSequence();
		}
	}
}
