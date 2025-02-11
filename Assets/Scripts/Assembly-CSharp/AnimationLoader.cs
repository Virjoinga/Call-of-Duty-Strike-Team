using UnityEngine;

public class AnimationLoader : MonoBehaviour
{
	public AnimationGroup AnimGroup;

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		LoadAnimations();
	}

	public string GetAnimName(AnimationGroup.AnimType type)
	{
		bool usedFallback = false;
		int index = -1;
		AnimationClip anim = GetAnim(type, out usedFallback, ref index, -1);
		if ((bool)anim)
		{
			return anim.name;
		}
		return null;
	}

	public string GetAnimName(AnimationGroup.AnimType type, int forceIndex)
	{
		bool usedFallback = false;
		int index = -1;
		AnimationClip anim = GetAnim(type, out usedFallback, ref index, forceIndex);
		if ((bool)anim)
		{
			return anim.name;
		}
		return null;
	}

	public string GetAnimName(AnimationGroup.AnimType type, out int chosenIndex)
	{
		bool usedFallback = false;
		chosenIndex = -1;
		AnimationClip anim = GetAnim(type, out usedFallback, ref chosenIndex, -1);
		if ((bool)anim)
		{
			return anim.name;
		}
		return null;
	}

	public bool HasSpecificAnim(AnimationGroup.AnimType type)
	{
		bool usedFallback = false;
		int index = -1;
		AnimationClip anim = GetAnim(type, out usedFallback, ref index, -1);
		if (anim != null && !usedFallback)
		{
			return true;
		}
		return false;
	}

	private AnimationClip GetAnim(AnimationGroup.AnimType type, out bool usedFallback, ref int index, int forceIndex)
	{
		AnimationClip animationClip = null;
		usedFallback = false;
		animationClip = AnimGroup.GetAnim(type, ref index, forceIndex);
		if (animationClip == null)
		{
			Debug.LogWarning(string.Concat("No valid animation found for type ", type, " on object ", base.gameObject.name));
		}
		return animationClip;
	}

	private void Init()
	{
		AnimGroup.Init();
	}

	private void LoadAnimations()
	{
		Animation component = GetComponent<Animation>();
		if ((bool)component)
		{
			AnimGroup.LoadAnimations(component);
		}
	}

	private void UnloadAnimations()
	{
		Animation component = GetComponent<Animation>();
		if ((bool)component)
		{
			AnimGroup.UnloadAnimations(component);
		}
	}
}
