using UnityEngine;

public class MissionBriefingOverride : ContainerOverride
{
	public ScriptedSequenceData m_OverrideData = new ScriptedSequenceData();

	public GameObject EntireLevelRenderObject;

	private void Awake()
	{
		if (MissionBriefingController.Instance != null)
		{
			MissionBriefingController.Instance.EntireLevelModel = EntireLevelRenderObject;
		}
	}

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		ScriptedSequence scriptedSequence = cont.FindComponentOfType(typeof(ScriptedSequence)) as ScriptedSequence;
		if (scriptedSequence != null)
		{
			scriptedSequence.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(scriptedSequence);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
	}
}
