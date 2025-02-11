using UnityEngine;

public class BriefingUpdateText : ContainerOverride
{
	public string Text;

	public float BlockTime;

	public int NarrationId = -1;

	public int StageId = -1;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		MissionBriefingShowInstuctions missionBriefingShowInstuctions = cont.FindComponentOfType(typeof(MissionBriefingShowInstuctions)) as MissionBriefingShowInstuctions;
		if (missionBriefingShowInstuctions != null)
		{
			missionBriefingShowInstuctions.StringKey = Text;
			missionBriefingShowInstuctions.NarrationId = NarrationId;
			missionBriefingShowInstuctions.BlockInSeconds = BlockTime;
			missionBriefingShowInstuctions.WaitId = StageId;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
	}
}
