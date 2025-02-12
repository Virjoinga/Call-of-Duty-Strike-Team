using System.Collections;
using UnityEngine;

public class DailyRewardMessageBox : MessageBox
{
	private const string kTimerName = "DailyRewardTimer";

	private const string kComebackReminderName = "DailyRewardReminder";

	public SpriteText TodaysReward;

	public SpriteText TodaysRewardInfo;

	public SpriteText TomorrowsReward;

	public SpriteText NextTokenReward;

	public SpriteText TodaysRewardTitle;

	public SpriteText TomorrowsRewardTitle;

	public PackedSprite TodaysTokenIcon;

	public PackedSprite TomorrowsTokenIcon;

	public EquipmentIconController TodaysRewardIcon;

	public EquipmentIconController TomorrowsRewardIcon;

	public override void Awake()
	{
		base.Awake();
	}

	public override IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		m_titleText.Text = Language.Get(Title).ToUpper();
		if (TodaysRewardTitle != null)
		{
			TodaysRewardTitle.Text = Language.Get("S_TODAYS_REWARD_TITLE").ToUpper();
		}
		if (TomorrowsRewardTitle != null)
		{
			TomorrowsRewardTitle.Text = Language.Get("S_TOMORROWS_REWARD_TITLE").ToUpper();
		}
		InterfaceSFX.Instance.MessageBoxOn.Play2D();
		GiveReward();
		CreateAndPositionButtons();
		mAnimator.AnimateOpen();
		while (mAnimator.IsOpening)
		{
			UIButton[] buttons = m_buttons;
			foreach (UIButton button in buttons)
			{
				button.GetComponent<Collider>().enabled = !button.GetComponent<Collider>().enabled;
				button.GetComponent<Collider>().enabled = !button.GetComponent<Collider>().enabled;
			}
			yield return new WaitForEndOfFrame();
		}
		while (mInternalResult == MessageBoxResults.Result.Unknown)
		{
			if (FrontEndController.Instance.ActiveScreen != 0 && FrontEndController.Instance.ActiveScreen != ScreenID.SquadLoadOut)
			{
				mInternalResult = MessageBoxResults.Result.OK;
			}
			yield return new WaitForEndOfFrame();
		}
		mAnimator.AnimateClosed();
		MenuSFX.Instance.MenuBoxClose.Play2D();
		while (mAnimator.IsClosing)
		{
			yield return new WaitForEndOfFrame();
		}
		if (Results != null)
		{
			Results.InvokeMethodForResult(mInternalResult);
		}
		UIButton[] buttons2 = m_buttons;
		foreach (UIButton button2 in buttons2)
		{
			Object.Destroy(button2.gameObject);
		}
		Object.Destroy(base.gameObject);
	}

	private void GiveReward()
	{
		int num = Mathf.Max(0, SecureStorage.Instance.ConsecutiveDays - 2);
		char c = CommonHelper.HardCurrencySymbol();
		SwrveEventsMetaGame.ClaimedReward(num);
		if (num > 11)
		{
			num -= Mathf.CeilToInt((float)(num - 11) * 0.25f) * 4;
		}
		int index = ((num == 11) ? 8 : (num + 1));
		int num2 = 4 - (num + 1) % 4;
		DailyReward dailyReward = DailyRewards.Instance.rewards[num];
		DailyReward dailyReward2 = DailyRewards.Instance.rewards[index];
		if (!dailyReward.addTokensInstead)
		{
			EquipmentSettings[] equipment = GameSettings.Instance.Equipment;
			foreach (EquipmentSettings equipmentSettings in equipment)
			{
				if (equipmentSettings.Descriptor != null && equipmentSettings.Descriptor.Type == dailyReward.item)
				{
					if (equipmentSettings.NumItems == equipmentSettings.SlotSize)
					{
						TodaysRewardInfo.Text = Language.GetFormatString("S_NO_ROOM_FOR_REWARD", Language.Get(equipmentSettings.Descriptor.Name));
					}
					else if (equipmentSettings.NumItems + dailyReward.amount > equipmentSettings.SlotSize)
					{
						TodaysRewardInfo.Text = Language.GetFormatString("S_PARTIAL_ROOM_FOR_REWARD", (equipmentSettings.SlotSize - equipmentSettings.NumItems).ToString(), Language.Get(equipmentSettings.Descriptor.Name));
						GameSettings.Instance.AddEquipment(equipmentSettings.Descriptor, equipmentSettings.SlotSize - equipmentSettings.NumItems);
						SecureStorage.Instance.SaveGameSettings();
					}
					else
					{
						TodaysRewardInfo.Text = string.Empty;
						GameSettings.Instance.AddEquipment(equipmentSettings.Descriptor, dailyReward.amount);
						SecureStorage.Instance.SaveGameSettings();
					}
				}
			}
			TodaysReward.Text = string.Format("      x{0}", dailyReward.amount.ToString());
			TodaysRewardIcon.SetEquipment(dailyReward.item, true);
			TodaysTokenIcon.Hide(true);
		}
		else
		{
			GameSettings.Instance.PlayerCash().AwardHardCash(dailyReward.amount, "DailyReward");
			TodaysReward.Text = string.Format("{0}{1}", c, dailyReward.amount.ToString());
			TodaysRewardInfo.Text = string.Empty;
			TodaysTokenIcon.Hide(false);
		}
		if (!dailyReward2.addTokensInstead)
		{
			TomorrowsReward.Text = string.Format("      x{0}", dailyReward2.amount.ToString(), dailyReward2.item.ToString());
			TomorrowsRewardIcon.SetEquipment(dailyReward2.item, true);
			TomorrowsTokenIcon.Hide(true);
		}
		else
		{
			TomorrowsReward.Text = string.Format("{0}{1}", c, dailyReward2.amount.ToString());
			TomorrowsTokenIcon.Hide(false);
		}
		if (num2 != 1)
		{
			NextTokenReward.Text = Language.GetFormatString("S_TOKEN_REWARD_PART_1", num2, c);
		}
		else
		{
			NextTokenReward.Text = Language.GetFormatString("S_TOKEN_REWARD_PART_2", c);
		}
		SecureStorage.Instance.NeedsDailyReward = false;
	}

	public static void ScheduleNotification()
	{
	}

	private static void CancelNotification()
	{
	}
}
