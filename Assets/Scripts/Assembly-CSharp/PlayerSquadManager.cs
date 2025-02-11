using System;
using UnityEngine;

public class PlayerSquadManager : MonoBehaviour
{
	private static PlayerSquadManager smInstance;

	private int mGrenadeCount;

	private int mMaxGrenadeCount;

	private int mClaymoreCount;

	private int mMaxClaymoreCount;

	private int mMedkitCount;

	private int mMaxMedkitCount;

	private PurchaseFlowHelper.PurchaseData mGrenadeData;

	private PurchaseFlowHelper.PurchaseData mClaymoreData;

	private PurchaseFlowHelper.PurchaseData mMedKitData;

	private Actor mLastMedKitTarget;

	private bool mDoingInGamePurchaseFlow;

	public static PlayerSquadManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	public int GrenadeCount
	{
		get
		{
			return mGrenadeCount;
		}
	}

	public int MaxGrenadeCount
	{
		get
		{
			return mMaxGrenadeCount;
		}
	}

	public int ClaymoreCount
	{
		get
		{
			return mClaymoreCount;
		}
	}

	public int MaxClaymoreCount
	{
		get
		{
			return mMaxClaymoreCount;
		}
	}

	public int MedkitCount
	{
		get
		{
			return mMedkitCount;
		}
	}

	public int MaxMedkitCount
	{
		get
		{
			return mMaxMedkitCount;
		}
	}

	public float ArmourProtection { get; private set; }

	public int GMGReviveCost { get; private set; }

	public void GMGRevive()
	{
		SetMedKitCount(Mathf.Max(0, mMedkitCount - GMGReviveCost));
		GMGReviveCost *= 2;
		if (GameController.Instance != null && GameController.Instance.mFirstPersonActor != null)
		{
			GameController.Instance.mFirstPersonActor.speech.GMGRevive();
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple PlayerSquadManager");
		}
		smInstance = this;
		mGrenadeData = new PurchaseFlowHelper.PurchaseData();
		mClaymoreData = new PurchaseFlowHelper.PurchaseData();
		mMedKitData = new PurchaseFlowHelper.PurchaseData();
		mGrenadeData.ScriptToCallWithResult = this;
		mGrenadeData.MethodToCallWithResult = "DoGrenade";
		mGrenadeData.NumItems = 3;
		mGrenadeData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
		mGrenadeData.UseInGamePrice = true;
		mClaymoreData.ScriptToCallWithResult = this;
		mClaymoreData.MethodToCallWithResult = "DoClaymore";
		mClaymoreData.NumItems = 3;
		mClaymoreData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
		mClaymoreData.UseInGamePrice = true;
		mMedKitData.ScriptToCallWithResult = this;
		mMedKitData.MethodToCallWithResult = "DoHeal";
		mMedKitData.NumItems = 3;
		mMedKitData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
		mMedKitData.UseInGamePrice = true;
		mDoingInGamePurchaseFlow = false;
		GMGReviveCost = 1;
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	public void Update()
	{
		if (mDoingInGamePurchaseFlow)
		{
			if (TimeManager.instance.GlobalTimeState != TimeManager.State.IngamePaused && MessageBoxController.Instance.IsAnyMessageActive)
			{
				TimeManager.instance.PauseGame();
				GameController.Instance.LockGyro = true;
			}
			else if (!MessageBoxController.Instance.IsAnyMessageActive && FrontEndController.Instance.ActiveScreen == ScreenID.None)
			{
				SetupInventory();
				TimeManager.instance.UnpauseGame();
				mDoingInGamePurchaseFlow = false;
				GameController.Instance.LockGyro = false;
			}
		}
	}

	public string GetGrenadeAmmoString()
	{
		return string.Format("{0} / {1}", GrenadeCount, MaxGrenadeCount);
	}

	public string GetClaymoreAmmoString()
	{
		return string.Format("{0} / {1}", ClaymoreCount, MaxClaymoreCount);
	}

	public void ReduceGrenadeCount()
	{
		if (GrenadeCount <= 0)
		{
			return;
		}
		SwrveEventsGameplay.GrenadeUsed();
		mGrenadeCount--;
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (descriptor != null && descriptor.Type == EquipmentIconController.EquipmentType.Grenade)
			{
				equipmentSettings.NumItems = GrenadeCount;
				break;
			}
		}
	}

