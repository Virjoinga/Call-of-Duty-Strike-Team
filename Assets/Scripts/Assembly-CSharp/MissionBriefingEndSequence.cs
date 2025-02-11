using System.Collections;
using UnityEngine;

public class MissionBriefingEndSequence : Command
{
	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		MissionBriefingController missionBriefing = MissionBriefingController.Instance;
		int id = missionBriefing.CurrentID;
		while (id == missionBriefing.WaitID)
		{
			yield return new WaitForEndOfFrame();
		}
		if (missionBriefing != null)
		{
			missionBriefing.EndSequence();
		}
		yield return new WaitForEndOfFrame();
	}
}
