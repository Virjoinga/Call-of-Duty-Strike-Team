using System.Collections;
using UnityEngine;

public class MissionBriefingBeginSequence : Command
{
	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		MissionBriefingController missionBriefing = MissionBriefingController.Instance;
		if (missionBriefing != null)
		{
			missionBriefing.BeginSequence();
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForEndOfFrame();
	}
}
