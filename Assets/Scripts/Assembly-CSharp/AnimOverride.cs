using System;
using System.Collections.Generic;

[Serializable]
public class AnimOverride
{
	public string OverrideName;

	public List<AnimCategoryOverride> CategoryOverrides;

	public List<OverrideStack> OverrideStacks;

	private void Start()
	{
	}

	private void Update()
	{
	}
}
