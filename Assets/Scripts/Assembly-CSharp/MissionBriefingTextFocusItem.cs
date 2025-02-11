using System.Collections;
using UnityEngine;

public class MissionBriefingTextFocusItem : MonoBehaviour
{
	private const float TEXT_FRAME_BORDER = 32f;

	private const float CIRCLE_SCALE_TIME = 0.2f;

	private const float LINE_OPEN_TIME = 0.2f;

	private const float FRAME_OPEN_TIME = 0.3f;

	private const float TEXT_ON_TIME = 0.8f;

	private const float TEXT_BORDER = 6f;

	private const float INNER_CIRCLE_ROTATION_SPEED = 20f;

	public PackedSprite OuterCircleImage;

	public PackedSprite InnerCircleImage;

	public PackedSprite LinesImage;

	public Scale9Grid FocusTextFrame;

	public SpriteText TextToFrame;

	private Rect mCachedLineUVs;

	private Vector2 mOriginalSize;

	private float mImageStartPosition;

	private float mTextWidth;

	private float mTextHeight;

	private float mOpenLinesProgress;

	private float mOpenFrameProgress;

	private float mShowTextProgress;

	private float mLinesWidth;

	private bool mDisplaying;

	public bool Open
	{
		get
		{
			return mShowTextProgress >= 1f;
		}
	}

	public bool Displaying
	{
		get
		{
			return mDisplaying;
		}
	}

	public void DisplayItem(string focusText, float duration, float delay, float z)
	{
		DisplayItem(focusText, duration, delay, z, MissionBriefingHelper.FocusDirection.Right);
	}

	public void DisplayItem(string focusText, float duration, float delay, float z, MissionBriefingHelper.FocusDirection direction)
	{
		if (focusText != null && TextToFrame != null)
		{
			mDisplaying = true;
			TextToFrame.Text = focusText;
			LayoutComponents(direction);
			StartCoroutine(DoDisplayItem(duration, delay, direction));
		}
	}

	private void Start()
	{
		mDisplaying = false;
		SetToStartState();
	}

