using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CMAmmoCache))]
public class AmmoCache : MonoBehaviour
{
	private AnimationClip mTakeAmmoClip;

	public GameObject ObjectiveBlipPrefab;

	public AmmoCacheData m_Interface;

	public EquipmentDescriptor AmmoDescriptor;

	[HideInInspector]
	public bool mActive;

	public AnimationClip UseCrateAnim;

	private Animation mCratePlayer;

	private ObjectiveBlip mBlip;

	private ContextMenuDistanceManager mCMDM;

	private CMAmmoCache mCMAC;

	private bool mDoingInGamePurchaseFlow;

	private PurchaseFlowHelper.PurchaseData m_AmmoData;

	private Actor m_PurchaseActor;

	private CMAmmoCache mContextMenu;

	private void Start()
	{
		mCMDM = IncludeDisabled.GetComponentInChildren<ContextMenuDistanceManager>(base.gameObject);
		mCMAC = IncludeDisabled.GetComponentInChildren<CMAmmoCache>(base.gameObject);
		mCratePlayer = base.transform.parent.gameObject.GetComponentInChildren<Animation>();
		if ((bool)mCratePlayer)
		{
			foreach (AnimationState item in mCratePlayer)
			{
				if (item != null)
				{
					mTakeAmmoClip = item.clip;
					break;
				}
			}
			if (mTakeAmmoClip != null)
			{
				mCratePlayer.enabled = false;
				mCratePlayer.enabled = true;
				mCratePlayer.Play(mTakeAmmoClip.name);
				mCratePlayer.animation[mTakeAmmoClip.name].speed = 1f;
			}
			else
			{
				Debug.LogWarning("Failed to find the required animation for " + base.name);
			}
		}
		CMAmmoCache component = GetComponent<CMAmmoCache>();
		if (component != null && component.CrateModel != null)
		{
			Component[] componentsInChildren = component.CrateModel.GetComponentsInChildren(typeof(Collider), true);
			Component[] array = componentsInChildren;
			foreach (Component component2 in array)
			{
				if (!component2.rigidbody && !component2.gameObject.isStatic)
				{
					Rigidbody rigidbody = component2.gameObject.AddComponent<Rigidbody>();
					rigidbody.isKinematic = true;
				}
			}
		}
		Deactivate();
	}

	public void Activate()
	{
		GameObject gameObject = Object.Instantiate(ObjectiveBlipPrefab) as GameObject;
		mBlip = gameObject.GetComponent<ObjectiveBlip>();
		if ((bool)mBlip)
		{
			mBlip.mObjectiveText = Language.Get("S_EQUIPMENT_NAME_AMMO");
			GameObject gameObject2 = new GameObject();
			gameObject2.transform.position = base.transform.position + new Vector3(0f, 0.5f, 0f);
			mBlip.Target = gameObject2.transform;
			ObjectiveBlip.BlipType blipType = ObjectiveBlip.BlipType.Ammo;
			mBlip.SetBlipType(blipType);
			mBlip.SwitchOn();
		}
		if (mCMDM != null)
		{
			mCMDM.enabled = true;
		}
		if (mTakeAmmoClip != null)
		{
			mCratePlayer.Play(mTakeAmmoClip.name);
			AnimationState animationState = mCratePlayer.animation[mTakeAmmoClip.name];
			if (animationState != null)
			{
				animationState.time = animationState.length;
				animationState.speed = -1f;
			}
		}
		HUDMessenger.Instance.PushMessage(Language.Get("S_SURVIVAL_MSG_CACHE_01"), string.Empty, string.Empty, false);
		string text = "S_SURVIVAL_DIALOGUE_CACHEOPEN_0" + Random.Range(1, 3);
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(text);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, SpecOpsVOSFX.Instance, text, false, 1f);
		mActive = true;
	}

	public void Deactivate()
	{
		if ((bool)mBlip)
		{
			mBlip.SwitchOff();
		}
		if (mCMAC != null)
		{
			mCMAC.ShowBlip(false);
		}
		if (mCMDM != null)
		{
			mCMDM.enabled = false;
		}
		if (mTakeAmmoClip != null)
		{
			mCratePlayer.Play(mTakeAmmoClip.name);
			mCratePlayer.animation[mTakeAmmoClip.name].speed = 1f;
		}
		mActive = false;
		if (CheckShouldTimerPause())
		{
			CommonHudController.Instance.MissionTimer.StartTimer();
		}
	}

	public void TryPurchase(Actor actor, CMAmmoCache cm)
	{
		mContextMenu = cm;
		AmmoDescriptor.HardCost = GMGData.Instance.GetAmmoCost();
		m_AmmoData = new PurchaseFlowHelper.PurchaseData();
		m_AmmoData.ScriptToCallWithResult = this;
		m_AmmoData.MethodToCallWithResult = "DoPurchase";
		m_AmmoData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
		m_AmmoData.NumItems = 1;
		m_AmmoData.EquipmentItem = AmmoDescriptor;
		m_PurchaseActor = actor;
		PurchaseFlowHelper.Instance.Purchase(m_AmmoData);
		mDoingInGamePurchaseFlow = true;
	}

	private void DoPurchase()
	{
		if (CheckShouldTimerPause())
		{
			CommonHudController.Instance.MissionTimer.PauseTimer();
		}
		string text = "S_SURVIVAL_DIALOGUE_AMMOPURCHASE_0" + Random.Range(1, 3);
		CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(text);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, SpecOpsVOSFX.Instance, text, false, 1f);
		OrdersHelper.OrderAmmoCache(GameplayController.instance, mContextMenu);
	}

	private bool CheckShouldTimerPause()
	{
		if ((bool)CommonHudController.Instance && (bool)CommonHudController.Instance.MissionTimer && GMGData.Instance.CurrentGameType != 0)
		{
			return true;
		}
		return false;
	}

	public void PickUpAmmo()
	{
		List<Actor> list = new List<Actor>();
		list.Add(m_PurchaseActor);
		float num = AmmoDropManager.Instance.AmmoCacheReward(m_PurchaseActor.weapon.PrimaryWeapon.GetClass());
		CharacterPropertyModifier.AddAmmoPercent(list, num.ToString());
		m_PurchaseActor.weapon.SwitchToPrimary();
		if (m_PurchaseActor.weapon.GetPercentageAmmoInClip() < 1f)
		{
			m_PurchaseActor.weapon.Reload();
		}
		GMGData.Instance.RegisterAmmoPurchase();
		Deactivate();
		int amount = (int)(num * (float)m_PurchaseActor.weapon.PrimaryWeapon.GetWeaponAmmo().MaxAmmo);
		EventHub.Instance.Report(new Events.AmmoCacheUsed(amount, m_PurchaseActor.weapon.PrimaryWeapon.GetClass()));
	}

	public void Update()
	{
		if (!mDoingInGamePurchaseFlow)
		{
			return;
		}
		if (TimeManager.instance.GlobalTimeState != TimeManager.State.IngamePaused)
		{
			TimeManager.instance.PauseGame();
			GameController.Instance.LockGyro = true;
		}
		else if (!MessageBoxController.Instance.IsAnyMessageActive && FrontEndController.Instance.ActiveScreen == ScreenID.None)
		{
			if (PlayerSquadManager.Instance != null)
			{
				PlayerSquadManager.Instance.SetupInventory();
			}
			TimeManager.instance.UnpauseGame();
			mDoingInGamePurchaseFlow = false;
			GameController.Instance.LockGyro = false;
		}
	}
}
