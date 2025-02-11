using System.Collections;
using UnityEngine;

public class TBFInfoMessageBox : MessageBox
{
	public CommonBackgroundBoxPlacement TBFFacebookButtonPosition;

	public CommonBackgroundBoxPlacement CODFacebookButtonPosition;

	public CommonBackgroundBoxPlacement TBFTwitterButtonPosition;

	public CommonBackgroundBoxPlacement CODTwitterButtonPosition;

	private GlobeCamera mGlobeSelectCamera;

	private CreditsController mCredits;

	public override void Awake()
	{
		base.Awake();
		mGlobeSelectCamera = Object.FindObjectOfType(typeof(GlobeCamera)) as GlobeCamera;
		mCredits = Object.FindObjectOfType(typeof(CreditsController)) as CreditsController;
	}

	public override IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		InterfaceSFX.Instance.MessageBoxOn.Play2D();
		SetText(Title, Message, messageIsTranslated);
		CreateAndPositionButtons();
		mAnimator.AnimateOpen();
		while (mAnimator.IsOpening)
		{
			yield return new WaitForEndOfFrame();
		}
		CreateLinkButton(CODFacebookButtonPosition, ButtonIcon.Facebook, "CODFacebookButtonPressed");
		CreateLinkButton(TBFTwitterButtonPosition, ButtonIcon.Twitter, "TBFTwitterButtonPressed");
		CreateLinkButton(CODTwitterButtonPosition, ButtonIcon.Twitter, "CODTwitterButtonPressed");
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

	private void CreateLinkButton(CommonBackgroundBoxPlacement position, ButtonIcon icon, string callback)
	{
		GameObject gameObject = Object.Instantiate(IconButtonPrefab) as GameObject;
		gameObject.transform.parent = position.transform;
		Vector3 position2 = new Vector3(position.BoundingRect.x + position.BoundingRect.width * 0.5f, position.BoundingRect.y + position.BoundingRect.height * 0.5f);
		Vector3 position3 = Camera.main.ScreenToWorldPoint(position2);
		position3.z = position.transform.position.z - 0.1f;
		gameObject.transform.position = position3;
		UIButton componentInChildren = gameObject.GetComponentInChildren<UIButton>();
		if (componentInChildren != null)
		{
			componentInChildren.scriptWithMethodToInvoke = this;
			componentInChildren.methodToInvoke = callback;
			PackedSprite componentInChildren2 = componentInChildren.GetComponentInChildren<PackedSprite>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.SetFrame(0, (int)icon);
			}
		}
	}

	private void TBFFacebookButtonPressed()
	{
		TBFUtils.LaunchURL(SwrveServerVariables.Instance.TBFFacebookURL);
		SwrveEventsUI.ViewedTBFFacebook();
		mInternalResult = MessageBoxResults.Result.Cancel;
	}

	private void CODFacebookButtonPressed()
	{
		TBFUtils.LaunchURL(SwrveServerVariables.Instance.CODFacebookURL);
		SwrveEventsUI.ViewedCODFacebook();
		mInternalResult = MessageBoxResults.Result.Cancel;
	}

	private void TBFTwitterButtonPressed()
	{
		TBFUtils.LaunchURL(SwrveServerVariables.Instance.TBFTwitterURL);
		SwrveEventsUI.ViewedTBFTwitter();
		mInternalResult = MessageBoxResults.Result.Cancel;
	}

	private void CODTwitterButtonPressed()
	{
		TBFUtils.LaunchURL(SwrveServerVariables.Instance.CODTwitterURL);
		SwrveEventsUI.ViewedCODTwitter();
		mInternalResult = MessageBoxResults.Result.Cancel;
	}

	private void CreditsButtonPressed()
	{
		if (mCredits != null && mGlobeSelectCamera != null)
		{
			mCredits.BeginSequence();
			mGlobeSelectCamera.ClearFocusMission();
		}
		mInternalResult = MessageBoxResults.Result.Cancel;
	}
}
