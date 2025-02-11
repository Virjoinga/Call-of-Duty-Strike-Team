using System.Collections;
using UnityEngine;

public class SpriteChar
{
	public int id;

	public Rect UVs;

	public float xOffset;

	public float yOffset;

	public float xAdvance;

	public Hashtable kernings;

	public Hashtable origKernings;

	public float GetKerning(int prevChar)
	{
		if (kernings == null)
		{
			return 0f;
		}
		float result = 0f;
		if (kernings.ContainsKey(prevChar))
		{
			result = (float)kernings[prevChar];
		}
		return result;
	}
}
