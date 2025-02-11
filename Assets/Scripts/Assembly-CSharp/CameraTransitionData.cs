using System;
using UnityEngine;

[Serializable]
public class CameraTransitionData
{
	public CameraBase CameraTo;

	public TweenFunctions.TweenType TweenType;

	public float Duration;

	private float mBlend;

	private float mBlendAmt;

	private float mRealTimeLastFrame = -1f;

	public float Progress
	{
		get
		{
			return mBlend;
		}
	}

	private CameraTransitionData()
	{
	}

	public CameraTransitionData(CameraBase to, TweenFunctions.TweenType type, float duration)
	{
		TBFAssert.DoAssert(to != null, "null cam on transition");
		CameraTo = to;
		TweenType = type;
		Duration = duration;
		Reset();
	}

	public CameraTransitionData(CameraBase to)
	{
		TBFAssert.DoAssert(to != null, "null cam on transition");
		CameraTo = to;
		TweenType = TweenFunctions.TweenType.linear;
		Duration = 0f;
		Reset();
	}

	public float UpdateProgress()
	{
		if (mRealTimeLastFrame < 0f || (GameController.Instance != null && GameController.Instance.IsPaused))
		{
			mRealTimeLastFrame = Time.realtimeSinceStartup;
		}
		mBlend = TweenFunctions.tween(TweenType, 0f, 1f, mBlendAmt);
		float num = Time.realtimeSinceStartup - mRealTimeLastFrame;
		mRealTimeLastFrame = Time.realtimeSinceStartup;
		mBlendAmt += num * (1f / Duration);
		float result = 0f;
		if (mBlendAmt > 1f)
		{
			result = mBlendAmt - 1f;
		}
		mBlendAmt = Mathf.Clamp01(mBlendAmt);
		mBlend = Mathf.Clamp01(mBlend);
		return result;
	}

	public void Reset()
	{
		mBlend = 0f;
		mBlendAmt = 0f;
		mRealTimeLastFrame = -1f;
	}
}
