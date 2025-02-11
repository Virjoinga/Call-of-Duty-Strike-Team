using UnityEngine;

public static class ColorExtensions
{
	public static Color Alpha(this Color c, float alpha)
	{
		return new Color(c.r, c.g, c.b, alpha);
	}
}
