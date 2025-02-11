using UnityEngine;

public class ContinueScreen : FrontEndScreen
{
	private const float c_fTimerTime = 5f;

	private const int c_iMaxExtraMedPacksToGive = 3;

	public SpriteText ContextText;

	public GameObject GMGNode;

	public SpriteText GMGUseMedKitsCost;

	public SpriteText GMGMedKitQuantity;

	public ProgressBar TimerBar;

	public EquipmentIconController Icon;

	public GameObject CampaignNode;

	public SpriteText CampaignReviveAllCost;

	public SpriteText CampaingReviveWithMedKitsCost;

	public SpriteText CampaignReviveAboveButtonText;

	public SpriteText CampaignReviveWithMedKitsText;

	private float m_fTimer = 5f;

	private bool m_bGMG;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private EquipmentDescriptor m_MedkitDescriptor;

	private int m_iMedKitsNeeded;

	private int m_iTotalMedKitsNeeded;

	private int m_iExtraMedkitsToGive;

	private PurchaseFlowHelper.PurchaseData m_MedKitData;

	private bool m_bGMGFailed;

	private bool m_TransitionOutPending;

	private bool m_HasSuicided;

	private bool m_DontShow;

	protected override void Awake()
	{
		ID = ScreenID.ContinueScreen;
		base.Awake();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	protected override void Start()
	{
		GMGNode.SetActive(false);
		CampaignNode.SetActive(false);
		m_MedKitData = new PurchaseFlowHelper.PurchaseData();
		m_MedKitData.ScriptToCallWithResult = this;
		m_MedKitData.MethodToCallWithResult = "DoHeal";
		m_MedKitData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
		m_MedKitData.UseInGamePrice = true;
		base.Start();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			m_fTimer = 5f;
			TimerBar.SetValueNow(1f);
		}
	}

