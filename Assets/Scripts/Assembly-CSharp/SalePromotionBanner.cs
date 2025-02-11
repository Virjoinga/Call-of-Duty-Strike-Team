using UnityEngine;

public class SalePromotionBanner : MenuScreenBlade
{
	public enum IconType
	{
		Token = 0,
		Claymore = 1,
		Grenade = 2,
		MedKit = 3
	}

	public EquipmentIconController EquipmentIcon;

	public PackedSprite IconBackground;

	public PackedSprite TokenIcon;

	public SpriteText TitleText;

	public SpriteText DescriptionText;

	private FrontEndButton mButton;

	private MonoBehaviour mCaller;

	private string mCallback;

	public void Start()
	{
		if (IconBackground != null)
		{
			mButton = IconBackground.GetComponent<FrontEndButton>();
		}
	}

	public void Setup(MonoBehaviour caller, string callback, float percentDiscount, float timeRemainingInSeconds, IconType icon)
	{
		FormatTitle(percentDiscount);
		FormatDescription(timeRemainingInSeconds);
		UpdateIcon(icon);
		mCaller = caller;
		mCallback = callback;
	}

	public void OnEnable()
	{
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
	}

	public void OnDisable()
	{
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		CommonBackgroundBox componentInChildren = GetComponentInChildren<CommonBackgroundBox>();
		if (CommonHelper.CreateRect(componentInChildren).Contains(fingerPos) && mCaller != null && !string.IsNullOrEmpty(mCallback))
		{
			mCaller.Invoke(mCallback, 0f);
			if (mButton != null)
			{
				mButton.Activate();
			}
		}
	}

	private void FormatTitle(float percentDiscount)
	{
		if (TitleText != null)
		{
			string format = Language.Get("S_SALE_BANNER_PERCENT_EXTRA");
			TitleText.Text = string.Format(format, (int)(percentDiscount * 100f));
		}
	}

	private void FormatDescription(float timeRemaining)
	{
		if (DescriptionText != null)
		{
			int num = Mathf.FloorToInt(timeRemaining) / 60;
			int num2 = Mathf.FloorToInt(num) / 60;
			num -= 60 * num2;
			string format = Language.Get("S_SALE_BANNER_TIME_REMAINING");
			DescriptionText.Text = string.Format(format, num2, num);
		}
	}

	private void UpdateIcon(IconType icon)
	{
		EquipmentIconController.EquipmentType equipment = EquipmentIconController.EquipmentType.Claymore;
		bool flag = false;
		bool flag2 = false;
		switch (icon)
		{
		case IconType.Claymore:
			flag = true;
			break;
		case IconType.Grenade:
			equipment = EquipmentIconController.EquipmentType.Grenade;
			flag = true;
			break;
		case IconType.MedKit:
			equipment = EquipmentIconController.EquipmentType.MediPack;
			flag = true;
			break;
		case IconType.Token:
			flag2 = true;
			break;
		}
		if (EquipmentIcon != null)
		{
			EquipmentIcon.gameObject.SetActive(flag);
			EquipmentIcon.SetEquipment(equipment, true);
		}
		if (TokenIcon != null)
		{
			TokenIcon.gameObject.SetActive(flag2);
		}
	}
}
