using UnityEngine;

public class SpriteHelper
{
	public static void SetupSprite(SimpleSprite sprite, float lowerLeftX, float lowerLeftY, float pixelWidth, float pixelHeight)
	{
		sprite.lowerLeftPixel = new Vector2(lowerLeftX, lowerLeftY);
		sprite.pixelDimensions = new Vector2(pixelWidth, pixelHeight);
		if (sprite.GetComponent<Renderer>().sharedMaterial != null && sprite.GetComponent<Renderer>().sharedMaterial.mainTexture != null)
		{
			Texture mainTexture = sprite.GetComponent<Renderer>().sharedMaterial.mainTexture;
			sprite.SetUVs(new Rect(lowerLeftX / (float)mainTexture.width, 1f - lowerLeftY / (float)mainTexture.height, pixelWidth / (float)mainTexture.width, pixelHeight / (float)mainTexture.height));
		}
		sprite.UpdateUVs();
	}

	public static void OffsetSprite(SimpleSprite sprite, float xOffset, float yOffset, SpriteRoot.ANCHOR_METHOD anchor)
	{
		sprite.SetOffset(new Vector3(xOffset, yOffset, 0f));
		sprite.anchor = anchor;
	}
}
