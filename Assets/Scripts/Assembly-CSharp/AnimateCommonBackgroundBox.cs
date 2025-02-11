using System.Collections.Generic;
using UnityEngine;

public class AnimateCommonBackgroundBox : MonoBehaviour
{
	private enum Stage
	{
		OpenBox = 0,
		FadeUpContent = 1,
		OnScreen = 2,
		FadeOffContent = 3,
		CloseBox = 4,
		OffScreen = 5
	}

	private static float mTimeToFade = 0.1f;

	private static float mTimeToOpen = 0.1f;

	private static float mTimeToClose = 0.1f;

	private CommonBackgroundBox Box;

	private SpriteRoot[] ImageContent;

	private SpriteText[] TextContent;

	private IconControllerBase[] IconContent;

	private Collider[] mColliderCache;

	private float[] ImageContentMaxAlpha;

	private float[] TextContentMaxAlpha;

	private Vector3 mCachedForegroundSize;

	private float mCurrentHeight;

	private float mCachedHeight;

	private float mTimeInStage;

	private float mTimeLastFrame;

	private Stage mCurrentStage;

	public bool IsOpen
	{
		get
		{
			return mCurrentStage == Stage.OnScreen;
		}
	}

	public bool IsClosed
	{
		get
		{
			return mCurrentStage == Stage.OffScreen;
		}
	}

	public bool IsOpening
	{
		get
		{
			return mCurrentStage < Stage.OnScreen;
		}
	}

	public bool IsClosing
	{
		get
		{
			return mCurrentStage > Stage.OnScreen && mCurrentStage < Stage.OffScreen;
		}
	}

	public Vector3 CachedForegroundSize
	{
		get
		{
			return mCachedForegroundSize;
		}
	}

	public void RecacheVariables()
	{
		Box = GetComponentInChildren<CommonBackgroundBox>();
		mCachedHeight = Box.ForegroundHeightInUnits;
		mCachedForegroundSize = Box.ForegroundSize;
		mCurrentHeight = 0f;
		Box.PreAnimateBoxSizeChange();
		Box.ForegroundHeightInUnits = mCurrentHeight;
		Box.Resize();
	}

	private void Awake()
	{
		Box = GetComponentInChildren<CommonBackgroundBox>();
		if (Box != null)
		{
			Box.PreAnimateBoxSizeChange();
			mCachedHeight = Box.ForegroundHeightInUnits;
			mCachedForegroundSize = Box.ForegroundSize;
			mCurrentHeight = 0f;
			Box.ForegroundHeightInUnits = mCurrentHeight;
			Box.Resize();
			Transform transform = base.transform.FindChild("Content");
			if (transform != null)
			{
				ImageContent = transform.GetComponentsInChildren<SpriteRoot>();
				TextContent = transform.GetComponentsInChildren<SpriteText>();
				IconContent = transform.GetComponentsInChildren<IconControllerBase>();
				List<SpriteRoot> list = new List<SpriteRoot>();
				SpriteRoot[] imageContent = ImageContent;
				foreach (SpriteRoot spriteRoot in imageContent)
				{
					IconControllerBase component = spriteRoot.GetComponent<IconControllerBase>();
					if (component == null)
					{
						list.Add(spriteRoot);
					}
				}
				ImageContent = list.ToArray();
			}
			else
			{
				Debug.LogWarning("No content found for CommonBackgroundBox " + base.name);
			}
			Transform transform2 = base.transform.FindChild("Icons");
			if (transform2 != null)
			{
				IconContent = transform2.GetComponentsInChildren<IconControllerBase>();
			}
			mColliderCache = GetComponentsInChildren<Collider>();
			CacheAndClearAlpha();
		}
		else
		{
			Debug.LogWarning("Cannot animate background box, no box found in " + base.name);
		}
		mCurrentStage = Stage.OffScreen;
		mTimeLastFrame = Time.realtimeSinceStartup;
	}

	public float AnimateOpen()
	{
		if (mCurrentStage == Stage.OffScreen)
		{
			mCurrentStage = Stage.OpenBox;
			mTimeInStage = 0f;
		}
		return mTimeToOpen + mTimeToFade;
	}

	public float AnimateClosed()
	{
		if (mCurrentStage == Stage.OnScreen)
		{
			mCurrentStage = Stage.FadeOffContent;
			mTimeInStage = 0f;
		}
		return mTimeToClose + mTimeToFade;
	}