	public bool ThrowGrenade(bool third)
	{
		GameplayController gameplayController = GameplayController.Instance();
		bool flag = GrenadeCount > 0;
		if (gameplayController != null && (!third || gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.ThrowGrenade)))
		{
			GameController instance = GameController.Instance;
			if (instance.ClaymoreDroppingModeActive || instance.PlacementModeActive || gameplayController.SettingClaymore)
			{
				flag = false;
			}
			else if (instance.GrenadeThrowingModeActive)
			{
				flag = false;
			}
			else if ((third && gameplayController.Selected.Count == 0) || (gameplayController.Selected.Count == 1 && gameplayController.Selected[0].realCharacter.Docked))
			{
				flag = false;
			}
			else if (instance.FirstPersonTransitionInActive)
			{
				flag = false;
			}
			else if (!flag)
			{
				mGrenadeData.MethodToCallWithResult = "DoGrenadeFromMTX";
				PurchaseFlowHelper.Instance.InGamePurchase(mGrenadeData, false);
				mDoingInGamePurchaseFlow = true;
			}
			else if (third)
			{
				DoGrenadeThird();
			}
			else
			{
				DoGrenadeFirst();
			}
		}
		return flag;
	}

	public void ReduceClaymoreCount()
	{
		Debug.Log("Reduce Claymore count");
		if (ClaymoreCount <= 0)
		{
			return;
		}
		SwrveEventsGameplay.ClaymoreUsed();
		mClaymoreCount--;
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (descriptor != null && descriptor.Type == EquipmentIconController.EquipmentType.Claymore)
			{
				equipmentSettings.NumItems = ClaymoreCount;
				break;
			}
		}
	}

	public void ClaymoreConfirmed()
	{
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (descriptor != null && descriptor.Type == EquipmentIconController.EquipmentType.Claymore)
			{
				equipmentSettings.NumItems = ClaymoreCount;
				break;
			}
		}
	}

	public bool DropClaymore(bool third)
	{
		GameplayController gameplayController = GameplayController.Instance();
		bool flag = ClaymoreCount > 0;
		if (gameplayController != null && gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.PlaceMine))
		{
			GameController instance = GameController.Instance;
			if (instance.ClaymoreDroppingModeActive || instance.PlacementModeActive || gameplayController.SettingClaymore)
			{
				gameplayController.CancelAnyPlacement();
				flag = false;
			}
			else if (gameplayController.Selected.Count == 0 || (gameplayController.Selected.Count == 1 && gameplayController.Selected[0].realCharacter.Docked))
			{
				flag = false;
			}
			else if (instance.GrenadeThrowingModeActive)
			{
				flag = false;
			}
			else if (!flag)
			{
				PurchaseFlowHelper.Instance.InGamePurchase(mClaymoreData, false);
				mDoingInGamePurchaseFlow = true;
			}
			else if (third)
			{
				DoClaymoreThird();
			}
		}
		return flag;
	}

	public void ReduceMedKitCount()
	{
		if (mMedkitCount <= 0)
		{
			return;
		}
		SwrveEventsGameplay.HealthKitUsed();
		mMedkitCount--;
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (descriptor != null && descriptor.Type == EquipmentIconController.EquipmentType.MediPack)
			{
				equipmentSettings.NumItems = MedkitCount;
				break;
			}
		}
	}

	public void SetMedKitCount(int iNewAmount)
	{
		mMedkitCount = iNewAmount;
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (descriptor != null && descriptor.Type == EquipmentIconController.EquipmentType.MediPack)
			{
				equipmentSettings.NumItems = MedkitCount;
				break;
			}
		}
	}

	public bool UseMedKit(Actor target, bool endGameOnFail)
	{
		GameController instance = GameController.Instance;
		bool flag = MedkitCount > 0;
		if (target != null && instance != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			mLastMedKitTarget = target;
			if (instance.ClaymoreDroppingModeActive || instance.PlacementModeActive || gameplayController.SettingClaymore)
			{
				flag = false;
			}
			if (!flag)
			{
				if (ActStructure.Instance.CurrentMissionSectionIsTutorial())
				{
					DoHeal();
				}
				else if (PurchaseFlowHelper.Instance != null)
				{
					PurchaseFlowHelper.Instance.InGamePurchase(mMedKitData, endGameOnFail);
					mDoingInGamePurchaseFlow = true;
				}
			}
			else
			{
				DoHeal();
				ReduceMedKitCount();
			}
		}
		return flag;
	}

	public void SetupInventory()
	{
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (!(descriptor == null))
			{
				if (descriptor.Type == EquipmentIconController.EquipmentType.Grenade)
				{
					mGrenadeCount = equipmentSettings.NumItems;
					mMaxGrenadeCount = equipmentSettings.SlotSize;
					mGrenadeData.EquipmentItem = descriptor;
				}
				else if (descriptor.Type == EquipmentIconController.EquipmentType.Claymore)
				{
					mClaymoreCount = equipmentSettings.NumItems;
					mMaxClaymoreCount = equipmentSettings.SlotSize;
					mClaymoreData.EquipmentItem = descriptor;
				}
				else if (descriptor.Type == EquipmentIconController.EquipmentType.MediPack)
				{
					mMedkitCount = equipmentSettings.NumItems;
					mMaxMedkitCount = equipmentSettings.SlotSize;
					mMedKitData.EquipmentItem = descriptor;
				}
			}
		}
		if (instance.Armour != null)
		{
			float num = StatsManager.Instance.PerksManager().GetModifierForPerk(PerkType.FlakJacket);
			if (num == 1f)
			{
				num = 0f;
			}
			ArmourProtection = 1f - (instance.Armour.Protection + num) / 100f;
		}
	}

	private void DoGrenadeFirst()
	{
		SetupInventory();
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor != null && mFirstPersonActor.weapon.ActiveWeapon != null)
		{
			IWeapon activeWeapon = mFirstPersonActor.weapon.ActiveWeapon;
			IWeapon desiredWeapon = mFirstPersonActor.weapon.DesiredWeapon;
			IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(activeWeapon);
			if (!(activeWeapon is Weapon_Grenade) && !(desiredWeapon is Weapon_Grenade) && weaponEquip != null && !weaponEquip.IsTakingOut() && !weaponEquip.IsPuttingAway())
			{
				mFirstPersonActor.weapon.SwapTo(new Weapon_Grenade(), 2f);
			}
		}
	}

	private void DoGrenadeFromMTX()
	{
	}

	private void DoGrenadeThird()
	{
		Vector3 initialTargetPosition = Vector3.zero;
		Vector3 position = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		Ray ray = CameraManager.Instance.CurrentCamera.ScreenPointToRay(position);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 1))
		{
			initialTargetPosition = hitInfo.point;
		}
		GameplayController gameplayController = GameplayController.Instance();
		OrdersHelper.OrderPrimeGrenade(gameplayController, initialTargetPosition);
	}

	private void DoClaymoreThird()
	{
		GameplayController gameplayController = GameplayController.Instance();
		gameplayController.BeginPlacingClaymore();
	}

	private void DoHeal()
	{
		if (mLastMedKitTarget.InitiatingGameFail)
		{
			EventHub.Instance.Report(new Events.CharacterHealed(mLastMedKitTarget.EventActor()));
			return;
		}
		GameplayController gameplayController = GameplayController.Instance();
		OrdersHelper.OrderHeal(gameplayController, mLastMedKitTarget);
		MusicManager.Instance.StopMortallyWoundedThemeMusic();
	}

	public void AwardGrenade(int amt)
	{
		if (mGrenadeCount + amt < mMaxGrenadeCount)
		{
			mGrenadeCount += amt;
		}
		else
		{
			mGrenadeCount = mMaxGrenadeCount;
		}
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (descriptor != null && descriptor.Type == EquipmentIconController.EquipmentType.Grenade)
			{
				equipmentSettings.NumItems = mGrenadeCount;
				break;
			}
		}
	}

	public void AwardMedkit(int amt)
	{
		if (mMedkitCount + amt < mMaxMedkitCount)
		{
			mMedkitCount += amt;
		}
		else
		{
			mMedkitCount = mMaxMedkitCount;
		}
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (descriptor != null && descriptor.Type == EquipmentIconController.EquipmentType.MediPack)
			{
				equipmentSettings.NumItems = mMedkitCount;
				break;
			}
		}
	}

	public void AwardClaymore(int amt)
	{
		if (mClaymoreCount + amt < mMaxClaymoreCount)
		{
			mClaymoreCount += amt;
		}
		else
		{
			mClaymoreCount = mMaxClaymoreCount;
		}
		GameSettings instance = GameSettings.Instance;
		EquipmentSettings[] equipment = instance.Equipment;
		foreach (EquipmentSettings equipmentSettings in equipment)
		{
			EquipmentDescriptor descriptor = equipmentSettings.Descriptor;
			if (descriptor != null && descriptor.Type == EquipmentIconController.EquipmentType.Claymore)
			{
				equipmentSettings.NumItems = mClaymoreCount;
				break;
			}
		}
	}
}
