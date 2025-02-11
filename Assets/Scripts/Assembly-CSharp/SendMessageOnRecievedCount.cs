using UnityEngine;

public class SendMessageOnRecievedCount : MonoBehaviour
{
	public RecievedCountData m_Interface;

	private int RecievedCount;

	public int Count
	{
		get
		{
			return RecievedCount;
		}
	}

	public int Target
	{
		get
		{
			return m_Interface.TargetMessageRecievedCount;
		}
	}

	public void Start()
	{
		RecievedCount = 0;
	}

	private void Activate()
	{
		RecievedCount++;
		DebugPrint();
		if (RecievedCount == m_Interface.TargetMessageRecievedCount)
		{
			TargetReached();
		}
	}

	private void Deactivate()
	{
		if (RecievedCount > 0)
		{
			RecievedCount--;
		}
		DebugPrint();
	}

	private void TargetReached()
	{
		if (m_Interface.OptionalStringParam != string.Empty || m_Interface.GroupStringParam.Count > 0 || m_Interface.OptionalObjectParam != null || m_Interface.GroupObjectParam.Count > 0)
		{
			if (m_Interface.OptionalStringParam != string.Empty)
			{
				Container.SendMessageWithParam(m_Interface.ObjectToMessage, m_Interface.OptionalMessage, m_Interface.OptionalStringParam, base.gameObject);
			}
			else
			{
				Container.SendMessageWithParam(m_Interface.ObjectToMessage, m_Interface.OptionalMessage, m_Interface.OptionalObjectParam, base.gameObject);
			}
		}
		else
		{
			Container.SendMessage(m_Interface.ObjectToMessage, m_Interface.OptionalMessage, base.gameObject);
		}
		if (m_Interface.GroupObjectToCall != null && m_Interface.GroupObjectToCall.Count > 0)
		{
			int num = 0;
			foreach (GameObject item in m_Interface.GroupObjectToCall)
			{
				string text = string.Empty;
				string message = string.Empty;
				GameObject param = null;
				if (m_Interface.GroupFunctionToCall != null && num < m_Interface.GroupFunctionToCall.Count)
				{
					message = m_Interface.GroupFunctionToCall[num];
				}
				if (m_Interface.GroupStringParam != null && num < m_Interface.GroupStringParam.Count)
				{
					text = m_Interface.GroupStringParam[num];
				}
				if (m_Interface.GroupObjectParam != null && num < m_Interface.GroupObjectParam.Count)
				{
					param = m_Interface.GroupObjectParam[num];
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
		if (m_Interface.ResetOnFinish)
		{
			RecievedCount = 0;
		}
		if (m_Interface.DestroyOnFinish)
		{
			Object.Destroy(this);
		}
	}

	public void ChangeTarget(int target)
	{
		RecievedCount = 0;
		m_Interface.TargetMessageRecievedCount = target;
		DebugPrint();
	}

	private void DebugPrint()
	{
	}
}
