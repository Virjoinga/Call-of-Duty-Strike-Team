using System;
using UnityEngine;

[Serializable]
public class PairedAnimation
{
	public AnimationClip AnimationA;

	public AnimationClip AnimationB;

	public AnimationState StateA { get; private set; }

	public AnimationState StateB { get; private set; }

	public float Duration
	{
		get
		{
			return Mathf.Max(StateA.length, StateB.length);
		}
	}

	public float NormalisedTime
	{
		get
		{
			return Mathf.Max(StateA.normalizedTime, StateB.normalizedTime);
		}
		set
		{
			StateA.normalizedTime = value;
			StateB.normalizedTime = value;
		}
	}

	public float RemainingTime
	{
		get
		{
			return Mathf.Max((StateA.length - StateA.time) / StateA.speed, (StateB.length - StateB.time) / StateB.speed);
		}
	}

	public void Initialise(string name, Animation actorA, Animation actorB)
	{
		StateA = actorA.AddClipSafe(AnimationA, name);
		StateB = actorB.AddClipSafe(AnimationB, name);
	}
}
