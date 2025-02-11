using UnityEngine;

[RequireComponent(typeof(SpriteText))]
public class MissionTimer : MonoBehaviour
{
	public enum TimerState
	{
		Paused = 0,
		Active = 1,
		Finished = 2,
		Inactive = 3
	}

	private float mCurrentTime;

	private float mStartTime;

	private float mEndTime;

	private TimerState mState = TimerState.Inactive;

	private SpriteText mSpriteText;

	public PackedSprite Background;

	public PackedSprite Brackets;

	public TimerState CurrentState
	{
		get
		{
			return mState;
		}
	}

	private void Awake()
	{
		mSpriteText = GetComponent<SpriteText>();
		StopTimer();
	}

	private void Start()
	{
		if (mState == TimerState.Inactive)
		{
			HideAll(true);
		}
	}

	private void HideAll(bool hide)
	{
		mSpriteText.Hide(hide);
		Background.Hide(hide);
		Brackets.Hide(hide);
	}

	public void Set(float startTime, float endTime)
	{
		mCurrentTime = (mStartTime = startTime);
		mEndTime = endTime;
		HideAll(false);
		UpdateTextSprite();
	}

	public void Add(float amt)
	{
		mCurrentTime += amt;
		UpdateTextSprite();
	}

	public void StartTimer()
	{
		if (mState != TimerState.Active)
		{
			mState = TimerState.Active;
			HideAll(false);
		}
	}

	public void PauseTimer()
	{
		if (mState != 0)
		{
			mState = TimerState.Paused;
		}
	}

	public void StopTimer()
	{
		mState = TimerState.Inactive;
		HideAll(true);
	}

	public float CurrentTime()
	{
		return mCurrentTime;
	}

	private void UpdateTextSprite()
	{
		int num = Mathf.FloorToInt(mCurrentTime);
		int num2 = Mathf.FloorToInt((float)num / 60f);
		int num3 = num - num2 * 60;
		mSpriteText.Text = num2.ToString("D2") + ":" + num3.ToString("D2");
	}

	private void Update()
	{
		TimerState timerState = mState;
		if (timerState != TimerState.Active)
		{
			return;
		}
		int num = Mathf.FloorToInt(mCurrentTime);
		if (mEndTime < mStartTime)
		{
			mCurrentTime -= Time.deltaTime;
			if (mCurrentTime < mEndTime)
			{
				mCurrentTime = mEndTime;
				mState = TimerState.Finished;
			}
		}
		else
		{
			mCurrentTime += Time.deltaTime;
			if (mCurrentTime > mEndTime)
			{
				mCurrentTime = mEndTime;
				mState = TimerState.Finished;
			}
		}
		int num2 = Mathf.FloorToInt(mCurrentTime);
		if (num2 != num)
		{
			UpdateTextSprite();
		}
	}
}
