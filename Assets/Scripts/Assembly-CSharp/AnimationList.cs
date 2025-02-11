using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationList
{
	public AnimationGroup.AnimType Type;

	public int AnimationLayer;

	public List<AnimationClip> Anims;

	public AnimationClip GetAnim(ref int index)
	{
		if (Anims.Count > 0 && index < Anims.Count - 1)
		{
			index = UnityEngine.Random.Range(0, Anims.Count);
			return Anims[index];
		}
		return null;
	}

	public AnimationClip GetAnim(int index)
	{
		if (Anims.Count > 0 && index < Anims.Count)
		{
			return Anims[index];
		}
		return null;
	}

	public void LoadAnimations(Animation animComponent)
	{
		foreach (AnimationClip anim in Anims)
		{
			if ((bool)anim)
			{
				if (!animComponent[anim.name])
				{
					animComponent.AddClip(anim, anim.name);
					animComponent[anim.name].layer = AnimationLayer;
				}
				else if (animComponent[anim.name].layer != AnimationLayer)
				{
					Debug.LogError("ERROR!!! The animation " + anim.name + " on layer " + AnimationLayer + " already exists for " + animComponent.gameObject.name + " on layer " + animComponent[anim.name].layer);
				}
			}
		}
	}

	public void UnloadAnimations(Animation animComponent)
	{
		foreach (AnimationClip anim in Anims)
		{
			if ((bool)anim)
			{
				animComponent.RemoveClip(anim);
			}
		}
	}
}
