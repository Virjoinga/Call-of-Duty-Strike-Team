using System;
using System.Collections.Generic;

[Serializable]
public class AnimDefinition
{
	public string DefinitionName;

	public List<AnimCategory> Categories;

	public AnimDefinition()
	{
		DefinitionName = "Default";
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
