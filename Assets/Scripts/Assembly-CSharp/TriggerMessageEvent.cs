using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class TriggerMessageEvent
{
	public enum Type
	{
		PlayView_Entry = 0,
		StrategyView_Entry = 1
	}

	public Type Id;

	public bool HasTriggered;

	public string Title;

	public string Message;

	public MessageBox MessageBoxPrefab;

	public bool TriggerOnlyOnce = true;

	public Vector2 MinSize = new Vector2(5f, 5f);

	public bool AutoSize = true;

	public float Delay;

	private bool mExecuting;

	public bool Execute
	{
		get
		{
			return mExecuting;
		}
		set
		{
			mExecuting = true;
		}
	}

	public TriggerMessageEvent()
	{
		mExecuting = false;
	}

	public void Update(TriggerMessageEventManager manager)
	{
		if (Execute && !HasTriggered)
		{
			Delay -= TimeManager.DeltaTime;
			if (Delay <= 0f)
			{
				HasTriggered = true;
				manager.StartCoroutine(PanAndShowMsgBox(manager));
			}
		}
	}

	private IEnumerator PanAndShowMsgBox(TriggerMessageEventManager manager)
	{
		InputManager.Instance.SetForMessageBox();
		TimeManager.instance.StopTime();
		yield return manager.StartCoroutine(GameController.Instance.DisplayInGameMessageBoxHelper(MessageBoxPrefab, Title, Message, MinSize, AutoSize));
	}
}
