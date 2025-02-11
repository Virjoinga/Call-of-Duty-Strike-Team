using UnityEngine;

public class HackingBlip : HudBlipIcon
{
	private UIProgressBar mProgressBar;

	private float mProgress;

	public float mPauseTime = 1f;

	public override void Start()
	{
		base.Start();
		base.ClampToEdgeOfScreen = true;
		IsAllowedInFirstPerson = true;
		mProgressBar = base.gameObject.GetComponent<UIProgressBar>();
		mProgressBar.Hide(true);
	}

	public override void LateUpdate()
	{
		if (!(GUISystem.Instance == null))
		{
			if (mProgressBar != null)
			{
				mProgressBar.Value = mProgress;
			}
			base.LateUpdate();
		}
	}

	public void SetProgress(float progress)
	{
		mProgress = progress;
	}

	public void HideBlip()
	{
		if (mProgressBar != null)
		{
			mProgressBar.Hide(true);
		}
	}

	public void ShowBlip()
	{
		if (mProgressBar != null)
		{
			mProgressBar.Hide(false);
		}
	}

	public bool IsVisible()
	{
		if (mProgressBar != null)
		{
			return !mProgressBar.IsHidden();
		}
		return false;
	}

	public void RemoveBlip()
	{
		Object.Destroy(base.gameObject);
	}
}
