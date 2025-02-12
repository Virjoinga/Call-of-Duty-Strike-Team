using System.Collections.Generic;
using UnityEngine;

public class LoadoutPerkMenuController : MenuScreenBlade, CarouselDataSource
{
	public LoadoutPerksPanel PerksPanel;

	public LoadoutSideMenu CategoriesSideMenuPanel;

	public Carousel CarouselControl;

	private Perk[] mCurrentData;

	private MenuScreenBlade mPerkPanelBlade;

	private LoadoutPerkPanelController mPerkPanel;

	private PurchaseFlowHelper.PurchaseData mData;

	private Transform mPerkPanelPosition;

	private int mCategory;

	private int mCurrentIndex;

	private int mEditingSlot;

	public MenuScreenBlade PerkPanelBlade
	{
		set
		{
			mPerkPanelBlade = value;
			mPerkPanel = mPerkPanelBlade.GetComponentInChildren<LoadoutPerkPanelController>();
		}
	}

	public LoadoutPerkPanelController PerkPanel
	{
		get
		{
			return mPerkPanel;
		}
	}

	public override void Awake()
	{
		base.Awake();
		mEditingSlot = 0;
		mPerkPanelPosition = base.transform.Find("PerkPanelPosition");
		mData = new PurchaseFlowHelper.PurchaseData();
		mData.ScriptToCallWithResult = this;
		mData.MethodToCallWithResult = "RefreshScreen";
	}

	private void Start()
	{
		if (GameSettings.Instance.HasPerk(PerkType.Perk3Greed))
		{
			SetDataToCategory(2);
		}
		else if (GameSettings.Instance.HasPerk(PerkType.Perk2Greed))
		{
			SetDataToCategory(1);
		}
		else
		{
			SetDataToCategory(0);
		}
	}

