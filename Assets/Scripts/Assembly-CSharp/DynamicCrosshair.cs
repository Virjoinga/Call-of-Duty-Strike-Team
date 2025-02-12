using UnityEngine;

public class DynamicCrosshair : MonoBehaviour
{
	public SpriteRoot Bottom;

	public SpriteRoot Left;

	public SpriteRoot Right;

	public SpriteRoot Top;

	public SpriteRoot DamageFeedback;

	public SpriteRoot StaticCrosshair;

	public float MinimumRadius;

	private Color mColor;

	private float mLastDamageTime;

	private float mDamageFeedbackFade;

	public float Accuracy
	{
		set
		{
			float num = MinimumRadius + InputSettings.FirstPersonCrosshairScaling * value;
			Left.transform.localPosition = new Vector3(0f - num, 0f, 0f);
			Right.transform.localPosition = new Vector3(num, 0f, 0f);
			Bottom.transform.localPosition = new Vector3(0f, 0f - num, 0f);
			Top.transform.localPosition = new Vector3(0f, num, 0f);
		}
	}

	public Color Color
	{
		get
		{
			return mColor;
		}
		set
		{
			mColor = value;
			Bottom.Color = mColor;
			Left.Color = mColor;
			Right.Color = mColor;
			Top.Color = mColor;
			float from = 0.9f;
			float alpha = Mathf.Lerp(from, 0f, mDamageFeedbackFade);
			DamageFeedback.Color = DamageFeedback.Color.Alpha(alpha);
			StaticCrosshair.Color = ColourChart.HudRed.Alpha(mColor.a);
		}
	}

	public void DidDamage()
	{
		mLastDamageTime = Time.realtimeSinceStartup;
	}

	public void Awake()
	{
		DamageFeedback.GetComponent<Renderer>().enabled = false;
	}

	public void Update()
	{
		float num = 0.4f;
		float num2 = 0.6f;
		float num3 = Time.realtimeSinceStartup - mLastDamageTime;
		mDamageFeedbackFade = Mathf.Clamp01((num3 - num) / num2);
		DamageFeedback.GetComponent<Renderer>().enabled = mDamageFeedbackFade < 1f;
	}
}
