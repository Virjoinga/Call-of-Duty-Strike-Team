public class LoadoutArmourMenuController : MenuScreenBlade
{
	public LoadoutArmourDetailsPanel ArmourPanel;

	private LoadoutArmourPanel mArmourPanel;

	private MenuScreenBlade mArmourPanelBlade;

	private PurchaseFlowHelper.PurchaseData mData;

	private EquipmentDescriptor mNextArmour;

	public MenuScreenBlade ArmourPanelBlade
	{
		set
		{
			mArmourPanelBlade = value;
			mArmourPanel = mArmourPanelBlade.GetComponentInChildren<LoadoutArmourPanel>();
		}
	}

	public override void Awake()
	{
		base.Awake();
		mData = new PurchaseFlowHelper.PurchaseData();
		mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.ArmourUpgrade;
		mData.ScriptToCallWithResult = this;
		mData.MethodToCallWithResult = "RefreshScreen";
		mData.ConfirmPurchase = true;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		GameSettings instance = GameSettings.Instance;
		if (instance != null)
		{
			mNextArmour = instance.GetNextArmourUpgrade();
		}
	}

	public override void OnScreen()
	{
		base.OnScreen();
		RefreshScreen();
	}

	public void UpdateArmourSelected()
	{
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		if (instance != null && mData != null)
		{
			mData.EquipmentItem = mNextArmour;
			mData.NumItems = 1;
			instance.Purchase(mData);
		}
	}

	private void RefreshScreen()
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null)
		{
			mNextArmour = instance.GetNextArmourUpgrade();
		}
		ArmourPanel.Setup(instance.Armour, mNextArmour);
		mArmourPanel.Refresh();
	}
}
