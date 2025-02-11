using UnityEngine;

public class PadInputPc360 : MonoBehaviour, iPadInput
{
	private struct ButtonData
	{
		private float mTimeDown;

		private float mLastTapTime;

		private int mTapCount;

		private int mLiveTapCount;

		private bool mButtonState;

		private bool mButtonLastState;

		private bool mIsHeld;

		public bool IsDown
		{
			get
			{
				return mButtonState;
			}
		}

		public bool JustDown
		{
			get
			{
				return mButtonState && mButtonState != mButtonLastState;
			}
		}

		public bool JustUp
		{
			get
			{
				return !mButtonState && mButtonState != mButtonLastState;
			}
		}

		public bool IsHeld
		{
			get
			{
				return mIsHeld;
			}
		}

		public int TapCount
		{
			get
			{
				return mLiveTapCount;
			}
		}

		public void Reset()
		{
			mTimeDown = 0f;
			mLastTapTime = 0f;
			mTapCount = 0;
			mButtonState = false;
			mButtonLastState = false;
			mIsHeld = false;
		}

		public void Update(bool buttonState)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			mButtonLastState = mButtonState;
			mButtonState = buttonState;
			mLiveTapCount = 0;
			if (mButtonState != mButtonLastState)
			{
				if (mButtonState)
				{
					mTimeDown = realtimeSinceStartup;
				}
				else
				{
					float num = realtimeSinceStartup - mTimeDown;
					if (num < 0.1f)
					{
						if (realtimeSinceStartup - mLastTapTime < 0.15f)
						{
							mTapCount++;
							if (mTapCount > 2)
							{
								mTapCount = 1;
							}
						}
						else
						{
							mTapCount = 1;
						}
						mLiveTapCount = mTapCount;
						mLastTapTime = realtimeSinceStartup;
					}
					else
					{
						mTapCount = 0;
					}
					mTimeDown = -1f;
				}
			}
			mIsHeld = mButtonState && realtimeSinceStartup - mTimeDown >= 0.8f;
		}
	}

	private struct AxisData
	{
		private float mValue;

		private float mLastValue;

		private float mTimeDown;

		private bool mIsHeldPositive;

		private bool mIsHeldNegative;

		public float Value
		{
			get
			{
				return mValue;
			}
		}

		public bool IsDownPositive
		{
			get
			{
				return mValue >= 0.8f;
			}
		}

		public bool IsDownNegative
		{
			get
			{
				return mValue <= -0.8f;
			}
		}

		public bool IsDownAny
		{
			get
			{
				return Mathf.Abs(mValue) >= 0.8f;
			}
		}

		public bool JustDownPositive
		{
			get
			{
				return mValue >= 0.8f && mLastValue < 0.8f;
			}
		}

		public bool JustDownNegative
		{
			get
			{
				return mValue <= -0.8f && mLastValue > -0.8f;
			}
		}

		public bool JustDownAny
		{
			get
			{
				return JustDownPositive || JustDownNegative;
			}
		}

		public bool JustUpPositive
		{
			get
			{
				return mValue < 0.8f && mLastValue >= 0.8f;
			}
		}

		public bool JustUpNegative
		{
			get
			{
				return mValue > -0.8f && mLastValue <= -0.8f;
			}
		}

		public bool IsHeldPositive
		{
			get
			{
				return mIsHeldPositive;
			}
		}

		public bool IsHeldNegative
		{
			get
			{
				return mIsHeldNegative;
			}
		}

		public bool IsHeldAny
		{
			get
			{
				return mIsHeldPositive || mIsHeldNegative;
			}
		}

		public void Reset()
		{
			mValue = 0f;
			mLastValue = 0f;
			mTimeDown = 0f;
			mIsHeldPositive = false;
			mIsHeldNegative = false;
		}

		public void Update(float newValue, bool inverse, float min, float max)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			newValue = Mathf.Clamp(newValue, min, max);
			if (inverse)
			{
				newValue = 0f - newValue;
			}
			mLastValue = mValue;
			mValue = newValue;
			if (JustDownAny)
			{
				mTimeDown = realtimeSinceStartup;
			}
			bool flag = realtimeSinceStartup - mTimeDown > 0.8f;
			mIsHeldPositive = IsDownPositive && flag;
			mIsHeldNegative = IsDownNegative && flag;
		}
	}

	private const float HELD_TIME = 0.8f;

	private const float TAP_TIME = 0.1f;

	private const float MULTI_TAP_SPACE = 0.15f;

	private const int MAX_TAPS = 2;

	private const float AXIS_TRIGGER_VALUE = 0.8f;

	private ButtonData[] mButtons = new ButtonData[10];

	private AxisData[] mAxis = new AxisData[8];

	private void Start()
	{
	}

	public virtual void ToggleDebugDraw()
	{
	}

	public virtual void Reset()
	{
		ButtonData[] array = mButtons;
		foreach (ButtonData buttonData in array)
		{
			buttonData.Reset();
		}
		AxisData[] array2 = mAxis;
		foreach (AxisData axisData in array2)
		{
			axisData.Reset();
		}
	}

	private void PadLog(string str)
	{
		Debug.Log(str);
	}

	public virtual bool IsConnected()
	{
		string[] joystickNames = Input.GetJoystickNames();
		return joystickNames.Length > 0;
	}

	public virtual bool Down(int button)
	{
		return mButtons[button].IsDown;
	}

	public virtual bool Pressed(int button)
	{
		return mButtons[button].JustDown;
	}

	public virtual bool Released(int button)
	{
		return mButtons[button].JustUp;
	}

	public virtual bool Held(int button)
	{
		return mButtons[button].IsHeld;
	}

	public virtual bool AxisPressedPositive(int axis)
	{
		return mAxis[axis].JustDownPositive;
	}

	public virtual bool AxisPressedNegative(int axis)
	{
		return mAxis[axis].JustDownNegative;
	}

	public virtual bool AxisHeldPositive(int axis)
	{
		return mAxis[axis].IsHeldPositive;
	}

	public virtual float AxisValue(int axis)
	{
		return mAxis[axis].Value;
	}

	private void UpdateButton(ref ButtonData bd, KeyCode key)
	{
		bd.Update(Input.GetKey(key));
	}

	private void Update()
	{
		for (int i = 0; i < mButtons.Length; i++)
		{
			mButtons[i].Update(Input.GetKey((KeyCode)(330 + i)));
		}
		mAxis[0].Update(Input.GetAxis("LeftStickX"), false, -1f, 1f);
		mAxis[1].Update(Input.GetAxis("LeftStickY"), false, -1f, 1f);
		mAxis[2].Update(Input.GetAxis("RightStickX"), false, -1f, 1f);
		mAxis[3].Update(Input.GetAxis("RightStickY"), false, -1f, 1f);
		mAxis[4].Update(Input.GetAxis("DPadX"), false, -1f, 1f);
		mAxis[5].Update(Input.GetAxis("DPadY"), false, -1f, 1f);
		mAxis[6].Update(Input.GetAxis("LeftTrigger"), false, 0f, 1f);
		mAxis[7].Update(Input.GetAxis("RightTrigger"), true, -1f, 0f);
	}
}
