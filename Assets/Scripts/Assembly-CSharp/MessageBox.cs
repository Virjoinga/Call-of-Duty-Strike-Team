using System;
using System.Collections;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
	[Serializable]
	public class MessageBoxButtonData
	{
		public string Label;

		public MessageBoxResults.Result Result;
	}

	public enum SpriteAnchorPoint
	{
		Top = 0,
		Left = 1
	}

	protected enum ButtonIcon
	{
		None = -1,
		Facebook = 0,
		Twitter = 1,
		Share = 2,
		ShareAndroid = 3
	}

	private const float BUTTON_BG_X_OFFSET = 16f;

	private const float BUTTON_BG_Y_OFFSET = 18f;

	private const float BUTTON_BG_Z_OFFSET = 1f;

	private const float TEXT_OFFSET = 32f;

	public SpriteText m_titleText;

	public SpriteText m_bodyText;

	public GameObject TextButtonPrefab;

	public GameObject IconButtonPrefab;

	public GameObject LargeTextButtonPrefab;

	public static bool MessageBoxInProgress;

	public MessageBoxButtonData[] ButtonData;

	public SpriteAnchorPoint SpriteAnchor;

	public PackedSprite SpritePrefab;

	public MessageBoxResults Results;

	public int SpriteAnimationFrame;

	protected AnimateCommonBackgroundBox mAnimator;

	protected MessageBoxResults.Result mInternalResult;

	protected UIButton[] m_buttons;

	protected CommonBackgroundBox mBgBox;

	private Transform mVisible;

	private Transform mInvisible;

	private Transform mContents;

	public AnimateCommonBackgroundBox Animator
	{
		get
		{
			return mAnimator;
		}
	}

	public virtual void Awake()
	{
		mAnimator = GetComponentInChildren<AnimateCommonBackgroundBox>();
		mBgBox = GetComponentInChildren<CommonBackgroundBox>();
		mVisible = base.transform.FindChild("Visible");
		mContents = mVisible.FindChild("Content");
		mInvisible = base.transform.FindChild("Invisible");
		UIButton componentInChildren = mInvisible.GetComponentInChildren<UIButton>();
		BoxCollider component = componentInChildren.GetComponent<BoxCollider>();
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		float num2 = (float)Screen.width * num;
		float num3 = (float)Screen.height * num;
		componentInChildren.SetSize(num2, num3);
		component.size = new Vector3(num2, num3, 0f);
		m_titleText.Text = string.Empty;
		m_bodyText.Text = string.Empty;
		MessageBoxInProgress = true;
	}

	private void OnDestroy()
	{
		MessageBoxInProgress = false;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (mAnimator != null && (mAnimator.IsOpening || mAnimator.IsClosing))
		{
			RepositionButtons();
		}
	}

	private void HandleStateDeactivated(string StateName)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void CreateAndPositionImage()
	{
		if (SpritePrefab != null)
		{
			PackedSprite packedSprite = UnityEngine.Object.Instantiate(SpritePrefab) as PackedSprite;
			packedSprite.SetFrame(0, SpriteAnimationFrame);
			switch (SpriteAnchor)
			{
			case SpriteAnchorPoint.Top:
				packedSprite.Anchor = SpriteRoot.ANCHOR_METHOD.UPPER_CENTER;
				break;
			case SpriteAnchorPoint.Left:
				packedSprite.Anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT;
				break;
			}
			packedSprite.transform.parent = mContents.transform.parent;
			packedSprite.transform.position = mBgBox.transform.parent.position + new Vector3(0f, 0f, -0.1f);
			SpritePrefab = packedSprite;
		}
	}

	protected void CreateAndPositionButtons()
	{
		if (ButtonData.Length <= 0 || !(mBgBox != null))
		{
			return;
		}
		m_buttons = new UIButton[ButtonData.Length];
		for (int i = 0; i < m_buttons.Length; i++)
		{
			GameObject gameObject = null;
			ButtonIcon buttonIcon = ButtonIcon.None;
			switch (ButtonData[i].Result)
			{
			case MessageBoxResults.Result.Facebook:
				buttonIcon = ButtonIcon.Facebook;
				gameObject = UnityEngine.Object.Instantiate(IconButtonPrefab) as GameObject;
				break;
			case MessageBoxResults.Result.Twitter:
				buttonIcon = ButtonIcon.Twitter;
				gameObject = UnityEngine.Object.Instantiate(IconButtonPrefab) as GameObject;
				break;
			case MessageBoxResults.Result.Share:
				buttonIcon = ButtonIcon.ShareAndroid;
				gameObject = UnityEngine.Object.Instantiate(IconButtonPrefab) as GameObject;
				break;
			case MessageBoxResults.Result.OK:
			case MessageBoxResults.Result.LargeOK:
				gameObject = UnityEngine.Object.Instantiate(LargeTextButtonPrefab) as GameObject;
				break;
			default:
				gameObject = UnityEngine.Object.Instantiate(TextButtonPrefab) as GameObject;
				break;
			}
			m_buttons[i] = gameObject.GetComponentInChildren<UIButton>();
			BoxCollider boxCollider = gameObject.collider as BoxCollider;
			if (m_buttons[i] != null && boxCollider != null)
			{
				m_buttons[i].Start();
				boxCollider.size = new Vector3(m_buttons[i].width, m_buttons[i].height, 0f);
			}
			if (buttonIcon != ButtonIcon.None)
			{
				PackedSprite componentInChildren = m_buttons[i].GetComponentInChildren<PackedSprite>();
				if (componentInChildren != null)
				{
					componentInChildren.SetFrame(0, (int)buttonIcon);
				}
			}
			m_buttons[i].gameObject.transform.parent = mContents;
			m_buttons[i].gameObject.transform.position = mBgBox.transform.parent.position;
			if (ButtonData[i].Label != null && ButtonData[i].Label != string.Empty)
			{
				m_buttons[i].Text = ((!ButtonData[i].Label.StartsWith("S_")) ? ButtonData[i].Label : AutoLocalize.Get(ButtonData[i].Label));
			}
			m_buttons[i].scriptWithMethodToInvoke = this;
			switch (ButtonData[i].Result)
			{
			case MessageBoxResults.Result.OK:
			case MessageBoxResults.Result.LargeOK:
			{
				m_buttons[i].methodToInvoke = "OnOkPressed";
				FrontEndButton component = m_buttons[i].GetComponent<FrontEndButton>();
				if (component != null)
				{
					component.CurrentState = FrontEndButton.State.Highlighted;
				}
				break;
			}
			case MessageBoxResults.Result.Cancel:
				m_buttons[i].methodToInvoke = "OnCancelPressed";
				break;
			case MessageBoxResults.Result.Facebook:
				m_buttons[i].methodToInvoke = "OnFacebookPressed";
				break;
			case MessageBoxResults.Result.Twitter:
				m_buttons[i].methodToInvoke = "OnTwitterPressed";
				break;
			case MessageBoxResults.Result.Share:
				m_buttons[i].methodToInvoke = "OnSharePressed";
				break;
			default:
				throw new Exception("unknown message box result");
			}
			AnimateButtonOnInvoke component2 = gameObject.GetComponent<AnimateButtonOnInvoke>();
			if (component2 != null)
			{
				component2.ReplaceButtonScript();
			}
		}
	}

	private void OnOkPressed()
	{
		mInternalResult = MessageBoxResults.Result.OK;
	}

	private void OnCancelPressed()
	{
		mInternalResult = MessageBoxResults.Result.Cancel;
	}

	private void OnFacebookPressed()
	{
		mInternalResult = MessageBoxResults.Result.Facebook;
	}

	private void OnTwitterPressed()
	{
		mInternalResult = MessageBoxResults.Result.Twitter;
	}

	private void OnSharePressed()
	{
		mInternalResult = MessageBoxResults.Result.Share;
	}

	protected void RepositionButtons()
	{
		if (m_buttons != null)
		{
			float num = 0f;
			for (int i = 0; i < m_buttons.Length; i++)
			{
				num += m_buttons[i].width;
			}
			float num2 = CommonHelper.CalculatePixelSizeInWorldSpace(mBgBox.transform);
			float num3 = mBgBox.GetHeight() * 0.525f * num2;
			float num4 = 0f - num * 0.5f;
			for (int num5 = m_buttons.Length - 1; num5 >= 0; num5--)
			{
				num4 += m_buttons[num5].width * 0.5f;
				float y = 0f - (num3 + m_buttons[num5].height * 0.525f);
				m_buttons[num5].gameObject.transform.position = mBgBox.transform.parent.position + new Vector3(num4, y, -1f);
				num4 += m_buttons[num5].width * 0.5f;
				m_buttons[num5].collider.enabled = !m_buttons[num5].collider.enabled;
				m_buttons[num5].collider.enabled = !m_buttons[num5].collider.enabled;
			}
		}
	}

	private void GetButtonAreaInfo(out float height, out float topLeftY)
	{
		height = 0f;
		topLeftY = float.MinValue;
		UIButton[] buttons = m_buttons;
		foreach (UIButton uIButton in buttons)
		{
			if (uIButton.height > height)
			{
				height = uIButton.height;
			}
			if (uIButton.TopLeft.y > topLeftY)
			{
				topLeftY = uIButton.TopLeft.y;
			}
		}
	}

	public virtual IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		SetText(Title, Message, messageIsTranslated);
		yield return new WaitForEndOfFrame();
		SoundManager.Instance.ActivateMessageBoxSFX();
		CreateAndPositionButtons();
		mAnimator.AnimateOpen();
		while (mInternalResult == MessageBoxResults.Result.Unknown)
		{
			yield return new WaitForEndOfFrame();
		}
		mAnimator.AnimateClosed();
		SoundManager.Instance.DeactivateMessageBoxSFX();
		while (mAnimator.IsClosing)
		{
			yield return new WaitForEndOfFrame();
		}
		if (Results != null)
		{
			Results.InvokeMethodForResult(mInternalResult);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected void SetText(string title, string message, bool messageIsTranslated)
	{
		if (title.StartsWith("S_"))
		{
			m_titleText.Text = AutoLocalize.Get(title);
		}
		else
		{
			m_titleText.Text = title;
		}
		if (messageIsTranslated)
		{
			m_bodyText.Text = message;
		}
		else
		{
			m_bodyText.Text = AutoLocalize.Get(message);
		}
		m_bodyText.maxWidth = mBgBox.GetWidth() - 32f;
		m_bodyText.maxWidthInPixels = true;
	}

	public void SetupForStandardMsg()
	{
		ButtonData = new MessageBoxButtonData[1];
		ButtonData[0] = new MessageBoxButtonData();
		ButtonData[0].Label = "S_CONTINUE";
		ButtonData[0].Result = MessageBoxResults.Result.OK;
		SpritePrefab = null;
	}

	public void SetupForOkCancelMsg()
	{
		ButtonData = new MessageBoxButtonData[2];
		ButtonData[0] = new MessageBoxButtonData();
		ButtonData[0].Label = "S_CONTINUE";
		ButtonData[0].Result = MessageBoxResults.Result.OK;
		ButtonData[1] = new MessageBoxButtonData();
		ButtonData[1].Label = "S_CANCEL";
		ButtonData[1].Result = MessageBoxResults.Result.Cancel;
		SpritePrefab = null;
	}

	public void SetupForCustomMsg(string okAction)
	{
		ButtonData = new MessageBoxButtonData[1];
		ButtonData[0] = new MessageBoxButtonData();
		ButtonData[0].Label = okAction;
		ButtonData[0].Result = MessageBoxResults.Result.OK;
		SpritePrefab = null;
	}

	public void SetupForCustomMsg(string okAction, string cancelAction)
	{
		ButtonData = new MessageBoxButtonData[2];
		ButtonData[0] = new MessageBoxButtonData();
		ButtonData[0].Label = okAction;
		ButtonData[0].Result = MessageBoxResults.Result.OK;
		ButtonData[1] = new MessageBoxButtonData();
		ButtonData[1].Label = cancelAction;
		ButtonData[1].Result = MessageBoxResults.Result.Cancel;
		SpritePrefab = null;
	}

	public void SetupForDestructiveActionMsg(string okAction, string cancelAction)
	{
		ButtonData = new MessageBoxButtonData[2];
		ButtonData[0] = new MessageBoxButtonData();
		ButtonData[0].Label = cancelAction;
		ButtonData[0].Result = MessageBoxResults.Result.OK;
		ButtonData[1] = new MessageBoxButtonData();
		ButtonData[1].Label = okAction;
		ButtonData[1].Result = MessageBoxResults.Result.Cancel;
		SpritePrefab = null;
	}

	public void SetupForSharableMsg(string okAction)
	{
		ButtonData = new MessageBoxButtonData[2];
		ButtonData[0] = new MessageBoxButtonData();
		ButtonData[0].Label = okAction;
		ButtonData[0].Result = MessageBoxResults.Result.OK;
		ButtonData[1] = new MessageBoxButtonData();
		ButtonData[1].Label = string.Empty;
		ButtonData[1].Result = MessageBoxResults.Result.Share;
		SpritePrefab = null;
	}

	public void SetupForSocialBroadcastMsg(bool facebook, bool twitter)
	{
		if (SwrveServerVariables.Instance.DisableFacebook)
		{
			facebook = false;
		}
		int num = 0;
		int num2 = ((!facebook || !twitter) ? 2 : 3);
		ButtonData = new MessageBoxButtonData[num2];
		if (facebook)
		{
			ButtonData[num] = new MessageBoxButtonData();
			ButtonData[num].Label = string.Empty;
			ButtonData[num].Result = MessageBoxResults.Result.Facebook;
			num++;
		}
		if (twitter)
		{
			ButtonData[num] = new MessageBoxButtonData();
			ButtonData[num].Label = string.Empty;
			ButtonData[num].Result = MessageBoxResults.Result.Twitter;
			num++;
		}
		ButtonData[num] = new MessageBoxButtonData();
		ButtonData[num].Label = "S_CLOSE";
		ButtonData[num].Result = MessageBoxResults.Result.Cancel;
		SpritePrefab = null;
	}

	public void SetupForMsgWithIcon(int spriteFrame, SpriteAnchorPoint anchor)
	{
		ButtonData = new MessageBoxButtonData[1];
		ButtonData[0] = new MessageBoxButtonData();
		ButtonData[0].Label = "S_OK";
		ButtonData[0].Result = MessageBoxResults.Result.OK;
		SpriteAnchor = anchor;
		SpriteAnimationFrame = spriteFrame;
	}
}
