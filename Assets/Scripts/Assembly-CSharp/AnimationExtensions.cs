using UnityEngine;

public static class AnimationExtensions
{
	public static AnimationState AddClipSafe(this Animation animation, AnimationClip clip, string name)
	{
		AnimationState animationState = animation[name];
		if (animationState == null && clip != null)
		{
			animation.AddClip(clip, name);
			return animation[name];
		}
		return animationState;
	}
}
