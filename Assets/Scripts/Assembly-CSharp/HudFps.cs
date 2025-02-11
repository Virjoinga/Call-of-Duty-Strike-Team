using UnityEngine;

public class HudFps : MonoBehaviour
{
	public float updateInterval = 0.5f;

	public bool ShowFpsInLog;

	private float accum;

	private int frames;

	private float timeleft;

	private string mFpsAsText = string.Empty;

	private float mFpsAsFloat;

	private SpriteText mSpriteText;

	public void Hide(bool hide)
	{
		if (mSpriteText != null)
		{
			mSpriteText.Hide(hide);
		}
	}

	public bool IsHidden()
	{
		if (mSpriteText != null)
		{
			return mSpriteText.IsHidden();
		}
		return true;
	}

	private void Start()
	{
		timeleft = updateInterval;
		mSpriteText = base.gameObject.GetComponent<SpriteText>();
		if (mSpriteText != null)
		{
			mSpriteText.Hide(true);
		}
	}

	private void Update()
	{
		if (!(mSpriteText != null) || mSpriteText.IsHidden())
		{
			return;
		}
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if (!((double)timeleft <= 0.0))
		{
			return;
		}
		mFpsAsFloat = accum / (float)frames;
		string text = string.Format("{0:F2} FPS", mFpsAsFloat);
		mFpsAsText = text;
		if (ShowFpsInLog)
		{
			Debug.Log(mFpsAsText);
		}
		timeleft = updateInterval;
		accum = 0f;
		frames = 0;
		if ((bool)mSpriteText)
		{
			if (mFpsAsFloat < 30f)
			{
				mSpriteText.Color = Color.yellow;
			}
			else if (mFpsAsFloat < 10f)
			{
				mSpriteText.Color = Color.red;
			}
			else
			{
				mSpriteText.Color = Color.green;
			}
			mSpriteText.Text = mFpsAsText;
		}
		else
		{
			mSpriteText = base.gameObject.GetComponent<SpriteText>();
		}
	}
}
