using System.Collections;
using UnityEngine;

public class MissionBriefingImageItem : MonoBehaviour
{
	private const float IMAGE_FRAME_BORDER = 6f;

	private const float FRAME_OPEN_TIME = 0.3f;

	private const float IMAGE_ON_TIME = 0.8f;

	private const float IMAGE_BORDER = 0f;

	public SimpleSprite FocusImage;

	public Scale9Grid FocusImageFrame;

	private Vector2 mOriginalSize;

	private Vector2 mScreenSize;

	private float mImageStartPosition;

	private float mImageWidth;

	private float mImageHeight;

	private float mOpenFrameProgress;

	private float mOpenImageProgress;

	private bool mWaiting;

	private bool mBusy;

	public bool Waiting
	{
		get
		{
			return mWaiting;
		}
	}

	public bool IsBusy
	{
		get
		{
			return mBusy;
		}
	}

	public void ReleaseFromWait()
	{
		mWaiting = false;
	}

	public void Awake()
	{
		base.transform.position = new Vector3(0f, 0f, 100f);
	}

	public void DisplayItem(Texture focusImage, Vector3 position, Vector2 size, float duration, Texture2D frame)
	{
		if (focusImage != null)
		{
			mBusy = true;
			if (FocusImage != null && FocusImage.GetComponent<Renderer>() != null && FocusImage.GetComponent<Renderer>().material != null)
			{
				Resources.UnloadAsset(FocusImage.GetComponent<Renderer>().material.mainTexture);
				FocusImage.GetComponent<Renderer>().material.mainTexture = focusImage;
			}
			bool flag = true;
			bool resizeForAlternateLayout = true;
			LayoutComponents(size, flag, resizeForAlternateLayout);
			SetToStartState();
			StartCoroutine(DoDisplayItem(position, flag, duration, false, true, resizeForAlternateLayout, frame));
		}
	}

	public void DisplayAndHoldItem(Texture focusImage, Vector3 position, Vector2 size, float duration, bool adjustForRetina)
	{
		if (focusImage != null)
		{
			mBusy = true;
			mWaiting = true;
			if (FocusImage != null && FocusImage.GetComponent<Renderer>() != null && FocusImage.GetComponent<Renderer>().material != null)
			{
				Resources.UnloadAsset(FocusImage.GetComponent<Renderer>().material.mainTexture);
				FocusImage.GetComponent<Renderer>().material.mainTexture = focusImage;
			}
			bool resizeForAlternateLayout = false;
			LayoutComponents(size, adjustForRetina, resizeForAlternateLayout);
			SetToStartState();
			StartCoroutine(DoDisplayItem(position, adjustForRetina, duration, true, false, resizeForAlternateLayout, null));
		}
	}

	private void OnDestroy()
	{
		if (FocusImage != null && FocusImage.GetComponent<Renderer>() != null && FocusImage.GetComponent<Renderer>().material != null)
		{
			Resources.UnloadAsset(FocusImage.GetComponent<Renderer>().material.mainTexture);
		}
	}

