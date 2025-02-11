using UnityEngine;

public class FlashpointBanner : MenuScreenBlade
{
	private const float MIN_FLASH_TIME = 0.1f;

	public PackedSprite FlashpointIcon;

	public SpriteText TitleText;

	public SpriteText DescriptionText;

	public FrontEndButton Button;

	private MonoBehaviour mCaller;

	private string mCallback;

	private int mIndex;

	public int ShowingIndex
	{
		get
		{
			return mIndex;
		}
	}

	public void Setup(MonoBehaviour caller, string callback, int index)
	{
		mCaller = caller;
		mCallback = callback;
		mIndex = index;
	}

	public override void Update()
	{
		base.Update();
		GlobalUnrestController instance = GlobalUnrestController.Instance;
		if (!(instance != null))
		{
			return;
		}
		int timeRemainingInSeconds = instance.GetTimeRemainingInSeconds(mIndex);
		float num = (float)timeRemainingInSeconds / 900f;
		if (timeRemainingInSeconds > 0)
		{
			if (FlashpointIcon != null)
			{
				Color color = Color.Lerp(ColourChart.MissionSurvival, ColourChart.MissionFlashpoint, num);
				FlashpointIcon.SetColor(color);
			}
			if (DescriptionText != null)
			{
				if (num > 1f)
				{
					DescriptionText.Text = Language.Get("S_NEW_FLASHPOINT");
					return;
				}
				int num2 = timeRemainingInSeconds / 60;
				int num3 = num2 / 60;
				int num4 = num2 - num3 * 60;
				DescriptionText.Text = string.Format(Language.Get("S_FL_BANNER_CRITICAL_TIME"), num3, num4);
			}
		}
		else
		{
			Deactivate();
		}
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		CommonBackgroundBox componentInChildren = GetComponentInChildren<CommonBackgroundBox>();
		if (CommonHelper.CreateRect(componentInChildren).Contains(fingerPos) && mCaller != null && !string.IsNullOrEmpty(mCallback))
		{
			mCaller.Invoke(mCallback, 0f);
			if (Button != null)
			{
				Button.Activate();
			}
		}
	}
}
