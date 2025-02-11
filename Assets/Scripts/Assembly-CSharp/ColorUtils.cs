using UnityEngine;

public static class ColorUtils
{
	public static Color ClampColor(Color _color)
	{
		float r = _color.r;
		r = Mathf.Max(r, _color.g);
		r = Mathf.Max(r, _color.b);
		if (r > 1f)
		{
			float num = 1f / r;
			_color.r *= num;
			_color.g *= num;
			_color.b *= num;
		}
		return _color;
	}
}
