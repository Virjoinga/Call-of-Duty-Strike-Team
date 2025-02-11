using UnityEngine;

public class LoadoutPerksPanel : MenuScreenBlade
{
	public SpriteText SelectedItemText;

	public SpriteText SelectedItemProText;

	public SpriteText SelectedItemDescription;

	public SpriteText LockedText;

	public PerkIconController SelectedItemProIcon;

	public ProgressBar SelectedItemProProgress;

	public PerkIconController LargeIcon;

	public PackedSprite LockedIcon;

	public PackedSprite NoneIcon;

	public FrontEndButton EquipButton;

	public FrontEndButton NowButton;

	public GameObject ProRoot;

	private CommonBackgroundBoxPlacement mProArea;

	private CommonBackgroundBox mBox;

	private string mCachedProDescription;

	private bool mPro;

	public override void Awake()
	{
		base.Awake();
		mBox = GetComponentInChildren<CommonBackgroundBox>();
		if (ProRoot != null)
		{
			mProArea = ProRoot.GetComponent<CommonBackgroundBoxPlacement>();
		}
	}

	public void Setup(Perk selected, bool locked, bool pro)
	{
		GameSettings instance = GameSettings.Instance;
		bool flag = selected.Identifier == PerkType.None;
		bool flag2 = !flag && instance.HasPerk(selected.Identifier);
		bool flag3 = selected.Identifier == PerkType.Perk1Greed || selected.Identifier == PerkType.Perk2Greed || selected.Identifier == PerkType.Perk3Greed;
		bool flag4 = selected.UnlockLevel != -1;
		mPro = pro;
		string text = selected.Identifier.ToString().ToUpper();
		if (SelectedItemText != null)
		{
			string text2 = Language.Get("S_" + text + ((!pro) ? string.Empty : "_PRO"));
			SelectedItemText.Text = text2;
			if (SelectedItemText.transform.parent != null)
			{
				SubtitleBackground component = SelectedItemText.transform.parent.GetComponent<SubtitleBackground>();
				if (component != null)
				{
					component.Resize();
				}
			}
		}
		if (SelectedItemProText != null)
		{
			string text3 = ((!flag) ? AutoLocalize.Get("S_PRO_UPGRADE_IN") : string.Empty);
			SelectedItemProText.Text = text3;
		}
		if (SelectedItemDescription != null)
		{
			mCachedProDescription = Language.Get("S_" + text + "_PRO_DESC");
			string text4 = ((!pro) ? Language.Get("S_" + text + "_DESC") : mCachedProDescription);
			SelectedItemDescription.Text = text4;
		}
		if (LargeIcon != null)
		{
			LargeIcon.SetPerk(selected.Identifier, pro, !locked);
			LargeIcon.Hide(flag);
		}
		if (LockedIcon != null && LockedText != null)
		{
			LockedIcon.Hide(!locked);
			LockedText.Hide(!locked);
			WeaponManager instance2 = WeaponManager.Instance;
			if (selected.UnlockLevel > 0)
			{
				string format = AutoLocalize.Get("S_WEAPON_UNLOCK_LEVEL");
				LockedText.Text = string.Format(format, selected.UnlockLevel);
			}
			else if (instance2 != null)
			{
				BundleDescriptor perkBundle = instance2.GetPerkBundle(selected.Identifier);
				if (perkBundle != null)
				{
					string format2 = AutoLocalize.Get("S_WEAPON_UNLOCK_BUNDLE");
					string arg = AutoLocalize.Get(perkBundle.Name);
					LockedText.Text = string.Format(format2, arg);
				}
			}
		}
		if (NoneIcon != null)
		{
			NoneIcon.Hide(!flag);
		}
		if (SelectedItemProIcon != null)
		{
			SelectedItemProIcon.SetPerk(selected.Identifier, true, !pro && !locked);
		}
		if (!flag && !pro && SelectedItemProProgress != null)
		{
			PerkStatus perkStatus = StatsManager.Instance.PerksManager().GetPerkStatus(selected.Identifier);
			float value = (float)perkStatus.ProXP / (float)selected.ProXPTarget;
			SelectedItemProProgress.SetValue(value);
		}
		else if (pro)
		{
			SelectedItemProProgress.SetValue(1f);
		}
		if (ProRoot != null)
		{
			ProRoot.SetActive(!flag && !flag3);
		}
		if (EquipButton != null)
		{
			if (flag)
			{
				EquipButton.Text = Language.Get("S_NONE");
			}
			else if (locked && flag4)
			{
				string arg2 = Language.Get("S_UNLOCK");
				char c = CommonHelper.HardCurrencySymbol();
				string arg3 = string.Format("{0}{1}", c, selected.UnlockCost);
				EquipButton.Text = string.Format("{0} {1}", arg2, arg3);
			}
			else if (flag2)
			{
				EquipButton.Text = Language.Get("S_ACTIVE_PERK");
			}
			else if (locked)
			{
				EquipButton.Text = Language.Get("S_LOCKED");
			}
			else
			{
				EquipButton.Text = Language.Get("S_EQUIP");
			}
			EquipButton.CurrentState = ((flag2 || (locked && !flag4)) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
		}
		if (NowButton != null)
		{
			string arg4 = Language.Get("S_NOW");
			char c2 = CommonHelper.HardCurrencySymbol();
			string arg5 = string.Format("{0}{1}", c2, selected.ProCost);
			NowButton.Text = ((!pro) ? string.Format("{0} {1}", arg4, arg5) : Language.Get("S_FULLY_UPGRADED"));
			NowButton.CurrentState = ((flag || pro || flag3 || locked) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
		}
		if (mBox != null)
		{
			mBox.RefreshPlacements = true;
			mBox.Resize();
		}
		ToolTipController.Instance.ClearToolTip();
	}

	private void OnEnable()
	{
		FingerGestures.OnFingerStationary += FingerGestures_OnFingerStationary;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerStationary -= FingerGestures_OnFingerStationary;
	}

	private void FingerGestures_OnFingerStationary(int fingerIndex, Vector2 fingerPos, float elapsedTime)
	{
		if (!mPro && mProArea != null && mProArea.BoundingRect.Contains(fingerPos))
		{
			string text = string.Format("{0}\n{1}", mCachedProDescription, Language.Get("S_HOW_TO_UNLOCK_PRO_PERK"));
			ToolTipController.Instance.DoTooltip(text, ProRoot);
		}
	}
}
