using UnityEngine;

public class IconControllerBase : MonoBehaviour
{
	public int IconsPerRow = 2;

	public int IconsPerColum = 2;

	public int Border;

	private AnimateCommonBackgroundBox mAnimator;

	private SimpleSprite mSprite;

	private Color mCurrentColour;

	private int mIndex;

	private int mBorder;

	private bool mAvailable;

	protected Color AlphaBlack = new Color(0.1f, 0.1f, 0.1f, 0.4f);

	public SimpleSprite Sprite
	{
		get
		{
			return mSprite;
		}
	}

	protected int Index
	{
		get
		{
			return mIndex;
		}
		set
		{
			mIndex = value;
		}
	}

	protected bool Available
	{
		set
		{
			mAvailable = value;
		}
	}

	public void Hide(bool hide)
	{
		if (mSprite != null)
		{
			mSprite.Hide(hide);
		}
	}

	public void SetColor(Color colour)
	{
		mCurrentColour = colour;
	}

	public void SetAlpha(float alpha)
	{
		mCurrentColour = ((!mAvailable) ? AlphaBlack : Color.white);
		mCurrentColour.a *= alpha;
		if (mSprite != null)
		{
			mSprite.SetColor(mCurrentColour);
		}
	}

	protected virtual void Awake()
	{
		mAnimator = LookForAnimator(base.transform);
		mSprite = GetComponentInChildren<SimpleSprite>();
		mBorder = Border;
		if (TBFUtils.IsRetinaHdDevice())
		{
			mBorder = Border * 2;
		}
	}

	private void Update()
	{
		if (mAnimator == null || mAnimator.IsOpen || mAnimator.IsClosed)
		{
			mSprite.SetColor(mCurrentColour);
		}
	}

	protected void CalculateSimpleSpriteSettings()
	{
		if (mSprite != null)
		{
			int num = mIndex % IconsPerRow;
			int num2 = mIndex / IconsPerRow;
			Material sharedMaterial = mSprite.renderer.sharedMaterial;
			Texture mainTexture = sharedMaterial.mainTexture;
			float num3 = mainTexture.width / IconsPerRow;
			float num4 = mainTexture.height / IconsPerColum;
			float num5 = num3 * (float)num + (float)mBorder;
			float top = num4 * (float)num2 + (float)mBorder;
			float y = num4 * (float)(num2 + 1) - (float)mBorder;
			num3 -= (float)mBorder * 2f;
			num4 -= (float)mBorder * 2f;
			mSprite.SetMaterial(sharedMaterial);
			mSprite.lowerLeftPixel = new Vector2(num5, y);
			mSprite.pixelDimensions = new Vector2(num3, num4);
			mSprite.SetUVsFromPixelCoords(new Rect(num5, top, num3, num4));
			mCurrentColour = ((!mAvailable) ? AlphaBlack : Color.white);
			mSprite.SetColor(mCurrentColour);
		}
	}

	private AnimateCommonBackgroundBox LookForAnimator(Transform trans)
	{
		AnimateCommonBackgroundBox result = null;
		if (trans.parent != null)
		{
			result = LookForAnimator(trans.parent);
		}
		return result;
	}
}
