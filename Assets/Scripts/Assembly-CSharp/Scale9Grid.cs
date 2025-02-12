using UnityEngine;

public class Scale9Grid : MonoBehaviour
{
	private SimpleSprite[] sprites;

	public Material mat;

	public Vector2 size;

	public int cornerSize = 64;

	public void FetchSprites()
	{
		sprites = GetComponentsInChildren<SimpleSprite>();
	}

	public void Resize()
	{
		float pixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Resize(pixelSize);
	}

	public void Resize(float pixelSize)
	{
		float x = size.x;
		float y = size.y;
		if (sprites == null || sprites.Length == 0)
		{
			FetchSprites();
		}
		SimpleSprite[] array = sprites;
		foreach (SpriteRoot spriteRoot in array)
		{
			spriteRoot.PixelSnapOnStart = false;
			if (spriteRoot.spriteMesh == null)
			{
				return;
			}
		}
		float num = mat.mainTexture.width;
		float num2 = mat.mainTexture.height;
		Vector2 vector = new Vector2(num / 2f, num2 / 2f);
		float num3 = x - num;
		float num4 = y - num2;
		float num5 = num3 * pixelSize;
		float num6 = num4 * pixelSize;
		float num7 = 0f;
		if (num3 < 0f)
		{
			num7 = num3 * 0.5f;
			num5 = 0f;
		}
		float num8 = 0f;
		if (num4 < 0f)
		{
			num8 = num4 * 0.5f;
			num6 = 0f;
		}
		vector.x += num7;
		vector.y += num8;
		SpriteHelper.SetupSprite(sprites[0], 0f, vector.y, vector.x, vector.y);
		SpriteHelper.SetupSprite(sprites[1], vector.x, vector.y, 1f, vector.y);
		SpriteHelper.SetupSprite(sprites[2], vector.x, vector.y, vector.x, vector.y);
		SpriteHelper.SetupSprite(sprites[3], 0f, vector.y, vector.x, 1f);
		SpriteHelper.SetupSprite(sprites[4], vector.x, vector.y, 1f, 1f);
		SpriteHelper.SetupSprite(sprites[5], vector.x, vector.y, vector.x, 1f);
		SpriteHelper.SetupSprite(sprites[6], 0f, 0f, vector.x, vector.y);
		SpriteHelper.SetupSprite(sprites[7], vector.x, 0f, 1f, vector.y);
		SpriteHelper.SetupSprite(sprites[8], vector.x, 0f, vector.x, vector.y);
		sprites[0].SetSize(vector.x * pixelSize, vector.y * pixelSize);
		sprites[1].SetSize(num5, vector.y * pixelSize);
		sprites[2].SetSize(vector.x * pixelSize, vector.y * pixelSize);
		sprites[3].SetSize(vector.x * pixelSize, num6);
		sprites[4].SetSize(num5, num6);
		sprites[5].SetSize(vector.x * pixelSize, num6);
		sprites[6].SetSize(vector.x * pixelSize, vector.y * pixelSize);
		sprites[7].SetSize(num5, vector.y * pixelSize);
		sprites[8].SetSize(vector.x * pixelSize, vector.y * pixelSize);
		float num9 = num5 * 0.5f;
		float num10 = num6 * 0.5f;
		SpriteHelper.OffsetSprite(sprites[0], 0f - num9, num10, SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT);
		SpriteHelper.OffsetSprite(sprites[1], 0f, num10, SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
		SpriteHelper.OffsetSprite(sprites[2], num9, num10, SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT);
		SpriteHelper.OffsetSprite(sprites[3], 0f - num9, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT);
		SpriteHelper.OffsetSprite(sprites[4], 0f, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
		SpriteHelper.OffsetSprite(sprites[5], num9, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
		SpriteHelper.OffsetSprite(sprites[6], 0f - num9, 0f - num10, SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT);
		SpriteHelper.OffsetSprite(sprites[7], 0f, 0f - num10, SpriteRoot.ANCHOR_METHOD.UPPER_CENTER);
		SpriteHelper.OffsetSprite(sprites[8], num9, 0f - num10, SpriteRoot.ANCHOR_METHOD.UPPER_LEFT);
		for (int j = 0; j < 9; j++)
		{
			sprites[j].SetSize(sprites[j].width, sprites[j].height);
			sprites[j].transform.localPosition = Vector3.zero;
		}
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
			Debug.LogError("Let's only do this stuff in the editor please ...");
			return;
		}
		if (force && sprites != null)
		{
			for (int i = 0; i < sprites.Length; i++)
			{
				if (sprites[i] != null)
				{
					Object.DestroyImmediate(sprites[i]);
				}
			}
			sprites = null;
		}
		if (sprites != null)
		{
			return;
		}
		sprites = new SimpleSprite[9];
		for (int j = 0; j < sprites.Length; j++)
		{
			int num = (int)Mathf.Floor((float)j / 3f);
			int num2 = j % 3;
			if ((float)cornerSize <= 0f && (num == 0 || num == 2))
			{
				sprites[j] = null;
				continue;
			}
			if ((float)cornerSize <= 0f && (num2 == 0 || num2 == 2))
			{
				sprites[j] = null;
				continue;
			}
			GameObject gameObject = new GameObject();
			gameObject.name = string.Format("{0} Grid [{1},{2}]", base.gameObject.name, num + 1, num2 + 1);
			gameObject.transform.parent = base.gameObject.transform;
			SimpleSprite simpleSprite = (SimpleSprite)gameObject.AddComponent(typeof(SimpleSprite));
			simpleSprite.GetComponent<Renderer>().sharedMaterial = mat;
			float num3 = simpleSprite.GetComponent<Renderer>().sharedMaterial.mainTexture.width;
			float num4 = simpleSprite.GetComponent<Renderer>().sharedMaterial.mainTexture.height;
			simpleSprite.lowerLeftPixel = new Vector2((float)num2 * (num3 * 0.25f), (float)(num + 2) * (num4 * 0.25f));
			simpleSprite.pixelDimensions = new Vector2(num3 * 0.5f, num4 * 0.5f);
			sprites[j] = simpleSprite;
		}
		Resize();
	}
}
