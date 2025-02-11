using System.Collections.Generic;
using UnityEngine;

public class LoadoutSoldierMenuController : MenuScreenBlade, CarouselDataSource
{
	private enum Category
	{
		AssaultRifles = 0,
		MachineGuns = 1,
		SniperRifles = 2,
		Shotguns = 3,
		SMGs = 4,
		None = -1
	}

	public LoadoutWeaponPanel WeaponPanel;

	public LoadoutWeaponStatsPanel WeaponStatsPanel;

	public LoadoutSideMenu CategoriesSideMenuPanel;

	public LoadoutSideMenu InformationSideMenuPanel;

	public Carousel CarouselControl;

	private MenuScreenBlade mSoldierPanelBlade;

	private LoadoutSoldierPanel mSoldierPanel;

	private MissionData mCurrentMission;

	private Transform mSoldierPanelPosition;

	private WeaponDescriptor[] mCurrentData;

	private WeaponDescriptor mCurrentMaxData;

	private Category mCategory;

	private PurchaseFlowHelper.PurchaseData mData;

	private int mCurrentIndex;

	private int mEditingIndex;

	public MenuScreenBlade SoldierPanelBlade
	{
		set
		{
			mSoldierPanelBlade = value;
			mSoldierPanel = mSoldierPanelBlade.GetComponentInChildren<LoadoutSoldierPanel>();
		}
	}

	public override void Awake()
	{
		base.Awake();
		mEditingIndex = -1;
		mSoldierPanelPosition = base.transform.FindChild("SoldierPanelPosition");
		MissionListings.eMissionID currentMissionID = ActStructure.Instance.CurrentMissionID;
		mCurrentMission = MissionListings.Instance.Mission(currentMissionID);
		mData = new PurchaseFlowHelper.PurchaseData();
		mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Weapon;
		mData.ScriptToCallWithResult = this;
		mData.MethodToCallWithResult = "RefreshScreen";
		mCategory = Category.None;
	}

	public void Editing(int index)
	{
		mEditingIndex = index;
		HighlightWeapon();
	}

	protected override void OnActivate()
	{
		GameSettings instance = GameSettings.Instance;
		bool auto = false;
		if (mData != null)
		{
			mData.SoldierIndex = mEditingIndex;
		}
		if (mSoldierPanel != null)
		{
			mSoldierPanel.Setup(mEditingIndex, instance.Soldiers[mEditingIndex], mCurrentMission, auto);
			mSoldierPanel.gameObject.MoveTo(mSoldierPanelPosition.localPosition, 0.2f, 0f, EaseType.easeInOutCubic);
			CommonBackgroundBox componentInChildren = mSoldierPanel.GetComponentInChildren<CommonBackgroundBox>();
			if (componentInChildren != null)
			{
				componentInChildren.Resize();
			}
		}
		MessageBoxController instance2 = MessageBoxController.Instance;
		if (instance2 != null)
		{
			instance2.DoLoadoutWeaponDialogue();
		}
		CarouselControl.SetDataSource(this);
		CarouselControl.Refresh();
		WeaponStatsPanel.gameObject.SetActive(true);
	}

	public void OnSelectedAssaultRifles()
	{
		if (mCategory != 0)
		{
			mCurrentData = WeaponManager.Instance.AssaultRifles;
			if (WeaponPanel.IsActive || WeaponPanel.IsTransitioningOn)
			{
				WeaponPanel.Deactivate(RefreshWeaponDataWhenOffScreen);
			}
			mCurrentIndex = 0;
			mCategory = Category.AssaultRifles;
			CategoriesSideMenuPanel.SetSelected((int)mCategory);
		}
	}

	public void OnSelectedLightMachineGuns()
	{
		if (mCategory != Category.MachineGuns)
		{
			mCurrentData = WeaponManager.Instance.LightMachineGuns;
			if (WeaponPanel.IsActive || WeaponPanel.IsTransitioningOn)
			{
				WeaponPanel.Deactivate(RefreshWeaponDataWhenOffScreen);
			}
			mCurrentIndex = 0;
			mCategory = Category.MachineGuns;
			CategoriesSideMenuPanel.SetSelected((int)mCategory);
		}
	}

