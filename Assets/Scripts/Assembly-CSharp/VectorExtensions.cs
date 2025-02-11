using UnityEngine;

public static class VectorExtensions
{
	public static Vector2 xy(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector2 xz(this Vector3 v)
	{
		return new Vector2(v.x, v.z);
	}

	public static Vector3 xyz(this Vector4 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	public static Color Color(this Vector3 v)
	{
		return new Color(v.x, v.y, v.z);
	}
}
