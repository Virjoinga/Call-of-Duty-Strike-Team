using UnityEngine;

public class BackgroundBoxFooterDetail : MonoBehaviour
{
	public Material Mat;

	public float FrameChangeMinTime = 0.25f;

	public float FrameChangeMaxTime = 0.5f;

	public int NumFrames = 14;

	private SimpleSprite mSprite;

	private float mTimeOnFrame;

	private float mPixelWidth;

	private float mPixelHeight;

	private float mWorldWidth;

	private float mWorldHeight;

	private int mFrame;

	private bool mSetup;

	private void Start()
	{
		FetchSprite();
		mTimeOnFrame = Random.Range(FrameChangeMinTime, FrameChangeMaxTime);
	}

	private void FetchSprite()
	{
		if (mSprite == null)
		{
			mSprite = GetComponentInChildren<SimpleSprite>();
		}
	}

	private void CalculateSimpleSpriteSettings()
	{
		if (mSprite != null)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (!mSetup)
			{
				Texture mainTexture = mSprite.GetComponent<Renderer>().sharedMaterial.mainTexture;
				float num4 = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
				float num5 = mainTexture.width;
				float num6 = mainTexture.height / NumFrames;
				num2 = num6 * (float)mFrame;
				num3 = num6 * (float)(mFrame + 1);
				mPixelWidth = num5;
				mPixelHeight = num6;
				mWorldWidth = num5 * num4;
				mWorldHeight = num6 * num4;
				mSprite.anchor = SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT;
				mSprite.lowerLeftPixel = new Vector2(num, num3);
				mSprite.pixelDimensions = new Vector2(mPixelWidth, mPixelHeight);
				mSprite.SetSize(mWorldWidth, mWorldHeight);
				mSetup = true;
			}
			else
			{
				num2 = mPixelHeight * (float)mFrame;
				num3 = mPixelHeight * (float)(mFrame + 1);
				mSprite.lowerLeftPixel = new Vector2(num, num3);
			}
			mSprite.SetUVsFromPixelCoords(new Rect(num, num2, mPixelWidth, mPixelHeight));
		}
	}

	private void Update()
	{
		if (mSprite != null)
		{
			mTimeOnFrame -= TimeManager.DeltaTime;
			if (mTimeOnFrame <= 0f)
			{
				mTimeOnFrame = Random.Range(FrameChangeMinTime, FrameChangeMaxTime);
				mFrame = Random.Range(0, NumFrames);
				CalculateSimpleSpriteSettings();
			}
		}
	}

	public void Build()
	{
		Build(false);
	}

	public void Build(bool force)
	{
		FetchSprite();
		if (Application.isPlaying)
		{
			Debug.LogError("Let's only do this in the editor please ...");
			return;
		}
		if (force && mSprite != null)
		{
			Object.DestroyImmediate(mSprite);
			mSprite = null;
		}
		if (mSprite == null)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Detail";
			gameObject.transform.parent = base.transform;
			mSprite = (SimpleSprite)gameObject.AddComponent(typeof(SimpleSprite));
			mSprite.GetComponent<Renderer>().sharedMaterial = Mat;
			CalculateSimpleSpriteSettings();
		}
	}
}
