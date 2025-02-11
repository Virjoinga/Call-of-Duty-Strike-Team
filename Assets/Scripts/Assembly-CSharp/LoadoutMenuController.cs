using UnityEngine;

public class LoadoutMenuController : MenuScreenBlade
{
	public LoadoutMenuNavigator Navigator;

	public MenuScreenBlade[] SoldierPanelBlades;

	public LoadoutSideMenu SideMenuPanel;

	public MenuScreenBlade InventoryPanel;

	public MenuScreenBlade PerkPanel;

	public MenuScreenBlade ArmourPanel;

	public MenuScreenBlade BundlesButton;

	private LoadoutInventoryPanel mInventoryPanel;

	private LoadoutPerkPanelController mPerkPanel;

	private LoadoutArmourPanel mArmourPanel;

	private LoadoutSoldierPanel[] mSoldierPanels;

	private MissionData mCurrentMission;

	private int mCurrentSoldierPanel;

	public int CurrentSoldierPanel
	{
		get
		{
			return mCurrentSoldierPanel;
		}
		set
		{
			mCurrentSoldierPanel = value;
		}
	}

	public override void OnScreen()
	{
		base.OnScreen();
		DailyRewards.Instance.CheckDailyReward();
	}

	public override void Awake()
	{
		base.Awake();
		if (InventoryPanel != null && PerkPanel != null && ArmourPanel != null)
		{
			mInventoryPanel = InventoryPanel.GetComponentInChildren<LoadoutInventoryPanel>();
			mPerkPanel = PerkPanel.GetComponentInChildren<LoadoutPerkPanelController>();
			mArmourPanel = ArmourPanel.GetComponentInChildren<LoadoutArmourPanel>();
		}
		mCurrentSoldierPanel = -1;
		mSoldierPanels = new LoadoutSoldierPanel[SoldierPanelBlades.Length];
		for (int i = 0; i < SoldierPanelBlades.Length; i++)
		{
			mSoldierPanels[i] = SoldierPanelBlades[i].GetComponentInChildren<LoadoutSoldierPanel>();
		}
	}

	public void Start()
	{
		RefreshScreen();
		MissionSetup instance = MissionSetup.Instance;
		if (instance != null && instance.LockWeaponSelection)
		{
			for (int i = 0; i < SideMenuPanel.MenuOptions.Length; i++)
			{
				SideMenuPanel.SetDisabled(true, i, "S_WEAPON_SELECTION_AUTO");
			}
		}
		else
		{
			GameSettings instance2 = GameSettings.Instance;
			instance2.CacheAutoLoadouts();
			SideMenuPanel.SetSelected((int)instance2.AutoLoadoutMode);
		}
		MissionListings.eMissionID currentMissionID = ActStructure.Instance.CurrentMissionID;
		mCurrentMission = MissionListings.Instance.Mission(currentMissionID);
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if (SoldierPanelBlades.Length != 4)
		{
			Debug.LogError("Unexpected number of soldier panel blades");
		}
		float num = 0.1f;
		int[] array = new int[4] { 0, 2, 1, 3 };
		for (int i = 0; i < SoldierPanelBlades.Length; i++)
		{
			if (array[i] != mCurrentSoldierPanel)
			{
				SoldierPanelBlades[array[i]].DelayedActivate(num * (float)i);
			}
		}
		mCurrentSoldierPanel = -1;
		if (InventoryPanel != null && PerkPanel != null && ArmourPanel != null && BundlesButton != null)
		{
			InventoryPanel.DelayedActivate(num * 1f);
			PerkPanel.DelayedActivate(num);
			ArmourPanel.DelayedActivate(num * 2f);
			BundlesButton.DelayedActivate(num * 3f);
		}
		GameSettings instance = GameSettings.Instance;
		if (instance != null)
		{
			SideMenuPanel.SetSelected((int)instance.AutoLoadoutMode);
		}
		MessageBoxController instance2 = MessageBoxController.Instance;
		if (instance2 != null)
		{
			instance2.DoLoadoutTutorialDialogue();
		}
		RefreshScreen();
		SwrveEventsUI.SwrveTalkTrigger_LoadOut();
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		if (SoldierPanelBlades.Length != 4)
		{
			Debug.LogError("Unexpected number of soldier panel blades");
		}
		float num = 0.1f;
		int[] array = new int[4] { 3, 1, 2, 0 };
		for (int i = 0; i < SoldierPanelBlades.Length; i++)
		{
			if (array[i] != mCurrentSoldierPanel)
			{
				SoldierPanelBlades[array[i]].DelayedDeactivate(num * (float)i);
			}
		}
		ScreenID activeScreen = FrontEndController.Instance.ActiveScreen;
		if (activeScreen != ScreenID.EquipmentSelect && InventoryPanel != null)
		{
			InventoryPanel.DelayedDeactivate(num);
		}
		if (activeScreen != ScreenID.PerkSelect && PerkPanel != null)
		{
			PerkPanel.DelayedDeactivate(num * 2f);
		}
		if (ArmourPanel != null)
		{
			ArmourPanel.DelayedDeactivate(num * 1f);
		}
		if (BundlesButton != null)
		{
			BundlesButton.DelayedDeactivate(num);
		}
	}

