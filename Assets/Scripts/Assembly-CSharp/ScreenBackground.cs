using UnityEngine;

public class ScreenBackground : MonoBehaviour
{
	private SimpleSprite mBackground;

	private void Awake()
	{
		Transform transform = base.transform.Find("BackgroundFlatColour");
		if (transform != null)
		{
			mBackground = transform.GetComponent<SimpleSprite>();
		}
	}

	private void Start()
	{
		if (mBackground != null)
		{
			float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
			mBackground.SetSize((float)Screen.width * num, (float)Screen.height * num);
		}
	}
}