	public void OnSelectedSniperRifles()
	{
		if (mCategory != Category.SniperRifles)
		{
			mCurrentData = WeaponManager.Instance.SniperRifles;
			if (WeaponPanel.IsActive || WeaponPanel.IsTransitioningOn)
			{
				WeaponPanel.Deactivate(RefreshWeaponDataWhenOffScreen);
			}
			mCurrentIndex = 0;
			mCategory = Category.SniperRifles;
			CategoriesSideMenuPanel.SetSelected((int)mCategory);
		}
	}

	public void OnSelectedShotguns()
	{
		if (mCategory != Category.Shotguns)
		{
			mCurrentData = WeaponManager.Instance.Shotguns;
			if (WeaponPanel.IsActive || WeaponPanel.IsTransitioningOn)
			{
				WeaponPanel.Deactivate(RefreshWeaponDataWhenOffScreen);
			}
			mCurrentIndex = 0;
			mCategory = Category.Shotguns;
			CategoriesSideMenuPanel.SetSelected((int)mCategory);
		}
	}

	public void OnSelectedSMGs()
	{
		if (mCategory != Category.SMGs)
		{
			mCurrentData = WeaponManager.Instance.SMGs;
			if (WeaponPanel.IsActive || WeaponPanel.IsTransitioningOn)
			{
				WeaponPanel.Deactivate(RefreshWeaponDataWhenOffScreen);
			}
			mCurrentIndex = 0;
			mCategory = Category.SMGs;
			CategoriesSideMenuPanel.SetSelected((int)mCategory);
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
		WeaponManager instance2 = WeaponManager.Instance;
		if (instance == null || instance2 == null)
		{
			return;
		}
		int xPLevelAbsolute = XPManager.Instance.GetXPLevelAbsolute();
		int num = (mCurrentIndex + (mCurrentData.Length - middleIndex)) % mCurrentData.Length;
		for (int i = 0; i < items.Count; i++)
		{
			int num2 = (num + i) % mCurrentData.Length;
			bool locked = instance2.IsWeaponLocked(mCurrentData[num2], xPLevelAbsolute);
			PopulateCarouselItemWithWeaponData(items[i], mCurrentData[num2], num2, locked);
			if (i == middleIndex && instance.Soldiers.Length > mEditingIndex)
			{
				WeaponPanel.Setup(mCurrentData[num2], instance.Soldiers[mEditingIndex].Weapon.Descriptor, locked);
			}
		}
		if (WeaponStatsPanel != null)
		{
			WeaponStatsPanel.Populate(mCurrentData[mCurrentIndex], instance.Soldiers[mEditingIndex].Weapon.Descriptor);
		}
	}

	private void PopulateCarouselItemWithWeaponData(CarouselItem carouselItem, WeaponDescriptor weaponData, int index, bool locked)
	{
		int childCount = carouselItem.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = carouselItem.transform.GetChild(i);
			if (child.name == "WeaponIconSmall")
			{
				WeaponIconController component = child.GetComponent<WeaponIconController>();
				if (component != null)
				{
					component.SetWeapon(weaponData.Type, !locked);
				}
			}
			else if (child.name == "lockedIcon_image")
			{
				child.gameObject.SetActive(locked);
			}
		}
	}

	public override void OnScreen()
	{
		base.OnScreen();
		TitleBarController.Instance.HighlightBackButton(true);
	}

