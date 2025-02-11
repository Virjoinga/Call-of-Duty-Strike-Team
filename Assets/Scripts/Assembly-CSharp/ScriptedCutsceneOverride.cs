using UnityEngine;

public class ScriptedCutsceneOverride : ContainerOverride
{
	public ScriptedSequenceData m_OverrideData = new ScriptedSequenceData();

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
		ScriptedSequence componentInChildren = gameObj.GetComponentInChildren<ScriptedSequence>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
