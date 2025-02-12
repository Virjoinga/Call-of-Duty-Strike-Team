using UnityEngine;

public class CommonHelper
{
	private const int mDefaultScreenWidth = 960;

	private const int mDefaultScreenHeight = 640;

	public static float CalculatePixelSizeInWorldSpace(Transform transform)
	{
		Camera main = Camera.main;
		float distanceToPoint = new Plane(main.transform.forward, main.transform.position).GetDistanceToPoint(transform.position);
		return Vector3.Distance(main.ScreenToWorldPoint(new Vector3(0f, 1f, distanceToPoint)), main.ScreenToWorldPoint(new Vector3(0f, 0f, distanceToPoint)));
	}

	public static Vector2 GetSpritePixelSize(SpriteRoot sprite)
	{
		Vector2 result = default(Vector2);
		if (sprite != null)
		{
			Texture mainTexture = sprite.GetComponent<Renderer>().sharedMaterial.mainTexture;
			Rect uVs = sprite.GetUVs();
			result.x = (float)mainTexture.width * uVs.width;
			result.y = (float)mainTexture.height * uVs.height;
		}
		return result;
	}

	public static char HardCurrencySymbol()
	{
		int num = 188;
		return (char)num;
	}

	public static Rect CreateRect(CommonBackgroundBox box)
	{
		Rect result = default(Rect);
		if (box != null)
		{
			Vector2 vector = Camera.main.WorldToScreenPoint(box.transform.position);
			float x = box.ForegroundSize.x;
			float y = box.ForegroundSize.y;
			result = new Rect(vector.x - x * 0.5f, vector.y - y * 0.5f, x, y);
		}
		return result;
	}

	public static Rect CreateRect(SpriteRoot sprite, float pixelSize)
	{
		Rect result = default(Rect);
		if (sprite != null)
		{
			Vector2 vector = Camera.main.WorldToScreenPoint(sprite.transform.position);
			float num = sprite.width / pixelSize;
			float num2 = sprite.height / pixelSize;
			result = new Rect(vector.x - num * 0.5f, vector.y - num2 * 0.5f, num, num2);
		}
		return result;
	}

	public static void FindTexturePaths(Texture image, out string texturePath, out string x2TexturePath)
	{
		texturePath = string.Empty;
		x2TexturePath = string.Empty;
	}

	public static void UnloadImageIfSurplus(Texture texture)
	{
		if (texture != null)
		{
			bool flag = TBFUtils.IsRetinaHdDevice();
			bool flag2 = texture.name.Contains("_@x2");
			if ((flag && !flag2) || (!flag && flag2))
			{
				Resources.UnloadAsset(texture);
			}
		}
	}

	public static float GetRankWidth(SpriteText rowText, string text)
	{
		float result = 0f;
		if (rowText != null)
		{
			result = rowText.GetWidth("0") * (float)((text.Length >= 3) ? text.Length : 3);
		}
		return result;
	}
}
