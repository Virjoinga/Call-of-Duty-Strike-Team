using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public enum SlowTimeType
	{
		Adrenaline = 0,
		Breach = 1,
		TankBreach = 2,
		Hostage = 3
	}

	public enum State
	{
		IngameRunning_Gameplay = 0,
		IngameRunning_SetPiece = 1,
		IngamePaused = 2
	}

	private enum TimeScaleState
	{
		Idle = 0,
		SlowingDown = 1,
		ResumingToNormal = 2
	}

	public const float kDefaultSlowTimeScale = 0.01f;

	public float SlowTimeScale = 0.01f;

	public float TimeToSlowDown = 1f;

	public float TimeToResume = 1f;

	public static TimeManager instance;

	public static float TIMESCALE_BREACH = 0.1f;

	private static float mLastFrameRealtimeSinceStartup;

	private static float mDeltaTime;

	private float mPrevStateTimeScale;

	private float mDuratonToSlowForRemaining;

	private float mSlowTimeDuration;

	private float mTargetTimeScale;

	private float mTimeScaleStartTime;

	private float mTimeScaleAtStartOfChange;

	private TimeScaleState mTimeScaleState;

	private State mPrevGlobalTimeState;

	public float StopTimeModeSpeed
	{
		get
		{
			return 1f;
		}
	}

	public float CurrentTargetTimeScale
	{
		get
		{
			return mTargetTimeScale;
		}
	}

	public State GlobalTimeState { get; set; }

	public static float DeltaTime
	{
		get
		{
			if (mDeltaTime == 0f)
			{
				return Time.deltaTime;
			}
			return mDeltaTime;
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			throw new Exception("Can not have multiple TimeManager");
		}
		instance = this;
	}

	private void Start()
	{
		mTargetTimeScale = 1f;
		mTimeScaleAtStartOfChange = 1f;
		mSlowTimeDuration = 0f;
		GlobalTimeState = State.IngameRunning_Gameplay;
		mTimeScaleState = TimeScaleState.Idle;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mDeltaTime = realtimeSinceStartup - mLastFrameRealtimeSinceStartup;
		mLastFrameRealtimeSinceStartup = realtimeSinceStartup;
		if (GlobalTimeState != State.IngamePaused)
		{
			UpdateTimeScale();
		}
	}

	public void PauseGame()
	{
		if (GlobalTimeState != State.IngamePaused)
		{
			mPrevStateTimeScale = mTargetTimeScale;
			mDuratonToSlowForRemaining = mSlowTimeDuration - Time.realtimeSinceStartup;
			StopTime();
			mPrevGlobalTimeState = GlobalTimeState;
			GlobalTimeState = State.IngamePaused;
			SoundManager.Instance.ActivatePaused();
		}
	}

	public void UnpauseGame()
	{
		if (GlobalTimeState == State.IngamePaused)
		{
			ResumeNormalTime(mPrevStateTimeScale);
			if (mDuratonToSlowForRemaining <= 0f)
			{
				mSlowTimeDuration = 0f;
			}
			else
			{
				mSlowTimeDuration = Time.realtimeSinceStartup + mDuratonToSlowForRemaining;
			}
			GlobalTimeState = mPrevGlobalTimeState;
			SoundManager.Instance.DeActivatePaused(true);
		}
	}

	public void StopTime()
	{
		SlowDownTime(0f, 0f);
		Time.timeScale = 0.0001f;
		mTimeScaleState = TimeScaleState.Idle;
	}

	public void DisableSlomo()
	{
		mPrevStateTimeScale = 1f;
		ResumeNormalTime();
		SlowTimeScale = 1f;
	}

	public float GetTimeScaleForType(SlowTimeType type)
	{
		float result = 1f;
		switch (type)
		{
		case SlowTimeType.Adrenaline:
			result = SlowTimeScale;
			break;
		case SlowTimeType.Breach:
		case SlowTimeType.TankBreach:
			result = TIMESCALE_BREACH;
			break;
		case SlowTimeType.Hostage:
			result = 0.3f;
			break;
		}
		return result;
	}

	public void SlowDownTime(float duratonToSlowFor, SlowTimeType type)
	{
		bool flag = false;
		float timeScaleForType = GetTimeScaleForType(type);
		SlowDownTime(duratonToSlowFor, timeScaleForType);
		if (flag)
		{
			GlobalTimeState = State.IngameRunning_SetPiece;
		}
	}

	public void SlowDownTime(float duratonToSlowFor, float timeSpeed)
	{
		if (GlobalTimeState != State.IngameRunning_SetPiece)
		{
			mTargetTimeScale = timeSpeed;
			mTimeScaleAtStartOfChange = Time.timeScale;
			mTimeScaleStartTime = Time.realtimeSinceStartup;
			if (duratonToSlowFor <= 0f)
			{
				mSlowTimeDuration = 0f;
			}
			else
			{
				mSlowTimeDuration = Time.realtimeSinceStartup + duratonToSlowFor;
			}
			mTimeScaleState = TimeScaleState.SlowingDown;
		}
	}

	public void ResumeNormalTime()
	{
		ResumeNormalTime(1f);
	}

	public void ResumeNormalTime(float targetTimeScale)
	{
		if (GlobalTimeState != State.IngameRunning_SetPiece)
		{
			mTargetTimeScale = targetTimeScale;
			mTimeScaleAtStartOfChange = Time.timeScale;
			mTimeScaleStartTime = Time.realtimeSinceStartup;
			mTimeScaleState = TimeScaleState.ResumingToNormal;
		}
	}

	public void ResumeNormalTimeImmediate()
	{
		Time.timeScale = 1f;
		mTargetTimeScale = 1f;
		mTimeScaleAtStartOfChange = Time.timeScale;
		mTimeScaleStartTime = Time.realtimeSinceStartup;
		mTimeScaleState = TimeScaleState.Idle;
	}

	private void UpdateTimeScale()
	{
		float num = 1f;
		switch (mTimeScaleState)
		{
		case TimeScaleState.Idle:
			CheckSlowMotionTimeout();
			break;
		case TimeScaleState.SlowingDown:
		{
			num = (Time.realtimeSinceStartup - mTimeScaleStartTime) / TimeToSlowDown;
			float num2 = TweenFunctions.tween(TweenFunctions.TweenType.easeOutSine, mTimeScaleAtStartOfChange, mTargetTimeScale, num);
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			Time.timeScale = num2;
			if (num >= 1f)
			{
				Time.timeScale = mTargetTimeScale;
				mTimeScaleState = TimeScaleState.Idle;
				CheckSlowMotionTimeout();
			}
			else
			{
				CheckSlowMotionTimeout();
			}
			break;
		}
		case TimeScaleState.ResumingToNormal:
			num = (Time.realtimeSinceStartup - mTimeScaleStartTime) / TimeToResume;
			Time.timeScale = TweenFunctions.tween(TweenFunctions.TweenType.easeInSine, mTimeScaleAtStartOfChange, mTargetTimeScale, num);
			if (num >= 1f)
			{
				Time.timeScale = mTargetTimeScale;
				mTimeScaleState = TimeScaleState.Idle;
			}
			break;
		}
		float from = 0.004f;
		if (GameController.Instance != null && !GameController.Instance.IsPlayerBreaching)
		{
			from = 0.01f;
		}
		Time.fixedDeltaTime = Mathf.Lerp(from, 0.04f, Mathf.InverseLerp(0.1f, 1f, Time.timeScale));
	}

	private void CheckSlowMotionTimeout()
	{
		if (mSlowTimeDuration > 0f && mSlowTimeDuration < Time.realtimeSinceStartup)
		{
			EndSlowDown();
		}
	}

	private void EndSlowDown()
	{
		mSlowTimeDuration = 0f;
		GlobalTimeState = State.IngameRunning_Gameplay;
		ResumeNormalTime();
	}
}
