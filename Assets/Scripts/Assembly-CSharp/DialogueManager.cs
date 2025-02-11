using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	private const float TIME_BEFORE_DIALOGUE_REPEAT = 40f;

	public DialogueListData m_Interface = new DialogueListData();

	private DialogueData mRepeatData;

	private float mTime;

	private int mRepeats;

	private void Start()
	{
		mRepeatData = null;
	}

	private void Update()
	{
		if (mRepeatData == null)
		{
			return;
		}
		mTime += Time.deltaTime;
		if (mTime >= 40f)
		{
			string text = mRepeatData.Text;
			int sfxId = mRepeatData.SfxId;
			mTime = 0f;
			if (mRepeats == 0 || (mRepeatData.SubsequentRepeatSfxId == -1 && mRepeats % 2 == 0))
			{
				text += "_FirstRepeat";
				sfxId = mRepeatData.FirstRepeatSfxId;
			}
			else if (mRepeatData.SubsequentRepeatSfxId != -1)
			{
				text += "_SubsequentRepeat";
				sfxId = mRepeatData.SubsequentRepeatSfxId;
			}
			CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(text, sfxId);
			mRepeats++;
		}
	}

	public void PlayDialogue(string imp)
	{
		foreach (DialogueData dialogue in m_Interface.DialogueList)
		{
			if (dialogue.Text == imp)
			{
				CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(imp, dialogue.SfxId);
				break;
			}
		}
	}

	public void StartRepeatingDialogue(string imp)
	{
		if (mRepeatData == null)
		{
			foreach (DialogueData dialogue in m_Interface.DialogueList)
			{
				if (dialogue.Text == imp)
				{
					CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(imp, dialogue.SfxId);
					mRepeatData = dialogue;
					mTime = 0f;
					mRepeats = 0;
					break;
				}
			}
			return;
		}
		Debug.LogWarning("Already repeating dialogue - only supports repeating one line of dialogue");
	}

	public void FinishRepeatingDialogue(string imp)
	{
		if (mRepeatData != null && mRepeatData.Text == imp)
		{
			mRepeatData = null;
		}
		else
		{
			Debug.LogWarning("Wasn't repeating this dialogue - so cannot finish");
		}
	}

	public void ClearDialogueQueue(bool stopCurrent)
	{
		CommonHudController.Instance.MissionDialogueQueue.ClearDialogueQueue(stopCurrent);
	}
}
