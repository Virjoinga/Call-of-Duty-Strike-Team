using System.Collections.Generic;
using UnityEngine;

public class MessageOnReceivedMessage_TS : MonoBehaviour
{
	public string WaitForMessage;

	public List<GameObject> GroupObjectToCall;

	public List<string> GroupFunctionToCall;

	public List<string> GroupStringParam;

	public List<GameObject> GroupObjectParam;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Restart()
	{
		if (GroupObjectToCall == null || GroupObjectToCall.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in GroupObjectToCall)
		{
			string text = string.Empty;
			string message = string.Empty;
			GameObject param = null;
			if (GroupFunctionToCall != null && num < GroupFunctionToCall.Count)
			{
				message = GroupFunctionToCall[num];
			}
			if (GroupStringParam != null && num < GroupStringParam.Count)
			{
				text = GroupStringParam[num];
			}
			if (GroupObjectParam != null && num < GroupObjectParam.Count)
			{
				param = GroupObjectParam[num];
			}
			if (text != string.Empty)
			{
				Container.SendMessageWithParam(item, message, text, base.gameObject);
			}
			else
			{
				Container.SendMessageWithParam(item, message, param, base.gameObject);
			}
			num++;
		}
	}
}
