using UnityEngine;

public class ContextMessagePopup : SelectableObject
{
	public string Title;

	public string Message;

	public MessageBox MessageBoxPrefab;

	public bool TriggerOnlyOnce = true;

	public Vector2 MinSize = new Vector2(5f, 5f);

	public bool AutoSize = true;

	private bool mHasTriggered;

	public bool CanDisplay
	{
		get
		{
			if (mHasTriggered && TriggerOnlyOnce)
			{
				return false;
			}
			return true;
		}
	}

	public override int OnSelected(Vector2 selectedScreenPos, bool fromTap)
	{
		if (TriggerOnlyOnce && mHasTriggered)
		{
			return 0;
		}
		mHasTriggered = true;
		StartCoroutine(GameController.Instance.DisplayInGameMessageBoxHelper(MessageBoxPrefab, Title, Message, MinSize, AutoSize));
		return 0;
	}
}