	public override void EnterScreen()
	{
		m_DontShow = false;
		if (ActStructure.Instance != null)
		{
			m_bGMG = ActStructure.Instance.CurrentMissionIsSpecOps();
		}
		m_bGMGFailed = false;
		m_HasSuicided = false;
		GMGNode.SetActive(m_bGMG);
		CampaignNode.SetActive(!m_bGMG);
		m_MedkitDescriptor = WeaponManager.Instance.GetEquipmentDescriptor("S_EQUIPMENT_NAME_MEDIKIT");
		if (m_bGMG)
		{
			m_iTotalMedKitsNeeded = PlayerSquadManager.Instance.GMGReviveCost;
		}
		else
		{
			m_iTotalMedKitsNeeded = 0;
			ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				if ((a.realCharacter.IsMortallyWounded() || a.InitiatingGameFail) && !a.realCharacter.IsDead())
				{
					m_iTotalMedKitsNeeded++;
				}
			}
		}
		m_iMedKitsNeeded = Mathf.Max(0, m_iTotalMedKitsNeeded - PlayerSquadManager.Instance.MedkitCount);
		if (m_iMedKitsNeeded > 0)
		{
			CampaignReviveAllCost.Text = GameSettings.Instance.CalculateInGameCostOfEquipment(m_MedkitDescriptor, m_iMedKitsNeeded).ToString() + CommonHelper.HardCurrencySymbol();
			GMGUseMedKitsCost.Text = GameSettings.Instance.CalculateInGameCostOfEquipment(m_MedkitDescriptor, m_iMedKitsNeeded).ToString() + CommonHelper.HardCurrencySymbol();
			ContextText.Text = Language.Get("S_MTX_HEAL_NORMAL_REVIVE_NOTENOUGHKITS");
			CampaignReviveAboveButtonText.Text = Language.Get("S_MTX_HEAL_REVIVE");
		}
		else
		{
			ContextText.Text = Language.Get("S_MTX_HEAL_FORCEDCUTSCENE_REVIVE");
			CampaignReviveAboveButtonText.Text = Language.Get("S_MTX_HEAL_JUSTREVIVE");
			CampaignReviveAllCost.Text = ((m_iTotalMedKitsNeeded != 1) ? Language.GetFormatString("S_MTX_HEAL_X_KITS", m_iTotalMedKitsNeeded) : Language.Get("S_MTX_HEAL_1_KIT"));
			GMGUseMedKitsCost.Text = Language.Get("S_MTX_HEAL_REVIVE");
		}
		if (m_bGMG)
		{
			ContextText.Text = Language.GetFormatString((m_iTotalMedKitsNeeded != 1) ? "S_MTX_HEAL_NUMBER_GMG_MESSAGE_PLURAL" : "S_MTX_HEAL_NUMBER_GMG_MESSAGE", m_iTotalMedKitsNeeded);
		}
		m_iExtraMedkitsToGive = Mathf.Clamp(PlayerSquadManager.Instance.MaxMedkitCount - (PlayerSquadManager.Instance.MedkitCount - m_iMedKitsNeeded), 1, 3);
		CampaignReviveWithMedKitsText.Text = ((m_iExtraMedkitsToGive != 1) ? Language.GetFormatString("S_MTX_HEAL_REVIVE_ADDXKITS", m_iExtraMedkitsToGive) : Language.Get("S_MTX_HEAL_REVIVE_ADD1KIT"));
		CampaingReviveWithMedKitsCost.Text = GameSettings.Instance.CalculateInGameCostOfEquipment(m_MedkitDescriptor, m_iMedKitsNeeded + m_iExtraMedkitsToGive).ToString() + CommonHelper.HardCurrencySymbol();
		if (GMGMedKitQuantity != null)
		{
			GMGMedKitQuantity.Text = "x" + PlayerSquadManager.Instance.GMGReviveCost;
		}
		m_MedKitData.EquipmentItem = m_MedkitDescriptor;
		if (m_iMedKitsNeeded > 0 && GameSettings.Instance.CalculateInGameCostOfEquipment(m_MedkitDescriptor, m_iMedKitsNeeded) > GameSettings.Instance.PlayerCash().HardCash())
		{
			ClearScreen(true);
			m_DontShow = true;
		}
		else
		{
			base.EnterScreen();
		}
	}

	public override void ExitScreen()
	{
		if (m_DontShow)
		{
			ClearScreen(false);
		}
		else
		{
			base.ExitScreen();
		}
	}

	public override void OnScreen()
	{
		if (m_DontShow)
		{
			m_fTimer = 0f;
			return;
		}
		m_fTimer = 5f;
		TimerBar.SetValueNow(1f);
		TimeManager.instance.PauseGame();
		GameController.Instance.SuppressHud(true);
		GameController.Instance.LockGyro = true;
		CameraManager.Instance.AllowInput(false);
		GameplayController.Instance().DisableInput();
		base.OnScreen();
		Icon.SetEquipment(EquipmentIconController.EquipmentType.MediPack, true);
	}

	public override void OffScreen()
	{
		if (FrontEndController.Instance.ActiveScreen == ScreenID.None)
		{
			GameplayController.Instance().EnableInput();
			if (!m_HasSuicided)
			{
				CameraManager.Instance.AllowInput(true);
				GameController.Instance.LockGyro = false;
				GameController.Instance.SuppressHud(false);
			}
			TimeManager.instance.UnpauseGame();
		}
		m_TransitionOutPending = false;
		base.OffScreen();
	}

	protected override void Update()
	{
		base.Update();
		if (!base.IsActive)
		{
			return;
		}
		if (m_TransitionOutPending)
		{
			FrontEndController.Instance.TransitionTo(ScreenID.None);
		}
		else if (m_bGMG && !m_bGMGFailed && !MessageBox.MessageBoxInProgress)
		{
			m_fTimer = Mathf.Max(0f, m_fTimer - Time.deltaTime / Time.timeScale);
			TimerBar.SetValueNow(m_fTimer / 5f);
			if (m_fTimer == 0f)
			{
				FailMission();
			}
		}
	}

	public void FailMission()
	{
		if (!FrontEndController.Instance.IsBusy)
		{
			GameplayController instance = GameplayController.instance;
			if (instance != null)
			{
				instance.SuicideSquad();
				m_HasSuicided = true;
			}
			m_bGMGFailed = true;
			FrontEndController.Instance.TransitionTo(ScreenID.None);
		}
	}

	public void GMGRevive()
	{
		if (!FrontEndController.Instance.IsBusy && !m_bGMGFailed)
		{
			if (m_iMedKitsNeeded > 0)
			{
				m_MedKitData.NumItems = m_iMedKitsNeeded;
				PurchaseFlowHelper.Instance.Purchase(m_MedKitData);
				m_fTimer = 5f;
			}
			else
			{
				DoHeal();
			}
		}
	}

	public void CampaignReviveAllWithMediKits()
	{
		if (!FrontEndController.Instance.IsBusy && !m_bGMGFailed)
		{
			m_MedKitData.NumItems = m_iMedKitsNeeded + m_iExtraMedkitsToGive;
			PurchaseFlowHelper.Instance.Purchase(m_MedKitData);
		}
	}

	public void CampaignReviveAll()
	{
		if (!FrontEndController.Instance.IsBusy && !m_bGMGFailed)
		{
			m_MedKitData.NumItems = m_iMedKitsNeeded;
			if (m_iMedKitsNeeded > 0)
			{
				PurchaseFlowHelper.Instance.Purchase(m_MedKitData);
			}
			else
			{
				DoHeal();
			}
		}
	}

	private void DoHeal()
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		GameplayController instance = GameplayController.instance;
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if ((a.realCharacter.IsMortallyWounded() || a.health.IsReviving) && !a.realCharacter.IsDead())
			{
				OrdersHelper.OrderHeal(instance, a);
			}
			else if (a.InitiatingGameFail)
			{
				EventHub.Instance.Report(new Events.CharacterHealed(a.EventActor()));
			}
		}
		MusicManager.Instance.StopMortallyWoundedThemeMusic();
		m_TransitionOutPending = true;
		if (m_bGMG)
		{
			PlayerSquadManager.Instance.GMGRevive();
			HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
			GameController.Instance.StartGMGReviveMode();
		}
		else
		{
			int medKitCount = Mathf.Max(PlayerSquadManager.Instance.MedkitCount + (m_MedKitData.NumItems - m_iTotalMedKitsNeeded), 0);
			PlayerSquadManager.Instance.SetMedKitCount(medKitCount);
		}
	}
}
