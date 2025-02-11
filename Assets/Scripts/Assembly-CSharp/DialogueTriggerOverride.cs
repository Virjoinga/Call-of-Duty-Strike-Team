using UnityEngine;

public class DialogueTriggerOverride : ContainerOverride
{
	public DialogueRequestData m_OverrideData = new DialogueRequestData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		DialogueTrigger dialogueTrigger = cont.FindComponentOfType(typeof(DialogueTrigger)) as DialogueTrigger;
		if (dialogueTrigger != null)
		{
			if (m_OverrideData == null)
			{
				Debug.Log("the override is null");
			}
			dialogueTrigger.m_Interface = m_OverrideData;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		DialogueTrigger componentInChildren = gameObj.GetComponentInChildren<DialogueTrigger>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
