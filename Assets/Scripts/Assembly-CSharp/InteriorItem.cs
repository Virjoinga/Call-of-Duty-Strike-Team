using System;

[Serializable]
public class InteriorItem
{
	public GuidRef Interior;

	public HighlightHudCommand.ContextType ContextType;

	public int IndexToReference;

	public void ResolveGuidLinks()
	{
		Interior.ResolveLink();
	}
}
