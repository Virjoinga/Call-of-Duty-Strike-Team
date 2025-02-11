public class LoadoutWeaponPanel : MenuScreenBlade
{
	public SpriteText SelectedWeaponText;

	public SpriteText CurrentWeaponText;

	public SpriteText ButtonText;

	public SpriteText SilencedText;

	public SpriteText LockedText;

	public WeaponIconController WeaponImage;

	public PackedSprite CurrentKey;

	public PackedSprite LockedIcon;

	public FrontEndButton EquipButton;

	public void Setup(WeaponDescriptor selected, WeaponDescriptor current, bool locked)
	{
		if (SelectedWeaponText != null)
		{
			SelectedWeaponText.Text = selected.Name;
			if (SelectedWeaponText.transform.parent != null)
			{
				SubtitleBackground component = SelectedWeaponText.transform.parent.GetComponent<SubtitleBackground>();
				if (component != null)
				{
					component.Resize();
				}
			}
		}
		if (CurrentWeaponText != null)
		{
			CurrentWeaponText.Text = current.Name;
			CurrentWeaponText.Hide(selected == current);
		}
		if (WeaponImage != null)
		{
			WeaponImage.SetWeapon(selected.Type, !locked);
		}
		if (CurrentKey != null)
		{
			CurrentKey.Hide(selected == current);
		}
		if (LockedIcon != null && LockedText != null)
		{
			LockedIcon.Hide(!locked);
			LockedText.Hide(!locked);
			WeaponManager instance = WeaponManager.Instance;
			if (selected.UnlockLevel > 0)
			{
				string format = AutoLocalize.Get("S_WEAPON_UNLOCK_LEVEL");
				LockedText.Text = string.Format(format, selected.UnlockLevel);
			}
			else if (instance != null)
			{
				BundleDescriptor weaponBundle = instance.GetWeaponBundle(selected);
				if (weaponBundle != null)
				{
					string format2 = AutoLocalize.Get("S_WEAPON_UNLOCK_BUNDLE");
					string arg = AutoLocalize.Get(weaponBundle.Name);
					LockedText.Text = string.Format(format2, arg);
				}
			}
		}
		if (ButtonText != null && EquipButton != null)
		{
			if (locked)
			{
				ButtonText.Text = AutoLocalize.Get("S_LOCKED");
				EquipButton.CurrentState = FrontEndButton.State.Disabled;
			}
			else if (selected == current)
			{
				ButtonText.Text = AutoLocalize.Get("S_EQUIPPED");
				EquipButton.CurrentState = FrontEndButton.State.Disabled;
			}
			else
			{
				ButtonText.Text = AutoLocalize.Get("S_EQUIP");
				EquipButton.CurrentState = FrontEndButton.State.Normal;
			}
		}
		if (SilencedText != null)
		{
			SilencedText.Hide(!WeaponUtils.IsWeaponSilenced(selected));
		}
	}
}