	public override void OffScreen()
	{
		base.OffScreen();
		Vector3 zero = Vector3.zero;
		if (!(mSoldierPanel != null) || !(mSoldierPanel.transform.parent != null))
		{
			return;
		}
		if (FrontEndController.Instance.ActiveScreen == ScreenID.SquadLoadOut)
		{
			zero = mSoldierPanel.transform.parent.localPosition;
			mSoldierPanel.gameObject.MoveTo(zero, 0.2f, 0f, EaseType.easeInOutCubic);
			CommonBackgroundBox componentInChildren = mSoldierPanel.GetComponentInChildren<CommonBackgroundBox>();
			if (componentInChildren != null)
			{
				componentInChildren.Resize();
			}
		}
		else
		{
			mSoldierPanel.gameObject.MoveTo(mOffScreenLeftOffset, 0.2f, 0f, EaseType.easeInOutCubic);
		}
	}

	public void HardCurrencySelected()
	{
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		if (instance != null && mData != null)
		{
			mData.WeaponItem = mCurrentData[mCurrentIndex];
			instance.Purchase(mData);
		}
	}

	public void RefreshWeaponDataWhenOffScreen(MenuScreenBlade blade, BladeTransition type)
	{
		if (base.IsActive && !base.IsTransitioning)
		{
			CarouselControl.Refresh();
			WeaponPanel.Activate();
		}
	}

	private void RefreshScreen()
	{
		GameSettings instance = GameSettings.Instance;
		WeaponManager instance2 = WeaponManager.Instance;
		if (!(instance == null) && !(instance2 == null))
		{
			bool auto = false;
			int xPLevelAbsolute = XPManager.Instance.GetXPLevelAbsolute();
			WeaponDescriptor descriptor = instance.Soldiers[mEditingIndex].Weapon.Descriptor;
			WeaponPanel.Setup(mCurrentData[mCurrentIndex], descriptor, instance2.IsWeaponLocked(descriptor, xPLevelAbsolute));
			CarouselControl.Refresh();
			if (mSoldierPanel != null)
			{
				mSoldierPanel.Setup(mEditingIndex, instance.Soldiers[mEditingIndex], mCurrentMission, auto);
			}
		}
	}

	private void HighlightWeapon()
	{
		GameSettings instance = GameSettings.Instance;
		WeaponManager instance2 = WeaponManager.Instance;
		if (!(instance != null) || !(instance2 != null))
		{
			return;
		}
		WeaponDescriptor descriptor = instance.Soldiers[mEditingIndex].Weapon.Descriptor;
		int index = 0;
		bool flag = false;
		if (!(descriptor != null))
		{
			return;
		}
		if (IsWeaponInList(descriptor, instance2.AssaultRifles, out index))
		{
			if (mCategory != 0)
			{
				OnSelectedAssaultRifles();
			}
			else
			{
				flag = true;
			}
			mCurrentIndex = index;
		}
		else if (IsWeaponInList(descriptor, instance2.LightMachineGuns, out index))
		{
			if (mCategory != Category.MachineGuns)
			{
				OnSelectedLightMachineGuns();
			}
			else
			{
				flag = true;
			}
			mCurrentIndex = index;
		}
		else if (IsWeaponInList(descriptor, instance2.Shotguns, out index))
		{
			if (mCategory != Category.Shotguns)
			{
				OnSelectedShotguns();
			}
			else
			{
				flag = true;
			}
			mCurrentIndex = index;
		}
		else if (IsWeaponInList(descriptor, instance2.SniperRifles, out index))
		{
			if (mCategory != Category.SniperRifles)
			{
				OnSelectedSniperRifles();
			}
			else
			{
				flag = true;
			}
			mCurrentIndex = index;
		}
		else if (IsWeaponInList(descriptor, instance2.SMGs, out index))
		{
			if (mCategory != Category.SMGs)
			{
				OnSelectedSMGs();
			}
			else
			{
				flag = true;
			}
			mCurrentIndex = index;
		}
		if (flag)
		{
			RefreshScreen();
		}
	}

	private bool IsWeaponInList(WeaponDescriptor weapon, WeaponDescriptor[] list, out int index)
	{
		bool result = false;
		index = 0;
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i] == weapon)
			{
				result = true;
				index = i;
				break;
			}
		}
		return result;
	}
}
