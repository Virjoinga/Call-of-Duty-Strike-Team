using System;
using UnityEngine;

[Serializable]
public class SingleAnimation
{
	public AnimationClip Clip;

	private Animation mPlayer;

	private string mName;

	public AnimationState State { get; private set; }

	public float Duration
	{
		get
		{
			if (State == null)
			{
				return 0f;
			}
			return State.length;
		}
	}

	public float NormalisedTime
	{
		get
		{
			return State.normalizedTime;
		}
		set
		{
			State.normalizedTime = value;
		}
	}

	public float RemainingTime
	{
		get
		{
			if (State == null || !State.enabled)
			{
				return 0f;
			}
			return (State.length - State.time) / State.speed;
		}
	}

	public void Initialise(string name, Animation actor)
	{
		mName = name;
		mPlayer = actor;
		State = actor.AddClipSafe(Clip, name);
	}

	public void Reset()
	{
		if (State != null)
		{
			State.enabled = false;
			State = null;
			mPlayer.RemoveClip(mName);
		}
		mName = null;
		mPlayer = null;
	}

	public void ResetTime()
	{
		if (State != null)
		{
			State.time = 0f;
		}
	}

	public void ClampToEnd()
	{
		if (State != null)
		{
			State.enabled = true;
			State.speed = 0f;
			State.wrapMode = WrapMode.ClampForever;
			State.normalizedTime = 1f;
		}
	}

	public void Enable(bool isEnabled)
	{
		if (State != null)
		{
			State.enabled = isEnabled;
		}
	}
}
