using UnityEngine;

public class DelayedMessage : MonoBehaviour
{
	public DelayedMessageData m_Interface;

	private float ReqDelay = 2f;

	private bool isRunning;

	private void Start()
	{
		isRunning = false;
		ReqDelay = m_Interface.TimeDelay;
	}

	private void Update()
	{
		if (!isRunning)
		{
			return;
		}
		ReqDelay -= Time.deltaTime;
		if (!(ReqDelay <= 0f))
		{
			return;
		}
		if (m_Interface.StringParam != string.Empty || m_Interface.GroupStringParam.Count > 0 || m_Interface.ObjectParam != null || m_Interface.GroupObjectParam.Count > 0)
		{
			if (m_Interface.StringParam != string.Empty)
			{
				Container.SendMessageWithParam(m_Interface.Target, m_Interface.Message, m_Interface.StringParam, base.gameObject);
			}
			else
			{
				Container.SendMessageWithParam(m_Interface.Target, m_Interface.Message, m_Interface.ObjectParam, base.gameObject);
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
		}
		else
		{
			Container.SendMessage(m_Interface.Target, m_Interface.Message, base.gameObject);
			if (m_Interface.GroupObjectToCall != null && m_Interface.GroupObjectToCall.Count > 0)
			{
				int num2 = 0;
				foreach (GameObject item2 in m_Interface.GroupObjectToCall)
				{
					string message2 = string.Empty;
					if (m_Interface.GroupFunctionToCall != null && num2 < m_Interface.GroupFunctionToCall.Count)
					{
						message2 = m_Interface.GroupFunctionToCall[num2];
					}
					Container.SendMessage(item2, message2, base.gameObject);
					num2++;
				}
			}
		}
		isRunning = false;
		if (m_Interface.LoopTimer)
		{
			ReqDelay = m_Interface.TimeDelay;
			isRunning = true;
		}
		else if (m_Interface.DestroyOnTrigger)
		{
			Object.Destroy(this);
		}
	}

	private void Activate()
	{
		ReqDelay = m_Interface.TimeDelay;
		isRunning = true;
	}

	public void Deactivate()
	{
		if (m_Interface.LoopTimer)
		{
			isRunning = false;
		}
		else if (!m_Interface.DestroyOnTrigger)
		{
			Object.Destroy(this);
		}
	}
}
