using System;
using UnityEngine;

public class MessageBoxResults
{
	public enum Result
	{
		Unknown = 0,
		OK = 1,
		Cancel = 2,
		Facebook = 3,
		Twitter = 4,
		LargeOK = 5,
		Share = 6
	}

	[Serializable]
	public class MessageBoxResultData
	{
		public Result Result = Result.OK;

		public MonoBehaviour ScriptWithMethodToInvoke;

		public string MethodToInvoke = string.Empty;
	}

	public MessageBoxResultData[] MessageBoxResultsData;

	public void InvokeMethodForResult(Result result)
	{
		MessageBoxResultData[] messageBoxResultsData = MessageBoxResultsData;
		foreach (MessageBoxResultData messageBoxResultData in messageBoxResultsData)
		{
			if (messageBoxResultData.Result == result)
			{
				messageBoxResultData.ScriptWithMethodToInvoke.Invoke(messageBoxResultData.MethodToInvoke, 0f);
			}
		}
	}
}
