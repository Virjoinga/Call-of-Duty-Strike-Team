using System.Collections.Generic;
using UnityEngine;

public class HUDMessenger : MonoBehaviour
{
	private enum eSubState
	{
		Idle = 0,
		AnimateIn = 1,
		Display = 2,
		DisplayTransitionOut = 3,
		AnimateOut = 4,
		PostMessageDelay = 5
	}

	private class MessageItem
	{
		public string strTitleMessage;

		public string strMessage;

		public string strSubMessage;

		public bool bHeld;
	}

	public static string HUDEmptyString = " ";

	private static HUDMessenger s_Instance;

	public float AnimateInTime = 0.5f;

	public float DisplayTime = 1f;

	public float AnimateOutTime = 0.5f;

	public GameObject GMGPositionObject;

	public GameObject SinglePlayerPositionObject;

	public float PostMessageDelay = 0.5f;

	private List<MessageItem> m_MessageQueue = new List<MessageItem>();

	private eSubState m_SubState;

	private SpriteText m_TextItem;

	private SearchTypeLabel m_TypeLabel;

	public GameObject m_TitleBackgroundItem;

	public SpriteText m_CampaignTextItem;

	private SearchTypeLabel m_CampaignTypeLabel;

	public SpriteText m_TitleTextItem;

	private SearchTypeLabel m_TitleTypeLabel;

	public SpriteText m_SubTextItem;

	private SearchTypeLabel m_SubTypeLabel;

	private bool m_bRunning;

	private float m_fTimer;

	private SearchTypeLabel ActiveLabel;

	private SpriteText ActiveText;

	public static HUDMessenger Instance
	{
		get
		{
			return s_Instance;
		}
	}

	private void SetLabelsBlank(bool bProcess)
	{
		m_TextItem.Text = HUDEmptyString;
		m_TitleTextItem.Text = HUDEmptyString;
		m_SubTextItem.Text = HUDEmptyString;
		m_CampaignTextItem.Text = HUDEmptyString;
		m_TitleBackgroundItem.SetActive(false);
		if (bProcess)
		{
			ProcessLabels();
		}
	}

	private void ProcessLabels()
	{
		m_TypeLabel.ResetAndProcess();
		m_TitleTypeLabel.ResetAndProcess();
		m_SubTypeLabel.ResetAndProcess();
		m_CampaignTypeLabel.ResetAndProcess();
	}

