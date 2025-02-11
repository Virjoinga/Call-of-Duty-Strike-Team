using UnityEngine;

public class SubtitleBackground : MonoBehaviour
{
	public Material Mat;

	private SimpleSprite[] Sprites;

	private SpriteText TitleText;

	private float mLastKnownWidth;

	public void Resize()
	{
		float pixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Resize(pixelSize);
	}

	public void Resize(float pixelSize)
	{
		if (Sprites == null || Sprites.Length == 0 || TitleText == null)
		{
			FetchComponents();
		}
		if (!(TitleText != null))
		{
			return;
		}
		float screenWidth = TitleText.GetScreenWidth(TitleText.Text);
		if (screenWidth == mLastKnownWidth || !(Mat != null))
		{
			return;
		}
		float num = Mat.mainTexture.width;
		float num2 = Mat.mainTexture.height;
		float num3 = num2;
		SimpleSprite[] sprites = Sprites;
		foreach (SpriteRoot spriteRoot in sprites)
		{
			spriteRoot.PixelSnapOnStart = false;
			if (spriteRoot.spriteMesh == null)
			{
				return;
			}
		}
		float num4 = num / 6f;
		float num5 = num - num4;
		SpriteHelper.SetupSprite(Sprites[0], 0f, num3, num4, num3);
		SpriteHelper.SetupSprite(Sprites[1], num4, num3, 1f, num3);
		SpriteHelper.SetupSprite(Sprites[2], num4, num3, num5, num3);
		float num6 = num4 * pixelSize;
		float w = num5 * pixelSize;
		float num7 = screenWidth * pixelSize;
		float h = num3 * pixelSize;
		SpriteHelper.OffsetSprite(Sprites[0], 0f, 0f, SpriteRoot.ANCHOR_METHOD.UPPER_LEFT);
		SpriteHelper.OffsetSprite(Sprites[1], num6, 0f, SpriteRoot.ANCHOR_METHOD.UPPER_LEFT);
		SpriteHelper.OffsetSprite(Sprites[2], num6 + num7, 0f, SpriteRoot.ANCHOR_METHOD.UPPER_LEFT);
		Sprites[0].SetSize(num6, h);
		Sprites[1].SetSize(num7, h);
		Sprites[2].SetSize(w, h);
		mLastKnownWidth = screenWidth;
	}

	public void BuildGrid()
	{
		BuildGrid(false);
	}

	public void BuildGrid(bool force)
	{
		FetchComponents();
		if (Application.isPlaying)
		{
			Debug.LogError("Let's only do this in the editor please ...");
			return;
		}
		if (force && Sprites != null)
		{
			for (int i = 0; i < Sprites.Length; i++)
			{
				if (Sprites[i] != null)
				{
					Object.DestroyImmediate(Sprites[i].gameObject);
				}
			}
			Sprites = null;
		}
		if (Sprites == null)
		{
			Sprites = new SimpleSprite[3];
			for (int j = 0; j < Sprites.Length; j++)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = string.Format("{0} Grid [{1}]", base.gameObject.name, j);
				gameObject.transform.parent = base.gameObject.transform;
				SimpleSprite simpleSprite = (SimpleSprite)gameObject.AddComponent(typeof(SimpleSprite));
				simpleSprite.renderer.sharedMaterial = Mat;
				Sprites[j] = simpleSprite;
			}
			Resize();
		}
	}

	private void Start()
	{
		Resize();
	}

	private void FetchComponents()
	{
		base.gameObject.SetActive(true);
		if (Sprites == null)
		{
			Sprites = GetComponentsInChildren<SimpleSprite>();
		}
		if (TitleText == null)
		{
			TitleText = GetComponentInChildren<SpriteText>();
		}
	}
}
