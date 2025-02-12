using System.Collections.Generic;
using UnityEngine;

public class LoadoutEquipmentMenuController : MenuScreenBlade, CarouselDataSource
{
	public LoadoutEquipmentPanel EquipmentPanel;

	public Carousel CarouselControl;

	public LoadoutIncreaseSlotSizePanel IncreaseSlotSizePanel;

	private LoadoutInventoryPanel mInventoryPanel;

	private MenuScreenBlade mInventoryPanelBlade;

	private EquipmentDescriptor[] mCurrentEquipmentData;

	private AnimatedHighlight mHighlight;

	private PurchaseFlowHelper.PurchaseData mData;

	private Transform mInventoryPanelPosition;

	private int mCurrentIndex;

	private int mCurrentlyHighlighted;

	public MenuScreenBlade InventoryPanelBlade
	{
		set
		{
			mInventoryPanelBlade = value;
			mInventoryPanel = mInventoryPanelBlade.GetComponentInChildren<LoadoutInventoryPanel>();
		}
	}

	public override void Awake()
	{
		base.Awake();
		mInventoryPanelPosition = base.transform.Find("InventoryPanelPosition");
		WeaponManager instance = WeaponManager.Instance;
		mCurrentEquipmentData = instance.LoadoutEquipment;
		mData = new PurchaseFlowHelper.PurchaseData();
		mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
		mData.ScriptToCallWithResult = this;
		mData.MethodToCallWithResult = "RefreshScreen";
	}

	public void SetSlot(int highlightSlot)
	{
		if (highlightSlot >= mCurrentEquipmentData.Length)
		{
			highlightSlot = 0;
		}
		mCurrentlyHighlighted = highlightSlot;
		if (base.IsActive)
		{
			mInventoryPanel.HighlightedItem = mCurrentlyHighlighted;
			mCurrentIndex = highlightSlot;
			RefreshScreen();
		}
		if (mData != null)
		{
			mData.SlotIndex = mCurrentlyHighlighted;
		}
	}

	private void Start()
	{
		SetSlot(0);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		CarouselControl.SetDataSource(this);
		CarouselControl.Refresh();
		if (mHighlight == null)
		{
			Transform root = base.transform.root;
			mHighlight = root.GetComponentInChildren<AnimatedHighlight>();
		}
		if (mHighlight != null && mInventoryPanelPosition != null)
		{
			mHighlight.SetHighlightNow(mInventoryPanelPosition.position, Vector2.zero);
		}
		if (mInventoryPanel != null && mInventoryPanelPosition != null)
		{
			mInventoryPanel.gameObject.MoveTo(mInventoryPanelPosition.localPosition, 0.2f, 0f, EaseType.easeInOutCubic);
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (mHighlight != null && mInventoryPanel != null)
		{
			mInventoryPanel.HighlightedItem = -1;
			mHighlight.DismissHighlight();
		}
	}

	public override void OnScreen()
	{
		base.OnScreen();
		SetSlot(mCurrentlyHighlighted);
	}

	public virtual void Next()
	{
		int num = mCurrentEquipmentData.Length;
		mCurrentIndex = (mCurrentIndex + (num - 1)) % num;
		SetSlot(mCurrentIndex);
	}

	public virtual void Previous()
	{
		int num = mCurrentEquipmentData.Length;
		mCurrentIndex = (mCurrentIndex + 1) % num;
		SetSlot(mCurrentIndex);
	}

	public virtual void Populate(List<CarouselItem> items, int middleIndex)
	{
		GameSettings instance = GameSettings.Instance;
		int xPLevelAbsolute = XPManager.Instance.GetXPLevelAbsolute();
		int num = mCurrentEquipmentData.Length;
		int num2 = (mCurrentIndex + (num - middleIndex)) % num;
		for (int i = 0; i < items.Count; i++)
		{
			int num3 = (num2 + i) % num;
			bool locked = mCurrentEquipmentData[num3].UnlockLevel > xPLevelAbsolute && !instance.WasUnlockedEarly(mCurrentEquipmentData[num3].Name);
			PopulateCarouselItemWithItemEquipmentData(items[i], mCurrentEquipmentData[num3], num3, locked);
			if (i == middleIndex)
			{
				int num4 = instance.SpaceForEquipment(mCurrentEquipmentData[num3]);
				int fillPrice = instance.CalculateCostOfEquipment(mCurrentEquipmentData[num3], num4);
				EquipmentPanel.SetupForEquipment(mCurrentEquipmentData[num3], num4, fillPrice, locked);
			}
		}
	}

	private void PopulateCarouselItemWithItemEquipmentData(CarouselItem carouselItem, EquipmentDescriptor item, int index, bool locked)
	{
		int childCount = carouselItem.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = carouselItem.transform.GetChild(i);
			if (!(child != null))
			{
				continue;
			}
			if (child.name == "EquipmentIconSmall")
			{
				EquipmentIconController component = child.GetComponent<EquipmentIconController>();
				if (component != null)
				{
					component.Hide(locked);
					component.SetEquipment(item.Type, !locked);
				}
			}
			else if (child.name == "Locked")
			{
				child.gameObject.SetActive(locked);
			}
		}
	}

