using System;

[Serializable]
public class OverrideStack
{
	public string stackThis;

	public string onThat;

	[NonSerialized]
	public int stackThisIndex = -1;

	[NonSerialized]
	public int onThatIndex = -1;
}
