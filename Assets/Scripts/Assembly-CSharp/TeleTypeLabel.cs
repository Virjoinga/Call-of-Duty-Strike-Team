using UnityEngine;

[RequireComponent(typeof(SpriteText))]
public class TeleTypeLabel : MonoBehaviour
{
	private enum TeleTypeState
	{
		Idle = 0,
		Typing = 1,
		TypeComplete = 2,
		FadingOut = 3
	}

	private SpriteText mLabel;

	private string mCompleteText;

	private float mCharactersToShow;

	public AudioSource TypeSfx;

	public bool StartTypingWhenCreated = true;

	public float TypeSpeed = 10f;

	public float FadeOutAfterTime;

	public float FadeOutTime = 0.5f;

	private float mFadeoutTimer;

	private TeleTypeState mState;

	public string CompleteText
	{
		get
		{
			return mCompleteText;
		}
	}

	private void Awake()
	{
		if (StartTypingWhenCreated)
		{
			mState = TeleTypeState.Typing;
		}
		else
		{
			mState = TeleTypeState.Idle;
		}
	}

	private void Start()
	{
		mLabel = base.gameObject.GetComponent<SpriteText>();
		mCompleteText = mLabel.Text;
		mCharactersToShow = 0f;
		mLabel.Text = string.Empty;
		mFadeoutTimer = 0f;
	}

	public void StartTyping(string text)
	{
		if (mState == TeleTypeState.Idle)
		{
			mState = TeleTypeState.Typing;
			mCompleteText = text;
		}
	}

	private void Update()
	{
		switch (mState)
		{
		case TeleTypeState.Typing:
			if (mCharactersToShow < (float)mCompleteText.Length)
			{
				int num = Mathf.FloorToInt(mCharactersToShow);
				mCharactersToShow += TimeManager.DeltaTime * TypeSpeed;
				int num2 = Mathf.FloorToInt(mCharactersToShow);
				if (num != num2)
				{
					string text = mCompleteText.Substring(num, 1);
					if (text != " ")
					{
						InterfaceSFX.Instance.TextType.Play2D();
					}
					num2 = Mathf.Clamp(num2, 0, mCompleteText.Length);
					mLabel.Text = mCompleteText.Substring(0, num2);
				}
			}
			else
			{
				mState = TeleTypeState.TypeComplete;
			}
			break;
		case TeleTypeState.TypeComplete:
			if (FadeOutAfterTime > 0f)
			{
				mFadeoutTimer += TimeManager.DeltaTime;
				if (mFadeoutTimer >= FadeOutAfterTime)
				{
					mState = TeleTypeState.FadingOut;
					base.gameObject.ColorTo(new Color(0f, 0f, 0f, 0f), FadeOutTime, 0f, EaseType.linear, "HasFadedOut", base.gameObject);
				}
			}
			break;
		case TeleTypeState.FadingOut:
			break;
		}
	}

	private void HasFadedOut()
	{
		Object.Destroy(base.gameObject);
	}
}
