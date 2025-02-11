using System.Collections;
using UnityEngine;

public class MissionBriefingFocusItem : MonoBehaviour
{
	private const float IMAGE_FRAME_BORDER = 6f;

	private const float CIRCLE_SCALE_TIME = 0.2f;

	private const float LINE_OPEN_TIME = 0.2f;

	private const float FRAME_OPEN_TIME = 0.3f;

	private const float IMAGE_ON_TIME = 0.8f;

	private const float IMAGE_BORDER = 5f;

	private const float INNER_CIRCLE_ROTATION_SPEED = 20f;

	public PackedSprite OuterCircleImage;

	public PackedSprite InnerCircleImage;

	public PackedSprite LinesImage;

	public SimpleSprite FocusImage;

	public Scale9Grid FocusImageFrame;

	private Rect mCachedLineUVs;

	private Vector2 mOriginalSize;

	private Vector2 mScreenSize;

	private float mImageStartPosition;

	private float mImageWidth;

	private float mImageHeight;

	private float mOpenLinesProgress;

	private float mOpenFrameProgress;

	private float mOpenImageProgress;

	private float mLinesWidth;

	private bool mWaiting;

	public bool Waiting
	{
		get
		{
			return mWaiting;
		}
	}

	public bool Open
	{
		get
		{
			return mOpenImageProgress >= 1f;
		}
	}

	public void ReleaseFromWait()
	{
		mWaiting = false;
	}

	public void DisplayItem(Texture focusImage, Vector2 size, float duration, float delay, float z)
	{
		DisplayItem(focusImage, size, duration, delay, z, MissionBriefingHelper.FocusDirection.Right, null);
	}

	public void DisplayItem(Texture focusImage, Vector2 size, float duration, float delay, float z, MissionBriefingHelper.FocusDirection direction, Texture2D frame)
	{
		if (focusImage != null)
		{
			mWaiting = true;
			if (FocusImage != null && FocusImage.renderer != null && FocusImage.renderer.material != null)
			{
				Resources.UnloadAsset(FocusImage.renderer.material.mainTexture);
				FocusImage.renderer.material.mainTexture = focusImage;
			}
			LayoutComponents(size, direction);
			StartCoroutine(DoDisplayItem(duration, delay, direction, frame));
		}
	}

	private void Start()
	{
		LayoutComponents(Vector2.zero, MissionBriefingHelper.FocusDirection.Left);
		SetToStartState();
	}

	private void OnDestroy()
	{
		if (FocusImage != null && FocusImage.renderer != null && FocusImage.renderer.material != null)
		{
			Resources.UnloadAsset(FocusImage.renderer.material.mainTexture);
		}
	}

