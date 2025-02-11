using UnityEngine;

public static class GameObjectExtensions
{
	public static T RequireComponent<T>(this GameObject gameObject) where T : Component
	{
		T component = gameObject.GetComponent<T>();
		return (!((Object)component != (Object)null)) ? gameObject.AddComponent<T>() : component;
	}
}
