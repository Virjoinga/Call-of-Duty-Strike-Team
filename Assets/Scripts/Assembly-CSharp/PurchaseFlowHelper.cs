using UnityEngine;

public class PurchaseFlowHelper : MonoBehaviour
{
	public class PurchaseData
	{
		public enum PurchaseType
		{
			Weapon = 0,
			Perk = 1,
			Equipment = 2,
			EquipmentSlot = 3,
			PerkSlot = 4,
			ArmourUpgrade = 5,
			PerkUpgrade = 6,
			Bundles = 7,
			KiaSoldier = 8,
			Mission = 9
		}

		public MonoBehaviour ScriptToCallWithResult;

		public string MethodToCallWithResult;

		public BundleDescriptor[] Bundles;

		public WeaponDescriptor WeaponItem;

		public EquipmentDescriptor EquipmentItem;

		public Perk PerkItem;

		public SectionData Section;

		public int SoldierIndex;

		public int SlotIndex;

		public int NumItems;

		public int SlotSizeLevel;

		public PurchaseType Type;

		public bool UseInGamePrice;

		public bool ConfirmPurchase;
	}

	private PurchaseData mData;

	private int mCachedRequiredFunds;

	private static PurchaseFlowHelper mInstance;

	public static PurchaseFlowHelper Instance
	{
		get
		{
			return mInstance;
		}
	}

	public PurchaseData PurchaseInProgress { get; set; }

