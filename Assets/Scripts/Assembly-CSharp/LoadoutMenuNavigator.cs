using System.Collections;
using UnityEngine;

public class LoadoutMenuNavigator : MonoBehaviour
{
	private const float DELAY_NEXT_SCREEN_ACTIVATE_MULTIPLIER = 0.75f;

	private const float LOADOUT_SCREEN_ANIM_OFF_TIME = 1.2f;

	private const float PERK_PANEL_ACTIVATE_DELAY = 0.5f;

	private static bool mLoadOutActive;

	private static bool mAutoAddingEquipment;

	public Material[] LoadoutMaterials;

	public LoadoutMenuController LoadoutMenuCtrl;

	public LoadoutSoldierMenuController LoadoutSoldierMenuCtrl;

	public LoadoutEquipmentMenuController LoadoutEquipmentMenuCtrl;

	public LoadoutArmourMenuController LoadoutArmourMenuCtrl;

	public LoadoutPerkMenuController LoadoutPerkMenuCtrl;

	public MenuScreenBlade InventoryBlade;

	public MenuScreenBlade PerkBlade;

	public MenuScreenBlade ArmourBlade;

	public GameObject CommonHud;

	public GameObject StrategyHud;

	private bool mDeploy;

	private bool mGameStarted;

	private bool mReadyForDestruction;

	private bool mRequestedDestroyed;

	public static bool LoadOutActive
	{
		get
		{
			return mLoadOutActive;
		}
	}

	public static bool AutoAddingEquipment
	{
		get
		{
			return mAutoAddingEquipment;
		}
	}

