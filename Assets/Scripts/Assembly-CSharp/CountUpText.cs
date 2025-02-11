using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(SpriteText))]
public class CountUpText : MonoBehaviour
{
	public enum Currency
	{
		None = 0,
		Hard = 1,
		Soft = 2
	}

	public string Prefix;

	public string Postfix;

	public Currency IsCurrency;

	private NumberFormatInfo mNfi;

	private SpriteText mText;

	private float mStartTime;

	private float mTimeToCount;

	private int mStart;

	private int mTarget;

	public int Value
	{
		get
		{
			return mTarget;
		}
	}

	public void CountTo(int target, float seconds)
	{
		mStart = mTarget;
		mTarget = target;
		mTimeToCount = seconds;
		mStartTime = Time.realtimeSinceStartup;
		if (seconds <= 0f)
		{
			SetValue(mTarget);
		}
	}

	private void Awake()
	{
		mText = GetComponent<SpriteText>();
		mStartTime = (mTimeToCount = 0f);
		mStart = (mTarget = 0);
		mNfi = GlobalizationUtils.GetNumberFormat(0);
	}

	private void Update()
	{
		if (mTimeToCount > 0f)
		{
			float num = Time.realtimeSinceStartup - mStartTime;
			int num2 = mTarget - mStart;
			float num3 = Mathf.Clamp(num / mTimeToCount, 0f, 1f);
			int value = mStart + (int)((float)num2 * num3);
			SetValue(value);
		}
	}

	public void Reset()
	{
		SetValue(0);
	}

	private void SetValue(int to)
	{
		if (IsCurrency == Currency.None)
		{
			mText.Text = Prefix + to.ToString("N", mNfi) + Postfix;
			return;
		}
		char c = CommonHelper.HardCurrencySymbol();
		mText.Text = c + to.ToString("N", mNfi);
	}
}
