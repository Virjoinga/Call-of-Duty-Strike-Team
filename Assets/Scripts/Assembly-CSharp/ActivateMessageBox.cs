using System.Collections;
using UnityEngine;

public class ActivateMessageBox : MessageBox
{
	public CommonBackgroundBoxPlacement TextLayout;

	public SpriteText Category1;

	public SpriteText Category1SubText;

	public SpriteText Category2;

	public SpriteText Category2SubText;

	public SpriteText Category3;

	public SpriteText Category3SubText;

	public override void Awake()
	{
		base.Awake();
	}

	public override IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		InterfaceSFX.Instance.MessageBoxOn.Play2D();
		SetText(Title, Message, messageIsTranslated);
		if (Category1SubText != null && Category1SubText.Text.Contains("{0}"))
		{
			Category1SubText.Text = string.Format(Category1SubText.Text, CommonHelper.HardCurrencySymbol());
		}
		if (Category2SubText != null && Category2SubText.Text.Contains("{0}"))
		{
			Category2SubText.Text = string.Format(Category2SubText.Text, CommonHelper.HardCurrencySymbol());
		}
		if (Category3SubText != null && Category3SubText.Text.Contains("{0}"))
		{
			Category3SubText.Text = string.Format(Category3SubText.Text, CommonHelper.HardCurrencySymbol());
		}
		ButtonData[0].Result = MessageBoxResults.Result.LargeOK;
		CreateAndPositionButtons();
		mAnimator.AnimateOpen();
		while (mAnimator.IsOpening)
		{
			UIButton[] buttons = m_buttons;
			foreach (UIButton button in buttons)
			{
				button.collider.enabled = !button.collider.enabled;
				button.collider.enabled = !button.collider.enabled;
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
