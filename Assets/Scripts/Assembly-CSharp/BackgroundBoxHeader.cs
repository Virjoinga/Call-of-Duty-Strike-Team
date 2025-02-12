using UnityEngine;

public class BackgroundBoxHeader : MonoBehaviour
{
	public enum Type
	{
		Button = 0,
		Plain = 1
	}

	private const int TEXTXOFFSET = 30;

	private const int TEXTYOFFSET = 5;

	public PackedSprite DetailPrefab;

	public Material Mat;

	public Vector2 size;

	public Type HeaderType;

	private SimpleSprite[] Sprites;

	private SpriteText TitleText;

	private PackedSprite Pixie;

	private string mLastKnownText = string.Empty;

	private float mLastKnownWidth;

	private int mTextXOffset;

	private int mTextYOffset;

	public void Resize()
	{
		float pixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Resize(pixelSize);
	}

	public void Resize(float pixelSize)
	{
		if ((!(TitleText != null) || !(TitleText.Text != mLastKnownText)) && size.x == mLastKnownWidth)
		{
			return;
		}
		int num = 1;
		if (TBFUtils.IsRetinaHdDevice())
		{
			num = 2;
		}
		mTextXOffset = 30 * num;
		mTextYOffset = 5 * num;
		float x = size.x;
		float y = size.y;
		float lowerLeftY = size.y + (float)HeaderType * size.y;
		if (Sprites == null || Sprites.Length == 0 || TitleText == null)
		{
			FetchComponents();
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
		float num2 = Mat.mainTexture.width;
		float num3 = x - num2;
		SpriteHelper.SetupSprite(Sprites[0], 0f, lowerLeftY, num2 * 0.5f, y);
		SpriteHelper.SetupSprite(Sprites[1], num2 * 0.5f, lowerLeftY, 1f, y);
		SpriteHelper.SetupSprite(Sprites[2], num2 * 0.5f, lowerLeftY, num2 * 0.5f, y);
		float h = y * pixelSize;
		float w = num2 * 0.5f * pixelSize;
		float num4 = num3 * pixelSize;
		float num5 = num4 * 0.5f;
		Sprites[0].SetSize(w, h);
		Sprites[1].SetSize(num4, h);
		Sprites[2].SetSize(w, h);
		SpriteHelper.OffsetSprite(Sprites[0], 0f - num5, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT);
		SpriteHelper.OffsetSprite(Sprites[1], 0f, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
		SpriteHelper.OffsetSprite(Sprites[2], num5, 0f, SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT);
		Vector3 position = Camera.main.WorldToScreenPoint(base.transform.position);
		if (TitleText != null)
		{
			position.x -= x * 0.5f - (float)mTextXOffset;
			position.y -= (float)mTextYOffset * 0.5f;
			Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
			TitleText.alignment = SpriteText.Alignment_Type.Left;
			TitleText.anchor = SpriteText.Anchor_Pos.Middle_Left;
			TitleText.pixelPerfect = true;
			position2.z -= 0.5f;
			TitleText.transform.position = position2;
			if (Pixie != null)
			{
				float screenWidth = TitleText.GetScreenWidth(TitleText.Text);
				float num6 = TitleText.BaseHeight / pixelSize;
				Vector2 spritePixelSize = CommonHelper.GetSpritePixelSize(Pixie);
				position.x = (int)(position.x + screenWidth + spritePixelSize.x);
				position.y = (int)(position.y - num6 * 0.5f - spritePixelSize.y * 0.5f);
				Vector3 position3 = Camera.main.ScreenToWorldPoint(position);
				position3.z -= 0.5f;
				Pixie.PixelSnapOnStart = false;
				Pixie.transform.position = position3;
			}
			mLastKnownText = TitleText.Text;
		}
		mLastKnownWidth = size.x;
	}

	public void Hide()
	{
		if (Sprites == null || Sprites.Length == 0 || TitleText == null)
		{
			FetchComponents();
		}
		for (int i = 0; i < Sprites.Length; i++)
		{
			Sprites[i].Hide(true);
		}
		if (TitleText != null)
		{
			TitleText.Hide(true);
		}
		if (Pixie != null)
		{
			Pixie.Hide(true);
		}
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
		if (force)
		{
			if (Sprites != null)
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
			if (TitleText != null)
			{
				Object.DestroyImmediate(TitleText.gameObject);
			}
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
				simpleSprite.GetComponent<Renderer>().sharedMaterial = Mat;
				float num = simpleSprite.GetComponent<Renderer>().sharedMaterial.mainTexture.width;
				float y = simpleSprite.GetComponent<Renderer>().sharedMaterial.mainTexture.height;
				float num2 = num * (1f / 3f);
				simpleSprite.lowerLeftPixel = new Vector2((float)j * num2, y);
				simpleSprite.pixelDimensions = new Vector2(num2, y);
				Sprites[j] = simpleSprite;
			}
			GameObject gameObject2 = new GameObject();
			gameObject2.name = "Title_text";
			TitleText = gameObject2.AddComponent<SpriteText>();
			gameObject2.transform.parent = base.transform;
			if (DetailPrefab != null)
			{
				Pixie = (PackedSprite)Object.Instantiate(DetailPrefab);
				Pixie.transform.parent = base.transform;
			}
			Resize();
		}
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
		if (Pixie == null)
		{
			Pixie = GetComponentInChildren<PackedSprite>();
		}
	}
}
