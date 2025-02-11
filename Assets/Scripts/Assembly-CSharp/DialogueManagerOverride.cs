using UnityEngine;

public class DialogueManagerOverride : ContainerOverride
{
	public DialogueListData m_OverrideData = new DialogueListData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		DialogueManager dialogueManager = cont.FindComponentOfType(typeof(DialogueManager)) as DialogueManager;
		if (!(dialogueManager != null))
		{
			return;
		}
		if (m_OverrideData == null)
		{
			Debug.Log("the override is null");
		}
		MissionSetup.LoadAndCopySectionSFX(MissionListings.eMissionID.MI_MAX, -1);
		Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(DialogueTrigger));
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			DialogueTrigger dialogueTrigger = (DialogueTrigger)array2[i];
			if (dialogueTrigger.m_Interface.Text != string.Empty)
			{
				AutoPopulateForText(dialogueTrigger.m_Interface.Text);
			}
			if (dialogueTrigger.m_Interface.Queue == null)
			{
				continue;
			}
			for (int j = 0; j < dialogueTrigger.m_Interface.Queue.Length; j++)
			{
				if (dialogueTrigger.m_Interface.Queue[j].Text != string.Empty)
				{
					AutoPopulateForText(dialogueTrigger.m_Interface.Queue[j].Text);
				}
			}
		}
		Object[] array3 = IncludeDisabled.FindSceneObjectsOfType(typeof(UnitsInVolumeObjective));
		Object[] array4 = array3;
		for (int k = 0; k < array4.Length; k++)
		{
			UnitsInVolumeObjective unitsInVolumeObjective = (UnitsInVolumeObjective)array4[k];
			if (unitsInVolumeObjective.m_UnitsVolInterface.PromptIfNotRequiredNumber != string.Empty)
			{
				AutoPopulateForText(unitsInVolumeObjective.m_UnitsVolInterface.PromptIfNotRequiredNumber);
			}
		}
		AutoAddTwoManMantle();
		dialogueManager.m_Interface = m_OverrideData;
	}

	private void AutoPopulateForText(string text)
	{
		bool flag = false;
		DialogueData dialogueData = null;
		foreach (DialogueData dialogue in m_OverrideData.DialogueList)
		{
			if (dialogue.Text == text)
			{
				flag = true;
				dialogueData = dialogue;
				break;
			}
		}
		if (!flag)
		{
			DialogueData dialogueData2 = new DialogueData();
			dialogueData2.Text = text;
			GameDialogueSFX.Instance.SearchForIds(dialogueData2.Text, out dialogueData2.SfxId, out dialogueData2.FirstRepeatSfxId, out dialogueData2.SubsequentRepeatSfxId);
			m_OverrideData.DialogueList.Add(dialogueData2);
		}
		else if (GameDialogueSFX.HasInstance)
		{
			GameDialogueSFX.Instance.SearchForIds(dialogueData.Text, out dialogueData.SfxId, out dialogueData.FirstRepeatSfxId, out dialogueData.SubsequentRepeatSfxId);
		}
	}

	private void AutoAddTwoManMantle()
	{
		for (int i = 1; i < 8; i++)
		{
			DialogueData dialogueData = new DialogueData();
			dialogueData.Text = "S_PICKUP_TWOMAN_PROMPT_0" + i;
			if (!DialogueExists(dialogueData.Text))
			{
				GameDialogueSFX.Instance.SearchForIds(dialogueData.Text, out dialogueData.SfxId, out dialogueData.FirstRepeatSfxId, out dialogueData.SubsequentRepeatSfxId);
				m_OverrideData.DialogueList.Add(dialogueData);
			}
		}
	}

	private bool DialogueExists(string text)
	{
		foreach (DialogueData dialogue in m_OverrideData.DialogueList)
		{
			if (dialogue.Text == text)
			{
				return true;
			}
		}
		return false;
	}

	public override void SendOverrideMessageWithParam(GameObject gameObj, string methodName, string param)
	{
		base.SendOverrideMessage(gameObj, methodName);
		DialogueManager componentInChildren = gameObj.GetComponentInChildren<DialogueManager>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName, param);
		}
	}
}
