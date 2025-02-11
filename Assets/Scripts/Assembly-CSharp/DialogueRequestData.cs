using System;
using UnityEngine;

[Serializable]
public class DialogueRequestData
{
	public string Text;

	public DialogueRequestQueueData[] Queue;

	public int RepeatUntilActivateCallsReceived = 1;

	public bool RepeatBehaviour;

	public bool UseTrigger = true;

	public bool OneShot = true;

	public GameObject OnQueueEmptyCallbackScript;

	public string OnQueueEmptyCallbackMethod;
}