	private IEnumerator DoDisplayItem(float duration, float delay, MissionBriefingHelper.FocusDirection direction, Texture2D frame)
	{
		float delayTime = 0f;
		float totalTime = 0f;
		while (delayTime < delay)
		{
			delayTime += TimeManager.DeltaTime;
			yield return null;
		}
		BriefingSFX.Instance.CircleOpen.Play2D();
		if (OuterCircleImage != null && InnerCircleImage != null)
		{
			OuterCircleImage.gameObject.ScaleTo(Vector3.one, 0.2f, 0f, EaseType.easeInOutCubic);
			InnerCircleImage.gameObject.ScaleTo(Vector3.one, 0.2f, 0f, EaseType.easeInOutCubic);
			while (totalTime < 0.2f)
			{
				totalTime += TimeManager.DeltaTime;
				yield return null;
			}
		}
		float startTime6 = totalTime;
		while (mOpenLinesProgress < 1f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenLinesProgress = (totalTime - startTime6) / 0.2f;
			mOpenLinesProgress = Mathf.Clamp(mOpenLinesProgress, 0f, 1f);
			MissionBriefingHelper.UpdateLineOpen(LinesImage, mOpenLinesProgress, mCachedLineUVs);
			yield return null;
		}
		if (FocusImageFrame != null)
		{
			if ((bool)frame)
			{
				FocusImageFrame.mat.mainTexture = frame;
			}
			FocusImageFrame.gameObject.SetActive(true);
		}
		BriefingSFX.Instance.BoxOpen.Play2D();
		startTime6 = totalTime;
		while (mOpenFrameProgress < 1f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenFrameProgress = (totalTime - startTime6) / 0.3f;
			mOpenFrameProgress = Mathf.Clamp(mOpenFrameProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFrameOpen(FocusImageFrame, base.transform, mOpenFrameProgress, mImageStartPosition, mImageWidth, mImageHeight, direction);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Play2D();
		MissionBriefingHelper.CorrectImageSize(FocusImage, base.transform, mScreenSize, mOriginalSize);
		startTime6 = totalTime;
		while (mOpenImageProgress < 1f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenImageProgress = (totalTime - startTime6) / 0.8f;
			mOpenImageProgress = Mathf.Clamp(mOpenImageProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFlicker(FocusImage, mOpenImageProgress);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Stop2D();
		float timeToDisplay = totalTime;
		float timeOnScreen = duration - timeToDisplay * 2f;
		while (totalTime - timeToDisplay < timeOnScreen)
		{
			totalTime += TimeManager.DeltaTime;
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Play2D();
		startTime6 = totalTime;
		while (mOpenImageProgress > 0f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenImageProgress = (totalTime - startTime6) / 0.8f;
			mOpenImageProgress = 1f - Mathf.Clamp(mOpenImageProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFlicker(FocusImage, mOpenImageProgress);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Stop2D();
		BriefingSFX.Instance.BoxClose.Play2D();
		startTime6 = totalTime;
		while (mOpenFrameProgress > 0f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenFrameProgress = (totalTime - startTime6) / 0.3f;
			mOpenFrameProgress = 1f - Mathf.Clamp(mOpenFrameProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFrameOpen(FocusImageFrame, base.transform, mOpenFrameProgress, mImageStartPosition, mImageWidth, mImageHeight, direction);
			yield return null;
		}
		if (FocusImageFrame != null)
		{
			FocusImageFrame.gameObject.SetActive(false);
		}
		startTime6 = totalTime;
		while (mOpenLinesProgress > 0f)
		{
			totalTime += TimeManager.DeltaTime;
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
				totalTime += TimeManager.DeltaTime;
				yield return null;
			}
		}
		yield return new WaitForEndOfFrame();
	}

	private void LayoutComponents(Vector2 size, MissionBriefingHelper.FocusDirection direction)
	{
		float num = 5f;
		float num2 = 6f;
		float num3 = 0f;
		float num4 = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		bool flag = TBFUtils.IsRetinaHdDevice();
		bool flag2 = TBFUtils.UseAlternativeLayout();
		if (flag && !flag2)
		{
			num *= 2f;
			num2 *= 2f;
			size *= 2f;
		}
		mOriginalSize = size;
		if (!flag && flag2)
		{
			num *= 0.5f;
			num2 *= 0.5f;
			FocusImage.pixelPerfect = false;
			size *= 0.5f;
		}
		mScreenSize = size;
		MissionBriefingHelper.CorrectImageSize(FocusImage, base.transform, mScreenSize, mOriginalSize);
		if (OuterCircleImage != null)
		{
			num3 = OuterCircleImage.width / num4;
		}
		if (LinesImage != null)
		{
			if (!LinesImage.Started)
			{
				LinesImage.Start();
			}
			if (mLinesWidth == 0f)
			{
				mLinesWidth = LinesImage.width / num4;
			}
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
			Vector3 vector2 = Camera.main.ScreenToWorldPoint(position);
			LinesImage.transform.position = vector2;
			LinesImage.transform.rotation = Quaternion.identity;
			LinesImage.transform.RotateAround(vector2, Vector3.forward, angle);
		}
		if (FocusImage != null && FocusImageFrame != null)
		{
			mImageWidth = size.x + num2 * 2f;
			mImageHeight = size.y + num2 * 2f;
			mImageStartPosition = num3 * 0.5f + mLinesWidth - num * 4f;
			Vector3 position2 = vector;
			switch (direction)
			{
			case MissionBriefingHelper.FocusDirection.Left:
				position2.x += mImageStartPosition + mImageWidth * 0.5f;
				break;
			case MissionBriefingHelper.FocusDirection.Right:
				position2.x -= mImageStartPosition + mImageWidth * 0.5f;
				break;
			case MissionBriefingHelper.FocusDirection.Top:
				position2.y += mImageStartPosition + mImageHeight * 0.5f;
				break;
			case MissionBriefingHelper.FocusDirection.Bottom:
				position2.y -= mImageStartPosition + mImageHeight * 0.5f;
				break;
			}
			Vector3 vector2 = Camera.main.ScreenToWorldPoint(position2);
			FocusImage.transform.position = vector2;
			FocusImageFrame.size.x = 0f;
			FocusImageFrame.size.y = 0f;
			FocusImageFrame.Resize();
		}
	}

	private void SetToStartState()
	{
		if (OuterCircleImage != null && InnerCircleImage != null)
		{
			OuterCircleImage.transform.localScale = Vector3.zero;
			InnerCircleImage.transform.localScale = Vector3.zero;
		}
		if (LinesImage != null)
		{
			LinesImage.Start();
			mCachedLineUVs = LinesImage.GetUVs();
			LinesImage.SetUVs(default(Rect));
		}
		Color color = new Color(1f, 1f, 1f, 0f);
		if (FocusImage != null)
		{
			FocusImage.SetColor(color);
		}
		if (FocusImageFrame != null)
		{
			FocusImageFrame.size.x = 0f;
			FocusImageFrame.size.y = 0f;
			FocusImageFrame.Resize();
		}
		mOpenLinesProgress = 0f;
	}

	private void Update()
	{
		if (InnerCircleImage != null)
		{
			InnerCircleImage.transform.Rotate(new Vector3(0f, 0f, 0f - 20f * TimeManager.DeltaTime));
		}
	}
}
