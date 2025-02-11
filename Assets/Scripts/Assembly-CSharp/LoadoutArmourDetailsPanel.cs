using System.Globalization;

public class LoadoutArmourDetailsPanel : MenuScreenBlade
{
	public EquipmentIconController ItemImage;

	public SpriteText SelectedItemText;

	public SpriteText SelectedItemDescription;

	public SpriteText SelectedItemCost;

	public ProgressBar UpgradeProgress;

	public FrontEndButton UpgradeButton;

	public void Setup(EquipmentDescriptor current, EquipmentDescriptor next)
	{
		NumberFormatInfo numberFormat = GlobalizationUtils.GetNumberFormat(0);
		if (SelectedItemText != null)
		{
			string text = AutoLocalize.Get((!(next != null)) ? current.Name : next.Name);
			SelectedItemText.Text = text.ToUpper();
			if (SelectedItemText.transform.parent != null)
			{
				SubtitleBackground component = SelectedItemText.transform.parent.GetComponent<SubtitleBackground>();
				if (component != null)
				{
					component.Resize();
				}
			}
		}
		if (ItemImage != null)
		{
			ItemImage.SetEquipment((!(next != null)) ? current.Type : next.Type, true);
		}
		if (SelectedItemDescription != null)
		{
			string text2 = AutoLocalize.Get((!(next != null)) ? current.LongDescription : next.LongDescription);
			text2 = text2 + " " + Language.GetFormatString("S_ARMOUR_VALUE_DESC", (!(next != null)) ? current.Protection.ToString() : next.Protection.ToString());
			SelectedItemDescription.Text = text2;
		}
		if (UpgradeProgress != null)
		{
			float num = ((!(current != null)) ? 0f : ((float)current.Armour));
			float num2 = ((!(next != null)) ? 4f : ((float)next.Armour));
			float value = num / 4f;
			UpgradeProgress.SetValue(value);
			value = num2 / 4f;
			UpgradeProgress.SetPreviewValue(value);
		}
		if (SelectedItemCost != null)
		{
			if (next != null)
			{
				char c = CommonHelper.HardCurrencySymbol();
				string format = AutoLocalize.Get("S_ARMOUR_UPGRADE");
				string arg = string.Format("{0}{1}", c, next.HardCost.ToString("N", numberFormat));
				SelectedItemCost.Text = string.Format(format, arg);
			}
			else
			{
				SelectedItemCost.Text = Language.Get("S_FULLY_UPGRADED");
			}
		}
		if (UpgradeButton != null)
		{
			UpgradeButton.CurrentState = ((!(next != null)) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
		}
		if (next == null)
		{
			TitleBarController.Instance.HighlightBackButton(true);
		}
	}
}
