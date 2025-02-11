using System;

[Serializable]
public class DialogueData
{
	public string Text;

	public int SfxId = -1;

	public int FirstRepeatSfxId = -1;

	public int SubsequentRepeatSfxId = -1;
}