	private IEnumerator DoDisplayItem(float duration, float delay, MissionBriefingHelper.FocusDirection direction)
	{
		float delayTime = 0f;
		float totalTime = 0f;
		while (delayTime < delay)
		{
			delayTime += Time.deltaTime;
			yield return null;
		}
		BriefingSFX.Instance.CircleOpen.Play2D();
		if (OuterCircleImage != null && InnerCircleImage != null)
		{
			OuterCircleImage.gameObject.ScaleTo(Vector3.one, 0.2f, 0f, EaseType.easeInOutCubic);
			InnerCircleImage.gameObject.ScaleTo(Vector3.one, 0.2f, 0f, EaseType.easeInOutCubic);
			while (totalTime < 0.2f)
			{
				totalTime += Time.deltaTime;
				yield return null;
			}
		}
		float startTime6 = totalTime;
		while (mOpenLinesProgress < 1f)
		{
			totalTime += Time.deltaTime;
			mOpenLinesProgress = (totalTime - startTime6) / 0.2f;
			mOpenLinesProgress = Mathf.Clamp(mOpenLinesProgress, 0f, 1f);
			MissionBriefingHelper.UpdateLineOpen(LinesImage, mOpenLinesProgress, mCachedLineUVs);
			yield return null;
		}
		if (FocusTextFrame != null)
		{
			FocusTextFrame.gameObject.SetActive(true);
		}
		BriefingSFX.Instance.BoxOpen.Play2D();
		startTime6 = totalTime;
		while (mOpenFrameProgress < 1f)
		{
			totalTime += Time.deltaTime;
			mOpenFrameProgress = (totalTime - startTime6) / 0.3f;
			mOpenFrameProgress = Mathf.Clamp(mOpenFrameProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFrameOpen(FocusTextFrame, base.transform, mOpenFrameProgress, mImageStartPosition, mTextWidth, mTextHeight, direction);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Play2D();
		startTime6 = totalTime;
		while (mShowTextProgress < 1f)
		{
			totalTime += Time.deltaTime;
			mShowTextProgress = (totalTime - startTime6) / 0.8f;
			mShowTextProgress = Mathf.Clamp(mShowTextProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFlicker(TextToFrame, mShowTextProgress);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Stop2D();
		float timeToDisplay = totalTime;
		float timeOnScreen = duration - timeToDisplay * 2f;
		while (totalTime - timeToDisplay < timeOnScreen)
		{
			totalTime += Time.deltaTime;
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Play2D();
		startTime6 = totalTime;
		while (mShowTextProgress > 0f)
		{
			totalTime += Time.deltaTime;
			mShowTextProgress = (totalTime - startTime6) / 0.8f;
			mShowTextProgress = 1f - Mathf.Clamp(mShowTextProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFlicker(TextToFrame, mShowTextProgress);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Stop2D();
		BriefingSFX.Instance.BoxClose.Play2D();
		startTime6 = totalTime;
		while (mOpenFrameProgress > 0f)
		{
			totalTime += Time.deltaTime;
			mOpenFrameProgress = (totalTime - startTime6) / 0.3f;
			mOpenFrameProgress = 1f - Mathf.Clamp(mOpenFrameProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFrameOpen(FocusTextFrame, base.transform, mOpenFrameProgress, mImageStartPosition, mTextWidth, mTextHeight, direction);
			yield return null;
		}
		if (FocusTextFrame != null)
		{
			FocusTextFrame.gameObject.SetActive(false);
		}
		startTime6 = totalTime;
		while (mOpenLinesProgress > 0f)
		{
			totalTime += Time.deltaTime;
			mOpenLinesProgress = (totalTime - startTime6) / 0.2f;
			mOpenLinesProgress = 1f - Mathf.Clamp(mOpenLinesProgress, 0f, 1f);
			MissionBriefingHelper.UpdateLineOpen(LinesImage, mOpenLinesProgress, mCachedLineUVs);
			yield return null;
		}
		BriefingSFX.Instance.CircleClose.Play2D();
		if (OuterCircleImage != null && InnerCircleImage != null)
		{
			OuterCircleImage.gameObject.ScaleTo(Vector3.zero, 0.2f, 0f, EaseType.easeInOutCubic);
			InnerCircleImage.gameObject.ScaleTo(Vector3.zero, 0.2f, 0f, EaseType.easeInOutCubic);
			while (OuterCircleImage.transform.localScale.x > 0f)
			{
				totalTime += Time.deltaTime;
				yield return null;
			}
		}
		mDisplaying = false;
		yield return new WaitForEndOfFrame();
	}

	private void LayoutComponents(MissionBriefingHelper.FocusDirection direction)
	{
		float num = 6f;
		float num2 = 32f;
		float num3 = 0f;
		float num4 = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		if (TBFUtils.IsRetinaHdDevice())
		{
			num *= 2f;
			num2 *= 2f;
		}
		Vector2 vector2 = default(Vector2);
		if (TextToFrame != null)
		{
			vector2.x = TextToFrame.TotalScreenWidth;
			vector2.y = TextToFrame.TotalScreenHeight;
		}
		if (OuterCircleImage != null)
		{
			if (!OuterCircleImage.Started)
			{
				OuterCircleImage.Start();
			}
			num3 = OuterCircleImage.width / num4;
		}
		if (LinesImage != null)
		{
			Vector3 position = vector;
			float angle = 0f;
			switch (direction)
			{
			case MissionBriefingHelper.FocusDirection.Left:
				position.x += num3 * 0.5f - num * 2f;
				break;
			case MissionBriefingHelper.FocusDirection.Right:
				position.x -= num3 * 0.5f - num * 2f;
				angle = 180f;
				break;
			case MissionBriefingHelper.FocusDirection.Top:
				position.y += num3 * 0.5f - num * 2f;
				angle = 90f;
				break;
			case MissionBriefingHelper.FocusDirection.Bottom:
				position.y -= num3 * 0.5f - num * 2f;
				angle = -90f;
				break;
			}
			Vector3 vector3 = Camera.main.ScreenToWorldPoint(position);
			LinesImage.transform.position = vector3;
			LinesImage.transform.rotation = Quaternion.identity;
			LinesImage.transform.RotateAround(vector3, Vector3.forward, angle);
		}
		if (TextToFrame != null && FocusTextFrame != null)
		{
			mTextWidth = vector2.x + num2 * 2f;
			mTextHeight = vector2.y + num2 * 2f;
			mImageStartPosition = num3 * 0.5f + mLinesWidth - num * 4f;
			Vector3 position2 = vector;
			switch (direction)
			{
			case MissionBriefingHelper.FocusDirection.Left:
				position2.x += mImageStartPosition + mTextWidth * 0.5f;
				break;
			case MissionBriefingHelper.FocusDirection.Right:
				position2.x -= mImageStartPosition + mTextWidth * 0.5f;
				break;
			case MissionBriefingHelper.FocusDirection.Top:
				position2.y += mImageStartPosition + mTextHeight * 0.5f;
				break;
			case MissionBriefingHelper.FocusDirection.Bottom:
				position2.y -= mImageStartPosition + mTextHeight * 0.5f;
				break;
			}
			Vector3 vector3 = Camera.main.ScreenToWorldPoint(position2);
			TextToFrame.transform.position = vector3;
			FocusTextFrame.size.x = 0f;
			FocusTextFrame.size.y = 0f;
			FocusTextFrame.Resize();
		}
	}

	private void SetToStartState()
	{
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		if (OuterCircleImage != null && InnerCircleImage != null)
		{
			OuterCircleImage.transform.localScale = Vector3.zero;
			InnerCircleImage.transform.localScale = Vector3.zero;
		}
		if (LinesImage != null)
		{
			if (!LinesImage.Started)
			{
				LinesImage.Start();
			}
			mLinesWidth = LinesImage.width / num;
			mCachedLineUVs = LinesImage.GetUVs();
			LinesImage.SetUVs(default(Rect));
		}
		Color color = new Color(1f, 1f, 1f, 0f);
		if (TextToFrame != null)
		{
			TextToFrame.SetColor(color);
		}
		if (FocusTextFrame != null)
		{
			FocusTextFrame.size.x = 0f;
			FocusTextFrame.size.y = 0f;
			FocusTextFrame.Resize();
		}
		mOpenLinesProgress = 0f;
	}

	private void Update()
	{
		if (InnerCircleImage != null)
		{
			InnerCircleImage.transform.Rotate(new Vector3(0f, 0f, 0f - 20f * Time.deltaTime));
		}
	}
}
