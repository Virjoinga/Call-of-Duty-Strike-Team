using UnityEngine;

public class ProgressBar : MonoBehaviour
{
	public SimpleSprite BackgroundBar;

	public PackedSprite ForegroundBar;

	public PackedSprite PreviewBar;

	public GameObject RightPin;

	private float mValue;

	private float mPreviewValue;

	private float mCurrent;

	private float mWidthInPixels;

	private float mHeightInPixels;

	private float mAnimationSpeed = 0.25f;

	private float mTime;

	private float mPixelSize;

	private bool mDirty;

	private bool mSoundPlaying;

	public float PixelWidth
	{
		get
		{
			return mWidthInPixels;
		}
		set
		{
			mWidthInPixels = value;
			mDirty = true;
		}
	}

	public float PixelHeight
	{
		get
		{
			return mHeightInPixels;
		}
	}

	public bool IsUpdating
	{
		get
		{
			return mValue != mCurrent;
		}
	}

	public float AnimationSpeed
	{
		get
		{
			return mAnimationSpeed;
		}
		set
		{
			mAnimationSpeed = value;
		}
	}

	public bool SoundActive { get; set; }

	public void OnDestroy()
	{
		StopSound();
	}

	private void Start()
	{
		mValue = 0f;
		mCurrent = 0f;
		mDirty = true;
		mPixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		if (ForegroundBar != null)
		{
			Texture mainTexture = ForegroundBar.renderer.material.mainTexture;
			Rect uVs = ForegroundBar.GetUVs();
			if (RightPin == null)
			{
				mWidthInPixels = ((mWidthInPixels != 0f) ? mWidthInPixels : (ForegroundBar.width / mPixelSize));
			}
			else
			{
				mWidthInPixels = (RightPin.transform.position.x - BackgroundBar.transform.position.x) / mPixelSize;
			}
			mHeightInPixels = (float)mainTexture.height * uVs.height;
			ForegroundBar.SetSize(0f, mHeightInPixels * mPixelSize);
			BackgroundBar.SetSize(mWidthInPixels * mPixelSize, mHeightInPixels * mPixelSize);
			if (PreviewBar != null)
			{
				PreviewBar.SetSize(0f, mHeightInPixels * mPixelSize);
			}
		}
	}

	private void Update()
	{
		UpdateCurrent();
		UpdatePreview();
		UpdateSound();
	}

	public void Hide(bool hide)
	{
		StopSound();
		if (ForegroundBar != null && BackgroundBar != null)
		{
			ForegroundBar.Hide(hide);
			BackgroundBar.Hide(hide);
		}
		if (PreviewBar != null)
		{
			PreviewBar.Hide(hide);
		}
	}

	public void SetValueNow(float val)
	{
		mValue = val;
		mCurrent = val;
		mDirty = true;
		mTime = 0f;
	}

	public void SetValue(float val)
	{
		mValue = val;
		mPreviewValue = val;
		mDirty = true;
		mTime = Time.realtimeSinceStartup;
	}

	public void SetPreviewValue(float val)
	{
		mPreviewValue = val;
	}

	private void UpdateCurrent()
	{
		if ((mValue != mCurrent || mDirty) && ForegroundBar != null && BackgroundBar != null)
		{
			float num = Time.realtimeSinceStartup - mTime;
			num = Mathf.Clamp(num * mAnimationSpeed, 0f, 1f);
			mCurrent = Mathf.Lerp(mCurrent, mValue, num);
			if (Mathf.Approximately(mCurrent, mValue))
			{
				mCurrent = mValue;
			}
			ForegroundBar.SetSize(mWidthInPixels * mPixelSize * mCurrent, mHeightInPixels * mPixelSize);
			BackgroundBar.SetSize(mWidthInPixels * mPixelSize, mHeightInPixels * mPixelSize);
			mDirty = false;
		}
	}

	private void UpdatePreview()
	{
		if (mPreviewValue != mCurrent && PreviewBar != null)
		{
			Vector3 position = base.transform.position;
			position.x += mWidthInPixels * mCurrent * mPixelSize;
			position.z = PreviewBar.transform.position.z;
			float num = mPreviewValue - mCurrent;
			PreviewBar.transform.position = position;
			PreviewBar.SetSize(mWidthInPixels * mPixelSize * num, mHeightInPixels * mPixelSize);
		}
	}

	private void UpdateSound()
	{
		if (SoundActive)
		{
			if (mValue != mCurrent)
			{
				StartSound();
			}
			else
			{
				StopSound();
			}
		}
	}

	private void StartSound()
	{
		if (!mSoundPlaying)
		{
			mSoundPlaying = true;
			MenuSFX.Instance.XPBarFill.Play2D();
		}
	}

	private void StopSound()
	{
		if (mSoundPlaying)
		{
			mSoundPlaying = false;
			MenuSFX.Instance.XPBarFill.Stop2D();
		}
	}
}