	public int RequiredFunds
	{
		get
		{
			return mCachedRequiredFunds;
		}
	}

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
		}
		else
		{
			Debug.LogError("Cannot have more than one instance of PurchaseFlowHelper");
		}
	}

	public void Purchase(PurchaseData data)
	{
		if (data == null)
		{
			return;
		}
		MessageBoxController instance = MessageBoxController.Instance;
		GameSettings instance2 = GameSettings.Instance;
		mData = data;
		mCachedRequiredFunds = 0;
		if (IsUnlocked())
		{
			Cash cash = instance2.PlayerCash();
			mCachedRequiredFunds = HardCost();
			if (mCachedRequiredFunds <= cash.HardCash())
			{
				if (mData.ConfirmPurchase && instance != null)
				{
					instance.DoPurchaseNowDialogue(this, ItemName(), mCachedRequiredFunds, "MessageBoxResultPurchaseNow");
				}
				else
				{
					PurchaseNow();
				}
			}
			else if (!(instance != null))
			{
			}
		}
		else if (mData.Type != 0 && (mData.Type != PurchaseData.PurchaseType.Perk || mData.PerkItem.UnlockLevel != -1))
		{
			mCachedRequiredFunds = UnlockCost();
			if (mCachedRequiredFunds <= instance2.PlayerCash().HardCash() && instance != null)
			{
				instance.DoUnlockNowDialogue(this, ItemName(), UnlockCost(), "MessageBoxResultUnlockNow");
			}
		}
	}

	public void InGamePurchase(PurchaseData data, bool endGameOnFail)
	{
		if (data == null)
		{
			return;
		}
		GameSettings instance = GameSettings.Instance;
		MessageBoxController instance2 = MessageBoxController.Instance;
		mData = data;
		if (!(instance2 != null) || !(instance != null))
		{
			return;
		}
		Cash cash = instance.PlayerCash();
		bool flag = HardCost() <= cash.HardCash();
		string equipment = mData.EquipmentItem.Name + "_PLURAL";
		if (flag)
		{
			string cancelAction = string.Empty;
			if (endGameOnFail)
			{
				cancelAction = "MessageBoxResultForceEndGame";
			}
			instance2.DoPurchaseDialogue(this, "MessageBoxResultAttemptPurchase", cancelAction, equipment, HardCost(), mData.NumItems, cash.HardCash());
		}
	}

	private void MessageBoxResultAttemptPurchase()
	{
		if (mData != null)
		{
			Purchase(mData);
		}
	}

	private void MessageBoxResultForceEndGame()
	{
		GameplayController instance = GameplayController.instance;
		if (instance != null)
		{
			instance.SuicideSquad();
		}
	}

	private void AnalyticsLogPurchase(PurchaseData data)
	{
		SwrveUserData.Instance.LogPurchase(data);
		bool flag = true;
		string itemName = "Unknown";
		switch (data.Type)
		{
		case PurchaseData.PurchaseType.Weapon:
			itemName = data.WeaponItem.Type.ToString();
			break;
		case PurchaseData.PurchaseType.Perk:
		{
			Perk perk = StatsManager.Instance.PerksManager().GetPerk(data.PerkItem.Identifier);
			SwrveEventsPurchase.HardCurrencyPurchase((ulong)UnlockCost(), 1uL, "Perk_" + perk.Tier.ToString() + "_" + perk.Index + "_" + perk.Identifier);
			EventHub.Instance.Report(new Events.PerkUnlocked(mData.PerkItem.Identifier));
			itemName = data.PerkItem.Identifier.ToString();
			break;
		}
		case PurchaseData.PurchaseType.Equipment:
			MenuSFX.Instance.ItemUnlock.Play2D();
			itemName = data.EquipmentItem.name;
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.Grenade)
			{
				SwrveEventsPurchase.GrenadePurchase(data.NumItems);
			}
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.Claymore)
			{
				SwrveEventsPurchase.ClaymorePurchase(data.NumItems);
			}
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.MediPack)
			{
				SwrveEventsPurchase.HealthKitPurchase(data.NumItems);
			}
			if (data.EquipmentItem.Type == EquipmentIconController.EquipmentType.AmmoPack)
			{
				SwrveEventsPurchase.AmmoPackPurchase(HardCost());
			}
			break;
		case PurchaseData.PurchaseType.EquipmentSlot:
			MenuSFX.Instance.ItemUnlock.Play2D();
			itemName = "SlotIncrease_" + data.EquipmentItem.name + "_" + (data.SlotSizeLevel + 1);
			SwrveEventsPurchase.SlotIncrease(data.EquipmentItem.Type, data.SlotSizeLevel + 1);
			break;
		case PurchaseData.PurchaseType.PerkSlot:
			MenuSFX.Instance.ItemUnlock.Play2D();
			itemName = "PerkSlotPurchase";
			SwrveEventsPurchase.HardCurrencyPurchase((ulong)UnlockCost(), (ulong)data.NumItems, itemName);
			SwrveEventsPurchase.PerkSlotPurchase();
			break;
		case PurchaseData.PurchaseType.ArmourUpgrade:
			MenuSFX.Instance.ItemUnlock.Play2D();
			itemName = "ArmourUpgrade_" + GetArmourLevelFromEquipmentType(data.EquipmentItem.Armour);
			SwrveEventsPurchase.ArmourIncrease(GetArmourLevelFromEquipmentType(data.EquipmentItem.Armour));
			break;
		case PurchaseData.PurchaseType.PerkUpgrade:
		{
			Perk perk2 = StatsManager.Instance.PerksManager().GetPerk(data.PerkItem.Identifier);
			SwrveEventsPurchase.HardCurrencyPurchase((ulong)UnlockCost(), 1uL, "ProPerk_" + perk2.Tier.ToString() + "_" + perk2.Index + "_" + perk2.Identifier);
			MenuSFX.Instance.ItemUnlock.Play2D();
			itemName = "PerkUpgrade";
			break;
		}
		case PurchaseData.PurchaseType.Bundles:
		{
			MenuSFX.Instance.BuyConfirm.Play2D();
			flag = false;
			if (data.Bundles.Length == WeaponManager.Instance.Bundles.Length)
			{
				SwrveEventsPurchase.BundlePurchase("complete");
				break;
			}
			for (int i = 0; i < data.Bundles.Length; i++)
			{
				SwrveEventsPurchase.HardCurrencyPurchase((ulong)HardCost(), 1uL, data.Bundles[i].name);
				SwrveEventsPurchase.BundlePurchase(data.Bundles[i].name);
			}
			break;
		}
		case PurchaseData.PurchaseType.KiaSoldier:
			MenuSFX.Instance.ItemUnlock.Play2D();
			itemName = "SoldierPurchase";
			flag = false;
			break;
		case PurchaseData.PurchaseType.Mission:
			MenuSFX.Instance.ItemUnlock.Play2D();
			itemName = "MissionPurchase";
			flag = false;
			break;
		}
		if (HardCost() > 0 && flag)
		{
			SwrveEventsPurchase.HardCurrencyPurchase((ulong)HardCost(), (ulong)data.NumItems, itemName);
		}
	}

	private int GetArmourLevelFromEquipmentType(EquipmentDescriptor.ArmourType type)
	{
		int result = 0;
		switch (type)
		{
		case EquipmentDescriptor.ArmourType.Level1:
			result = 1;
			break;
		case EquipmentDescriptor.ArmourType.Level2:
			result = 2;
			break;
		case EquipmentDescriptor.ArmourType.Level3:
			result = 3;
			break;
		case EquipmentDescriptor.ArmourType.Level4:
			result = 4;
			break;
		}
		return result;
	}

	private void PurchaseNow()
	{
		EquipItem();
		AdjustCash();
		mData.ScriptToCallWithResult.Invoke(mData.MethodToCallWithResult, 0f);
	}

	private void MessageBoxResultPurchaseNow()
	{
		PurchaseNow();
	}

	private void MessageBoxResultUnlockNow()
	{
		MenuSFX.Instance.ItemUnlock.Play2D();
		GameSettings instance = GameSettings.Instance;
		instance.UnlockEarly(ItemCodeName());
		EquipItem();
		instance.PlayerCash().AdjustHardCash(-UnlockCost());
		mData.ScriptToCallWithResult.Invoke(mData.MethodToCallWithResult, 0f);
	}

	private void MessageBoxResultPayWithHardCurrency()
	{
		MenuSFX.Instance.BuyConfirm.Play2D();
		GameSettings instance = GameSettings.Instance;
		instance.UnlockEarly(ItemCodeName());
		EquipItem();
		instance.PlayerCash().AdjustHardCash(-HardCost());
		mData.ScriptToCallWithResult.Invoke(mData.MethodToCallWithResult, 0f);
	}

	private void MessageBoxResultGoToMTX()
	{
		PurchaseInProgress = mData;
		FrontEndController.Instance.TransitionTo(ScreenID.MTXSelect);
	}

	private int UnlockCost()
	{
		int result = 0;
		switch (mData.Type)
		{
		case PurchaseData.PurchaseType.Weapon:
			result = mData.WeaponItem.UnlockCost;
			break;
		case PurchaseData.PurchaseType.Perk:
			result = mData.PerkItem.UnlockCost;
			break;
		case PurchaseData.PurchaseType.Equipment:
			result = mData.EquipmentItem.UnlockCost;
			break;
		case PurchaseData.PurchaseType.PerkUpgrade:
			result = mData.PerkItem.ProCost;
			break;
		case PurchaseData.PurchaseType.PerkSlot:
			result = GameSettings.Instance.PerkSlotCost();
			break;
		case PurchaseData.PurchaseType.Mission:
			result = mData.Section.UnlockEarlyCost;
			break;
		}
		return result;
	}

	private int HardCost()
	{
		int result = 0;
		switch (mData.Type)
		{
		case PurchaseData.PurchaseType.Weapon:
		case PurchaseData.PurchaseType.Perk:
		case PurchaseData.PurchaseType.Mission:
			result = 0;
			break;
		case PurchaseData.PurchaseType.Equipment:
		{
			GameSettings instance = GameSettings.Instance;
			result = (mData.UseInGamePrice ? instance.CalculateInGameCostOfEquipment(mData.EquipmentItem, mData.NumItems) : instance.CalculateCostOfEquipment(mData.EquipmentItem, mData.NumItems));
			break;
		}
		case PurchaseData.PurchaseType.EquipmentSlot:
			result = GameSettings.Instance.CalculateCostOfIncrease(mData.EquipmentItem.name, mData.SlotIndex);
			break;
		case PurchaseData.PurchaseType.ArmourUpgrade:
			result = mData.EquipmentItem.HardCost;
			break;
		case PurchaseData.PurchaseType.Bundles:
			result = ((mData.Bundles.Length <= 1) ? mData.Bundles[0].HardCost : GameSettings.Instance.AllBundleCost());
			break;
		case PurchaseData.PurchaseType.KiaSoldier:
			result = SwrveServerVariables.Instance.PurchaseSoliderCost;
			break;
		}
		return result;
	}

	private string ItemName()
	{
		string result = string.Empty;
		switch (mData.Type)
		{
		case PurchaseData.PurchaseType.Weapon:
			result = Language.Get(mData.WeaponItem.Name.ToUpper());
			break;
		case PurchaseData.PurchaseType.Perk:
			result = Language.Get("S_" + mData.PerkItem.Identifier.ToString().ToUpper());
			break;
		case PurchaseData.PurchaseType.Equipment:
		case PurchaseData.PurchaseType.ArmourUpgrade:
			result = Language.Get(mData.EquipmentItem.Name.ToUpper());
			break;
		case PurchaseData.PurchaseType.EquipmentSlot:
			result = Language.Get("S_INCREASED_CAPACITY");
			break;
		case PurchaseData.PurchaseType.PerkSlot:
			result = Language.Get("S_PERK_SLOT");
			break;
		case PurchaseData.PurchaseType.PerkUpgrade:
			result = Language.Get("S_" + mData.PerkItem.Identifier.ToString().ToUpper() + "_PRO");
			break;
		case PurchaseData.PurchaseType.Bundles:
			result = Language.Get((mData.Bundles.Length != 1) ? "S_COMPLETE_BUNDLE" : mData.Bundles[0].Name);
			break;
		case PurchaseData.PurchaseType.KiaSoldier:
			result = Language.Get("S_SOLDIER_XP");
			break;
		case PurchaseData.PurchaseType.Mission:
			result = Language.Get(mData.Section.Name);
			break;
		}
		return result;
	}

	private string ItemCodeName()
	{
		string result = string.Empty;
		switch (mData.Type)
		{
		case PurchaseData.PurchaseType.Weapon:
			result = mData.WeaponItem.Name;
			break;
		case PurchaseData.PurchaseType.Perk:
		case PurchaseData.PurchaseType.PerkUpgrade:
			result = mData.PerkItem.Identifier.ToString();
			break;
		case PurchaseData.PurchaseType.Equipment:
		case PurchaseData.PurchaseType.ArmourUpgrade:
			result = mData.EquipmentItem.Name;
			break;
		case PurchaseData.PurchaseType.EquipmentSlot:
			result = "EquipmentSlot";
			break;
		case PurchaseData.PurchaseType.PerkSlot:
			result = "PerkSlot";
			break;
		case PurchaseData.PurchaseType.Bundles:
			result = "Bundles";
			break;
		case PurchaseData.PurchaseType.KiaSoldier:
			result = "SoldierPurchase";
			break;
		case PurchaseData.PurchaseType.Mission:
			result = "MissionPurchase";
			break;
		}
		return result;
	}

	private bool IsUnlocked()
	{
		bool result = false;
		GameSettings instance = GameSettings.Instance;
		WeaponManager instance2 = WeaponManager.Instance;
		int xPLevelAbsolute = XPManager.Instance.GetXPLevelAbsolute();
		switch (mData.Type)
		{
		case PurchaseData.PurchaseType.Weapon:
			result = !instance2.IsWeaponLocked(mData.WeaponItem, xPLevelAbsolute);
			break;
		case PurchaseData.PurchaseType.Perk:
			result = (mData.PerkItem.UnlockLevel <= xPLevelAbsolute && mData.PerkItem.UnlockLevel != -1) || instance.WasUnlockedEarly(mData.PerkItem.Identifier.ToString());
			break;
		case PurchaseData.PurchaseType.PerkSlot:
			result = !instance.PerkSlotLocked(mData.SlotIndex);
			break;
		case PurchaseData.PurchaseType.PerkUpgrade:
		{
			PerkStatus perkStatus = StatsManager.Instance.PerksManager().GetPerkStatus(mData.PerkItem.Identifier);
			result = mData.PerkItem.ProXPTarget <= perkStatus.ProXP || instance.WasProPerkUnlockedEarly(mData.PerkItem.Identifier);
			break;
		}
		case PurchaseData.PurchaseType.Equipment:
			result = mData.EquipmentItem.UnlockLevel <= xPLevelAbsolute || instance.WasUnlockedEarly(mData.EquipmentItem.Name);
			break;
		case PurchaseData.PurchaseType.EquipmentSlot:
		case PurchaseData.PurchaseType.ArmourUpgrade:
		case PurchaseData.PurchaseType.Bundles:
		case PurchaseData.PurchaseType.KiaSoldier:
			result = true;
			break;
		case PurchaseData.PurchaseType.Mission:
			result = mData.Section.Locked && mData.Section.UnlockedAtXpLevel <= xPLevelAbsolute && mData.Section.UnlockedAtXpLevel != -1;
			break;
		}
		return result;
	}

	private void AdjustCash()
	{
		GameSettings instance = GameSettings.Instance;
		Cash cash = instance.PlayerCash();
		cash.AdjustHardCash(-mCachedRequiredFunds);
	}

	private bool IsRoomForItem()
	{
		bool result = true;
		GameSettings instance = GameSettings.Instance;
		PurchaseData.PurchaseType type = mData.Type;
		if (type == PurchaseData.PurchaseType.Equipment && !instance.IsThereSpaceForEquipment(mData.EquipmentItem, mData.NumItems))
		{
			result = false;
		}
		return result;
	}

	private void EquipItem()
	{
		AnalyticsLogPurchase(mData);
		GameSettings instance = GameSettings.Instance;
		switch (mData.Type)
		{
		case PurchaseData.PurchaseType.Weapon:
			instance.SetSoldierWeapon(mData.SoldierIndex, mData.WeaponItem);
			break;
		case PurchaseData.PurchaseType.Perk:
			instance.AddPerk(mData.SlotIndex, mData.PerkItem);
			SwrveUserData.Instance.LogPurchase(mData);
			break;
		case PurchaseData.PurchaseType.Equipment:
			instance.AddEquipment(mData.EquipmentItem, mData.NumItems);
			break;
		case PurchaseData.PurchaseType.EquipmentSlot:
			instance.IncreaseSpaceForEquipment(mData.SlotIndex);
			break;
		case PurchaseData.PurchaseType.PerkSlot:
			instance.UnlockPerkSlot(mData.SlotIndex);
			SwrveUserData.Instance.LogPurchase(mData);
			break;
		case PurchaseData.PurchaseType.ArmourUpgrade:
			instance.UpgradeArmour(mData.EquipmentItem);
			break;
		case PurchaseData.PurchaseType.PerkUpgrade:
			instance.UnlockProPerkEarly(mData.PerkItem.Identifier);
			SwrveUserData.Instance.LogPurchase(mData);
			break;
		case PurchaseData.PurchaseType.Bundles:
			instance.UnlockWithBundles(mData.Bundles);
			break;
		case PurchaseData.PurchaseType.Mission:
			mData.Section.Locked = false;
			SecureStorage.Instance.SaveActStructure();
			break;
		case PurchaseData.PurchaseType.KiaSoldier:
			break;
		}
	}
}
