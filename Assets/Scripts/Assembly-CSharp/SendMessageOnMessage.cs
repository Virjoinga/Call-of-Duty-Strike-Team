using UnityEngine;

public class SendMessageOnMessage : MonoBehaviour
{
	public GroupMessageData m_Interface;

	private void Activate()
	{
		if (m_Interface.targetObjs != null)
		{
			int num = 0;
			foreach (GameObject targetObj in m_Interface.targetObjs)
			{
				string text = string.Empty;
				string message = string.Empty;
				GameObject gameObject = null;
				if (m_Interface.Message != null && num < m_Interface.Message.Count)
				{
					message = m_Interface.Message[num];
				}
				if (m_Interface.StringParam != null && num < m_Interface.StringParam.Count)
				{
					text = m_Interface.StringParam[num];
				}
				if (m_Interface.ObjectParam != null && num < m_Interface.ObjectParam.Count)
				{
					gameObject = m_Interface.ObjectParam[num];
				}
				if (text != string.Empty)
				{
					Container.SendMessageWithParam(targetObj, message, text, base.gameObject);
				}
				else if (gameObject != null)
				{
					Container.SendMessageWithParam(targetObj, message, gameObject, base.gameObject);
				}
				else
				{
					Container.SendMessage(targetObj, message, base.gameObject);
				}
				num++;
			}
		}
		if (m_Interface.OneShot)
		{
			Object.Destroy(this);
		}
	}
}
