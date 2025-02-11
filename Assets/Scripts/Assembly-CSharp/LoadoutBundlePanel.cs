using UnityEngine;

public class LoadoutBundlePanel : MenuScreenBlade
{
	public SpriteText Name;

	public SpriteText PerkName;

	public SpriteText PerkDescription;

	public SpriteText EquipmentName;

	public SpriteText EquipmentDescription;

	public SpriteText WeaponName;

	public PerkIconController Perk;

	public EquipmentIconController Equipment;

	public WeaponIconController Weapon;

	public FrontEndButton Button;

	private BundleDescriptor mBundle;

	private string mWeaponTooltipString;

	public override void Awake()
	{
		CommonBackgroundBox componentInChildren = GetComponentInChildren<CommonBackgroundBox>();
		EZScreenPlacement componentInChildren2 = GetComponentInChildren<EZScreenPlacement>();
		if (componentInChildren != null && componentInChildren2 != null)
		{
			if (TBFUtils.UseAlternativeLayout())
			{
				componentInChildren.SmallScreenUnitsWide = 19.8f;
				componentInChildren.SmallScreenForegroundHeightInUnits = 14f;
				componentInChildren2.smallScreenDevicePos.x = 156f;
				componentInChildren2.screenPos = componentInChildren2.smallScreenDevicePos * 2f;
			}
			else
			{
				componentInChildren.UnitsWide = 20f;
			}
		}
		base.Awake();
	}

	public void OnEnable()
	{
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
	}

	public void OnDisable()
	{
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
	}

	public void Refresh(BundleDescriptor bundle)
	{
		if (!(bundle != null))
		{
			return;
		}
		mBundle = bundle;
		if (Name != null)
		{
			Name.Text = Language.Get(mBundle.Name);
			SubtitleBackground component = Name.GetComponent<SubtitleBackground>();
			if (component != null)
			{
				component.Resize();
			}
		}
		if (Weapon != null && WeaponName != null)
		{
			WeaponName.Text = mBundle.Weapon.Name;
			Weapon.SetWeapon(mBundle.Weapon.Type, true);
		}
		if (Equipment != null)
		{
			Equipment.SetEquipment(mBundle.EquipmentIcon, true);
		}
		if (Perk != null)
		{
			Perk.SetPerk(mBundle.Perk, false, true);
		}
		if (Button != null)
		{
			char c = CommonHelper.HardCurrencySymbol();
			GameSettings instance = GameSettings.Instance;
			if (instance != null)
			{
				bool flag = instance.WasUnlockedEarly(mBundle.Weapon.Name);
				Button.CurrentState = (flag ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
				Button.Text = (flag ? Language.Get("S_OWNED") : string.Format("{0}{1}", c, mBundle.HardCost));
			}
		}
		if (PerkName != null && PerkDescription != null)
		{
			PerkName.Text = Language.Get("S_" + mBundle.Perk.ToString().ToUpper()).ToUpper();
			PerkDescription.Text = Language.Get("S_" + mBundle.Perk.ToString().ToUpper() + "_DESC").ToUpper();
		}
		string text = ((mBundle.NumItemsPerEquipment <= 1) ? string.Empty : string.Format("{0}x ", mBundle.NumItemsPerEquipment));
		string text2 = string.Empty;
		for (int i = 0; i < mBundle.Equipment.Length; i++)
		{
			text += Language.Get(mBundle.Equipment[i].Name);
			text2 += Language.Get(mBundle.Equipment[i].ShortDescription);
			if (i + 1 < mBundle.Equipment.Length)
			{
				text += " - ";
				text2 += " ";
			}
		}
		if (EquipmentName != null && EquipmentDescription != null)
		{
			EquipmentName.Text = text.ToUpper();
			EquipmentDescription.Text = text2.ToUpper();
		}
		mWeaponTooltipString = Language.Get(mBundle.Weapon.ShortDescription).ToUpper();
	}

	private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		float pixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		if (CommonHelper.CreateRect(Weapon.Sprite, pixelSize).Contains(fingerPos))
		{
			ToolTipController.Instance.DoTooltip(mWeaponTooltipString, Weapon.gameObject);
		}
	}
}