	private IEnumerator DoDisplayItem(Vector3 position, bool adjustForRetina, float duration, bool hold, bool destroy, bool resizeForAlternateLayout, Texture2D frame)
	{
		float totalTime = 0f;
		bool retina = TBFUtils.IsRetinaHdDevice();
		bool alternateLayout = TBFUtils.UseAlternativeLayout();
		if (!alternateLayout && adjustForRetina && retina)
		{
			position *= 2f;
		}
		if (alternateLayout && resizeForAlternateLayout && !retina)
		{
			position.x += (mOriginalSize - mScreenSize).x * 0.5f;
		}
		base.transform.position = Camera.main.ScreenToWorldPoint(position);
		if (FocusImageFrame != null)
		{
			if (frame != null)
			{
				FocusImageFrame.mat.mainTexture = frame;
			}
			FocusImageFrame.gameObject.SetActive(true);
		}
		BriefingSFX.Instance.BoxOpen.Play2D();
		float startTime4 = totalTime;
		while (mOpenFrameProgress < 1f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenFrameProgress = (totalTime - startTime4) / 0.3f;
			mOpenFrameProgress = Mathf.Clamp(mOpenFrameProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFrameOpen(FocusImageFrame, base.transform, mOpenFrameProgress, mImageStartPosition, mImageWidth, mImageHeight, MissionBriefingHelper.FocusDirection.Left);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Play2D();
		MissionBriefingHelper.CorrectImageSize(FocusImage, base.transform, mScreenSize, mOriginalSize);
		startTime4 = totalTime;
		while (mOpenImageProgress < 1f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenImageProgress = (totalTime - startTime4) / 0.8f;
			mOpenImageProgress = Mathf.Clamp(mOpenImageProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFlicker(FocusImage, mOpenImageProgress);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Stop2D();
		float timeToDisplay = totalTime;
		float timeOnScreen = duration - timeToDisplay * 2f;
		while (totalTime - timeToDisplay < timeOnScreen || (hold && Waiting))
		{
			totalTime += TimeManager.DeltaTime;
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Play2D();
		startTime4 = totalTime;
		while (mOpenImageProgress > 0f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenImageProgress = (totalTime - startTime4) / 0.8f;
			mOpenImageProgress = 1f - Mathf.Clamp(mOpenImageProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFlicker(FocusImage, mOpenImageProgress);
			yield return null;
		}
		BriefingSFX.Instance.BoxStatic.Stop2D();
		BriefingSFX.Instance.BoxClose.Play2D();
		startTime4 = totalTime;
		while (mOpenFrameProgress > 0f)
		{
			totalTime += TimeManager.DeltaTime;
			mOpenFrameProgress = (totalTime - startTime4) / 0.3f;
			mOpenFrameProgress = 1f - Mathf.Clamp(mOpenFrameProgress, 0f, 1f);
			MissionBriefingHelper.UpdateFrameOpen(FocusImageFrame, base.transform, mOpenFrameProgress, mImageStartPosition, mImageWidth, mImageHeight, MissionBriefingHelper.FocusDirection.Left);
			yield return null;
		}
		if (FocusImageFrame != null)
		{
			FocusImageFrame.gameObject.SetActive(false);
		}
		if (destroy)
		{
			Object.Destroy(base.gameObject);
		}
		mBusy = false;
		yield return new WaitForEndOfFrame();
	}

	private void LayoutComponents(Vector2 size, bool adjustSizeForRetina, bool resizeForAlternateLayout)
	{
		float num = 0f;
		float num2 = 6f;
		Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		bool flag = TBFUtils.IsRetinaHdDevice();
		bool flag2 = TBFUtils.UseAlternativeLayout();
		if (flag && !flag2)
		{
			num *= 2f;
			num2 *= 2f;
			if (adjustSizeForRetina)
			{
				size *= 2f;
			}
		}
		mOriginalSize = size;
		if (!flag && flag2 && resizeForAlternateLayout)
		{
			num *= 0.5f;
			num2 *= 0.5f;
			FocusImage.pixelPerfect = false;
			size *= 0.5f;
		}
		mScreenSize = size;
		MissionBriefingHelper.CorrectImageSize(FocusImage, base.transform, mScreenSize, mOriginalSize);
		if (FocusImage != null && FocusImageFrame != null)
		{
			mImageWidth = size.x + num2 * 2f;
			mImageHeight = size.y + num2 * 2f;
			mImageStartPosition = num * 4f;
			Vector3 position = vector;
			position.x += mImageStartPosition + mImageWidth * 0.5f;
			Vector3 position2 = Camera.main.ScreenToWorldPoint(position);
			FocusImage.transform.position = position2;
			FocusImageFrame.size.x = 0f;
			FocusImageFrame.size.y = 0f;
			FocusImageFrame.Resize();
		}
	}

	private void SetToStartState()
	{
		if (FocusImage != null)
		{
			Color color = new Color(1f, 1f, 1f, 0f);
			FocusImage.SetColor(color);
		}
	}
}
