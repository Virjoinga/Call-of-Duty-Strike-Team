using System.Collections.Generic;
using UnityEngine;

public class DialogueQueue : MonoBehaviour
{
	private enum DialogueState
	{
		Off = 0,
		FadeOn = 1,
		On = 2,
		FadeOff = 3
	}

	public MissionBriefingInstructionsPanel InstructionsPanelPrefab;

	private MissionBriefingInstructionsPanel mInstructionsPanel;

	private GameObject mScriptToCallOnQueueFinished;

	private string mMethodToCallOnQueueFinished;

	private MissionListings.eMissionID mMissionId;

	private List<DialogueEntry> mQueue = new List<DialogueEntry>();

	private float DefaultDisplayTime = 5f;

	private float MinimumDisplayTime = 1f;

	private float PostVoFadeOff = 0.5f;

	private float FadeInDuration = 0.1f;

	private float DisplayDuration = 5f;

	private float FadeOutDuration = 0.2f;

	private DialogueState mState;

	private float mStateTimer;

	public void SetOnQueueFinishedCallback(GameObject script, string method)
	{
		mScriptToCallOnQueueFinished = script;
		mMethodToCallOnQueueFinished = method;
	}

	private void Start()
	{
		CreateBriefingPanel();
		mState = DialogueState.Off;
		mStateTimer = 0f;
		mMissionId = ActStructure.Instance.CurrentMissionID;
	}

	private void CreateBriefingPanel()
	{
		mInstructionsPanel = (MissionBriefingInstructionsPanel)Object.Instantiate(InstructionsPanelPrefab, new Vector3(0f, 200f, 0f), Quaternion.identity);
		mInstructionsPanel.PositionType = MissionBriefingInstructionsPanel.Type.Centre;
		mInstructionsPanel.Messenger = DeliveredBy.None;
		mInstructionsPanel.transform.parent = base.transform;
		mInstructionsPanel.ActivateOnStart = false;
		mInstructionsPanel.DestroyOnDeactivate = false;
	}

	private void Update()
	{
		mStateTimer += Time.deltaTime;
		switch (mState)
		{
		case DialogueState.Off:
			if (!IsDialogueQueueEmpty())
			{
				mInstructionsPanel.SetInstructions(mQueue[0].GetDialogue());
				mInstructionsPanel.Activate();
				int gameDialogueSFXId = mQueue[0].GetGameDialogueSFXId();
				if (gameDialogueSFXId != -1)
				{
					DisplayDuration = GameDialogueSFX.Instance.PlayDialogue(mMissionId, gameDialogueSFXId) + PostVoFadeOff;
					DisplayDuration = Mathf.Max(MinimumDisplayTime, DisplayDuration);
				}
				else
				{
					DisplayDuration = DefaultDisplayTime;
				}
				mStateTimer = 0f;
				mState = DialogueState.FadeOn;
				mQueue.RemoveAt(0);
			}
			break;
		case DialogueState.FadeOn:
			if (mStateTimer >= FadeInDuration)
			{
				mStateTimer = 0f;
				mState = DialogueState.On;
			}
			break;
		case DialogueState.On:
			if (!(mStateTimer >= DisplayDuration))
			{
				break;
			}
			mStateTimer = 0f;
			if (!IsDialogueQueueEmpty())
			{
				mInstructionsPanel.SetInstructions(mQueue[0].GetDialogue());
				int gameDialogueSFXId2 = mQueue[0].GetGameDialogueSFXId();
				if (gameDialogueSFXId2 != -1)
				{
					DisplayDuration = GameDialogueSFX.Instance.PlayDialogue(mMissionId, gameDialogueSFXId2) + PostVoFadeOff;
					DisplayDuration = Mathf.Max(MinimumDisplayTime, DisplayDuration);
				}
				else
				{
					DisplayDuration = DefaultDisplayTime;
				}
				mQueue.RemoveAt(0);
			}
			else
			{
				mState = DialogueState.FadeOff;
				mInstructionsPanel.DelayedDeactivate(FadeOutDuration);
			}
			break;
		case DialogueState.FadeOff:
			if (mStateTimer >= FadeOutDuration)
			{
				mInstructionsPanel.ClearInstructions();
				Color color = default(Color);
				color.a = 0f;
				mStateTimer = 0f;
				mState = DialogueState.Off;
				if (mScriptToCallOnQueueFinished != null && mMethodToCallOnQueueFinished != null && mMethodToCallOnQueueFinished != string.Empty)
				{
					Container.SendMessage(mScriptToCallOnQueueFinished, mMethodToCallOnQueueFinished);
					mScriptToCallOnQueueFinished = null;
					mMethodToCallOnQueueFinished = string.Empty;
				}
			}
			break;
		}
	}

	public void AddDialogueToQueue(string text)
	{
		mQueue.Add(new DialogueEntry(text));
	}

	public void AddDialogueToQueue(string text, int sfxId)
	{
		mQueue.Add(new DialogueEntry(text, sfxId));
	}

	public void AddDialogueToTopOfQueue(string text)
	{
		mQueue.Insert(0, new DialogueEntry(text));
	}

	public void AddDialogueToTopOfQueue(string text, int sfxId)
	{
		mQueue.Insert(0, new DialogueEntry(text, sfxId));
	}

	public bool AlreadyInQueue(string text)
	{
		foreach (DialogueEntry item in mQueue)
		{
			if (item.GetKey() == text)
			{
				return true;
			}
		}
		return false;
	}

	public void ClearDialogueQueue(bool stopCurrent)
	{
		mQueue.Clear();
		if (mInstructionsPanel != null && mInstructionsPanel.gameObject.activeInHierarchy)
		{
			mInstructionsPanel.DelayedDeactivate(FadeOutDuration);
		}
		if (stopCurrent)
		{
			GameDialogueSFX.Instance.StopAllDialogue(mMissionId);
			mState = DialogueState.FadeOff;
		}
	}

	public bool IsDialogueQueueEmpty()
	{
		return mQueue.Count == 0;
	}

	public bool IsDialogueBeingPlayed()
	{
		return mState != DialogueState.FadeOff && mState != DialogueState.Off;
	}
}
