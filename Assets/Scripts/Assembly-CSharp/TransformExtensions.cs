using System;
using UnityEngine;

public static class TransformExtensions
{
	public static void ParentAndZeroLocalPositionAndRotation(this Transform transform, Transform parent)
	{
		transform.parent = parent;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	public static void ZeroLocalPositionAndRotation(this Transform transform)
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
	}

	public static Transform FindInHierarchy(this Transform transform, string name)
	{
		if (transform.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
		{
			return transform;
		}
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform transform2 = transform.GetChild(i).FindInHierarchy(name);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}

	public static Transform FindInHierarchyStartsWith(this Transform transform, string name)
	{
		if (transform.name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
		{
			return transform;
		}
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform transform2 = transform.GetChild(i).FindInHierarchyStartsWith(name);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}
}
