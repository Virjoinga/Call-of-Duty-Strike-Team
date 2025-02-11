using UnityEngine;

public class LoadoutIncreaseSlotSizePanel : MonoBehaviour
{
	public ProgressBar Progress;

	public SpriteText Description;

	public SpriteText Increase;

	public FrontEndButton Button;

	private void Awake()
	{
		Transform transform = Button.transform.parent.FindChild("control_text");
		if (transform != null)
		{
			transform.parent = Button.transform;
			transform.localPosition = Vector3.zero;
		}
	}

	public void SetSlot(int slot)
	{
		GameSettings instance = GameSettings.Instance;
		if (!(instance != null) || slot >= instance.Equipment.Length)
		{
			return;
		}
		EquipmentDescriptor descriptor = instance.Equipment[slot].Descriptor;
		int slotSize = instance.Equipment[slot].SlotSize;
		int totalSlotSize = descriptor.TotalSlotSize;
		int num = instance.CalculateSlotIncrease(slot);
		int num2 = ((num > 0) ? instance.CalculateCostOfIncrease(descriptor.name, slot) : 0);
		string format = AutoLocalize.Get("S_INCREASE_CAPACITY_DESC");
		string text = AutoLocalize.Get((num <= 0) ? "S_INCREASE_CAPACITY_FULL" : "S_INCREASE_CAPACITY_INCREASE");
		string text2 = AutoLocalize.Get(descriptor.Name + "_PLURAL");
		if (Description != null)
		{
			Description.Text = string.Format(format, slotSize, text2.ToUpper());
		}
		if (Increase != null)
		{
			Increase.Text = ((num <= 0) ? text : string.Format(text, num));
		}
		if (Button != null)
		{
			if (num > 0)
			{
				char c = CommonHelper.HardCurrencySymbol();
				Button.Text = string.Format("{0}{1}", c, num2);
				Button.CurrentState = FrontEndButton.State.Normal;
			}
			else
			{
				bool flag = slotSize < totalSlotSize;
				Button.Text = Language.Get((!flag) ? "S_MAX_REACHED" : "S_GET_MORE_BUNDLES");
				Button.CurrentState = FrontEndButton.State.Disabled;
			}
		}
		if (Progress != null)
		{
			float value = (float)slotSize / (float)totalSlotSize;
			Progress.SetValue(value);
			value = (float)(slotSize + num) / (float)totalSlotSize;
			Progress.SetPreviewValue(value);
		}
	}
}
