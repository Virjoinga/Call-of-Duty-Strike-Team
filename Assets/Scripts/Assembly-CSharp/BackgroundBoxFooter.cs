using UnityEngine;

public class BackgroundBoxFooter : MonoBehaviour
{
	public Material Mat;

	public Vector2 size;

	private SimpleSprite[] Sprites;

	private float mLastKnownWidth;

	private void FetchSprites()
	{
		base.gameObject.SetActive(true);
		Sprites = GetComponentsInChildren<SimpleSprite>();
	}

	public void Resize()
	{
		float pixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Resize(pixelSize);
	}

	public void Resize(float pixelSize)
	{
		float x = size.x;
		if (x == mLastKnownWidth)
		{
			return;
		}
		if (Sprites == null || Sprites.Length == 0)
		{
			FetchSprites();
		}
		SimpleSprite[] sprites = Sprites;
		foreach (SpriteRoot spriteRoot in sprites)
		{
			spriteRoot.PixelSnapOnStart = false;
			if (spriteRoot.spriteMesh == null)
			{
				return;
			}
		}
		float num = Mat.mainTexture.width;
		float num2 = Mat.mainTexture.height;
		float num3 = x - num;
		SpriteHelper.SetupSprite(Sprites[0], 0f, num2, num * 0.25f, num2);
		SpriteHelper.SetupSprite(Sprites[1], num * 0.25f, num2, 1f, num2);
		SpriteHelper.SetupSprite(Sprites[2], num * 0.25f, num2, num * 0.5f, num2);
		SpriteHelper.SetupSprite(Sprites[3], num * 0.75f, num2, 1f, num2);
		SpriteHelper.SetupSprite(Sprites[4], num * 0.75f, num2, num * 0.25f, num2);
		float h = num2 * pixelSize;
		float w = num * 0.5f * pixelSize;
		float num4 = num * 0.25f * pixelSize;
		float num5 = num3 * pixelSize * 0.5f;
		Sprites[0].SetSize(num4, h);
		Sprites[1].SetSize(num5, h);
		Sprites[2].SetSize(w, h);
		Sprites[3].SetSize(num5, h);
		Sprites[4].SetSize(num4, h);
		SpriteHelper.OffsetSprite(Sprites[0], 0f - (num5 + num4), 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT);
		SpriteHelper.OffsetSprite(Sprites[1], 0f - (num5 + num4), 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
		SpriteHelper.OffsetSprite(Sprites[2], 0f, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
		SpriteHelper.OffsetSprite(Sprites[3], num5 + num4, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT);
		SpriteHelper.OffsetSprite(Sprites[4], num5 + num4, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
		mLastKnownWidth = x;
	}

	public void BuildGrid()
	{
		BuildGrid(false);
	}

	public void BuildGrid(bool force)
	{
		FetchSprites();
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
					Object.DestroyImmediate(Sprites[i]);
				}
			}
			Sprites = null;
		}
		if (Sprites == null)
		{
			Sprites = new SimpleSprite[5];
			for (int j = 0; j < Sprites.Length; j++)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = string.Format("{0} Grid [{1}]", base.gameObject.name, j);
				gameObject.transform.parent = base.gameObject.transform;
				SimpleSprite simpleSprite = (SimpleSprite)gameObject.AddComponent(typeof(SimpleSprite));
				simpleSprite.renderer.sharedMaterial = Mat;
				float num = simpleSprite.renderer.sharedMaterial.mainTexture.width;
				float y = simpleSprite.renderer.sharedMaterial.mainTexture.height;
				simpleSprite.lowerLeftPixel = new Vector2((float)j * (num * 0.2f), y);
				simpleSprite.pixelDimensions = new Vector2(num * 0.2f, y);
				Sprites[j] = simpleSprite;
			}
			Resize();
		}
	}
}
