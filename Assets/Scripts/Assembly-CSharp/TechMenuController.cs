using UnityEngine;

public class TechMenuController : MenuScreenBlade
{
	private const float TITLE_ITEM_WIDTH = 464f;

	private const float TITLE_ITEM_HEIGHT = 24f;

	private const float TITLE_X_OFFSET = 2f;

	private const float SHOP_ITEM_WIDTH = 464f;

	private const float SHOP_ITEM_HEIGHT = 147f;

	private const float BRACKETS_LEFT_BORDER = 11f;

	private const float BRACKETS_RIGHT_BORDER = 9f;

	private const float BRACKETS_Y_BORDER = 10f;

	private const float TEXT_COUNT_TIME = 0.5f;

	private const int MAX_ON_SCREEN_ITEMS = 4;

	public Transform Header;

	public Transform Footer;

	public UIButton InputBlocker;

	public CountUpText PlayerHardFunds;

	public CountUpText PlayerSoftFunds;

	public UIScrollList ScrollList;

	public UIListItemContainer ShopItemPrefab;

	private EquipmentDescriptor mEquipmentToDrop;

	private TechPurchaseEventArgs mArgs;

	private float totalHeight;

	private float titleItemWidth;

	private float titleItemHeight;

	private float titleXOffset;

	private float shopItemWidth;

	private float shopItemHeight;

	private float bracketsLeftBorder;

	private float bracketsRightBorder;

	private float bracketsYBorder;

	public TechPurchaseEventArgs Args
	{
		set
		{
			mArgs = value;
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (GameController.Instance != null)
		{
			GameController.Instance.LinkTechMenuController(this);
		}
	}

	public void Start()
	{
		LayoutComponents();
		PopulateWithSupplies();
	}

	public override void Activate()
	{
		base.Activate();
		RefreshPlayerFunds(0.2f);
	}

	private void RefreshPlayerFunds(float countTime)
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null && PlayerHardFunds != null && PlayerSoftFunds != null)
		{
			PlayerHardFunds.CountTo(instance.PlayerCash().HardCash(), countTime);
			PlayerSoftFunds.CountTo(instance.PlayerCash().SoftCash(), countTime);
		}
	}

	private void LayoutComponents()
	{
		titleItemWidth = 464f;
		titleItemHeight = 24f;
		shopItemWidth = 464f;
		shopItemHeight = 147f;
		bracketsLeftBorder = 11f;
		bracketsRightBorder = 9f;
		bracketsYBorder = 10f;
		titleXOffset = 2f;
		if (TBFUtils.IsRetinaHdDevice())
		{
			titleItemWidth *= 2f;
			titleItemHeight *= 2f;
			shopItemWidth *= 2f;
			shopItemHeight *= 2f;
			bracketsLeftBorder *= 2f;
			bracketsRightBorder *= 2f;
			bracketsYBorder *= 2f;
			titleXOffset *= 2f;
		}
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		float num2 = (float)Screen.width * num;
		float num3 = (float)Screen.height * num;
		InputBlocker.SetSize(num2, num3);
		BoxCollider component = InputBlocker.GetComponent<BoxCollider>();
		component.size = new Vector3(num2, num3, 1f);
	}

	private void LayoutTitle(Transform titleRoot, float width, float height, float yOffset)
	{
		Scale9Grid componentInChildren = titleRoot.GetComponentInChildren<Scale9Grid>();
		if (componentInChildren != null)
		{
			componentInChildren.size.x = titleItemWidth + bracketsLeftBorder + bracketsRightBorder;
			componentInChildren.size.y = titleItemHeight + bracketsYBorder * 2f;
			componentInChildren.Resize();
		}
		Vector3 position = Camera.main.WorldToScreenPoint(base.transform.position);
		position.x -= bracketsLeftBorder - bracketsRightBorder;
		position.y += yOffset;
		Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
		componentInChildren.transform.position = position2;
		position.x += titleXOffset;
		position2 = Camera.main.ScreenToWorldPoint(position);
		Vector2 boxSize = new Vector2(titleItemWidth - titleXOffset, titleItemHeight);
		CommonBackgroundBoxPlacement[] componentsInChildren = titleRoot.GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		CommonBackgroundBoxPlacement[] array = componentsInChildren;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(position2, boxSize);
		}
	}

	private void PopulateWithSupplies()
	{
		GameSettings instance = GameSettings.Instance;
		WeaponManager instance2 = WeaponManager.Instance;
		EquipmentDescriptor[] supportEquipment = instance2.SupportEquipment;
		int num = supportEquipment.Length;
		float num2 = ((num >= 4) ? (shopItemHeight * 3f) : ((float)num * shopItemHeight));
		float width = titleItemWidth + bracketsLeftBorder + bracketsRightBorder;
		float num3 = titleItemHeight + bracketsYBorder * 2f;
		float num4 = (num2 + num3) * 0.5f;
		LayoutTitle(Header, width, num3, num4);
		LayoutTitle(Footer, width, num3, 0f - num4);
		int num5 = 0;
		num5 = StatsHelper.PlayerXP();
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		XPManager.Instance.ConvertXPToLevel(num5, out level, out prestigeLevel, out xpToNextLevel, out percent);
		ShopItem[] array = new ShopItem[num];
		if (ScrollList != null)
		{
			ScrollList.viewableArea = new Vector2(titleItemWidth * 1.1f, num2);
			ScrollList.UpdateCamera();
		}
		for (int i = 0; i < num; i++)
		{
			UIListItemContainer uIListItemContainer = (UIListItemContainer)Object.Instantiate(ShopItemPrefab);
			array[i] = uIListItemContainer.GetComponentInChildren<ShopItem>();
			EquipmentDescriptor equipmentDescriptor = supportEquipment[i];
			bool locked = equipmentDescriptor.UnlockLevel > level && !instance.WasUnlockedEarly(equipmentDescriptor.Name);
			array[i].LayoutComponents(shopItemWidth, shopItemHeight, bracketsLeftBorder, bracketsRightBorder, bracketsYBorder);
			array[i].SetToItem(this, equipmentDescriptor, locked);
			if (uIListItemContainer != null && ScrollList != null)
			{
				ScrollList.AddItem(uIListItemContainer);
			}
		}
	}

	private void ItemPurchased(EquipmentDescriptor equipment)
	{
		mEquipmentToDrop = equipment;
		DropItemAtLocation();
		RefreshPlayerFunds(0.5f);
		DelayedDeactivate(0.5f);
	}

	private void DropItemAtLocation()
	{
		mArgs.Obj.ActivateObject(mEquipmentToDrop);
		mEquipmentToDrop = null;
	}
}
