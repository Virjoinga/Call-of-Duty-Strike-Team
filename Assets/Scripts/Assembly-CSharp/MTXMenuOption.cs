using System.Globalization;
using UnityEngine;

public class MTXMenuOption : MonoBehaviour
{
	public SpriteText Title;

	public SpriteText Product;

	public SpriteText Price;

	public SpriteText Value;

	public SpriteText SalePreValue;

	public SpriteText SaleEndsIn;

	public SpriteText SaleText;

	public PackedSprite SaleIcon;

	public UIButton Button;

	public void ClearOption(int product)
	{
		SetupOption(string.Empty, string.Empty, string.Empty, 0, null, string.Empty);
		ClearSale();
	}

	public void SetupOption(string titleString, string valueKey, string price, int product, MonoBehaviour callbackScript, string callback)
	{
		NumberFormatInfo numberFormat = GlobalizationUtils.GetNumberFormat(0);
		if (Title != null)
		{
			Title.Text = titleString;
		}
		if (Value != null)
		{
			if (valueKey != string.Empty)
			{
				Value.Text = AutoLocalize.Get(valueKey);
			}
			else
			{
				Value.Text = string.Empty;
			}
		}
		if (Price != null)
		{
			Price.Text = price;
		}
		if (Product != null)
		{
			char c = CommonHelper.HardCurrencySymbol();
			Product.Text = ((product != 0) ? string.Format("{0}{1}", c, product.ToString("N", numberFormat)) : string.Empty);
		}
		if (Button != null)
		{
			Button.scriptWithMethodToInvoke = callbackScript;
			Button.methodToInvoke = callback;
		}
	}

	public void SetupTokenSale(float productDiscounted, float timeRemainingInSeconds)
	{
		NumberFormatInfo numberFormat = GlobalizationUtils.GetNumberFormat(0);
		if (SalePreValue != null)
		{
			SalePreValue.gameObject.SetActive(true);
			SalePreValue.Text = Product.Text;
			char c = CommonHelper.HardCurrencySymbol();
			Product.Text = ((productDiscounted != 0f) ? string.Format("{0}{1}", c, productDiscounted.ToString("N", numberFormat)) : string.Empty);
		}
		if (SaleEndsIn != null)
		{
			SaleEndsIn.gameObject.SetActive(true);
			SaleEndsIn.Text = FormatDescription(timeRemainingInSeconds);
		}
		if (SaleText != null)
		{
			SaleText.gameObject.SetActive(true);
		}
		if (SaleIcon != null)
		{
			SaleIcon.gameObject.SetActive(true);
		}
		if (Value != null)
		{
			Value.gameObject.SetActive(false);
		}
	}

	public void ClearSale()
	{
		if (SalePreValue != null)
		{
			SalePreValue.gameObject.SetActive(false);
		}
		if (SaleEndsIn != null)
		{
			SaleEndsIn.gameObject.SetActive(false);
		}
		if (SaleText != null)
		{
			SaleText.gameObject.SetActive(false);
		}
		if (SaleIcon != null)
		{
			SaleIcon.gameObject.SetActive(false);
		}
		if (Value != null)
		{
			Value.gameObject.SetActive(true);
		}
	}

	public void OnScreen()
	{
		if (Button != null)
		{
			Button.collider.enabled = !Button.collider.enabled;
			Button.collider.enabled = !Button.collider.enabled;
		}
	}

	private string FormatDescription(float timeRemaining)
	{
		int num = Mathf.FloorToInt(timeRemaining) / 60;
		int num2 = Mathf.FloorToInt(num) / 60;
		num -= 60 * num2;
		string format = Language.Get("S_SALE_BANNER_TIME_REMAINING");
		return string.Format(format, num2, num);
	}
}
