using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationGroup
{
	public enum AnimType
	{
		Run = 0,
		Idle = 1,
		Dead = 2,
		IdleToFiring = 3,
		Firing = 4,
		FiringToIdle = 5,
		IdleCrouch = 6
	}

	public AnimationList[] Anims;

	private Dictionary<int, AnimationList> mLookupTable;

	public void Init()
	{
		mLookupTable = new Dictionary<int, AnimationList>();
		AnimationList[] anims = Anims;
		foreach (AnimationList animationList in anims)
		{
			mLookupTable.Add((int)animationList.Type, animationList);
		}
	}

	public AnimationClip GetAnim(AnimType type, ref int chosenIndex, int forceIndex)
	{
		AnimationList animationListFromType = GetAnimationListFromType(type);
		if (animationListFromType != null)
		{
			if (forceIndex >= 0)
			{
				return animationListFromType.GetAnim(forceIndex);
			}
			return animationListFromType.GetAnim(ref chosenIndex);
		}
		return null;
	}

	private AnimationList GetAnimationListFromType(AnimType type)
	{
		AnimationList value = null;
		mLookupTable.TryGetValue((int)type, out value);
		return value;
	}

	public void LoadAnimations(Animation animComponent)
	{
		AnimationList[] anims = Anims;
		foreach (AnimationList animationList in anims)
		{
			animationList.LoadAnimations(animComponent);
		}
	}

	public void UnloadAnimations(Animation animComponent)
	{
		AnimationList[] anims = Anims;
		foreach (AnimationList animationList in anims)
		{
			animationList.UnloadAnimations(animComponent);
		}
	}
}
