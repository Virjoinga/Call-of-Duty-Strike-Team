using System.Text;
using UnityEngine;

[RequireComponent(typeof(SpriteText))]
public class SearchTypeLabel : MonoBehaviour
{
	private enum State
	{
		SearchingOn = 0,
		OnScreen = 1,
		SearchingOff = 2,
		OffScreen = 3
	}

	public float MaxAnimationLength = 1f;

	public float MinAnimationLength = 0.1f;

	public float OnScreenTimeOrZeroToIgnore;

	public float OffScreenTimeAfterStringChange = 1f;

	public float TimeBetweenCharChanges = 0.1f;

	public bool StartSearchingWhenCreated = true;

	public bool IncludeLowercase = true;

	public bool IncludeNumbers = true;

	private SpriteText mLabel;

	private string mCompleteText;

	private string mLastKnownText;

	private float[] mPlan;

	private float[] mLastChange;

	private float[] mNewPlan;

	private float[] mNewLastChange;

	private float mTimeInStage;

	private State mState;

	private bool mStringChange;

	public bool IsOnScreen
	{
		get
		{
			return mState == State.OnScreen;
		}
	}

	public bool IsChangingText
	{
		get
		{
			return mStringChange;
		}
		set
		{
			mStringChange = value;
		}
	}

	private void Awake()
	{
		mLabel = base.gameObject.GetComponent<SpriteText>();
		mState = ((!StartSearchingWhenCreated) ? State.OffScreen : State.SearchingOn);
		mStringChange = false;
	}

	private void Start()
	{
		Prepare();
		mPlan = mNewPlan;
		mLastChange = mNewLastChange;
	}

	private void Update()
	{
		mTimeInStage += TimeManager.DeltaTime;
		Process();
	}

	public void ResetAndProcess()
	{
		mTimeInStage = 0f;
		Process();
	}

	public void Process()
	{
		if (mLastKnownText != null && mLabel.Text != mLastKnownText)
		{
			Prepare();
			mLabel.Text = mLastKnownText;
			mState = State.SearchingOff;
			mStringChange = true;
		}
		if (mState == State.SearchingOn || mState == State.SearchingOff)
		{
			UpdateSearchingText();
			SoundManager.Instance.PlayTeleTypeSfx();
		}
		else if (mState == State.OnScreen && OnScreenTimeOrZeroToIgnore != 0f)
		{
			if (OnScreenTimeOrZeroToIgnore < mTimeInStage)
			{
				mState = State.SearchingOff;
				mTimeInStage = 0f;
			}
		}
		else if (OffScreenTimeAfterStringChange < mTimeInStage && mStringChange)
		{
			mState = State.SearchingOn;
			mPlan = mNewPlan;
			mLastChange = mNewLastChange;
			mStringChange = false;
			mTimeInStage = 0f;
		}
		if (mLabel != null)
		{
			mLastKnownText = mLabel.Text;
		}
	}

	private void UpdateSearchingText()
	{
		if (mPlan == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		bool flag = mState == State.SearchingOn;
		for (int i = 0; i < mPlan.Length; i++)
		{
			float num2 = Mathf.Clamp(mTimeInStage / mPlan[i], 0f, 1f);
			char value = ((flag || mLastKnownText.Length <= i) ? ' ' : mLastKnownText[i]);
			if (num2 >= 0.2f)
			{
				if (num2 >= 1f)
				{
					value = ((!flag) ? ' ' : mCompleteText[i]);
					num++;
				}
				else if (mTimeInStage - mLastChange[i] > TimeBetweenCharChanges)
				{
					value = GetLetter();
					mLastChange[i] = mTimeInStage;
				}
				else if (mLastKnownText != null && mLastKnownText.Length > i)
				{
					value = mLastKnownText[i];
				}
			}
			stringBuilder.Append(value);
		}
		mLabel.Text = stringBuilder.ToString();
		if (num == mPlan.Length)
		{
			mState = (flag ? State.OnScreen : State.OffScreen);
			mTimeInStage = 0f;
		}
	}

	private void Prepare()
	{
		mCompleteText = mLabel.Text;
		mNewPlan = new float[mCompleteText.Length];
		mNewLastChange = new float[mCompleteText.Length];
		for (int i = 0; i < mCompleteText.Length; i++)
		{
			if (mCompleteText[i] == ' ' || mCompleteText[i] == '\r' || mCompleteText[i] == '\n')
			{
				mNewPlan[i] = 0f;
			}
			else
			{
				mNewPlan[i] = Random.Range(MinAnimationLength, MaxAnimationLength);
			}
			mNewLastChange[i] = 0f;
		}
		int num = mCompleteText.Length;
		for (int j = 0; j < mCompleteText.Length - 1; j++)
		{
			if (mCompleteText[j] == '[' && mCompleteText[j + 1] == '#')
			{
				num = j;
			}
			if (mCompleteText[j] == ']' && num != -1)
			{
				for (int k = num; k <= j; k++)
				{
					mNewPlan[k] = 0f;
				}
				num = -1;
			}
		}
		mTimeInStage = 0f;
	}

	private char GetLetter()
	{
		int num = Random.Range(0, 2);
		int num2 = Random.Range(0, 26);
		if (num == 0 && IncludeNumbers)
		{
			string text = Random.Range(0, 10).ToString();
			return text[0];
		}
		if (num == 1 && IncludeLowercase)
		{
			char c = 'a';
			return (char)(c + num2);
		}
		char c2 = 'A';
		return (char)(c2 + num2);
	}
}
