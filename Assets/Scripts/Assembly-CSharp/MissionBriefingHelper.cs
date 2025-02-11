using UnityEngine;

public class MissionBriefingHelper
{
	public enum FocusDirection
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}

	public static void UpdateLineOpen(PackedSprite linesImage, float openLinesProgress, Rect lineUVs)
	{
		float num = openLinesProgress;
		float num2 = 1f;
		Rect uVs = lineUVs;
		uVs.width *= openLinesProgress;
		if (linesImage != null)
		{
			num *= linesImage.width;
			num2 *= linesImage.height;
			linesImage.SetSize(num, num2);
			linesImage.SetUVs(uVs);
		}
	}

	public static void UpdateFrameOpen(Scale9Grid focusImageFrame, Transform transform, float openFrameProgress, float imageStartPosition, float imageWidth, float imageHeight, FocusDirection direction)
	{
		bool flag = openFrameProgress > 0.5f;
		Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
		switch (direction)
		{
		case FocusDirection.Left:
			imageWidth = Mathf.Lerp(64f, imageWidth, (!flag) ? 0f : ((openFrameProgress - 0.5f) * 2f));
			imageHeight = Mathf.Lerp(128f, imageHeight, flag ? 1f : (openFrameProgress * 2f));
			position.x += imageStartPosition + imageWidth * 0.5f;
			break;
		case FocusDirection.Right:
			imageWidth = Mathf.Lerp(64f, imageWidth, (!flag) ? 0f : ((openFrameProgress - 0.5f) * 2f));
			imageHeight = Mathf.Lerp(128f, imageHeight, flag ? 1f : (openFrameProgress * 2f));
			position.x -= imageStartPosition + imageWidth * 0.5f;
			break;
		case FocusDirection.Top:
			imageWidth = Mathf.Lerp(128f, imageWidth, flag ? 1f : (openFrameProgress * 2f));
			imageHeight = Mathf.Lerp(64f, imageHeight, (!flag) ? 0f : ((openFrameProgress - 0.5f) * 2f));
			position.y += imageStartPosition + imageHeight * 0.5f;
			break;
		case FocusDirection.Bottom:
			imageWidth = Mathf.Lerp(128f, imageWidth, flag ? 1f : (openFrameProgress * 2f));
			imageHeight = Mathf.Lerp(64f, imageHeight, (!flag) ? 0f : ((openFrameProgress - 0.5f) * 2f));
			position.y -= imageStartPosition + imageHeight * 0.5f;
			break;
		}
		position.z -= 0.1f;
		Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
		if (focusImageFrame != null)
		{
			focusImageFrame.size.x = imageWidth;
			focusImageFrame.size.y = imageHeight;
			focusImageFrame.Resize();
			focusImageFrame.transform.position = position2;
		}
	}

	public static void UpdateFlicker(SimpleSprite image, float flickerProgress)
	{
		float a = UpdateFlicker(flickerProgress);
		if (image != null)
		{
			Color color = new Color(1f, 1f, 1f, a);
			image.SetColor(color);
		}
	}

	public static void UpdateFlicker(SpriteText text, float flickerProgress)
	{
		float a = UpdateFlicker(flickerProgress);
		if (text != null)
		{
			Color color = text.Color;
			color.a = a;
			text.SetColor(color);
		}
	}

	public static float UpdateFlicker(float flickerProgress)
	{
		float num = 0f;
		bool flag = flickerProgress > 0.75f;
		bool flag2 = flickerProgress < 0.02f;
		if (flag)
		{
			return Mathf.Lerp(0f, 1f, (flickerProgress - 0.5f) * 2f);
		}
		if (flag2)
		{
			return 0f;
		}
		return Mathf.Lerp(0f, 1f, Random.Range(0f, 1f));
	}

	public static void CorrectImageSize(SimpleSprite image, Transform transform, Vector2 size, Vector2 originalSize)
	{
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(transform);
		image.autoResize = false;
		image.Setup(size.x * num, size.y * num, new Vector2(0f, originalSize.y), new Vector2(originalSize.x, originalSize.y));
		image.SetBleedCompensation(0f, 1f);
	}
}
