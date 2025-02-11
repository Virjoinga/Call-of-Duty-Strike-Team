using System;
using System.Collections;
using UnityEngine;

public class InformationPopup : MonoBehaviour
{
	private const float SCALE = 1.3f;

	private const float BORDER = 20f;

	private const float TRANSITIONTIME = 0.33f;

	private const float Z = -9f;

	public Scale9Grid Background;

	public SpriteText InformationText;

	public PackedSprite Marker;

	private float mBorder;

	public GameObject Over { get; private set; }

	public IEnumerator Display(string information, GameObject displayOver)
	{
		Over = displayOver;
		yield return new WaitForEndOfFrame();
		if (!(InformationText != null) || !(Background != null) || !(displayOver != null) || !(Marker != null))
		{
			yield break;
		}
		float pixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(displayOver.transform);
		Vector3 position = displayOver.transform.position;
		float multiplier = 1f;
		if (TBFUtils.IsRetinaHdDevice())
		{
			multiplier = 2f;
		}
		mBorder = 20f * multiplier;
		InformationText.Text = information;
		string[] splitUp = InformationText.Text.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
		int longestIndex = -1;
		int longest = 0;
		for (int count = 0; count < splitUp.Length; count++)
		{
			if (splitUp[count].Length > longest)
			{
				longest = splitUp[count].Length;
				longestIndex = count;
			}
		}
		float width = 0f;
		float height = InformationText.BaseHeight + mBorder * 2f * (float)splitUp.Length;
		if (longestIndex != -1)
		{
			width = InformationText.GetWidth(splitUp[longestIndex]) / pixelSize + mBorder * 2f;
		}
		Background.size.x = width;
		Background.size.y = height;
		Background.Resize();
		position.y += Marker.height * 2f;
		position.z = -9.5f;
		Marker.transform.position = position;
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
		float rightEdge = screenPosition.x + width * 0.5f;
		if (rightEdge > (float)Screen.width)
		{
			position.x -= (rightEdge - (float)Screen.width) * pixelSize;
		}
		else
		{
			float leftEdge = screenPosition.x - width * 0.5f;
			if (leftEdge < 0f)
			{
				position.x += (0f - leftEdge) * pixelSize;
			}
		}
		position.y += height * 0.5f * pixelSize;
		position.z = -9f;
		Background.transform.position = position;
		position.z = -9.5f;
		InformationText.transform.position = position;
		Background.gameObject.ScaleFrom(Vector3.zero, 0.33f, 0f);
		InformationText.gameObject.ScaleFrom(Vector3.zero, 0.33f, 0f);
		Marker.gameObject.ScaleFrom(Vector3.zero, 0.33f, 0f);
	}

	public IEnumerator Remove()
	{
		base.gameObject.ScaleTo(Vector3.zero, 0.33f, 0f);
		yield return new WaitForSeconds(0.33f);
		if (this != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
