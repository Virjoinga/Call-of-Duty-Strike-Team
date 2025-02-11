using System;
using System.Collections.Generic;

[Serializable]
public class AnimCategoryOverride
{
	public List<AnimActionOverride> ActionOverrides;

	public AnimCategory DefaultCategory;

	public int CategoryIndex;

	public AnimCategoryOverride(AnimCategory Category)
	{
		DefaultCategory = Category;
		ActionOverrides = new List<AnimActionOverride>();
		foreach (AnimAction action in DefaultCategory.Actions)
		{
			AnimActionOverride item = new AnimActionOverride(action);
			ActionOverrides.Add(item);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
