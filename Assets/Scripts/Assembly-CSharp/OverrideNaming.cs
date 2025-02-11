using System;
using System.Collections.Generic;
using UnityEngine;

public class OverrideNaming : MonoBehaviour
{
	public enum IgnoreNames
	{
		_geom = 0,
		_seam = 1,
		_ceil = 2,
		_tact = 3,
		_col = 4,
		_dyncol = 5,
		_vol = 6
	}

	public List<IgnoreNames> IgnoreTags = new List<IgnoreNames> { IgnoreNames._geom };

	public bool IgnoreTag(string tag)
	{
		foreach (IgnoreNames ignoreTag in IgnoreTags)
		{
			if (string.Compare(ignoreTag.ToString(), tag, StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				return true;
			}
		}
		return false;
	}
}