	private void Update()
	{
		if (mCurrentStage != Stage.OnScreen && mCurrentStage != Stage.OffScreen)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			mTimeInStage += ((!(realtimeSinceStartup > mTimeLastFrame)) ? 0f : (realtimeSinceStartup - mTimeLastFrame));
			Invoke("Do" + mCurrentStage, 0f);
			mTimeLastFrame = realtimeSinceStartup;
		}
	}

	private void NextStage()
	{
		mCurrentStage++;
		mTimeInStage = 0f;
	}

	private void DoOpenBox()
	{
		float num = Mathf.Clamp(mTimeInStage / mTimeToOpen, 0f, 1f);
		mCurrentHeight = mCachedHeight * num;
		if (num >= 1f)
		{
			Box.RefreshPlacements = true;
			NextStage();
			Collider[] array = mColliderCache;
			foreach (Collider collider in array)
			{
				if (collider != null)
				{
					collider.enabled = !collider.enabled;
					collider.enabled = !collider.enabled;
				}
			}
		}
		Box.ForegroundHeightInUnits = mCurrentHeight;
		Box.Resize();
	}

	private void DoFadeUpContent()
	{
		float num = Mathf.Clamp(mTimeInStage / mTimeToFade, 0f, 1f);
		UpdateFade(num);
		if (num >= 1f)
		{
			NextStage();
		}
	}

	private void DoFadeOffContent()
	{
		float num = Mathf.Clamp(mTimeInStage / mTimeToFade, 0f, 1f);
		UpdateFade(1f - num);
		if (num >= 1f)
		{
			NextStage();
		}
	}

	private void DoCloseBox()
	{
		float num = Mathf.Clamp(mTimeInStage / mTimeToClose, 0f, 1f);
		mCurrentHeight = mCachedHeight * (1f - num);
		Box.ForegroundHeightInUnits = mCurrentHeight;
		Box.Resize();
		if (num >= 1f)
		{
			NextStage();
		}
	}

	private void CacheAndClearAlpha()
	{
		if (ImageContent != null)
		{
			ImageContentMaxAlpha = new float[ImageContent.Length];
			for (int i = 0; i < ImageContent.Length; i++)
			{
				SpriteRoot spriteRoot = ImageContent[i];
				ImageContentMaxAlpha[i] = spriteRoot.Color.a;
				Color color = new Color(spriteRoot.Color.r, spriteRoot.Color.g, spriteRoot.Color.b, 0f);
				spriteRoot.SetColor(color);
			}
		}
		if (TextContent != null)
		{
			TextContentMaxAlpha = new float[TextContent.Length];
			for (int j = 0; j < TextContent.Length; j++)
			{
				SpriteText spriteText = TextContent[j];
				TextContentMaxAlpha[j] = spriteText.Color.a;
				Color color2 = new Color(spriteText.Color.r, spriteText.Color.g, spriteText.Color.b, 0f);
				spriteText.SetColor(color2);
			}
		}
		if (IconContent != null)
		{
			IconControllerBase[] iconContent = IconContent;
			foreach (IconControllerBase iconControllerBase in iconContent)
			{
				iconControllerBase.SetAlpha(0f);
			}
		}
	}

	private void UpdateFade(float progress)
	{
		if (ImageContent != null)
		{
			for (int i = 0; i < ImageContent.Length; i++)
			{
				SpriteRoot spriteRoot = ImageContent[i];
				float a = Mathf.Lerp(0f, ImageContentMaxAlpha[i], progress);
				Color color = new Color(spriteRoot.Color.r, spriteRoot.Color.g, spriteRoot.Color.b, a);
				spriteRoot.SetColor(color);
			}
		}
		if (TextContent != null)
		{
			for (int j = 0; j < TextContent.Length; j++)
			{
				SpriteText spriteText = TextContent[j];
				float a2 = Mathf.Lerp(0f, TextContentMaxAlpha[j], progress);
				Color color2 = new Color(spriteText.Color.r, spriteText.Color.g, spriteText.Color.b, a2);
				spriteText.SetColor(color2);
			}
		}
		if (IconContent != null)
		{
			IconControllerBase[] iconContent = IconContent;
			foreach (IconControllerBase iconControllerBase in iconContent)
			{
				iconControllerBase.SetAlpha(progress);
			}
		}
	}
}
