using System;
using System.Collections.Generic;

[Serializable]
public class AnimCategory
{
	public enum BlendingType
	{
		Override = 0,
		Blended = 1,
		Add = 2,
		Context = 3
	}

	public string CategoryName;

	public BlendingType BlendType;

	public string FromBone;

	public bool Multibone;

	public List<AnimAction> Actions;

	public AnimCategory(string Name)
	{
		CategoryName = Name;
	}
}
