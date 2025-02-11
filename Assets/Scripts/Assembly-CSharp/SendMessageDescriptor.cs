using System.Collections.Generic;
using UnityEngine;

public class SendMessageDescriptor : TaskDescriptor
{
	public GameObject ObjectToMessage;

	public string FunctionToCall = string.Empty;

	public GameObject GameObjectParam;

	public List<GameObject> GroupObjectToCall;

	public List<string> GroupFunctionToCall;

	public List<GameObject> GroupGameObjectParam;

	public override Task CreateTask(TaskManager owner, TaskManager.Priority priority, Task.Config flags)
	{
		if (GameObjectParam != null)
		{
			Container.SendMessageWithParam(ObjectToMessage, FunctionToCall, GameObjectParam, base.gameObject);
			if (GroupObjectToCall != null && GroupObjectToCall.Count > 0)
			{
				int num = 0;
				foreach (GameObject item in GroupObjectToCall)
				{
					GameObject sender = GameObjectParam;
					string message = string.Empty;
					if (GroupFunctionToCall != null && num < GroupFunctionToCall.Count)
					{
						message = GroupFunctionToCall[num];
					}
					if (GroupGameObjectParam != null && num < GroupGameObjectParam.Count)
					{
						sender = GroupGameObjectParam[num];
					}
					Container.SendMessageWithParam(item, message, base.gameObject, sender);
					num++;
				}
			}
		}
		else
		{
			Container.SendMessage(ObjectToMessage, FunctionToCall, base.gameObject);
			if (GroupObjectToCall != null && GroupObjectToCall.Count > 0)
			{
				int num2 = 0;
				foreach (GameObject item2 in GroupObjectToCall)
				{
					string message2 = string.Empty;
					if (GroupFunctionToCall != null && num2 < GroupFunctionToCall.Count)
					{
						message2 = GroupFunctionToCall[num2];
					}
					Container.SendMessage(item2, message2, base.gameObject);
					num2++;
				}
			}
		}
		return null;
	}
}
