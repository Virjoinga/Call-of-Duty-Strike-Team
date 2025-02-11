public struct DialogueEntry
{
	private string mDialogueKey;

	private int mDialogueId;

	public DialogueEntry(string textKey)
	{
		mDialogueKey = textKey;
		mDialogueId = -1;
	}

	public DialogueEntry(string textKey, int dialogueSfxId)
	{
		mDialogueKey = textKey;
		mDialogueId = dialogueSfxId;
	}

	public string GetDialogue()
	{
		return AutoLocalize.Get(mDialogueKey);
	}

	public string GetKey()
	{
		return mDialogueKey;
	}

	public int GetGameDialogueSFXId()
	{
		return mDialogueId;
	}
}