	public void RefreshEquipmentDataWhenOffScreen(BladeTransition type)
	{
		if (base.IsActive && !base.IsTransitioning)
		{
			CarouselControl.Refresh();
			EquipmentPanel.Activate();
		}
	}

	public override void OffScreen()
	{
		base.OffScreen();
		Vector3 zero = Vector3.zero;
		if (mInventoryPanel != null && mInventoryPanel.transform.parent != null)
		{
			if (FrontEndController.Instance.ActiveScreen == ScreenID.SquadLoadOut)
			{
				zero = mInventoryPanel.transform.parent.localPosition;
				mInventoryPanel.gameObject.MoveTo(zero, 0.2f, 0f, EaseType.easeInOutCubic);
			}
			else
			{
				mInventoryPanel.gameObject.MoveTo(mOffScreenLeftOffset, 0.2f, 0f, EaseType.easeInOutCubic);
			}
		}
	}

	public void PurchaseOneItemSelected()
	{
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		if (instance != null && mData != null)
		{
			mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
			mData.EquipmentItem = mCurrentEquipmentData[mCurrentIndex];
			mData.NumItems = 1;
			mData.ConfirmPurchase = false;
			instance.Purchase(mData);
		}
	}

	public void PurchaseEnoughToFillSlotSelected()
	{
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		GameSettings instance2 = GameSettings.Instance;
		if (instance != null && instance2 != null && mData != null)
		{
			int numItems = instance2.SpaceForEquipment(mCurrentEquipmentData[mCurrentIndex]);
			mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
			mData.EquipmentItem = mCurrentEquipmentData[mCurrentIndex];
			mData.NumItems = numItems;
			mData.ConfirmPurchase = false;
			instance.Purchase(mData);
		}
	}

	public void IncreaseCurrentSlotSizePressed()
	{
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		GameSettings instance2 = GameSettings.Instance;
		if (instance != null && instance2 != null && mData != null)
		{
			mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.EquipmentSlot;
			mData.EquipmentItem = mCurrentEquipmentData[mCurrentIndex];
			mData.NumItems = instance2.CalculateSlotIncrease(mCurrentlyHighlighted);
			mData.SlotSizeLevel = instance2.CurrentSlotsSizeLevel(mCurrentlyHighlighted);
			mData.ConfirmPurchase = true;
			instance.Purchase(mData);
		}
	}

	private void RefreshScreen()
	{
		GameSettings instance = GameSettings.Instance;
		int xPLevelAbsolute = XPManager.Instance.GetXPLevelAbsolute();
		bool locked = mCurrentEquipmentData[mCurrentIndex].UnlockLevel > xPLevelAbsolute && !instance.WasUnlockedEarly(mCurrentEquipmentData[mCurrentIndex].Name);
		int num = instance.SpaceForEquipment(mCurrentEquipmentData[mCurrentIndex]);
		int fillPrice = instance.CalculateCostOfEquipment(mCurrentEquipmentData[mCurrentIndex], num);
		EquipmentPanel.SetupForEquipment(mCurrentEquipmentData[mCurrentIndex], num, fillPrice, locked);
		CarouselControl.Refresh();
		mInventoryPanel.Refresh();
		IncreaseSlotSizePanel.SetSlot(mCurrentlyHighlighted);
	}
}
