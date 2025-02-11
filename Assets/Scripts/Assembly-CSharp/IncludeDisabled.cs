using System;
using System.Collections.Generic;
using UnityEngine;

public class IncludeDisabled
{
	public static UnityEngine.Object[] FindSceneObjectsOfType(Type type)
	{
		return FindSceneObjectsOfType(type, null);
	}

	public static UnityEngine.Object[] FindSceneObjectsOfType(Type type, GameObject parent)
	{
		List<UnityEngine.Object> list = new List<UnityEngine.Object>();
		return list.ToArray();
	}

	public static GameObject Find(string name)
	{
		return null;
	}

	public static GameObject[] FindGameObjectsWithTag(string tag)
	{
		List<GameObject> list = new List<GameObject>();
		return list.ToArray();
	}

	public static T GetComponentInChildren<T>(GameObject parent) where T : Component
	{
		T result = (T)null;
		Component[] componentsInChildren = parent.GetComponentsInChildren(typeof(T), true);
		if (componentsInChildren.Length > 0)
		{
			result = componentsInChildren[0] as T;
		}
		return result;
	}
}
