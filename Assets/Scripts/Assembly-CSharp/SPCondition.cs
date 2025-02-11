using System;
using UnityEngine;

[Serializable]
public class SPCondition
{
	public enum SPConditionType
	{
		Invalid = 0,
		Always = 1,
		WaitForActorAnim = 2,
		WaitForTime = 3,
		WaitForFrames = 4,
		WaitForActorToReachPoint = 5,
		SkipOnGesture = 6,
		WaitForSignal = 7,
		OutroEnd = 8,
		NumConditions = 9
	}

	public enum ReturnCode
	{
		Fail = 0,
		Pass = 1,
		Skip = 2
	}

	private const int kAnimationFramesPerSecond = 30;

	private const float kOutroTransitionTime = 0.5f;

	public SPConditionType Type;

	public SPObjectReference Actor;

	public SPObjectReference Point;

	public int Frames;

	public float Timer;

	public float MinusDelay;

	public string Signal;

	private float TimeStarted;

	public SPGestureDetector gestureDetector;

	public SPCondition(SPConditionType type)
	{
		Type = type;
	}

	public void StartCondition(float statementTime)
	{
		TimeStarted = statementTime;
		if (gestureDetector != null)
		{
			gestureDetector.Activate();
		}
		SPConditionType type = Type;
		if ((type == SPConditionType.WaitForActorAnim || type == SPConditionType.OutroEnd) && Actor != null)
		{
			float num = 0f;
			if (Actor.CurrentAnim != null)
			{
				num = ((!(Actor.AnimDirector != null)) ? Actor.ActorAnimation[Actor.CurrentAnim.name].length : Actor.AnimDirector.GetCategoryLength(Actor.AnimDirector.GetCategoryHandle("SetPiece")));
			}
			if (Actor.AnimationStartTime > 0f)
			{
				Timer = num - MinusDelay - (TimeStarted - Actor.AnimationStartTime);
			}
			else
			{
				Timer = num - MinusDelay;
			}
			if (Timer < 0f)
			{
				Timer = 0f;
			}
		}
	}

	public ReturnCode TestCondition(float statementTime)
	{
		ReturnCode result = ReturnCode.Fail;
		switch (Type)
		{
		case SPConditionType.Always:
			result = ReturnCode.Pass;
			break;
		case SPConditionType.WaitForActorAnim:
		case SPConditionType.WaitForTime:
		case SPConditionType.WaitForFrames:
			if (statementTime - TimeStarted >= Timer)
			{
				result = ReturnCode.Pass;
			}
			break;
		case SPConditionType.SkipOnGesture:
			if (gestureDetector != null)
			{
				result = ((!gestureDetector.detected) ? ReturnCode.Pass : ReturnCode.Skip);
			}
			break;
		case SPConditionType.OutroEnd:
			if (statementTime - TimeStarted >= Timer - 0.5f && GameController.Instance != null)
			{
				GameController.Instance.OnMissionPassed(this, 0f);
			}
			if (statementTime - TimeStarted >= Timer)
			{
				result = ReturnCode.Pass;
			}
			break;
		}
		return result;
	}

	public void ReceiveSignal(string sig)
	{
		if (Signal != null && Signal.Length != 0 && Signal == sig)
		{
			Type = SPConditionType.WaitForTime;
			TimeStarted = Time.time;
		}
	}

	public void Reset()
	{
		gestureDetector.Deactivate();
	}
}