	private void Awake()
	{
		if (s_Instance == null)
		{
			s_Instance = this;
			m_TextItem = GetComponent<SpriteText>();
			if (m_TextItem == null)
			{
				Debug.Break();
			}
			m_TypeLabel = GetComponent<SearchTypeLabel>();
			if (m_TypeLabel == null)
			{
				Debug.Break();
			}
			m_TitleTypeLabel = m_TitleTextItem.GetComponent<SearchTypeLabel>();
			if (m_TitleTypeLabel == null)
			{
				Debug.Break();
			}
			m_SubTypeLabel = m_SubTextItem.GetComponent<SearchTypeLabel>();
			if (m_SubTypeLabel == null)
			{
				Debug.Break();
			}
			m_CampaignTypeLabel = m_CampaignTextItem.GetComponent<SearchTypeLabel>();
			if (m_CampaignTypeLabel == null)
			{
				Debug.Break();
			}
			if (TBFUtils.IsRetinaHdDevice())
			{
				m_TextItem.maxWidth *= 2f;
				m_CampaignTextItem.maxWidth *= 2f;
			}
			SetLabelsBlank(false);
			StartMessenger();
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void OnDestroy()
	{
		s_Instance = null;
	}

	private void LateUpdate()
	{
		switch (m_SubState)
		{
		case eSubState.Idle:
			if (m_MessageQueue.Count > 0)
			{
				m_SubState = eSubState.AnimateIn;
				m_TitleTextItem.Text = m_MessageQueue[0].strTitleMessage;
				m_TitleBackgroundItem.SetActive(m_MessageQueue[0].strTitleMessage != HUDEmptyString);
				m_SubTextItem.Text = m_MessageQueue[0].strSubMessage;
				m_fTimer = ((!m_MessageQueue[0].bHeld) ? DisplayTime : float.MaxValue);
				ActiveText.Text = m_MessageQueue[0].strMessage;
				ActiveLabel.ResetAndProcess();
			}
			break;
		case eSubState.AnimateIn:
			if (!ActiveLabel.IsChangingText)
			{
				m_SubState = eSubState.Display;
			}
			break;
		case eSubState.Display:
			m_fTimer -= Time.deltaTime;
			if (m_fTimer < 0f)
			{
				m_SubState = eSubState.DisplayTransitionOut;
				SetLabelsBlank(true);
			}
			break;
		case eSubState.DisplayTransitionOut:
			m_SubState = eSubState.AnimateOut;
			break;
		case eSubState.AnimateOut:
			if (!ActiveLabel.IsChangingText)
			{
				m_SubState = eSubState.PostMessageDelay;
				m_fTimer = PostMessageDelay;
			}
			break;
		case eSubState.PostMessageDelay:
			m_fTimer -= Time.deltaTime;
			if (m_fTimer <= 0f)
			{
				if (m_MessageQueue.Count > 0)
				{
					m_MessageQueue.RemoveAt(0);
				}
				m_SubState = eSubState.Idle;
			}
			break;
		}
	}

	public void PushMessage(string strMessage, bool bHeld)
	{
		PushMessage(HUDEmptyString, strMessage, HUDEmptyString, bHeld);
	}

	public void PushMessage(string strTitleMessage, string strMessage, string strSubMessage, bool bHeld)
	{
		MessageItem messageItem = new MessageItem();
		messageItem.bHeld = bHeld;
		messageItem.strMessage = strMessage;
		messageItem.strTitleMessage = strTitleMessage;
		messageItem.strSubMessage = strSubMessage;
		m_MessageQueue.Add(messageItem);
		SetupPosition();
	}

	public void PushPriorityMessage(string strMessage, bool bHeld)
	{
		PushPriorityMessage(HUDEmptyString, strMessage, HUDEmptyString, bHeld);
	}

	public void PushPriorityMessage(string strTitleMessage, string strMessage, string strSubMessage, bool bHeld)
	{
		ClearAllMessages(false);
		MessageItem messageItem = new MessageItem();
		messageItem.bHeld = bHeld;
		messageItem.strMessage = strMessage;
		messageItem.strTitleMessage = strTitleMessage;
		messageItem.strSubMessage = strSubMessage;
		m_MessageQueue.Add(messageItem);
		SetupPosition();
	}

	public void ClearAllMessages()
	{
		ClearAllMessages(true);
	}

	public void HideActiveMessages(bool hide)
	{
		m_TextItem.Hide(hide);
		m_TitleTextItem.Hide(hide);
		m_SubTextItem.Hide(hide);
		m_CampaignTextItem.Hide(hide);
	}

	public void ClearAllMessages(bool bClearOutExistingMessages)
	{
		if (m_MessageQueue.Count > 0)
		{
			if (base.gameObject.activeInHierarchy)
			{
				StopMessenger();
			}
			if (bClearOutExistingMessages)
			{
				ProcessLabels();
			}
			m_MessageQueue.Clear();
			m_SubState = eSubState.Idle;
			if (base.gameObject.activeInHierarchy)
			{
				StartMessenger();
			}
		}
	}

	public void ClearHeldMessage()
	{
		Debug.Log("HUDMessenger: Held Message Cleared");
		m_fTimer = 0f;
		for (int i = 0; i < m_MessageQueue.Count; i++)
		{
			m_MessageQueue[i].bHeld = false;
		}
	}

	public void StartMessenger()
	{
		if (!m_bRunning)
		{
			SetupPosition();
			m_fTimer = 0f;
			m_SubState = eSubState.Idle;
			m_bRunning = true;
		}
	}

	public void StopMessenger()
	{
		m_bRunning = false;
	}

	public void SetupPosition()
	{
		if (ActStructure.Instance.CurrentMissionIsSpecOps())
		{
			if (GMGPositionObject != null)
			{
				base.gameObject.transform.position = GMGPositionObject.transform.position;
				ActiveLabel = m_TypeLabel;
				ActiveText = m_TextItem;
			}
		}
		else if (SinglePlayerPositionObject != null)
		{
			base.gameObject.transform.position = SinglePlayerPositionObject.transform.position;
			ActiveLabel = m_CampaignTypeLabel;
			ActiveText = m_CampaignTextItem;
		}
		HideActiveMessages(false);
	}
}
