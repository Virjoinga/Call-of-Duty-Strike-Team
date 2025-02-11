using System.Collections.Generic;
using UnityEngine;

public class AnimLibrary : MonoBehaviour
{
	public string LibraryName;

	public AnimDefinition AnimDefaults;

	public AnimationClip yRotAnim;

	public AnimationClip segueStartAnim;

	public AnimationClip segueEndAnim;

	public GameObject segueModel;

	public List<AnimOverride> AnimOverrides;

	public int totalAnimCount;

	protected AnimCategory AddDefaultCategory(string Name, AnimCategory.BlendingType Blend, string BlendBone)
	{
		AnimCategory animCategory = new AnimCategory(Name);
		animCategory.BlendType = Blend;
		animCategory.FromBone = BlendBone;
		AnimDefaults.Categories.Add(animCategory);
		return animCategory;
	}

	protected AnimAction AddDefaultAction(AnimCategory Category, string Name)
	{
		AnimAction animAction = new AnimAction(Name);
		Category.Actions.Add(animAction);
		return animAction;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
