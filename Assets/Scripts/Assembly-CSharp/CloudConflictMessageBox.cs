using System.Collections;
using System.Globalization;
using UnityEngine;

public class CloudConflictMessageBox : MessageBox
{
	public SpriteText LocalLastPlayed;

	public SpriteText OnlineLastPlayed;

	public SpriteText LocalLastTotalXP;

	public SpriteText OnlineLastTotalXP;

	public SpriteText LocalLastTotalCurrency;

	public SpriteText OnlineLastTotalCurrency;

	private int mRecommendedButtonIndex;

	public void SetupData(SecureStorageMetadata local, SecureStorageMetadata online)
	{
		NumberFormatInfo numberFormat = GlobalizationUtils.GetNumberFormat(0);
		if (LocalLastPlayed != null && OnlineLastPlayed != null)
		{
			LocalLastPlayed.Text = local.LastPlayedDateTime;
			OnlineLastPlayed.Text = online.LastPlayedDateTime;
		}
		if (LocalLastTotalXP != null && OnlineLastTotalXP != null)
		{
			string text = Language.Get("S_RESULT_XP");
			LocalLastTotalXP.Text = local.TotalXP + text;
			OnlineLastTotalXP.Text = online.TotalXP + text;
		}
		if (LocalLastTotalCurrency != null && OnlineLastTotalCurrency != null)
		{
			char c = CommonHelper.HardCurrencySymbol();
			LocalLastTotalCurrency.Text = string.Format("{0}{1}", c, local.HardCurrencyPurchased.ToString("N", numberFormat));
			OnlineLastTotalCurrency.Text = string.Format("{0}{1}", c, online.HardCurrencyPurchased.ToString("N", numberFormat));
		}
		mRecommendedButtonIndex = ((local.TotalXP > online.TotalXP || local.HardCurrencyPurchased > online.HardCurrencyPurchased) ? 1 : 0);
	}

	public override IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		InterfaceSFX.Instance.MessageBoxOn.Play2D();
		SetText(Title, Message, messageIsTranslated);
		CreateAndPositionButtons();
		for (int count = 0; count < m_buttons.Length; count++)
		{
			if (m_buttons[count] != null)
			{
				FrontEndButton feButton = m_buttons[count].GetComponent<FrontEndButton>();
				if (feButton != null)
				{
					feButton.CurrentState = ((count == mRecommendedButtonIndex) ? FrontEndButton.State.Highlighted : FrontEndButton.State.Normal);
				}
			}
		}
		mAnimator.AnimateOpen();
		while (mAnimator.IsOpening)
		{
			UIButton[] buttons = m_buttons;
			foreach (UIButton button in buttons)
			{
				button.GetComponent<Collider>().enabled = !button.GetComponent<Collider>().enabled;
				button.GetComponent<Collider>().enabled = !button.GetComponent<Collider>().enabled;
			}
			yield return new WaitForEndOfFrame();
		}
		while (mInternalResult == MessageBoxResults.Result.Unknown)
		{
			yield return new WaitForEndOfFrame();
		}
		mAnimator.AnimateClosed();
		MenuSFX.Instance.MenuBoxClose.Play2D();
		while (mAnimator.IsClosing)
		{
			yield return new WaitForEndOfFrame();
		}
		if (Results != null)
		{
			Results.InvokeMethodForResult(mInternalResult);
		}
		Object.Destroy(base.gameObject);
	}
}
