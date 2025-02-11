using System.Collections;
using System.Globalization;
using UnityEngine;

public class ChallengeMessageBox : MessageBox
{
	public PackedSprite GoldMedal;

	public PackedSprite SilverMedal;

	public PackedSprite BronzeMedal;

	public SpriteText RewardText;

	private ChallengeMedalType mMedal;

	private uint mReward;

	public void Setup(ChallengeMedalType medal, uint reward)
	{
		mMedal = medal;
		mReward = reward;
	}

	public override IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		InterfaceSFX.Instance.MessageBoxOn.Play2D();
		m_titleText.Text = Language.Get(Title);
		m_bodyText.Text = Message;
		if (GoldMedal != null && mMedal != ChallengeMedalType.Gold)
		{
			GoldMedal.Hide(true);
		}
		if (SilverMedal != null && mMedal != ChallengeMedalType.Silver)
		{
			SilverMedal.Hide(true);
		}
		if (BronzeMedal != null && mMedal != ChallengeMedalType.Bronze)
		{
			BronzeMedal.Hide(true);
		}
		NumberFormatInfo nfi = GlobalizationUtils.GetNumberFormat(0);
		char token = CommonHelper.HardCurrencySymbol();
		if (RewardText != null)
		{
			RewardText.Text = string.Format("{0}{1}", token, mReward.ToString("N", nfi));
		}
		mAnimator.AnimateOpen();
		while (mAnimator.IsOpening)
		{
			yield return new WaitForEndOfFrame();
		}
		CreateAndPositionButtons();
		RepositionButtons();
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
