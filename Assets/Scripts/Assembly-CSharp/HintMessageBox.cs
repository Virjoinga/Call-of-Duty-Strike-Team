using System.Collections;
using UnityEngine;

public class HintMessageBox : MessageBox
{
	public enum ImageLayout
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}

	public SimpleSprite HintSprite;

	public CommonBackgroundBoxPlacement SpriteLayout;

	public CommonBackgroundBoxPlacement TextLayout;

	private Texture2D mTexture;

	public override void Awake()
	{
		float num = 864f;
		float num2 = 480f;
		bool flag = TBFUtils.UseAlternativeLayout();
		if (flag)
		{
			num = (float)Screen.width * 0.9f;
			num2 = (float)Screen.height * 0.75f;
		}
		base.Awake();
		mBgBox.UnitsWide = num / 32f;
		mBgBox.ForegroundHeightInUnits = num2 / 32f;
		mBgBox.AdjustForRetina = !flag;
	}

	public void LoadImageAndLayoutComponents(ImageLayout imageLayout, string imageLocation)
	{
		LoadTexture(imageLocation);
		float startPositionAsPercentageOfBoxWidth2;
		float num3;
		float num;
		float startPositionAsPercentageOfBoxWidth;
		float widthAsPercentageOfBoxWidth;
		float num2;
		switch (imageLayout)
		{
		default:
			startPositionAsPercentageOfBoxWidth2 = 0f;
			num3 = (num = 0.1f);
			startPositionAsPercentageOfBoxWidth = (widthAsPercentageOfBoxWidth = 0.5f);
			num2 = 0.9f;
			break;
		case ImageLayout.Right:
			startPositionAsPercentageOfBoxWidth = 0f;
			num3 = (num = 0.1f);
			startPositionAsPercentageOfBoxWidth2 = (widthAsPercentageOfBoxWidth = 0.5f);
			num2 = 0.9f;
			break;
		case ImageLayout.Top:
			startPositionAsPercentageOfBoxWidth2 = (startPositionAsPercentageOfBoxWidth = 0f);
			num3 = 0.1f;
			num2 = 0.45f;
			num = num3 + num2;
			widthAsPercentageOfBoxWidth = 1f;
			break;
		case ImageLayout.Bottom:
			startPositionAsPercentageOfBoxWidth2 = (startPositionAsPercentageOfBoxWidth = 0f);
			num = 0.1f;
			num2 = 0.45f;
			num3 = num + num2;
			widthAsPercentageOfBoxWidth = 1f;
			break;
		}
		if (SpriteLayout != null)
		{
			SpriteLayout.StartPositionAsPercentageOfBoxWidth = startPositionAsPercentageOfBoxWidth2;
			SpriteLayout.StartPositionAsPercentageOfBoxHeight = num3;
			SpriteLayout.WidthAsPercentageOfBoxWidth = widthAsPercentageOfBoxWidth;
			SpriteLayout.HeightAsPercentageOfBoxHeight = num2;
		}
		if (TextLayout != null)
		{
			TextLayout.StartPositionAsPercentageOfBoxWidth = startPositionAsPercentageOfBoxWidth;
			TextLayout.StartPositionAsPercentageOfBoxHeight = num;
			TextLayout.WidthAsPercentageOfBoxWidth = widthAsPercentageOfBoxWidth;
			TextLayout.HeightAsPercentageOfBoxHeight = num2;
		}
		if (HintSprite != null && mTexture != null)
		{
			HintSprite.SetTexture(mTexture);
			HintSprite.autoResize = false;
			if (!HintSprite.Started)
			{
				HintSprite.Start();
			}
			SpriteHelper.SetupSprite(HintSprite, 0f, mTexture.height, mTexture.width, mTexture.height);
			if (TBFUtils.UseAlternativeLayout())
			{
				HintSprite.pixelPerfect = false;
				HintSprite.SetSize((float)mTexture.width * 0.5f, (float)mTexture.height * 0.5f);
			}
		}
	}

	public override IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		InterfaceSFX.Instance.MessageBoxOn.Play2D();
		SetText(Title, Message, messageIsTranslated);
		CreateAndPositionButtons();
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
		UnloadTexture();
		if (Results != null)
		{
			Results.InvokeMethodForResult(mInternalResult);
		}
		Object.Destroy(base.gameObject);
	}

	private void LoadTexture(string imageLocation)
	{
		string text = imageLocation;
		if (TBFUtils.IsRetinaHdDevice())
		{
			text += "_@x2";
		}
		if (text != string.Empty && text != "_@x2")
		{
			mTexture = new Texture2D(4, 4);
			TextAsset textAsset = Resources.Load(text + ".png") as TextAsset;
			if (textAsset != null)
			{
				mTexture.LoadImage(textAsset.bytes);
				Resources.UnloadAsset(textAsset);
			}
		}
	}

	private void UnloadTexture()
	{
		mTexture = null;
	}
}
