using System.Globalization;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
	public Scale9Grid Brackets;

	public SpriteText ItemName;

	public SpriteText ItemDescription;

	public SpriteText HardCost;

	public SpriteText SoftCost;

	public EquipmentIconController ItemImage;

	public PackedSprite LockedIcon;

	public UIButton BuyHardButton;

	public UIButton BuySoftButton;

	private EquipmentDescriptor mEquipment;

	private PurchaseFlowHelper.PurchaseData mData;

	private MonoBehaviour mPurchaseCallback;

	private bool mLocked;

	private bool mDirty;

	public void SetToItem(MonoBehaviour purchaseCallback, EquipmentDescriptor equipment, bool locked)
	{
		NumberFormatInfo numberFormat = GlobalizationUtils.GetNumberFormat(0);
		UpdateBouncyButton(BuyHardButton, "BuyItemWithHardCurrency");
		UpdateBouncyButton(BuySoftButton, "BuyItemWithSoftCurrency");
		if (equipment != null)
		{
			if (ItemName != null && ItemDescription != null)
			{
				ItemName.Text = AutoLocalize.Get(equipment.Name);
				ItemDescription.Text = AutoLocalize.Get(equipment.ShortDescription);
			}
			if (HardCost != null && SoftCost != null)
			{
				char c = CommonHelper.HardCurrencySymbol();
				HardCost.Text = string.Format("{0}{1}", c, equipment.HardCost.ToString("N", numberFormat));
			}
			if (ItemImage != null)
			{
				ItemImage.Hide(locked);
				ItemImage.SetEquipment(equipment.Type, !locked);
			}
			if (LockedIcon != null)
			{
				LockedIcon.Hide(!locked);
			}
			mData = new PurchaseFlowHelper.PurchaseData();
			mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Equipment;
			mData.EquipmentItem = equipment;
			mData.ScriptToCallWithResult = this;
			mData.MethodToCallWithResult = "ItemPurchaseResult";
			mPurchaseCallback = purchaseCallback;
			mEquipment = equipment;
			mLocked = locked;
			mDirty = false;
		}
	}

	public void LayoutComponents(float width, float height, float bracketsLeftBorder, float bracketsRightBorder, float bracketsYBorder)
	{
		if (Brackets != null)
		{
			Brackets.size.x = width + bracketsLeftBorder + bracketsRightBorder;
			Brackets.size.y = height - bracketsYBorder * 2f;
			Brackets.Resize();
		}
		Vector3 position = Camera.main.WorldToScreenPoint(base.transform.position);
		position.x -= bracketsLeftBorder - bracketsRightBorder;
		position.y += bracketsYBorder * 2f;
		Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
		Brackets.transform.position = position2;
		Vector2 boxSize = new Vector2(width, height);
		CommonBackgroundBoxPlacement[] componentsInChildren = GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		CommonBackgroundBoxPlacement[] array = componentsInChildren;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.Position(base.transform.position, boxSize);
		}
	}

	private void LateUpdate()
	{
		if (!mDirty)
		{
			return;
		}
		if (mEquipment != null)
		{
			if (ItemImage != null)
			{
				ItemImage.gameObject.SetActive(!mLocked);
			}
			if (LockedIcon != null)
			{
				LockedIcon.gameObject.SetActive(mLocked);
			}
		}
		mDirty = false;
	}

	private void OnEnable()
	{
		mDirty = true;
	}

	private void UpdateBouncyButton(UIButton button, string method)
	{
		button.methodToInvoke = method;
		button.scriptWithMethodToInvoke = this;
		BouncyButton component = button.gameObject.GetComponent<BouncyButton>();
		if (component != null)
		{
			component.ReplaceButtonScript();
		}
	}

	private void BuyItemWithHardCurrency()
	{
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		if (instance != null && mData != null)
		{
			instance.Purchase(mData);
		}
	}

	private void ItemPurchaseResult()
	{
		mDirty = true;
		if (mPurchaseCallback != null)
		{
			mPurchaseCallback.SendMessage("ItemPurchased", mEquipment);
		}
	}
}