	public void SetSlot(int highlightSlot)
	{
		mEditingSlot = highlightSlot;
		GameObject objectToDisplayOver = null;
		if (mPerkPanel != null && base.IsActive)
		{
			objectToDisplayOver = mPerkPanel.SetPerkHighlighted(mEditingSlot);
		}
		if (mData != null)
		{
			mData.SlotIndex = mEditingSlot;
		}
		StatsManager instance = StatsManager.Instance;
		GameSettings instance2 = GameSettings.Instance;
		if (instance2 != null && instance != null && instance.PerksManager() != null)
		{
			PerkType perkType = instance2.PerkSlotContents(mEditingSlot);
			Perk perk = instance.PerksManager().GetPerk(perkType);
			if (perkType != PerkType.None && perk != null)
			{
				if (base.IsActive)
				{
					PerkStatus perkStatus = StatsManager.Instance.PerksManager().GetPerkStatus(perkType);
					bool flag = perk.ProXPTarget <= perkStatus.ProXP || instance2.WasProPerkUnlockedEarly(perkType);
					string text = perkType.ToString().ToUpper();
					string text2 = Language.Get("S_" + text + ((!flag) ? string.Empty : "_PRO"));
					ToolTipController.Instance.DoTooltip(text2, objectToDisplayOver);
				}
				else
				{
					SetDataToCategory(perk.Tier - 1);
					mCurrentIndex = 0;
					for (int i = 0; i < mCurrentData.Length; i++)
					{
						if (mCurrentData[i].Identifier == perkType)
						{
							mCurrentIndex = i;
							break;
						}
					}
				}
			}
			else if (!base.IsActive)
			{
				int xPLevelAbsolute = XPManager.Instance.GetXPLevelAbsolute();
				if (GameSettings.Instance.HasPerk(PerkType.Perk3Greed))
				{
					SetDataToCategory(2);
				}
				else if (GameSettings.Instance.HasPerk(PerkType.Perk2Greed))
				{
					SetDataToCategory(1);
				}
				else
				{
					SetDataToCategory(0);
				}
				for (int j = 0; j < mCurrentData.Length; j++)
				{
					if (!IsPerkLocked(instance2, xPLevelAbsolute, j))
					{
						mCurrentIndex = j;
						break;
					}
				}
			}
		}
		if (base.IsActive)
		{
			CheckForLockedSlot();
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if (mPerkPanel != null)
		{
			mPerkPanel.gameObject.MoveTo(mPerkPanelPosition.localPosition, 0.2f, 0f, EaseType.easeInOutCubic);
			CommonBackgroundBox componentInChildren = mPerkPanel.GetComponentInChildren<CommonBackgroundBox>();
			if (componentInChildren != null)
			{
				componentInChildren.Resize();
			}
		}
		Transform root = base.transform.root;
		AnimatedHighlight componentInChildren2 = root.GetComponentInChildren<AnimatedHighlight>();
		if (componentInChildren2 != null && mPerkPanelPosition != null)
		{
			componentInChildren2.SetHighlightNow(mPerkPanelPosition.position, Vector2.zero);
		}
		MessageBoxController instance = MessageBoxController.Instance;
		if (instance != null)
		{
			instance.DoLoadoutPerkDialogue();
		}
		CarouselControl.SetDataSource(this);
		CarouselControl.Refresh();
	}

	public void OnSelectedPerkTier()
	{
		int num = CategoriesSideMenuPanel.FindPressed();
		if (num != -1 && mCategory != num && !GameSettings.Instance.HasGreed())
		{
			PerksPanel.Deactivate(RefreshPerkDataWhenOffScreen);
			SetDataToCategory(num);
		}
	}

	public virtual void Next()
	{
		mCurrentIndex = (mCurrentIndex + (mCurrentData.Length - 1)) % mCurrentData.Length;
	}

	public virtual void Previous()
	{
		mCurrentIndex = (mCurrentIndex + 1) % mCurrentData.Length;
	}

	public virtual void Populate(List<CarouselItem> items, int middleIndex)
	{
		GameSettings instance = GameSettings.Instance;
		int xPLevelAbsolute = XPManager.Instance.GetXPLevelAbsolute();
		int num = (mCurrentIndex + (mCurrentData.Length - middleIndex)) % mCurrentData.Length;
		for (int i = 0; i < items.Count; i++)
		{
			int num2 = (num + i) % mCurrentData.Length;
			bool locked = false;
			bool pro = false;
			if (mCurrentData[num2].Identifier != PerkType.None)
			{
				locked = IsPerkLocked(instance, xPLevelAbsolute, num2);
				PerkStatus perkStatus = StatsManager.Instance.PerksManager().GetPerkStatus(mCurrentData[num2].Identifier);
				pro = mCurrentData[num2].ProXPTarget <= perkStatus.ProXP || instance.WasProPerkUnlockedEarly(mCurrentData[num2].Identifier);
			}
			PopulateCarouselItemWithPerkData(items[i], mCurrentData[num2], num2, locked, pro);
			if (i == middleIndex)
			{
				PerksPanel.Setup(mCurrentData[num2], locked, pro);
			}
		}
	}

	private void SetDataToCategory(int category)
	{
		StatsManager instance = StatsManager.Instance;
		mCategory = category;
		mCurrentIndex = 0;
		CategoriesSideMenuPanel.SetSelected(mCategory);
		Perk[] perksForTier = instance.PerksList.GetPerksForTier(mCategory + 1);
		mCurrentData = new Perk[perksForTier.Length + 1];
		perksForTier.CopyTo(mCurrentData, 0);
		mCurrentData[perksForTier.Length] = new Perk();
	}

	private void PopulateCarouselItemWithPerkData(CarouselItem carouselItem, Perk perkData, int index, bool locked, bool pro)
	{
		bool flag = perkData.Identifier == PerkType.None;
		PerkIconController componentInChildren = carouselItem.GetComponentInChildren<PerkIconController>();
		if (componentInChildren != null)
		{
			componentInChildren.Hide(flag);
			if (!flag)
			{
				componentInChildren.SetPerk(perkData.Identifier, pro, !locked);
			}
			componentInChildren.Hide(flag);
		}
		PackedSprite[] componentsInChildren = carouselItem.GetComponentsInChildren<PackedSprite>();
		PackedSprite[] array = componentsInChildren;
		foreach (PackedSprite packedSprite in array)
		{
			if (packedSprite != null)
			{
				if (packedSprite.name == "Locked")
				{
					packedSprite.Hide(!locked);
				}
				else if (packedSprite.name == "None")
				{
					packedSprite.Hide(!flag);
				}
			}
		}
	}

	public override void OnScreen()
	{
		base.OnScreen();
		if (mPerkPanel != null)
		{
			mPerkPanel.SetPerkHighlighted(mEditingSlot);
		}
		CheckForLockedSlot();
	}

	public override void OffScreen()
	{
		base.OffScreen();
		Vector3 zero = Vector3.zero;
		if (!(mPerkPanel != null) || !(mPerkPanel.transform.parent != null))
		{
			return;
		}
		if (FrontEndController.Instance.ActiveScreen == ScreenID.SquadLoadOut)
		{
			zero = mPerkPanel.transform.parent.localPosition;
			mPerkPanel.gameObject.MoveTo(zero, 0.2f, 0f, EaseType.easeInOutCubic);
			CommonBackgroundBox componentInChildren = mPerkPanel.GetComponentInChildren<CommonBackgroundBox>();
			if (componentInChildren != null)
			{
				componentInChildren.Resize();
			}
		}
		else
		{
			mPerkPanel.gameObject.MoveTo(mOffScreenLeftOffset, 0.2f, 0f, EaseType.easeInOutCubic);
		}
		mPerkPanel.ClearHighlights();
	}

	public void HardCurrencySelected()
	{
		if (mCurrentData[mCurrentIndex].Identifier != PerkType.None)
		{
			PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
			if (instance != null && mData != null)
			{
				MenuSFX.Instance.SelectEquip.Play2D();
				mData.PerkItem = mCurrentData[mCurrentIndex];
				mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Perk;
				instance.Purchase(mData);
			}
		}
		else
		{
			MenuSFX.Instance.BuyInsufficient.Play2D();
			ClearPerkForSlot();
		}
	}

	public void ClearPerkForSlot()
	{
		GameSettings instance = GameSettings.Instance;
		instance.ClearPerk(mEditingSlot);
		RefreshScreen();
	}

	public void ProPerkNowButtonPress()
	{
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		if (instance != null && mData != null)
		{
			mData.PerkItem = mCurrentData[mCurrentIndex];
			mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.PerkUpgrade;
			mData.NumItems = 1;
			instance.Purchase(mData);
		}
	}

	public void RefreshPerkDataWhenOffScreen(MenuScreenBlade blade, BladeTransition type)
	{
		if (base.IsActive && !base.IsTransitioning)
		{
			PerksPanel.Activate();
			RefreshScreen();
		}
	}

	private void CheckForLockedSlot()
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null && instance.PerkSlotLocked(mEditingSlot))
		{
			SwrveEventsUI.ViewedLockedSlot();
			PurchaseFlowHelper instance2 = PurchaseFlowHelper.Instance;
			if (instance2 != null && mData != null)
			{
				mData.PerkItem = null;
				mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.PerkSlot;
				mData.NumItems = 1;
				instance2.Purchase(mData);
			}
		}
	}

	private void RefreshScreen()
	{
		GameSettings instance = GameSettings.Instance;
		if (mCurrentData[mCurrentIndex].Identifier != PerkType.None)
		{
			int xPLevelAbsolute = XPManager.Instance.GetXPLevelAbsolute();
			bool locked = IsPerkLocked(instance, xPLevelAbsolute, mCurrentIndex);
			PerkStatus perkStatus = StatsManager.Instance.PerksManager().GetPerkStatus(mCurrentData[mCurrentIndex].Identifier);
			bool pro = mCurrentData[mCurrentIndex].ProXPTarget <= perkStatus.ProXP || instance.WasProPerkUnlockedEarly(mCurrentData[mCurrentIndex].Identifier);
			PerksPanel.Setup(mCurrentData[mCurrentIndex], locked, pro);
		}
		CarouselControl.Refresh();
		int num = GameSettings.Instance.FindFirstFreePerkSlot();
		if (num != -1)
		{
			mEditingSlot = num;
		}
		mPerkPanel.HighlightedItem = mEditingSlot;
		if (mData != null)
		{
			mData.SlotIndex = mEditingSlot;
		}
		mPerkPanel.Refresh();
	}

	private bool IsPerkLocked(GameSettings settings, int xpLevel, int dataIndex)
	{
		return (mCurrentData[dataIndex].UnlockLevel > xpLevel || mCurrentData[dataIndex].UnlockLevel == -1) && !settings.WasUnlockedEarly(mCurrentData[dataIndex].Identifier.ToString());
	}
}
