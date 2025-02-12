using UnityEngine;

public class ContextObjectBlip : HudBlipIcon
{
	private const float mFirstPersonButtonSize = 4f;

	private const float mThirdPersonButtonSize = 1.9f;

	private InterfaceableObject mInterfacable;

	private PackedSprite mSelectedIcon;

	public PackedSprite mActionIcon;

	public PackedSprite mActionIconBG;

	public SpriteText CostText;

	public SpriteText InfoText;

	public UIButton m_ContextButton;

	public BoxCollider m_ContextButtonCollider;

	private bool mStartViewable;

	public bool ShouldBeDisabled { get; set; }

	public bool StartViewable
	{
		get
		{
			return mStartViewable;
		}
		set
		{
			mStartViewable = value;
		}
	}

	public void Awake()
	{
		mSelectedIcon = base.gameObject.GetComponent<PackedSprite>();
	}

	public override void Start()
	{
		mInterfacable = Target.GetComponent<InterfaceableObject>();
		base.Start();
		mActionIcon.Hide(true);
		mActionIconBG.Hide(true);
		InfoText.Hide(true);
		SetCost(0);
		if (!StartViewable)
		{
			mSelectedIcon.Hide(true);
			SwitchOff();
		}
		mSelectedIcon.SetFrame(0, 0);
		if (m_ContextButton != null)
		{
			m_ContextButton.gameObject.GetComponent<Renderer>().enabled = false;
		}
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, FinalPositionZOffset);
	}

	public bool IsInTriggerRange()
	{
		if (mInterfacable == null)
		{
			return true;
		}
		if (!GameController.Instance.IsFirstPerson)
		{
			return false;
		}
		if (GameController.Instance.mFirstPersonActor.baseCharacter.IsInASetPiece || GameController.Instance.IsPlayerBreaching)
		{
			return false;
		}
		return DistToFppSqrMag < mInterfacable.FirstPersonTriggerRadiusSqr;
	}

	public override void UpdateOnScreen()
	{
		if (mInterfacable != null)
		{
			if (mInterfacable.enabled && mInterfacable.gameObject.activeInHierarchy)
			{
				SwitchOn();
			}
			else
			{
				SwitchOff();
			}
		}
		base.UpdateOnScreen();
		if (base.IsSwitchedOn)
		{
			m_ContextButtonCollider.enabled = true;
			if (OverwatchController.Instance != null && OverwatchController.Instance.Active)
			{
				m_ContextButtonCollider.enabled = false;
			}
			else if (GameController.Instance.IsFirstPerson)
			{
				m_ContextButtonCollider.size = new Vector3(4f, 4f, 0f);
				if (!IsInTriggerRange() || ShouldBeDisabled)
				{
					m_ContextButtonCollider.enabled = false;
					if (mActionIcon.gameObject.transform.localScale.x == 1f)
					{
						mActionIcon.gameObject.ScaleTo(new Vector3(0f, 0f, 0f), 0.2f, 0f);
						mActionIconBG.gameObject.ScaleTo(new Vector3(0f, 0f, 0f), 0.2f, 0f);
					}
					if (!CostText.IsHidden())
					{
						CostText.Hide(true);
					}
					if (!InfoText.IsHidden())
					{
						InfoText.Hide(true);
					}
				}
				else
				{
					if (mActionIcon.gameObject.transform.localScale.x == 0f)
					{
						mActionIcon.gameObject.ScaleTo(new Vector3(1f, 1f, 1f), 0.2f, 0f);
						mActionIconBG.gameObject.ScaleTo(new Vector3(1f, 1f, 1f), 0.2f, 0f);
					}
					if (CommonHudController.Instance != null && !GameController.Instance.AimimgDownScopeThisFrame)
					{
						CommonHudController.Instance.SetContextInteractionButton(mInterfacable);
					}
					mActionIcon.Hide(false);
					mActionIconBG.Hide(false);
					CostText.Hide(false);
					InfoText.Hide(false);
				}
			}
			else
			{
				m_ContextButtonCollider.size = new Vector3(1.9f, 1.9f, 0f);
			}
		}
		else
		{
			m_ContextButtonCollider.enabled = false;
		}
		if (m_ContextButton != null && m_ContextButton.gameObject.GetComponent<Renderer>().enabled != base.GetComponent<Renderer>().enabled)
		{
			m_ContextButton.gameObject.GetComponent<Renderer>().enabled = base.GetComponent<Renderer>().enabled;
		}
	}

	public override void LateUpdate()
	{
		bool flag = (bool)OverwatchController.Instance && OverwatchController.Instance.Active;
		if (base.IsOnScreen && (CameraManager.Instance.ActiveCamera == CameraManager.ActiveCameraType.StrategyCamera || flag))
		{
			base.GetComponent<Renderer>().enabled = false;
		}
		base.LateUpdate();
	}

	public override void UpdateOffScreen()
	{
		m_ContextButtonCollider.enabled = false;
	}

	public override void JustGoneOffScreen()
	{
		mSelectedIcon.Hide(true);
		mActionIcon.Hide(true);
		mActionIconBG.Hide(true);
		CostText.Hide(true);
		InfoText.Hide(true);
	}

	public override void JustComeOnScreen()
	{
		if (mSelectedIcon.IsHidden())
		{
			mSelectedIcon.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
			mSelectedIcon.gameObject.ScaleTo(new Vector3(1f, 1f, 1f), 0.5f, 0f, EaseType.easeOutBounce);
			RealCharacter component = Target.GetComponent<RealCharacter>();
			if (component == null && Target.parent != null)
			{
				component = Target.parent.GetComponent<RealCharacter>();
			}
			if (component == null || !component.IsAHumanCharacter() || component.IsDead() || component.IsMortallyWounded())
			{
				mSelectedIcon.Hide(false);
			}
			if (GameController.Instance.IsFirstPerson)
			{
				mActionIcon.Hide(true);
				mActionIconBG.Hide(true);
				CostText.Hide(true);
				InfoText.Hide(true);
				mSelectedIcon.SetFrame(0, 1);
			}
			else
			{
				mActionIcon.Hide(true);
				mActionIconBG.Hide(true);
				CostText.Hide(true);
				InfoText.Hide(true);
				mSelectedIcon.SetFrame(0, 0);
			}
		}
	}

	protected override void SwitchToStrategyView()
	{
		SwitchOff();
	}

	protected override void SwitchToGameplayView()
	{
		SwitchOn();
	}

	public void SetActionIcon(ContextMenuIcons icon)
	{
		mActionIcon.SetFrame(0, (int)icon);
	}

	public void SetCost(int cost)
	{
		if (cost > 0)
		{
			CostText.gameObject.SetActive(true);
			CostText.Text = string.Format("{0} {1}", CommonHelper.HardCurrencySymbol(), cost.ToString());
		}
		else
		{
			CostText.gameObject.SetActive(false);
		}
	}

	public void SetInfoText(string text)
	{
		if (text == null || text == string.Empty)
		{
			InfoText.gameObject.SetActive(false);
			return;
		}
		InfoText.gameObject.SetActive(true);
		InfoText.Text = Language.Get(text);
		InfoText.Color = ColourChart.HudYellow;
	}
}
