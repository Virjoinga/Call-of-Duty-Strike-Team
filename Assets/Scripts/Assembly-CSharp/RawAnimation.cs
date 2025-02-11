using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RawAnimation
{
	public enum AnimBlendType
	{
		kLinearCrossFade = 0,
		kSnapTo = 1,
		kSoftSoft = 2,
		kSoftLinear = 3,
		kSoftSharp = 4,
		kLinearSoft = 5,
		kLinearSharp = 6,
		kSharpSoft = 7,
		kSharpLinear = 8,
		kSharpSharp = 9
	}

	public const int kMaxBlendTreeBranches = 10;

	public int RefIndex;

	public AnimationClip AnimClip;

	public bool Looping;

	public bool Clamp;

	public bool ForceAdditive;

	public AnimBlendType BlendType;

	public float BlendTime = 0.25f;

	public string OverrideBone;

	public bool Multibone;

	public float Speed = 1f;

	public bool PreventAiming;

	public bool PreventReloading;

	public bool Monopoly;

	public SegueData segueData;

	public List<AnimationEvent> Events;

	public RawAnimation[] Branches;

	public int branchCount = 1;

	public AnimDirector.BlendEasing EaseBegin()
	{
		return EaseBegin(BlendType);
	}

	public static AnimDirector.BlendEasing EaseBegin(AnimBlendType b)
	{
		switch (b)
		{
		case AnimBlendType.kSoftSoft:
		case AnimBlendType.kSoftLinear:
		case AnimBlendType.kSoftSharp:
			return AnimDirector.BlendEasing.Soft;
		case AnimBlendType.kLinearCrossFade:
		case AnimBlendType.kLinearSoft:
		case AnimBlendType.kLinearSharp:
			return AnimDirector.BlendEasing.Linear;
		case AnimBlendType.kSharpSoft:
		case AnimBlendType.kSharpLinear:
		case AnimBlendType.kSharpSharp:
			return AnimDirector.BlendEasing.Sharp;
		default:
			return AnimDirector.BlendEasing.Linear;
		}
	}

	public AnimDirector.BlendEasing EaseEnd()
	{
		return EaseEnd(BlendType);
	}

	public static AnimDirector.BlendEasing EaseEnd(AnimBlendType b)
	{
		switch (b)
		{
		case AnimBlendType.kSoftSoft:
		case AnimBlendType.kLinearSoft:
		case AnimBlendType.kSharpSoft:
			return AnimDirector.BlendEasing.Soft;
		case AnimBlendType.kLinearCrossFade:
		case AnimBlendType.kSoftLinear:
		case AnimBlendType.kSharpLinear:
			return AnimDirector.BlendEasing.Linear;
		case AnimBlendType.kSoftSharp:
		case AnimBlendType.kLinearSharp:
		case AnimBlendType.kSharpSharp:
			return AnimDirector.BlendEasing.Sharp;
		default:
			return AnimDirector.BlendEasing.Linear;
		}
	}

	public RawAnimation GetBranch(int b)
	{
		if (b == 0)
		{
			return this;
		}
		return Branches[b - 1];
	}
}
