using System.Collections;
using UnityEngine;

public class LevelUpMessageBox : MessageBox
{
	private enum MsgType
	{
		LevelUp = 0,
		Prestige = 1,
		PostPrestige = 2
	}

	public RankIconController RankIcon;

	public PerkIconController PerkIcon;

	public WeaponIconController WeaponIcon;

	public EquipmentIconController EquipmentIcon;

	public PerkIconController NextUnlockPerkIcon;

	public WeaponIconController NextUnlockWeaponIcon;

	public EquipmentIconController NextUnlockEquipmentIcon;

	public ProgressBar LevelBar;

	public SpriteText NewLevelText;

	public SpriteText OldLevelText;

	public SpriteText UnlockText;

	public SpriteText ItemUnlockedText;

	public SpriteText NextUnlockText;

	public PackedSprite TokensSprite;

	public PackedSprite VeteranSprite;

	public PackedSprite NextUnlockTokensSprite;

	public PackedSprite NextUnlockVeteranSprite;

	private AnimatedIconListHelper mUnlockAnim;

	private AnimatedIconListHelper mNextUnlockAnim;

	private Transform FinalPositionOfRankText;

	private int mPrestigeLevel;

	private int mNewLevel;

	private int mLeveledFrom;

	private int mMaxLevels;

	private int mAbsoluteLevel;

	private MsgType mMsgType;

	public void SetNewLevel(int oldLevel, int newLevel, int prestigeLevel, int maxLevels)
	{
		mNewLevel = newLevel;
		mLeveledFrom = oldLevel;
		mMaxLevels = maxLevels;
		mPrestigeLevel = prestigeLevel;
		mAbsoluteLevel = -1;
		mMsgType = ((prestigeLevel > 0) ? MsgType.PostPrestige : MsgType.LevelUp);
	}

	public void SetPrestigeLevel(int oldLevel, int newLevel, int prestigeLevel, int maxLevels)
	{
		mNewLevel = newLevel;
		mLeveledFrom = oldLevel;
		mPrestigeLevel = prestigeLevel;
		mMaxLevels = maxLevels;
		mAbsoluteLevel = mPrestigeLevel * mMaxLevels + mNewLevel;
		mMsgType = MsgType.Prestige;
	}