	public override void Update()
	{
		base.Update();
		if (base.IsTransitioning)
		{
			return;
		}
		for (int i = 0; i < SoldierPanelBlades.Length; i++)
		{
			if (base.IsTransitioning)
			{
				break;
			}
			UpdateTransitioningIfNone(SoldierPanelBlades[i]);
		}
		if (!base.IsTransitioning)
		{
			UpdateTransitioningIfNone(InventoryPanel);
			UpdateTransitioningIfNone(PerkPanel);
			UpdateTransitioningIfNone(ArmourPanel);
		}
	}

	public void OnSelectedBalanced()
	{
		if (!MissionSetup.Instance.LockWeaponSelection)
		{
			GameSettings instance = GameSettings.Instance;
			instance.SetToBalanced();
			RefreshScreen();
			SideMenuPanel.SetSelected((int)instance.AutoLoadoutMode);
			SwrveEventsUI.UsedPreset();
		}
	}

	public void OnSelectedAssault()
	{
		if (!MissionSetup.Instance.LockWeaponSelection)
		{
			GameSettings instance = GameSettings.Instance;
			instance.SetToAssault();
			RefreshScreen();
			SideMenuPanel.SetSelected((int)instance.AutoLoadoutMode);
			SwrveEventsUI.UsedPreset();
		}
	}

	public void OnSelectedStealth()
	{
		if (!MissionSetup.Instance.LockWeaponSelection)
		{
			GameSettings instance = GameSettings.Instance;
			instance.SetToStealth();
			RefreshScreen();
			SideMenuPanel.SetSelected((int)instance.AutoLoadoutMode);
			SwrveEventsUI.UsedPreset();
		}
	}

	public void OnSelectedCustom()
	{
		if (!MissionSetup.Instance.LockWeaponSelection)
		{
			GameSettings instance = GameSettings.Instance;
			instance.SetToCustom();
			RefreshScreen();
			SideMenuPanel.SetSelected((int)instance.AutoLoadoutMode);
		}
	}

	public MenuScreenBlade GetSoldierPanelBlade(int index)
	{
		MenuScreenBlade result = null;
		if (index >= 0 && index < SoldierPanelBlades.Length)
		{
			result = SoldierPanelBlades[index];
		}
		return result;
	}

	public LoadoutPerkPanelController GetPerkPanel()
	{
		return mPerkPanel;
	}

	private void RefreshScreen()
	{
		GameSettings instance = GameSettings.Instance;
		bool auto = false;
		MissionSetup instance2 = MissionSetup.Instance;
		if (instance2 != null && instance2.LockWeaponSelection)
		{
			auto = true;
		}
		if (mSoldierPanels.Length != instance.Soldiers.Length)
		{
			Debug.LogError("Number of soldier panels not equal to the number of soldiers in game settings");
		}
		for (int i = 0; i < mSoldierPanels.Length && i < instance.Soldiers.Length; i++)
		{
			mSoldierPanels[i].Setup(i, instance.Soldiers[i], mCurrentMission, auto);
		}
		if (mInventoryPanel != null && mPerkPanel != null && mArmourPanel != null)
		{
			mInventoryPanel.Refresh();
			mPerkPanel.Refresh();
			mArmourPanel.Refresh();
		}
	}
}
