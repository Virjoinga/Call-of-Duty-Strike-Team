using System.Globalization;

public class LoadoutEquipmentPanel : MenuScreenBlade
{
	public SpriteText SelectedItemText;

	public SpriteText SelectedItemDescription;

	public SpriteText SelectedItemCost;

	public SpriteText SelectedItemFillSlotCost;

	public SpriteText SelectedItemCount;

	public SpriteText CurrentlyOwnedText;

	public FrontEndButton PurchaseOneButton;

	public FrontEndButton PurchaseFillButton;

	public EquipmentIconController ItemImage;

	public PackedSprite LockedIcon;

	public void SetupForEquipment(EquipmentDescriptor item, int slotsFree, int fillPrice, bool locked)
	{
		NumberFormatInfo numberFormat = GlobalizationUtils.GetNumberFormat(0);
		if (SelectedItemText != null)
		{
			string text = AutoLocalize.Get(item.Name);
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
			ItemImage.Hide(locked);
			ItemImage.SetEquipment(item.Type, !locked);
		}
		if (SelectedItemDescription != null)
		{
			string text2 = AutoLocalize.Get(item.LongDescription);
			SelectedItemDescription.Text = text2;
			SelectedItemDescription.renderer.enabled = true;
		}
		char c = CommonHelper.HardCurrencySymbol();
		if (SelectedItemCost != null)
		{
			if (slotsFree > 0)
			{
				string format = Language.Get("S_EQUIPMENT_BUY_ONE");
				string arg = string.Format("{0}{1}", c, item.HardCost.ToString("N", numberFormat));
				SelectedItemCost.Text = string.Format(format, arg);
			}
			else
			{
				SelectedItemCost.Text = string.Empty;
			}
		}
		if (SelectedItemFillSlotCost != null)
		{
			if (slotsFree > 0)
			{
				string format2 = Language.Get("S_EQUIPMENT_BUY_FILL");
				string arg2 = string.Format("{0}{1}", c, fillPrice.ToString("N", numberFormat));
				SelectedItemFillSlotCost.Text = string.Format(format2, slotsFree, arg2);
			}
			else
			{
				SelectedItemFillSlotCost.Text = Language.Get("S_CAPACITY_FULL");
			}
		}
		if (PurchaseOneButton != null)
		{
			PurchaseOneButton.CurrentState = ((slotsFree <= 0) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
		}
		if (PurchaseFillButton != null)
		{
			PurchaseFillButton.CurrentState = ((slotsFree <= 0) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
		}
		if (SelectedItemCount != null)
		{
			SelectedItemCount.gameObject.SetActive(false);
		}
		if (CurrentlyOwnedText != null)
		{
			CurrentlyOwnedText.gameObject.SetActive(false);
		}
		if (LockedIcon != null)
		{
			LockedIcon.Hide(!locked);
		}
	}
}