	private void Awake()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.LinkLoadoutMenuController(this);
		}
		mDeploy = false;
		mGameStarted = false;
	}

	private void Update()
	{
		if (mReadyForDestruction)
		{
			if (mRequestedDestroyed)
			{
				Object.Destroy(base.gameObject);
				if (GameController.Instance != null)
				{
					GameController.Instance.LinkLoadoutMenuController(null);
				}
			}
		}
		else if (mDeploy && !FrontEndController.Instance.IsBusy && !mGameStarted)
		{
			mGameStarted = true;
			SceneNanny.EndSkippableScene();
			if (CameraManager.Instance != null)
			{
				CameraManager.Instance.AllowInput(true);
				CameraManager.Instance.SetStratCamEnable(true);
			}
			GameController instance = GameController.Instance;
			if (instance != null)
			{
				instance.PostLoadoutBeginGameplay();
			}
			PlayerSquadManager.Instance.SetupInventory();
			StartCoroutine(DeactivateBackgroundAfterTime(3f));
		}
	}

	public void DeleteFromGameController()
	{
		mRequestedDestroyed = true;
		if (!mDeploy)
		{
			mReadyForDestruction = true;
		}
	}

	public void StartLoadout()
	{
		mLoadOutActive = true;
		SoundManager.Instance.ActivateLoadOutSFX();
		GameSettings instance = GameSettings.Instance;
		if (instance != null)
		{
			instance.UpdateGameSettingsWithSoldierPresence();
			instance.CacheAutoLoadouts();
			if (ActStructure.Instance.CurrentMissionIsSpecOps())
			{
				instance.SetupForSpecOps();
			}
		}
		if (CameraManager.Instance != null)
		{
			CameraManager.Instance.AllowInput(false);
			CameraManager.Instance.SetStratCamEnable(false);
		}
		LoadoutMenuCtrl.CurrentSoldierPanel = -1;
		GameController instance2 = GameController.Instance;
		if (instance2 != null)
		{
			instance2.SuppressHud(true);
		}
		SwrveEventsUI.ViewedLoadOut();
		GoToLoadoutMenu();
		SceneNanny.BeginSkippableScene();
	}

	public void FinishLoadout()
	{
		if (!FrontEndController.Instance.IsBusy && !MessageBoxController.Instance.IsAnyMessageActive)
		{
			mLoadOutActive = false;
			mDeploy = true;
			mGameStarted = false;
			SecureStorage.Instance.SaveGameSettings();
			LoadoutMenuCtrl.CurrentSoldierPanel = -1;
			LoadoutMenuCtrl.Deactivate();
			SoundManager.Instance.DeactivateLoadOutSFX();
			FrontEndController.Instance.TransitionTo(ScreenID.None);
		}
	}

	public void GoToLoadoutMenu()
	{
		if (!FrontEndController.Instance.IsBusy)
		{
			FrontEndController.Instance.TransitionTo(ScreenID.SquadLoadOut);
		}
	}

	public void GoToLoadoutSoldierMenu(int editing)
	{
		if (!FrontEndController.Instance.IsBusy && (MissionSetup.Instance == null || !MissionSetup.Instance.LockWeaponSelection))
		{
			LoadoutSoldierMenuCtrl.Editing(editing);
			LoadoutMenuCtrl.CurrentSoldierPanel = editing;
			LoadoutSoldierMenuCtrl.SoldierPanelBlade = LoadoutMenuCtrl.GetSoldierPanelBlade(editing);
			FrontEndController.Instance.TransitionTo(ScreenID.WeaponSelect);
			SwrveEventsUI.ViewedWeapons();
			InterfaceSFX.Instance.GeneralButtonPress.Play2D();
		}
	}

	public void GoToLoadoutPerkMenu(int slot)
	{
		LoadoutPerkMenuCtrl.SetSlot(slot);
		if (!FrontEndController.Instance.IsBusy)
		{
			LoadoutMenuCtrl.CurrentSoldierPanel = -1;
			LoadoutPerkMenuCtrl.PerkPanelBlade = LoadoutMenuCtrl.PerkPanel;
			FrontEndController.Instance.TransitionTo(ScreenID.PerkSelect);
			SwrveEventsUI.ViewedPerks();
		}
	}

	public void GoToEquipmentScreen(int highlightSlot)
	{
		if (!FrontEndController.Instance.IsBusy)
		{
			LoadoutMenuCtrl.CurrentSoldierPanel = -1;
			LoadoutEquipmentMenuCtrl.InventoryPanelBlade = InventoryBlade;
			FrontEndController.Instance.TransitionTo(ScreenID.EquipmentSelect);
			SwrveEventsUI.ViewedEquipment();
		}
		LoadoutEquipmentMenuCtrl.SetSlot(highlightSlot);
	}

	public void ArmourPressed()
	{
		if (!FrontEndController.Instance.IsBusy)
		{
			LoadoutMenuCtrl.CurrentSoldierPanel = -1;
			LoadoutArmourMenuCtrl.ArmourPanelBlade = ArmourBlade;
			FrontEndController.Instance.TransitionTo(ScreenID.ArmourSelect);
			SwrveEventsUI.ViewedArmour();
		}
	}

	public void BundlesPressed()
	{
		if (!FrontEndController.Instance.IsBusy)
		{
			LoadoutMenuCtrl.CurrentSoldierPanel = -1;
			FrontEndController.Instance.TransitionTo(ScreenID.BundleSelect);
			SwrveEventsUI.ViewedBundles();
		}
	}

	public void UnloadLoadoutTextureAssets()
	{
		if (LoadoutMaterials == null)
		{
			return;
		}
		for (int i = 0; i < LoadoutMaterials.Length; i++)
		{
			Material material = LoadoutMaterials[i];
			if (material != null)
			{
				Resources.UnloadAsset(material.mainTexture);
				Texture texture = material.GetTexture("_AlphaTex");
				if (texture != null)
				{
					Resources.UnloadAsset(texture);
				}
			}
		}
	}

	private IEnumerator DeactivateBackgroundAfterTime(float waitTime)
	{
		float time2 = Time.realtimeSinceStartup + (waitTime - 0.5f);
		while (Time.realtimeSinceStartup < time2)
		{
			yield return null;
		}
		MissionDescriptor MissionDesc = MissionDescriptor.Instance;
		if (MissionDesc != null)
		{
			MissionDesc.StartIntroSequence();
		}
		time2 = Time.realtimeSinceStartup + 0.5f;
		while (Time.realtimeSinceStartup < time2)
		{
			yield return null;
		}
		AnimatedScreenBackground bg = AnimatedScreenBackground.Instance;
		if (bg != null)
		{
			bg.Deactivate();
		}
		mReadyForDestruction = true;
	}
}