	public override IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		Transform visible = base.transform.Find("Visible");
		Transform content = visible.Find("Content");
		FinalPositionOfRankText = content.Find("PositionOfRankTextFinal");
		string level = AutoLocalize.Get("S_LEVEL");
		string prestige = Language.Get("S_PRESTIGE_LEVEL");
		string newRank = XPManager.Instance.XPLevelName(mNewLevel);
		string oldRank = XPManager.Instance.XPLevelName(mLeveledFrom);
		string newRankDisplayText2 = string.Empty;
		newRankDisplayText2 = ((mMsgType == MsgType.LevelUp) ? (level + " " + mNewLevel + ": " + newRank) : (prestige + " " + mPrestigeLevel + ": " + newRank));
		string oldRankDisplayText2 = string.Empty;
		if (mMsgType == MsgType.PostPrestige || (mMsgType == MsgType.Prestige && mPrestigeLevel > 1))
		{
			int oldPrestigeLevel = ((mMsgType != MsgType.Prestige) ? mPrestigeLevel : (mPrestigeLevel - 1));
			oldRankDisplayText2 = prestige + " " + oldPrestigeLevel + ": " + oldRank;
		}
		else
		{
			oldRankDisplayText2 = level + " " + mLeveledFrom + ": " + oldRank;
		}
		mUnlockAnim = new AnimatedIconListHelper();
		mUnlockAnim.Setup(PerkIcon, WeaponIcon, EquipmentIcon, TokensSprite, VeteranSprite, ItemUnlockedText);
		mNextUnlockAnim = new AnimatedIconListHelper();
		mNextUnlockAnim.Setup(NextUnlockPerkIcon, NextUnlockWeaponIcon, NextUnlockEquipmentIcon, NextUnlockTokensSprite, NextUnlockVeteranSprite, NextUnlockText);
		UnlockText.Hide(true);
		MenuSFX.Instance.LevelUp.Play2D();
		SetText(Title, oldRankDisplayText2, true);
		NewLevelText.Text = mNewLevel.ToString();
		OldLevelText.Text = mLeveledFrom.ToString();
		RankIcon.SetRank(mLeveledFrom);
		mAnimator.AnimateOpen();
		while (mAnimator.IsOpening)
		{
			yield return new WaitForEndOfFrame();
		}
		LevelBar.SetValueNow(0f);
		LevelBar.SetValue(1f);
		while (LevelBar.IsUpdating)
		{
			yield return new WaitForEndOfFrame();
		}
		SearchTypeLabel searchAnim = m_bodyText.gameObject.GetComponent<SearchTypeLabel>();
		m_bodyText.Text = newRankDisplayText2;
		searchAnim.IsChangingText = true;
		if (mMsgType == MsgType.Prestige)
		{
			RankIcon.SetRank(mAbsoluteLevel);
		}
		else
		{
			RankIcon.SetRank(mNewLevel);
		}
		while (searchAnim.IsChangingText || !searchAnim.IsOnScreen)
		{
			yield return new WaitForEndOfFrame();
		}
		float delayTime = 0f;
		while (delayTime < 0.5f)
		{
			delayTime += TimeManager.DeltaTime;
			yield return null;
		}
		CreateAndPositionButtons();
		RepositionButtons();
		int levelOfNextUnlock = SearchForNextUnlock(mNewLevel, mPrestigeLevel);
		if (levelOfNextUnlock != -1)
		{
			string baseString2 = Language.Get("S_NEXT_UNLOCK_MSG");
			baseString2 = string.Format(baseString2, levelOfNextUnlock, "{0}");
			mNextUnlockAnim.Begin(baseString2);
		}
		int numUnlocks = PrepareUnlocks(mLeveledFrom + 1, mNewLevel);
		if (numUnlocks > 0)
		{
			mUnlockAnim.Begin();
			UnlockText.Hide(false);
			RankIcon.gameObject.SetActive(false);
			LevelBar.gameObject.SetActive(false);
			m_bodyText.gameObject.MoveTo(FinalPositionOfRankText.position, 0.25f, 0f, EaseType.easeInOutCubic);
			NewLevelText.gameObject.FadeTo(0f, 0.25f, 0f);
			OldLevelText.gameObject.FadeTo(0f, 0.25f, 0f);
			UnlockText.gameObject.FadeTo(1f, 0.25f, 0f);
		}
		while (mInternalResult == MessageBoxResults.Result.Unknown)
		{
			mUnlockAnim.Update();
			mNextUnlockAnim.Update();
			yield return new WaitForEndOfFrame();
		}
		mUnlockAnim.Finish();
		mNextUnlockAnim.Finish();
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
		Object.Destroy(base.gameObject);
		SwrveEventsUI.SwrveTalkTrigger_LevelUp();
	}

	private int PrepareUnlocks(int levelfrom, int newLevel)
	{
		int num = 0;
		int num2 = mPrestigeLevel;
		if (mMsgType == MsgType.Prestige && num2 == 1)
		{
			num2 = 0;
			newLevel = mMaxLevels;
		}
		WeaponManager instance = WeaponManager.Instance;
		int num3 = 0;
		for (int i = levelfrom; i <= newLevel; i++)
		{
			if (num2 == 0)
			{
				if (i == 10)
				{
					mUnlockAnim.AddVeteranIcon();
				}
				SearchWeaponsForUnlocks(i, instance.AssaultRifles, mUnlockAnim, true);
				SearchWeaponsForUnlocks(i, instance.LightMachineGuns, mUnlockAnim, true);
				SearchWeaponsForUnlocks(i, instance.Shotguns, mUnlockAnim, true);
				SearchWeaponsForUnlocks(i, instance.SniperRifles, mUnlockAnim, true);
				SearchPerksForUnlocks(i, StatsManager.Instance.PerksList.Perks, mUnlockAnim, true);
				SearchEquipmentForUnlocks(i, instance.LoadoutEquipment, mUnlockAnim, true);
			}
			num3 += SwrveServerVariables.Instance.LevelUpReward(i);
		}
		if (num3 != 0)
		{
			GameSettings instance2 = GameSettings.Instance;
			if (instance2 != null)
			{
				instance2.PlayerCash().AwardHardCash(num3, "LevelUp");
			}
			mUnlockAnim.AddTokenIcon(num3, true);
		}
		return num + mUnlockAnim.Count;
	}

	private int SearchForNextUnlock(int newLevel, int prestigeLevel)
	{
		int result = -1;
		WeaponManager instance = WeaponManager.Instance;
		int num = Mathf.Min(XPManager.Instance.m_MaxPrestigeLevels, prestigeLevel + 1);
		for (int i = prestigeLevel; i < num; i++)
		{
			if (mNextUnlockAnim.Count != 0)
			{
				break;
			}
			for (int j = newLevel + 1; j <= mMaxLevels; j++)
			{
				if (mNextUnlockAnim.Count != 0)
				{
					break;
				}
				result = j;
				if (i == 0)
				{
					if (j == 10)
					{
						mNextUnlockAnim.AddVeteranIcon();
					}
					SearchWeaponsForUnlocks(j, instance.AssaultRifles, mNextUnlockAnim, false);
					SearchWeaponsForUnlocks(j, instance.LightMachineGuns, mNextUnlockAnim, false);
					SearchWeaponsForUnlocks(j, instance.Shotguns, mNextUnlockAnim, false);
					SearchWeaponsForUnlocks(j, instance.SniperRifles, mNextUnlockAnim, false);
					SearchPerksForUnlocks(j, StatsManager.Instance.PerksList.Perks, mNextUnlockAnim, false);
					SearchEquipmentForUnlocks(j, instance.LoadoutEquipment, mNextUnlockAnim, false);
				}
				int num2 = SwrveServerVariables.Instance.LevelUpReward(j);
				if (num2 != 0)
				{
					mNextUnlockAnim.AddTokenIcon(num2, false);
				}
			}
		}
		return result;
	}

	private void SearchWeaponsForUnlocks(int newLevel, WeaponDescriptor[] weapons, AnimatedIconListHelper unlockHelper, bool unlocking)
	{
		GameSettings instance = GameSettings.Instance;
		foreach (WeaponDescriptor weaponDescriptor in weapons)
		{
			if (weaponDescriptor.UnlockLevel == newLevel && !instance.WasUnlockedEarly(weaponDescriptor.Name))
			{
				unlockHelper.AddWeaponIcon(weaponDescriptor);
				if (unlocking)
				{
					SwrveEventsProgression.WeaponUnlocked(weaponDescriptor.Name);
				}
			}
		}
	}

	private void SearchEquipmentForUnlocks(int newLevel, EquipmentDescriptor[] equipment, AnimatedIconListHelper unlockHelper, bool unlocking)
	{
		GameSettings instance = GameSettings.Instance;
		foreach (EquipmentDescriptor equipmentDescriptor in equipment)
		{
			if (equipmentDescriptor.UnlockLevel == newLevel && !instance.WasUnlockedEarly(equipmentDescriptor.Name))
			{
				unlockHelper.AddEquipmentIcon(equipmentDescriptor, 0);
			}
		}
	}

	private void SearchPerksForUnlocks(int newLevel, Perk[] perks, AnimatedIconListHelper unlockHelper, bool unlocking)
	{
		GameSettings instance = GameSettings.Instance;
		foreach (Perk perk in perks)
		{
			if (perk.UnlockLevel == newLevel && !instance.WasUnlockedEarly(perk.Identifier.ToString()))
			{
				unlockHelper.AddPerkIcon(perk.Identifier);
				if (unlocking)
				{
					SwrveEventsProgression.PerkUnlocked(perk.Identifier, false);
				}
			}
		}
	}
}
